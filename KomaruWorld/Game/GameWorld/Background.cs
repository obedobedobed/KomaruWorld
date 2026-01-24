using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class Background
{
    public const float PARALAX = 0.2f;
    public static OrthographicCamera Camera { get; private set; } = new OrthographicCamera(Game1.Instance.GraphicsDevice);

    // Clouds
    private static Atlas cloudsAtlas;
    private static List<BackgroundObject> clouds = new List<BackgroundObject>();
    private const float CLOUDS_Y_POS = 32;
    private const float CLOUDS_SPEED = 5;
    private const int START_CLOUDS_AMOUNT = 5;
    private const int CLOUDS_SPACING = 8 * BG_MOD;

    public static void Load(Atlas cloudsAtlas)
    {
        Background.cloudsAtlas = cloudsAtlas;

        float cloudXPos = 64;
        for (int i = 0; i < START_CLOUDS_AMOUNT; i++)
        {
            CreateCloud(cloudXPos);
            cloudXPos += CloudSize.X + CLOUDS_SPACING;
        }
    }

    public static void CreateCloud(float xPos)
    {
        int targetTexture = Random.Shared.Next(0, cloudsAtlas.Rectangles.Count);
        clouds.Add(new BackgroundObject(cloudsAtlas, new Vector2(xPos, CLOUDS_Y_POS), CloudSize, targetTexture));
    }

    public static void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        var gameCamPos = GameScene.Instance.Camera.Position;
        Camera.Position = gameCamPos - gameCamPos * (1f - PARALAX);

        var cloudsToRemove = new List<BackgroundObject>();

        foreach (var cloud in clouds)
        {
            cloud.Move(new Vector2(-CLOUDS_SPEED * BG_MOD * deltaTime, 0));
            if (cloud.Rectangle.Right < 0)
            {
                cloudsToRemove.Add(cloud);
                Logger.Log("rmv");
            }
        }

        foreach (var toRemove in cloudsToRemove)
            clouds.Remove(toRemove);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var cloud in clouds)
            cloud.Draw(spriteBatch);
    }
}