using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameArenaChase;

namespace AxeGameCollection.GameObjects.ArenaChase;

public sealed class Shot : Sprite
{
    public Tank Tank { get; }
    public Circle Circle { get; }
    public Circle DamageCircle { get; }

    public Shot(Tank tank, Vector2 position, Vector2 velocity, float scale) : base("graphic/common/shot", position, 0, scale, (int)Layer.Shot, CollisionType.BoundingBox)
    {
        Tank = tank;
        this.velocity = velocity;
        flip = false;
        Circle = new Circle(Center, Width / 2f);
        DamageCircle = new Circle(Center, Width);
        damage = 20;
    }

    public override Sprite Update(GameTime gameTime)
    {
        position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Circle.Center = Center;
        DamageCircle.Center = Center;
        return OnScreen ? null : this;
    }
}
