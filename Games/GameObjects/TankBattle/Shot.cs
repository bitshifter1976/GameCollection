using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.TankBattle;

public class Shot : Sprite
{
    private Gravity gravity;
    private Tank tank;
    private Circle circle;
    private Circle damageCircle;
    public int DamageIndirect { get; }

    public Tank Tank { get => tank; }
    public Circle Circle { get => circle; }

    public Circle DamageCircle { get => damageCircle; }

    public Shot(Tank tank, Vector2 position, Vector2 velocity) : base("graphic/common/shot", position, 0, 1f, 1, CollisionType.BoundingBox)
    {
        this.tank = tank;
        gravity = new Gravity(GravityType.UpDown, velocity, 0.1f);
        flip = false;
        circle = new Circle(Center, Width / 2f);
        damageCircle = new Circle(Center, 50);
        damage = 33;
        DamageIndirect = 12;
    }

    public override Sprite Update(GameTime gameTime)
    {
        velocity = gravity.Update(gameTime);
        position += velocity + Wind.Direction * (float)gameTime.ElapsedGameTime.TotalSeconds;
        circle.Center = new Vector2(position.X, position.Y);
        damageCircle.Center = new Vector2(position.X, position.Y);

        return null;
    }
}
