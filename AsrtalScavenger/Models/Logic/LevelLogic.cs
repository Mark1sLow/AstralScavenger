using System;
using AstralScavenger.Models.Entities;
using AstralScavenger.Models.States;

namespace AstralScavenger.Models.Logic;

public class LevelLogic
{
    private Random _rand = new();

    public void GenerateDebrisForLevel(GameState state, int width, int height)
    {
        var level = state.CurrentLevel;

        // Уровень 12 — бесконечный (выживание)
        if (level == GameLevel.Survival)
        {
            // Сложность растёт со временем
            float difficultyFactor = 1.0f + (float)(state.Score / 200.0); // каждые 200 очков — +1 сложность

            // Шанс генерации: базовый + рост
            int spawnChance = (int)(8 * difficultyFactor);
            if (_rand.Next(0, 100) < spawnChance)
            {
                var side = _rand.Next(0, 4);
                Point pos, vel;

                switch (side)
                {
                    case 0: pos = new Point(_rand.Next(0, width), -30); vel = new Point(_rand.Next(-4, 5), _rand.Next(2, 6)); break;
                    case 1: pos = new Point(_rand.Next(0, width), height + 30); vel = new Point(_rand.Next(-4, 5), _rand.Next(-6, -2)); break;
                    case 2: pos = new Point(-30, _rand.Next(0, height)); vel = new Point(_rand.Next(2, 6), _rand.Next(-4, 5)); break;
                    case 3: pos = new Point(width + 30, _rand.Next(0, height)); vel = new Point(_rand.Next(-6, -2), _rand.Next(-4, 5)); break;
                    default: pos = new Point(0, 0); vel = new Point(0, 0); break;
                }

                int rand = _rand.Next(0, 100);
                DebrisType type;
                bool isCollectible = true;
                int size = 40;

                if (rand < 50) // Металл
                {
                    type = DebrisType.Metal;
                }
                else if (rand < 66) // Золото (16%)
                {
                    type = DebrisType.Gold;
                }
                else if (rand < 74) // Алмаз (8%)
                {
                    type = DebrisType.Diamond;
                }
                else if (rand < 77) // Energy (3%)
                {
                    type = DebrisType.Energy;
                }
                else if (rand < 80) // Fuel (3%)
                {
                    type = DebrisType.Fuel;
                }
                else // метеорит (20%)
                {
                    isCollectible = false;
                    type = DebrisType.Standard;
                    size = _rand.Next(30, 70);
                }

                state.Debris.Add(new Debris
                {
                    Position = pos,
                    Velocity = vel,
                    IsCollectible = isCollectible,
                    Type = type,
                    Size = isCollectible ? 30 : size
                });
            }

            return;
        }

        // Для всех остальных уровней — фиксированный шанс 8%
        if (_rand.Next(0, 100) < 20)
        {
            var side = _rand.Next(0, 4);
            Point pos, vel;

            switch (side)
            {
                case 0: pos = new Point(_rand.Next(0, width), -30); vel = new Point(_rand.Next(-3, 4), _rand.Next(1, 5)); break;
                case 1: pos = new Point(_rand.Next(0, width), height + 30); vel = new Point(_rand.Next(-3, 4), _rand.Next(-5, -1)); break;
                case 2: pos = new Point(-30, _rand.Next(0, height)); vel = new Point(_rand.Next(1, 5), _rand.Next(-3, 4)); break;
                case 3: pos = new Point(width + 30, _rand.Next(0, height)); vel = new Point(_rand.Next(-5, -1), _rand.Next(-3, 4)); break;
                default: pos = new Point(0, 0); vel = new Point(0, 0); break;
            }

            // Вероятности по ТЗ
            int r = _rand.Next(0, 100);
            DebrisType debrisType;
            bool isCollectible = true;
            int size = 40;

            if (r < 20) // Металл
            {
                debrisType = DebrisType.Metal;
            }
            else if (r < 30) // Золото
            {
                debrisType = DebrisType.Gold;
            }
            else if (r < 6) // Алмаз (только с уровня 7+)
            {
                if (level >= GameLevel.RichHunt)
                    debrisType = DebrisType.Diamond;
                else
                    debrisType = DebrisType.Gold; // на ранних уровнях — золото вместо алмаза
            }
            else // метеорит
            {
                isCollectible = false;
                debrisType = DebrisType.Standard;
                size = _rand.Next(20, 80);
            }

            state.Debris.Add(new Debris
            {
                Position = pos,
                Velocity = vel,
                IsCollectible = isCollectible,
                Type = debrisType,
                Size = isCollectible ? size : size
            });
        }

        // Статичные метеориты для уровней с ними
        if (state.Debris.Count(d => d.IsStatic) == 0)
        {
            if (level == GameLevel.StaticHazards ||
                level == GameLevel.ResourceGoal2 ||
                level == GameLevel.StaticInverted ||
                level == GameLevel.DarkStatic ||
                level == GameLevel.DarkInverted ||
                level == GameLevel.Survival)
            {
                AddStaticHazards(state, width, height, count: _rand.Next(12, 16));
            }
        }
    }

    private void AddStaticHazards(GameState state, int width, int height, int count)
    {
        var safeZone = new Rectangle(width / 2 - 100, height / 2 - 100, 200, 200);

        for (int i = 0; i < count; i++)
        {
            var size = _rand.Next(45, 90);
            var position = new Point(_rand.Next(50, width - 50), _rand.Next(100, height - 100));
            var debrisRect = new Rectangle(position.X, position.Y, size, size);

            if (safeZone.IntersectsWith(debrisRect)) { i--; continue; }

            bool overlap = false;
            foreach (var d in state.Debris)
            {
                if (d.IsStatic && new Rectangle(d.Position.X, d.Position.Y, d.Size, d.Size).IntersectsWith(debrisRect))
                {
                    overlap = true;
                    break;
                }
            }
            if (overlap) { i--; continue; }

            state.Debris.Add(new Debris
            {
                Position = position,
                IsCollectible = false,
                IsStatic = true,
                Type = DebrisType.Standard,
                Size = size
            });
        }
    }

    public void SetLevelRequirements(GameState state)
    {
        var level = state.CurrentLevel;

        // Сброс
        state.Score = 0;
        state.CollectedMetal = 0;
        state.CollectedGold = 0;
        state.CollectedDiamond = 0;
        state.UsesResourceGoals = false;
        state.RequiredMetal = 0;
        state.RequiredGold = 0;
        state.RequiredDiamond = 0;

        switch (level)
        {
            case GameLevel.Tutorial: state.TimeLeft = 90f; break;
            case GameLevel.ResourceHunt: state.TimeLeft = 60f; break;
            case GameLevel.ResourceGoal1: state.TimeLeft = 60f; state.UsesResourceGoals = true; state.RequiredMetal = 5; state.RequiredGold = 3; break;
            case GameLevel.InvertedControls: state.TimeLeft = 90f; break;
            case GameLevel.StaticHazards: state.TimeLeft = 90f; break;
            case GameLevel.ResourceGoal2: state.TimeLeft = 90f; state.UsesResourceGoals = true; state.RequiredMetal = 8; state.RequiredGold = 5; break;
            case GameLevel.RichHunt: state.TimeLeft = 60f; break;
            case GameLevel.DarkZone: state.TimeLeft = 90f; break;
            case GameLevel.StaticInverted: state.TimeLeft = 120f; break;
            case GameLevel.DarkStatic: state.TimeLeft = 90f; break;
            case GameLevel.DarkInverted: state.TimeLeft = 120f; break;
            case GameLevel.Survival: state.TimeLeft = float.PositiveInfinity; break; // бесконечно
            default: state.TimeLeft = 90f; break;
        }
    }

    public bool IsLevelComplete(GameState state)
    {
        if (state.CurrentLevel == GameLevel.Survival)
            return false; // выживание — никогда не завершается сам по себе

        if (state.UsesResourceGoals)
        {
            return state.CollectedMetal >= state.RequiredMetal &&
                   state.CollectedGold >= state.RequiredGold &&
                   state.CollectedDiamond >= state.RequiredDiamond;
        }
        else
        {
            int required = state.CurrentLevel switch
            {
                GameLevel.Tutorial => 100,
                GameLevel.ResourceHunt => 100,
                GameLevel.InvertedControls => 100,
                GameLevel.StaticHazards => 150,
                GameLevel.RichHunt => 150,
                GameLevel.DarkZone => 150,
                GameLevel.StaticInverted => 150,
                GameLevel.DarkStatic => 200,
                GameLevel.DarkInverted => 150,
                _ => 100
            };
            return state.Score >= required;
        }
    }
}