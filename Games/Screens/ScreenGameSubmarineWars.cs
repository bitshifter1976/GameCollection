using AxeGameCollection.GameObjects.SubmarineWars;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace AxeGameCollection.Screens;

public class ScreenGameSubmarineWars : GameScreen
{
    public enum GameState
    {
        Load,
        Play,
        End
    }
    public enum Layer
    {
        Submarine = 10,
        Planet = 20,
        Fish = 30,
        Shot = 40,
        Explosion = 50,
        Water = 90,
        Hud = 100,
    }

    private Color backColor;
    private GameState state;
    private readonly int level;
    private Submarine submarine;
    private Hud hud;
    private float gameEndTime = 0;
    private const float GameEndTimeout = 2;
    private bool acceptEndInput;

    private int EnemyKillCount => (int)((level+1f) * 1.5f);
    private float LevelFactor => (level + 10f) / 10f;

    public ScreenGameSubmarineWars(Game game, int level) : base(game)
    {
        state = GameState.Load;
        this.level = level;
        Manager.Input.CreateHoldingKeys(0.10f, Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.W, Keys.S, Keys.A, Keys.D);
        Manager.Input.CreateHoldingButtons(0.10f, Buttons.DPadUp, Buttons.DPadDown, Buttons.DPadLeft, Buttons.DPadRight, Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight);
    }

    public override void LoadContent()
    {
        Manager.Sound.LoadSong("game");
        Manager.Sound.PlaySong("game");
        CreateScene();
        base.LoadContent();
    }

    private void CreateScene()
    {
        backColor = Color.CornflowerBlue;
        SpriteManager.Init(submarine = Submarine.Create(1, false));
        SpriteManager.AddImmediate(new Water());
        SpriteManager.AddImmediate(hud = new Hud());
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        switch (state)
        {
            case GameState.Load:
                {
                    break;
                }
            case GameState.Play:
                {
                    SpriteManager.Add(Fish.Create(100));
                    SpriteManager.Add(Mine.Create((int)(500f / LevelFactor)));
                    SpriteManager.Add(Squid.Create(1000));
                    SpriteManager.Add(Submarine.Create((int)(1000f / LevelFactor), true));
                    SpriteManager.Add(EnemyShip.Create((int)(1000f / LevelFactor)));
                    hud.Energy = submarine.Energy;
                    hud.ElapsedTime += TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds);
                    hud.EnemiesKilled += SpriteManager.Update(gameTime, true);
                    if (submarine.Energy <= 0 || hud.EnemiesKilled >= EnemyKillCount)
                        state = GameState.End; 
                    break;
                }
            case GameState.End:
                {
                    SpriteManager.Update(gameTime, false);
                    break;
                }
        }
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    public override void Draw(GameTime gameTime)
    {
        InitDraw(backColor);
        switch (state)
        {
            case GameState.Load:
                {
                    SpriteManager.Draw();
                    var text = CreateCenteredText(AxeGameCollection.Games.SubmarineWars.ToString(), 1, $"Level: {level}", $"Objective: Kill {EnemyKillCount} enemies", "Press Enter or A");
                    ShowCenterText(text, AxeGameCollection.Games.SubmarineWars.ToString(), Color.Black, 1);
                    break;
                }
            case GameState.Play:
                {
                    SpriteManager.Draw();
                    SpriteManager.ScrollX(-submarine.Speed);
                    break;
                }
            case GameState.End:
                {
                    SpriteManager.Draw();
                    var text = submarine.Energy > 0 ? $"Level {level} finished!" : "End of game!";
                    gameEndTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (gameEndTime >= GameEndTimeout)
                    {
                        text = CreateCenteredText(AxeGameCollection.Games.SubmarineWars.ToString(), 1, text, "Press Enter or A"); ;
                        acceptEndInput = true;
                    }
                    ShowCenterText(text, AxeGameCollection.Games.SubmarineWars.ToString(), Color.Black, 1);
                    break;
                }
        }
        EndDraw(Color.Black);
    }

    public override void HandleInput()
    {
        // exit
        if (Manager.Input.KeyPressed(Keys.Escape) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.Start))
        {
            ExitGame();
        }
        // next level
        if (Manager.Input.KeyPressed(Keys.F1))
        {
            SpriteManager.Clear();
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new ScreenGameSubmarineWars(game, level + 1));
        }
        // toggle debug
        if (Manager.Input.KeyPressed(Keys.F2))
        {
            Manager.Debug = !Manager.Debug;
        }
        // toggle fullscreen
        if (Manager.Input.KeyPressed(Keys.F3))
        {
            Manager.Graphics.IsFullScreen = !Manager.Graphics.IsFullScreen;
            Manager.Graphics.ApplyChanges();
        }
        // dive
        var automaticRotateBack = true;
        if (Manager.Input.HoldingKey(Keys.Up, Keys.W) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, Buttons.DPadUp, Buttons.LeftThumbstickUp))
        {
            automaticRotateBack = false;
            submarine.Dive(true);
        }
        if (Manager.Input.HoldingKey(Keys.Down, Keys.S) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, Buttons.DPadDown, Buttons.LeftThumbstickDown))
        {
            automaticRotateBack = false;
            submarine.Dive(false);
        }
        if (automaticRotateBack)
        {
            submarine.RotateBack(0.005f);
        }
        var automaticSpeedDown = true;
        if (Manager.Input.HoldingKey(Keys.Left, Keys.A) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, Buttons.DPadLeft, Buttons.LeftThumbstickLeft))
        {
            automaticSpeedDown = false;
            submarine.AddSpeed(-0.02f);
        }
        if (Manager.Input.HoldingKey(Keys.Right, Keys.D) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, Buttons.DPadRight, Buttons.LeftThumbstickRight))
        {
            automaticSpeedDown = false;
            submarine.AddSpeed(0.02f);
        }
        if (automaticSpeedDown)
        {
            submarine.SpeedDown(0.02f);
        }
        // shoot
        if (Manager.Input.KeyPressed(Keys.Enter) || Manager.Input.KeyPressed(Keys.Space) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A))
        {
            if (state == GameState.Play)
            {
                if (hud.Munition > 0)
                {
                    submarine.Shoot();
                    hud.Munition--;
                }
            }
        }
        // accept message
        if (Manager.Input.KeyPressed(Keys.Enter) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A))
        {
            switch (state)
            {
                case GameState.Load:
                    {
                        state = GameState.Play;
                        break;
                    }
                case GameState.End:
                    {
                        if (acceptEndInput)
                        {
                            SpriteManager.Clear();
                            ScreenManager.RemoveScreen(this);
                            ScreenManager.AddScreen(submarine.Energy <= 0 ? new ScreenMenuSubmarineWars(game) : new ScreenGameSubmarineWars(game, level + 1));
                        }
                        break;
                    }
            }
        }

        base.HandleInput();
    }

    private void ExitGame()
    {
        if (Manager.Debug)
        {
            game.Exit();
        }
        else
        {
            Manager.Sound.PauseSong();
            var messageBox = new ScreenMessage(game, "Want to exit?", "Yes: Press Enter or A", "No: Press Escape or B", Color.Gold, Color.Red, Color.Gold);
            messageBox.Accepted += (sender, e) =>
            {
                Manager.Sound.StopSong();
                ScreenManager.RemoveScreen(this);
                ScreenManager.AddScreen(new ScreenMenuMain(game));
            };
            messageBox.Cancelled += (sender, e) =>
            {
                Manager.Sound.ResumeSong();
            };
            ScreenManager.AddScreen(messageBox);
        }
    }

    public override void UnloadContent()
    {
        SpriteManager.Clear();
        base.UnloadContent();
    }
}