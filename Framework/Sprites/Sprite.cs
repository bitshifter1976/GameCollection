using System;
using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework
{
    [Serializable]
    public abstract class Sprite : IComparable<Sprite>
    {
        private static long     globalIdx = 0;
        protected float         rotation;
        protected float         scale    = 1f;
        [NonSerialized]
        protected Texture2D     texture;
        protected Vector2       position = new(0,0);
        protected CollisionType collisionType = CollisionType.None;
        protected Vector2       origin = Vector2.Zero;
        protected bool          scrolling;
        protected Color         color = Color.White;
    	protected int			layer;
		protected float			energy;
        protected int           damage;
        protected int           cure;
        protected Vector2       velocity = new(0, 0);
        protected float         mass;
    	protected bool          flip;
        protected float         friction;
        protected float         speed;

		public long Idx { get; set; }
    	private long SortValue => 100000000000000000 + layer * 100000000000000 + Idx;
        public virtual float Width => (int)(texture.Width*scale);
        public virtual float Height => (int)(texture.Height*scale);
        public virtual Rectangle BoundingBox => new((int)position.X, (int)position.Y, (int)Width, (int)Height);
        public virtual RectangleF BoundingBoxF => new(BoundingBox);
        public virtual RotatedRectangle BoundingBoxRotated => new(BoundingBoxF, rotation);
        public virtual CollisionType CollisionType
        {
            get => collisionType;
            set => collisionType = value;
        }
        public virtual Vector2 Center
        {
            get => BoundingBoxRotated.Center;
            set
            {
                position.X = value.X - Width / 2;
                position.Y = value.Y - Height / 2;
            }
        }
        public virtual Vector2 Position
        {
            get => position;
            set => position = value;
        }
        public virtual Color Color
        {
            get => color;
            set => color = value;
        }
        public virtual string Image
        {
            get => texture.Name;
            set => texture = Manager.Content.Load<Texture2D>("graphic/" + value);
        }
        public virtual bool Flip
        {
            get => flip;
            set => flip = value;
        }
        public virtual Texture2D Texture
        {
            get => texture;
            set => texture = value;
        }
        public virtual int Damage => damage;
        public virtual float Energy
        {
            get => energy;
            set => energy = value;
        }
        public virtual int Cure => cure;
        public virtual float Scale
        {
            get => scale;
            set => scale = value;
        }
        public virtual bool OnScreen => (position.X + Width >= 0) && (position.X <= Manager.DesignWidth) && (position.Y + Height >= 0) && (position.Y <= Manager.DesignHeight);
        public float Rotation
        {
            get => rotation;
            set => rotation = value;
        }
        public Vector2 Velocity
        {
            get => velocity;
            set => velocity = value;
        }
        public float Mass
        {
            get => mass;
            set => mass = value;
        }
        public float Friction
        {
            get => friction;
            set => friction = value;
        }

		protected Sprite()
		{
		}

        protected Sprite(int layer)
        {
            this.layer = layer;
        }

        protected Sprite(string texture, int layer, CollisionType collType) : this(texture, new Vector2(0,0), 0f, 1f, layer, collType)
        {
        }

        protected Sprite(string texture, Vector2 position, float rotation, float scale, int layer, CollisionType collType) : this(texture, position, rotation, scale, Color.White, layer, collType)
        {
        }

        protected Sprite(string texture, Vector2 position, float rotation, float scale, Color color, int layer, CollisionType collType) : this(Manager.Content.Load<Texture2D>(texture), position, rotation, scale, color, layer, collType)
        {
        }

        protected Sprite(Texture2D texture, Vector2 position, float rotation, float scale, Color color, int layer, CollisionType collType)
        {
            this.texture = texture;
            Idx = globalIdx++;
            this.position = position;
            this.origin = new Vector2(Width / 2f, Height / 2f);
            this.rotation = rotation;
            this.scale = scale;
            this.color = color;
            this.layer = layer;
            this.collisionType = collType;
        }

        public virtual Sprite Update(GameTime gameTime)
        {
            return scrolling || OnScreen ? null : this;
        }

        public virtual void Draw()
        {
            Manager.SpriteBatch.Draw(texture, position, null, color, rotation, origin, scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            if (Manager.Debug)
            {
                Debug.Rects.Add(new RectDebug(BoundingBoxRotated.ToLines(), "BoundingBox", Color.Red));
                Debug.Points.Add(new PointDebug(new Dot(Center), "Center", Color.Gold));
                Debug.Points.Add(new PointDebug(new Dot(position), "Position", Color.Green));
                Manager.SpriteBatch.DrawString(Manager.Fonts.Get("Debug"),$"{position}({this.GetType().Name})", new Vector2(position.X, position.Y + Height + 2), Color.Black);
            }
        }

        public virtual void ScrollX(float speed)
        {
            if (scrolling)
            {
                position.X += speed;
                velocity.X = speed;
            }
        }

        public virtual void ScrollY(float speed)
        {
            if (scrolling)
            { 
                position.Y += speed;
                velocity.Y = speed;
            }
        }

        public virtual void Scroll(Vector2 speed)
        {
            if (scrolling)
            {
                position += speed;
                velocity = speed;
            }
        }

        public virtual void BounceBack(float distance)
        {
            if (velocity.Length() > 0)
            {
                position += -velocity / velocity.Length() * distance;
                speed = 0;
            }
        }

        public int CompareTo(Sprite other)
        {
			return SortValue.CompareTo(other.SortValue);
        }

        public virtual bool Collide(Sprite s)
        {
            return Collision.Do(this, s);
        }

        public virtual RotatedRectangle GetBoundingBox(Vector2 pos)
        {
            return new RotatedRectangle(new RectangleF(pos.X, pos.Y, Width, Height), rotation);
        }

        public virtual RectangleF GetScaledBB(Vector2 pos, Vector2 worldScale)
        {
            var deltaX = BoundingBox.X*worldScale.X - pos.X;
            var deltaY = BoundingBox.Y*worldScale.Y - pos.Y;
            return new RectangleF(BoundingBox.X-deltaX, BoundingBox.Y-deltaY, BoundingBox.Width*worldScale.X, BoundingBox.Height*worldScale.Y);
        }

        public virtual float GetScaledWidth(float worldScaleX)
        {
            return Width * worldScaleX;
        }

        public virtual float GetScaledHeight(float worldScaleY)
        {
            return Height * worldScaleY;
        }

        public virtual Vector2 GetScaledPosition(Vector2 worldScale)
        {
            return new Vector2(position.X*worldScale.X, position.Y*worldScale.Y);
        }

        public virtual Vector2 GetScaledCenterPosition(Vector2 worldScale)
        {
            return new Vector2(Center.X * worldScale.X, Center.Y * worldScale.Y);
        }

        public virtual Vector2 GetScaledVelocity(Vector2 worldScale)
        {
            return new Vector2(Velocity.X * worldScale.X, Velocity.Y * worldScale.Y);
        }

        public virtual Sprite ReactToCollision(Sprite sprite)
        {
            return null;
        }
    }
}
