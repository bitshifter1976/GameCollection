using AxeGameCollection.GameObjects.ArenaChase;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AxeGameCollection.Screens;

public class ScreenGameSubmarineWars : GameScreen
{
    public enum GameState
    {
        Load,
        Start,
        Play,
        End
    }

    private Color backColor;
    private GameState state;
    private int level;

    public ScreenGameSubmarineWars(Game game, int level) : base(game)
    {
        state = GameState.Load;
        this.level = level;

        Manager.Input.CreateHoldingKeys(0.10f, Keys.Up, Keys.Down, Keys.Left, Keys.Right);
        Manager.Input.CreateHoldingButtons(0.10f, Buttons.DPadUp, Buttons.DPadDown, Buttons.DPadLeft, Buttons.DPadRight, Buttons.LeftThumbstickRight, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown);
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
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        switch (state)
        {
            case GameState.Load:
                {
                    break;
                }
            case GameState.Start:
                {
                    break;
                }
            case GameState.Play:
                {
                    break;
                }
            case GameState.End:
                {
                    break;
                }
        }
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    public override void Draw(GameTime gameTime)
    {
        InitDraw(Color.Black);
        switch (state)
        {
            case GameState.Load:
                {
                    break;
                }
            case GameState.Start:
                {
                    break;
                }
            case GameState.Play:
                {
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
            ScreenManager.AddScreen(new ScreenGameTankBattle(game, true, level + 1));
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
        // accept messages
        if (Manager.Input.KeyPressed(Keys.Enter) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A) || Manager.Input.GamePadKeyPressed(PlayerIndex.Two, Buttons.A))
        {
            switch (state)
            {
                case GameState.Load:
                    break;
                case GameState.End:
                    break;
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