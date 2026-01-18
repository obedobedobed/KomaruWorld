using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class DoorTile : Tile
{
    private static List<DoorTile> doorsToToggle = new List<DoorTile>();
    private Texture2D closedTexture;
    private Texture2D openedTexture;

    public bool ToggleOperationEnded { get; private set; } = false;

    public DoorTile(Texture2D closedTexture, Texture2D openedTexture, Vector2 position, Vector2 size, bool canCollide,
    Tiles tileType, ToolToDestroy toolToDestroy, float destroyTime, int minimalToolPower, DropData drop)
    : base(closedTexture, position, size, canCollide, tileType, toolToDestroy, destroyTime, minimalToolPower, drop)
    {
        this.closedTexture = closedTexture;
        this.openedTexture = openedTexture;
    }

    public void Toggle()
    {
        var tilesAround = World.SearchTilesAround(Position);

        ToggleOperationEnded = true;

        foreach (var tile in tilesAround)
            if (tile is DoorTile door)
                door.GetDoorsAround();

        foreach (var door in doorsToToggle)
            door._Toggle();

        doorsToToggle = new List<DoorTile>();
    }

    private void _Toggle()
    {
        CanCollide = !CanCollide;
        texture = CanCollide ? closedTexture : openedTexture;
    }

    private void GetDoorsAround()
    {
        var around = World.SearchTilesAround(Position);
        foreach (var tile in around)
            if (tile is DoorTile door && !doorsToToggle.Contains(door))
            {
                doorsToToggle.Add(door);
                door.GetDoorsAround();
            }
    }
}