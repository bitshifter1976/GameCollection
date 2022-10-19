using Framework;
using Microsoft.Xna.Framework;
using System.Reflection.Metadata;
using static AxeGameCollection.Screens.ScreenGameTankBattle;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public class Bomb : Sprite
{
    private Gravity gravity;
    private int sign;
    private float goalRotation;
    private bool speedLowered;

    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)(position.Y - Height / 2f), (int)Width, (int)Height);

    public Bomb(Vector2 position, Vector2 velocity, bool flip, float rotation) : base("graphic/submarineWars/bomb", position, rotation, 0.25f, (int)Layer.Shot, CollisionType.BoundingBoxRotated)
    {
        scrolling = true;
        gravity = new Gravity(GravityType.UpDown, velocity, 0.1f);
        this.flip = flip;
        damage = 33;
        speed = 100;
        sign = flip ? 1 : -1;
        goalRotation = MathHelper.ToRadians(180*sign);
    }

    public override Sprite Update(GameTime gameTime)
    {
        var x = (int)position.X;
        if (!speedLowered && Water.TopPixel.ContainsKey(x) && position.Y >= Water.TopPixel[x])
        {
            speed /= 2;
            gravity = new Gravity(GravityType.UpDown, velocity, 0.001f);
            speedLowered = true;
        }
        if (flip)
        {
            if (rotation < goalRotation)
            {
                rotation += 0.05f;
            }
        }
        else
        {
            if (rotation > goalRotation)
            {
                rotation -= 0.05f;
            }
        }
        velocity = gravity.Update(gameTime);
        position += velocity * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        return null;
    }
}
