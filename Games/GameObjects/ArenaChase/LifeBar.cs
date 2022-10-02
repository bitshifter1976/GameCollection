using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.ArenaChase;

public sealed class LifeBar : ProgressBar
{
	private static Color ColorHealthy = Color.Green;
	private static Color ColorUnhealthy = Color.Red;
	private static int PercentageUnhealthy = 20;

	public override float Percentage
	{
		get => percentage;
		set
		{
			percentage = MathHelper.Clamp(value, 0, MaxValue);
			color = (percentage > PercentageUnhealthy) ? ColorHealthy : ColorUnhealthy;
		}
	}

	public LifeBar(float scale) : base(Vector2.Zero, scale, ColorHealthy, (int)Layer.Hud, "Energy")
	{
	}

    public override Sprite Update(GameTime gameTime)
    {
        return null;
    }
}
