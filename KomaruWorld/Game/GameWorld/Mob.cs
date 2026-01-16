using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public abstract class Mob : GameObject
{
    // Movement
    protected readonly float speed;
    protected Direction Direction = Direction.Null;
    protected SpriteEffects flip = SpriteEffects.None;
    protected float moveTime;
    protected readonly RangeF moveTimeRange;
    protected readonly float moveBreak;
    protected float moveBreakEstimated;

    // Gravity
    private float GravityVelocity;
    protected bool IsGrounded;
    protected bool LastIsGrounded;
    protected bool IsJumping;
    private readonly float jumpForce;
    private float jumpMod;
    private readonly float originalJumpTime;
    private float jumpTime;

    // Collisions
    protected Rectangle Hitbox;
    protected Rectangle HitboxPosApplied
    {
        get
        {
            return new Rectangle
            (
                (int)(Position.X + Hitbox.X), (int)(Position.Y + Hitbox.Y),
                Hitbox.Width, Hitbox.Height
            );
        }
    }

    // Game
    protected float DeltaTime;

    // Animation
    protected float timeToFrame = FRAME_TIME;

    public Mob(Atlas atlas, Vector2 position, Vector2 size, float speed, int defaultFrame,
    float jumpForce, float jumpTime, Rectangle hitbox, RangeF moveTimeRange, float moveBreak)
    : base(atlas, position, size, defaultFrame)
    {
        this.speed = speed;
        this.jumpForce = jumpForce;
        jumpMod = jumpForce;
        originalJumpTime = jumpTime;
        this.jumpTime = originalJumpTime;
        Hitbox = hitbox;
        this.moveTimeRange = moveTimeRange;
        this.moveBreak = moveBreak;
        moveBreakEstimated = moveBreak;
    }

    public override void Update(GameTime gameTime)
    {
        DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Move();
        if (IsJumping)
            Jump();
        else
            Gravity();
        Animation();
    }

    protected abstract void Move();
    protected abstract void Animation();

    protected void Gravity()
    {
        GravityVelocity += GRAVITY_ACELERATION * DeltaTime;

        if (GravityVelocity > MAXIMAL_GRAVITY)
            GravityVelocity = MAXIMAL_GRAVITY;

        IsGrounded = false;
        var nextHitbox = new Rectangle(HitboxPosApplied.X, (int)(HitboxPosApplied.Y + GravityVelocity),
        HitboxPosApplied.Width, HitboxPosApplied.Height);

        foreach (var tile in World.Tiles)
        {
            if (nextHitbox.Intersects(tile.Hitbox))
            {
                IsGrounded = true;
                GravityVelocity = DEFAULT_GRAVITY;
                Position = new Vector2(Position.X, tile.Rectangle.Top - Rectangle.Height);
                break;
            }
        }


        if (IsGrounded)
        {
            LastIsGrounded = IsGrounded;
            return;
        }
        else if (!IsGrounded && LastIsGrounded)
        {
            System.Console.WriteLine("chicken jockey");
            IsJumping = true;
        }
        
        LastIsGrounded = IsGrounded;
        Position += new Vector2(0f, GravityVelocity);
    }

    protected void Jump()
    {
        if ((jumpTime -= DeltaTime) <= 0f)
        {
            IsJumping = false;
            jumpMod = jumpForce;
            jumpTime = originalJumpTime;
            return;
        }

        jumpMod = jumpForce * (jumpTime / originalJumpTime);
        float velocity = jumpMod * DeltaTime;
        var nextHitbox = new Rectangle(Hitbox.X, (int)(Hitbox.Y - velocity), Hitbox.Width, Hitbox.Height);

        foreach (var tile in World.Tiles)
        {
            if (nextHitbox.Intersects(tile.Hitbox))
            {
                IsGrounded = true;
                GravityVelocity = DEFAULT_GRAVITY;
                Position = new Vector2(Position.X, tile.Rectangle.Bottom);
                jumpTime = 0f;
                velocity = 0f;
                break;
            }
        }

        Position -= new Vector2(0f, velocity);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw
        (
            atlas.Texture, Rectangle, atlas.Rectangles[frame],
            Color.White, 0f, Vector2.Zero, flip, 0f
        );
    }
}