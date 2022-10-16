using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework
{
    public abstract class GameScreen
    {
        protected Game game;
        protected RenderTarget2D renderTarget;

        public bool IsPopup { get; set; }

        protected GameScreen(Game game)
        {
            this.game = game;
        }

        public virtual void LoadContent() 
        {
            renderTarget = new RenderTarget2D(Manager.Graphics.GraphicsDevice, Manager.DesignWidth, Manager.DesignHeight);
        }

        public virtual void UnloadContent() 
        {
            renderTarget.Dispose();
        }


        public virtual void HandleInput() 
        { 
        }

        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            Manager.Update(gameTime);
        }

        protected void InitDraw(Color color)
        {
            // draw to render target
            Manager.Graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            Manager.Graphics.GraphicsDevice.Clear(color);
            Manager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        protected void EndDraw(Color color)
        {
            Manager.SpriteBatch.End();
            // draw render target to back buffer
            Manager.Graphics.GraphicsDevice.SetRenderTarget(null);
            Manager.Graphics.GraphicsDevice.Clear(color);
            Manager.SpriteBatch.Begin();
            if (renderTarget != null)
                Manager.SpriteBatch.Draw(renderTarget, CalculateDestinationRectangle(), Color.White);
            Manager.SpriteBatch.End();
        }

        private static Rectangle CalculateDestinationRectangle()
        {
            var backbufferBounds = Manager.Graphics.GraphicsDevice.PresentationParameters.Bounds;
            var backbufferAspectRatio = (float)backbufferBounds.Width / backbufferBounds.Height;
            var screenAspectRatio = (float)Manager.DesignWidth / Manager.DesignHeight;
            var rect = new RectangleF(0, 0, backbufferBounds.Width, backbufferBounds.Height);

            if (backbufferAspectRatio > screenAspectRatio)
            {
                rect.Width = rect.Height * screenAspectRatio;
                rect.X = (backbufferBounds.Height - rect.Height) / 2f;
            }
            else if (backbufferAspectRatio < screenAspectRatio)
            {
                rect.Height = rect.Width / screenAspectRatio;
                rect.Y = (backbufferBounds.Height - rect.Height) / 2f;
            }

            return rect.ToRectangle();
        }

        public static void ShowCenterText(string text, string fontName, Color color, int scale)
        {
            var font = Manager.Fonts.Get(fontName);
            var textSize = font.MeasureString(text) * scale;
            var pos = new Vector2(Manager.DesignWidth / 2f - textSize.X / 2f, Manager.DesignHeight / 2f - textSize.Y / 2f);
            Manager.SpriteBatch.DrawString(font, text, pos, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
}
