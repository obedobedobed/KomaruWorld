using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class PassiveMob : Mob
{
    // Frames
    private const int FRAME_IDLE = 0;
    private const int FRAME_RUN_0 = 1;
    private const int FRAME_RUN_1 = 2;
    private const int FRAME_JUMP = 3;

    public PassiveMob(Atlas atlas, Vector2 position, Vector2 size, string name, float speed, int defaultFrame,
    float jumpForce, float jumpTime, Rectangle hitbox, RangeF moveTimeRange, int health) : base(atlas, position, size, name,
    speed, defaultFrame, jumpForce, jumpTime, hitbox, moveTimeRange, 3f, health)
    {
         
    }

    protected override void Move()
    {
        if ((moveTime -= DeltaTime) > 0)
        {
            int moveMod = Direction switch
            {
                Direction.Right => 1,  
                Direction.Left => -1,  
                _ => 0,
            };

            float velocity = speed * DeltaTime * moveMod;
            var nextHitbox = new Rectangle((int)(HitboxPosApplied.X + velocity), HitboxPosApplied.Y,
            HitboxPosApplied.Width, HitboxPosApplied.Height);

            bool applyVelocity = true;

            foreach (var tile in World.Tiles)
            {
                if (nextHitbox.IntersectsNonInclusive(tile.Hitbox))
                {
                    Direction = Direction switch
                    {
                        Direction.Right => Direction.Left,
                        _ => Direction.Right
                    };
                    applyVelocity = false;
                    break;
                }
            }

            if (applyVelocity)
                Position += new Vector2(velocity, 0f);
        }
        else
        {
            Direction = Direction.Null;
            if ((moveBreakEstimated -= DeltaTime) <= 0)
            {
                moveBreakEstimated = moveBreak;
                moveTime = Random.Shared.Next
                (
                    (int)(moveTimeRange.MinimalValue * 100),
                    (int)(moveTimeRange.MaximalValue * 100)
                ) / 100;
                Direction = (Direction)Random.Shared.Next(0, 2);
            }
        }
    }

    protected override void Animation()
    {
        Flip = Direction switch
        {
            Direction.Right => SpriteEffects.None,
            Direction.Left => SpriteEffects.FlipHorizontally,
            _ => Flip
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