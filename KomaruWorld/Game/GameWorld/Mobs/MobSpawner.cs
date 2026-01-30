using System;
using Microsoft.Xna.Framework;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class MobsSpawner
{
    private const float MOB_SPAWN_TIME = 15f;
    private static float timeToMobSpawn = MOB_SPAWN_TIME;
    private static int mobSpawnChance = 10;

    public static void Update(GameTime gameTime)
    {
        timeToMobSpawn -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (timeToMobSpawn <= 0 && Random.Shared.Next(0, 101) <= mobSpawnChance)
        {
            World.AddMob(MobsBank.Slime(new Vector2
            (Random.Shared.Next(0, (int)(VerySmallWorldSize.X * TileSize.X - EntitySize.X)), 100)));
            
            Logger.Log("Spawned mob");
            timeToMobSpawn = MOB_SPAWN_TIME;
        }
    }
}