using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public class TimeBar : Sprite
{ 
    private readonly int width;
    private readonly int height;

    public TimeSpan ElapsedTime;

    public TimeBar(Vector2 position, float scale, int width, int height) : base((int)Layer.Hud)
    {
        this.position = position;
        this.scale = scale;
        this.width = width;
        this.height = height;
        color = new Color(Color.Black.R, Color.Black.G, Color.Black.B, (byte)50);
        texture = new Texture2D(Manager.Graphics.GraphicsDevice, 1, 1);
        texture.SetData(new Color[1] { Color.White });
    }

    public override Sprite Update(GameTime gameTime)
    {
        return null;
    }

    public override void Draw()
    {
        var font = Manager.Fonts.Get(AxeGameCollection.Games.SubmarineWars.ToString());
        var text = $@"Time: {ElapsedTime:mm\:ss}";
        var textSize = font.MeasureString(text) * scale;

        // background
        var backgroundRect = new Rectangle((int)position.X, (int)position.Y, width, height);
        Manager.SpriteBatch.Draw(texture, backgroundRect, null, color, 0, Vector2.Zero, SpriteEffects.None, 0);

        // total points
        Manager.SpriteBatch.DrawString(font, text, new Vector2(backgroundRect.Center.X - textSize.X/2f, backgroundRect.Center.Y-textSize.Y/2f), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
    }
}
