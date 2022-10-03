using Framework;

namespace AxeGameCollection.GameObjects.SubmarineWars;

public static class Fishes
{
    public static Sprite Create(int probabilityToCreateNewOne)
    {
        if (probabilityToCreateNewOne > 0 && Rand.Bool(1, probabilityToCreateNewOne))
            SpriteManager.Add(new Fish());
        return null;
    }
}
