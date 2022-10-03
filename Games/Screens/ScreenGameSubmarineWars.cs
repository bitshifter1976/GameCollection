using AxeGameCollection.GameObjects.SubmarineWars;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Security.Policy;

namespace AxeGameCollection.Screens;

public class ScreenGameSubmarineWars : GameScreen
{
    public enum GameState
    {
        Load,
        Play,
        End
    }

    private Color backColor;
    private GameState state;
    private int level;
    private Submarine submarine;

    public ScreenGameSubmarineWars(Game game, int level) : base(game)
    {
        state = GameState.Load;
        this.level = level;

        Manager.Input.CreateHoldingKeys(0.10f, Keys.Up, Keys.Down, Keys.W, Keys.S);
        Manager.Input.CreateHoldingButtons(0.10f, Buttons.DPadUp, Buttons.DPadDown, Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown);
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
        Water.Load();
        submarine = new Submarine(new Vector2(Manager.DesignWidth / 2f, Manager.DesignHeight / 2f), 0, 1);
        SpriteManager.AddImmediate(submarine);
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        switch (state)
        {
            case GameState.Load:
            {
                SpriteManager.Update(gameTime);
                Water.Update(gameTime);
                break;
            }
            case GameState.Play:
            {
                SpriteManager.Update(gameTime);
                Water.Update(gameTime);
                break;
            }
            case GameState.End:
            {
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
                Water.Draw(gameTime);
                var str = $"Get ready!\nLevel {level}";
                ShowCenterText(str, AxeGameCollection.Games.ArenaChase.ToString(), Color.Gold, 1);
                break;
            }
            case GameState.Play:
            {
                SpriteManager.Draw();
                Water.Draw(gameTime);
                break;
            }
            case GameState.End:
            {
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
        // accept messages
        if (Manager.Input.KeyPressed(Keys.Enter) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A))
        {
            switch (state)
            {
                case GameState.Load:
                {
                    state = GameState.Play;
                    break;
                }
                case GameState.Play:
                {
                    state = GameState.Play;
                    break;
                }
                case GameState.End:
                {
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