using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace Framework
{
	public static class Manager
	{
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static ContentManager Content { get; private set; }
        public static SpriteBatch SpriteBatch { get; private set; }
        public static InputManager Input { get; private set; }
        public static SoundManager Sound { get; private set; }
        public static ScreenManager Screens { get; private set; }
        public static FontManager Fonts { get; private set; }
        public static int DesignWidth { get; private set; }
        public static int DesignHeight { get; private set; }
        public static bool Debug { get; set; }

        public static void Create(ContentManager contentManager, GraphicsDeviceManager manager, Game game, bool soundEnabled, Size designSize, bool debug, string fontSubDirectory, List<string> fontNames)
		{
			Content = contentManager;
			Manager.Graphics = manager;
			SpriteBatch = new SpriteBatch(manager.GraphicsDevice);
			Input = new InputManager(game);
            Sound = new SoundManager(game) { Enabled = soundEnabled };
            Screens = new ScreenManager(game);
            Fonts = new FontManager(fontSubDirectory, fontNames);
            DesignWidth = designSize.Width;
            DesignHeight = designSize.Height;
            Debug = debug;
        }

		public static void Update(GameTime gameTime)
		{
			Sound.Update(gameTime);
		}
	}
}