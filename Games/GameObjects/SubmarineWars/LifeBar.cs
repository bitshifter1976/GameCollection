using Framework;
using Microsoft.Xna.Framework;
using static AxeGameCollection.Screens.ScreenGameSubmarineWars;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public sealed class LifeBar : ProgressBar
{
	private static Color ColorHealthy = Color.Green;
	private static Color ColorUnhealthy = Color.Red;
	private static readonly int PercentageUnhealthy = 20;

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
