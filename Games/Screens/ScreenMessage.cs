using System;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AxeGameCollection.Screens;

public class ScreenMessage : GameScreen
{
    private readonly Color textColor;
    private readonly Color backColor;
    private readonly Color borderColor;
    private Texture2D texture;
    private readonly string message;
    public event EventHandler<EventArgs> Accepted;
    public event EventHandler<EventArgs> Cancelled;
    private readonly bool cancelEnabled = true;

    public ScreenMessage(Game game, string message, string acceptText, string cancelText, Color textColor, Color backColor, Color borderColor) : base(game)
    {
        this.textColor = textColor;
        this.backColor = backColor;
        this.borderColor = borderColor;
        this.message = message + "\n\n";
        if (acceptText != "")
            this.message += acceptText + "\n";
        if (cancelText != "")
            this.message += cancelText;
        else
            cancelEnabled = false;

        IsPopup = true;
    }

    public override void LoadContent()
    {
        texture = Manager.Content.Load<Texture2D>("graphic/common/blank_white");
        base.LoadContent();
    }

    public override void  HandleInput()
    {
        if (Manager.Input.KeyPressed(Keys.Enter) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A))
        {
            Accepted?.Invoke(this, EventArgs.Empty);
            ScreenManager.RemoveScreen(this);
        }
        else if (cancelEnabled && (Manager.Input.KeyPressed(Keys.Escape) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.B, Buttons.Back)))
        {
            Cancelled?.Invoke(this, EventArgs.Empty);
            ScreenManager.RemoveScreen(this);
        }  
    }

    public override void Draw(GameTime gameTime)
    {
        // center the message text in the viewport
        const float scale = 0.5f;
        var font = Manager.Fonts.Get("Standard");
        var textSize = font.MeasureString(message) * scale;
        var position = new Vector2(Manager.Graphics.PreferredBackBufferWidth/2f - textSize.X/2f, Manager.Graphics.PreferredBackBufferHeight / 2f - textSize.Y/2f);
        // the background includes a border somewhat larger than the text itself
        const int hPad = 32;
        const int vPad = 16;
        const int borderWidth = 3;
        var backgroundRect = new RectangleF(position.X - hPad, position.Y - vPad, textSize.X + hPad * 2, textSize.Y + vPad * 2);
        var borderRect = new RectangleF(backgroundRect.Left - borderWidth, backgroundRect.Top - borderWidth, backgroundRect.Width + borderWidth * 2, backgroundRect.Height + borderWidth * 2);
        // draw the background rectangle and border
        Manager.SpriteBatch.Begin(SpriteSortMode.Immediate);
        Manager.SpriteBatch.Draw(texture, borderRect.ToRectangle(), borderColor);
        Manager.SpriteBatch.Draw(texture, backgroundRect.ToRectangle(), backColor);
        // draw the message box text
        Manager.SpriteBatch.DrawString(font, message, position, textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        Manager.SpriteBatch.End();
    }
}
