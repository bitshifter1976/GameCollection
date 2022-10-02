using System;
using Microsoft.Xna.Framework;

namespace Framework
{
    [Serializable]
    public abstract class SpriteParallaxing : Sprite
    {
        #region members

        protected RectangleF[] destinationRects;

        #endregion

        #region methods

        protected SpriteParallaxing(string texture, float speed, int layer, CollisionType collType = CollisionType.None)
            : base(texture, new Vector2(0, 0), 0, 1f, layer, collType)
        {
            velocity.X = speed;
            destinationRects = new RectangleF[Manager.DesignWidth / this.texture.Width + 1];
            for (int i = 0; i < destinationRects.Length; i++)
            {
                destinationRects[i] = new RectangleF(i * this.texture.Width, 0, Manager.DesignWidth, Manager.DesignHeight);
            }
        }

        public override Sprite Update(GameTime time)
        {
            foreach (RectangleF r in destinationRects)
            {
                r.X += velocity.X;
                if (velocity.X <= 0)
                {
                    if (r.X <= -texture.Width)
                    {
                        r.X = texture.Width * (destinationRects.Length - 1) + (r.X % texture.Width);
                    }
                }
                else
                {
                    if (r.X >= texture.Width * (destinationRects.Length - 1))
                    {
                        r.X = -(texture.Width - (r.X % texture.Width));
                    }
                }
            }
            return null;
        }

        public override void Draw()
        {
            foreach (RectangleF r in destinationRects)
            {
                Manager.SpriteBatch.Draw(texture, r.ToRectangle(), Color.White);
            }
        }

        #endregion
    }
}
