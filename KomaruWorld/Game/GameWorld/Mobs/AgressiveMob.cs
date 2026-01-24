using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class AgressiveMob : Mob
{
    // Frames
    private const int FRAME_IDLE = 0;
    private const int FRAME_RUN_0 = 1;
    private const int FRAME_RUN_1 = 2;
    private const int FRAME_JUMP = 3;

    public AgressiveMob(Atlas atlas, Vector2 position, Vector2 size, string name, float speed, int defaultFrame,
    float jumpForce, float jumpTime, Rectangle hitbox, int health) : base(atlas, position, size, name,
    speed, defaultFrame, jumpForce, jumpTime, hitbox, new RangeF(), 3f, health)
    {
         
    }

    protected override void Move()
    {
        if (GameScene.Instance.Player.Position.X == Position.X)
            return;

        Logger.Log("------------------Move------------------");
        int moveMod = GameScene.Instance.Player.Position.X > Position.X ? 1 : -1;
        float velocity = speed * DeltaTime * moveMod;

        Direction = moveMod > 0 ? Direction.Right : Direction.Left;

        Logger.Log($"Vel = {velocity}");
        Logger.Log($"Dir = {Direction}");

        var nextHitbox = new Rectangle((int)(HitboxPosApplied.X + velocity), HitboxPosApplied.Y,
        HitboxPosApplied.Width, HitboxPosApplied.Height);

        foreach (var tile in World.Tiles)
        {
            if (nextHitbox.IntersectsNonInclusive(tile.Hitbox))
            {
                if (tile.Hitbox.Bottom <= Hitbox.Top || tile.Hitbox.Top >= Hitbox.Bottom)
                    continue;
                Position = new Vector2(moveMod > 0
                ? tile.Hitbox.Left - Rectangle.Width
                : tile.Hitbox.Right, Position.Y);
                velocity = 0f;
                Logger.Log($"Intersection with: {tile.TileType} - {tile.TileWorldID}");
                break;
            }
        }

        Position += new Vector2(velocity, 0f);
        Logger.Log($"Applyed Vel: pos x{Position.X} y{Position.Y}");
        Logger.Log("----------------------------------------");
    }

    protected override void Animation()
    {
        flip = Direction switch
        {
            Direction.Right => SpriteEffects.None,
            Direction.Left => SpriteEffects.FlipHorizontally,
            _ => flip
        };

        if (IsJumping)
        {
            timeToFrame = 0;
            Frame = FRAME_JUMP;
            return;
        }
        else if ((timeToFrame -= DeltaTime) <= 0)
        {
            if (Direction == Direction.Null)
                Frame = FRAME_IDLE;
            else
                Frame = Frame switch
                {
                    FRAME_RUN_0 => FRAME_RUN_1,
                    _ => FRAME_RUN_0  
                };

            timeToFrame = FRAME_TIME;
        }
    }
}