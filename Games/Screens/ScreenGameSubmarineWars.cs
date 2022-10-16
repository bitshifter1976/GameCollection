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
        Trees = 20,
        Stars = 40,
        Planet = 50,
        Cloud = 60,
        Bird = 70,
        Fish = 71,
        Shot = 80,
        Explosion = 90,
        Water = 95,
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

    public int EnemyKillCount => (int)((level+1f) * 1.5f);

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
        Manager.Sound.LoadEffect("fireBurn");
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
                    var levelFactor = (level + 10f) / 10f;
                    SpriteManager.Add(Fish.Create(100));
                    SpriteManager.Add(Mine.Create((int)(300f / levelFactor)));
                    SpriteManager.Add(Submarine.Create((int)(500f / levelFactor), true));
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
                    var str = $"Level: {level}\nObjective: Kill {EnemyKillCount} enemies\nPress Enter or A";
                    ShowCenterText(str, AxeGameCollection.Games.ArenaChase.ToString(), Color.White, 1);
                    break;
                }
            case GameState.Play:
                {
                    SpriteManager.Draw();
                    break;
                }
            case GameState.End:
                {
                    SpriteManager.Draw();
                    gameEndTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (gameEndTime >= GameEndTimeout)
                    {
                        acceptEndInput = true;
                        var str = submarine.Energy > 0 ? $"Level {level} finished!" : "End of game!";
                        str += "\nPress Enter or A";
                        ShowCenterText(str, AxeGameCollection.Games.SubmarineWars.ToString(), Color.White, 1);
                    }
                    break;
                }
        }
        EndDraw(Color.Black);
    }

    public override void HandleInput()
    {
        // helping keys
        if (Manager.Input.KeyPressed(Keys.Escape) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.Start))
        {
            ExitGame();
        }
        if (Manager.Input.KeyPressed(Keys.F1))
        {
            SpriteManager.Clear();
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new ScreenGameSubmarineWars(game, level + 1));
        }
        if (Manager.Input.KeyPressed(Keys.F2))
        {
            Manager.Debug = !Manager.Debug;
        }
        if (Manager.Input.KeyPressed(Keys.F3))
        {
            Manager.Graphics.IsFullScreen = !Manager.Graphics.IsFullScreen;
            Manager.Graphics.ApplyChanges();
        }
        // dive
        if (Manager.Input.HoldingKey(Keys.Up, Keys.W) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, Buttons.DPadUp, Buttons.LeftThumbstickUp))
        {
            submarine.Dive(true);
        }
        if (Manager.Input.HoldingKey(Keys.Down, Keys.S) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, Buttons.DPadDown, Buttons.LeftThumbstickDown))
        {
            submarine.Dive(false);
        }
        var automaticSpeedDown = true;
        if (Manager.Input.HoldingKey(Keys.Left, Keys.A) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, Buttons.DPadLeft, Buttons.LeftThumbstickLeft))
        {
            automaticSpeedDown = false;
            submarine.AddSpeed(-0.01f);
            SpriteManager.ScrollX(-submarine.Speed);
        }
        if (Manager.Input.HoldingKey(Keys.Right, Keys.D) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, Buttons.DPadRight, Buttons.LeftThumbstickRight))
        {
            automaticSpeedDown = false;
            submarine.AddSpeed(0.01f);
            SpriteManager.ScrollX(-submarine.Speed);
        }
        if (automaticSpeedDown)
        {
            submarine.SpeedDown(0.01f);
            SpriteManager.ScrollX(-submarine.Speed);
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
                            ScreenManager.AddScreen(submarine.Energy <= 0 ? new ScreenMenuMain(game) : new ScreenGameSubmarineWars(game, level + 1));
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