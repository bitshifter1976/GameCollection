using System;
using System.Collections.Generic;
using System.Linq;
using AxeGameCollection.GameObjects.ArenaChase;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AxeGameCollection.Screens;

public class ScreenGameArenaChase : GameScreen
{
    public enum GameState
    {
        Load,
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

    private GameState state;
    private readonly bool hasAi;
    private readonly int level;
    private Color backColor;
    private Tank tank;
    private List<Tank> enemies;
    private LifeBar lifeBar;
    private float gameEndTime = 0;
    private bool acceptEndInput;
    private const float GameEndTimeout = 2;

    public ScreenGameArenaChase(Game game, bool ai, int level) : base(game)
    {
        state = GameState.Load;
        hasAi = ai;
        this.level = level;

        Manager.Input.CreateHoldingKeys(0.05f, Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.W, Keys.A, Keys.S, Keys.D);
        Manager.Input.CreateHoldingButtons(0.05f, Buttons.DPadLeft, Buttons.DPadRight, Buttons.RightTrigger, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight);
    }

    public override void LoadContent()
    {
        Manager.Sound.LoadSong("game");
        Manager.Sound.PlaySong("game");
        CreateScene(level);
        base.LoadContent();
    }

    private void CreateScene(int enemyCount)
    {
        backColor = Rand.Color(Color.Brown, Color.RosyBrown, Color.SaddleBrown, Color.SandyBrown);
        lifeBar = new LifeBar(2f) { Percentage = LifeBar.MaxValue };
        lifeBar.Position = new Vector2(Manager.DesignWidth / 2f - lifeBar.Width / 2f, 10);
        SpriteManager.AddImmediate(lifeBar);
        tank = new Tank(new Vector2(Manager.DesignWidth / 2f, Manager.DesignHeight / 2f), 0.4f, 0,false);
        enemies = CreateEnemies(hasAi ? enemyCount : 1);
        SpriteManager.AddImmediate(tank);
        enemies.ForEach(e => SpriteManager.AddImmediate(e));
        var notToCollideWith = new List<Sprite> { tank };
        notToCollideWith.AddRange(enemies);
        var trees = Trees.Create(Rand.Int(0, 20), notToCollideWith);
        SpriteManager.AddImmediate(trees);
        notToCollideWith.AddRange(trees);
        var stones = Stones.Create(Rand.Int(0, 20), notToCollideWith);
        SpriteManager.AddImmediate(stones); 
        notToCollideWith.AddRange(stones);
        var mines = Mines.Create(Rand.Int(1, 10), notToCollideWith);
        SpriteManager.AddImmediate(mines);
    }

    private List<Tank> CreateEnemies(int enemyCount)
    {
        var list = new List<Tank>();
        for (var i = 0; i < enemyCount; i++)
        {
            var scale = Rand.Float(0.3f, 0.5f);
            var rotation = Rand.Float(0f, 1f);
            var enemy = new Tank(Vector2.Zero, scale, rotation, true, tank);
            var x = Rand.Int(50, Manager.DesignWidth - (int)enemy.Width - 50);
            var y = Rand.Int(50, Manager.DesignHeight - (int)enemy.Height - 50);
            enemy.Position = new Vector2(x, y);
            if (tank.Collide(enemy) || list.Any(e => e.Collide(enemy)))
                enemyCount++;
            else
                list.Add(enemy);
        }
        return list;
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

                if (tank.Energy <= 0)
                    state = GameState.End;

                lifeBar.Percentage = tank.Energy;

                if (hasAi)
                {
                    enemies.Where(e => e.Energy <= 0).ToList().ForEach(e => enemies.Remove(e));
                    if (enemies.Count == 0)
                        state = GameState.End;
                }
                else
                {
                    if (enemies[0].Energy <= 0)
                        state = GameState.End;
                }
                SpriteManager.Update(gameTime);
                break;
            }
            case GameState.End:
            {
                SpriteManager.Update(gameTime);
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
                tank.Draw();
                if (!hasAi) 
                    enemies[0].Draw();
                var str = $"Get ready!{(hasAi ? $"\nLevel {level}" : "")}";
                ShowCenterText(str, AxeGameCollection.Games.ArenaChase.ToString(), Color.Gold, 1);
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
                    var str = hasAi ? (tank.Energy > 0 ? $"Level {level} finished!" : "End of game!") : $"Winner is Player {(tank.Energy > 0 ? "1" : "2")}";
                    ShowCenterText(str, AxeGameCollection.Games.ArenaChase.ToString(), Color.Gold, 1);
                }
                break;
            }
        }
        EndDraw(Color.Black);
    }

    public override void HandleInput()
    {
        // helping keys
        if (Manager.Input.KeyPressed(Keys.Escape) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, new[] { Buttons.Start, Buttons.Back }))
        {
            ExitGame();
        }
        if (Manager.Input.KeyPressed(Keys.F1))
        {
            SpriteManager.Clear();
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new ScreenGameArenaChase(game, true, level + 1));
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
        // steer right
        if (hasAi)
        {
            if (Manager.Input.HoldingKey(Keys.Right) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, new[] { Buttons.DPadRight, Buttons.LeftThumbstickRight }))
                tank.Rotate(false);
        }
        else
        {
            if (Manager.Input.HoldingKey(Keys.D) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, new[] { Buttons.DPadRight, Buttons.LeftThumbstickRight }))
                tank.Rotate(false);
            if (Manager.Input.HoldingKey(Keys.Right) || Manager.Input.GamePadHoldingKey(PlayerIndex.Two, new[] { Buttons.DPadRight, Buttons.LeftThumbstickRight }))
                enemies[0].Rotate(false);
        }

        // steer left
        if (hasAi)
        {
            if (Manager.Input.HoldingKey(Keys.Left) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, new[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft}))
                tank.Rotate(true);
        }
        else
        {
            if (Manager.Input.HoldingKey(Keys.A) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, new[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft }))
                tank.Rotate(true);
            if (Manager.Input.HoldingKey(Keys.Left) || Manager.Input.GamePadHoldingKey(PlayerIndex.Two, new[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft }))
                enemies[0].Rotate(true);
        }
        // speed up
        if (hasAi)
        {
            if (Manager.Input.HoldingKey(Keys.Up) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, Buttons.RightTrigger))
                tank.SetSpeed(true);
        }
        else
        {
            if (Manager.Input.HoldingKey(Keys.W) || Manager.Input.GamePadHoldingKey(PlayerIndex.One, Buttons.RightTrigger))
                tank.SetSpeed(true);
            if (Manager.Input.HoldingKey(Keys.Up) || Manager.Input.GamePadHoldingKey(PlayerIndex.Two, Buttons.RightTrigger))
                enemies[0].SetSpeed(true);
        }
        // speed down
        if (hasAi)
        {
            if (!Manager.Input.HoldingKey(Keys.Up) && !Manager.Input.GamePadHoldingKey(PlayerIndex.One, Buttons.RightTrigger))
                tank.SetSpeed(false);
        }
        else
        {
            if (!Manager.Input.HoldingKey(Keys.S) && !Manager.Input.GamePadHoldingKey(PlayerIndex.One, Buttons.RightTrigger))
                tank.SetSpeed(false);
            if (!Manager.Input.HoldingKey(Keys.Down) && !Manager.Input.GamePadHoldingKey(PlayerIndex.Two, Buttons.RightTrigger))
                enemies[0].SetSpeed(false);
        }
        // shoot
        if (hasAi)
        {
            if (Manager.Input.KeyPressed(Keys.Space) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A))
                tank.Shoot();
        }
        else
        {
            if (Manager.Input.KeyPressed(Keys.Enter) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A))
                tank.Shoot();
            if (Manager.Input.KeyPressed(Keys.Space) || Manager.Input.GamePadKeyPressed(PlayerIndex.Two, Buttons.A))
                enemies[0].Shoot();
        }
        // place mine
        if (hasAi)
        {
            if (Manager.Input.KeyPressed(Keys.M) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.B))
                tank.PlaceMine();
        }
        else
        {
            if (Manager.Input.KeyPressed(Keys.RightShift) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.B))
                tank.PlaceMine();
            if (Manager.Input.KeyPressed(Keys.M) || Manager.Input.GamePadKeyPressed(PlayerIndex.Two, Buttons.B))
                enemies[0].PlaceMine();
        }
        // navigation
        if (Manager.Input.KeyPressed(Keys.Enter) ||  Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A))
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
                        ScreenManager.AddScreen(tank.Energy <= 0 ? new ScreenMenuMain(game) : new ScreenGameArenaChase(game, hasAi, level + 1));
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
}