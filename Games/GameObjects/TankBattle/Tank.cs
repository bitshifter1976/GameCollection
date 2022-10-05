using System;
using System.Threading.Tasks;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AxeGameCollection.GameObjects.TankBattle;

public class Tank : Sprite
{
    private Texture2D textureWeapon;
    private Vector2 positionWeapon;
    private Vector2 originWeapon;
    private float rotationWeapon;
    private float angleWeapon;
    private const float rotationDelta = 0.01f;
    private readonly float rotationDeltaDegrees = MathHelper.ToDegrees(rotationDelta);
    private const float shotOffsetDegrees = 90;
    private float shotPower;
    private float aiAngle;
    private float aiPower;
    private float aiDeltaPower;
    private bool? aiShouldShootWider = null;
    private bool aiPowerCalculating;
    private bool aiPowerCalculated;
    private bool aiCalculationFinished;
    private float aiWaitTime;
    private float aiWaitTimeout;
    private LifeBar lifeBar;
    private PowerBar powerBar;
    private float powerPercentageFactor;
    private bool tankExplosion;
    private int level;
    private bool isActive;

    public bool IsAi { get; }
    public Shot LastAiShotCollision { get; set; }
    public bool AiShouldShoot { get; private set; }
    public bool IsShooting { get; set; }
    public Tank OtherTank { get; set; }
    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;
            lifeBar.SetActive(value);
            powerBar.SetActive(value);
        }
    }


    public Tank(bool ai, bool flip, float scale, bool isActive, int level) : base("graphic/tankBattle/tank", Vector2.Zero, 0, scale, (int)Layer.Cannon, CollisionType.BoundingBoxRotated)
    {
        this.IsAi = ai;
        this.flip = flip;
        this.level = level;
        this.isActive = isActive;
        origin = Vector2.Zero;
        Load();
    }

    private void Load()
    {
        textureWeapon = Manager.Content.Load<Texture2D>("graphic/tankBattle/weapon");
        tankExplosion = false;

        rotationWeapon = 0;
        if (flip)
            angleWeapon = 360 - shotOffsetDegrees;
        else
            angleWeapon = shotOffsetDegrees;
        
        shotPower = 10;
        IsShooting = false;
        
        aiAngle = 325;
        aiPower = 10f;
        aiDeltaPower = 2;
        aiPowerCalculated = false;
        aiCalculationFinished = false;
        aiWaitTime = 0;
        aiWaitTimeout = 4f;
        LastAiShotCollision = null;

        const int barScale = 2;
        const int offset = 10 * barScale;
        Energy = 100;
        lifeBar = new LifeBar(barScale) { Percentage = LifeBar.MaxValue };
        lifeBar.Position = new Vector2(flip ? Manager.DesignWidth - lifeBar.Width - offset : offset, offset);

        powerBar = new PowerBar(barScale);
        powerPercentageFactor = 4f;
        powerBar.Position = new Vector2(flip ? Manager.DesignWidth - powerBar.Width - offset : offset, powerBar.Height + 5);

        SpriteManager.Add(lifeBar);
        SpriteManager.Add(powerBar);
    }

    public override Sprite Update(GameTime gameTime)
    {
        if (Energy <= 0)
        {
            if (!tankExplosion)
            {
                SpriteManager.CreateExplosion(Center, 4f);
                lifeBar.Percentage = 0;
                tankExplosion = true;
            }
        }
        else
        {
            lifeBar.Percentage = Energy;
            powerBar.Percentage = (IsAi ? aiPower : shotPower) * powerPercentageFactor;

            if (IsActive && IsAi && !aiCalculationFinished)
                DoAiCalculations(gameTime);
        }
        return tankExplosion ? this : null;
    }

    private void DoAiCalculations(GameTime gameTime)
    {
        if (Rand.Bool(1, 300))
        {
            aiAngle += Rand.Bool(1, 2) ? -Rand.Int(3, 7) : Rand.Int(3, 7);
            aiAngle = Math.Clamp(aiAngle, 280, 340);
        }

        if (angleWeapon < aiAngle)
            RotateWeapon(true);
        if (angleWeapon > aiAngle)
            RotateWeapon(false);

        if (!aiPowerCalculating)
        {
            aiPowerCalculating = true;
            CalculateAiPower(gameTime);
        }
        else if (aiPowerCalculated)
        {
            aiWaitTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (aiWaitTime > aiWaitTimeout)
            {
                aiWaitTime = 0;
                aiPowerCalculating = false;
                aiPowerCalculated = false;
                aiCalculationFinished = true;
                AiShouldShoot = true;
            }
        }
    }

    private void CalculateAiPower(GameTime gameTime)
    {
        Task.Factory.StartNew(() =>
        {
            var minShotDistance = 100f / level;
            minShotDistance = Math.Clamp(minShotDistance, 10, 100);
            var up = new Vector2(0, -1);
            var rotMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(aiAngle));
            float distance;
            aiPower = Rand.Float(8f, 15f);
            aiDeltaPower = 2;
            aiShouldShootWider = null;
            var iteration = 0;
            //Log.Out(LogLevel.Dbg1, $"-----------------------------------------------------");
            do
            {
                iteration++;
                // simulate shot
                var shotVelocity = Vector2.Transform(up, rotMatrix);
                var shotPos = positionWeapon + shotVelocity * textureWeapon.Width;
                var gravity = new Gravity(GravityType.UpDown, shotVelocity * aiPower, 0.1f);
                while (shotPos.Y < OtherTank.Center.Y || shotVelocity.Y <= 0)
                {
                    shotVelocity = gravity.Update(gameTime);
                    shotPos += shotVelocity + Wind.Direction * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                // now look if it is in range of enemy tank; if not calculate new shot power
                var distanceVect = shotPos - OtherTank.Center;
                distance = distanceVect.Length();
                //Log.Out(LogLevel.Dbg1, $"iteration ({iteration})");
                //Log.Out(LogLevel.Dbg1, $"shot pos ({shotPos})"); 
                //Log.Out(LogLevel.Dbg1, $"tank pos ({OtherTank.Center})");
                //Log.Out(LogLevel.Dbg1, $"distanceVect ({distanceVect})");
                //Log.Out(LogLevel.Dbg1, $"distance ({distance})");
                //Log.Out(LogLevel.Dbg1, $"power ({aiPower})");
                if (distance > minShotDistance)
                {
                    var shouldShootWider = distanceVect.X > 0f;
                    if (aiShouldShootWider.HasValue && aiShouldShootWider.Value != shouldShootWider)
                        aiDeltaPower *= 0.8f;
                    //Log.Out(LogLevel.Dbg1, $"aiDeltaPower ({aiDeltaPower})");
                    //Log.Out(LogLevel.Dbg1, $"shouldShootWider ({shouldShootWider})");
                    aiShouldShootWider = shouldShootWider;
                    aiPower += shouldShootWider ? aiDeltaPower : -aiDeltaPower;
                }
            } 
            while (distance > minShotDistance && iteration < 50);

            aiPowerCalculated = true;
        });
    }

    public override void Draw()
    {
        // if tank is exploding
        if (Energy <= 0) 
            return;
        
        // draw tank
        Manager.SpriteBatch.Draw(
            texture, 
            new Vector2((int)position.X, (int)position.Y), 
            null, 
            Color.White, 
            0,
            Vector2.Zero, 
            scale,
            flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 
            0);
        // draw weapon
        positionWeapon = new Vector2(flip ? position.X + 40 * scale : position.X + Width - 40 * scale, (int)position.Y + 10);
        originWeapon = new Vector2(flip ? textureWeapon.Width : 0, textureWeapon.Height / 2);
        Manager.SpriteBatch.Draw(
            textureWeapon, 
            positionWeapon, 
            null,
            Color.White,
            rotationWeapon,
            originWeapon,
            scale,
            flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            0);
    }

    public void WeaponUp()
    {
        RotateWeapon(true);
    }

    public void WeaponDown()
    {
        RotateWeapon(false);
    }

    private void RotateWeapon(bool up)
    {
        var sign = flip ? -1 : 1;
        rotationWeapon += up ? -rotationDelta * sign : rotationDelta * sign;
        angleWeapon += up ? -rotationDeltaDegrees * sign : rotationDeltaDegrees * sign;
    }

    public void SetWeaponPower(bool increase)
    {
        shotPower += increase ? 0.1f : -0.1f;
        shotPower = MathHelper.Clamp(shotPower, 0, 100 / powerPercentageFactor);
    }

    public void Shoot()
    {
        Log.Out(LogLevel.Dbg1, $"shooting");
        IsShooting = true;
        aiCalculationFinished = false;
        AiShouldShoot = false;
        var up = new Vector2(0, -1);
        var rotMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(angleWeapon));
        var shotVelocity = Vector2.Transform(up, rotMatrix);
        var shotPos = positionWeapon + shotVelocity * textureWeapon.Width;
        SpriteManager.Add(new Shot(this, shotPos, shotVelocity * (IsAi ? aiPower : shotPower)));
    }

    public void Fall(GameTime gameTime)
    {
        var deltaY = 200 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        position = new Vector2(position.X, position.Y + deltaY);
    }

    public bool Collide(Circle circle)
    {
        if (circle == null) return false;
        return circle.IntersectSegment(new Vector2(BoundingBox.Left, BoundingBox.Top), new Vector2(BoundingBox.Right, BoundingBox.Top), out _, out _) > 0 ||
               circle.IntersectSegment(new Vector2(BoundingBox.Left, BoundingBox.Top), new Vector2(BoundingBox.Left, BoundingBox.Bottom), out _, out _) > 0 ||
               circle.IntersectSegment(new Vector2(BoundingBox.Left, BoundingBox.Bottom), new Vector2(BoundingBox.Right, BoundingBox.Bottom), out _, out _) > 0 ||
               circle.IntersectSegment(new Vector2(BoundingBox.Right, BoundingBox.Top), new Vector2(BoundingBox.Right, BoundingBox.Bottom), out _, out _) > 0;
    }

    public override Sprite ReactToCollision(Sprite sprite)
    {
        if (sprite is Shot shot)
        {
            if (Collide(shot.Circle))
                Energy -= shot.Damage;
            else
                Energy -= shot.DamageIndirect;
        }
        return null;
    }
}