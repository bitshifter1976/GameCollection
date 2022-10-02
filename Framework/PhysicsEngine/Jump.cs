using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Framework
{
    public class Jump
    {
        private int height;
        private float startY;
        private float gravity;
        private bool falling = false;
        private Vector2 position;
        private float mass;
        public  Vector2 Velocity;
        bool active = true;

        public bool Falling
        {
            get { return falling && active; }
            set 
            {
                if (falling != value)
                {
                    falling = value;
                    gravity = 0;
                    Velocity.Y = 0;
                }
            }
        }

        public Jump(Vector2 position, Vector2 velocity, int height, float mass)
        {
            this.position = position;
            this.Velocity = new Vector2(velocity.X, 0);
            this.height = height;
            this.mass = mass;
            gravity = 1;
            startY = position.Y;
        }

        public Vector2 Update(GameTime time)
        {
            if (active)
            {

                if (falling)
                {
                    gravity += Physics.Gravity * mass * (float)time.ElapsedGameTime.TotalSeconds;
                    Velocity.Y += gravity;
                }
                else
                {
                    gravity -= Physics.Gravity * 4f * mass * (float)time.ElapsedGameTime.TotalSeconds;
                    Velocity.Y -= gravity;
                }
                // changing jump direction from up to down
                if (position.Y < startY - height && !falling)
                {
                    falling = true;
                    Velocity.Y = 0;
                    gravity = 0;
                }
                position.Y += Velocity.Y;
            }

            // going down
            return Velocity;
        }

        public void End()
        {
            active = false;
            Velocity = new Vector2(0, 0);
        }
    }
}
