using System.Collections.Generic;
using KomaruWorld.Content.Registries;
using KomaruWorld.Content.SceneSystem;
using KomaruWorld.Content.TextSystem;
using KomaruWorld.Content.WorldSystem;
using KomaruWorld.Content.WorldSystem.IDs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace KomaruWorld.Content.Scenes;

public class WorldScene(SpriteBatch sp, GraphicsDeviceManager gm, ContentManager c) : GameScene(sp, gm, c)
{
    private Player player = new Player(new Vector2(105, 0));
    public OrthographicCamera Camera = new OrthographicCamera(gm.GraphicsDevice);

    public static WorldScene Instance;

    public override void Load()
    {
        base.Load();

        Instance = this;

        var worldSize = new Point(100, 100);
        World.LoadWorld(worldSize, WorldGenerator.Generate(worldSize));
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        World.Update(gameTime);
        player.Update(gameTime);

        Camera.Position = player.Position + new Vector2(Player.PIXEL_WIDTH * World.SIZE_MOD / 2,
        Player.PIXEL_HEIGHT * World.SIZE_MOD / 2 + 8 * World.SIZE_MOD) - new Vector2(Game1.PIXEL_SCREEN_WIDTH / 2f,
        Game1.PIXEL_SCREEN_HEIGHT / 2f);
    }

    public override void Draw()
    {
        base.Draw();

        var view = Camera.GetViewMatrix();
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: view);

        World.Draw(spriteBatch);
        player.Draw(spriteBatch);

        spriteBatch.End();
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        Text.Draw("Press LEFT to place tile", Vector2.Zero, Color.White, spriteBatch,
        TextDrawingMode.Right, outline: true, Color.Black);
        Text.Draw("Press RIGHT to destroy tile", new Vector2(0, 20), Color.White, spriteBatch,
        TextDrawingMode.Right, outline: true, Color.Black);
        Text.Draw("Press MOUSE WHEEL to change tile", new Vector2(0, 40), Color.White, spriteBatch,
        TextDrawingMode.Right, outline: true, Color.Black);
        
        TilesRegistry.tiles[player.TileID].DrawOutline(spriteBatch, new Vector2
        (Game1.PIXEL_SCREEN_WIDTH - 2 * World.SIZE_MOD - 8 * World.SIZE_MOD,
        2 * World.SIZE_MOD));

        TilesRegistry.tiles[player.TileID].DrawTile(spriteBatch, new Vector2
        (Game1.PIXEL_SCREEN_WIDTH - 2 * World.SIZE_MOD - 8 * World.SIZE_MOD,
        2 * World.SIZE_MOD));

        spriteBatch.End();
    }
}