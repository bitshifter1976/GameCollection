using System;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AxeGameCollection.Screens;

public class ScreenIntro : GameScreen
{
    private Texture2D texture;

    public ScreenIntro(Game game) : base(game)
    {
        this.game = game;
    }

    public override void LoadContent()
    {
        Manager.Sound.LoadSong("intro");
        Manager.Sound.PlaySong("intro");
        texture = Manager.Content.Load<Texture2D>("graphic/common/intro");
        base.LoadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        InitDraw(Color.Black);

        // pulsating text
        var time = gameTime.TotalGameTime.TotalSeconds;
        var pulsate = (float)Math.Sin(time * 6);
        var scale = 0.5f + pulsate * 0.025f;
        var text = "Press Enter or A to continue!";
        var font = Manager.Fonts.Get("Standard");
        var textSize = font.MeasureString(text) * scale;

        Manager.SpriteBatch.Draw(
            texture,
            new Rectangle((int)(Manager.DesignWidth/2f - texture.Width/2f), (int)(Manager.DesignHeight/2f - texture.Height/2f), texture.Width, texture.Height),
            Color.White);

        Manager.SpriteBatch.DrawString(
            font, 
            text,
            new Vector2((Manager.DesignWidth / 2f - textSize.X / 2f), Manager.DesignHeight - textSize.Y * 4), 
            Color.Black, 
            0, 
            Vector2.Zero, 
            scale, 
            SpriteEffects.None, 
            0);

        EndDraw(Color.Black);
    }

    public override void HandleInput()
    {
        if (Manager.Input.KeyPressed(Keys.Escape) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.Back))
        {
            game.Exit();
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
        if (Manager.Input.KeyPressed(Keys.Enter) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A))
        {
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new ScreenMenuMain(game));
        }
        base.HandleInput();
    }

    public override void UnloadContent()
    {
        texture.Dispose();
    }
}
