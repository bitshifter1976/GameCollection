using AxeGameCollection.GameObjects.TankBattle;
using Framework;
using Microsoft.Xna.Framework;
using System;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars
{
    public class Submarine : Sprite
    {
        private readonly bool isAi;
        private bool waterlineReached;
        private readonly float MinSpeed = -5f;
        private readonly float MaxSpeed = 5f;

        public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)position.Y, (int)Width, (int)(Height/2f));
        public override bool OnScreen => (BoundingBox.Right >= 0) && (BoundingBox.Left <= Manager.DesignWidth) && (BoundingBox.Bottom >= 0) && (BoundingBox.Top <= Manager.DesignHeight);

        public float Speed
        {
            get => speed;
            private set
            {
                speed = MathHelper.Clamp(value, MinSpeed, MaxSpeed);
                velocity = Vector2.Transform(new Vector2(1, 0), Matrix.CreateRotationZ(rotation)) * speed;
                if (speed != 0)
                    flip = speed < 0;
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

        public bool IsAi 
        { 
            get => isAi;
        }

        public static Submarine Create(int possibility, bool createEnemy)
        {
            var scale = 1;
            Submarine sub = null;
            if (Rand.Bool(1, possibility))
                sub = createEnemy ? CreateEnemy(scale) : CreatePlayer(scale);
            return sub;
        }

        private static Submarine CreatePlayer(int scale)
        {
            return new Submarine(new Vector2(Manager.DesignWidth / 2f, Manager.DesignHeight / 2f), 0, scale, false);
        }

        private static Submarine CreateEnemy(float scale)
        {
            var sub = new Submarine(Vector2.Zero, 0, scale, true);
            var posX = -sub.Width / 2f;
            if (Rand.Bool(1, 2))
                posX = Manager.DesignWidth + sub.Width / 2f;
            var posY = Rand.Float(Water.MaxYTopPixel + sub.Height, Water.MinYGroundPixel - sub.Height);
            sub.Position = new Vector2(posX, posY);
            return sub;
        }

        public Submarine(Vector2 position, float rotation, float scale, bool isAi) : base("graphic/submarineWars/submarine", position, rotation, scale, (int)Layer.Submarine, CollisionType.BoundingBoxRotated)
        {
            origin = new Vector2(Width / 2f, Height / 2f);
            flip = isAi;
            this.isAi = isAi;
            Energy = 100;
            if (isAi)
                scrolling = true;
        }

        public void Dive(bool up)
        {
            var minY = Water.TopPixel[(int)position.X];
            var maxY = Water.GroundPixel[(int)position.X] - Height / 2f;
            var y = MathHelper.Clamp(up ? position.Y - 1 : position.Y + 1, minY, maxY);
            position.Y = y;
            waterlineReached = (y == minY);
            if (Speed > 0)
                AddRotation(up ? -0.005f : 0.005f);
            if (Speed < 0)
                AddRotation(up ? 0.005f : -0.005f);
        }

        public void AddSpeed(float delta)
        {
            Speed += delta;
        }

        public void SpeedDown(float delta)
        {
            var newSpeed = speed;
            if (newSpeed > 0)
                newSpeed -= delta;
            if (newSpeed < 0)
                newSpeed += delta;
            if (Math.Abs(newSpeed) < 0.001f)
                newSpeed = 0;
            Speed = newSpeed;
        }

        public void AddRotation(float delta)
        {
            Rotation += delta;
        }

        public void RotateBack(float delta)
        {
            var newRotation = rotation;
            if (newRotation < 0)
                newRotation += delta;
            if (newRotation > 0)
                newRotation -= delta;
            if (Math.Abs(newRotation) < 0.001f)
                newRotation = 0;
            Rotation = newRotation;
        }

        public override Sprite Update(GameTime gameTime)
        {
            // if computer enemy
            if (IsAi)
            {
                if (Rand.Bool(1, 50))  AddRotation(Rand.Float(-0.05f, 0.05f));
                if (Rand.Bool(1, 100)) AddSpeed(Rand.Float(-0.5f, 0.5f));
                if (Rand.Bool(1, 300)) Shoot();
                position -= velocity;
            }
            // if water line reached, stay on top
            if (waterlineReached && speed != 0)
            {
                position.Y = Water.TopPixel[(int)position.X];
            }
            return null;
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
