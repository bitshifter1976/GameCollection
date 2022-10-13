using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public class Hud : Sprite
{
    public float MaxHeight => 100f;
    private LifeBar energyBar;
    private MunitionBar torpedoBar;
    private int munition;

    public override float Energy
    {
        get => Energy;
        set
        {
            energy = value;
            energyBar.Percentage = value;
        }
    }

    public int Munition 
    {
        get => munition;
        set
        {
            munition = value;
            torpedoBar.MunitionCount = munition;
        }
    }

    public Hud(int munition) : base((int)Layer.Hud)
    {
        color = new Color(Color.Black.R, Color.Black.G, Color.Black.B, (byte)50);
        texture = new Texture2D(Manager.Graphics.GraphicsDevice, 1, 1);
        texture.SetData(new Color[1] { Color.White });
        energyBar = new LifeBar(2.5f);
        energyBar.Position = new Vector2(10, Manager.DesignHeight - MaxHeight / 2f - energyBar.Height / 2f);
        Energy = 100;
        var torpedoBarPosX = energyBar.Position.X + energyBar.Width + 50;
        var torpedoBarPosY = energyBar.Position.Y;
        this.munition = munition;
        torpedoBar = new MunitionBar(new Vector2(torpedoBarPosX, torpedoBarPosY), 0.7f, munition);
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
        torpedoBar.Draw();
    }
}