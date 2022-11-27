using Microsoft.Xna.Framework;
using Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;
using System.Collections.Generic;

namespace AxeGameCollection.GameObjects.MrSunny;

public sealed class Rain : Sprite
{
    private readonly float durationSec;
    private float timeElapsed;
    private readonly List<Particle> particles = new();

    public Rain(int durationSec) : base("graphic/mrSunny/rain", (int)Layer.Water, CollisionType.None)
    {
        this.durationSec = durationSec;
        timeElapsed = 0;
    }

    public override Sprite Update(GameTime time)
    {
        timeElapsed += (float)time.ElapsedGameTime.TotalSeconds;
        if (timeElapsed < durationSec)
        {
            if (timeElapsed < durationSec - 2)
            {
                for (var i = 0; i < Rand.Int(10, 20); ++i)
                    GenerateNewParticle(durationSec * 5);
            }
        }
        return particles.Count == 0 ? this : null;
    }

    private void GenerateNewParticle(float ttl)
    {
        var position = new Vector2(Rand.Int(0,Manager.DesignWidth),0);
        var velocity = new Vector2(Rand.Float(-0.1f, 0.1f), Rand.Float(2f, 4f));
        var color = new Color(0, 0, Rand.Float(0, 255));
        var scale = Rand.Float(0.05f, 0.15f);
        var mass = scale;
        var particle = new Particle(texture, position, velocity, rotation, 0, color, (int)Layer.Water, scale, (int)ttl, Physics.Gravity * mass, 0);
        particles.Add(particle);
        SpriteManager.Add(particle);
    }
}
