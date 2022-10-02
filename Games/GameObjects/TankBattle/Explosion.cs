using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.TankBattle;

public sealed class Explosion : SpriteAnimatedMultiLine
{
	private float existTime = 0;
	private float existTimeout = 2;

	public override int Damage
	{
		get { return 12; }
	}

	public Explosion(Vector2 position, float scale) : base("graphic/common/explosion", position, 25, 5, 25, 0, scale, (int)Layer.Explosion)
	{
		Center = position;
		Manager.Sound.LoadEffect("explosion");
		Manager.Sound.PlayEffect("explosion");
	}

    public override Sprite Update(GameTime gameTime)
    {
		base.Update(gameTime);

		existTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (existTime >= existTimeout)
			return this;

		return null;
    }
}