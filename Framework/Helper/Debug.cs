using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Framework;

public class LineDebug
{
    public Line Line { get; set; }
    public string Name { get; set; }
    public Color Color { get; set; }

    public LineDebug(Line line, string name, Color color)
    {
        Line = line;
        Name = name;
        Color = color;
    }
}

public class PointDebug
{
    public Dot Point { get; set; }
    public string Name { get; set; }
    public Color Color { get; set; }
    public PointDebug(Dot point, string name, Color color)
    {
        Point = point;
        Name = name;
        Color = color;
    }
}

public class RectDebug
{
    public List<Line> Lines { get; set; }
    public string Name { get; set; }
    public Color Color { get; set; }
    public RectDebug(List<Line> lines, string name, Color color)
    {
        Lines = lines;
        Name = name;
        Color = color;
    }
}

public class EllipseDebug
{
    public Ellipse Ellipse { get; set; }
    public string Name { get; set; }
    public Color Color { get; set; }
    public int Resolution { get; set; }
    public EllipseDebug(Ellipse ellipse, string name, Color color, int resolution)
    {
        Ellipse = ellipse;
        Name = name;
        Color = color;
        Resolution = resolution;
    }
}

public static class Debug
{
    private static Vector2 shapeInfoPos;
    private static float scale;
    private static SpriteFont font;

    public static string DebugText { get; set; }
    public static Vector2 DebugTextPos { get; private set; }
    public static Vector2 ShapeInfoPosOrig { get; private set; }
    public static float DebugTextScale { get; private set; }
    public static List<LineDebug> Lines { get; private set; }
    public static Vector2 ShapeInfoPos
    {
        get { return shapeInfoPos; }
        private set { shapeInfoPos = value; }
    }
    public static List<PointDebug> Points { get; set; }
    public static List<RectDebug> Rects { get; set; }
    public static List<EllipseDebug> Ellipses { get; set; }
    public static bool ShowText { get; private set; }

    static Debug()
    {
        Ellipses = new List<EllipseDebug>();
        Rects = new List<RectDebug>();
        Points = new List<PointDebug>();
        Lines = new List<LineDebug>();
        DebugText = "";
    }

    public static void Create(Vector2 debugTextPos, Vector2 shapeInfoPos, bool showText, string fontName, float textScale = 1)
    {
        DebugTextPos = debugTextPos;
        ShapeInfoPosOrig = ShapeInfoPos = shapeInfoPos;
        ShowText = showText;
        scale = textScale;
        font = Manager.Fonts.Get(fontName);
    }

    public static void Clear()
    {
        DebugTextScale = 1;
        DebugText = "";
        Lines.Clear();
        Points.Clear();
        Rects.Clear();
        Ellipses.Clear();
    }

    public static void Log(string text)
    {
        if (ShowText)
        {
            DebugText += text.Replace('{', '[').Replace('}', ']') + "\n";
        }
    }

    public static void Draw()
    {
        ShapeInfoPos = ShapeInfoPosOrig;
        Manager.SpriteBatch.DrawString(Manager.Fonts.Get("Debug"), DebugText, DebugTextPos, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        TextOut("------POINTS------", Color.White);
        foreach (PointDebug p in Points)
        {
            p.Point.Draw(p.Color);
            TextOut(p.Name + "   " + p.Point, p.Color);
        }
			TextOut("------LINES------", Color.White);
        foreach (LineDebug l in Lines)
        {
            l.Line.Draw(l.Color, 1);
            TextOut(l.Name + "   " + l.Line, l.Color);
        }
			TextOut("------RECTS------", Color.White);
        foreach (RectDebug r in Rects)
        {
            foreach (Line l in r.Lines)
                l.Draw(r.Color, 1);
            TextOut(r.Name, r.Color);
        }
			TextOut("------ELLIPSES------", Color.White);
        foreach (EllipseDebug e in Ellipses)
        {
            e.Ellipse.Draw(e.Color, e.Resolution);
            TextOut(e.Name + "   " + e.Ellipse, e.Color);
        }
    }

    private static void TextOut(string text, Color color)
    {
        if (ShowText)
        {
            Manager.SpriteBatch.DrawString(font, text, ShapeInfoPos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            shapeInfoPos.Y += font.MeasureString(text).Y * scale;
        }
    }
}
