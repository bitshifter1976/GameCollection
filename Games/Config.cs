using Framework;

namespace AxeGameCollection
{
    public class Config : ConfigBase
    {
        public static readonly int DesignWidth = 1920;
        public static readonly int DesignHeight = 1080;

        public static string LogDir => GetOrDefault("LogDir", string.Empty);
        public static LogLevel LogLevel => GetOrDefault("LogLevel", LogLevel.Error);
        public static bool SoundEnabled => GetOrDefault("SoundEnabled", true);
        public static bool FullScreen => GetOrDefault("FullScreen", true);
        public static int Height { get; set; } = GetOrDefault("Height", 1080);
        public static int Width { get; set; } = GetOrDefault("Width", 1920);
        public static string Title => GetOrDefault("Title", "");
        public static bool Debug { get; set; } = GetOrDefault("Debug", false);
        public static AxeGameCollection.Games StartGameImmediate => GetOrDefault("StartGameImmediate", AxeGameCollection.Games.None);
        public static int LevelArenaChase { get; set; } = GetOrDefault("LevelArenaChase", 1);
        public static int LevelTankBattle { get; set; } = GetOrDefault("LevelTankBattle", 1);
        public static int LevelSubmarineWars { get; set; } = GetOrDefault("LevelSubmarineWars", 1);
        public static int LevelMrSunny { get; set; } = GetOrDefault("LevelMrSunny", 1);
    }
}
