using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public class Torpedo : Sprite
{
    private Submarine submarine;

    public Submarine Submarine { get => submarine; }

    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)(position.Y - Height / 2f), (int)Width, (int)Height);

    public Torpedo(Submarine submarine, Vector2 position, Vector2 velocity, float scale, float rotation, float speed) : base("graphic/submarineWars/torpedo", position, rotation, scale, (int)Layer.Shot, CollisionType.BoundingBoxRotated)
    {
        this.submarine = submarine;
        this.velocity = velocity;
        this.speed = speed;
        flip = velocity.X < 0;
        damage = 100;
    }

    public override Sprite Update(GameTime gameTime)
    {
        position += velocity * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        return base.Update(gameTime);
    }
}
