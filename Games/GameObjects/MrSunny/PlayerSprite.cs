using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Framework;

namespace AxeGameCollection.GameObjects.MrSunny;

public sealed class PlayerSprite : SpriteMultipleAnimated
{
    private int alpha;
    private bool alphaGrowing;
    private const int boundingXOffset = 25;
    private const int boundingTopOffset = 25;
    private const int boundingBottomOffset = 11;
    private const int boundingHeightOffset = boundingBottomOffset + boundingTopOffset;
    private static bool energyLost;

    public bool EnergyLost
    {
        get { return energyLost; }
        set
        {
            energyLost = value;
        }
    }

    public Vector2 CenterBEllipse
    {
        get { return new Vector2(BoundingBox.Center.X, BoundingBox.Center.Y); }
        set
        {
            Vector2 dist = Center - CenterBEllipse;
            Vector2 newPos = value;
            newPos += dist;
            newPos.X -= Width / 2;
            newPos.Y -= Height / 2;
            position = newPos;
        }
    }

    public Ellipse BoundingEllipse
    {
        get { return new Ellipse(CenterBEllipse, BoundingBox.Width, BoundingBox.Height); }
    }

    public override Rectangle BoundingBox => new((int)Position.X + boundingXOffset, (int)Position.Y + boundingTopOffset, spriteWidth - boundingXOffset * 2, spriteHeight - boundingHeightOffset);

    public PlayerSprite() : base("graphic/mrSunny/sunny", Vector2.Zero, 162, 9, 0f, 1f, 15)
    {
        collisionType = CollisionType.BoundingBox;
        Position = new Vector2(Manager.DesignWidth / 2 - spriteWidth / 2, Position.Y);
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
            Manager.SpriteBatch.DrawString(Manager.Fonts.Get("Debug"), Position.ToString(), new Vector2(Position.X, Position.Y + spriteHeight + 10), Color.Black);
    }

    public override Sprite Update(GameTime gameTime)
    {
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

    public override RotatedRectangle GetBoundingBox(Vector2 pos)
    {
        return new RotatedRectangle(new RectangleF(pos.X + boundingXOffset, pos.Y + boundingTopOffset, spriteWidth - boundingXOffset * 2, spriteHeight - boundingHeightOffset),rotation);
    }

    public override Sprite ReactToCollision(Sprite s)
    {
        return Player.ReactToCollision(s);
    }

    public Ellipse GetScaledBEllipse(Vector2 worldScale)
    {
        return new Ellipse(new Vector2(CenterBEllipse.X*worldScale.X,CenterBEllipse.Y*worldScale.Y), BoundingBox.Width * worldScale.X, BoundingBox.Height * worldScale.Y);
    }
}
