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
    private TotalPointsBar totalPointsBar;
    private int munition;
    private int totalPoints;

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
            torpedoBar.MunitionCount = value;
        }
    }

    public int TotalPoints
    {
        get => totalPoints;
        set
        {
            totalPoints = value;
            totalPointsBar.TotalPoints = value;
        }
    }

    public Hud() : base((int)Layer.Hud)
    {
        color = new Color(Color.Black.R, Color.Black.G, Color.Black.B, (byte)50);
        texture = new Texture2D(Manager.Graphics.GraphicsDevice, 1, 1);
        texture.SetData(new Color[1] { Color.White });
        energyBar = new LifeBar(2.5f);
        energyBar.Position = new Vector2(10, Manager.DesignHeight - MaxHeight / 2f - energyBar.Height / 2f);
        Energy = 100;
        var torpedoBarPosX = energyBar.Position.X + energyBar.Width + 50;
        var torpedoBarPosY = energyBar.Position.Y;
        this.munition = 20;
        torpedoBar = new MunitionBar(new Vector2(torpedoBarPosX, torpedoBarPosY), 0.7f, munition);
        totalPointsBar = new TotalPointsBar(new Vector2(torpedoBar.Position.X + torpedoBar.Width + 50, torpedoBarPosY), 0.7f);
        TotalPoints = 0;
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
        totalPointsBar.Draw();
    }
}