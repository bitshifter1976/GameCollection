using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AxeGameCollection.Screens;

public class ScreenControlsArenaChase : GameScreen
{
    private SpriteFont font;
    private readonly string[][] rows = new[]
    {
        new []{ "Player", "Action",      "Keyboard",   "Gamepad" },
        new []{ "",       "",            "",           "" },
        new []{ "1",      "Steer left",  "A",          "      DPadLeft\nLeftThumbstickLeft" },
        new []{ "",       "Steer right", "D",          "      DPadRight\nLeftThumbstickRight" },
        new []{ "",       "Accelerate",  "W",          "RightTrigger" },
        new []{ "",       "Shoot",       "Space",      "A" },
        new []{ "",       "Place Mine",  "M",          "B" },
        new []{ "",       "", "", "" },
        new []{ "2",      "Steer left",  "Left",       "     DPadLeft\nLeftThumbstickLeft" },
        new []{ "",       "Steer right", "Right",      "     DPadRight\nLeftThumbstickRight" },
        new []{ "",       "Accelerate",  "Up",         "RightTrigger" },
        new []{ "",       "Shoot",       "Enter",      "A" },
        new []{ "",       "Place Mine",  "RightShift", "B" },
        new []{ "",       "",            "",           "" },

    };

    public ScreenControlsArenaChase(Game game) : base(game)
    {
        this.game = game;
    }

    public override void LoadContent()
    {
        font = Manager.Fonts.Get(AxeGameCollection.Games.ArenaChase.ToString());
        base.LoadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        InitDraw(Color.WhiteSmoke);

        // draw table
        var rowCount = rows.Length;
        var colCount = rows[0].Length;
        var colWidth = (float)Manager.DesignWidth / colCount;
        var rowHeight = (float)Manager.DesignHeight / rowCount;
        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            new Line(1, 1 + rowIndex * rowHeight, Manager.DesignWidth - 1, 1 + rowIndex * rowHeight).Draw(Color.Black, 1);
            for (var colIndex = 0; colIndex < colCount; colIndex++)
            {
                var text = rows[rowIndex][colIndex];
                var textYOffset = text.Contains('\n') || rowIndex == 0 ? 0 : 20;
                var scale = 0.35f;
                if (rowIndex == 0)
                    scale = 0.7f;
                var textSize = font.MeasureString(text) * scale;
                var x = colIndex * colWidth + colWidth / 2f - textSize.X / 2f;
                var y = rowIndex * rowHeight;
                new Line(1 + colIndex * colWidth, y, 1 + colIndex * colWidth, y + rowHeight).Draw(Color.Black, 1);
                Manager.SpriteBatch.DrawString(font, text, new Vector2(x, y+textYOffset), Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }

        EndDraw(Color.Black);
    }

    public override void HandleInput()
    {
        if (Manager.Input.KeyPressed(Keys.F2))
        {
            Manager.Debug = !Manager.Debug;
        }
        if (Manager.Input.KeyPressed(Keys.F3))
        {
            Manager.Graphics.IsFullScreen = !Manager.Graphics.IsFullScreen;
            Manager.Graphics.ApplyChanges();
        }
        // enter
        if (Manager.Input.KeyPressed(Keys.Enter, Keys.Escape) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A))
        {
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new ScreenMenuMain(game));
        }
    }
}
