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
        int spawnChance;

        // Уровень 12 — выживание
        if (level == GameLevel.Survival)
        {
            // Сложность растёт до 5 минут (300 секунд)
            float difficultyFactor = 1.0f;
            if (state.TimeLeft > 300)
            {
                difficultyFactor = 1.0f + (float)(state.Score / 150.0);
            }
            else
            {
                // После 5 минут сложность фиксирована
                difficultyFactor = 1.0f + (float)(state.Score / 150.0);
            }

            spawnChance = (int)(8 * difficultyFactor);
            if (_rand.Next(0, 100) < spawnChance)
            {
                var side = _rand.Next(0, 4);
                Point pos, vel;

                switch (side)
                {
                    case 0: pos = new Point(_rand.Next(0, width), -45); vel = new Point(_rand.Next(-5, 6), _rand.Next(2, 7)); break;
                    case 1: pos = new Point(_rand.Next(0, width), height + 45); vel = new Point(_rand.Next(-5, 6), _rand.Next(-7, -2)); break;
                    case 2: pos = new Point(-45, _rand.Next(0, height)); vel = new Point(_rand.Next(2, 7), _rand.Next(-5, 6)); break;
                    case 3: pos = new Point(width + 45, _rand.Next(0, height)); vel = new Point(_rand.Next(-7, -2), _rand.Next(-5, 6)); break;
                    default: pos = new Point(0, 0); vel = new Point(0, 0); break;
                }

                int r = _rand.Next(0, 100);
                DebrisType type;
                bool isCollectible = true;
                int size = 45;

                if (r < 35) // Металл
                {
                    type = DebrisType.Metal;
                }
                else if (r < 47) // Золото (12%)
                {
                    type = DebrisType.Gold;
                }
                else if (r < 50) // Алмаз (3%)
                {
                    type = DebrisType.Diamond;
                }
                else if (r < 53) // Топливо (3%)
                {
                    type = DebrisType.Fuel;
                }
                else if (r < 55) // Энергия (2%)
                {
                    type = DebrisType.Energy;
                }
                else // Метеорит (45%)
                {
                    isCollectible = false;
                    type = DebrisType.Standard;
                    size = _rand.Next(45, 80);
                }

                state.Debris.Add(new Debris
                {
                    Position = pos,
                    Velocity = vel,
                    IsCollectible = isCollectible,
                    Type = type,
                    Size = isCollectible ? 45 : size
                });
            }

            // Появление статичных метеоритов после 8 минут (480 секунд) в темноте
            if (state.TimeLeft <= 120 && state.Debris.Count(d => d.IsStatic) < 15) // Если прошло 3 мин в темноте
            {
                var safeZone = new Rectangle(
                    state.Player.Position.X - 100,
                    state.Player.Position.Y - 100,
                    200,
                    200
                );

                var x = _rand.Next(50, width - 50);
                var y = _rand.Next(50, height - 50);
                var pos = new Point(x, y);
                var size = _rand.Next(45, 90);
                var rect = new Rectangle(x, y, size, size);

                if (!safeZone.IntersectsWith(rect))
                {
                    bool overlap = false;
                    foreach (var d in state.Debris)
                    {
                        if (d.IsStatic && new Rectangle(d.Position.X, d.Position.Y, d.Size, d.Size).IntersectsWith(rect))
                        {
                            overlap = true;
                            break;
                        }
                    }

                    if (!overlap)
                    {
                        state.Debris.Add(new Debris
                        {
                            Position = pos,
                            IsCollectible = false,
                            IsStatic = true,
                            Type = DebrisType.Standard,
                            Size = size
                        });
                    }
                }
            }

            return;
        }

        // Остальные уровни — сбалансированные шансы
        spawnChance = level switch
        {
            GameLevel.Tutorial or GameLevel.ResourceHunt => 10,
            GameLevel.ResourceGoal1 or GameLevel.InvertedControls => 12,
            GameLevel.StaticHazards or GameLevel.ResourceGoal2 => 14,
            GameLevel.RichHunt or GameLevel.DarkZone => 15,
            GameLevel.StaticInverted or GameLevel.DarkStatic or GameLevel.DarkInverted => 16,
            _ => 8
        };

        if (_rand.Next(0, 100) < spawnChance)
        {
            var side = _rand.Next(0, 4);
            Point pos, vel;

            switch (side)
            {
                case 0: pos = new Point(_rand.Next(0, width), -45); vel = new Point(_rand.Next(-4, 5), _rand.Next(1, 6)); break;
                case 1: pos = new Point(_rand.Next(0, width), height + 45); vel = new Point(_rand.Next(-4, 5), _rand.Next(-6, -1)); break;
                case 2: pos = new Point(-45, _rand.Next(0, height)); vel = new Point(_rand.Next(1, 6), _rand.Next(-4, 5)); break;
                case 3: pos = new Point(width + 45, _rand.Next(0, height)); vel = new Point(_rand.Next(-6, -1), _rand.Next(-4, 5)); break;
                default: pos = new Point(0, 0); vel = new Point(0, 0); break;
            }

            int r = _rand.Next(0, 100);
            DebrisType debrisType;
            bool isCollectible = true;
            int size = 45;

            if (level == GameLevel.Tutorial || level == GameLevel.ResourceHunt)
            {
                // Только металл на 1 и 2 уровне
                if (r < 40) // Металл
                {
                    debrisType = DebrisType.Metal;
                }
                else // Метеорит
                {
                    isCollectible = false;
                    debrisType = DebrisType.Standard;
                    size = _rand.Next(45, 80);
                }
            }
            else
            {
                // Остальные уровни
                if (r < 40) // Металл
                {
                    debrisType = DebrisType.Metal;
                }
                else if (r < 55) // Золото (15%)
                {
                    debrisType = DebrisType.Gold;
                }
                else if (r < 60) // Алмаз (5%)
                {
                    debrisType = DebrisType.Diamond;
                }
                else // Метеорит
                {
                    isCollectible = false;
                    debrisType = DebrisType.Standard;
                    size = _rand.Next(45, 80);
                }
            }

            state.Debris.Add(new Debris
            {
                Position = pos,
                Velocity = vel,
                IsCollectible = isCollectible,
                Type = debrisType,
                Size = isCollectible ? 45 : size
            });
        }

        // Статичные метеориты
        if (state.Debris.Count(d => d.IsStatic) == 0 && (
                level == GameLevel.StaticHazards ||
                level == GameLevel.ResourceGoal2 ||
                level == GameLevel.StaticInverted ||
                level == GameLevel.DarkStatic ||
                level == GameLevel.DarkInverted))
        {
            AddStaticHazards(state, width, height, count: _rand.Next(12, 16));
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
            case GameLevel.ResourceGoal1: state.TimeLeft = 60f; state.UsesResourceGoals = true; state.RequiredMetal = 5; state.RequiredGold = 3; break; // Уровень 3
            case GameLevel.InvertedControls: state.TimeLeft = 90f; break;
            case GameLevel.StaticHazards: state.TimeLeft = 90f; break;
            case GameLevel.ResourceGoal2: state.TimeLeft = 90f; state.UsesResourceGoals = true; state.RequiredMetal = 8; state.RequiredGold = 5; break; // Уровень 6
            case GameLevel.RichHunt: state.TimeLeft = 60f; break;
            case GameLevel.DarkZone: state.TimeLeft = 90f; break;
            case GameLevel.StaticInverted: state.TimeLeft = 120f; break;
            case GameLevel.DarkStatic: state.TimeLeft = 90f; break;
            case GameLevel.DarkInverted: state.TimeLeft = 120f; break;
            case GameLevel.Survival: state.TimeLeft = 60f; break; // Изначально 60 секунд
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