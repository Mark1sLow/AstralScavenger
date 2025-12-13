using System.Collections.Generic;
using AstralScavenger.Models.Entities;

namespace AstralScavenger.Models.States;

public class GameState
{
    public List<Debris> Debris { get; set; } = new();
    public Player Player { get; set; } = new();
    public int Score { get; set; }

    // ← новые поля для ресурсов по типам
    public int CollectedMetal { get; set; }
    public int CollectedGold { get; set; }
    public int CollectedDiamond { get; set; }

    // ← требования для уровней с целями по ресурсам
    public int RequiredMetal { get; set; }
    public int RequiredGold { get; set; }
    public int RequiredDiamond { get; set; }
    public bool UsesResourceGoals { get; set; } = false;

    public GameLevel CurrentLevel { get; set; } = GameLevel.Tutorial;
    public float TimeLeft { get; set; } = 90.0f; // по умолчанию — как в ТЗ Уровень 1
    public bool IsGameOver { get; set; } = false;
    public bool IsLevelComplete { get; set; } = false;

    public GameScreen CurrentScreen { get; set; } = GameScreen.Menu;

    public PlayerColor SelectedColor { get; set; } = PlayerColor.Blue;
    public ShipType SelectedShipType { get; set; } = ShipType.Cargo;
    public GameDifficulty Difficulty { get; set; } = GameDifficulty.Normal;
    public BackgroundStyle SelectedBackground { get; set; } = BackgroundStyle.Default;
}