using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class Tile : GameObject
{
    public bool CanCollide { get; protected set; }
    public Rectangle Hitbox { get { return CanCollide ? Rectangle : Rectangle.Empty; } }

    private static int totalTiles = 0;
    public int TileWorldID { get; private set; }
    public Tiles TileType { get; private set; }
    public DropData DropData { get; private set; }
    public ToolToDestroy ToolToDestroy { get; private set; }

    private readonly float takeDamageTime;
    private float timeToTakeDamage;
    private int health = 4;
    private int destroyingFrame;
    private float deltaTime;
    private int minimalToolPower;

    private static SoundEffectInstance Damage;
    private const float DAMAGE_SFX_COOLDOWN = 0.15f;
    private static float damageSfxCooldown = DAMAGE_SFX_COOLDOWN;
    private static SoundEffectInstance Destroy;

    public Tile(Texture2D texture, Vector2 position, Vector2 size, bool canCollide, Tiles tileType,
    ToolToDestroy toolToDestroy, float destroyTime, int minimalToolPower, DropData drop) : base(texture, position, size)
    {
        CanCollide = canCollide;
        TileWorldID = ++totalTiles;
        TileType = tileType;
        DropData = drop;
        ToolToDestroy = toolToDestroy;
        takeDamageTime = destroyTime / health;
        destroyingFrame = health;
        this.minimalToolPower = minimalToolPower;
    }

    public static void SetupSFX(SoundEffect damage, SoundEffect destroy)
    {
        Damage = damage.CreateInstance();
        Destroy = destroy.CreateInstance();
    }

    public override void Update(GameTime gameTime)
    {
        deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (health <= 0)
        {
            Drop();
            Destroy.Play();
            World.RemoveTile(this);
        }
    }

    public static void StaticUpdate(GameTime gameTime)
    {
        damageSfxCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public void TakeDamage(float speed, int power)
    {
        if (power < minimalToolPower)
            return;
            
        if ((timeToTakeDamage -= deltaTime * speed) <= 0)
        {
            if (damageSfxCooldown <= 0 && health > 1)
            {
                Damage.Play();
                damageSfxCooldown = DAMAGE_SFX_COOLDOWN;
            }

            health -= 1;
            timeToTakeDamage = takeDamageTime;
            destroyingFrame = health;
        }
    }

    public void Drop()
    {
        var drop = DropData.CalculateDrop();

        foreach (var dropItem in drop)
            for (int i = 0; i < dropItem.Amount; i++)
                World.AddItem(new DroppedItem(dropItem.Item, Position));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.Draw(ItemsBank.DestroyingAtlas.Texture,
        Rectangle, ItemsBank.DestroyingAtlas.Rectangles[destroyingFrame], Color.White);
    }
}