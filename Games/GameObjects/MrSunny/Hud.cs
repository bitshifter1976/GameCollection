using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public class Hud : Sprite
{
    private readonly LifeBar energyBar;
    private readonly MunitionBar shotBar;
    private readonly TimeBar timeBar;

    public override float Height => 70;
    public override float Width => Manager.DesignWidth;

    public override float Energy
    {
        get => energyBar.Points;
        set => energyBar.Points = (int)value;
    }

    public int Munition
    {
        get => shotBar.MunitionCount;
        set => shotBar.MunitionCount = value;
    }
    public int MaxMunition
    {
        get => shotBar.MaxMunition;
    }

    public TimeSpan ElapsedTime
    {
        get => timeBar.ElapsedTime;
        set => timeBar.ElapsedTime = value;
    }

    public Hud() : base((int)Layer.Hud)
    {
        color = new Color(Color.Black.R, Color.Black.G, Color.Black.B, (byte)50);
        texture = new Texture2D(Manager.Graphics.GraphicsDevice, 1, 1);
        texture.SetData(new Color[1] { Color.White });
        var hudItemCount = 3;
        var itemWidth = (int)(Width / hudItemCount);
        var posX = 0f;
        var posY = 0;
        shotBar = new MunitionBar(new Vector2(posX, posY), 0.2f, 10, itemWidth, (int)Height);
        posX += itemWidth;
        energyBar = new LifeBar(new Vector2(posX, posY), 0.25f, itemWidth, (int)Height);
        posX += itemWidth;
        timeBar = new TimeBar(new Vector2(posX, posY), 0.25f, itemWidth, (int)Height);
        Energy = 100;
        Munition = 10;
        ElapsedTime = TimeSpan.FromSeconds(0);
    }

    public override void Draw()
    {
        energyBar.Draw();
        shotBar.Draw();
        timeBar.Draw();
    }
}