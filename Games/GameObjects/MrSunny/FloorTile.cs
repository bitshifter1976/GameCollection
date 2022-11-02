using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny
{
    public class FloorTile : Sprite
    {
        public FloorTileProps Tile;
        private Texture2D left;
        private Texture2D right;
        public new float Width;

        public int TopOffset => (int)(10*scale);

        public override Rectangle BoundingBox => new((int)position.X, (int)position.Y+TopOffset, (int)Width, (int)Height-TopOffset);

        public bool LeftEnd
        {
            get => Tile.leftEnd;
            set
            {
                Tile.leftEnd = value;
                if (value)
                    left = Manager.Content.Load<Texture2D>("graphic/mrSunny/" + Tile.left);
            }
        }

        public bool RightEnd 
        {
            get => Tile.rightEnd;
            set
            {
                Tile.rightEnd = value;
                if (value)                   
                    right = Manager.Content.Load<Texture2D>("graphic/mrSunny/" + Tile.right);
            } 
        }

        public FloorTile(Vector2 position, FloorTileProps tile) : base("graphic/mrSunny/" + tile.name, position, 0, 1, (int)Layer.Beach, CollisionType.BoundingBox)
        {
            Tile = tile;
            scrolling = true;
            Width = tile.width;
            LeftEnd = tile.leftEnd;
            RightEnd = tile.rightEnd;
        }

        public override void Draw()
        {
            if (Tile.name != "gap")
            {
                Manager.SpriteBatch.Draw(texture, Position, Tile.color);
                if (LeftEnd)
                    Manager.SpriteBatch.Draw(left, new Vector2(Position.X - 10, Position.Y), Tile.color);
                if (RightEnd)
                    Manager.SpriteBatch.Draw(right, new Vector2(Position.X + texture.Width, Position.Y), Tile.color);
            }
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
