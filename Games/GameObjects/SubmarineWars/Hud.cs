using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public class Hud : Sprite
{
    private readonly LifeBar energyBar;
    private readonly MunitionBar torpedoBar;
    private readonly EnemiesKilledBar enemiesKilledBar;
    private readonly TimeBar timeBar;
    private readonly int HudItemCount = 4;

    private new int Height => 100;
    private new int Width => Manager.DesignWidth;
    private int ItemWidth => Width / HudItemCount;

    public override float Energy
    {
        get => energyBar.Points;
        set => energyBar.Points = (int)value;
    }

    public int Munition
    {
        get => torpedoBar.MunitionCount;
        set => torpedoBar.MunitionCount = value;
    }

    public int EnemiesKilled
    {
        get => enemiesKilledBar.Points;
        set => enemiesKilledBar.Points = value;
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
        var posX = 0f;
        var posY = Manager.DesignHeight - Height;
        torpedoBar = new MunitionBar(new Vector2(posX, posY), 0.7f, 20, ItemWidth, Height);
        posX += ItemWidth;
        energyBar = new LifeBar(new Vector2(posX, posY), 0.4f, ItemWidth, Height);
        posX += ItemWidth;
        enemiesKilledBar = new EnemiesKilledBar(new Vector2(posX, posY), 0.4f, ItemWidth, Height);
        posX += ItemWidth;
        timeBar = new TimeBar(new Vector2(posX, posY), 0.4f, ItemWidth, Height);
        Energy = 100;
        Munition = 20;
        EnemiesKilled = 0;
        ElapsedTime = TimeSpan.FromSeconds(0);
    }

    public override void Draw()
    {
        // background
        Manager.SpriteBatch.Draw(
            texture,
            new Rectangle(0, Manager.DesignHeight - Height, Manager.DesignWidth, Height),
            null,
            color,
            0,
            Vector2.Zero,
            SpriteEffects.None,
            0);

        energyBar.Draw();
        torpedoBar.Draw();
        enemiesKilledBar.Draw();
        timeBar.Draw();
    }
}