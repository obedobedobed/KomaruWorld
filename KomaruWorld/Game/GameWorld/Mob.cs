using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public abstract class Mob : GameObject
{
    // Statistic
    public string Name { get; private set; }
    public readonly int MaxHealth;
    public int Health { get; private set; }

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
    public Rectangle HitboxPosApplied
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

    protected int HitboxRightOffset => Rectangle.Right - HitboxPosApplied.Right;
    protected int HitboxBottomOffset => Rectangle.Bottom - HitboxPosApplied.Bottom;

    private const float TAKE_DAMAGE_COOLDOWN = 1f;
    private float timeToTakeDamage = 0f;

    // Game
    protected float DeltaTime;

    // Animation
    protected float timeToFrame = FRAME_TIME;

    // Combat
    private const float IMMORTAL_TIME = 0.5f;
    private float immortalTime = 0f;

    private Vector2 knockbackVelocity;

    public Mob(Atlas atlas, Vector2 position, Vector2 size, string name, float speed, int defaultFrame,
    float jumpForce, float jumpTime, Rectangle hitbox, RangeF moveTimeRange, float moveBreak, int health)
    : base(atlas, position, size, defaultFrame)
    {
        Name = name;
        this.speed = speed;
        this.jumpForce = jumpForce;
        jumpMod = jumpForce;
        originalJumpTime = jumpTime;
        this.jumpTime = originalJumpTime;
        Hitbox = hitbox;
        this.moveTimeRange = moveTimeRange;
        this.moveBreak = moveBreak;
        moveBreakEstimated = moveBreak;
        MaxHealth = health;
        Health = health;
    }

    public override void Update(GameTime gameTime)
    {
        DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        timeToTakeDamage -= DeltaTime;
        immortalTime -= DeltaTime;

        if (knockbackVelocity.X == 0 && knockbackVelocity.Y == 0)
            Move();
        else
            TakeKnockback();

        if (IsJumping)
            Jump();
        else
            Gravity();

        Animation();


        if (Health <= 0 && immortalTime <= 0)
        {
            Logger.Log($"{Name} died");
            World.RemoveMob(this);
        }
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
            IsJumping = true;
        
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

    public void TakeDamage(int damage)
    {
        if (timeToTakeDamage <= 0 && immortalTime <= 0)
        {
            Health -= damage;

            timeToTakeDamage = TAKE_DAMAGE_COOLDOWN;
            immortalTime = IMMORTAL_TIME;
            knockbackVelocity = new Vector2(5f * (GameScene.Instance.Player.Flip == SpriteEffects.None ? 1 : -1), -0.12f);
        }
    }

    public void TakeKnockback()
    {
        int knockbackXMod = knockbackVelocity.X > 0 ? 1 : -1;

        knockbackVelocity -= new Vector2(DeltaTime * 5f * knockbackXMod, DeltaTime * -5f);
        Position += knockbackVelocity;

        if (knockbackVelocity.X < 0f && knockbackXMod > 0)
            knockbackVelocity.X = 0f;
        else if (knockbackVelocity.X > 0f && knockbackXMod < 0)
            knockbackVelocity.X = 0f;

        if (knockbackVelocity.Y > 0f)
            knockbackVelocity.Y = 0f;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw
        (
            atlas.Texture, Rectangle, atlas.Rectangles[Frame],
            immortalTime <= 0 ? Color.White : Color.Red, 0f, Vector2.Zero, flip, 0f
        );
    }
}