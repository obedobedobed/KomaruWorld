using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class GameScene(ContentManager content, SpriteBatch spriteBatch, GraphicsDeviceManager graphicsManager)
: Scene(content, spriteBatch, graphicsManager)
{
    public static GameScene Instance;
    public OrthographicCamera Camera { get; private set; } = new OrthographicCamera(graphicsManager.GraphicsDevice);
    private Vector2 maximalCameraPos = new Vector2
    (
        VerySmallWorldSize.X * TileSize.X - VIRTUAL_WIDTH,
        VerySmallWorldSize.Y * TileSize.Y - VIRTUAL_HEIGHT
    );
    private Vector2 minimalCameraPos = new Vector2(0f, 0f);
    public Player Player { get; private set; }
    private KeyboardState lastKeyboard;
    private Texture2D pixel;
    private TextButton respawnButton;

    private SpriteButton inventoryMenuButton;
    private CraftMenu craftMenu;

    // Debug
    private bool debugMenuOpened = false;

    // Inventory
    public InventoryMenu InventoryMenu { get; private set;  }
    public bool Crafting { get; private set; }

    public override void Load()
    {
        Instance = this;
        pixel = Content.Load<Texture2D>("Sprites/Pixel");

        WorldGenerator.Generate(VerySmallWorldSize.X, VerySmallWorldSize.Y);

        // Player
        var playerAtlas = new Atlas(texture: Content.Load<Texture2D>("Sprites/KomaruAtlas"), spriteSize: EntitySize / SIZE_MOD);
        var slotAtlas = new Atlas(texture: Content.Load<Texture2D>("Sprites/UI/SlotAtlas"), spriteSize: SlotSize / SIZE_MOD);
        var heartAtlas = new Atlas(texture: Content.Load<Texture2D>("Sprites/UI/HeartAtlas"), spriteSize: HeartSize / SIZE_MOD);
        var placeSfx = Content.Load<SoundEffect>("Audio/SFX/TilePlace");
        var jumpSfx = Content.Load<SoundEffect>("Audio/SFX/PlayerJump");
        var collectSfx = Content.Load<SoundEffect>("Audio/SFX/PlayerCollect");
        Player = new Player(playerAtlas, new Vector2(VerySmallWorldSize.X * TileSize.X / 2, 100), EntitySize,
        defaultFrame: 1, slotAtlas: slotAtlas, heartAtlas: heartAtlas);
        Player.SetupSFX(placeSfx, jumpSfx, collectSfx);

        // Door
        var doorSFX = Content.Load<SoundEffect>("Audio/SFX/Tiles/Door");
        DoorTile.SetupSFX(doorSFX);

        // Craft
        var inventoryMenuAtlas = new Atlas
        (texture: Content.Load<Texture2D>("Sprites/UI/InventoryMenuAtlas"), SlotSize / SIZE_MOD);
        inventoryMenuButton = new SpriteButton(inventoryMenuAtlas, new Vector2 (UI_SPACING, UI_SPACING),
        SlotSize, 0, 1, action: CraftSwitchCall);

        var arrowsAtlas = new Atlas(texture: Content.Load<Texture2D>("Sprites/UI/Arrows"),
        spriteSize: SlotSize / SIZE_MOD);
        CraftsBank.CreateCraftSlots(slotAtlas, arrowsAtlas, InventorySlotsPos, OpenCraftMenuCall);

        var craftMenuSprite = Content.Load<Texture2D>("Sprites/UI/CraftMenu");
        var closeButtonAtlas = new Atlas(Content.Load<Texture2D>("Sprites/UI/CloseButtonAtlas"), SlotSize / SIZE_MOD);
        craftMenu = new CraftMenu(craftMenuSprite, new Vector2
        (VIRTUAL_WIDTH / 2 - CraftMenuSize.X / 2,
        VIRTUAL_HEIGHT / 2 - CraftMenuSize.Y / 2),
        CraftMenuSize, CallPlayerCraft, closeButtonAtlas);

        // UI
        respawnButton = new TextButton("Respawn", new Vector2(VIRTUAL_WIDTH / 2, VIRTUAL_HEIGHT / 2), Color.White,
        Color.Yellow, RespawnPlayer);

        // Background
        var cloudsAtlas = new Atlas(Content.Load<Texture2D>("Sprites/CloudsAtlas"), CloudSize / BG_MOD);

        Background.Load(cloudsAtlas);

        // Camera
        Camera.Position = Player.Position;

        Logger.Log("Game scene loaded");
    }

    public override void Update(GameTime gameTime)
    {
        inventoryMenuButton.Update(gameTime);
        World.Update(gameTime);
        MobsSpawner.Update(gameTime);
        Tile.StaticUpdate(gameTime);
        Background.Update(gameTime);

        if (Player.IsDead)
            respawnButton.Update(gameTime);

        var keyboard = Keyboard.GetState();

        // Functions
        if (keyboard.IsKeyDown(Keys.F1) && !lastKeyboard.IsKeyDown(Keys.F1))
            debugMenuOpened = !debugMenuOpened;

        if (keyboard.IsKeyDown(Keys.F2) && !lastKeyboard.IsKeyDown(Keys.F2))
        {
            GraphicsManager.SynchronizeWithVerticalRetrace = !GraphicsManager.SynchronizeWithVerticalRetrace;
            GraphicsManager.ApplyChanges();
        }

        lastKeyboard = keyboard;

        if (!SignTile.BlockedInput)
            Player.Update(gameTime);

        if (Crafting)
            craftMenu.Update(gameTime);

        if (InventoryMenu == InventoryMenu.Craft)
            CraftsBank.UpdateCraftSlots();

        var playerPosCentered = Player.Position - new Vector2
        (VIRTUAL_WIDTH / 2, VIRTUAL_HEIGHT / 2) +
        Player.Size / 2;
        Camera.Position = Vector2.Lerp(Camera.Position, playerPosCentered, 0.1f);

        // Checking camera position
        if (Camera.Position.X < minimalCameraPos.X)
            Camera.Position = new Vector2(minimalCameraPos.X, Camera.Position.Y);
        else if (Camera.Position.X > maximalCameraPos.X)
            Camera.Position = new Vector2(maximalCameraPos.X, Camera.Position.Y);

        if (Camera.Position.Y < minimalCameraPos.Y)
            Camera.Position = new Vector2(Camera.Position.X, minimalCameraPos.Y);
        else if (Camera.Position.Y > maximalCameraPos.Y)
            Camera.Position = new Vector2(Camera.Position.X, maximalCameraPos.Y);

        foreach (var _object in Objects)
            _object.Update(gameTime);

        
    }

    public override void Draw()
    {
        var view = Camera.GetViewMatrix();

        // Background
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Background.Camera.GetViewMatrix());
        Background.Draw(SpriteBatch);
        SpriteBatch.End();

        // Game world (applying transform matrix)
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: view);

        World.Draw(SpriteBatch);
        Player.Draw(SpriteBatch);

        foreach (var _object in Objects)
            _object.Draw(SpriteBatch);

        SpriteBatch.End(); 

        // UI (not applying transform matrix)
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        if (Player.InInventory && !Player.IsDead)
        {
            SpriteBatch.Draw(pixel, new Rectangle(0, 0, VIRTUAL_WIDTH,
            VIRTUAL_HEIGHT), new Color(0, 0, 0, 150));
            string title = string.Empty;
            if (InventoryMenu == InventoryMenu.Inventory)
            {
                title = "Inventory";
                Player.Inventory.DrawInventory(SpriteBatch); 
            }
            else if (InventoryMenu == InventoryMenu.Craft)
            {
                title = "Craft";
                CraftsBank.DrawCraftSlots(SpriteBatch);
                if (Crafting)
                {
                    SpriteBatch.Draw(pixel, new Rectangle(0, 0, VIRTUAL_WIDTH,
                    VIRTUAL_HEIGHT), new Color(0, 0, 0, 150));
                    craftMenu.Draw(SpriteBatch);
                }
            }
            Text.Draw(title, new Vector2(VIRTUAL_WIDTH / 2, UI_SPACING), Color.White,
            SpriteBatch, TextDrawingMode.Center);
            inventoryMenuButton.Draw(SpriteBatch);
        }
        else if (InventoryMenu == InventoryMenu.Craft)
        {
            CraftSwitch();
            Crafting = false;
        }

        Player.Inventory.DrawHotbar(SpriteBatch);
        Player.DrawHearts(SpriteBatch);

        var mouse = Mouse.GetState();
        var normalizedCursorPos = mouse.NormalizeForWindow();
        var cursorRectangle = new Rectangle(normalizedCursorPos.X, normalizedCursorPos.Y, 1, 1);

        bool drawDescription = false;
        Slot slotToDescription = null;
        bool hotbarSlot = false;
        ArmorSlot armorSlotToDescription = null;

        if (Player.InInventory && InventoryMenu == InventoryMenu.Inventory && !Player.IsDead)
        {
            foreach (var slot in Player.Inventory.Slots)
                if (slot.Rectangle.Intersects(cursorRectangle) && slot.Item != null)
                {
                    drawDescription = true; 
                    slotToDescription = slot;
                    break;
                }

            foreach (var slot in Player.Inventory.HotbarSlots)
                if (slot.Rectangle.Intersects(cursorRectangle) && slot.Item != null)
                {
                    drawDescription = true;
                    slotToDescription = slot;
                    hotbarSlot = true;
                    break;
                }

            foreach (var slot in Player.Inventory.ArmorSlots)
                if (slot.Rectangle.Intersects(cursorRectangle) && slot.Item != null)
                {
                    drawDescription = true;
                    armorSlotToDescription = slot;
                    break;
                }
        }

        if (!Player.InInventory && !Player.IsDead)
        {
            var cursorWorldRectangle = new Rectangle
            (
                cursorRectangle.X + (int)Camera.Position.X,  
                cursorRectangle.Y + (int)Camera.Position.Y,  
                1, 1  
            );

            foreach (var mob in World.Mobs)
                if (mob.Rectangle.Intersects(cursorWorldRectangle))
                {
                    DrawMobDescription(mob);
                    break;
                }

            foreach (var tile in World.Tiles)
                if (tile is SignTile sign && sign.Rectangle.Intersects(cursorWorldRectangle))
                {
                    DrawSignText(sign);
                    break;
                }
        }

        string slotItemName = Player.Inventory.HotbarSlots[Player.HotbarSlot].Item?.Name;
        var itemNamePos = new Vector2
        (
            Player.Inventory.HotbarSlots[Player.HotbarSlot].Position.X + SlotSize.X / 2,
            Player.Inventory.HotbarSlots[0].Position.Y - GlyphSize.Y - UI_SPACING
        );
        if (slotItemName != null)
            Text.Draw(slotItemName, itemNamePos, Color.White, SpriteBatch, TextDrawingMode.Center);

        var itemInCursor = Player.ItemInCursor;
        int itemInCursorAmount = Player.ItemInCursorAmount;
        if (itemInCursor != null)
        {
            string itemAmountString = itemInCursorAmount > 1 ? itemInCursorAmount.ToString() : string.Empty;

            SpriteBatch.Draw(itemInCursor.Texture, new Rectangle(normalizedCursorPos + CursorSize, ItemSize.ToPoint()),
            Color.White); Text.Draw(itemAmountString, (normalizedCursorPos + CursorSize + ItemSize.ToPoint()).ToVector2(),
            Color.White, SpriteBatch, TextDrawingMode.Center);
        }

        if (drawDescription && !Player.IsDead)
        {
            if (slotToDescription != null)
                DrawItemDescription(slotToDescription, hotbarSlot);
            else if (armorSlotToDescription != null)
                DrawItemDescription(armorSlotToDescription);
        }
        
        if (SignTile.BlockedInput)
            foreach (var tile in World.Tiles)
                if (tile is SignTile sign && sign.Writing && !Player.IsDead)
                    sign.DrawInputMenu(SpriteBatch, pixel);

        if (Player.IsDead)
        {
            SpriteBatch.Draw(pixel, new Rectangle(0, 0, VIRTUAL_WIDTH, VIRTUAL_HEIGHT), new Color(87, 7, 12, 35));
            Text.Draw("You are dead...", new Vector2(VIRTUAL_WIDTH / 2, 100), Color.White, SpriteBatch,
            TextDrawingMode.Center, true, Color.Black);
            respawnButton.Draw(SpriteBatch);
        }

        if (debugMenuOpened)
            DrawDebugMenu();
            
        SpriteBatch.End();
    }

    private void DrawItemDescription(Slot slot, bool hotbarSlot = false)
    {
        var mouse = Mouse.GetState();
        var normalizedCursorPos = mouse.NormalizeForWindow();
        var descriptionPos = hotbarSlot 
            ? new Vector2(normalizedCursorPos.X + CursorSize.X, normalizedCursorPos.Y)
            : new Vector2(normalizedCursorPos.X + CursorSize.X, normalizedCursorPos.Y + CursorSize.Y);
        List<string> description = new List<string>() { slot.Item.Name + $" (x{slot.ItemAmount})" };

        if (slot.Item is PlaceableItem)
            description.Add("Placeable");
        else if (slot.Item.IsTool)
        {
            bool isSword = false;
            string damage = "[Unknown parameter]";
            string power = "[Unknown parameter]";

            if (slot.Item is SwordItem sword)
            {
                damage = $"Damage: {sword.Damage}";
                isSword = true;
            }
            else if (slot.Item is PickaxeItem pickaxe)
            {
                damage = $"Speed: {pickaxe.Speed}";
                power = $"Power: {pickaxe.Power}";
            }
            else if (slot.Item is AxeItem axe)
            {
                damage = $"Speed: {axe.Speed}";
                power = $"Power: {axe.Power}";
            }

            if (!isSword)
                description.Add(power);
            description.Add(damage);
            description.Add("Tool");
        }
        else if (slot.Item is ArmorElementItem armor)
        {
            string armorElement = armor.Element switch
            {
                ArmorElement.Helmet => "Helmet",
                ArmorElement.Chestplate => "Chestplate",
                ArmorElement.Leggins => "Leggins",
                _ => "[Unknown parameter]"
            };

            description.Add(armorElement);
            description.Add($"Armor: {armor.Armor}");
        }

        if (hotbarSlot)
        {
            descriptionPos.Y += (GlyphSize.Y + TEXT_SPACING) * -1;
            description.Reverse();
        }

        for (int i = 0; i < description.Count; i++)
        {
            Text.Draw(description[i], descriptionPos, Color.White, SpriteBatch, TextDrawingMode.Right,
            outline: true, outlineColor: Color.Black);
            descriptionPos.Y += (GlyphSize.Y + TEXT_SPACING) * (hotbarSlot ? -1 : 1);
        }
    }

    private void DrawItemDescription(ArmorSlot slot)
    {
        var mouse = Mouse.GetState();
        var normalizedCursorPos = mouse.NormalizeForWindow();
        var descriptionPos = new Vector2(normalizedCursorPos.X + CursorSize.X, normalizedCursorPos.Y
        + CursorSize.Y + GlyphSize.Y);
        List<string> description = new List<string>() { slot.Item.Name + $" (x1)" };

        string armorElement = slot.Item.Element switch
        {
            ArmorElement.Helmet => "Helmet",
            ArmorElement.Chestplate => "Chestplate",
            ArmorElement.Leggins => "Leggins",
            _ => "[Unknown parameter]"
        };

        description.Add(armorElement);
        description.Add($"Armor: {slot.Item.Armor}");

        for (int i = 0; i < description.Count; i++)
        {
            Text.Draw(description[i], descriptionPos, Color.White, SpriteBatch, TextDrawingMode.Right,
            outline: true, outlineColor: Color.Black);
            descriptionPos.Y += GlyphSize.Y + TEXT_SPACING;
        }
    }

    private void DrawMobDescription(Mob mob)
    {
        var mouse = Mouse.GetState();
        var normalizedCursorPos = mouse.NormalizeForWindow();
        var descriptionPos = new Vector2(normalizedCursorPos.X + CursorSize.X, normalizedCursorPos.Y
        + CursorSize.Y + GlyphSize.Y);
        List<string> description = new List<string>()
        {
            mob.Name,
            $"Health: {mob.Health}/{mob.MaxHealth}",
            $"Pos: x{(int)(mob.Position.X / TileSize.X / SIZE_MOD)} y{(int)(mob.Position.Y / TileSize.Y / SIZE_MOD)}"
        };

        for (int i = 0; i < description.Count; i++)
        {
            Text.Draw(description[i], descriptionPos, Color.White, SpriteBatch, TextDrawingMode.Right,
            outline: true, outlineColor: Color.Black);
            descriptionPos.Y += GlyphSize.Y + TEXT_SPACING;
        }
    }

    private void DrawSignText(SignTile sign)
    {
        var mouse = Mouse.GetState();
        var normalizedCursorPos = mouse.NormalizeForWindow();
        var textPos = new Vector2(normalizedCursorPos.X + CursorSize.X, normalizedCursorPos.Y
        + CursorSize.Y);
        Text.Draw(sign.SignText, textPos, Color.White, SpriteBatch, TextDrawingMode.Right,
        outline: true, outlineColor: Color.Black);
    }

    private void DrawDebugMenu()
    {
        int slot = Player.HotbarSlot;
        var slotItem = Player.Inventory?.HotbarSlots[slot];
        string hotbarSlotString =  (slotItem?.Item != null)
            ? $"{slot + 1} - {slotItem.Item.Name} (x{slotItem.ItemAmount})"
            : $"{slot + 1} - Air (x0)";

        string vSyncString = GraphicsManager.SynchronizeWithVerticalRetrace
            ? "(VSync, press F2 to disable)"
            : "(Non-VSync, press F2 to enable)";

        Text.Draw($"{GAME_NAME} - v{GAME_VERSION}",
        new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 0 + UI_SPACING - GlyphSize.Y / 2 * 0),
        Color.White, SpriteBatch, TextDrawingMode.Right, outline: true, outlineColor: Color.Black);
        Text.Draw($"FPS:{Game1.Instance.FPS} {vSyncString}",
        new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 1 + UI_SPACING - GlyphSize.Y / 2 * 1),
        Color.White, SpriteBatch, TextDrawingMode.Right, outline: true, outlineColor: Color.Black);
        Text.Draw($"Position: x{(int)Player.Position.X}, y{(int)Player.Position.Y}",
        new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 2 + UI_SPACING - GlyphSize.Y / 2 * 2),
        Color.White, SpriteBatch, TextDrawingMode.Right, outline: true, outlineColor: Color.Black);
        Text.Draw($"Gravity: {(int)Player.GravityVelocity}",
        new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 3 + UI_SPACING - GlyphSize.Y / 2 * 3),
        Color.White, SpriteBatch, TextDrawingMode.Right, outline: true, outlineColor: Color.Black);
        Text.Draw($"Hotbar slot: {hotbarSlotString}",
        new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 4 + UI_SPACING - GlyphSize.Y / 2 * 4),
        Color.White, SpriteBatch, TextDrawingMode.Right, outline: true, outlineColor: Color.Black);
        Text.Draw($"Cursor: x{Player.cursorWorldPosition.X}, y{Player.cursorWorldPosition.Y}",
        new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 5 + UI_SPACING - GlyphSize.Y / 2 * 5),
        Color.White, SpriteBatch, TextDrawingMode.Right, outline: true, outlineColor: Color.Black);

        long usedMemory = GC.GetTotalMemory(false) / 1024;
        long totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024 / 1024;
        var windowSize = Game1.Instance.Window.ClientBounds.Size;
        string windowMode = GraphicsManager.IsFullScreen ? "Fullscreen" : "Windowed";

        Text.Draw($"OS: {RuntimeInformation.OSDescription}",
        new Vector2(UI_SPACING, VIRTUAL_HEIGHT - GlyphSize.Y * TEXT_SPACING * 5 + GlyphSize.Y / 2 * 5),
        Color.White, SpriteBatch, TextDrawingMode.Right, outline: true, outlineColor: Color.Black);
        Text.Draw($"Screen: {windowSize.X}x{windowSize.Y} ({windowMode}, F11 for toggle)",
        new Vector2(UI_SPACING, VIRTUAL_HEIGHT - GlyphSize.Y * TEXT_SPACING * 4 + GlyphSize.Y / 2 * 4),
        Color.White, SpriteBatch, TextDrawingMode.Right, outline: true, outlineColor: Color.Black);
        Text.Draw($"CPU: {Environment.ProcessorCount} threads {RuntimeInformation.ProcessArchitecture} CPU",
        new Vector2(UI_SPACING, VIRTUAL_HEIGHT - GlyphSize.Y * TEXT_SPACING * 3 + GlyphSize.Y / 2 * 3),
        Color.White, SpriteBatch, TextDrawingMode.Right, outline: true, outlineColor: Color.Black);
        Text.Draw($"Memory: {usedMemory}MB/{totalMemory}MB used",
        new Vector2(UI_SPACING, VIRTUAL_HEIGHT - GlyphSize.Y * TEXT_SPACING * 2 + GlyphSize.Y / 2 * 2),
        Color.White, SpriteBatch, TextDrawingMode.Right, outline: true, outlineColor: Color.Black);
        Text.Draw($"GPU: {GraphicsAdapter.DefaultAdapter.Description}",
        new Vector2(UI_SPACING, VIRTUAL_HEIGHT - GlyphSize.Y * TEXT_SPACING * 1 + GlyphSize.Y / 2 * 1),
        Color.White, SpriteBatch, TextDrawingMode.Right, outline: true, outlineColor: Color.Black);
    }

    public static void CraftSwitchCall() => Instance.CraftSwitch();
    private void CraftSwitch()
    {
        InventoryMenu = InventoryMenu switch
        {
            InventoryMenu.Inventory => InventoryMenu.Craft,
            _ => InventoryMenu.Inventory
        };
        inventoryMenuButton.frameAdder = InventoryMenu == InventoryMenu.Inventory ? 0 : 2;
    }

    public static void OpenCraftMenuCall(CraftData craftData) => Instance.OpenCraftMenu(craftData);
    public void OpenCraftMenu(CraftData craftData)
    {
        craftMenu.SetCraftData(craftData);
        Crafting = true;
    }

    public static void CloseCraftMenuCall() => Instance.CloseCraftMenu();
    private void CloseCraftMenu()
    {
        Crafting = false;
    }

    public static void CallPlayerCraft()
    {
        Instance.Player.Craft(Instance.craftMenu.CraftData);
    }

    public static void RespawnPlayer() => Instance.Player.Respawn();
}