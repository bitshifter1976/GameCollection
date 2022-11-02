using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AxeGameCollection.GameObjects.MrSunny;

public static class Player
{
    private static PlayerSprite dude;
    private static int energy;
    private static int water;
    private static int shotPower;
    private static int shotPowerBonus;
    private static int shotConsumption;
    private static float walkStartSpeed;
    private static float walkSpeed;
    private static float walkSpeedMax;
    private static float walkSpeedAcceleration;
    private static float runSpeed;
    private static float runSpeedMax;
    private static float runSpeedAcceleration;
    private static float unduckTime;
    private static float unduckElapsedTime;
    private static bool directionRight;
    private static int maxEnergy;
    private static float shotStartTime;
    private static float shotStartTimeElapsed;
    private static bool timeToShoot;
    //private static Direction shotDirection;
    private static Vector2 worldScale;
    private static int jumpAccelleration;
    private static float gravity;
    private static float startFallVelocity;
    private static bool forceStandAfterCollision;
    private static Direction shotDirection;
    private static int speed;

    public static PlayerSprite Sprite
    {
        get { return dude; }
    }

    public static int Energy
    {
        get { return energy; }
        set
        {
            if (value < energy)
            {
                Manager.Sound.PlayEffect("autsch");
                EnergyChanged = true;
                EnergyLost = true;
                dude.EnergyLost = true;
            }
            else if (value > energy)
            {
                EnergyChanged = true;
            }
            energy = value;
            energy = (int)MathHelper.Clamp(energy, 0, maxEnergy);
        }
    }

    public static string Animation
    {
        get { return dude.Animation; }
        set { dude.Animation = value; }
    }

    public static int Water
    {
        get { return water; }
        set
        {
            if (value < water)
            {
                WaterChanged = true;
            }
            else if (value > water)
            {
                WaterChanged = true;
                shotPower += shotPowerBonus;
            }
            water = value;
            water = (int)MathHelper.Clamp(water, 0, maxEnergy);
            //Weapon.ChangeCount(WeaponType.WaterBall, water / shotConsumption);
        }
    }

    public static bool Dead
    {
        get
        {
            if (Energy <= 0)
                return true;
            if (!dude.BoundingBox.Intersects(new Rectangle(0, 0, Manager.DesignWidth, Manager.DesignHeight)))
                return true;
            return false;
        }
    }

    public static float Mass
    {
        get { return dude.Mass; }
    }

    public static Vector2 WorldScale
    {
        get { return worldScale; }
    }

    public static Vector2 Gravity
    {
        get { return new Vector2(0, gravity); }
    }

    public static float VelocityX
    {
        get { return dude.Velocity.X; }
        set { dude.Velocity = new Vector2(value, dude.Velocity.Y); }
    }

    public static float VelocityY
    {
        get { return dude.Velocity.Y; }
        set { dude.Velocity = new Vector2(dude.Velocity.X, value); }
    }

    public static Vector2 Position
    {
        get { return dude.Position; }
        set { dude.Position = value; }
    }

    public static float PositionX
    {
        get { return dude.Position.X; }
        set { dude.Position = new Vector2(value, dude.Position.Y); }
    }

    public static float PositionY
    {
        get { return dude.Position.Y; }
        set { dude.Position = new Vector2(dude.Position.X, value); }
    }


    public static bool EnergyChanged { get; set; }

    public static bool EnergyLost { get; set; }

    public static bool WaterChanged { get; set; }

    public static int Speed => speed;

    public static int SpriteWidth => dude.SpriteWidth;

    public static Sprite Create()
    {
        dude = new PlayerSprite();
        var ani = new Animation {Fps = 20};
        dude.AddAnimation("Stand", 1, 18, ani.Copy());
        dude.AddAnimation("WalkRight", 2, 12, ani.Copy());
        ani.SpriteEffect = SpriteEffects.FlipHorizontally;
        dude.AddAnimation("WalkLeft", 2, 12, ani.Copy());
        ani.SpriteEffect = SpriteEffects.None;
        ani.Fps = 40;
        dude.AddAnimation("RunRight", 2, 12, ani.Copy());
        ani.SpriteEffect = SpriteEffects.FlipHorizontally;
        dude.AddAnimation("RunLeft", 2, 12, ani.Copy());
        ani.SpriteEffect = SpriteEffects.None;
        ani.Fps = 20;
        ani.IsLooping = false;
        dude.AddAnimation("Jump", 5, 4, ani.Copy());
        dude.AddAnimation("Land", 6, 6, ani.Copy());
        dude.AddAnimation("Duck", 3, 4, ani.Copy());
        dude.AddAnimation("UnDuck", 4, 4, ani.Copy());
        dude.AddAnimation("Shoot", 7, 10, ani.Copy());
        dude.AddAnimation("ShootDuck", 8, 10, ani.Copy());
        dude.AddAnimation("Fall", 9, 12, ani.Copy());
        Animation = "Fall";
        directionRight = true;
        Init();
        Manager.Sound.LoadEffect("jump");
        Manager.Sound.LoadEffect("shot");
        Manager.Sound.LoadEffect("explosion");
        Manager.Sound.LoadEffect("collect");
        Manager.Sound.LoadEffect("autsch");
        Manager.Sound.LoadEffect("fireDelete");
        return dude;
    }

    private static void Init()
    {
        maxEnergy = 190;
        energy = maxEnergy;
        EnergyChanged = false; 
        shotPower = 15;
        shotConsumption = 38;
        shotPowerBonus = 3;
        Water = maxEnergy;
        WaterChanged = false;
        walkSpeedMax = 2f;
        walkStartSpeed = 0.5f;
        walkSpeed = walkStartSpeed;
        walkSpeedAcceleration = 0.03f;
        runSpeed = walkSpeedMax;
        runSpeedMax = walkSpeedMax * 1.5f;
        runSpeedAcceleration = 0.03f;
        shotStartTime = 0.2f;
        shotStartTimeElapsed = 0f;
        timeToShoot = false;
        unduckTime = 0.5f;
        unduckElapsedTime = 0f;
        jumpAccelleration = 7;
        startFallVelocity = 2f;
        dude.Mass = 12f;
        dude.Friction = 0.8f;
        worldScale = new Vector2((float)dude.BoundingBox.Height / dude.BoundingBox.Width, 1);
        gravity = Physics.Gravity * Mass;
        forceStandAfterCollision = false;
        //Weapon.Select(WeaponType.WaterBall);
    }

    public static void HandleInput()
    {
        if (Animation != "UnDuck" && Animation != "Shoot" && Animation != "ShootDuck")
        {
            if (Animation != "Jump" && Animation != "Fall")
            {
                if (Action.Jump)
                    Jump();
                else if (Action.Duck)
                    Duck();
                else if (Action.UnDuck)
                    UnDuck();
                else if (Action.WalkLeft)
                {
                    if (Action.RunLeft)
                        RunLeft();
                    else
                        WalkLeft();
                }
                else if (Action.WalkRight)
                {
                    if (Action.RunRight)
                        RunRight();
                    else
                        WalkRight();
                }
                else
                    Stand();
            }
            else
                SteerFall();
        }
        if (Action.Shoot)
        {
            if (Action.ShootUp)
                shotDirection = Direction.Up;
            else if (directionRight && Animation != "Duck")
                shotDirection = Direction.UpRight;
            else if (!directionRight && Animation != "Duck")
                shotDirection = Direction.UpLeft;
            else if (directionRight && Animation == "Duck")
                shotDirection = Direction.Right;
            else if (!directionRight && Animation == "Duck")
                shotDirection = Direction.Left;
            Shoot();
        }
        //if (Action.Weapon1)
        //    Weapon.Select(WeaponType.WaterBall);
        //if (Action.Weapon2)
        //    Weapon.Select(WeaponType.RainDance);
    }

    private static void Stand()
    {
        if (Animation != "Stand")
        {
            dude.Flip("Stand", !directionRight);
            Animation = "Stand";
            walkSpeed = walkStartSpeed;
            //VelocityY = 0;
        }
    }

    private static void Jump()
    {
        if (Animation != "Jump")
        {
            VelocityY = -jumpAccelleration;
            dude.Flip("Jump", !directionRight);
            Manager.Sound.PlayEffect("jump");
            Animation = "Jump";
        }
    }

    private static void Shoot()
    {
        timeToShoot = true;
        if (Animation != "Duck")
        {
            Animation = "Shoot";
            dude.Flip("Shoot", !directionRight);
        }
        else
        {
            Animation = "ShootDuck";
            dude.Flip("ShootDuck", !directionRight);
        }
    }

    private static void Duck()
    {
        dude.Flip("Duck", !directionRight);
        Animation = "Duck";
    }

    private static void UnDuck()
    {
        dude.Flip("UnDuck", !directionRight);
        Animation = "UnDuck";
    }

    private static void WalkLeft()
    {
        if (Animation != "WalkLeft")
        {
            directionRight = false;
            Animation = "WalkLeft";
            walkSpeed = walkStartSpeed;
        }
        if (walkSpeed < walkSpeedMax)
            walkSpeed += walkSpeedAcceleration;
        runSpeed = walkSpeed;
        VelocityX = -walkSpeed;
    }

    private static void WalkRight()
    {
        if (Animation != "WalkRight")
        {
            directionRight = true;
            Animation = "WalkRight";
            walkSpeed = walkStartSpeed;
        }
        if (walkSpeed < walkSpeedMax)
            walkSpeed += walkSpeedAcceleration;
        runSpeed = walkSpeed;
        VelocityX = walkSpeed;
    }

    private static void RunLeft()
    {
        if (Animation != "RunLeft")
        {
            directionRight = false;
            Animation = "RunLeft";
            runSpeed = walkSpeedMax;
        }
        if (runSpeed < runSpeedMax)
            runSpeed += runSpeedAcceleration;
        walkSpeed = runSpeed;
        VelocityX = -walkSpeed;
    }

    private static void RunRight()
    {
        if (Animation != "RunRight")
        {
            directionRight = true;
            Animation = "RunRight";
            runSpeed = walkSpeedMax;
        }
        if (runSpeed < runSpeedMax)
            runSpeed += runSpeedAcceleration;
        walkSpeed = runSpeed;
        VelocityX = walkSpeed;
    }

    private static void Fall()
    {
        if (Animation != "Fall")
        {
            Animation = "Fall";
            dude.Flip("Fall", !directionRight);
        }
    }

    private static void SteerFall()
    {
        if (Action.WalkLeft)
        {
            directionRight = false;
            dude.Flip("Fall", true);
            VelocityX -= runSpeedAcceleration;
        }
        else if (Action.WalkRight)
        {
            directionRight = true;
            dude.Flip("Fall", false);
            VelocityX += runSpeedAcceleration;
        }
    }

    public static void Update(GameTime time)
    {
        if (VelocityY > startFallVelocity)
            Fall();

        if (forceStandAfterCollision)
        {
            Stand();
            forceStandAfterCollision = false;
        }

        if (timeToShoot)
        {
            shotStartTimeElapsed += (float)time.ElapsedGameTime.TotalSeconds;
            if (shotStartTimeElapsed > shotStartTime && timeToShoot)
            {
                timeToShoot = false;
                shotStartTimeElapsed = 0;
                Manager.Sound.PlayEffect("shot");
                dude.Shoot((Animation == "ShootDuck"), shotPower, shotDirection);
                shotPower -= shotPowerBonus;
                Water -= shotConsumption;
                if (Animation != "Jump" && Animation != "Fall")
                    Animation = Animation == "Shoot" ? "Stand" : "Duck";
            }
        }
        else if (Animation == "UnDuck")
        {
            unduckElapsedTime += (float)time.ElapsedGameTime.TotalSeconds;
            if (unduckElapsedTime > unduckTime)
            {
                unduckElapsedTime = 0f;
                Animation = "Stand";
            }
        }
    }

    public static void Draw()
    {
        dude.Draw();
    }

    public static Sprite ReactToCollision(Sprite s)
    {
        //Log.Out(LogLevel.DBG1, "ReactToCollision");
        Sprite removeSprite = null;

        if (Animation == "Fall" || Animation == "Jump")
            forceStandAfterCollision = true;

        return removeSprite;
    }
}