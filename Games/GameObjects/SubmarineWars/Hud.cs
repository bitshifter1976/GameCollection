using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public class Hud : Sprite
{
    public int Height => 100;

    public Hud() : base((int)Layer.Hud)
    {
        texture = Manager.Content.Load<Texture2D>("graphic/common/dot");
        color = Color.Gray;
    }

    public override void Draw()
    {
        // background
        Manager.SpriteBatch.Draw(
            texture,
            new Rectangle(0, Manager.DesignHeight-Height, Manager.DesignWidth, Manager.DesignHeight),
            null,
            color,
            0,
            Vector2.Zero,
            SpriteEffects.None,
            0);
    }
}