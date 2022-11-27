using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public class Hud : Sprite
{
    private readonly LifeBar energyBar;
    //private readonly MunitionBar torpedoBar;
    //private readonly EnemiesKilledBar enemiesKilledBar;
    private readonly TimeBar timeBar;

    public override float Height => 70;
    public override float Width => Manager.DesignWidth;

    public override float Energy
    {
        get => energyBar.Points;
        set => energyBar.Points = (int)value;
    }

    //public int Munition
    //{
    //    get => torpedoBar.MunitionCount;
    //    set => torpedoBar.MunitionCount = value;
    //}
    //public int MaxMunition
    //{
    //    get => torpedoBar.MaxMunition;
    //}

    //public int EnemiesKilled
    //{
    //    get => enemiesKilledBar.Points;
    //    set => enemiesKilledBar.Points = value;
    //}

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
        var hudItemCount = 2;
        var itemWidth = (int)(Width / hudItemCount);
        var posX = 0f;
        var posY = 0;
        //torpedoBar = new MunitionBar(new Vector2(posX, posY), 0.7f, 20, ItemWidth, Height);
        //posX += ItemWidth;
        energyBar = new LifeBar(new Vector2(posX, posY), 0.25f, itemWidth, (int)Height);
        posX += itemWidth;
        //enemiesKilledBar = new EnemiesKilledBar(new Vector2(posX, posY), 0.4f, ItemWidth, Height);
        //posX += ItemWidth;
        timeBar = new TimeBar(new Vector2(posX, posY), 0.25f, itemWidth, (int)Height);
        Energy = 100;
        //Munition = 20;
        //EnemiesKilled = 0;
        ElapsedTime = TimeSpan.FromSeconds(0);
    }

    public override void Draw()
    {
        energyBar.Draw();
        //torpedoBar.Draw();
        //enemiesKilledBar.Draw();
        timeBar.Draw();
    }

    public void AddMunition(float minuitionPart)
    {
        //addMunitionCount += minuitionPart;
        /*if (Munition < MaxMunition && addMunitionCount >= 1)
        {
            Munition++;
            addMunitionCount = 0;
        }*/
    }
}