using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.TankBattle;

public sealed class PowerBar : ProgressBar
{
	public PowerBar(float scale) : base(Vector2.Zero, scale, Color.DeepSkyBlue, (int)Layer.Hud, "Power")
	{
    }

    public override Sprite Update(GameTime gameTime)
    {
        return null;
    }
}
