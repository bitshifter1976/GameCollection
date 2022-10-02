using Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AxeGameCollection.GameObjects.TankBattle;

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
            var velocity = new Vector2(Rand.Float(-0.4f, 0.4f), Rand.Float(-0.2f, -1f));
            var speed = Rand.Float(2.5f, 7.5f);
            velocity *= speed;
            var scale = Rand.Float(0.1f, 0.5f);
            var mass = scale / 4f;
            var ttl = Rand.Int(50, 200);
            var factor = Rand.Float(0.25f, 0.75f);
            if (color != Color.Black)
            {
                color.R = (byte)MathHelper.Lerp(color.R, Color.Black.R, factor);
                color.G = (byte)MathHelper.Lerp(color.G, Color.Black.G, factor);
                color.B = (byte)MathHelper.Lerp(color.B, Color.Black.B, factor);
            }
            else
            {
                color.R = (byte)MathHelper.Lerp(color.R, Color.White.R, factor);
                color.G = (byte)MathHelper.Lerp(color.G, Color.White.G, factor);
                color.B = (byte)MathHelper.Lerp(color.B, Color.White.B, factor);
            }
            list.Add(new Particle(texture, position, velocity, 0, 0, color, (int)Layer.Explosion-1, scale, ttl, Physics.Gravity * mass, 0, true));
        }
        return list;
    }
}
