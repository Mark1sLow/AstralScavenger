using System.Collections.Generic;
using AstralScavenger.Models.Entities;

namespace AstralScavenger.Models.States;

public class GameState
{
    public List<Debris> Debris { get; set; } = new();
    public Player Player { get; set; } = new();
    public int Score { get; set; }
    public int ResourceScore { get; set; } = 0; 

    public int CollectedMetal { get; set; }
    public int CollectedGold { get; set; }
    public int CollectedDiamond { get; set; }

    public int RequiredMetal { get; set; }
    public int RequiredGold { get; set; }
    public int RequiredDiamond { get; set; }
    public bool UsesResourceGoals { get; set; } = false;

    public GameLevel CurrentLevel { get; set; } = GameLevel.Tutorial;
    public float TimeLeft { get; set; } = 90.0f;
    public bool IsGameOver { get; set; } = false;
    public bool IsLevelComplete { get; set; } = false;

    public GameScreen CurrentScreen { get; set; } = GameScreen.Menu;

    public PlayerColor SelectedColor { get; set; } = PlayerColor.Blue;
    public ShipType SelectedShipType { get; set; } = ShipType.Cargo;
    public List<Debris> ActiveStaticHazards { get; set; } = new();

    private Dictionary<GameLevel, GameDifficulty> _levelDifficulties = new()
    {
        { GameLevel.Tutorial, GameDifficulty.Normal },
        { GameLevel.ResourceHunt, GameDifficulty.Normal },
        { GameLevel.ResourceGoal1, GameDifficulty.Normal },
        { GameLevel.InvertedControls, GameDifficulty.Normal },
        { GameLevel.StaticHazards, GameDifficulty.Normal },
        { GameLevel.ResourceGoal2, GameDifficulty.Normal },
        { GameLevel.RichHunt, GameDifficulty.Normal },
        { GameLevel.DarkZone, GameDifficulty.Normal },
        { GameLevel.StaticInverted, GameDifficulty.Normal },
        { GameLevel.DarkStatic, GameDifficulty.Normal },
        { GameLevel.DarkInverted, GameDifficulty.Normal },
        { GameLevel.RichHuntPlus, GameDifficulty.Normal },
        { GameLevel.RichHuntPlusDark, GameDifficulty.Normal },
        { GameLevel.RichHuntPlusChaos, GameDifficulty.Normal },
        { GameLevel.Survival, GameDifficulty.Normal }
    };

    public GameDifficulty GetDifficultyForLevel(GameLevel level) => _levelDifficulties[level];

    public void SetDifficultyForLevel(GameLevel level, GameDifficulty difficulty) => _levelDifficulties[level] = difficulty;

    public BackgroundStyle SelectedBackground { get; set; } = BackgroundStyle.Default;

    public float ElapsedTime { get; set; } = 0f; 
    public float LastStaticSpawnTime { get; set; } = 0f;
    public int TotalResourcesCollected { get; set; } = 0; 
}