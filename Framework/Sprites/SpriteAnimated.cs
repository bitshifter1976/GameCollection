using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework
{
    [Serializable]
    public class SpriteAnimated : Sprite
    {
        #region members

        private float timeElapsed;
        protected int FrameIndex;
        protected int FrameCount;
        // default to 20 frames per second
        private readonly float timeToUpdate = 0.05f;
        protected int SpriteWidth;

        #endregion

        #region properies

        public override float Width
        {
            get { return SpriteWidth*scale; }
        }

        #endregion

        #region methods

        public SpriteAnimated(string texture, Vector2 position, int frameCount, float framesPerSecond, float rotation, float scale, int layer, CollisionType collType = CollisionType.None) : base(texture, position, rotation, scale, layer, collType)
        {
            FrameCount = frameCount;
            timeToUpdate = 1 / framesPerSecond;
            SpriteWidth = this.texture.Width / frameCount;
        }

        public override Sprite Update(GameTime gameTime)
        {
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timeElapsed > timeToUpdate)
            {
                timeElapsed = 0;
                FrameIndex++;
                if (FrameIndex == FrameCount)
                    FrameIndex = 0;
            }
            return base.Update(gameTime);
        }

        public override void Draw()
        {
            var src = new Rectangle(texture.Width / FrameCount * FrameIndex, 0, texture.Width / FrameCount, texture.Height);
			Manager.SpriteBatch.Draw(texture, Position, src, color, rotation, origin, scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        #endregion
    }
}
