using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.TankBattle;

public class Star : Sprite
{
    private bool alphaUp;
    private const int minAlpha = 100;
    private const int deltaAlpha = 2;

	public Star(Vector2 position) : base("graphic/common/dotBig", position, 0f, 0.25f, (int)Layer.Stars, CollisionType.None)
	{
		color = Color.Gold;
		color.B = (byte)Rand.Int(minAlpha, byte.MaxValue);
	}

	public override Sprite Update(GameTime gameTime)
	{
		if (alphaUp)
			if (color.B < byte.MaxValue - deltaAlpha)
				color.B += deltaAlpha;
			else
				alphaUp = false;
		else
			if (color.B > minAlpha + deltaAlpha)
				color.B -= deltaAlpha;
			else
				alphaUp = true;
		
		return null;
	}
}
