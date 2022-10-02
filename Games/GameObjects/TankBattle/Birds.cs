using Framework;

namespace AxeGameCollection.GameObjects.TankBattle;

public static class Birds
{
    public static Sprite Create(int probabilityToCreateNewOne)
    {
        if (probabilityToCreateNewOne > 0 && Rand.Bool(1, probabilityToCreateNewOne))
            SpriteManager.Add(new Bird());
        return null;
    }
}
