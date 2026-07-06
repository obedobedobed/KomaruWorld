using System;
using KomaruWorld.Content.Registries;
using KomaruWorld.Content.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KomaruWorld.Content.WorldSystem;

public class Player
{
    private Texture2D texture;
    public const int PIXEL_WIDTH = 13;
    public const int PIXEL_HEIGHT = 21;
    private Rectangle hitboxNonFlipped => new Rectangle(x: 0, y: 0, width: 13, height: 21);
    private Rectangle hitboxFlipped => new Rectangle(x: 0, y: 0, width: 9, height: 21);
    private Vector2 velocity = Vector2.Zero;

    private Vector2 position = Vector2.Zero;
    public Vector2 Position
    {
        get => position;
    }

    public int TileID { get; private set; } = 1;

    private const float SPEED = 4f;
    private const float GRAVITY = 0.25f;
    private const float MAXIMAL_GRAVITY = 50f;
    private const float JUMP_FORCE = 7.5f;
    private bool gravityFromJump = false;

    private float deltaTime = 0f;

    private SpriteEffects flip = SpriteEffects.None;

    private int frame = 0;
    private int lastFrame = 0;
    private const float FRAME_TIME = 0.2f;
    private float timeToFrame = FRAME_TIME;

    private MouseState lastMouse;

    public Player(Vector2 position)
    {
        bool canGetTexture = TexturesRegistry.Textures.TryGetValue(TexturesRegistry.PLAYER, out texture);
        if (!canGetTexture)
            throw new Exception($"Cannot get texture of {Type.FilterName}");

        this.position = position;
    }

    public void Update(GameTime gameTime)
    {
        deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        GetInput();

        MoveHorizontal();
        MoveVertical();

        Animate();
    }

    private void GetInput()
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.D) && keyboard.IsKeyDown(Keys.A))
            velocity.X = 0f;
        else if (keyboard.IsKeyDown(Keys.D))
            velocity.X = SPEED;
        else if (keyboard.IsKeyDown(Keys.A))
            velocity.X = -SPEED;
        else
            velocity.X = 0f;

        if (keyboard.IsKeyDown(Keys.Space) && velocity.Y == 0f && !gravityFromJump)
        {
            velocity.Y = -JUMP_FORCE;
            gravityFromJump = true;
        }

        var mouse = Mouse.GetState();

        float scaleX = (float)Game1.PIXEL_SCREEN_WIDTH / Game1.Instance.Graphics.PreferredBackBufferWidth;
        float scaleY = (float)Game1.PIXEL_SCREEN_HEIGHT / Game1.Instance.Graphics.PreferredBackBufferHeight;

        var mouseScaled = new Vector2
        (
            mouse.X * scaleX,
            mouse.Y * scaleY
        );

        int gridSize = 8 * World.SIZE_MOD;
        var mouseCorrectedPosition = new Vector2
        (
            (int)((mouseScaled.X + WorldScene.Instance.Camera.Position.X) / gridSize),
            (int)((mouseScaled.Y + WorldScene.Instance.Camera.Position.Y) / gridSize)
        ) * gridSize;

        if (mouse.LeftButton == ButtonState.Pressed)
        {
            World.AddTile(TileID, mouseCorrectedPosition);
        }

        if (mouse.RightButton == ButtonState.Pressed)
        {
            World.RemoveTile(mouseCorrectedPosition);
        }

        if (mouse.ScrollWheelValue < lastMouse.ScrollWheelValue)
        {
            TileID++;
            if (TileID >= TilesRegistry.tiles.Length)
                TileID = 0;
        }
        else if (mouse.ScrollWheelValue > lastMouse.ScrollWheelValue)
        {
            TileID--;
            if (TileID < 0)
                TileID = TilesRegistry.tiles.Length - 1;
        }

        lastMouse = mouse;
    }

    private void MoveHorizontal()
    {
        var nextRectangle = flip == SpriteEffects.None
            ? new Rectangle((int)(position.X + hitboxNonFlipped.X + velocity.X), (int)position.Y,
            hitboxNonFlipped.Width * World.SIZE_MOD, hitboxNonFlipped.Height * World.SIZE_MOD)
            : new Rectangle((int)(position.X + hitboxFlipped.X + velocity.X), (int)position.Y,
            hitboxFlipped.Width * World.SIZE_MOD, hitboxFlipped.Height * World.SIZE_MOD);

        foreach (var tile in World.Tiles)
        {
            if (tile.Rectangle.Intersects(nextRectangle))
            {
                position.X = velocity.X < 0f
                    ? tile.Rectangle.Right
                    : tile.Rectangle.Left - PIXEL_WIDTH * World.SIZE_MOD;

                velocity.X = 0f;
                break;
            }
        }

        position.X += velocity.X;
    }

    private void MoveVertical()
    {
        if (velocity.Y < MAXIMAL_GRAVITY)
            velocity.Y += GRAVITY;

        var nextRectangle = flip == SpriteEffects.None
            ? new Rectangle((int)(position.X + hitboxNonFlipped.X), (int)(position.Y + velocity.Y),
            hitboxNonFlipped.Width * World.SIZE_MOD, hitboxNonFlipped.Height * World.SIZE_MOD)
            : new Rectangle((int)(position.X + hitboxFlipped.X), (int)(position.Y + velocity.Y),
            hitboxFlipped.Width * World.SIZE_MOD, hitboxFlipped.Height * World.SIZE_MOD);

        foreach (var tile in World.Tiles)
        {
            if (tile.Rectangle.Intersects(nextRectangle))
            {
                position.Y = velocity.Y < 0
                    ? tile.Rectangle.Bottom
                    : tile.Rectangle.Top - PIXEL_HEIGHT * World.SIZE_MOD;
                velocity.Y = 0f;
                gravityFromJump = false;
                break;
            }
        }

        position.Y += velocity.Y;
    }

    private void Animate()
    {
        if (velocity.X != 0f)
            flip = velocity.X > 0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        // if (velocity.Y != 0f)
        // {
        //     frame = 7;
        //     timeToFrame = 0.1f;
        // }

        if ((timeToFrame -= deltaTime) <= 0)
        {
            int current = frame;

            if (velocity.X == 0f)
            {
                frame = frame switch
                {
                    0 => 1,
                    1 => lastFrame == 0 ? 2 : 0,
                    _ => 1
                };
            }
            else
            {
                frame = frame switch
                {
                    3 => 4,
                    4 => 5,
                    5 => 6,
                    _ => 3
                };
            }

            lastFrame = current;

            timeToFrame = FRAME_TIME;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRectangle = new Rectangle
        (
            0, PIXEL_HEIGHT * frame,
            PIXEL_WIDTH, PIXEL_HEIGHT
        );

        spriteBatch.Draw(texture, new Rectangle((int)Position.X, (int)Position.Y,
        PIXEL_WIDTH * World.SIZE_MOD, PIXEL_HEIGHT * World.SIZE_MOD), sourceRectangle, Color.White,
        0f, Vector2.Zero, flip, 0f);
    }
}