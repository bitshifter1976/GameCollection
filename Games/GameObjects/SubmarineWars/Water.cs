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
    private static int speed;
    private static float waveWidth;
    private static float waveHeight;
    private static float xOffset;
    private static float yOffset;
    private static float xDelta;
    public static Dictionary<int, int> TopPixel { get; private set; }


    public static void Load()
    {
        texture = new Texture2D(Manager.Graphics.GraphicsDevice, 1, 1);
        texture.SetData(new Color[1] { Color.White });
        color = new Color(Color.DarkBlue.R, Color.DarkBlue.G, Color.DarkBlue.B, (byte)100);
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
    }

    public static void Update(GameTime gameTime)
    {
        var delta = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        xDelta += delta;
        xOffset += delta;
        if (xOffset > Manager.DesignWidth)
            xOffset -= Manager.DesignWidth;
        if (xDelta > 1)
        {
            // shift pixel to the left (x to x-1)
            for (var i=1; i < TopPixel.Count; i++)
                TopPixel[i-1] = TopPixel[i];
            // now calculate waves y at end of list
            var y = (int)Math.Round(yOffset + Math.Sin((xOffset + Manager.DesignWidth) / waveWidth) * waveHeight, 0);
            TopPixel[TopPixel.Count - 1] = y;
            xDelta %= 1;
        }
    }

    public static void Draw(GameTime gameTime)
    {
        foreach (var p in TopPixel)
        {
            Manager.SpriteBatch.Draw(
                texture,
                new Rectangle(p.Key, p.Value, 1, Manager.DesignHeight - p.Value),
                null,
                color,
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