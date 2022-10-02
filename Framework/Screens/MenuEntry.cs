using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework
{
    public class MenuEntry
    {
        private readonly string text;
        private float selectionFade;
        public event EventHandler<EventArgs> Selected;
        
        public MenuEntry(string text)
        {
            this.text = text;
        }

        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            var fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
            selectionFade = isSelected ? Math.Min(selectionFade + fadeSpeed, 1) : Math.Max(selectionFade - fadeSpeed, 0);
        }

        public virtual void Draw(MenuScreen screen, Vector2 position, bool isSelected, GameTime gameTime, float scale, Color colorSelected, Color colorUnselected)
        {
            var font = Manager.Fonts.Get("Standard");
            // Draw the selected entry in yellow, otherwise white.
            var color = isSelected ? colorSelected : colorUnselected;
            // Draw text, centered on the middle of each line.
            var origin = new Vector2(0, font.LineSpacing * scale / 2);
            //var t = new Text();
            Manager.SpriteBatch.DrawString(font, text, position, color, 0, origin, scale, SpriteEffects.None, 0);
        }

        public virtual int GetHeight(float scale)
        {
            return (int)(Manager.Fonts.Get("Standard").LineSpacing * scale);
        }

        public virtual int GetWidth(float scale)
        {
            return (int)(Manager.Fonts.Get("Standard").MeasureString(text).X * scale);
        }

        public virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty);
        }
    }
}
