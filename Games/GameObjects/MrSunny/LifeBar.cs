using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public class LifeBar : Sprite
{ 
    private Rectangle backgroundRect;
    private SpriteFont font;
    private string text;
    private Vector2 textSize;
    private int width;
    private int height;
    private int points;

    public override float Width => textSize.X - 10;

    public int Points
    {
        get { return points; }
        set 
        { 
            points = value;
            CreateDrawItems();
        }
    }

    public LifeBar(Vector2 position, float scale, int width, int height) : base((int)Layer.Hud)
    {
        this.position = position;
        this.scale = scale;
        this.width = width;
        this.height = height;
        color = new Color(Color.Black.R, Color.Black.G, Color.Black.B, (byte)150);
        texture = new Texture2D(Manager.Graphics.GraphicsDevice, 1, 1);
        texture.SetData(new Color[1] { Color.White });
        CreateDrawItems();
    }

    private void CreateDrawItems()
    {
        font = Manager.Fonts.Get(AxeGameCollection.Games.SubmarineWars.ToString());
        text = $"Energy: {Points}";
        textSize = font.MeasureString(text) * scale;
        backgroundRect = new Rectangle((int)position.X, (int)position.Y, width, height);
    }

    public override Sprite Update(GameTime gameTime)
    {
        return null;
    }

    public override void Draw()
    {
        Manager.SpriteBatch.Draw(texture, backgroundRect, null, color, 0, Vector2.Zero, SpriteEffects.None, 0);
        Manager.SpriteBatch.DrawString(font, text, new Vector2(backgroundRect.Center.X - textSize.X/2f, backgroundRect.Center.Y-textSize.Y/2f), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
    }
}
