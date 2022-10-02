using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework
{
    [Serializable]
    public class Animation
    {
        public Rectangle[] Rectangles;
        public Color Color = Color.White;
        public Vector2 Origin = Vector2.Zero;
        public float Rotation;
        public float Scale = 1f;
        public SpriteEffects SpriteEffect;
        public bool IsLooping = true;
        public int Frames;
        public float TimeElapsed;
        public float TimeToUpdate = 0.05f;

        public float Fps
        {
            get { return 1 / TimeToUpdate; }
            set { TimeToUpdate = 1 / value; }
        }

        public Animation Copy()
        {
        	return new Animation
        	{
        	    Rectangles = Rectangles,
        	    Color = Color,
        	    Origin = Origin,
        	    Rotation = Rotation,
        	    Scale = Scale,
        	    SpriteEffect = SpriteEffect,
        	    IsLooping = IsLooping,
        	    Frames = Frames,
        	    TimeElapsed = TimeElapsed,
        	    TimeToUpdate = TimeToUpdate
        	};
        }
    }
}
