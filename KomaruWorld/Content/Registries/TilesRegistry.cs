using KomaruWorld.Content.GameContent.Tiles;
using KomaruWorld.Content.WorldSystem;

namespace KomaruWorld.Content.Registries;

public static class TilesRegistry
{
    public static Tile[] tiles { get; private set; } =
    {
        new DirtTile(),
        new StoneTile(),
        new GrassTile(),
        new PlanksTile(),
        new LogTile(),
        new StoneBricksTile()
    };

    public static void Register()
    {
        for (int i = 0; i < tiles.Length; i++)
            tiles[i].RegistryRegister(i);
    }
}