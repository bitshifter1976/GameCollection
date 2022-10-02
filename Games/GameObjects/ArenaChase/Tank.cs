using System;
using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.ArenaChase;

public sealed class Tank : Sprite
{
    private readonly bool isAi;
    private readonly Tank tankToChase;
    private readonly float shotPower;
    private float trackTime;
    private const float TrackTimeout = 0.05f;

    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)(position.Y - Height / 2f), (int)Width, (int)Height);

    public Tank(Vector2 position, float scale, float rotation, bool ai, Tank tankToChase = null) : base("graphic/arenaChase/tank" + (ai ? "2" : "1"), position, rotation, scale, (int)Layer.Cannon, CollisionType.BoundingBoxRotated)
    {
        isAi = ai;
        this.tankToChase = tankToChase;
        shotPower = 700;
        energy = 100;
        speed = 0;
    }

    public override Sprite Update(GameTime gameTime)
    {
        if (isAi) CalculateAi();
        if (energy > 0)
        {
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            IfOffScreenBounceBack();

            trackTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (trackTime >= TrackTimeout)
            {
                trackTime = 0;
                if (speed > 0)
                    LeaveTrack();
            }
        }
        else
        {
            SpriteManager.CreateExplosion(Center, 2);
            return this;
        }

        return null;
    }


    private void IfOffScreenBounceBack()
    {
        if (position.X - Width/2f < 0 || position.X + Width/2f > Manager.DesignWidth || position.Y - Height/2f < 0 || position.Y + Height / 2f > Manager.DesignHeight)
            BounceBack(4);
    }

    private void CalculateAi()
    {
        if (tankToChase == null) return;

        var distance = new Vector2(tankToChase.Center.X - Center.X, tankToChase.Center.Y - Center.Y);
        var wantedRotation = (float)Math.Atan2(distance.Y, distance.X) + MathHelper.ToRadians(90);
        if (rotation < wantedRotation)
            Rotate(false);
        if (rotation > wantedRotation)
            Rotate(true);

        if (Rand.Bool(1, 10))
            SetSpeed(Rand.Bool(7, 10));

        if (Rand.Bool(1, 200))
            Shoot();
    }

    public void Rotate(bool left)
    {
        rotation += left ? -0.05f : 0.05f;
        CalculateVelocity();
    }

    public void SetSpeed(bool up)
    {
        speed += up ? 5f : -5f;
        speed = MathHelper.Clamp(speed, 0, 300);
        CalculateVelocity();
    }

    private void CalculateVelocity()
    {
        var up = new Vector2(0, -1);
        var rotMatrix = Matrix.CreateRotationZ(rotation);
        velocity = Vector2.Transform(up, rotMatrix) * speed;
    }

    public void Shoot()
    {
        var up = new Vector2(0, -1);
        var rotMatrix = Matrix.CreateRotationZ(rotation);
        var shotVelocity = Vector2.Transform(up, rotMatrix);
        var shot = new Shot(this, Vector2.Zero, shotVelocity * shotPower, 0.5f);
        var shotPos = position + shotVelocity * (Width-30) / 2f;
        shot.Position = shotPos;
        SpriteManager.Add(shot);
    }

    public void PlaceMine()
    {
        var up = new Vector2(0, -1);
        var rotMatrix = Matrix.CreateRotationZ(rotation);
        var mineDirection = -Vector2.Transform(up, rotMatrix);
        var mine = new Mine(Vector2.Zero, 0.5f, rotation);
        var pos = position + mineDirection * (Height/2f + mine.Height/2f + 10);
        mine.Position = pos;
        SpriteManager.Add(mine);
    }

    private void LeaveTrack()
    {
        var up = new Vector2(0, -1);
        var trackDirection = -Vector2.Transform(up, Matrix.CreateRotationZ(rotation));
        var track1 = new Track(Vector2.Zero, rotation, 0.5f);
        var track2 = new Track(Vector2.Zero, rotation, 0.5f);
        var pos = position + trackDirection * Height / 2f;
        track1.Position = pos + Vector2.Transform(up, Matrix.CreateRotationZ(rotation - MathHelper.ToRadians(-90))) * (Width / 2f - track1.Width);
        track2.Position = pos + Vector2.Transform(up, Matrix.CreateRotationZ(rotation - MathHelper.ToRadians(90))) * (Width / 2f);
        SpriteManager.Add(track1);
        SpriteManager.Add(track2);
    }
}
