using Framework;
using Microsoft.Xna.Framework;
using System.Linq;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public sealed class Mine : Sprite
{
    public override Rectangle BoundingBox => new((int)(position.X - Width / 2f), (int)(position.Y - Height / 2f), (int)Width, (int)Height);

    public static Sprite Create(int probabilityToCreateNewOne)
    {
        Mine obj = null;
        if (probabilityToCreateNewOne > 0 && Rand.Bool(1, probabilityToCreateNewOne))
            obj = new Mine();
        return obj;
    }

    public Mine() : base("graphic/submarineWars/watermine", Vector2.Zero, 0, 1, (int)Layer.Shot, CollisionType.BoundingBoxRotated)
    {
        scrolling = true;
        scale = Rand.Float(0.1f, 0.2f);
        color = Color.White;
        var leftSide = Rand.Bool(1, 2);
        var posX = leftSide ? -Width - 1 : Manager.DesignWidth + 1;
        var posY = Rand.Float(Water.TopPixel.Max(p => p.Value) + Height * 2, Manager.DesignHeight - Water.GroundYOffset - Height * 2);
        position = new Vector2(posX, posY);
        damage = 55;
    }

    public override Sprite Update(GameTime gameTime)
    {
        rotation += Rand.Float(-0.01f, 0.01f);
        rotation = MathHelper.Clamp(rotation, -0.7f, 0.7f);
        position.X += Rand.Float(-50, 50) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        position.Y += Rand.Float(-50, 50) * (float)gameTime.ElapsedGameTime.TotalSeconds;

        return base.Update(gameTime);
    }
}
