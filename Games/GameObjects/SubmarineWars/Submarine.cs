using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars
{
    public class Submarine : Sprite
    {
        private bool isAi;
        private bool diving;
        private bool waterlineReached;
        private bool groundReached;
        private readonly float MinSpeed = -1.5f;
        private readonly float MaxSpeed = 5f;

        public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)position.Y, (int)Width, (int)(Height/2f));

        public float Speed
        {
            get => speed;
            set
            {
                speed = MathHelper.Clamp(value, MinSpeed, MaxSpeed);
                velocity = Vector2.Transform(new Vector2(1, 0), Matrix.CreateRotationZ(rotation)) * speed;
            }
        }

        private new float Rotation
        {
            get => rotation;
            set
            {
                rotation = MathHelper.Clamp(value, -0.3f, 0.3f);
                velocity = Vector2.Transform(new Vector2(1, 0), Matrix.CreateRotationZ(rotation)) * speed;
            }
        }

        public Submarine(Vector2 position, float rotation, float scale, bool isAi) : base("graphic/submarineWars/submarine", position, rotation, scale, (int)Layer.Submarine, CollisionType.BoundingBoxRotated)
        {
            flip = this.isAi = isAi;
            Energy = 100;
            if (isAi)
            {
                Speed = MinSpeed;
            }
        }

        public void Dive(bool up)
        {
            diving = true;
            var minY = Water.TopPixel[(int)position.X];
            var maxY = Water.GroundPixel[(int)position.X] - Height / 2f;
            var y = MathHelper.Clamp(up ? position.Y - 1 : position.Y + 1, minY, maxY);
            waterlineReached = (y == minY);
            groundReached = (y == maxY);
            position.Y = MathHelper.Clamp(y, minY, maxY);
            Rotation += up ? -0.005f : 0.005f;
        }

        public override Sprite Update(GameTime gameTime)
        {
            if (Energy <= 0)
            {
                SpriteManager.CreateExplosion(position, 1);
                return this;
            }
            // if computer enemy
            if (isAi)
            {
                if (Rand.Bool(1, 500))
                    Shoot();
                if (Rand.Bool(1, 50))
                    Rotation += Rand.Float(-0.05f, 0.05f);
                position += velocity;
            }
            // if water line reached, stay on top
            if (waterlineReached && speed != 0)
                position.Y = Water.TopPixel[(int)position.X];
            // if ground reached, stay on top
            else if (groundReached && speed != 0)
            {
                position.Y = Water.GroundPixel[(int)position.X] - Height / 2f;
                if (!Manager.Sound.IsEffectPlaying("fireBurn"))
                    Manager.Sound.PlayEffect("fireBurn");
                Energy -= 0.05f;
            }            
            // rotate back if not diving
            if (!isAi && !diving)
            {
                if (rotation < 0)
                    Rotation += 0.005f;
                else if (rotation > 0)
                    Rotation -= 0.005f;
            }
            diving = false;
            return base.Update(gameTime);
        }

        public void Shoot()
        {
            var right = new Vector2(1, 0);
            var rotMatrix = Matrix.CreateRotationZ(rotation + (flip ? MathHelper.ToRadians(180) : 0));
            var shotVelocity = Vector2.Transform(right, rotMatrix);
            var torpedo = new Torpedo(this, Vector2.Zero, shotVelocity, 0.5f, rotation, 500);
            shotVelocity.Normalize();
            torpedo.Position = new Vector2(position.X, position.Y + 10) + shotVelocity * Width / 2f;
            SpriteManager.Add(torpedo);
        }
    }
}
