using Microsoft.Xna.Framework;

namespace Framework
{
    public enum GravityType
    {
        Down,
        UpDown
    }

    public class Gravity
    {
        private Vector2 velocity;
        private float gravity = 0;
        private GravityType type;
        private float mass;

        public Vector2 Velocity
        {
            get { return velocity; }
        }

        public Gravity(GravityType type, Vector2 velocity, float mass)
        {
            this.type = type;
            this.velocity = velocity;
            this.mass = mass;
        }

        public Vector2 Update(GameTime gameTime)
        {
            float oldGravity = gravity;
            gravity += Physics.Gravity * mass * (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity.Y += gravity;
            if ((oldGravity >= 0 && gravity < 0) || (oldGravity < 0 && gravity >= 0))
                gravity = 0;
            return velocity;
        }

        public override string ToString()
        {
            return velocity.ToString();
        }
    }
}
