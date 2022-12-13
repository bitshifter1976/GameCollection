using System;
using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public class EnemyBoss : Sprite
{
    private float gravity;
    private float maxSpeed;
    private float accelleration;

    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)(position.Y - Height / 2f), (int)Width, (int)Height);

    public override RotatedRectangle GetBoundingBox(Vector2 pos)
    {
        return new RotatedRectangle(new RectangleF(pos.X - Width / 2f, pos.Y - Height / 2f, Width, Height), rotation);
    }


    public EnemyBoss(float scale, int level) : base("graphic/mrSunny/enemy", Vector2.Zero, 0, scale, Color.White, (int)Layer.Player, CollisionType.BoundingBoxRotated)
    {
        scrolling = true;
        mass = 10;
        energy = Player.MaxEnergy;
        damage = Player.MaxEnergy / 5;
        speed = 0;
        maxSpeed = 100 * level;
        accelleration = level;
    }

    public override Sprite Update(GameTime gameTime)
    {
        if (energy <= 0)
            return this;

        if (OnScreen)
        {
            var direction = Player.Position.X > position.X ? 1 : -1;
            flip = direction == -1;
            var elapsedSec = (float)gameTime.ElapsedGameTime.TotalSeconds;
            speed += accelleration * elapsedSec;
            speed = Math.Clamp(speed, 0, maxSpeed);
            velocity.X = speed * direction * elapsedSec;
            gravity += Physics.Gravity * mass * elapsedSec;
            velocity.Y += gravity;
            position += velocity;
        }

        return null;
    }

    public override Sprite ReactToCollision(Sprite s)
    {
        Sprite removeSprite = null;

        var collDir = Collision.GetCollisionDirection(BoundingBoxF, s.BoundingBoxF);
        switch (collDir)
        {
            case CollisionDirection.BottomLeft:
            case CollisionDirection.BottomRight:
            case CollisionDirection.Bottom:
                if (s is GroundTile || s is Stone || s is PlayerSprite)
                {
                    position.Y -= velocity.Y;
                    gravity = 0;
                    velocity.Y = 0;
                }
                break;
            case CollisionDirection.Left:
            case CollisionDirection.Right:
            case CollisionDirection.TopLeft:
            case CollisionDirection.TopRight:
                if (s is PlayerSprite || s is Stone)
                {
                    position.X -= velocity.X;
                    speed = 0;
                }
                break;
        }

        return removeSprite;
    }
}
