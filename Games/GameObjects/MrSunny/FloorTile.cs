using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;
using System.Linq;

namespace AxeGameCollection.GameObjects.MrSunny
{
    public class FloorTile : Sprite
    {
        private int heightLevel;
        private FloorTileProps3 props;
        private Texture2D bottomTexture;

        public override float Width => 256;

        public int TopOffset
        {
            get
            {
                switch (props.type)
                {
                    case FloorTileType.groundTopLeft:
                    case FloorTileType.groundTopMiddle:
                    case FloorTileType.groundTopRight:
                    case FloorTileType.groundTopSingle:
                        return 120;
                    case FloorTileType.groundBottomLeft:
                    case FloorTileType.groundBottomMiddle:
                    case FloorTileType.groundBottomRight:
                    case FloorTileType.groundBottomSingle:
                    case FloorTileType.gap:
                    default:
                        return 0;
                }
            }
        }

        public int LeftOffset
        {
            get
            {
                switch (props.type)
                {
                    case FloorTileType.groundTopLeft:
                    case FloorTileType.groundBottomLeft:
                        return 110;
                    case FloorTileType.groundTopRight:
                    case FloorTileType.groundTopMiddle:
                    case FloorTileType.groundBottomMiddle:
                    case FloorTileType.groundBottomRight:
                    case FloorTileType.groundTopSingle:
                    case FloorTileType.groundBottomSingle:
                    case FloorTileType.gap:
                    default:
                        return 0;
                }
            }
        }

        public int RightOffset
        {
            get
            {
                switch (props.type)
                {
                    case FloorTileType.groundTopRight:
                    case FloorTileType.groundBottomRight:
                        return 110;
                    case FloorTileType.groundTopLeft:
                    case FloorTileType.groundBottomLeft:
                    case FloorTileType.groundTopMiddle:
                    case FloorTileType.groundBottomMiddle:
                    case FloorTileType.groundTopSingle:
                    case FloorTileType.groundBottomSingle:
                    case FloorTileType.gap:
                    default:
                        return 0;
                }
            }
        }

        public override Rectangle BoundingBox => new((int)position.X + LeftOffset, (int)position.Y + TopOffset, (int)Width - RightOffset - LeftOffset, (int)Height*heightLevel - TopOffset);

        public FloorTile(float x, int heightLevel, FloorTileProps3 props) : base("graphic/mrSunny/" + (heightLevel < 3 ? props.type.ToString() : props.platformType.ToString()), Vector2.Zero, 0, 1, (int)Layer.Beach, CollisionType.BoundingBox)
        {
            this.heightLevel = heightLevel;
            this.props = props;
            if (heightLevel == 2)
                bottomTexture = Manager.Content.Load<Texture2D>("graphic/mrSunny/" + props.bottomType.ToString());
            scrolling = true;
            position = new Vector2(x, Manager.DesignHeight - Height * heightLevel);
        }

        public override void Draw()
        {
            if (props.type != FloorTileType.gap)
            {
                Manager.SpriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                if (heightLevel == 2)
                    Manager.SpriteBatch.Draw(bottomTexture, new Vector2(position.X, position.Y + Height), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

                if (Manager.Debug)
                {
                    Debug.Rects.Add(new RectDebug(Line.GetFromRectangle(BoundingBox).ToList(), "BoundingBox", Color.Red));
                    Manager.SpriteBatch.DrawString(Manager.Fonts.Get("Debug"), $"{position}({this.GetType().Name})", new Vector2(position.X, position.Y + Height + 2), Color.Black);
                }
            }
        }
    }

    public enum FloorTileType
    {
        groundTopLeft,
        groundTopMiddle,
        groundTopRight,
        groundBottomLeft,
        groundBottomMiddle,
        groundBottomRight,
        groundTopSingle,
        groundBottomSingle,
        platformLeft,
        platformMiddle,
        platformRight,
        platformSingle,
        gap
    }

    public class FloorTileProps3
    {
        public FloorTileType type;
        public FloorTileType bottomType;
        public FloorTileType platformType;

        public FloorTileProps3(FloorTileType type, FloorTileType bottomType, FloorTileType platformType)
        {
            this.type = type;
            this.bottomType = bottomType;
            this.platformType = platformType;
        }

        public FloorTileProps3 Clone()
        {
            return new FloorTileProps3(type, bottomType, platformType);
        }
    }
}
