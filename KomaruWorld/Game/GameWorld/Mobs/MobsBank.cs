using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class MobsBank
{
    // Textures
    private static Atlas ChickenAtlas;
    private static Atlas SkeletonAtlas;

    public static void LoadContent(ContentManager Content)
    {
        ChickenAtlas = new Atlas(Content.Load<Texture2D>("Sprites/Mobs/Chicken"), EntitySize / SIZE_MOD);
        SkeletonAtlas = new Atlas(Content.Load<Texture2D>("Sprites/Mobs/Skeleton"), EntitySize / SIZE_MOD);
    }

    // Mobs
    public static Mob Chicken(Vector2 position) => new PassiveMob(ChickenAtlas, position, EntitySize, "Chicken", 35f * SIZE_MOD, 0, 125f * SIZE_MOD, 0.5f, new Rectangle(2 * SIZE_MOD, 1 * SIZE_MOD, (int)EntitySize.X - 4 * SIZE_MOD, (int)EntitySize.Y - 1 * SIZE_MOD), new RangeF(2, 5), health: 2);
    public static Mob Skeleton(Vector2 position) => new AgressiveMob(SkeletonAtlas, position, EntitySize, "Skeleton", 30f * SIZE_MOD, 0, 105f * SIZE_MOD, 0.5f, new Rectangle(0, 2 * SIZE_MOD, (int)EntitySize.X - 2 * SIZE_MOD, (int)EntitySize.Y), health: 4);
}