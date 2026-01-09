using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class Player : GameObject
{
    // Movement
    private const float SPEED = 150f;
    private Direction direction = Direction.Null;
    private SpriteEffects flip = SpriteEffects.None;

    // Gravity
    private const float JUMP_FORCE = 500f;
    private bool isJumping = false;
    private bool isGrounded = false;
    public float GravityMod { get; private set; } = DEFAULT_GRAVITY;
    private float jumpMod = JUMP_FORCE;
    private const float ORIGINAL_JUMP_TIME = 0.5f;
    private float jumpTime = ORIGINAL_JUMP_TIME;

    // Inventory
    public int TileInHand { get; private set; } = 0;
    private Tiles[] tilesOrder =
    {
        Tiles.Grass, Tiles.Dirt, Tiles.Stone,
        Tiles.Log, Tiles.Leaf, Tiles.Planks
    };
    public Point cursorPosition { get; private set; } = Point.Zero;

    // Game
    private float deltaTime = 0f;
    private Rectangle hitbox
    { get { return new Rectangle((int)Position.X + 2, (int)Position.Y, (int)Size.X - 4, (int)Size.Y); } }

    // Input
    private MouseState lastMouse;

    // Keybinds
    private readonly Keys key_MoveRight = Keys.D;
    private readonly Keys key_MoveLeft = Keys.A;
    private readonly Keys key_Jump = Keys.Space;

    // Animation
    private float timeToFrame = FRAME_TIME;

    // Frames
    private const int FRAME_IDLE_0 = 0;
    private const int FRAME_IDLE_1 = 1;
    private const int FRAME_RUN_0 = 2;
    private const int FRAME_RUN_1 = 3;
    private const int FRAME_ISNT_GROUNDED = 4;

    public Player(Atlas atlas, Vector2 position, Vector2 size, int defaultFrame) : base(atlas, position, size, defaultFrame)
    {
        
    }

    public override void Update(GameTime gameTime)
    {
        deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        GetInput();
        Move();
        if (isJumping)
            Jump();
        else
            Gravity();
        Animate();
    }

    private void GetInput()
    {
        var keyboard = Keyboard.GetState();
        var mouse = Mouse.GetState();
        direction = Direction.Null;

        // Keyboard input

        if (keyboard.IsKeyDown(key_MoveRight))
            direction = Direction.Right;
        else if (keyboard.IsKeyDown(key_MoveLeft))
            direction = Direction.Left;

        if (keyboard.IsKeyDown(key_Jump) && isGrounded)
        {
            isGrounded = false;
            isJumping = true;
        }

        // Mouse Input

        cursorPosition = new Point
        (
            (int)((mouse.X + GameScene.Instance.camera.Position.X) / TileSize.X / 4),
            (int)((mouse.Y + GameScene.Instance.camera.Position.Y) / TileSize.Y / 4)
        );

        if (mouse.ScrollWheelValue < lastMouse.ScrollWheelValue)
        {
            TileInHand++;
            if (TileInHand >= tilesOrder.Length)
                TileInHand = 0;
        }
        else if (mouse.ScrollWheelValue > lastMouse.ScrollWheelValue)
        {
            TileInHand--;
            if (TileInHand < 0)
                TileInHand = tilesOrder.Length - 1;
        }

        if (mouse.LeftButton == ButtonState.Pressed)
        {
            Vector2 targetPosition = new Vector2
            (
                cursorPosition.X * TileSize.X * 4,
                cursorPosition.Y * TileSize.Y * 4
            );
            
            World.AddTile(TilesBank.FindTile(tilesOrder[TileInHand], targetPosition));
        }

        if (mouse.RightButton == ButtonState.Pressed)
        {
            Vector2 targetPosition = new Vector2
            (
                cursorPosition.X * TileSize.X * 4,
                cursorPosition.Y * TileSize.Y * 4
            );
            
            World.RemoveTile(World.SearchTile(targetPosition));
        }

        lastMouse = mouse;
    }

    private void Move()
    {
        if (direction == Direction.Null)
            return;

        int moveMod = direction switch
        {
            Direction.Right => 1,  
            Direction.Left => -1,  
            _ => 0,
        };

        float velocity = SPEED * deltaTime * moveMod;
        var nextHitbox = new Rectangle((int)(hitbox.X + velocity), hitbox.Y, hitbox.Width, hitbox.Height);

        foreach (var tile in World.Tiles)
        {
            if (nextHitbox.Intersects(tile.Hitbox))
            {
                isGrounded = true;
                Position = new Vector2(moveMod > 0 ? tile.Hitbox.Left - Rectangle.Width : tile.Hitbox.Right, Position.Y);
                velocity = 0f;
                break;
            }
        }

        Position += new Vector2(velocity, 0f);
    }

    private void Gravity()
    {
        GravityMod += GRAVITY_ACELERATION * deltaTime;

        if (GravityMod > MAXIMAL_GRAVITY)
            GravityMod = MAXIMAL_GRAVITY;

        isGrounded = false;
        var nextHitbox = new Rectangle(hitbox.X, (int)(hitbox.Y + GravityMod), hitbox.Width, hitbox.Height);

        foreach (var tile in World.Tiles)
        {
            if (nextHitbox.Intersects(tile.Hitbox))
            {
                isGrounded = true;
                GravityMod = DEFAULT_GRAVITY;
                Position = new Vector2(Position.X, tile.Rectangle.Top - Rectangle.Height);
                break;
            }
        }

        if (isGrounded)
            return;
        
        Position += new Vector2(0f, GravityMod);
    }

    private void Jump()
    {
        if ((jumpTime -= deltaTime) <= 0f)
        {
            isJumping = false;
            jumpMod = JUMP_FORCE;
            jumpTime = ORIGINAL_JUMP_TIME;
            return;
        }

        jumpMod = JUMP_FORCE * (jumpTime / ORIGINAL_JUMP_TIME);
        float velocity = jumpMod * deltaTime;
        var nextHitbox = new Rectangle(hitbox.X, (int)(hitbox.Y - velocity), hitbox.Width, hitbox.Height);

        foreach (var tile in World.Tiles)
        {
            if (nextHitbox.Intersects(tile.Hitbox))
            {
                isGrounded = true;
                GravityMod = DEFAULT_GRAVITY;
                Position = new Vector2(Position.X, tile.Rectangle.Bottom);
                velocity = 0f;
                break;
            }
        }

        Position -= new Vector2(0f, velocity);
    }

    private void Animate()
    {
        if (direction == Direction.Right)
            flip = SpriteEffects.None;
        else if (direction == Direction.Left)
            flip = SpriteEffects.FlipHorizontally;

        if (!isGrounded)
        {
            frame = FRAME_ISNT_GROUNDED;
            timeToFrame = 0f;
            return;
        }

        if ((timeToFrame -= deltaTime) <= 0f)
        {
            if (direction == Direction.Null)
            {
                frame = frame switch
                {
                    FRAME_IDLE_0 => FRAME_IDLE_1,
                    _ => FRAME_IDLE_0  
                };
            }
            else
            {
                frame = frame switch
                {
                    FRAME_RUN_0 => FRAME_RUN_1,
                    _ => FRAME_RUN_0  
                };
            }

            timeToFrame = FRAME_TIME;
        }
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