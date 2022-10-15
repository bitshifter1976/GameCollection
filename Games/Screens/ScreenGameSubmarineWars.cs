using AxeGameCollection.GameObjects.SubmarineWars;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
    private float gamePlayTime = 0;
    private const float GamePlayTimeout = 60;
    private float gameEndTime = 0;
    private const float GameEndTimeout = 2;
    private bool acceptEndInput;

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
        SpriteManager.AddImmediate(new Water());
        SpriteManager.AddImmediate(hud = new Hud());
        SpriteManager.AddImmediate(submarine = new Submarine(new Vector2(Manager.DesignWidth / 2f, Manager.DesignHeight / 2f), 0, 1, false));
        CreateEnemy(1);
        hud.TotalPoints += SpriteManager.Update(new GameTime(), level);
    }

    private void CreateEnemy(int possibility)
    {
        if (!SpriteManager.HasEnemy && Rand.Bool(1, possibility))
            SpriteManager.AddImmediate(new Submarine(new Vector2(Manager.DesignWidth + submarine.Width/2f - 1, Rand.Float(Water.MaxYTopPixel + submarine.Height, Water.MinYGroundPixel - submarine.Height)), 0, 1, true));
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
                    CreateEnemy(1000 / level);
                    hud.Energy = submarine.Energy;
                    hud.TotalPoints += SpriteManager.Update(gameTime, level);
                    break;
                }
            case GameState.End:
                {
                    hud.TotalPoints += SpriteManager.Update(gameTime, level);
                    break;
                }
        }
        Fishes.Create(100);
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
                    var str = $"Get ready!\n    Level {level}";
                    ShowCenterText(str, AxeGameCollection.Games.ArenaChase.ToString(), Color.Gold, 1);
                    break;
                }
            case GameState.Play:
                {
                    gamePlayTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (gamePlayTime >= GamePlayTimeout || submarine.Energy <= 0)
                        state = GameState.End;
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
                        ShowCenterText(str, AxeGameCollection.Games.SubmarineWars.ToString(), Color.Gold, 1);
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
            submarine.Speed += 0.01f;
            SpriteManager.ScrollX(submarine.Speed);
        }
        if (Manager.Input.HoldingKey(Keys.Right, Keys.D) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, Buttons.DPadRight, Buttons.LeftThumbstickRight))
        {
            automaticSpeedDown = false;
            submarine.Speed -= 0.01f;
            SpriteManager.ScrollX(submarine.Speed);
        }
        if (automaticSpeedDown)
        {
            if (submarine.Speed > 0)
                submarine.Speed -= 0.01f;
            if (submarine.Speed < 0)
                submarine.Speed += 0.01f;
            SpriteManager.ScrollX(submarine.Speed);
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