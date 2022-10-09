using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public class Hud : Sprite
{
    public float MaxHeight => 75f;
    private LifeBar energyBar;

    public override float Energy
    {
        get
        {
            return Energy;
        }
        set
        { 
            energy = value;
            energyBar.Percentage = value; 
        }
    }

    public Hud() : base((int)Layer.Hud)
    {
        texture = Manager.Content.Load<Texture2D>("graphic/common/dot");
        color = new Color(Color.Black.R, Color.Black.G, Color.Black.B, (byte)50);
        energyBar = new LifeBar(2.5f);
        energyBar.Position = new Vector2(10, Manager.DesignHeight - MaxHeight / 2f - energyBar.Height / 2f);
        Energy = 100;
    }

    public override void Draw()
    {
        // background
        Manager.SpriteBatch.Draw(
            texture,
            new Rectangle(0, Manager.DesignHeight - (int)MaxHeight, Manager.DesignWidth, Manager.DesignHeight),
            null,
            color,
            0,
            Vector2.Zero,
            SpriteEffects.None,
            0);

        energyBar.Draw();
    }
}