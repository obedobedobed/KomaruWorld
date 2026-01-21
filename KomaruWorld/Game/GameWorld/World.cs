using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class World
{
    public static List<Tile> Tiles { get; private set; } = new List<Tile>();
    public static List<Tile> Walls { get; private set; } = new List<Tile>();
    public static List<DroppedItem> Items { get; private set; } = new List<DroppedItem>();
    public static List<Mob> Mobs { get; private set; } = new List<Mob>();

    private static Vector2 cameraPosOffset = new Vector2(-TileSize.X, -TileSize.Y);
    private static Vector2 cameraSizeOffset = new Vector2(TileSize.X * 2, TileSize.Y * 2);

    public static bool AddTile(Tile tile, bool replace = false)
    {
        var tileOnSamePosition = SearchTile(tile.Position);

        if (tileOnSamePosition?.TileType == tile.TileType)
            return false;

        if (tileOnSamePosition != null && replace)
            RemoveTile(tileOnSamePosition);
        if (tileOnSamePosition != null && !replace)
            return false;

        Tiles.Add(tile);
        return true;
    }

    public static void RemoveTile(Tile tile)
    {
        Tiles.Remove(tile);
    }

    public static Tile SearchTile(Vector2 position)
    {
        foreach (var tile in Tiles)
            if (tile.Position == position)
                return tile;

        return null;
    }

    public static Tile[] SearchTilesAround(Vector2 position)
    {
        List<Tile> tiles = new List<Tile>();
        var offsets = new Vector2[]
        {
            new(-TileSize.X, -TileSize.Y), new(0, -TileSize.Y), new(TileSize.X, -TileSize.Y),
            new(-TileSize.X, 0),                                new(TileSize.X, 0),
            new(-TileSize.X, TileSize.Y),  new(0, TileSize.Y),  new(TileSize.X, TileSize.Y),
        };

        foreach (var offset in offsets)
        {
            var tile = SearchTile(position + offset);
            if (tile != null)
                tiles.Add(tile);
        }

        return tiles.ToArray();
    }

    public static bool AddWall(Tile wall, bool replace = false)
    {
        var wallOnSamePosition = SearchWall(wall.Position);

        if (wallOnSamePosition?.TileType == wall.TileType)
            return false;

        if (wallOnSamePosition != null && replace)
            RemoveWall(wallOnSamePosition);
        if (wallOnSamePosition != null && !replace)
            return false;

        Walls.Add(wall);
        return true;
    }

    public static void RemoveWall(Tile wall)
    {
        Walls.Remove(wall);
    }

    public static Tile SearchWall(Vector2 position)
    {
        foreach (var wall in Walls)
            if (wall.Position == position)
                return wall;

        return null;
    }

    public static void AddItem(DroppedItem item)
    {
        Items.Add(item);
    }

    public static void RemoveItem(DroppedItem item)
    {
        Items.Remove(item);
    }

    public static void AddMob(Mob mob)
    {
        Mobs.Add(mob);
    }

    public static void Update(GameTime gameTime)
    {
        Tile[] tiles = Tiles.ToArray();
        Tile[] walls = Walls.ToArray();
        DroppedItem[] items = Items.ToArray();
        Mob[] mobs = Mobs.ToArray();

        foreach (var tile in tiles)
            tile?.Update(gameTime);

        foreach (var wall in walls)
            wall?.Update(gameTime);

        foreach (var item in items)
            item?.Update(gameTime);

        foreach (var mob in mobs)
            mob?.Update(gameTime);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        var cameraRect = GameScene.Instance.Camera.BoundingRectangle;
        cameraRect.Position += cameraPosOffset;
        cameraRect.Width += cameraSizeOffset.X;
        cameraRect.Height += cameraSizeOffset.Y;

        foreach (var wall in Walls)
            if (wall.Rectangle.Intersects(cameraRect.ToRectangle()))
                wall.Draw(spriteBatch);

        foreach (var tile in Tiles)
            if (tile.Rectangle.Intersects(cameraRect.ToRectangle()))
                tile.Draw(spriteBatch);

        foreach (var item in Items)
            if (item.Rectangle.Intersects(cameraRect.ToRectangle()))
                item.Draw(spriteBatch);

        foreach (var mob in Mobs)
            if (mob.Rectangle.Intersects(cameraRect.ToRectangle()))
                mob.Draw(spriteBatch);
    }
}