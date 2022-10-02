using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.TankBattle;

public sealed class Moon : Sprite
{
    public Moon(Vector2 position, float speed, float scale) : base("graphic/tankBattle/moon", position, 0f, scale, (int)Layer.Planet, CollisionType.None)
    {
        Velocity = new Vector2(speed, 0);
    }

    public override Sprite Update(GameTime gameTime)
    {
        position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        return base.Update(gameTime);
    }
}
