using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Framework;
using System.Collections.Generic;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;
public class WaterTrack : Sprite
{
    public Vector2 EmitterLocation;
    private readonly List<Particle> particles;

    public WaterTrack(Vector2 position) : base()
    {
        texture = Manager.Content.Load<Texture2D>("graphic/mrSunny/shot");
        EmitterLocation = position;
        particles = new List<Particle>();
    }

    public override Sprite Update(GameTime time)
    {
        var total = 5;
        for (var i = 0; i < total; i++)
        {
            particles.Add(GenerateNewParticle());
        }

        return base.Update(time);
    }

    private Particle GenerateNewParticle()
    {
        var position = EmitterLocation;
        var velocity = new Vector2(Rand.Float(0f,1f), Rand.Float(0f,1f));
        var rotation = 0f;
        var rotationSpeed = 0f;
        var color = new Color(0, 0, Rand.Float(0,255));
        var scale = Rand.Float(0.01f,0.05f);
        var shrinkFactor = Rand.Float(0.001f, 0.003f); 
        var ttl = Rand.Int(20,100);
        var mass = scale;
        return new Particle(texture, position, velocity, rotation, rotationSpeed, color, (int)Layer.Water, scale, ttl, Physics.Gravity * mass, shrinkFactor);
    }
}
