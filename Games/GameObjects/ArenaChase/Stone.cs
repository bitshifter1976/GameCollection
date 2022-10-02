using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.ArenaChase;

public sealed class Stone : Sprite
{
    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)(position.Y - Height / 2f), (int)Width, (int)Height);
    public Stone(Vector2 position, float scale) : base("graphic/stone", position, Rand.Float(0f,MathHelper.ToRadians(359)), scale, (int)Layer.Trees, CollisionType.BoundingBoxRotated)
    {
    }

    public override Sprite Update(GameTime gameTime)
    {
        return null;
    }
}
