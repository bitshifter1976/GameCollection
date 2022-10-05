using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.SubmarineWars
{
    public class Submarine : Sprite
    {
        private bool diving;
        private bool waterlineReached;
        private bool groundReached;
        private readonly float MinSpeed = -2.5f;
        private readonly float MaxSpeed = 5f;

        public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)position.Y, (int)Width, (int)(Height/2f));

        public float Speed
        {
            get => speed;
            set => speed = MathHelper.Clamp(value, MinSpeed, MaxSpeed);
        }

        public Submarine(Vector2 position, float rotation, float scale) : base("graphic/submarineWars/submarine", position, rotation, scale, (int)Layer.Submarine, CollisionType.BoundingBoxRotated)
        {
        }

        public void Dive(bool up)
        {
            diving = true;
            var minY = Water.TopPixel[(int)position.X];
            var maxY = Water.GroundPixel[(int)position.X] - Height/2f;
            var y = MathHelper.Clamp(up ? position.Y - 1 : position.Y + 1, minY, maxY);
            waterlineReached = (y == minY);
            groundReached = (y == maxY);
            position.Y = MathHelper.Clamp(y, minY, maxY);
            rotation += up ? -0.005f : 0.005f;
            rotation = MathHelper.Clamp(rotation, -0.2f, 0.2f);
        }

        public override Sprite Update(GameTime gameTime)
        {
            // if water line reached, stay on top
            if (waterlineReached)
                position.Y = Water.TopPixel[(int)position.X];
            // if ground reached, stay on top
            if (groundReached)
                position.Y = Water.GroundPixel[(int)position.X] - Height/2f;
            // rotate back if not diving
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
