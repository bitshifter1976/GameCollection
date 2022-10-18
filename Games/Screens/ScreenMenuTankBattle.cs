using AxeGameCollection;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AxeGameCollection.Screens;

public class ScreenMenuTankBattle : MenuScreen
{
    private Texture2D texture;

    public ScreenMenuTankBattle(Game game) : base(game, "Tank Battle")
    {
        var playGame1MenuEntry = new MenuEntry("Play against AI");
        var playGame2MenuEntry = new MenuEntry("Play against human");
        var playGame3MenuEntry = new MenuEntry("Controls");
        var exitMenuEntry = new MenuEntry("Back");

        playGame1MenuEntry.Selected += (s, e) =>
        {
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new ScreenGameTankBattle(game, true, Config.LevelTankBattle));
        };
        playGame2MenuEntry.Selected += (s, e) =>
        {
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new ScreenGameTankBattle(game, false, Config.LevelTankBattle));
        };
        playGame3MenuEntry.Selected += (s, e) =>
        {
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new ScreenControlsTankBattle(game));
        };
        exitMenuEntry.Selected += (s, e) =>
        {
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new ScreenMenuMain(game));
        };

        menuEntries.Add(playGame1MenuEntry);
        menuEntries.Add(playGame2MenuEntry);
        menuEntries.Add(playGame3MenuEntry);
        menuEntries.Add(exitMenuEntry);
    }

    public override void LoadContent()
    {
        Manager.Sound.LoadEffect("menu_start");
        Manager.Sound.PlayEffect("menu_start");
        Manager.Sound.LoadEffect("menu_next");
        Manager.Sound.LoadEffect("menu_open");
        texture = Manager.Content.Load<Texture2D>("graphic/common/menu");

        base.LoadContent();
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
        // Move to the previous menu entry
        if (Manager.Input.KeyPressed(Keys.Up) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.DPadUp, Buttons.LeftThumbstickUp))
        {
            selectedEntry--;
            if (selectedEntry < 0)
                selectedEntry = menuEntries.Count - 1;
            Manager.Sound.PlayEffect("menu_next");
        }
        // Move to the next menu entry
        if (Manager.Input.KeyPressed(Keys.Down) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.DPadDown, Buttons.LeftThumbstickDown))
        {
            selectedEntry++;
            if (selectedEntry >= menuEntries.Count)
                selectedEntry = 0;
            Manager.Sound.PlayEffect("menu_next");
        }
        // Accept the menu
        if (Manager.Input.KeyPressed(Keys.Enter) || Manager.Input.GamePadKeyPressed(PlayerIndex.One, Buttons.A))
        {
            OnSelectEntry(selectedEntry);
            Manager.Sound.PlayEffect("menu_open");
        }
    }

    public override void Draw(GameTime gameTime)
    {
        InitDraw(Color.Black);
        Manager.SpriteBatch.Draw(texture, new Rectangle(0, 0, texture.Width, texture.Height), Color.White);
        // Draw the menu title.
        const float titleScale = 1f;
        const float entryScale = 0.7f;
        var position = new Vector2(Manager.DesignWidth / 2f, Manager.DesignHeight / 2f);
        var titleColor = Color.Black;
        var font = Manager.Fonts.Get("Standard");
        var titleSize = font.MeasureString(menuTitle) * titleScale;
        position.Y -= titleSize.Y * 3;
        var titlePosition = new Vector2(position.X - titleSize.X / 2f - 350, position.Y);
        Manager.SpriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0, Vector2.Zero, titleScale, SpriteEffects.None, 0);
        // Draw each menu entry in turn.
        position.Y += titleSize.Y * 3;
        for (var i = 0; i < menuEntries.Count; i++)
        {
            var menuEntry = menuEntries[i];
            var isSelected = (i == selectedEntry);
            var x = position.X - menuEntry.GetWidth(entryScale) / 2f;
            menuEntry.Draw(this, new Vector2(x, position.Y), isSelected, gameTime, entryScale, Color.DarkRed, Color.Black);
            position.Y += menuEntry.GetHeight(entryScale);
        }
        EndDraw(Color.Black);
    }
}
