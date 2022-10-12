using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameTankBattle;

namespace AxeGameCollection.GameObjects.TankBattle;

public sealed class Wind : Sprite
{
    private Vector2 up = new Vector2(0, -1);
    private static Vector2 direction;
    private float radius = 40;
    private float topRadius = 15;
    private Line windDirectionLine; 
    private Line windTop1Line;
    private Line windTop2Line;
    private float angle;

    public static Vector2 Direction => direction * 20;
    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)(position.Y - Height / 2f), (int)Width, (int)Height);

    public override Vector2 Position
    {
        get => position;
        set { position = value; SetDirection(angle); }
    }
    public Wind(Vector2 position, float angle, float scale) : base("graphic/tankBattle/wind", position, 0f, scale, Color.Black, (int)Layer.Hud, CollisionType.None)
    {
        SetDirection(angle); 
    }

    public void SetDirection(float angle)
    {
        this.angle = angle;
        direction = Vector2.Transform(up, Matrix.CreateRotationZ(MathHelper.ToRadians(angle)));
        var velocity = direction * radius;
        var end = Center + velocity;
        var velocity2 = Vector2.Transform(up, Matrix.CreateRotationZ(MathHelper.ToRadians(angle + 150))) * topRadius; 
        var velocity3 = Vector2.Transform(up, Matrix.CreateRotationZ(MathHelper.ToRadians(angle - 150))) * topRadius;
        windDirectionLine = new Line(Center, Center+velocity);
        windTop1Line = new Line(end, end+velocity2);
        windTop2Line = new Line(end, end+velocity3);
    }

    public override Sprite Update(GameTime gameTime)
	{
		return null;
	}

    public override void Draw()
    {
        base.Draw();
        windDirectionLine.Draw(Color.Red, 1);
        windTop1Line.Draw(Color.Red, 1);
        windTop2Line.Draw(Color.Red, 1);
    }
}
