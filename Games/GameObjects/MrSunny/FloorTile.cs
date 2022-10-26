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
        public  static int TopOffset = 20;
        private RectangleF boundingBox;

        public FloorTile(Vector2 position, float scale, FloorTileProps tile) : base("graphic/mrSunny/" + tile.name, position, 0f, scale, (int)Layer.Beach, CollisionType.BoundingBox)
        {
            this.tile = tile;
            scrolling = true;
            if (tile.leftEnd)
                left = Manager.Content.Load<Texture2D>("graphic/mrSunny/" + tile.left);
            if (tile.rightEnd)
                right = Manager.Content.Load<Texture2D>("graphic/mrSunny/" + tile.right);
        }

        public void SetBoundingBox(RectangleF bb)
        {
            boundingBox = bb;
        }

        public override void Draw()
        {
            Manager.SpriteBatch.Draw(texture, Position, tile.color);
            if (tile.leftEnd)
                Manager.SpriteBatch.Draw(left, new Vector2(Position.X - 10, Position.Y), tile.color);
            if (tile.rightEnd)
                Manager.SpriteBatch.Draw(right, new Vector2(Position.X + texture.Width, Position.Y), tile.color);
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
