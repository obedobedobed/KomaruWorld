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

        int moveMod = GameScene.Instance.Player.Position.X > Position.X ? 1 : -1;
        float velocity = speed * DeltaTime * moveMod;

        Direction = moveMod > 0 ? Direction.Right : Direction.Left;

        var nextHitbox = new Rectangle((int)(HitboxPosApplied.X + velocity), HitboxPosApplied.Y,
        HitboxPosApplied.Width, HitboxPosApplied.Height);

        foreach (var tile in World.Tiles)
        {
            if (tile.Hitbox.Bottom <= Position.Y || tile.Hitbox.Top >= Position.Y + Hitbox.Height)
                continue;

            if (nextHitbox.Right > tile.Hitbox.Left && nextHitbox.Left < tile.Hitbox.Right)
            {
                velocity = 0f;
                break;
            }
        }

        Position += new Vector2(velocity, 0f);
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