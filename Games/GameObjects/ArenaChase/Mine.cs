using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.ArenaChase
{
    public sealed class Mine : Sprite
    {
        public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)(position.Y - Height / 2f), (int)Width, (int)Height);
        public Mine(Vector2 position, float scale, float rotation) : base("graphic/mine", position, rotation, scale, (int)Layer.Cannon-1, CollisionType.BoundingBox)
        {
            damage = 35;
        }

        public override Sprite Update(GameTime gameTime)
        {
            return null;
        }
    }
}
