using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.TankBattle;

public sealed class Cloud : Sprite
{
    public Cloud(int index, float speed, Vector2 position, float rotation, float scale)	: 
		base("graphic/tankBattle/cloud" + index, position, rotation, scale, (int)Layer.Cloud, CollisionType.None)
	{
		this.speed = speed;
	}

	public override Sprite Update(GameTime gameTime)
	{
		position.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
		return null;
	}
}
