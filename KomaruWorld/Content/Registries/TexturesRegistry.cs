using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld.Content.Registries;

public static class TexturesRegistry
{
    public static Dictionary<int, Texture2D> Textures { get; private set; } = [ ];

    public const int DIRT_TILE = 0;
    public const int PLAYER = 1;
    public const int STONE_TILE = 2;
    public const int GRASS_TILE = 3;
    public const int FONT = 4;
    public const int PLANKS_TILE = 5;
    public const int LOG_TILE = 6;
    public const int STONE_BRICKS_TILE = 7;

    public static void Register(ContentManager Content)
    {
        Textures.Add(DIRT_TILE, Content.Load<Texture2D>("Assets/Textures/Tiles/DirtTile"));
        Textures.Add(PLAYER, Content.Load<Texture2D>("Assets/Textures/Entity/Player"));
        Textures.Add(STONE_TILE, Content.Load<Texture2D>("Assets/Textures/Tiles/StoneTile"));
        Textures.Add(GRASS_TILE, Content.Load<Texture2D>("Assets/Textures/Tiles/GrassTile"));
        Textures.Add(FONT, Content.Load<Texture2D>("Assets/Textures/Font"));
        Textures.Add(PLANKS_TILE, Content.Load<Texture2D>("Assets/Textures/Tiles/PlanksTile"));
        Textures.Add(LOG_TILE, Content.Load<Texture2D>("Assets/Textures/Tiles/LogTile"));
        Textures.Add(STONE_BRICKS_TILE, Content.Load<Texture2D>("Assets/Textures/Tiles/StoneBricksTile"));
    }
}