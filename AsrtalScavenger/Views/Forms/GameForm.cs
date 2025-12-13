using AstralScavenger.Controllers;
using AstralScavenger.Models.States;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AstralScavenger.View.Forms;

public partial class GameForm : Form
{
    private System.Windows.Forms.Timer _gameTimer;
    private GameController _controller;

    private bool _upPressed = false;
    private bool _downPressed = false;
    private bool _leftPressed = false;
    private bool _rightPressed = false;

    public GameForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Normal;
        Size = Screen.PrimaryScreen.Bounds.Size;
        Location = Point.Empty;
        TopMost = true;
        DoubleBuffered = true;
        Text = "ASTRAL SCAVENGER";

        SetupGame();
    }

    private void SetupGame()
    {
        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        int screenHeight = Screen.PrimaryScreen.Bounds.Height;

        _controller = new GameController(screenWidth, screenHeight);

        _gameTimer = new System.Windows.Forms.Timer { Interval = 1000 / 60 };
        _gameTimer.Tick += (s, e) =>
        {
            if (_controller.GetGameState().CurrentScreen == GameScreen.Playing)
            {
                int dx = 0, dy = 0;

                if (_leftPressed) dx -= 1;
                if (_rightPressed) dx += 1;
                if (_upPressed) dy -= 1;
                if (_downPressed) dy += 1;

                _controller.UpdateWithDirection(dx, dy);
            }
            else
            {
                _controller.Update();
            }

            Invalidate();
        };
        _gameTimer.Start();

        KeyPreview = true; // Для обработки Escape на форме
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;
        Paint += OnPaint;
        MouseClick += OnMouseClick;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            _controller.HandleEscape();
            return;
        }

        switch (e.KeyCode)
        {
            case Keys.W: _upPressed = true; break;
            case Keys.S: _downPressed = true; break;
            case Keys.A: _leftPressed = true; break;
            case Keys.D: _rightPressed = true; break;
        }
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.W: _upPressed = false; break;
            case Keys.S: _downPressed = false; break;
            case Keys.A: _leftPressed = false; break;
            case Keys.D: _rightPressed = false; break;
        }
    }

    private void OnMouseClick(object sender, MouseEventArgs e)
    {
        var state = _controller.GetGameState();
        System.Diagnostics.Debug.WriteLine($"[GameForm] Текущий экран: {state.CurrentScreen}, Клик: {e.Location}");

        if (state.CurrentScreen == GameScreen.Menu || state.CurrentScreen == GameScreen.LevelSelection || state.CurrentScreen == GameScreen.Pause || state.CurrentScreen == GameScreen.LevelComplete || state.CurrentScreen == GameScreen.GameOverScreen || state.CurrentScreen == GameScreen.Customization)
        {
            System.Diagnostics.Debug.WriteLine("[GameForm] Вызываем HandleMenuClick");
            _controller.HandleMenuClick(e.Location);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("[GameForm] Клик проигнорирован, т.к. экран не поддерживает клики");
        }
    }

    private void OnPaint(object sender, PaintEventArgs e)
    {
        var state = _controller.GetGameState();
        _controller.Render(e.Graphics, state.SelectedColor, state.SelectedShipType, state.SelectedBackground);
    }
}