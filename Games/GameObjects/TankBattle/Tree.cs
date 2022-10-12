using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameTankBattle;

namespace AxeGameCollection.GameObjects.TankBattle;

public sealed class Tree : Sprite
{
    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)(position.Y - Height / 2f), (int)Width, (int)Height);

    public Tree(Vector2 position, float scale) : base("graphic/tankBattle/tree", position, 0, scale, Color.White, (int)Layer.Trees, CollisionType.BoundingBox)
    {
        Position += new Vector2(-Width/2f,-Height/2f);
    }

    public override Sprite Update(GameTime gameTime)
    {
        return null;
    }
}
