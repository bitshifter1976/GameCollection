using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AxeGameCollection.GameObjects.ArenaChase;

public class ExplosionParticles
{
    private static readonly Texture2D texture;

    static ExplosionParticles()
    {
        texture = Manager.Content.Load<Texture2D>("graphic/dotBig");
    }

    public static List<Sprite> Create(int count, Color color, Vector2 position)
    {
        List<Sprite> list = new();
        for (var i = 0; i < count; i++)
        {
            var velocity = new Vector2(Rand.Float(-0.4f, 0.4f), Rand.Float(-0.4f, 0.4f));
            var speed = Rand.Float(2.5f, 7.5f);
            var scale = Rand.Float(0.2f, 0.4f);
            var ttl = Rand.Float(1.5f, 3.5f);
            var factor = Rand.Float(0.25f, 0.75f);
            velocity *= speed;
            color.R = (byte)MathHelper.Lerp(color.R, Color.White.R, factor);
            color.G = (byte)MathHelper.Lerp(color.G, Color.White.G, factor);
            color.B = (byte)MathHelper.Lerp(color.B, Color.White.B, factor);
            list.Add(new Particle(texture, position, velocity, 0, 0, color, (int)Layer.Explosion, scale, ttl, 0, 0));
        }
        return list;
    }
}
