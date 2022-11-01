using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny
{
    public class FloorTile : Sprite
    {
        private readonly FloorTileProps tile;
        private readonly Texture2D left;
        private readonly Texture2D right;
        public new float Width;

        public int TopOffset => (int)(10*scale);

        public override Rectangle BoundingBox => new((int)position.X, (int)position.Y+TopOffset, (int)Width, (int)Height-TopOffset);

        public FloorTile(Vector2 position, float scale, FloorTileProps tile) : base("graphic/mrSunny/" + tile.name, position, 0f, scale, (int)Layer.Beach, CollisionType.BoundingBox)
        {
            this.tile = tile;
            scrolling = true;
            Width = tile.width*scale;
            if (tile.leftEnd)
            {
                left = Manager.Content.Load<Texture2D>("graphic/mrSunny/" + tile.left);
                Width += left.Width * scale;
            }
            if (tile.rightEnd)
            {
                right = Manager.Content.Load<Texture2D>("graphic/mrSunny/" + tile.right);
                Width += right.Width * scale;
            }
        }

        public override void Draw()
        {
            Manager.SpriteBatch.Draw(texture, Position, null, tile.color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            if (tile.leftEnd)
                Manager.SpriteBatch.Draw(left, new Vector2(Position.X - 10*scale, Position.Y), null, tile.color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            if (tile.rightEnd)
                Manager.SpriteBatch.Draw(right, new Vector2(Position.X + texture.Width*scale, Position.Y), null, tile.color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }

    public class FloorTileProps
    {
        public string name;
        public string left;
        public string right;
        public int width;
        public Color color;
        public bool leftEnd = false;
        public bool rightEnd = false;

        public FloorTileProps(string name, string left, string right, int width)
        {
            this.name = name;
            this.left = left;
            this.right = right;
            this.width = width;
            this.color = Color.White;
        }

        public FloorTileProps Clone()
        {
            var t = new FloorTileProps(this.name, this.left, this.right, this.width);
            t.leftEnd = this.leftEnd;
            t.rightEnd = this.rightEnd;
            t.color = this.color;
            return t;
        }
    }
}
