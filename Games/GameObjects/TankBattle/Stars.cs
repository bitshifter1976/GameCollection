using System;
using System.Collections.Generic;
using Framework;
using Microsoft.Xna.Framework;

namespace AxeGameCollection.GameObjects.TankBattle;

public class Stars : Sprite
{
	public static void Create(int count)
	{
        for (var i = 0; i < count; i++)
			SpriteManager.Add(new Star(new Vector2(Rand.Float(1, Manager.DesignWidth - 1), Rand.Float(1, Water.Top - 1))));
	}
}
