using System;
using System.Collections.Generic;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public class Water : Sprite
{
    private Color groundColor;
    private float waveWidth;
    private float waveHeight;
    private float waveSpeed;
    private float yWaveOffset;
    private static double xWaveOffset;
    private float xWaveDelta;
    private float xGroundDelta;
    private readonly int GroundYMaxOffset = 300;
    public static readonly int GroundYOffset = 150;

    public static Dictionary<int, int> TopPixel { get; private set; }
    public static Dictionary<int, int> GroundPixel { get; private set; }
    public static float MaxYTopPixel => TopPixel.Max(p => p.Value);
    public static float MinYGroundPixel => GroundPixel.Min(p => p.Value);

    public Water() : base((int)Layer.Water)
    {
        texture = Manager.Content.Load<Texture2D>("graphic/common/dot");
        color = new Color(Color.DarkBlue.R, Color.DarkBlue.G, Color.DarkBlue.B, (byte)100);
        groundColor = Color.SaddleBrown;
        waveWidth = Rand.Float(20f, 50f);
        waveHeight = Rand.Float(2f, 20f);
        waveSpeed = Rand.Float(-20f, -30f);
        xWaveOffset = 0;
        yWaveOffset = Manager.DesignHeight / 6f;
        TopPixel = new Dictionary<int, int>();
        for (var x = 0; x <= Manager.DesignWidth; x++)
        {
            var y = (int)Math.Round(yWaveOffset + Math.Sin(x / waveWidth) * waveHeight, 0);
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

    public override void ScrollX(float deltaX)
    {
        if (deltaX != 0)
        {
            ScrollXWave(deltaX);
            ScrollXGround(deltaX);
        }
    }

    private void ScrollXWave(float deltaX)
    {
        // top pixel calculation
        xWaveDelta += deltaX;
        xWaveOffset -= deltaX;
        while (xWaveDelta > 1 || xWaveDelta < -1)
        {
            int y = 0;
            if (deltaX > 0)
                y = (int)Math.Round(yWaveOffset + Math.Sin(xWaveOffset / waveWidth) * waveHeight, 0);
            if (deltaX < 0)
                y = (int)Math.Round(yWaveOffset + Math.Sin((Manager.DesignWidth + xWaveOffset) / waveWidth) * waveHeight, 0);
            ScrollPixel(deltaX, y, TopPixel, ref xWaveDelta);
        }
    }

    private void ScrollXGround(float deltaX)
    {
        // ground pixel calculation
        xGroundDelta += deltaX;
        while (xGroundDelta > 1 || xGroundDelta < -1)
        {
            int y = 0;
            if (deltaX > 0)
                y = GroundPixel[0] + Rand.Int(-5, 5);
            if (deltaX < 0)
                y = GroundPixel[Manager.DesignWidth] + Rand.Int(-5, 5);
            ScrollPixel(deltaX, y, GroundPixel, ref xGroundDelta);
        }
    }

    private static void ScrollPixel(float deltaX, int y, Dictionary<int,int> pixel, ref float xDelta)
    {
        if (deltaX < 0)
        {
            for (var x = 1; x < pixel.Count; x++)
                pixel[x - 1] = pixel[x];
            pixel[pixel.Count - 1] = y;
            xDelta++;
        }
        if (deltaX > 0)
        {
            for (var i = pixel.Count - 1; i > 0; i--)
                pixel[i] = pixel[i-1];
            pixel[0] = y;
            xDelta--;
        }
    }

    public override Sprite Update(GameTime gameTime)
    {
        ScrollXWave(waveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
        return null;
    }

    public override void Draw()
    {
        for (var x = 0; x <= Manager.DesignWidth; x++)
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