using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class Player : GameObject
{
    // Movement
    private const float SPEED = 35f * SIZE_MOD;
    private Direction direction = Direction.Null;
    private SpriteEffects flip = SpriteEffects.None;
    private SpriteEffects lastFlip = SpriteEffects.None;

    // Gravity
    private const float JUMP_FORCE = 1.5f * SIZE_MOD; // jumping 3 blocks up
    private bool isGrounded = false;
    public float GravityVelocity { get; private set; } = DEFAULT_GRAVITY;

    // Inventory
    public Point cursorWorldPosition { get; private set; } = Point.Zero;
    public int HotbarSlot { get; private set; } = 0;
    public Inventory Inventory { get; private set; }
    public bool InInventory { get; private set; }
    public Item ItemInCursor { get; private set; }
    public int ItemInCursorAmount { get; private set; }

    // Combat
    private const int MAX_HEALTH = 16;
    private int health = MAX_HEALTH;
    private GameObject[] hearts = new GameObject[MAX_HEALTH];

    private Rectangle swordAttackRectangle
    {
        get
        {
            return new Rectangle
            (
                (int)(Position.X + EntitySize.X / 4 * (flip == SpriteEffects.None ? 1 : -1)),
                (int)Position.Y, (int)EntitySize.X, (int)EntitySize.Y
            );
        }
    }

    // Game
    private float deltaTime = 0f;
    private InventoryMenu inventoryMenu;

    // Hitbox
    private int hitboxXSpacing = 2 * SIZE_MOD;
    private Rectangle hitbox
    {
        get
        {
            return new Rectangle
            (
                (int)Position.X + hitboxXSpacing, (int)Position.Y,
                (int)Size.X - hitboxXSpacing * 2, (int)Size.Y
            );
        }
    }

    private Rectangle actionRectangle
    {
        get
        {
            var playerPosCentered = Position + Size / 2;
            var PlrActRadWorldSize = PlayerActionRadius.ToVector2() * TileSize;

            return new Rectangle
            (
                (int)(playerPosCentered.X - PlrActRadWorldSize.X),  
                (int)(playerPosCentered.Y - PlrActRadWorldSize.Y),
                (int)(PlrActRadWorldSize.X * 2),
                (int)(PlrActRadWorldSize.Y * 2)
            );
        }
    }

    // Sounds
    private SoundEffectInstance PlaceSFX;
    private SoundEffectInstance JumpSFX;
    private SoundEffectInstance CollectSFX;

    // Input
    private MouseState lastMouse;

    // Keybinds
    private readonly Keys key_MoveRight = Keys.D;
    private readonly Keys key_MoveLeft = Keys.A;
    private readonly Keys key_Jump = Keys.Space;
    private readonly Keys key_Inventory = Keys.E;
    private readonly Keys key_Back = Keys.Escape;

    // Animation
    private float timeToFrame = FRAME_TIME;

    private Rectangle toolRectangle
    {
        get
        {
            int xPos = flip == SpriteEffects.None
                ? (int)(Position.X + EntitySize.X - TileSize.X / 4)
                : (int)(Position.X + TileSize.X / 4);

            return new Rectangle
            (
                xPos, (int)(Position.Y + EntitySize.Y / 2),
                (int)TileSize.X, (int)TileSize.Y
            );
        }
    }
    private bool usingTools = false;
    private float toolRotation;
    private const float TOOL_ROTATION_SPEED = 7.5f;
    private readonly float minimalToolRotation = MathHelper.ToRadians(-90f);
    private readonly float maximalToolRotation = MathHelper.ToRadians(90f);

    // Frames
    private const int FRAME_IDLE_0 = 0;
    private const int FRAME_IDLE_1 = 1;
    private const int FRAME_RUN_0 = 2;
    private const int FRAME_RUN_1 = 3;
    private const int FRAME_ISNT_GROUNDED = 4;

    public Player(Atlas atlas, Vector2 position, Vector2 size, int defaultFrame, Atlas slotAtlas, Atlas heartAtlas)
    : base(atlas, position, size, defaultFrame)
    {
        // Creating inventory
        float lineXHotbar = (VIRTUAL_WIDTH - SlotSize.X * INV_SLOTS_IN_LINE -
        UI_SPACING * (INV_SLOTS_IN_LINE - 1)) / 2;

        Inventory = new Inventory
        (
            slotAtlas, hotbarSlotsPos: new Vector2(lineXHotbar, VIRTUAL_HEIGHT - SlotSize.Y
            - UI_SPACING), slotsPos: InventorySlotsPos, INV_SLOTS_LINES
        );

        Inventory.HotbarSlots[0].UpdateItem(ItemsBank.Sword);
        Inventory.HotbarSlots[1].UpdateItem(ItemsBank.Pickaxe);
        Inventory.HotbarSlots[2].UpdateItem(ItemsBank.Axe);

        // Creating hearts
        float xPos = VIRTUAL_WIDTH - HeartSize.X * HEARTS_IN_LINE - UI_SPACING * HEARTS_IN_LINE;
        float yPos = UI_SPACING;
        float xAdder = 0f;

        for (int i = 0; i < MAX_HEALTH; i++)
        {
            if (i != 0 && i % HEARTS_IN_LINE == 0)
            {
                yPos += HeartSize.Y + UI_SPACING;
                xAdder = 0f;
            }

            hearts[i] = new GameObject(heartAtlas, new Vector2(xPos + xAdder, yPos), HeartSize, 0);
            xAdder += HeartSize.X + UI_SPACING;
        }
    }

    public void SetupSFX(SoundEffect place, SoundEffect jump, SoundEffect collect)
    {
        PlaceSFX = place.CreateInstance();
        JumpSFX = jump.CreateInstance();
        CollectSFX = collect.CreateInstance();
    }

    public override void Update(GameTime gameTime)
    {
        deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        inventoryMenu = GameScene.Instance.InventoryMenu;

        for (int i = hearts.Length - 1; i >= 0; i--)
        {
            if (i >= health)
                hearts[i].Frame = 1;
            else
                hearts[i].Frame = 0;
        }

        if (inventoryMenu == InventoryMenu.Inventory)
            foreach (var item in Inventory.HotbarSlots)
                item.UpdateFrame();

        GetInput();
        Move();
        Gravity();
        Animate();
        CollectDroppedItems();
    }

    private void GetInput()
    {
        var keyboard = Keyboard.GetState();
        var mouse = Mouse.GetState();
        direction = Direction.Null;

        // Keyboard input

        if (keyboard.IsKeyDown(key_Inventory) && !InInventory)
            InInventory = true;
        else if (keyboard.IsKeyDown(key_Back) && InInventory)
            InInventory = false;

        if (keyboard.IsKeyDown(key_MoveRight) && !keyboard.IsKeyDown(key_MoveLeft) && !InInventory)
            direction = Direction.Right;
        else if (keyboard.IsKeyDown(key_MoveLeft) && !keyboard.IsKeyDown(key_MoveRight) && !InInventory)
            direction = Direction.Left;

        if (keyboard.IsKeyDown(key_Jump) && isGrounded && !InInventory)
        {
            JumpSFX.Play();
            isGrounded = false;
            GravityVelocity = -JUMP_FORCE;
        }

        // Hotbar selection via number keys
        if (keyboard.IsKeyDown(Keys.D1)) HotbarSlot = 0;
        else if (keyboard.IsKeyDown(Keys.D2)) HotbarSlot = 1;
        else if (keyboard.IsKeyDown(Keys.D3)) HotbarSlot = 2;
        else if (keyboard.IsKeyDown(Keys.D4)) HotbarSlot = 3;
        else if (keyboard.IsKeyDown(Keys.D5)) HotbarSlot = 4;

        // Mouse Input

        var normalizedCursorPos = mouse.NormalizeForWindow();

        cursorWorldPosition = new Point
        (
            (int)((normalizedCursorPos.X + GameScene.Instance.Camera.Position.X) / TileSize.X),
            (int)((normalizedCursorPos.Y + GameScene.Instance.Camera.Position.Y) / TileSize.Y)
        );

        if (mouse.ScrollWheelValue < lastMouse.ScrollWheelValue)
        {
            HotbarSlot++;
            if (HotbarSlot >= Inventory.HotbarSlots.Length)
                HotbarSlot = 0;
        }
        else if (mouse.ScrollWheelValue > lastMouse.ScrollWheelValue)
        {
            HotbarSlot--;
            if (HotbarSlot < 0)
                HotbarSlot = Inventory.HotbarSlots.Length - 1;
        }

        if (mouse.LeftButton == ButtonState.Pressed && !InInventory)
        {
            var item = Inventory.HotbarSlots[HotbarSlot].Item;

            var targetPosition = new Vector2
            (
                cursorWorldPosition.X * TileSize.X,
                cursorWorldPosition.Y * TileSize.Y
            );

            var targetRectangle = new Rectangle
            (
                (int)(normalizedCursorPos.X + GameScene.Instance.Camera.Position.X),
                (int)(normalizedCursorPos.Y + GameScene.Instance.Camera.Position.Y),
                1, 1
            );

            if (item != null && targetRectangle.Intersects(actionRectangle))
                if (item is PlaceableItem tile)
                {
                    bool canAddTile = World.AddTile(TilesBank.FindTile(tile.ItemTile, targetPosition));
                    if (canAddTile)
                    {
                        Inventory.HotbarSlots[HotbarSlot].CountItem(countBack: true);
                        PlaceSFX.Play();
                    }

                    if (Inventory.HotbarSlots[HotbarSlot].ItemAmount <= 0)
                        Inventory.HotbarSlots[HotbarSlot].UpdateItem(null);
                }
                else if (item.IsTool)
                {
                    usingTools = true;

                    if (item is SwordItem sword)
                    {
                        foreach (var mob in World.Mobs)
                            if (mob.HitboxPosApplied.Intersects(swordAttackRectangle))
                                mob.TakeDamage(sword.Damage);
                    }
                    else
                    {
                        var _tile = World.SearchTile(targetPosition);
                        ToolToDestroy? tool = _tile?.ToolToDestroy;
                        Item itemInSlot = Inventory.HotbarSlots[HotbarSlot]?.Item;

                        if (tool != null && itemInSlot != null)
                            if (tool == ToolToDestroy.Pickaxe && itemInSlot is PickaxeItem pickaxe)
                                _tile.TakeDamage(pickaxe.Speed, pickaxe.Power);
                            else if (tool == ToolToDestroy.Axe && itemInSlot is AxeItem axe)
                                _tile.TakeDamage(axe.Speed, axe.Power);
                            else if (tool == ToolToDestroy.Both)
                                if (itemInSlot is AxeItem _axe)
                                    _tile.TakeDamage(_axe.Speed, _axe.Power);
                                else if (itemInSlot is PickaxeItem _pickaxe)
                                    _tile.TakeDamage(_pickaxe.Speed, _pickaxe.Power);
                    }

                }
        }

        if (mouse.LeftButton == ButtonState.Released)
            usingTools = false;

        if (mouse.RightButton == ButtonState.Pressed && lastMouse.RightButton == ButtonState.Released && !InInventory)
        {
            Vector2 targetPosition = new Vector2
            (
                cursorWorldPosition.X * TileSize.X,
                cursorWorldPosition.Y * TileSize.Y
            );

            var tile = World.SearchTile(targetPosition);

            if (tile is DoorTile door)
                door.Toggle();
            else if (tile is SignTile sign)
                sign.Writing = true;
        }

        if (mouse.LeftButton == ButtonState.Pressed && lastMouse.LeftButton != ButtonState.Pressed && InInventory &&
            inventoryMenu == InventoryMenu.Inventory)
        {
            var cursorRectangle = new Rectangle(normalizedCursorPos.X, normalizedCursorPos.Y, 1, 1);

            if (ItemInCursor == null)
            {
                foreach (var slot in Inventory.HotbarSlots)
                if (slot.Rectangle.Intersects(cursorRectangle))
                {
                    TakeItemFromSlot(slot);
                    break;
                }

            foreach (var slot in Inventory.Slots)
                if (slot.Rectangle.Intersects(cursorRectangle))
                {
                    TakeItemFromSlot(slot);
                    break;
                }

            foreach (var slot in Inventory.ArmorSlots)
                if (slot.Rectangle.Intersects(cursorRectangle))
                {
                    TakeItemFromSlot(slot);
                    break;
                }
            }
            else
            {
                foreach (var slot in Inventory.HotbarSlots)
                    if (slot.Rectangle.Intersects(cursorRectangle))
                    {
                        PutItemToSlot(slot);
                        break;
                    }

                foreach (var slot in Inventory.Slots)
                    if (slot.Rectangle.Intersects(cursorRectangle))
                    {
                        PutItemToSlot(slot);
                        break;
                    }

                foreach (var slot in Inventory.ArmorSlots)
                    if (slot.Rectangle.Intersects(cursorRectangle))
                    {
                        PutItemToSlot(slot);
                        break;
                    }

                if (Inventory.DeleteSlot.Rectangle.Intersects(cursorRectangle))
                {
                    ItemInCursor = null;
                    ItemInCursorAmount = 0;
                }
            }
        }

        lastMouse = mouse;
    }

    private void TakeItemFromSlot(Slot slot)
    {
        if (slot.Item == null)
            return;
        else
        {
            ItemInCursor = slot.Item;
            ItemInCursorAmount = slot.ItemAmount;
            slot.UpdateItem(null, 0);
        }
    }

    private void TakeItemFromSlot(ArmorSlot slot)
    {
        if (slot.Item == null)
            return;
        else if (ItemInCursor != null && ItemInCursor is ArmorElementItem armorElement)
        {
            var slotItem = slot.Item;

            if (armorElement.Element == slot.TargetElement)
            {
                slot.UpdateItem(armorElement);

                ItemInCursor = slotItem;
                ItemInCursorAmount = 1;
            }
        }
        else if (ItemInCursor == null || ItemInCursor is ArmorElementItem)
        {
            ItemInCursor = slot.Item;
            ItemInCursorAmount = 1;
            slot.UpdateItem(null);
        }
    }

    private void PutItemToSlot(Slot slot)
    {
        if (ItemInCursor != null)
        {
            var item = slot.Item;
            var itemAmount = slot.ItemAmount;

            if (item == null)
            {
                slot.UpdateItem(ItemInCursor, ItemInCursorAmount);
                ItemInCursor = null;
                ItemInCursorAmount = 0;
            }
            else if (item.ID == ItemInCursor.ID && itemAmount < item.MaxStack)
            {
                int fixedItemMaxStackMinusItemAmount = item.MaxStack - itemAmount;
                int fixedItemInCursorAmount = ItemInCursorAmount;

                if ((itemAmount + ItemInCursorAmount) >= item.MaxStack)
                    for (int i = 0; i < fixedItemMaxStackMinusItemAmount; i++)
                        PutItemToSlotOneTime(slot);
                else
                    for (int i = 0; i < fixedItemInCursorAmount; i++)
                        PutItemToSlotOneTime(slot);
            }
            else
            {
                var slotItem = slot.Item;
                int slotItemAmount = slot.ItemAmount;

                slot.UpdateItem(ItemInCursor, ItemInCursorAmount);

                ItemInCursor = slotItem;
                ItemInCursorAmount = slotItemAmount;
            }
        }
    }

    private void PutItemToSlot(ArmorSlot slot)
    {
        if (ItemInCursor != null && ItemInCursor is ArmorElementItem armorElement)
        {
            var item = slot.Item;

            if (item == null && armorElement.Element == slot.TargetElement)
            {
                slot.UpdateItem(armorElement);
                ItemInCursor = null;
                ItemInCursorAmount = 0;
            }
            else
            {
                var slotItem = slot.Item;

                slot.UpdateItem(ItemInCursor as ArmorElementItem);

                ItemInCursor = slotItem;
                ItemInCursorAmount = 0;
            }
        }
    }

    private void PutItemToSlotOneTime(Slot slot)
    {
        slot.CountItem();
        ItemInCursorAmount--;
        if (ItemInCursorAmount <= 0)
        {
            ItemInCursor = null;
            ItemInCursorAmount = 0;
        }
    }

    private void Move()
    {
        if (direction == Direction.Null)
            return;

        int moveMod = direction switch
        {
            Direction.Right => 1,  
            Direction.Left => -1,  
            _ => 0,
        };

        float velocity = SPEED * deltaTime * moveMod;
        var nextHitbox = new Rectangle((int)(hitbox.X + velocity), hitbox.Y, hitbox.Width, hitbox.Height);

        foreach (var tile in World.Tiles)
        {
            if (nextHitbox.Intersects(tile.Hitbox))
            {
                isGrounded = true;
                Position = new Vector2(moveMod > 0
                    ? tile.Hitbox.Left - Rectangle.Width + hitboxXSpacing
                    : tile.Hitbox.Right - hitboxXSpacing, Position.Y);
                velocity = 0f;
                break;
            }
        }

        Position += new Vector2(velocity, 0f);
    }

    private void Gravity()
    {
        GravityVelocity += GRAVITY_ACELERATION * deltaTime;

        if (GravityVelocity > MAXIMAL_GRAVITY)
            GravityVelocity = MAXIMAL_GRAVITY;

        isGrounded = false;
        var nextHitbox = new Rectangle(hitbox.X, (int)(hitbox.Y + GravityVelocity), hitbox.Width, hitbox.Height);

        foreach (var tile in World.Tiles)
        {
            if (nextHitbox.Intersects(tile.Hitbox))
            {
                if (GravityVelocity > 0)
                {
                    isGrounded = true;
                    GravityVelocity = DEFAULT_GRAVITY;
                    Position = new Vector2(Position.X, tile.Rectangle.Top - Rectangle.Height);
                }
                else
                {
                    Position = new Vector2(Position.X, tile.Rectangle.Bottom);
                    GravityVelocity = DEFAULT_GRAVITY;
                }

                break;
            }
        }

        if (isGrounded)
            return;
        
        Position += new Vector2(0f, GravityVelocity);
    }

    private void Animate()
    {
        if (direction == Direction.Right)
            flip = SpriteEffects.None;
        else if (direction == Direction.Left)
            flip = SpriteEffects.FlipHorizontally;

        if (usingTools)
        {
            int flipMod = flip == SpriteEffects.None ? 1 : -1;

            if (lastFlip != flip)
                toolRotation = minimalToolRotation;

            toolRotation += TOOL_ROTATION_SPEED * deltaTime * flipMod;

            if ((toolRotation > maximalToolRotation * flipMod && flipMod == 1) ||
                (toolRotation < maximalToolRotation * flipMod && flipMod == -1))
                toolRotation = minimalToolRotation * flipMod;
        }
        else
            toolRotation = minimalToolRotation;

        if (!isGrounded)
        {
            Frame = FRAME_ISNT_GROUNDED;
            timeToFrame = 0f;
            return;
        }

        if ((timeToFrame -= deltaTime) <= 0f)
        {
            if (direction == Direction.Null)
            {
                Frame = Frame switch
                {
                    FRAME_IDLE_0 => FRAME_IDLE_1,
                    _ => FRAME_IDLE_0  
                };
            }
            else
            {
                Frame = Frame switch
                {
                    FRAME_RUN_0 => FRAME_RUN_1,
                    _ => FRAME_RUN_0  
                };
            }

            timeToFrame = FRAME_TIME;
        }

        lastFlip = flip;
    }

    private void CollectDroppedItems()
    {
        var itemsToRemove = new List<DroppedItem>();

        foreach (var item in World.Items)
            if (hitbox.Intersects(item.Rectangle))
            {
                bool collected = Inventory.TryCollectItem(item.Item);
                if (collected)
                {
                    CollectSFX.Play();
                    itemsToRemove.Add(item);
                }
            }

        foreach (var item in itemsToRemove)
            World.RemoveItem(item);
    }

    public void Craft(CraftData craftData)
    {
        var slotsWithMaterials = new List<Slot>();
        var materialsAmounts = new List<int>();

        foreach (var material in craftData.Materials.Keys)
        {
            int materialAmount = craftData.Materials.GetValueOrDefault(material);
            var slotWithMaterial = Inventory.SearchItemSlot(material, materialAmount);
            if (slotWithMaterial != null)
            {
                slotsWithMaterials.Add(slotWithMaterial);
                materialsAmounts.Add(materialAmount);
            }
            else
                return;
        }

        if (slotsWithMaterials.Count < craftData.Materials.Count)
            return;

        for (int i = 0; i < slotsWithMaterials.Count; i++)
        {
            var slot = slotsWithMaterials[i];
            slotsWithMaterials[i].UpdateItem(slot.ItemAmount - materialsAmounts[i]);
        }

        for (int i = 0; i < craftData.ItemAmount; i++)
            World.AddItem(new DroppedItem(craftData.Item, Position + EntitySize / 2));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw
        (
            atlas.Texture, Rectangle, atlas.Rectangles[Frame],
            Color.White, 0f, Vector2.Zero, flip, 0f
        );

        foreach (var slot in Inventory.ArmorSlots)
        {
            if (slot.Item != null)
                spriteBatch.Draw
                (
                    slot.Item.ArmorAtlas.Texture, Rectangle, slot.Item.ArmorAtlas.Rectangles[Frame],
                    Color.White, 0f, Vector2.Zero, flip, 0f
                );
        }

        var _slot = Inventory.HotbarSlots[HotbarSlot];

        if (usingTools && _slot.Item.IsTool)
        {
            var texture = _slot.Item.Texture;
            var origin = new Vector2(texture.Width / 2, texture.Height / 2);
            spriteBatch.Draw
            (
                texture, toolRectangle, null, Color.White,
                toolRotation, origin, flip, 0f
            );
        }
    }

    public void DrawHearts(SpriteBatch spriteBatch)
    {
        foreach (var heart in hearts)
            heart.Draw(spriteBatch);
    }
}
