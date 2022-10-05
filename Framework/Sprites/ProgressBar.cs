using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Framework
{
	public class ProgressBar : Sprite
	{
		protected float percentage;
		private readonly float xOffset = 10;
		private readonly float yOffset = 5;
        private readonly string text;
		private readonly float textScale = 0.1f;
        private readonly Texture2D textureWhite;
        private Color progressColor;
        private Color textColor;
        public const int MaxValue = 100;

		public virtual float Percentage 
		{
			get => percentage;
			set => percentage = MathHelper.Clamp(value, 0, MaxValue);
		}

		public ProgressBar(Vector2 position, float scale, Color color, int layer, string text = "") : base("graphic/common/progressBar", position, 0, scale, color, layer, CollisionType.None)
		{
			this.text = text;
            textureWhite = Manager.Content.Load<Texture2D>("graphic/common/blank_white");
            xOffset *= scale;
            yOffset *= scale;
			SetActive(true);
        }

        public void SetActive(bool active)
        {
            progressColor = active ? color : Color.LightGray;
            textColor = active ? Color.Black : Color.Gray;
        }

		public override void  Draw()
		{
			Manager.SpriteBatch.Draw(
				textureWhite, 
				new Rectangle((int)(Position.X + xOffset), (int)(Position.Y + yOffset), (int)(Width - xOffset * 2), (int)(Height - yOffset * 2)), 
				null, 
				Color.White, 
				0, 
				Vector2.Zero,
				SpriteEffects.None, 
				0);

			Manager.SpriteBatch.Draw(
				textureWhite, 
				new Rectangle((int)(Position.X + xOffset), (int)(Position.Y + yOffset), (int)(percentage / MaxValue * (Width - xOffset * 2)), (int)(Height - yOffset * 2)), 
				null, 
				progressColor, 
				0, 
				Vector2.Zero, 
				SpriteEffects.None, 
				0);

			Manager.SpriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

			if (text != "")
			{
				var font = Manager.Fonts.Get("Standard");
				var textSize = font.MeasureString(text) * textScale * scale;
				Manager.SpriteBatch.DrawString(
					font,
					text,
					new Vector2(Center.X - textSize.X / 2f, Center.Y - textSize.Y / 2f - 1.5f * scale),
					textColor,
					0,
					Vector2.Zero,
					scale*textScale,
					SpriteEffects.None,
					0);
			}
		}
	}
}
