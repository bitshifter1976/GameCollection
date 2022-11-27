using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Security.Policy;

namespace Framework
{
    public class Particle : Sprite
    {
        public float RotationSpeed { get; set; }
        public float ShrinkFactor { get; set; }
        public float TimeToLive { get; set; }
        public float Gravity { get; set; }
        public Rectangle SourceRect { get; set; }
        public Vector2 Origin { get; set; }

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity, float rotation, float rotationSpeed, Color color, int layer, float scale, float timeToLiveSec, float gravity, float shrinkFactor, bool useGravity = true, bool scrolling = false)
            : base(texture, position, rotation, scale, color, layer, CollisionType.None)
        {
            Velocity = velocity;
            RotationSpeed = rotationSpeed;
            TimeToLive = timeToLiveSec;
            if (useGravity)
                Gravity = gravity;
            ShrinkFactor = shrinkFactor;
            SourceRect = new Rectangle(0, 0, Texture.Width, Texture.Height);
            this.scrolling = scrolling;
        }

        public override Sprite Update(GameTime time)
        {
            TimeToLive -= (float)time.ElapsedGameTime.TotalSeconds;
            velocity.Y += Gravity;
            Position += Velocity;
            Rotation += RotationSpeed;
            Scale -= ShrinkFactor;
            if (Scale <= 0 || TimeToLive <= 0)
            {
                Scale = 0;
                TimeToLive = 0;
                return this;
            }
            return OnScreen ? null : this;
        }

        public override void Draw()
        {
            Manager.SpriteBatch.Draw(Texture, Position, SourceRect, Color, Rotation, Origin, Scale, SpriteEffects.None, 0);
        }
    }
}

