using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Framework
{
    public class Dot
    {
        private static Texture2D sprite;
        private Vector2 position;
        private static bool isInitialized = false;

        public Dot(Vector2 pos)
        {
            position.X = pos.X - 2.5f;
            position.Y = pos.Y - 2.5f;
            if (!isInitialized)
            {
                sprite = Manager.Content.Load<Texture2D>("graphic/common/dotBig");
                isInitialized = true;
            }
        }

        public void Draw(Color color)
        {
            Manager.SpriteBatch.Draw(sprite, position, color);
        }

        public override string ToString()
        {
            return new Vector2(position.X+2.5f, position.Y+2.5f).ToString();
        }
    }
}
