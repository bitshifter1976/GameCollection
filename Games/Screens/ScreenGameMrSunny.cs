using AxeGameCollection.GameObjects.MrSunny;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AxeGameCollection.Screens;

public class ScreenGameMrSunny : GameScreen
{
    public enum GameState
    {
        Load,
        Play,
        Paused,
        End
    }
    public enum Layer
    {
        Beach = 20,
        Platform = 30,
        Player = 40,
        Shot = 50,
        Explosion = 60,
        Water = 90,
        Hud = 100,
    }

    private Color backgroundColor; 
    private Texture2D textureBackground;
    private GameState state;
    private readonly int level;
    private float gameEndTime = 0;
    private const float GameEndTimeout = 2;
    private bool acceptEndInput;

    public ScreenGameMrSunny(Game game, int level) : base(game)
    {
        state = GameState.Load;
        this.level = level;
        Manager.Input.CreateHoldingKeys(1.5f, Keys.Left, Keys.Right, Keys.A, Keys.D, Keys.F4);
        //Manager.Input.CreateHoldingButtons(0.10f, Buttons.DPadUp, Buttons.DPadDown, Buttons.DPadLeft, Buttons.DPadRight, Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight);
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
        // background
        var alpha = Rand.Int(50, 255);
        var color = Rand.Color(Color.Orange, Color.Red, Color.CornflowerBlue, Color.AliceBlue, Color.DarkBlue, Color.CadetBlue);
        backgroundColor = new Color(color.R, color.G, color.B, alpha);
        textureBackground = Manager.Content.Load<Texture2D>("graphic/mrSunny/background");
        // how many screens to boss?
        var distanceToBoss = Manager.DesignWidth * (level/4+3);
        // create player
        Player.Create(2);
        Player.Position = new Vector2(Manager.DesignWidth / 2 - Player.SpriteWidth / 2f, 200);
        // create floor
        SpriteManager.AddImmediate(Floor3.Create(distanceToBoss, level, out var gaps));
        //SpriteManager.Add(new Water());
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
                    if (Player.Energy <= 0)
                    {
                        state = GameState.End;
                    }
                    else
                    {
                        Player.Update(gameTime);
                        SpriteManager.Update(gameTime, true);
                    }
                    break;
                }
            case GameState.Paused:
                {
                    break;
                }
            case GameState.End:
                {
                    Player.Update(gameTime);
                    SpriteManager.Update(gameTime, false);
                    break;
                }
        }
        
        Manager.Update(gameTime);
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        // activate to debug frame for frame with F4
        //state = GameState.Paused;
    }

    public override void Draw(GameTime gameTime)
    {
        InitDraw(backgroundColor);
        Manager.SpriteBatch.Draw(textureBackground, new Rectangle(0, 0, Manager.DesignWidth, Manager.DesignHeight), backgroundColor);
        SpriteManager.Draw();
        Player.Draw();
        switch (state)
        {
            case GameState.Load:
                {
                    var text = CreateCenteredText(AxeGameCollection.Games.MrSunny.ToString(), 1, $"Level: {level}", $"Kill the enemy BOSS at end of level", "Press Enter or A");
                    ShowCenterText(text, AxeGameCollection.Games.MrSunny.ToString(), Color.Black, 1);
                    break;
                }
            case GameState.Play:
            case GameState.Paused:
                {
                    break;
                }
            case GameState.End:
                {
                    var text = Player.Energy > 0 ? $"Level {level} finished!" : "End of game!";
                    gameEndTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (gameEndTime >= GameEndTimeout)
                    {
                        text = CreateCenteredText(AxeGameCollection.Games.MrSunny.ToString(), 1, text, "Press Enter or A"); ;
                        acceptEndInput = true;
                    }
                    ShowCenterText(text, AxeGameCollection.Games.MrSunny.ToString(), Color.Black, 1);
                    break;
                }
        }
        EndDraw(Color.Black);
    }

    public override void HandleInput()
    {
        Player.HandleInput();
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
            ScreenManager.AddScreen(new ScreenGameMrSunny(game, level + 1));
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
        if (Manager.Input.KeyPressed(Keys.F4) || Manager.Input.HoldingKey(Keys.F4))
        {
            if (state == GameState.Paused)
                state = GameState.Play;
            else
                state = GameState.Paused;
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
                            ScreenManager.AddScreen(Player.Energy <= 0 ? new ScreenMenuMrSunny(game) : new ScreenGameMrSunny(game, level + 1));
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
            var messageBox = new ScreenMessage(game, AxeGameCollection.Games.MrSunny.ToString(), "Want to exit?", "Yes: Press Enter or A", "No: Press Escape or B", Color.Gold, Color.Red, Color.Gold);
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