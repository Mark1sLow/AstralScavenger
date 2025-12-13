namespace AstralScavenger.Models.States;

public enum PlayerColor
{
    Blue,
    Red,
    Green,
    Orange
}

public enum ShipType
{
    Cargo,
    Transport
}

public enum GameDifficulty
{
    Easy,
    Normal,
    Hard,
    Extreme
}

public enum GameScreen
{
    Menu,
    Playing,
    Pause,
    Customization,
    LevelSelection,
    LevelComplete,
    GameOverScreen
}

public enum GameLevel
{
    Tutorial = 1,           // Уровень 1
    ResourceHunt = 2,       // Уровень 2
    ResourceGoal1 = 3,      // Уровень 3
    InvertedControls = 4,   // Уровень 4
    StaticHazards = 5,      // Уровень 5
    ResourceGoal2 = 6,      // Уровень 6
    RichHunt = 7,           // Уровень 7
    DarkZone = 8,           // Уровень 8
    StaticInverted = 9,     // Уровень 9
    DarkStatic = 10,        // Уровень 10
    DarkInverted = 11,      // Уровень 11
    Survival = 12           // Уровень 12
}

public enum BackgroundStyle
{
    Default,        // Space1
    DarkSpace,      // Space2
    MultiSpace,     // Space3
    PixelSpace,     // PixelSpace
    Nebula          // Nebula
}

public enum DebrisType
{
    Standard, // мусор
    Metal,    // ресурс: металл
    Gold,     // ресурс: золото
    Diamond,  // ресурс: алмаз
    Energy,   // +1 жизнь
    Fuel      // +20 сек топлива
}