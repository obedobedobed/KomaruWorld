using System;
using KomaruWorld.Content.Registries;
using KomaruWorld.Content.WorldSystem;

namespace KomaruWorld.Content.GameContent.Tiles;

public class PlanksTile : Tile
{
    public override void SetDefaults()
    {
        bool canGetTexture = TexturesRegistry.Textures.TryGetValue(TexturesRegistry.PLANKS_TILE, out texture);
        if (!canGetTexture)
            throw new Exception($"Cannot get texture of {Type.FilterName}");
    }
}