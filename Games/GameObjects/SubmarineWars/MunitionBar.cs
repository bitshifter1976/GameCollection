using Framework;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public class MunitionBar : Sprite
{
    private List<Torpedo> torpedos = new();
    private float torpedoWidth;
    private float torpedoHeight;
    private int munition;
    private int height;
    private readonly int Offset = 50;
    private readonly int MaxMunition;

    public override float Width => (int)(torpedoWidth * MaxMunition + Offset/2f);

    public int MunitionCount
    {
        get { return munition; }
        set
        {
            if (value >= 0)
            {
                munition = value;
                while (torpedos.Count > munition)
                    torpedos.RemoveAt(torpedos.Count - 1);

                while (torpedos.Count < value)
                    torpedos.Add(CreateIcon(torpedos.Count));
            }
        }
    }

    public MunitionBar(Vector2 position, float scale, int munitionCount, int height) : base((int)Layer.Hud)
    {
        this.position = position;
        this.scale = scale;
        this.height = height;
        color = new Color(Color.Black.R, Color.Black.G, Color.Black.B, (byte)50);
        texture = new Texture2D(Manager.Graphics.GraphicsDevice, 1, 1);
        texture.SetData(new Color[1] { Color.White });
        MaxMunition = munitionCount;
        MunitionCount = munitionCount;
    }

    private Torpedo CreateIcon(int i)
    {
        var torpedo = new Torpedo(null, position, Vector2.Zero, scale, MathHelper.ToRadians(-90), 0);
        torpedoWidth = torpedo.Height + 20 * scale;
        torpedoHeight = torpedo.Width;
        torpedo.Position += new Vector2(Offset / 2f + torpedoWidth * i, height/2f);
        return torpedo;
    }

    public override Sprite Update(GameTime gameTime)
    {
        return null;
    }

    public override void Draw()
    {
        var backgroundRect = new Rectangle((int)position.X, (int)position.Y, (int)(torpedoWidth * MaxMunition + Offset/2f), height);
        // background
        Manager.SpriteBatch.Draw(
            texture,
            backgroundRect,
            null,
            color,
            0,
            Vector2.Zero,
            SpriteEffects.None,
            0);
        torpedos.ForEach(t => t.Draw());
    }
}
