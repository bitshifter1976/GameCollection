using Microsoft.Xna.Framework;
using Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public sealed class Shot : Sprite
{
    private readonly WaterTrack waterTrack;
    private readonly Gravity gravity;

    public Shot(Vector2 position, Vector2 direction, float scale) : base("graphic/mrSunny/shot", position, 0f, scale, (int)Layer.Water, CollisionType.BoundingBox)
    {
        Velocity = direction;
        damage = 20;
        mass = 0.5f;
        waterTrack = new WaterTrack(Position);
        gravity = new Gravity(GravityType.UpDown, Velocity, mass);
        scrolling = true;
    }
    
    public override Sprite Update(GameTime time)
    {
        Velocity = gravity.Update(time);
        Position += Velocity;

        waterTrack.EmitterLocation = Position;
        waterTrack.Update(time);

        if (Position.Y > Manager.DesignHeight)
            return this;
        return null;
    }

    public override void Draw()
    {
        waterTrack.Draw();
        base.Draw();
    }
}
