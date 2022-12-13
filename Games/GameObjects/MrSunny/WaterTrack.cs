using Microsoft.Xna.Framework;
using Framework;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny;
public class WaterTrack : Sprite
{
    public WaterTrack(Vector2 position) : base("graphic/mrSunny/shot", position, 0, 1, (int)Layer.Water, CollisionType.None)
    {
    }

    public override Sprite Update(GameTime time)
    {
        var total = 5;
        for (var i = 0; i < total; i++)
        {
            SpriteManager.Add(GenerateNewParticle());
        }

        return base.Update(time);
    }

    public override void Draw()
    {
    }

    private Particle GenerateNewParticle()
    {
        var velocity = new Vector2(Rand.Float(0f,1f), Rand.Float(0f,1f));
        var rotation = 0f;
        var rotationSpeed = 0f;
        var color = new Color(Color.Aquamarine.R, Color.Aquamarine.G, Color.Aquamarine.B, (byte)Rand.Int(100,200));
        var scale = Rand.Float(0.01f,0.05f);
        var shrinkFactor = 0; 
        var ttl = Rand.Int(100,200);
        var mass = scale;
        return new Particle(texture, position, velocity, rotation, rotationSpeed, color, (int)Layer.Water, scale, ttl, Physics.Gravity * mass, shrinkFactor);
    }
}
