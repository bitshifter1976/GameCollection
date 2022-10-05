using System;
using System.Collections.Generic;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public class Water : Sprite
{
    private Color groundColor;
    private float waveWidth;
    private float waveHeight;
    private float yOffset;
    private static float xOffset;
    private float xDelta;
    private float xDeltaGround;
    private readonly int GroundYMaxOffset = 300;
    public static readonly int GroundYOffset = 100;

    public static Dictionary<int, int> TopPixel;
    public static Dictionary<int, int> GroundPixel;

    public Water() : base((int)Layer.Water)
    {
        texture = Manager.Content.Load<Texture2D>("graphic/common/dot");
        color = new Color(Color.DarkBlue.R, Color.DarkBlue.G, Color.DarkBlue.B, (byte)150);
        groundColor = Color.SaddleBrown;
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
            var y = oldY + Rand.Int(-5, 5);
            y = MathHelper.Clamp(y, Manager.DesignHeight - GroundYMaxOffset, Manager.DesignHeight - GroundYOffset);
            GroundPixel.Add(x, y);
            oldY = y;
        }
    }

    public override void ScrollX(float speed)
    {
        // top pixel calculation
        xDelta += speed;
        xOffset += speed;
        if (xOffset > Manager.DesignWidth)
            xOffset -= Manager.DesignWidth;
        while (xDelta > 1 || xDelta < -1)
        {
            int y = 0;
            if (speed > 0)
                y = (int)Math.Round(yOffset + Math.Sin((xOffset + Manager.DesignWidth) / waveWidth) * waveHeight, 0);
            if (speed < 0)
                y = (int)Math.Round(yOffset + Math.Sin((xOffset) / waveWidth) * waveHeight, 0);
            CalculatePixel(speed, y, ref TopPixel, ref xDelta);
        }
        // ground pixel calculation
        xDeltaGround += speed;
        while (xDeltaGround > 1 || xDeltaGround < -1)
        {
            int y = 0;
            if (speed > 0)
                y = GroundPixel[Manager.DesignWidth] + Rand.Int(-5, 5);
            if (speed < 0)
                y = GroundPixel[0] + Rand.Int(-5, 5);
            CalculatePixel(speed, y, ref GroundPixel, ref xDeltaGround);
        }
    }

    private static void CalculatePixel(float speed, int y, ref Dictionary<int,int> pixel, ref float xDelta)
    {
        if (speed < 0)
        {
            for (var i = 1; i < pixel.Count; i++)
                pixel[i - 1] = pixel[i];
            pixel[pixel.Count - 1] = y;

            if (xDelta > -2)
                xDelta %= -1;
            else
                xDelta++;
        }
        if (speed > 0)
        {
            for (var i = pixel.Count - 1; i > 0; i--)
                pixel[i] = pixel[i-1];
            pixel[0] = y;

            if (xDelta < 2)
                xDelta %= 1;
            else
                xDelta--;
        }
    }

    public override void Draw()
    {
        for (var x = 0; x < TopPixel.Count; x++)
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
}