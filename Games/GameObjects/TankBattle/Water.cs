using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AxeGameCollection.GameObjects.TankBattle;

public sealed class Water : SpriteAnimatedMultiLine
{
    private readonly Rectangle destRect;

    public static int Top = Manager.DesignHeight - 200;

	public Water() : base("graphic/tankBattle/water", new Vector2(0,0), 20, 4, 10, 0f, 1f, (int)Layer.Water)
    {
        Position = new Vector2(0, Manager.DesignHeight - Height / RowCount);
        scrolling = false;
        destRect = new Rectangle(0, Manager.DesignHeight - texture.Height / RowCount, Manager.DesignWidth, SpriteHeight);
        Top = (int)Position.Y;
    }

    public override void Draw()
    {
        Rectangle src = new Rectangle(SpriteWidth * FrameIndex, SpriteHeight * RowIndex, SpriteWidth, SpriteHeight);
        Manager.SpriteBatch.Draw(texture, destRect, src, Color.White, rotation, origin, SpriteEffects.None, 0);
    }
}
