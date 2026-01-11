using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
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
    public Player Player { get; private set; }
    private KeyboardState lastKeyboard;
    private Texture2D pixel;

    // World
    private int worldWidth = 60;
    private int worldHeight = 40;

    // Debug
    private bool debugMenuOpened = false;

    public override void Load()
    {
        Instance = this;
        pixel = Content.Load<Texture2D>("Sprites/Pixel");

        WorldGenerator.Generate(worldWidth, worldHeight);

        var playerAtlas = new Atlas(texture: Content.Load<Texture2D>("Sprites/KomaruAtlas"), spriteSize: PlayerSize / SIZE_MOD);
        var slotAtlas = new Atlas(texture: Content.Load<Texture2D>("Sprites/UI/SlotAtlas"), spriteSize: SlotSize / SIZE_MOD);
        Player = new Player(playerAtlas, new Vector2(worldWidth * TileSize.X / 2, 100), PlayerSize,
        defaultFrame: 1, slotAtlas: slotAtlas);

        Camera.Position = Player.Position;
    }

    public override void Update(GameTime gameTime)
    {
        World.Update(gameTime);
        Player.Update(gameTime);

        var playerPosCentered = Player.Position - new Vector2
        (GraphicsManager.PreferredBackBufferWidth / 2, GraphicsManager.PreferredBackBufferHeight / 2) +
        Player.Size / 2;
        Camera.Position = Vector2.Lerp(Camera.Position, playerPosCentered, 0.1f);

        foreach (var _object in Objects)
            _object.Update(gameTime);

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
    }

    public override void Draw()
    {
        var view = Camera.GetViewMatrix();
        int screenWidth = GraphicsManager.PreferredBackBufferWidth;
        int screenHeight = GraphicsManager.PreferredBackBufferHeight;

        // Game world (applying transform matrix)
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: view);

        World.Draw(SpriteBatch);
        Player.Draw(SpriteBatch);

        foreach (var _object in Objects)
            _object.Draw(SpriteBatch);

        SpriteBatch.End(); 

        // UI (not applying transform matrix)
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        if (Player.InInventory)
        {
            SpriteBatch.Draw(pixel, new Rectangle(0, 0, GraphicsManager.PreferredBackBufferWidth,
            GraphicsManager.PreferredBackBufferHeight), new Color(0, 0, 0, 150));
            Player.Inventory.DrawInventory(SpriteBatch); 
            Text.Draw("Inventory", new Vector2(screenWidth / 2, UI_SPACING), Color.White, SpriteBatch, TextDrawingMode.Center);
        }

        Player.Inventory.DrawHotbar(SpriteBatch);

        var mouse = Mouse.GetState();
        var cursorRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

        foreach (var slot in Player.Inventory.Slots)
        {
            if (slot.Rectangle.Intersects(cursorRectangle) && slot.Item != null)
            {
                var descriptionPos = new Vector2(slot.Position.X + SlotSize.X, slot.Position.Y);
                Text.Draw(slot.Item.Name + $" (x{slot.ItemAmount})", new Vector2(descriptionPos.X, descriptionPos.Y
                + (GlyphSize.X + TEXT_SPACING) * 0), Color.White, SpriteBatch, TextDrawingMode.Right);

                if (slot.Item is PlaceableItem)
                    Text.Draw("Placeable", new Vector2(descriptionPos.X, descriptionPos.Y
                    + (GlyphSize.X + TEXT_SPACING) * 1), Color.White, SpriteBatch, TextDrawingMode.Right);
                else if (slot.Item.IsTool)
                {
                    string toWritePower = "[Unknown parameter]";

                    if (slot.Item is SwordItem sword)
                        toWritePower = $"Damage: {sword.Damage}";
                    else if (slot.Item is PickaxeItem pickaxe)
                        toWritePower = $"Pickaxe power: {pickaxe.Damage}";
                    else if (slot.Item is AxeItem axe)
                        toWritePower = $"Axe power: {axe.Damage}";

                    Text.Draw(toWritePower, new Vector2(descriptionPos.X, descriptionPos.Y
                        + (GlyphSize.X + TEXT_SPACING) * 1), Color.White, SpriteBatch, TextDrawingMode.Right);
                    Text.Draw("Tool", new Vector2(descriptionPos.X, descriptionPos.Y
                    + (GlyphSize.X + TEXT_SPACING) * 2), Color.White, SpriteBatch, TextDrawingMode.Right);
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

                    Text.Draw(armorElement, new Vector2(descriptionPos.X, descriptionPos.Y
                    + (GlyphSize.X + TEXT_SPACING) * 1), Color.White, SpriteBatch, TextDrawingMode.Right);
                    Text.Draw($"Armor: {armor.Armor}", new Vector2(descriptionPos.X, descriptionPos.Y
                    + (GlyphSize.X + TEXT_SPACING) * 2), Color.White, SpriteBatch, TextDrawingMode.Right);
                }
            }
        }

        var itemInCursor = Player.ItemInCursor;
        int itemInCursorAmount = Player.ItemInCursorAmount;
        if (itemInCursor != null)
        {
            string itemAmountString = itemInCursorAmount > 1 ? itemInCursorAmount.ToString() : string.Empty;

            SpriteBatch.Draw(itemInCursor.Texture, new Rectangle(mouse.Position + CursorSize, ItemSize.ToPoint()), Color.White);
            Text.Draw(itemAmountString, (mouse.Position + CursorSize + ItemSize.ToPoint()).ToVector2(),
            Color.White, SpriteBatch, TextDrawingMode.Center);
        }

        if (debugMenuOpened)
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
            Color.White, SpriteBatch, TextDrawingMode.Right);
            Text.Draw($"FPS:{Game1.Instance.FPS} {vSyncString}",
            new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 1 + UI_SPACING - GlyphSize.Y / 2 * 1),
            Color.White, SpriteBatch, TextDrawingMode.Right);
            Text.Draw($"Position: x{(int)Player.Position.X}, y{(int)Player.Position.Y}",
            new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 2 + UI_SPACING - GlyphSize.Y / 2 * 2),
            Color.White, SpriteBatch, TextDrawingMode.Right);
            Text.Draw($"Gravity: {(int)Player.GravityVelocity}",
            new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 3 + UI_SPACING - GlyphSize.Y / 2 * 3),
            Color.White, SpriteBatch, TextDrawingMode.Right);
            Text.Draw($"Hotbar slot: {hotbarSlotString}",
            new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 4 + UI_SPACING - GlyphSize.Y / 2 * 4),
            Color.White, SpriteBatch, TextDrawingMode.Right);
            Text.Draw($"Cursor: x{Player.cursorWorldPosition.X}, y{Player.cursorWorldPosition.Y}",
            new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 5 + UI_SPACING - GlyphSize.Y / 2 * 5),
            Color.White, SpriteBatch, TextDrawingMode.Right);

            

            long usedMemory = GC.GetTotalMemory(false) / 1024;
            long totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024 / 1024;

            Text.Draw($"OS: {RuntimeInformation.OSDescription}",
            new Vector2(UI_SPACING, screenHeight - GlyphSize.Y * TEXT_SPACING * 4 + GlyphSize.Y / 2 * 4),
            Color.White, SpriteBatch, TextDrawingMode.Right);
            Text.Draw($"CPU: {Environment.ProcessorCount} threads CPU",
            new Vector2(UI_SPACING, screenHeight - GlyphSize.Y * TEXT_SPACING * 3 + GlyphSize.Y / 2 * 3),
            Color.White, SpriteBatch, TextDrawingMode.Right);
            Text.Draw($"Memory: {usedMemory}MB/{totalMemory}MB used",
            new Vector2(UI_SPACING, screenHeight - GlyphSize.Y * TEXT_SPACING * 2 + GlyphSize.Y / 2 * 2),
            Color.White, SpriteBatch, TextDrawingMode.Right);
            Text.Draw($"GPU: {GraphicsAdapter.DefaultAdapter.Description}",
            new Vector2(UI_SPACING, screenHeight - GlyphSize.Y * TEXT_SPACING * 1 + GlyphSize.Y / 2 * 1),
            Color.White, SpriteBatch, TextDrawingMode.Right);
        }

        SpriteBatch.End();
    }
}