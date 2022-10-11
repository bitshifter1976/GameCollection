using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public class ExplosionParticles
{
    private static readonly Texture2D texture;

    static ExplosionParticles()
    {
        texture = Manager.Content.Load<Texture2D>("graphic/common/dotBig");
    }

    public static List<Sprite> Create(int count, Color color, Vector2 position)
    {
        List<Sprite> list = new();
        for (var i = 0; i < count; i++)
        {
            var velocity = new Vector2(Rand.Float(-0.1f, 0.1f), Rand.Float(-0.1f, 0.1f));
            var scale = Rand.Float(0.2f, 0.4f);
            var ttl = Rand.Float(3.5f, 5.5f);
            list.Add(new Particle(texture, position, velocity, 0, 0, color, (int)Layer.Explosion, scale, ttl, 0, 0, false, true));
        }
        return list;
    }
}
