using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Framework
{
    [Serializable]
    public abstract class SpriteMultipleAnimated : Sprite
    {
        #region members

        private string animation;
        protected Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
        protected int frameCount;
        protected int frameIndex;
        protected int spriteHeight;
        protected int spriteWidth;

        #endregion

        #region properties

        public string Animation
        {
            get { return animation; }
            set
            {
                if (animation != value)
                {
                    animation = value;
                    frameIndex = 0;
                }
            }
        }

        public override Rectangle BoundingBox
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, spriteWidth, spriteHeight); }
        }

        public override float Width
        {
            get { return spriteWidth * scale; }
        }

        public override float Height
        {
            get { return spriteHeight * scale; }
        }

        #endregion

        #region methods

        protected SpriteMultipleAnimated(string texture, Vector2 position, int frameCount, int animationCount, float rotation, float scale, int layer, CollisionType collType = CollisionType.None)
            : base(texture, position, rotation, scale, layer, collType)
        {
            this.frameCount = frameCount;
            int framesPerRow = frameCount / animationCount;
            spriteWidth = this.texture.Width / framesPerRow;
            spriteHeight = this.texture.Height / animationCount;
        }

        public void AddAnimation(string name, int row, int frames, Animation animation)
        {
            var recs = new Rectangle[frames];
            for (int i = 0; i < frames; i++)
            {
                recs[i] = new Rectangle(i * spriteWidth, (row - 1) * spriteHeight, spriteWidth, spriteHeight);
            }
            animation.Frames = frames;
            animation.Rectangles = recs;
            animation.Scale = scale;
            animations.Add(name, animation);
        }

        public override Sprite Update(GameTime gameTime)
        {
            animations[animation].TimeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (animations[animation].TimeElapsed > animations[animation].TimeToUpdate)
            {
                animations[animation].TimeElapsed -= animations[animation].TimeToUpdate;
                if (frameIndex < animations[Animation].Frames - 1)
                    frameIndex++;
                else if (animations[Animation].IsLooping)
                    frameIndex = 0;
            }

            return base.Update(gameTime);
        }

        public override void Draw()
        {
            Manager.SpriteBatch.Draw(texture, Position,
                                     animations[animation].Rectangles[frameIndex],
                                     animations[animation].Color,
                                     animations[animation].Rotation,
                                     animations[animation].Origin,
                                     animations[animation].Scale,
                                     animations[animation].SpriteEffect,
                                     0);

            if (Manager.Debug)
            {
                Debug.Rects.Add(new RectDebug(BoundingBoxRotated.ToLines(), "BoundingBox", Color.Red));
                Debug.Points.Add(new PointDebug(new Dot(Center), "Center", Color.Gold));
                Debug.Points.Add(new PointDebug(new Dot(position), "Position", Color.Green));
                Manager.SpriteBatch.DrawString(Manager.Fonts.Get("Debug"), $"{position}({this.GetType().Name})", new Vector2(position.X, position.Y + Height + 2), Color.Black);
            }
        }

        #endregion
    }
}