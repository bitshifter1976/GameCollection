using AxeGameCollection.GameObjects.TankBattle;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AxeGameCollection.Screens;

public class ScreenGameTankBattle : GameScreen
{
    public enum GameState
    {
        Load,
        Start,
        Play,
        End
    }
    public enum Layer
    {
        Shot = 1,
        Mountain = 2,
        Cannon = 10,
        Trees = 20,
        Water = 30,
        Stars = 40,
        Planet = 50,
        Cloud = 60,
        Bird = 70,
        Explosion = 90,
        Hud = 100,
    }

    private Color backColor;
    private Tank tank1;
    private Tank tank2;
    private Wind wind;
    private GameState state;
    private int mountainWidth = 0;
    private float loadMountainTime = 0;
    private const float LoadMountainTimeout = 0.01f;
    private float gameEndTime = 0;
    private const float GameEndTimeout = 2;
    private readonly bool hasAi;
    private readonly int level;
    private static Texture2D texture;
    private bool acceptEndInput;
    private int nextTankIdx;

    public ScreenGameTankBattle(Game game, bool ai, int level) : base(game)
    {
        state = GameState.Load;
        hasAi = ai;
        this.level = level;

        Manager.Input.CreateHoldingKeys(0.10f, Keys.Up, Keys.Down, Keys.Left, Keys.Right);
        Manager.Input.CreateHoldingButtons(0.10f, Buttons.DPadUp, Buttons.DPadDown, Buttons.DPadLeft, Buttons.DPadRight, Buttons.LeftThumbstickRight, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown);
    }

    public override void LoadContent()
    {
        texture = new Texture2D(Manager.Graphics.GraphicsDevice, 1, 1);
        texture.SetData(new Color[1] { Color.White }); 
        Manager.Sound.LoadSong("game");
        Manager.Sound.PlaySong("game");
        Manager.Sound.LoadEffect("helicopter");

        CreateScene();

        base.LoadContent();
    }

    private void CreateScene()
    {
        backColor = Rand.Color(Color.Blue, Color.BlueViolet, Color.AliceBlue, Color.CadetBlue, Color.CornflowerBlue,
                                           Color.DarkBlue, Color.DarkSlateBlue, Color.DeepSkyBlue, Color.DodgerBlue, Color.LightBlue,
                                           Color.LightSkyBlue, Color.LightSteelBlue, Color.MediumBlue, Color.MediumSlateBlue,
                                           Color.MidnightBlue, Color.PowderBlue, Color.RoyalBlue, Color.SkyBlue, Color.SlateBlue, Color.SteelBlue);
        tank1 = new Tank(false, false, 0.75f, false, level);
        tank2 = new Tank(hasAi, true, 0.75f, false, level);
        tank1.Position = new Vector2(Rand.Float(100f, 300f), -tank1.Height);
        tank2.Position = new Vector2(Rand.Float(Manager.DesignWidth - 300f - tank2.Width, Manager.DesignWidth - 100f - tank2.Width), -tank2.Height);
        tank1.OtherTank = tank2;
        tank2.OtherTank = tank1;

        SpriteManager.Initialize(tank1, tank2);
        Mountain.Load(Color.DarkGray);

        var moon = Rand.Bool(1, 2);
        var sign = Rand.Int(0, 1) == 1 ? -1 : 1;
        var pos = new Vector2(Rand.Float(100, Manager.DesignWidth - 100), Rand.Float(10, 50));
        var speed = Rand.Float(0.2f, 0.5f) * sign;
        if (moon)
        {
            SpriteManager.AddImmediate(new Moon(pos, speed, Rand.Float(0.25f, 1f)));
            Stars.Create(Rand.Int(50, 200));
        }
        else
        {
            SpriteManager.AddImmediate(new Sun(pos, speed, Rand.Float(0.25f, 0.5f)));
        }

        SpriteManager.AddImmediate(new Water());
        wind = new Wind(Vector2.Zero, Rand.Float(0, 359), 0.5f);
        wind.Position = new Vector2(Manager.DesignWidth / 2f, 10 + wind.Height / 2f);
        SpriteManager.AddImmediate(wind);

        for (var i=0; i<Rand.Int(0,30); i++)
            Birds.Create(1);
        for (var i = 0; i < Rand.Int(0, 5); i++)
            Clouds.Create(1);
        Trees.Create(Rand.Int(0, 20));
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
                    SpriteManager.Update(gameTime);
                    if (!Manager.Sound.IsEffectPlaying("helicopter"))
                        Manager.Sound.PlayEffect("helicopter");
                    bool tank1Collide, tank2Collide;
                    if ((tank1Collide = Mountain.Collide(tank1.Center, out _)) == false)
                        tank1.Fall(gameTime);
                    if ((tank2Collide = Mountain.Collide(tank2.Center, out _)) == false)
                        tank2.Fall(gameTime);
                    if (tank1Collide && tank2Collide)
                    {
                        Manager.Sound.StopAllEffects();
                        tank1.IsActive = true;
                        state = GameState.Play;
                    }
                    break;
                }
            case GameState.Play:
                {
                    if (tank1.Energy <= 0 || tank2.Energy <= 0)
                        state = GameState.End;
                    if (!Mountain.Collide(tank1.Center, out _))
                        tank1.Fall(gameTime);
                    if (!Mountain.Collide(tank2.Center, out _))
                        tank2.Fall(gameTime);
                    if (SpriteManager.Update(gameTime))
                    {
                        wind.SetDirection(Rand.Float(0, 359));
                        tank1.IsActive = nextTankIdx == 1;
                        tank2.IsActive = nextTankIdx == 2;
                    }
                    break;
                }
            case GameState.End:
                {
                    tank1.IsActive = false;
                    tank2.IsActive = false;
                    SpriteManager.Update(gameTime);
                    break;
                }
        }

        Clouds.Create(1000);
        Birds.Create(500);
        Mountain.Update(gameTime);
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    public override void Draw(GameTime gameTime)
    {
        InitDraw(state == GameState.Load ? Color.LightGray : backColor);
        switch (state)
        {
            case GameState.Load:
                {
                    loadMountainTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (loadMountainTime > LoadMountainTimeout)
                    {
                        loadMountainTime = 0;
                        mountainWidth += 10;
                    }
                    Mountain.Draw(gameTime, mountainWidth);
                    if (mountainWidth > Manager.DesignWidth)
                    {
                        var text = CreateCenteredText(AxeGameCollection.Games.TankBattle.ToString(), 1, $"Get ready!", $"{(hasAi ? $"Level {level}" : "")}", "Press Enter or A");
                        ShowCenterText(text, AxeGameCollection.Games.TankBattle.ToString(), Color.Gold, 1);
                    }
                    break;
                }
            case GameState.Start:
                {
                    new Line(tank1.Center.X, 0, tank1.Center.X, tank1.Center.Y).Draw(Color.Black, 1);
                    new Line(tank2.Center.X, 0, tank2.Center.X, tank2.Center.Y).Draw(Color.Black, 1);
                    Mountain.Draw(gameTime, Manager.DesignWidth);
                    SpriteManager.Draw();
                    break;
                }
            case GameState.Play:
                {
                    Mountain.Draw(gameTime, Manager.DesignWidth);
                    SpriteManager.Draw();
                    break;
                }
            case GameState.End:
                {
                    Mountain.Draw(gameTime, Manager.DesignWidth);
                    SpriteManager.Draw();
                    gameEndTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (gameEndTime >= GameEndTimeout)
                    {
                        acceptEndInput = true;
                        var text = hasAi ? (tank1.Energy > 0 ? $"Level {level} finished!" : "End of game!") : $"Winner is Player {(tank1.Energy > 0 ? "1" : "2")}";
                        text = CreateCenteredText(AxeGameCollection.Games.TankBattle.ToString(), 1, text, "Press Enter or A");
                        ShowCenterText(text, AxeGameCollection.Games.TankBattle.ToString(), Color.Gold, 1);
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
        // canon up
        if (Manager.Input.HoldingKey(Keys.Up) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, new[] { Buttons.DPadUp, Buttons.LeftThumbstickUp }))
        {
            if (tank1.IsActive) tank1.WeaponUp();
        }
        if (Manager.Input.HoldingKey(Keys.Up) || Manager.Input.GamePadHoldingKey(PlayerIndex.Two, new[] { Buttons.DPadUp, Buttons.DPadUp }))
        {
            if (tank2.IsActive && !tank2.IsAi) tank2.WeaponUp();
        }
        // canon down
        if (Manager.Input.HoldingKey(Keys.Down) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, new[] { Buttons.DPadDown, Buttons.LeftThumbstickDown }))
        {
            if (tank1.IsActive) tank1.WeaponDown();
        }
        if (Manager.Input.HoldingKey(Keys.Down) || Manager.Input.GamePadHoldingKey(PlayerIndex.Two, new[] { Buttons.DPadDown, Buttons.LeftThumbstickDown }))
        {
            if (tank2.IsActive && !tank2.IsAi) tank2.WeaponDown();
        }
        // power up
        if (Manager.Input.HoldingKey(Keys.Right) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, new[] { Buttons.DPadRight, Buttons.LeftThumbstickRight }))
        {
            if (tank1.IsActive) tank1.SetWeaponPower(true);
        }
        if (Manager.Input.HoldingKey(Keys.Right) || Manager.Input.GamePadHoldingKey(PlayerIndex.Two, new[] { Buttons.DPadRight, Buttons.LeftThumbstickRight }))
        {
            if (tank2.IsActive && !tank2.IsAi) tank2.SetWeaponPower(true);
        }
        // power down
        if (Manager.Input.HoldingKey(Keys.Left) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, new[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft }))
        {
            if (tank1.IsActive) tank1.SetWeaponPower(false);
        }
        if (Manager.Input.HoldingKey(Keys.Left) || Manager.Input.GamePadHoldingKey(PlayerIndex.Two, new[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft }))
        {
            if (tank2.IsActive && !tank2.IsAi) tank2.SetWeaponPower(false);
        }
        // shoot
        if (tank1.IsActive && (Manager.Input.KeyPressed(Keys.Space) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A)))
        {
            nextTankIdx = 2;
            tank1.IsActive = false;
            tank1.Shoot();

        }
        if (tank2.IsActive)
        {
            if (tank2.AiShouldShoot || !tank2.IsAi && (Manager.Input.KeyPressed(Keys.Space) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A)))
            {
                nextTankIdx = 1;
                tank2.IsActive = false;
                tank2.Shoot();
            }
        }
        // accept messages
        if (Manager.Input.KeyPressed(Keys.Enter) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A) || Manager.Input.GamePadKeyPressed(PlayerIndex.Two, Buttons.A))
        {
            switch (state)
            {
                case GameState.Load:
                    mountainWidth = 0;
                    state = GameState.Start;
                    break;
                case GameState.End:
                    if (acceptEndInput)
                    {
                        SpriteManager.Clear();
                        ScreenManager.RemoveScreen(this);
                        ScreenManager.AddScreen(tank1.Energy <= 0 ? new ScreenMenuTankBattle(game) : new ScreenGameTankBattle(game, hasAi, level + 1));
                    }
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
            var messageBox = new ScreenMessage(game, AxeGameCollection.Games.TankBattle.ToString(), "Want to exit?", "Yes: Press Enter or A", "No: Press Escape or B", Color.Gold, Color.Red, Color.Gold);
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
        texture.Dispose();
        base.UnloadContent();
    }
}