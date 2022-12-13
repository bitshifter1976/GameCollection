using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Framework;
using System.Collections.Generic;
using static AxeGameCollection.Screens.ScreenGameMrSunny;

namespace AxeGameCollection.GameObjects.MrSunny
{
    public class WaterSplash : Sprite
    {
        private readonly List<Sprite> particles;

        public WaterSplash(Vector2 position) : base("graphic/mrSunny/shot", position, 0, 1, (int)Layer.Water, CollisionType.None)
        {
            particles = new List<Sprite>();
            var count = Rand.Int(100,300);
            for (var i=0; i<count; ++i)
                GenerateNewParticle(position);
        }

        public override Sprite Update(GameTime gameTime)
        {
            var toRemove = new List<Sprite>();
            particles.ForEach(s => toRemove.Add(s.Update(gameTime)));
            toRemove.ForEach(s => particles.Remove(s));
            return (particles.Count == 0) ? this : null;
        }

        private void GenerateNewParticle(Vector2 position)
        {
            var velocity = new Vector2(Rand.Float(-0.4f, 0.4f), Rand.Float(-0.2f, -1f));
            var speed = Rand.Float(3f, 7f);
            velocity *= speed;
            var rotation = 0f;
            var rotationSpeed = 0f;
            var color = new Color(Color.Aquamarine.R, Color.Aquamarine.G, Color.Aquamarine.B, (byte)Rand.Int(100, 200));
            var scale = Rand.Float(0.01f, 0.2f);
            var ttl = Rand.Int(20, 100);
            var mass = scale;
            var shrinkFactor = Rand.Float(0.001f, 0.003f);
            particles.Add(new Particle(texture, position, velocity, rotation, rotationSpeed, color, (int)Layer.Water, scale, ttl, Physics.Gravity*mass, shrinkFactor));
        }

        public override void Draw()
        {
            particles.ForEach(s => s.Draw());
        }
    }
}
