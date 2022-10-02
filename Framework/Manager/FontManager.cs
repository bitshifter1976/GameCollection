using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework
{ 
    public class FontManager
    {
        private readonly Dictionary<string, SpriteFont> fonts = new();

        public FontManager(string dir, List<string> fontNames)
        {
            fontNames.ForEach(f => fonts.Add(f, Manager.Content.Load<SpriteFont>($"{dir}/{f}")));
        }

        public SpriteFont Get(string font)
        {
            return fonts[font];
        }
    }
}
