using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework
{
    public class ScreenManager : DrawableGameComponent
    {
        private static readonly List<GameScreen> Screens = new ();

        public ScreenManager(Game game) : base(game)
        {
            Enabled = true;
            game.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            // Make a copy of the master screen list, to avoid confusion if the process of updating one screen adds or removes others.
            var screensToUpdate = Screens.ToList();
            var otherScreenHasFocus = !Game.IsActive;
            var coveredByOtherScreen = false;
            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                var idx = screensToUpdate.Count - 1;
                var screen = screensToUpdate[idx];
                screensToUpdate.RemoveAt(idx);
                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                // If this is the first active screen we came across, give it a chance to handle input
                if (!otherScreenHasFocus)
                {
                    screen.HandleInput();
                    otherScreenHasFocus = true;
                }
                // If this is an active non-popup, inform any subsequent screens that they are covered by it.
                if (!screen.IsPopup)
                    coveredByOtherScreen = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Screens.ForEach(screen => screen.Draw(gameTime));
        }

        public static void AddScreen(GameScreen screen)
        {
            screen.LoadContent();
            Screens.Add(screen);
        }

        public static void RemoveScreen(GameScreen screen)
        {
            screen.UnloadContent();
            Screens.Remove(screen);
        }
    }
}
