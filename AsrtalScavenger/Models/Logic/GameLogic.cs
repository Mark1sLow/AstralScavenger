using System;
using System.Collections.Generic;
using System.Drawing;
using AstralScavenger.Models.Entities;
using AstralScavenger.Models.States;

namespace AstralScavenger.Models.Logic;

public class GameLogic
{
    private readonly LevelLogic _levelLogic = new();
    private readonly PlayerLogic _playerLogic = new();
    private Random _rand = new();
    private int _width, _height;

    public GameLogic(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void Update(GameState state)
    {
        if (state.CurrentScreen != GameScreen.Playing || state.IsGameOver || state.IsLevelComplete)
            return;

        if (state.CurrentLevel != GameLevel.Survival)
        {
            state.TimeLeft -= 1f / 60f;
            if (state.TimeLeft <= 0)
            {
                state.IsGameOver = true;
                state.CurrentScreen = GameScreen.GameOverScreen;
                return;
            }
        }

        // Движение мусора
        foreach (var d in state.Debris)
        {
            if (!d.IsActive || d.IsStatic) continue;
            d.Position = new Point(d.Position.X + d.Velocity.X, d.Position.Y + d.Velocity.Y);
            if (d.Position.X < -d.Size || d.Position.X > _width || d.Position.Y < -d.Size || d.Position.Y > _height)
                d.IsActive = false;
        }
        state.Debris.RemoveAll(d => !d.IsActive);

        CheckCollisions(state);
        _levelLogic.GenerateDebrisForLevel(state, _width, _height);

        if (_levelLogic.IsLevelComplete(state))
        {
            state.IsLevelComplete = true;
            state.CurrentScreen = GameScreen.LevelComplete;
        }

        _playerLogic.UpdatePlayerRotation(state.Player);
    }

    private void CheckCollisions(GameState state)
    {
        var playerRect = new Rectangle(state.Player.Position.X, state.Player.Position.Y, state.Player.Size, state.Player.Size);

        foreach (var d in state.Debris)
        {
            if (!d.IsActive) continue;
            var debrisRect = new Rectangle(d.Position.X, d.Position.Y, d.Size, d.Size);

            if (playerRect.IntersectsWith(debrisRect))
            {
                if (d.IsCollectible)
                {
                    switch (d.Type)
                    {
                        case DebrisType.Metal:
                            state.Score += 10;
                            state.CollectedMetal++;
                            break;
                        case DebrisType.Gold:
                            state.Score += 15;
                            state.CollectedGold++;
                            break;
                        case DebrisType.Diamond:
                            state.Score += 20; 
                            state.CollectedDiamond++;
                            break;
                        case DebrisType.Energy:
                            state.Player.Health++;
                            break;
                        case DebrisType.Fuel:
                            state.TimeLeft += 20f;
                            break;
                    }
                    d.IsActive = false;
                }
                else
                {
                    if (d.IsStatic && (
                        state.CurrentLevel == GameLevel.StaticHazards ||
                        state.CurrentLevel == GameLevel.ResourceGoal2 ||
                        state.CurrentLevel == GameLevel.StaticInverted ||
                        state.CurrentLevel == GameLevel.DarkStatic ||
                        state.CurrentLevel == GameLevel.DarkInverted ||
                        state.CurrentLevel == GameLevel.Survival))
                    {
                        state.IsGameOver = true;
                        state.CurrentScreen = GameScreen.GameOverScreen;
                        return;
                    }
                    else
                    {
                        state.Player.Health--;
                        d.IsActive = false;
                        if (state.Player.Health <= 0)
                        {
                            state.IsGameOver = true;
                            state.CurrentScreen = GameScreen.GameOverScreen;
                        }
                    }
                }
            }
        }
    }

    public void SetPlayerDirectionAndMove(GameState state, int dx, int dy)
    {
        bool isInverted = state.CurrentLevel == GameLevel.InvertedControls ||
                          state.CurrentLevel == GameLevel.StaticInverted ||
                          state.CurrentLevel == GameLevel.DarkInverted;

        if (isInverted)
        {
            if (dx != 0 || dy != 0)
            {
                state.Player.TargetRotation = (float)Math.Atan2(dy, -dx) - (float)(Math.PI / 2);
            }
            var newX = state.Player.Position.X + dx * state.Player.Speed;
            var newY = state.Player.Position.Y - dy * state.Player.Speed;

            if (newX >= 0 && newX <= _width - state.Player.Size)
                state.Player.Position = new Point(newX, state.Player.Position.Y);
            if (newY >= 0 && newY <= _height - state.Player.Size)
                state.Player.Position = new Point(state.Player.Position.X, newY);
        }
        else
        {
            _playerLogic.SetPlayerDirectionAndMove(state.Player, dx, dy, _width, _height, state.Player.Size);
        }
    }

    public void EnsurePlayerSafePosition(GameState state)
    {
        var safeZone = new Rectangle(_width / 2 - 100, _height / 2 - 100, 200, 200);
        bool collision;
        do
        {
            collision = false;
            foreach (var d in state.Debris)
            {
                if (d.IsStatic && new Rectangle(d.Position.X, d.Position.Y, d.Size, d.Size).IntersectsWith(
                    new Rectangle(state.Player.Position.X, state.Player.Position.Y, state.Player.Size, state.Player.Size)))
                {
                    state.Player.Position = new Point(
                        _rand.Next(100, _width - 100),
                        _rand.Next(100, _height - 100)
                    );
                    collision = true;
                    break;
                }
            }
        } while (collision);
    }
}