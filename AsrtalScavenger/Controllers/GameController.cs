using AstralScavenger.Models.Entities;
using AstralScavenger.Models.Logic;
using AstralScavenger.Models.States;
using AstralScavenger.Views.Rendering;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AstralScavenger.Controllers;

public class GameController
{
    private GameState _state = new();
    private GameLogic _logic;
    private Renderer _renderer;
    private int _width, _height;

    public GameController(int width, int height)
    {
        _width = width;
        _height = height;
        _logic = new GameLogic(width, height);
        _renderer = new Renderer(width, height, _state);
    }

    public void Update()
    {
        _logic.Update(_state);
    }

    public void UpdateWithDirection(int dx, int dy)
    {
        if (_state.CurrentScreen == GameScreen.Playing)
        {
            _logic.SetPlayerDirectionAndMove(_state, dx, dy);
        }
        _logic.Update(_state);
    }

    public void Render(Graphics g, PlayerColor selectedColor, ShipType shipType, BackgroundStyle backgroundStyle)
    {
        _renderer.Render(g, selectedColor, shipType, backgroundStyle);
    }

    public void HandleMenuClick(Point mousePos)
    {
        var state = _state.CurrentScreen;

        if (state == GameScreen.Menu)
        {
            HandleMainMenuClick(mousePos);
        }
        else if (state == GameScreen.LevelSelection)
        {
            HandleLevelSelectionClick(mousePos);
        }
        else if (state == GameScreen.Pause)
        {
            HandlePauseMenuClick(mousePos);
        }
        else if (state == GameScreen.LevelComplete)
        {
            HandleLevelCompleteClick(mousePos);
        }
        else if (state == GameScreen.GameOverScreen)
        {
            HandleGameOverClick(mousePos);
        }
        else if (state == GameScreen.Customization)
        {
            HandleCustomizationClick(mousePos);
        }
    }

    private void HandleMainMenuClick(Point mousePos)
    {
        var buttonWidth = 240;
        var buttonHeight = 60;
        var spacing = 20;
        var totalHeight = 3 * buttonHeight + 2 * spacing;
        var startY = _height / 2 - totalHeight / 2;

        var startButton = new Rectangle(_width / 2 - buttonWidth / 2, startY, buttonWidth, buttonHeight);
        var customButton = new Rectangle(_width / 2 - buttonWidth / 2, startY + (buttonHeight + spacing), buttonWidth, buttonHeight);
        var closeButton = new Rectangle(_width / 2 - buttonWidth / 2, startY + 2 * (buttonHeight + spacing), buttonWidth, buttonHeight);

        if (startButton.Contains(mousePos))
        {
            _state.CurrentScreen = GameScreen.LevelSelection;
        }
        else if (customButton.Contains(mousePos))
        {
            _state.CurrentScreen = GameScreen.Customization;
        }
        else if (closeButton.Contains(mousePos))
        {
            Application.Exit();
        }
    }

    private void HandleLevelSelectionClick(Point mousePos)
    {
        var buttonWidth = 200;
        var buttonHeight = 50;
        var spacing = 15;
        var cols = 3;
        var rows = 4;
        var startX = _width / 2 - (cols * buttonWidth + (cols - 1) * spacing) / 2;
        var startY = 150;

        for (int i = 0; i < 12; i++)
        {
            int col = i % cols;
            int row = i / cols;
            var rect = new Rectangle(
                startX + col * (buttonWidth + spacing),
                startY + row * (buttonHeight + spacing),
                buttonWidth,
                buttonHeight
            );

            if (rect.Contains(mousePos))
            {
                _state.CurrentLevel = (GameLevel)(i + 1);
                _state.CurrentScreen = GameScreen.Playing;
                ResetGameState();
                return;
            }
        }

        var backButton = new Rectangle(_width / 2 - buttonWidth / 2, startY + rows * (buttonHeight + spacing) + 20, buttonWidth, buttonHeight);
        if (backButton.Contains(mousePos))
        {
            _state.CurrentScreen = GameScreen.Menu;
        }
    }

    private void HandleCustomizationClick(Point mousePos)
    {
        // Сложность
        var diffRect = new Rectangle(_width / 2 + 50, 190, 40, 40);
        var diffLeft = new Rectangle(_width / 2 - 30, 190, 40, 40);
        if (diffRect.Contains(mousePos))
        {
            _state.Difficulty = (GameDifficulty)(((int)_state.Difficulty + 1) % 4);
        }
        else if (diffLeft.Contains(mousePos))
        {
            _state.Difficulty = (GameDifficulty)(((int)_state.Difficulty + 3) % 4);
        }

        // Тип корабля
        var shipRect = new Rectangle(_width / 2 + 50, 260, 40, 40);
        var shipLeft = new Rectangle(_width / 2 - 30, 260, 40, 40);
        if (shipRect.Contains(mousePos))
        {
            _state.SelectedShipType = (ShipType)(((int)_state.SelectedShipType + 1) % 2);
        }
        else if (shipLeft.Contains(mousePos))
        {
            _state.SelectedShipType = (ShipType)(((int)_state.SelectedShipType + 1) % 2);
        }

        // Цвет
        var colorRect = new Rectangle(_width / 2 + 50, 330, 40, 40);
        var colorLeft = new Rectangle(_width / 2 - 30, 330, 40, 40);
        if (colorRect.Contains(mousePos))
        {
            _state.SelectedColor = (PlayerColor)(((int)_state.SelectedColor + 1) % 4);
        }
        else if (colorLeft.Contains(mousePos))
        {
            _state.SelectedColor = (PlayerColor)(((int)_state.SelectedColor + 3) % 4);
        }

        // Фон
        var bgRect = new Rectangle(_width / 2 + 50, 400, 40, 40);
        var bgLeft = new Rectangle(_width / 2 - 30, 400, 40, 40);
        if (bgRect.Contains(mousePos))
        {
            _state.SelectedBackground = (BackgroundStyle)(((int)_state.SelectedBackground + 1) % 5);
        }
        else if (bgLeft.Contains(mousePos))
        {
            _state.SelectedBackground = (BackgroundStyle)(((int)_state.SelectedBackground + 4) % 5);
        }

        var buttonWidth = 240;
        var buttonHeight = 60;
        var backButton = new Rectangle(_width / 2 - buttonWidth / 2, _height - 120, buttonWidth, buttonHeight);
        if (backButton.Contains(mousePos))
        {
            _state.CurrentScreen = GameScreen.Menu;
        }
    }

    private void HandlePauseMenuClick(Point mousePos)
    {
        var buttonWidth = 240;
        var buttonHeight = 60;
        var spacing = 20;
        var totalHeight = 3 * buttonHeight + 2 * spacing;
        var startY = _height / 2 - totalHeight / 2;

        var resume = new Rectangle(_width / 2 - buttonWidth / 2, startY, buttonWidth, buttonHeight);
        var menu = new Rectangle(_width / 2 - buttonWidth / 2, startY + buttonHeight + spacing, buttonWidth, buttonHeight);
        var restart = new Rectangle(_width / 2 - buttonWidth / 2, startY + 2 * (buttonHeight + spacing), buttonWidth, buttonHeight);

        if (resume.Contains(mousePos)) _state.CurrentScreen = GameScreen.Playing;
        else if (menu.Contains(mousePos)) _state.CurrentScreen = GameScreen.Menu;
        else if (restart.Contains(mousePos))
        {
            _state.CurrentScreen = GameScreen.Playing;
            ResetGameState();
        }
    }

    private void HandleLevelCompleteClick(Point mousePos)
    {
        var buttonWidth = 240;
        var buttonHeight = 60;
        var spacing = 20;
        var totalHeight = 3 * buttonHeight + 2 * spacing;
        var startY = _height / 2 - totalHeight / 2;

        var menu = new Rectangle(_width / 2 - buttonWidth / 2, startY, buttonWidth, buttonHeight);
        var restart = new Rectangle(_width / 2 - buttonWidth / 2, startY + buttonHeight + spacing, buttonWidth, buttonHeight);
        var next = new Rectangle(_width / 2 - buttonWidth / 2, startY + 2 * (buttonHeight + spacing), buttonWidth, buttonHeight);

        if (menu.Contains(mousePos)) _state.CurrentScreen = GameScreen.Menu;
        else if (restart.Contains(mousePos))
        {
            _state.CurrentScreen = GameScreen.Playing;
            ResetGameState();
        }
        else if (next.Contains(mousePos))
        {
            if ((int)_state.CurrentLevel < 12)
            {
                _state.CurrentLevel = (GameLevel)((int)_state.CurrentLevel + 1);
                _state.CurrentScreen = GameScreen.Playing;
                ResetGameState();
            }
            else
            {
                _state.CurrentScreen = GameScreen.Menu;
            }
        }
    }

    private void HandleGameOverClick(Point mousePos)
    {
        var buttonWidth = 240;
        var buttonHeight = 60;
        var spacing = 20;
        var totalHeight = 3 * buttonHeight + 2 * spacing;
        var startY = _height / 2 - totalHeight / 2;

        var menu = new Rectangle(_width / 2 - buttonWidth / 2, startY, buttonWidth, buttonHeight);
        var settings = new Rectangle(_width / 2 - buttonWidth / 2, startY + buttonHeight + spacing, buttonWidth, buttonHeight); // ← Добавлена кнопка "НАСТРОЙКИ"
        var restart = new Rectangle(_width / 2 - buttonWidth / 2, startY + 2 * (buttonHeight + spacing), buttonWidth, buttonHeight);

        if (menu.Contains(mousePos)) _state.CurrentScreen = GameScreen.Menu;
        else if (settings.Contains(mousePos)) _state.CurrentScreen = GameScreen.Customization; // ← Переход в настройки
        else if (restart.Contains(mousePos))
        {
            _state.CurrentScreen = GameScreen.Playing;
            ResetGameState();
        }
    }

    public void HandleEscape()
    {
        if (_state.CurrentScreen == GameScreen.Playing)
        {
            _state.CurrentScreen = GameScreen.Pause;
        }
    }

    private void ResetGameState()
    {
        _state.Player = new Player();
        _state.Debris.Clear();
        _state.Score = 0;
        _state.CollectedMetal = 0;
        _state.CollectedGold = 0;
        _state.CollectedDiamond = 0;
        _state.IsGameOver = false;
        _state.IsLevelComplete = false;
        _state.Player.Health = 3;

        // Установка скорости в зависимости от типа корабля
        _state.Player.Speed = _state.SelectedShipType == ShipType.Transport ? 8 : 6; // +30% ≈ 8

        // Установка стартовой позиции
        _state.Player.Position = new Point(
            _width / 2 - _state.Player.Size / 2,
            _height / 2 - _state.Player.Size / 2
        );

        // Настройка уровня
        var levelLogic = new LevelLogic();
        levelLogic.SetLevelRequirements(_state);

        // Убедиться, что старт безопасен
        _logic.EnsurePlayerSafePosition(_state);
    }

    public GameState GetGameState() => _state;
}