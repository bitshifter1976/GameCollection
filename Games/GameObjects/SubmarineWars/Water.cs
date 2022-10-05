using System;
using System.Collections.Generic;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public static class Water
{
    private static Texture2D texture;
    private static Color color;
    private static Color groundColor;
    private static int speed;
    private static float waveWidth;
    private static float waveHeight;
    private static float xOffset;
    private static float yOffset;
    private static float xDelta;
    private static int GroundYOffset = 200;
    private static int GroundYMaxOffset = 100;
    private static float xDeltaGround;

    public static Dictionary<int, int> TopPixel { get; private set; }
    public static Dictionary<int, int> GroundPixel { get; private set; }


    public static void Load()
    {
        texture = new Texture2D(Manager.Graphics.GraphicsDevice, 1, 1);
        texture.SetData(new Color[1] { Color.White });
        color = new Color(Color.DarkBlue.R, Color.DarkBlue.G, Color.DarkBlue.B, (byte)100);
        groundColor = Color.DarkGray;
        speed = Rand.Int(20, 70);
        waveWidth = Rand.Float(20f, 50f);
        waveHeight = Rand.Float(2f, 20f);
        yOffset = Manager.DesignHeight / 6f;
        TopPixel = new Dictionary<int, int>();
        for (var x = 0; x <= Manager.DesignWidth; x++)
        {
            var y = (int)Math.Round(yOffset + Math.Sin((xOffset + x) / waveWidth) * waveHeight, 0);
            TopPixel.Add(x, y);
        }
        GroundPixel = new Dictionary<int, int>();
        var oldY = Manager.DesignHeight - GroundYOffset;
        for (var x = 0; x <= Manager.DesignWidth; x++)
        {
            var y = oldY + Rand.Int(-5,5);
            y = MathHelper.Clamp(y, Manager.DesignHeight - GroundYOffset, Manager.DesignHeight - GroundYMaxOffset);
            GroundPixel.Add(x, y);
            oldY = y;
        }
    }

    public static void Update(GameTime gameTime)
    {
        var delta = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        // top pixel calculation
        xDelta += delta;
        xOffset += delta;
        if (xOffset > Manager.DesignWidth)
            xOffset -= Manager.DesignWidth;
        if (xDelta > 1)
        {
            for (var i=1; i < TopPixel.Count; i++)
                TopPixel[i-1] = TopPixel[i];
            // now calculate waves y at end of list
            var y = (int)Math.Round(yOffset + Math.Sin((xOffset + Manager.DesignWidth) / waveWidth) * waveHeight, 0);
            TopPixel[TopPixel.Count - 1] = y;
            xDelta %= 1;
        }
        // ground pixel calculation
        xDeltaGround += delta / 2f;
        if (xDeltaGround > 1)
        {
            for (var i = 1; i < GroundPixel.Count; i++)
                GroundPixel[i - 1] = GroundPixel[i];
            var y = GroundPixel[Manager.DesignWidth] + Rand.Int(-5, 5);
            GroundPixel[GroundPixel.Count - 1] = y;
            xDeltaGround %= 1;
        }
    }

    public static void Draw(GameTime gameTime)
    {
        for (var x=0; x < TopPixel.Count; x++)
        {
            Manager.SpriteBatch.Draw(
                texture,
                new Rectangle(x, TopPixel[x], 1, GroundPixel[x] - TopPixel[x]),
                null,
                color,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                0);

            Manager.SpriteBatch.Draw(
                texture,
                new Rectangle(x, GroundPixel[x], 1, Manager.DesignHeight - GroundPixel[x]),
                null,
                groundColor,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                0);
        }
    }

    public static void Unload()
    {
        texture.Dispose();
    }
}