using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.SubmarineWars
{
    public class Submarine : Sprite
    {
        private bool diving;

        public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)position.Y, (int)Width, (int)(Height/2f));

        public Submarine(Vector2 position, float rotation, float scale) : base("graphic/submarineWars/submarine", position, rotation, scale, (int)Layer.Submarine, CollisionType.BoundingBoxRotated)
        {
        }

        public void Dive(bool up)
        {
            diving = true;
            position = new Vector2(position.X, up ? position.Y-1 : position.Y+1);
            rotation += up ? -0.005f : 0.005f;
            rotation = MathHelper.Clamp(rotation, -0.2f, 0.2f);
        }

        public override Sprite Update(GameTime gameTime)
        {
            if (!diving)
            {
                if (rotation < 0)
                    rotation += 0.005f;
                else if (rotation > 0)
                    rotation -= 0.005f;
            }
            diving = false;
            return base.Update(gameTime);
        }
    }
}
