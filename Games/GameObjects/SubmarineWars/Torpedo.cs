using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public class Torpedo : Sprite
{
    private Submarine submarine;

    public Submarine Submarine { get => submarine; }

    public Torpedo(Submarine submarine, Vector2 position, Vector2 velocity, float scale, float rotation, float speed) : base("graphic/submarineWars/torpedo", position, rotation, scale, (int)Layer.Shot, CollisionType.BoundingBoxRotated)
    {
        this.submarine = submarine;
        this.velocity = velocity;
        this.speed = speed;
        flip = velocity.X < 0;
        damage = 33;
    }

    public override Sprite Update(GameTime gameTime)
    {
        position += velocity * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        return base.Update(gameTime);
    }
}
