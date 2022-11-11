using Microsoft.Xna.Framework;
using Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;
using Framework.Geometry;
using System.Collections.Generic;

namespace AxeGameCollection.GameObjects.MrSunny;

public class FloorTile2 : Sprite
{
    private Line line;
    private List<Line> fillLines;
    private RotatedRectangle boundingBox;
    private float width;
    private float height;

    public override float Width => width;

    public override float Height => height;

    public override Rectangle BoundingBox => new Rectangle((int)line.Start.X, (int)line.Start.Y, (int)Width, (int)Height);
    public override RectangleF BoundingBoxF => new RectangleF(line.Start.X, line.Start.Y, Width, height);
    public override RotatedRectangle BoundingBoxRotated => boundingBox;

    public override RotatedRectangle GetBoundingBox(Vector2 pos)
    {
        var rect = new RotatedRectangle(new RectangleF(pos.X, pos.Y, Width, Height), rotation);
        rect.Origin = pos;
        return rect;
    }

    public override Vector2 Position
    {
        get => line.Start;
        set
        {
            var diff = value - line.Start;
            if (diff != Vector2.Zero)
            {
                line.Start += diff;
                line.End += diff;
                fillLines.ForEach(l => { l.Start += diff; l.End += diff; });
            }
        }
    }

    public override void ScrollX(float speed)
    {
        Position = new Vector2(Position.X+speed,Position.Y);
    }

    public FloorTile2(Vector2 start, Vector2 end, Color color) : base("graphic/mrSunny/gap", start, 0, 1, color, (int)Layer.Beach, CollisionType.BoundingBoxRotated)
    {
        scrolling = true;
        fillLines = new List<Line>();
        line = new Line(start.X, start.Y, end.X, end.Y);
        var length = Vector2.Distance(end, start);
        width = length;
        height = 50;
        rotation = Angle.CalculeAngleInRadians(start, end);
        boundingBox = new RotatedRectangle(BoundingBoxF, rotation);
        boundingBox.Origin = start; 
        foreach (var p in line.GetPoints((int)length))
            fillLines.Add(new Line(p.X, p.Y, p.X, Manager.DesignHeight));
    }

    public override void Draw()
    {
        if (line.Start.X >= 0 || line.End.X <= Manager.DesignWidth)
        {
            line.Draw(color, 1);
            fillLines.ForEach(l => l.Draw(color, 1));
            if (Manager.Debug)
            {
                Debug.Rects.Add(new RectDebug(BoundingBoxRotated.ToLines(), "BoundingBox", Color.Red));
                Debug.Points.Add(new PointDebug(new Dot(Center), "Center", Color.Gold));
                Debug.Points.Add(new PointDebug(new Dot(position), "Position", Color.Green));
                Manager.SpriteBatch.DrawString(Manager.Fonts.Get("Debug"), $"{position}({this.GetType().Name})", new Vector2(position.X, position.Y + Height + 2), Color.Black);
            }
        }
    }
}
