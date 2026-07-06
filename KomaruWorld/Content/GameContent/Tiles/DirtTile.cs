using System;
using KomaruWorld.Content.Registries;
using KomaruWorld.Content.WorldSystem;

namespace KomaruWorld.Content.GameContent.Tiles;

public class DirtTile : Tile
{
    public override void SetDefaults()
    {
        bool canGetTexture = TexturesRegistry.Textures.TryGetValue(TexturesRegistry.DIRT_TILE, out texture);
        if (!canGetTexture)
            throw new Exception($"Cannot get texture of {Type.FilterName}");
    }
}