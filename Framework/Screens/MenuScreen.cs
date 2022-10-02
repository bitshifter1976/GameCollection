using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework;

public abstract class MenuScreen : GameScreen
{
    protected List<MenuEntry> menuEntries = new();
    protected int selectedEntry = 0;
    protected string menuTitle;

    protected MenuScreen(Game game, string menuTitle) : base(game)
    {
        this.menuTitle = menuTitle;
    }

    protected virtual void OnSelectEntry(int entryIndex)
    {
        menuEntries[entryIndex].OnSelectEntry();
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        for (var i = 0; i < menuEntries.Count; i++)
        {
            menuEntries[i].Update(this, i == selectedEntry, gameTime);
        }
    }
}
