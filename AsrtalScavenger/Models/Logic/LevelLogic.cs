using System;
using System.Collections.Generic;
using System.Drawing;
using AstralScavenger.Models.Entities;
using AstralScavenger.Models.States;

namespace AstralScavenger.Models.Logic;

public class LevelLogic
{
    private Random _rand = new();

    public void GenerateDebrisForLevel(GameState state, int width, int height)
    {
        switch (state.CurrentLevel)
        {
            case GameLevel.Tutorial:
                GenerateDebrisForLevel1(state, width, height);
                break;
            case GameLevel.ResourceHunt:
                GenerateDebrisForLevel2(state, width, height);
                break;
            case GameLevel.ResourceGoal1:
                GenerateDebrisForLevel3(state, width, height);
                break;
            case GameLevel.InvertedControls:
                GenerateDebrisForLevel4(state, width, height);
                break;
            case GameLevel.StaticHazards:
                GenerateDebrisForLevel5(state, width, height);
                break;
            case GameLevel.ResourceGoal2:
                GenerateDebrisForLevel6(state, width, height);
                break;
            case GameLevel.RichHunt:
                GenerateDebrisForLevel7(state, width, height);
                break;
            case GameLevel.DarkZone:
                GenerateDebrisForLevel8(state, width, height);
                break;
            case GameLevel.StaticInverted:
                GenerateDebrisForLevel9(state, width, height);
                break;
            case GameLevel.DarkStatic:
                GenerateDebrisForLevel10(state, width, height);
                break;
            case GameLevel.DarkInverted:
                GenerateDebrisForLevel11(state, width, height);
                break;
            case GameLevel.Survival:
                GenerateDebrisForLevel12(state, width, height);
                break;
            default:
                GenerateDebrisForLevelDefault(state, width, height);
                break;
        }
    }


    private void GenerateDebrisForLevel1(GameState state, int width, int height)
    {
        int spawnChance = 15; // Базовый шанс
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

            if (r < 20)
            {
                debrisType = DebrisType.Metal;
            }
            else 
            {
                isCollectible = false;
                debrisType = DebrisType.Standard;
                size = _rand.Next(45, 80);
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
    }

    private void GenerateDebrisForLevel2(GameState state, int width, int height)
    {
        int spawnChance = 17;
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

            if (r < 10)
            {
                debrisType = DebrisType.Gold;
            }
            else if (r < 25) 
            {
                debrisType = DebrisType.Metal;
            }
            else 
            {
                isCollectible = false;
                debrisType = DebrisType.Standard;
                size = _rand.Next(45, 80);
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
    }

    private void GenerateDebrisForLevel3(GameState state, int width, int height)
    {
        int spawnChance = 10;
        if (_rand.Next(0, 100) < spawnChance)
        {
            int resourceR = _rand.Next(0, 100);
            if (resourceR < 50)
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

                if (r < 40)
                {
                    debrisType = DebrisType.Metal;
                }
                else if (r < 55) 
                {
                    debrisType = DebrisType.Gold;
                }
                else 
                {
                    isCollectible = false;
                    debrisType = DebrisType.Standard;
                    size = _rand.Next(45, 80);
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
            else
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

                state.Debris.Add(new Debris
                {
                    Position = pos,
                    Velocity = vel,
                    IsCollectible = false,
                    Type = DebrisType.Standard,
                    Size = _rand.Next(45, 80)
                });
            }
        }
    }

    private void GenerateDebrisForLevel4(GameState state, int width, int height)
    {
        int spawnChance = 10;
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

            if (r < 12) 
            {
                debrisType = DebrisType.Gold;
            }
            else if (r < 30)
            {
                debrisType = DebrisType.Metal;
            }
            else 
            {
                isCollectible = false;
                debrisType = DebrisType.Standard;
                size = _rand.Next(45, 80);
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
    }

    private void GenerateDebrisForLevel5(GameState state, int width, int height)
    {
        int spawnChance = 10;
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

            if (r < 10)
            {
                debrisType = DebrisType.Gold;
            }
            else if (r < 25) 
            {
                debrisType = DebrisType.Metal;
            }
            else 
            {
                isCollectible = false;
                debrisType = DebrisType.Standard;
                size = _rand.Next(45, 80);
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

        if (state.Debris.Count(d => d.IsStatic) == 0)
        {
            AddStaticHazards(state, width, height, count: _rand.Next(12, 16));
        }
    }

    private void GenerateDebrisForLevel6(GameState state, int width, int height)
    {
        int spawnChance = 10;
        if (_rand.Next(0, 100) < spawnChance)
        {
            int resourceR = _rand.Next(0, 100);
            if (resourceR < 50)
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

                if (r < 6)
                {
                    debrisType = DebrisType.Diamond;
                }
                else if (r < 13)
                {
                    debrisType = DebrisType.Gold;
                }
                else if (r < 35) 
                {
                    debrisType = DebrisType.Metal;
                }
                else
                {
                    isCollectible = false;
                    debrisType = DebrisType.Standard;
                    size = _rand.Next(45, 70);
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
            else
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

                state.Debris.Add(new Debris
                {
                    Position = pos,
                    Velocity = vel,
                    IsCollectible = false,
                    Type = DebrisType.Standard,
                    Size = _rand.Next(45, 70)
                });
            }
        }

        if (state.Debris.Count(d => d.IsStatic) == 0)
        {
            AddStaticHazards(state, width, height, count: _rand.Next(12, 14));
        }
    }

    private void GenerateDebrisForLevel7(GameState state, int width, int height)
    {
        int spawnChance = 20;
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

            if (r < 5)
            {
                debrisType = DebrisType.Diamond;
            }
            else if (r < 10)
            {
                debrisType = DebrisType.Gold;
            }
            else if (r < 25)
            {
                debrisType = DebrisType.Metal;
            }
            else
            {
                isCollectible = false;
                debrisType = DebrisType.Standard;
                size = _rand.Next(45, 80);
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
    }

    private void GenerateDebrisForLevel8(GameState state, int width, int height)
    {
        int spawnChance = 10;
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

            if (r < 6) 
            {
                debrisType = DebrisType.Diamond;
            }
            else if (r < 12)
            {
                debrisType = DebrisType.Gold;
            }
            else if (r < 28)
            {
                debrisType = DebrisType.Metal;
            }
            else
            {
                isCollectible = false;
                debrisType = DebrisType.Standard;
                size = _rand.Next(45, 80);
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
    }

    private void GenerateDebrisForLevel9(GameState state, int width, int height)
    {
        int spawnChance = 10;
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

            if (r < 40)
            {
                debrisType = DebrisType.Metal;
            }
            else if (r < 55)
            {
                debrisType = DebrisType.Gold;
            }
            else if (r < 60)
            {
                debrisType = DebrisType.Diamond;
            }
            else
            {
                isCollectible = false;
                debrisType = DebrisType.Standard;
                size = _rand.Next(45, 80);
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

        if (state.Debris.Count(d => d.IsStatic) == 0)
        {
            AddStaticHazards(state, width, height, count: _rand.Next(12, 16));
        }
    }

    private void GenerateDebrisForLevel10(GameState state, int width, int height)
    {
        int spawnChance = 10;
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

            if (r < 6)
            {
                debrisType = DebrisType.Diamond;
            }
            else if (r < 12)
            {
                debrisType = DebrisType.Gold;
            }
            else if (r < 28)
            {
                debrisType = DebrisType.Metal;
            }
            else
            {
                isCollectible = false;
                debrisType = DebrisType.Standard;
                size = _rand.Next(45, 80);
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

        if (state.Debris.Count(d => d.IsStatic) == 0)
        {
            AddStaticHazards(state, width, height, count: _rand.Next(12, 14));
        }
    }

    private void GenerateDebrisForLevel11(GameState state, int width, int height)
    {
        int spawnChance = 10;
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

            if (r < 6)
            {
                debrisType = DebrisType.Diamond;
            }
            else if (r < 12)
            {
                debrisType = DebrisType.Gold;
            }
            else if (r < 28)
            {
                debrisType = DebrisType.Metal;
            }
            else 
            {
                isCollectible = false;
                debrisType = DebrisType.Standard;
                size = _rand.Next(45, 80);
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
    }

    private void GenerateDebrisForLevel12(GameState state, int width, int height)
    {
        if (state.ElapsedTime < 200f)
        {
            float elapsedMinutes = state.ElapsedTime / 10f;
            int baseSpawnChance = (int)(8 * (1.0f + elapsedMinutes * 0.1f));

            if (_rand.Next(0, 100) < baseSpawnChance)
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

                if (r < 35) 
                {
                    type = DebrisType.Metal;
                }
                else if (r < 47) 
                {
                    type = DebrisType.Gold;
                }
                else if (r < 50) 
                {
                    type = DebrisType.Diamond;
                }
                else if (r < 52) 
                {
                    type = DebrisType.Fuel;
                }
                else if (r < 54) 
                {
                    type = DebrisType.Energy;
                }
                else 
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
        }
        else if (state.ElapsedTime < 300f) 
        {
            float elapsedMinutes = state.ElapsedTime / 60f;
            int baseSpawnChance = (int)(8 * (1.0f + elapsedMinutes * 0.1f));

            if (_rand.Next(0, 100) < baseSpawnChance)
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

                if (r < 35)
                {
                    type = DebrisType.Metal;
                }
                else if (r < 47) 
                {
                    type = DebrisType.Gold;
                }
                else if (r < 50) 
                {
                    type = DebrisType.Diamond;
                }
                else if (r < 53) 
                {
                    type = DebrisType.Fuel;
                }
                else if (r < 55) 
                {
                    type = DebrisType.Energy;
                }
                else 
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
        }

        else 
        {
            float elapsedMinutes = state.ElapsedTime / 60f;
            int baseSpawnChance = (int)(8 * (1.0f + elapsedMinutes * 0.1f));

            if (_rand.Next(0, 100) < baseSpawnChance)
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

                if (r < 35)
                {
                    type = DebrisType.Metal;
                }
                else if (r < 47)
                {
                    type = DebrisType.Gold;
                }
                else if (r < 50)
                {
                    type = DebrisType.Diamond;
                }
                else if (r < 53) 
                {
                    type = DebrisType.Fuel;
                }
                else if (r < 55) 
                {
                    type = DebrisType.Energy;
                }
                else 
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

            if (state.ElapsedTime - state.LastStaticSpawnTime >= 10f && state.ActiveStaticHazards.Count < 13)
            {
                var safeZone = new Rectangle(
                    state.Player.Position.X - 100,
                    state.Player.Position.Y - 100,
                    200,
                    200
                );

                int attempts = 0;
                bool placed = false;
                while (attempts < 10 && !placed) 
                {
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
                            var staticDebris = new Debris
                            {
                                Position = pos,
                                IsCollectible = false,
                                IsStatic = true,
                                Type = DebrisType.Standard,
                                Size = size
                            };

                            state.Debris.Add(staticDebris);
                            state.ActiveStaticHazards.Add(staticDebris); 
                            state.LastStaticSpawnTime = state.ElapsedTime; 
                            placed = true;
                        }
                    }
                    attempts++;
                }
            }
        }
    }

    private void GenerateDebrisForLevelDefault(GameState state, int width, int height)
    {
        GenerateDebrisForLevel1(state, width, height);
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

        state.Score = 0;
        state.CollectedMetal = 0;
        state.CollectedGold = 0;
        state.CollectedDiamond = 0;
        state.TotalResourcesCollected = 0;
        state.UsesResourceGoals = false; 
        state.RequiredMetal = 0;
        state.RequiredGold = 0;
        state.RequiredDiamond = 0;
        state.ElapsedTime = 0f;
        state.LastStaticSpawnTime = 0f;
        state.ActiveStaticHazards.Clear(); 

        switch (level)
        {
            case GameLevel.Tutorial:        
                state.TimeLeft = 90f;
                break;
            case GameLevel.ResourceHunt:    
                state.TimeLeft = 60f;
                break;
            case GameLevel.ResourceGoal1:   
                state.TimeLeft = 40f;
                state.UsesResourceGoals = true; 
                state.RequiredMetal = 5;        
                state.RequiredGold = 3;         
                break;
            case GameLevel.InvertedControls: 
                state.TimeLeft = 40f;
                break;
            case GameLevel.StaticHazards:  
                state.TimeLeft = 60f;
                break;
            case GameLevel.ResourceGoal2:   
                state.TimeLeft = 60f;
                state.UsesResourceGoals = true;
                state.RequiredMetal = 8;        
                state.RequiredGold = 4;       
                state.RequiredDiamond = 3;      
                break;
            case GameLevel.RichHunt:       
                state.TimeLeft = 40f;
                break;
            case GameLevel.DarkZone:        
                state.TimeLeft = 80f;
                break;
            case GameLevel.StaticInverted:  
                state.TimeLeft = 120f;
                break;
            case GameLevel.DarkStatic:     
                state.TimeLeft = 90f;
                break;
            case GameLevel.DarkInverted:    
                state.TimeLeft = 120f;
                break;
            case GameLevel.Survival:
                state.TimeLeft = 60f;
                state.UsesResourceGoals = false; 
                break;
            default:
                state.TimeLeft = 90f;
                break;
        }
    }

    public bool IsLevelComplete(GameState state)
    {
        if (state.CurrentLevel == GameLevel.Survival)
            return false; 

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
                GameLevel.RichHunt => 350,
                GameLevel.DarkZone => 250,
                GameLevel.StaticInverted => 150,
                GameLevel.DarkStatic => 200,
                GameLevel.DarkInverted => 150,
                _ => 100
            };
            return state.Score >= required;
        }
    }

    public float GetTimeForLevel(GameLevel level) => level switch
    {
        GameLevel.Tutorial => 90.0f,
        GameLevel.ResourceHunt => 60.0f,
        GameLevel.ResourceGoal1 => 40.0f,
        GameLevel.InvertedControls => 40.0f,
        GameLevel.StaticHazards => 60.0f,
        GameLevel.ResourceGoal2 => 60.0f,
        GameLevel.RichHunt => 40.0f,
        GameLevel.DarkZone => 80.0f,
        GameLevel.StaticInverted => 120.0f,
        GameLevel.DarkStatic => 90.0f,
        GameLevel.DarkInverted => 120.0f,
        GameLevel.Survival => float.PositiveInfinity, 
        _ => 90.0f
    };
}