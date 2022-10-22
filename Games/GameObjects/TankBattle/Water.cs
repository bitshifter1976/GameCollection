using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static AxeGameCollection.Screens.ScreenGameTankBattle;

namespace AxeGameCollection.GameObjects.TankBattle;

public sealed class Water : SpriteAnimatedMultiLine
{
    private readonly Rectangle destRect;
    private static int TopY = 0;

    public static int Top => TopY;

	public Water() : base("graphic/tankBattle/water", new Vector2(0,0), 4, 4, 10, 0f, 1f, (int)Layer.Water)
    {
        TopY = (int)(Manager.DesignHeight - Height);
        Position = new Vector2(0, TopY);
        scrolling = false;
        destRect = new Rectangle(0, TopY, Manager.DesignWidth, SpriteHeight);
    }

    public override void Draw()
    {
        Rectangle src = new Rectangle(SpriteWidth * FrameIndex, SpriteHeight * RowIndex, SpriteWidth, SpriteHeight);
        Manager.SpriteBatch.Draw(texture, destRect, src, Color.White, rotation, origin, SpriteEffects.None, 0);
    }
}
