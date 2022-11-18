using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public sealed class Water : Sprite
{
    private new int Height = 300;
	public Water() : base("graphic/common/dot", (int)Layer.Water, CollisionType.None)
    {
        Position = new Vector2(0, Manager.DesignHeight - Height);
        scrolling = false;
        color = new Color(Color.DeepSkyBlue.R, Color.DeepSkyBlue.G, Color.DeepSkyBlue.B, (byte)100);
    }

    public override void Draw()
    {
        Manager.SpriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, Manager.DesignWidth, Height), null, color, 0, Vector2.Zero, SpriteEffects.None, 0);
    }
}