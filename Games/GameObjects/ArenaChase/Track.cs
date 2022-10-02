using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AxeGameCollection.GameObjects.ArenaChase;

public class Track : Particle
{
    private static readonly Texture2D textureStatic;

    static Track()
    {
        textureStatic = Manager.Content.Load<Texture2D>("graphic/arenaChase/track");
    }

    public Track(Vector2 position, float rotation, float scale) : base(textureStatic, position, Vector2.Zero, rotation, 0, Color.White, (int)Layer.Cannon-1, scale, 10, 0, 0, false)
		{
    }

    public override Sprite Update(GameTime time)
    {
        var alpha = color.A - (float)time.ElapsedGameTime.TotalSeconds * 10f;
        if (alpha <= 0)
            return this;
        color.A = (byte)alpha;
        return base.Update(time);
    }
}
