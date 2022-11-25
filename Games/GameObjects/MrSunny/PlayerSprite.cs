using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;

public sealed class PlayerSprite : SpriteMultipleAnimated
{
    private int alpha;
    private bool alphaGrowing;
    private const int boundingXOffset = 25;
    private const int boundingTopOffset = 25;
    private const int boundingBottomOffset = 11;
    private const int boundingHeightOffset = boundingTopOffset + boundingBottomOffset;
    private bool energyLost;
    private float energyLostTime;
    private readonly float energyLostTimeout = 5;
    public float Gravity;

    public bool EnergyLost
    {
        get { return energyLost; }
        set
        {
            energyLost = value;
        }
    }

    public override Rectangle BoundingBox => new((int)(position.X + boundingXOffset * scale), (int)(position.Y + boundingTopOffset * scale), (int)(Width - boundingXOffset * scale * 2), (int)(Height - boundingHeightOffset * scale));

    public int SpriteWidth => spriteWidth;

    public float VelocityX
    {
        get => velocity.X;
        set => velocity = new(value, velocity.Y);
    }

    public float VelocityY
    {
        get => velocity.Y;
        set => velocity = new(velocity.X, value);
    }

    public float PositionX
    {
        get => position.X;
        set => position = new(value, position.Y);
    }

    public float PositionY
    {
        get => position.Y;
        set => position = new(position.X, value);
    }
    public override Vector2 Center
    {
        get => BoundingBoxRotated.Center;
        set => throw new NotImplementedException();
    }

    public PlayerSprite(float scale) : base("graphic/mrSunny/sunny", Vector2.Zero, 162, 9, 0f, scale, (int)Layer.Player, CollisionType.BoundingBox)
    {
        collisionType = CollisionType.BoundingBox;
    }

    public override void Draw()
    {
        if (!energyLost)
        {
            Manager.SpriteBatch.Draw(texture, Position,
                animations[Animation].Rectangles[frameIndex],
                animations[Animation].Color,
                animations[Animation].Rotation,
                animations[Animation].Origin,
                animations[Animation].Scale,
                animations[Animation].SpriteEffect,
                0f);
            alpha = 255;
            alphaGrowing = false;
        }
        else
        {
            Manager.SpriteBatch.Draw(texture, Position,
                animations[Animation].Rectangles[frameIndex],
                new Color(255,100,100, alpha),
                animations[Animation].Rotation,
                animations[Animation].Origin,
                animations[Animation].Scale,
                animations[Animation].SpriteEffect,
                0f);
            if (alphaGrowing)
                alpha += 20;
            else
                alpha -= 20;
            if (alpha < 0)
            {
                alpha = 255;
                alphaGrowing = false;
            }
            else if (alpha > 255)
            {
                alpha = 0;
                alphaGrowing = true;
            }
        }

        //Weapon.Draw();

        if (Manager.Debug)
        {
            Debug.Rects.Add(new RectDebug(BoundingBoxRotated.ToLines(), "BoundingBox", Color.Red));
            Debug.Points.Add(new PointDebug(new Dot(Center), "Center", Color.Gold));
            Debug.Points.Add(new PointDebug(new Dot(position), "Position", Color.Green));
            Manager.SpriteBatch.DrawString(Manager.Fonts.Get("Debug"), $"{position}({this.GetType().Name})", new Vector2(position.X, position.Y + Height + 2), Color.Black);
        }
    }

    public override Sprite Update(GameTime gameTime)
    {
        if (energyLost)
        {
            energyLostTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (energyLostTime >= energyLostTimeout)
            {
                energyLost = false;
                energyLostTime = 0;
            }
        }
        // next frame of animation
        base.Update(gameTime);
        //Weapon.Update(gameTime);
        return null;
    }

    public new void Flip(string animation, bool flip)
    {
        animations[animation].SpriteEffect = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
    }

    public void Shoot(bool ducking, int power, Direction direction)
    {
        //Weapon.Shoot(Center, power, ducking, direction);
    }

    public Ellipse GetScaledBEllipse(Vector2 worldScale)
    {
        return new Ellipse(new Vector2(BoundingBox.Center.X * worldScale.X, BoundingBox.Center.Y * worldScale.Y), BoundingBox.Width * worldScale.X, BoundingBox.Height * worldScale.Y);
    }

    public override Sprite ReactToCollision(Sprite sprite)
    {
        return Player.ReactToCollision(sprite);
    }
}
