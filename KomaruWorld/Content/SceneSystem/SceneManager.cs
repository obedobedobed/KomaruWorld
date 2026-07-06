using Microsoft.Xna.Framework;

namespace KomaruWorld.Content.SceneSystem;

public static class SceneManager
{
    public static GameScene currentScene { get; private set; }

    public static void Load(GameScene scene)
    {
        
        currentScene = scene;
        currentScene.Load();
    }
    public static void Update(GameTime gameTime) => currentScene.Update(gameTime);
    public static void Draw() => currentScene.Draw();
}