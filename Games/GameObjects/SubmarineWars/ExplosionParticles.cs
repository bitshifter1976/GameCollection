using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public class ExplosionParticles
{
    private static readonly Texture2D texture;

    static ExplosionParticles()
    {
        texture = Manager.Content.Load<Texture2D>("graphic/common/dotBig");
    }

    public static List<Sprite> Create(Vector2 position, float scale)
    {
        var count = Rand.Int((int)(100 * scale), (int)(200 * scale));
        var color = new Color(Color.White.R, Color.White.G, Color.White.B, (byte)200);
        List<Sprite> list = new();
        for (var i = 0; i < count; i++)
        {
            var rotation = MathHelper.ToRadians(Rand.Int(0,359));
            var velocity = Vector2.Transform(new Vector2(0, -1), Matrix.CreateRotationZ(rotation)) * scale * Rand.Float(-0.5f, 0.5f);
            var ttl = Rand.Float(3, 5);
            list.Add(new Particle(texture, position, velocity, 0, 0, color, (int)Layer.Explosion, 0.25f, ttl, 0, 0, false, true));
        }
        return list;
    }
}
