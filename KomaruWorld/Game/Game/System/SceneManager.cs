using Microsoft.Xna.Framework;

namespace KomaruWorld;

public static class SceneManager
{
    public static Scene Scene { get; private set; }

    public static void Load(Scene scene)
    {
        Scene = scene;
        Scene.Load();
    }

    public static void Update(GameTime gameTime) => Scene.Update(gameTime);
    public static void Draw() => Scene.Draw();
}