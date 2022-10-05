using System.Collections.Generic;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AxeGameCollection.Screens;

public class ScreenControlsTankBattle : GameScreen
{
    private Texture2D texture;

    public ScreenControlsTankBattle(Game game) : base(game)
    {
        this.game = game;
    }

    public override void LoadContent()
    {
        texture = Manager.Content.Load<Texture2D>("graphic/background_menu");
        base.LoadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        InitDraw(Color.Black);

        // background
        Manager.SpriteBatch.Draw(texture, new Rectangle((int)(Manager.DesignWidth/2f - texture.Width/2f), (int)(Manager.DesignHeight/2f - texture.Height/2f), texture.Width, texture.Height), Color.White);
        // draw table
        var font = Manager.Fonts.Get(AxeGameCollection.Games.TankBattle.ToString());
        var rows = new[]
        {
            new []{ "Player", "Action",     "Keyboard", "Gamepad" },
            new []{ "",       "",           "",         "" },
            new []{ "1",      "Canon up",   "Up",       "      DPadUp\nLeftThumbstickUp" },
            new []{ "",       "Canon down", "Down",     "      DPadDown\nLeftThumbstickDown" },
            new []{ "",       "Power up",   "Right",    "      DPadRight\nLeftThumbstickRight" },
            new []{ "",       "Power down", "Left",     "      DPadLeft\nLeftThumbstickLeft" },
            new []{ "",       "Shoot",      "Space",    "A" },
            new []{ "",       "", "", "" },
            new []{ "2",      "Canon up",   "Up",       "      DPadUp\nLeftThumbstickUp" },
            new []{ "",       "Canon down", "Down",     "      DPadDown\nLeftThumbstickDown" },
            new []{ "",       "Power up",   "Right",    "      DPadRight\nLeftThumbstickRight" },
            new []{ "",       "Power down", "Left",     "      DPadLeft\nLeftThumbstickLeft" },
            new []{ "",       "Shoot",      "Space",    "A" },
            new []{ "",       "", "", "" },
        };
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
                var textYOffset = text.Contains("\n") || rowIndex == 0 ? 0 : 20;
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
