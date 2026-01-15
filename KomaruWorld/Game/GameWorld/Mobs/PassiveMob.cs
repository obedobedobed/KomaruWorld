using System;
using System.Diagnostics;
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

    public PassiveMob(Atlas atlas, Vector2 position, Vector2 size, float speed, int defaultFrame,
    float jumpForce, Rectangle hitbox, RangeF moveTimeRange) : base(atlas, position, size, speed,
    defaultFrame, jumpForce, hitbox, moveTimeRange, 3f)
    {
         
    }

    protected override void Move()
    {
        if ((moveTime -= DeltaTime) > 0)
        {
            Direction = (Direction)Random.Shared.Next(0, 2);

            int moveMod = Direction switch
            {
                Direction.Right => 1,  
                Direction.Left => -1,  
                _ => 0,
            };

            float velocity = speed * DeltaTime * moveMod;
            var nextHitbox = new Rectangle((int)(Hitbox.X + velocity), Hitbox.Y, Hitbox.Width, Hitbox.Height);

            foreach (var tile in World.Tiles)
            {
                if (nextHitbox.Intersects(tile.Hitbox))
                {
                    Direction = Direction switch
                    {
                        Direction.Right => Direction.Left,
                        _ => Direction.Right
                    };
                    break;
                }
            }

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
                    (int)moveTimeRange.MinimalValue * 100,
                    (int)moveTimeRange.MaximalValue * 100
                ) / 100;
            }
        }
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
            frame = FRAME_JUMP;
            return;
        }
        else if ((timeToFrame -= DeltaTime) <= 0)
        {
            if (Direction == Direction.Null)
                frame = FRAME_IDLE;
            else
                frame = frame switch
                {
                    FRAME_RUN_0 => FRAME_RUN_1,
                    _ => FRAME_RUN_0  
                };
        }
    }
}