using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework
{
    [Serializable]
    public class SpriteAnimatedMultiLine : Sprite
    {
        protected float TimeElapsed;
        protected int FrameIndex;
        protected int RowIndex;
        protected int FrameCount;
        protected int RowCount;
        protected float TimeToUpdate = 0.05f;
        protected int SpriteWidth, SpriteHeight;
        protected int FramesPerRow;
        private bool loop;

        public override float Width
        {
            get { return SpriteWidth * scale; }
        }

        public override float Height
        {
            get { return SpriteHeight * scale; }
        }

        public SpriteAnimatedMultiLine(string texture, Vector2 position, int frameCount, int rowCount, float framesPerSecond, float rotation, float scale, int layer, bool loop = true, CollisionType collType = CollisionType.None) : 
            base(texture, position, rotation, scale, layer, collType)
        {
            FrameCount = frameCount;
            TimeToUpdate = 1 / framesPerSecond;
            FramesPerRow = frameCount / rowCount;
            SpriteWidth = this.texture.Width / FramesPerRow;
            SpriteHeight = this.texture.Height / rowCount;
            RowCount = rowCount;
            this.loop = loop;
        }

        public override Sprite Update(GameTime gameTime)
        {
            TimeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (TimeElapsed >= TimeToUpdate)
            {
                TimeElapsed -= TimeToUpdate;

                if (FrameIndex < FramesPerRow - 1)
                {
                    FrameIndex++;
                }
                else
                {
                    FrameIndex = 0;
                    if (RowIndex < RowCount - 1)
                        RowIndex++;
                    else if (loop)
                        RowIndex = 0;
                    else
                        return this;
                }
            }

            return null;
        }

        public override void Draw()
        {
            var src = new Rectangle(SpriteWidth * FrameIndex, SpriteHeight * RowIndex, SpriteWidth, SpriteHeight);
            Manager.SpriteBatch.Draw(texture, Position, src, Color.White, rotation, origin, scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            if (Manager.Debug)
            {
                Debug.Rects.Add(new RectDebug(BoundingBoxRotated.ToLines(), "BoundingBox", Color.Red));
                Debug.Points.Add(new PointDebug(new Dot(Center), "Center", Color.Gold));
                Debug.Points.Add(new PointDebug(new Dot(position), "Position", Color.Green));
                Manager.SpriteBatch.DrawString(Manager.Fonts.Get("Debug"), $"{position}({this.GetType().Name})", new Vector2(position.X, position.Y + Height + 2), Color.Black);
            }
        }
    }
}
