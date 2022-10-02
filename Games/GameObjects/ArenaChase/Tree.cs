using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.ArenaChase;

public sealed class Tree : Sprite
{
    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)(position.Y - Height / 2f), (int)Width, (int)Height);

    public Tree(Vector2 position, float scale) : base("graphic/treeFromTop", position, MathHelper.ToRadians(359), scale, Color.White, (int)Layer.Trees, CollisionType.BoundingBox)
    {
    }

    public override Sprite Update(GameTime gameTime)
    {
        return null;
    }
}
