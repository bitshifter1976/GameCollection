using AxeGameCollection.Screens;
using Framework;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AxeGameCollection;

public class AxeGameCollection : Game
{
    public enum Games
    {
        None,
        ArenaChase,
        MrSunny,
        TankBattle,
        SubmarineWars
    }

    private readonly GraphicsDeviceManager graphics;

    public AxeGameCollection()
    {
        graphics = new GraphicsDeviceManager(this);
        IsFixedTimeStep = true;
        MaxElapsedTime = TimeSpan.FromSeconds(1f);
        TargetElapsedTime = TimeSpan.FromSeconds(1f / Config.Fps);
    }

    protected override void Initialize()
    {
        Window.Title = Config.Title;
        // resolution
        graphics.IsFullScreen = Config.FullScreen;
        if (Config.FullScreen)
        {
            graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
        }
        else
        {
            graphics.PreferredBackBufferWidth = Config.Width;
            graphics.PreferredBackBufferHeight = Config.Height;
        }
        graphics.ApplyChanges();
        // order important!
        Content.RootDirectory = "Content";
        Log.Init(Config.LogDir, Config.LogLevel);
        Manager.Create(Content, graphics, this, Config.SoundEnabled, new Size(Config.DesignWidth,Config.DesignHeight), Config.Debug, "font", new List<string> { "Debug", "Standard", Games.ArenaChase.ToString(), Games.MrSunny.ToString(), Games.TankBattle.ToString(), Games.SubmarineWars.ToString() });
        Debug.Create(new Vector2(10, 10), new Vector2(200, 10), true, "Debug", 2f);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        switch (Config.StartGameImmediate)
        {
            case Games.None:
                ScreenManager.AddScreen(new ScreenIntro(this));
                break;
            case Games.ArenaChase:
                ScreenManager.AddScreen(new ScreenGameArenaChase(this, true, Config.LevelArenaChase));
                break;
            case Games.MrSunny:
                ScreenManager.AddScreen(new ScreenGameMrSunny(this, Config.LevelMrSunny));
                break;
            case Games.TankBattle:
                ScreenManager.AddScreen(new ScreenGameArenaChase(this, true, Config.LevelTankBattle));
                break;
            case Games.SubmarineWars:
                ScreenManager.AddScreen(new ScreenGameSubmarineWars(this, Config.LevelSubmarineWars));
                break;
        }

        base.LoadContent();
    }
}