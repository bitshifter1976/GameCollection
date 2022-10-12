using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public sealed class Explosion : SpriteAnimatedMultiLine
{
	private float existTime = 0;
	private float existTimeout = 1;

	public Explosion(Vector2 position, float scale) : base("graphic/common/explosion", position, 25, 5, 25, 0, scale, (int)Layer.Explosion)
	{
		scrolling = true;
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