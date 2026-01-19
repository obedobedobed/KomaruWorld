using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public abstract class Mob : GameObject
{
    // Statistic
    public string Name { get; private set; }

    // Movement
    protected readonly float speed;
    protected Direction Direction = Direction.Null;
    protected SpriteEffects flip = SpriteEffects.None;
    protected float moveTime;
    protected readonly RangeF moveTimeRange;
    protected readonly float moveBreak;
    protected float moveBreakEstimated;
    
    // Expose flip for NetworkManager
    public SpriteEffects Flip => flip;
    
    // Gravity
    private float GravityVelocity;
    protected bool IsGrounded;
    protected bool LastIsGrounded;
    protected bool IsJumping;
    private readonly float jumpForce;
    private float jumpMod;
    private readonly float originalJumpTime;
    private float jumpTime;

    // Network
    public int NetworkId { get; set; } = -1;
    public bool IsRemote { get; set; } = false;
    private int _netUpdateTimer = 0; // Throttle packets
    
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

    public Mob(Atlas atlas, Vector2 position, Vector2 size, string name, float speed, int defaultFrame,
    float jumpForce, float jumpTime, Rectangle hitbox, RangeF moveTimeRange, float moveBreak)
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
        
        // Auto-detect if we should be a remote entity (Client side)
        // Note: This logic assumes Mobs are deterministic or synced via World.AddMob
        if (Game1.Instance.NetworkManager != null && 
            Game1.Instance.NetworkManager.IsRunning && 
            !Game1.Instance.NetworkManager.IsHost)
        {
            IsRemote = true;
        }
    }

    public override void Update(GameTime gameTime)
    {
        DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // --- 1. Client (Remote) Logic ---
        // If we are a client, we STOP here. We do not run physics or AI.
        // We rely entirely on SetRemotePosition() called by NetworkManager.
        if (IsRemote) return;
        
        // --- 2. Host / Local Logic ---
        // Calculate physics, AI, and animation normally.
        Move();
        if (IsJumping)
            Jump();
        else
            Gravity();
        Animation();
        
        // --- 3. Network Send (Host Only) ---
        // Send the results of the calculation above to clients.
        if (Game1.Instance.NetworkManager != null && 
            Game1.Instance.NetworkManager.IsHost && 
            Game1.Instance.NetworkManager.IsRunning)
        {
            // Send update approx every 3 frames (20 times/sec) to save bandwidth
            _netUpdateTimer++;
            if (_netUpdateTimer >= 3)
            {
                _netUpdateTimer = 0;
                Game1.Instance.NetworkManager.SendMobPosition(this);
            }
        }
    }
    
    public void SetRemotePosition(Vector2 pos, int frameIdx, bool flipX)
    {
        Position = pos;
        frame = frameIdx;
        flip = flipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
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

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw
        (
            atlas.Texture, Rectangle, atlas.Rectangles[frame],
            Color.White, 0f, Vector2.Zero, flip, 0f
        );
    }
}