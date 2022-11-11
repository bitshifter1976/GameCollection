using System;
using System.Linq;
using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public sealed class TankShip : Sprite
{
    private readonly int sign;
    private Texture2D texture2;

    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)position.Y, (int)Width, (int)(Height / 2f));

    public RectangleF BoundingBoxForMissiles => new RectangleF(position.X - 5, position.Y + Height / 2f, 10, 100);

    public static TankShip Create(int probabilityToCreateNewOne)
    {
        TankShip obj = null;
        if (probabilityToCreateNewOne > 0 && Rand.Bool(1, probabilityToCreateNewOne))
            obj = new TankShip();
        return obj;
    }

    public TankShip() : base("graphic/submarineWars/ship2", Vector2.Zero, 0, 1, (int)Layer.Submarine-1, CollisionType.BoundingBox)
    {
        texture2 = new Texture2D(Manager.Graphics.GraphicsDevice, 1, 1);
        texture2.SetData(new Color[1] { Color.White });
        scrolling = true;
        var direction = Rand.Int(0, 1);
        if (direction == 1)
        {
            sign = -1;
            position.X = Manager.DesignWidth-1;
            flip = true;
        }
        else
        {
    	    sign = 1;
            position.X = -Width+1;
            flip = false;
        }
        position.Y = Water.TopPixel.Max(p => p.Value) - Height / 3f;
		speed = Rand.Float(40,80);
        color = Color.White;
    }

    public override Sprite Update(GameTime gameTime)
    {
        if (Rand.Bool(1, 50))
        {
            rotation += Rand.Float(-0.01f, 0.01f);
            rotation = MathHelper.Clamp(rotation, -0.05f, 0.05f);
        }
        position.X += speed * sign * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (Water.TopPixel.ContainsKey((int)position.X))
            position.Y = Water.TopPixel[(int)position.X] - Height / 3f;
        if (position.X < 0)
            return position.X < -Manager.DesignWidth * 2 ? this : null;
        if (position.X > Manager.DesignWidth)
            return position.X > Manager.DesignWidth * 2 ? this : null;
        return null;
    }

    public override void Draw()
    {
        Manager.SpriteBatch.Draw(texture, position, null, color, rotation, origin, scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        // background
        Manager.SpriteBatch.Draw(
            texture2,
            BoundingBoxForMissiles.ToRectangle(),
            null,
            new Color(133,153,138),
            0,
            Vector2.Zero,
            SpriteEffects.None,
            0);

        if (Manager.Debug)
        {
            Debug.Rects.Add(new RectDebug(BoundingBoxRotated.ToLines(), "BoundingBox", Color.Red));
            Debug.Points.Add(new PointDebug(new Dot(Center), "Center", Color.Gold));
            Debug.Points.Add(new PointDebug(new Dot(position), "Position", Color.Green));
            Manager.SpriteBatch.DrawString(Manager.Fonts.Get("Debug"), $"{position}({this.GetType().Name})", new Vector2(position.X, position.Y + Height + 2), Color.Black);
        }
    }
}
