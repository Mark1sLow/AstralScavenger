using AstralScavenger.Models.Logic;
using AstralScavenger.Models.States;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace AstralScavenger.Views.Rendering;

public class Renderer
{
    private GameState _state;
    private int _width, _height;

    private Image _playerTexture;
    private Image _collectibleTexture;     
    private Image _rareCollectibleTexture;  
    private Image _diamondCollectibleTexture; 
    private Image _energyTexture;          
    private Image _fuelTexture;            
    private Image _dangerTexture;          
    private Image _backgroundTexture;

    private BackgroundStyle _currentBackgroundStyle = BackgroundStyle.Default;

    public Renderer(int width, int height, GameState state)
    {
        _width = width;
        _height = height;
        _state = state;
        LoadTextures();
    }

    private Image LoadImage(string filename)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{GetType().Assembly.GetName().Name}.Resources.{filename}";
        using var stream = assembly.GetManifestResourceStream(resourceName);
        return stream == null ? null : Image.FromStream(stream);
    }

    private void LoadTextures()
    {
        _collectibleTexture = LoadImage("iron.png");
        _rareCollectibleTexture = LoadImage("gold.png");
        _diamondCollectibleTexture = LoadImage("diamond.png");
        _energyTexture = LoadImage("energy.png");
        _fuelTexture = LoadImage("fuel.png");
        _dangerTexture = LoadImage("meteor.png");
        LoadBackgroundTexture(_state.SelectedBackground);
    }

    private void LoadBackgroundTexture(BackgroundStyle style)
    {
        string filename = style switch
        {
            BackgroundStyle.Default => "background_0001.jpg",
            BackgroundStyle.DarkSpace => "background_0002.jpg",
            BackgroundStyle.MultiSpace => "background_0003.jpg",
            BackgroundStyle.PixelSpace => "background_0004.jpg",
            BackgroundStyle.Nebula => "background_0005.jpg",
            _ => "background_0001.jpg"
        };

        _backgroundTexture?.Dispose();
        _backgroundTexture = LoadImage(filename) ?? LoadImage("background_0001.jpg");
    }

    private void LoadPlayerTexture(PlayerColor color, ShipType type)
    {
        string shipPrefix = type == ShipType.Cargo ? "playerShip1" : "playerShip2";
        string colorName = color switch
        {
            PlayerColor.Blue => "blue",
            PlayerColor.Red => "red",
            PlayerColor.Green => "green",
            PlayerColor.Orange => "orange",
            _ => "blue"
        };
        string filename = $"{shipPrefix}_{colorName}.png";

        _playerTexture?.Dispose();
        _playerTexture = LoadImage(filename) ?? LoadImage("playerShip1_blue.png");
    }

    public void Render(Graphics g, PlayerColor selectedColor, ShipType shipType, BackgroundStyle backgroundStyle)
    {
        if (_currentBackgroundStyle != backgroundStyle)
        {
            _currentBackgroundStyle = backgroundStyle; 
            LoadBackgroundTexture(backgroundStyle);  
        }

        LoadPlayerTexture(selectedColor, shipType);

        switch (_state.CurrentScreen)
        {
            case GameScreen.Menu:
                RenderMenu(g);
                break;
            case GameScreen.LevelSelection:
                RenderLevelSelectionMenu(g);
                break;
            case GameScreen.Playing:
                RenderGame(g);
                break;
            case GameScreen.Pause:
                RenderGame(g);
                RenderPauseMenu(g);
                break;
            case GameScreen.LevelComplete:
                RenderGame(g);
                RenderLevelCompleteMenu(g);
                break;
            case GameScreen.GameOverScreen:
                RenderGame(g);
                RenderGameOverMenu(g);
                break;
            case GameScreen.Customization:
                RenderCustomizationMenu(g);
                break;
        }
    }

    private void RenderMenu(Graphics g) 
    {
        DrawBackground(g);
        var titleFont = new Font("Arial", 48, FontStyle.Bold);
        var titleSize = g.MeasureString("ASTRAL SCAVENGER", titleFont);
        g.DrawString("ASTRAL SCAVENGER", titleFont, Brushes.White, _width / 2 - titleSize.Width / 2, 150);

        var buttonWidth = 240;
        var buttonHeight = 60;
        var spacing = 20;
        var totalHeight = 3 * buttonHeight + 2 * spacing;
        var startY = _height / 2 - totalHeight / 2;

        DrawButton(g, new Rectangle(_width / 2 - buttonWidth / 2, startY, buttonWidth, buttonHeight), "НАЧАТЬ ИГРУ");
        DrawButton(g, new Rectangle(_width / 2 - buttonWidth / 2, startY + buttonHeight + spacing, buttonWidth, buttonHeight), "НАСТРОЙКИ");
        DrawButton(g, new Rectangle(_width / 2 - buttonWidth / 2, startY + 2 * (buttonHeight + spacing), buttonWidth, buttonHeight), "ВЫЙТИ");
    }

    private void RenderLevelSelectionMenu(Graphics g)
    {
        DrawBackground(g);
        var titleFont = new Font("Arial", 28, FontStyle.Bold);
        var titleSize = g.MeasureString("ВЫБЕРИТЕ УРОВЕНЬ", titleFont);
        g.DrawString("ВЫБЕРИТЕ УРОВЕНЬ", titleFont, Brushes.White, _width / 2 - titleSize.Width / 2, 60);

        var buttonWidth = 240;
        var buttonHeight = 60;
        var spacing = 20;
        var cols = 3;
        int totalLevels = 15; 

        int rows = (int)Math.Ceiling((double)totalLevels / cols); 

        var startX = _width / 2 - (cols * buttonWidth + (cols - 1) * spacing) / 2;
        var startY = 150;

        for (int i = 0; i < totalLevels; i++) 
        {
            int col = i % cols; 
            int row = i / cols; 
            var rect = new Rectangle(
                startX + col * (buttonWidth + spacing),
                startY + row * (buttonHeight + spacing),
                buttonWidth,
                buttonHeight
            );
            DrawButton(g, rect, $"УРОВЕНЬ {i + 1}");
        }

        var backButtonY = startY + rows * (buttonHeight + spacing) + 20;
        var backButton = new Rectangle(_width / 2 - 240 / 2, backButtonY, 240, 60);
        DrawButton(g, backButton, "НАЗАД");
    }

    private void RenderPauseMenu(Graphics g)
    {
        RenderGame(g);
        using var overlay = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
        g.FillRectangle(overlay, 0, 0, _width, _height);

        var titleFont = new Font("Arial", 36, FontStyle.Bold);
        var titleSize = g.MeasureString("ПАУЗА", titleFont);
        g.DrawString("ПАУЗА", titleFont, Brushes.White, _width / 2 - titleSize.Width / 2, _height / 2 - 200);

        var buttonWidth = 240;
        var buttonHeight = 60;
        var spacing = 20;
        var totalHeight = 3 * buttonHeight + 2 * spacing;
        var startY = _height / 2 - totalHeight / 2;

        DrawButton(g, new Rectangle(_width / 2 - buttonWidth / 2, startY, buttonWidth, buttonHeight), "ПРОДОЛЖИТЬ");
        DrawButton(g, new Rectangle(_width / 2 - buttonWidth / 2, startY + buttonHeight + spacing, buttonWidth, buttonHeight), "ГЛАВНОЕ МЕНЮ");
        DrawButton(g, new Rectangle(_width / 2 - buttonWidth / 2, startY + 2 * (buttonHeight + spacing), buttonWidth, buttonHeight), "ПЕРЕЗАПУСТИТЬ");
    }

    private void RenderLevelCompleteMenu(Graphics g)
    {
        RenderGame(g);
        using var overlay = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
        g.FillRectangle(overlay, 0, 0, _width, _height);

        var titleFont = new Font("Arial", 36, FontStyle.Bold);
        var titleSize = g.MeasureString("УРОВЕНЬ ПРОЙДЕН!", titleFont);
        g.DrawString("УРОВЕНЬ ПРОЙДЕН!", titleFont, Brushes.Yellow, _width / 2 - titleSize.Width / 2, _height / 2 - 200);

        var buttonWidth = 240;
        var buttonHeight = 60;
        var spacing = 20;
        var totalHeight = 3 * buttonHeight + 2 * spacing;
        var startY = _height / 2 - totalHeight / 2;

        DrawButton(g, new Rectangle(_width / 2 - buttonWidth / 2, startY, buttonWidth, buttonHeight), "ГЛАВНОЕ МЕНЮ");
        DrawButton(g, new Rectangle(_width / 2 - buttonWidth / 2, startY + buttonHeight + spacing, buttonWidth, buttonHeight), "ПЕРЕЗАПУСТИТЬ");
        DrawButton(g, new Rectangle(_width / 2 - buttonWidth / 2, startY + 2 * (buttonHeight + spacing), buttonWidth, buttonHeight), "СЛЕДУЮЩИЙ УРОВЕНЬ");
    }

    private void RenderGameOverMenu(Graphics g)
    {
        RenderGame(g);
        using var overlay = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
        g.FillRectangle(overlay, 0, 0, _width, _height);

        string text = _state.TimeLeft <= 0 ? "ТОПЛИВО ЗАКОНЧИЛОСЬ!" : "УРОВЕНЬ НЕ ПРОЙДЕН!";
        var brush = Brushes.Red;
        var titleFont = new Font("Arial", 36, FontStyle.Bold);
        var titleSize = g.MeasureString(text, titleFont);
        g.DrawString(text, titleFont, brush, _width / 2 - titleSize.Width / 2, _height / 2 - 200);

        var buttonWidth = 240;
        var buttonHeight = 60;
        var spacing = 20;
        var totalHeight = 3 * buttonHeight + 2 * spacing;
        var startY = _height / 2 - totalHeight / 2;

        DrawButton(g, new Rectangle(_width / 2 - buttonWidth / 2, startY, buttonWidth, buttonHeight), "ГЛАВНОЕ МЕНЮ");
        DrawButton(g, new Rectangle(_width / 2 - buttonWidth / 2, startY + buttonHeight + spacing, buttonWidth, buttonHeight), "НАСТРОЙКИ");
        DrawButton(g, new Rectangle(_width / 2 - buttonWidth / 2, startY + 2 * (buttonHeight + spacing), buttonWidth, buttonHeight), "ПЕРЕЗАПУСТИТЬ");
    }

    private void RenderGame(Graphics g)
    {
        bool isDarkLevel = false;

        if (_state.CurrentLevel == GameLevel.Survival)
        {
            if (_state.ElapsedTime >= 200f) 
            {
                isDarkLevel = true;
            }
        }
        else if (_state.CurrentLevel is GameLevel.DarkZone or GameLevel.DarkStatic or GameLevel.DarkInverted or GameLevel.RichHuntPlusDark)
        {
            isDarkLevel = true;
        }

        if (isDarkLevel)
        {
            g.Clear(Color.Black);
        }
        else
        {
            DrawBackground(g);
        }

        if (isDarkLevel)
        {
            var lightRadius = (int)(_state.Player.Size * 4 * 1.25f);
            var lightX = _state.Player.Position.X + _state.Player.Size / 2;
            var lightY = _state.Player.Position.Y + _state.Player.Size / 2;

            using var path = new GraphicsPath();
            path.AddEllipse(lightX - lightRadius, lightY - lightRadius, lightRadius * 2, lightRadius * 2);
            using var brush = new PathGradientBrush(path);
            brush.CenterColor = Color.FromArgb(50, 255, 255, 255);
            brush.SurroundColors = new[] { Color.FromArgb(0, 255, 255, 255) };
            g.FillPath(brush, path);
        }

        if (_playerTexture != null)
        {
            var p = _state.Player;
            var rect = new Rectangle(p.Position.X, p.Position.Y, p.Size, p.Size);
            var oldTransform = g.Transform;
            g.TranslateTransform(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            g.RotateTransform((float)(p.CurrentRotation * 180 / Math.PI));
            g.TranslateTransform(-rect.Width / 2, -rect.Height / 2);
            g.DrawImage(_playerTexture, 0, 0, rect.Width, rect.Height);
            g.Transform = oldTransform;
        }
        else
        {
            g.FillEllipse(Brushes.White, _state.Player.Position.X, _state.Player.Position.Y, _state.Player.Size, _state.Player.Size);
        }

        foreach (var d in _state.Debris)
        {
            if (!d.IsActive) continue;

            Image texture = d.IsCollectible ? (d.Type switch
            {
                DebrisType.Gold => _rareCollectibleTexture,
                DebrisType.Diamond => _diamondCollectibleTexture,
                DebrisType.Energy => _energyTexture,
                DebrisType.Fuel => _fuelTexture,
                _ => _collectibleTexture
            }) : _dangerTexture;

            if (isDarkLevel)
            {
                var lightRadius = (int)(_state.Player.Size * 4 * 1.25f);
                var dx = d.Position.X + d.Size / 2 - (_state.Player.Position.X + _state.Player.Size / 2);
                var dy = d.Position.Y + d.Size / 2 - (_state.Player.Position.Y + _state.Player.Size / 2);
                var distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance > lightRadius)
                {
                    continue;
                }

                float alpha = (float)(1.0 - distance / lightRadius);
                alpha = Math.Max(0.1f, alpha);

                if (texture != null)
                {
                    using var attr = new ImageAttributes();
                    attr.SetColorMatrix(new ColorMatrix { Matrix00 = 1, Matrix11 = 1, Matrix22 = 1, Matrix33 = alpha, Matrix44 = 1 });
                    g.DrawImage(texture, new Rectangle(d.Position.X, d.Position.Y, d.Size, d.Size), 0, 0, texture.Width, texture.Height, GraphicsUnit.Pixel, attr);
                }
                else
                {
                    Brush b = d.IsCollectible ? (d.Type switch
                    {
                        DebrisType.Gold => Brushes.Gold,
                        DebrisType.Diamond => Brushes.Cyan,
                        DebrisType.Energy => Brushes.Green,
                        DebrisType.Fuel => Brushes.Orange,
                        _ => Brushes.Gray
                    }) : Brushes.Red;

                    using var translucentBrush = new SolidBrush(Color.FromArgb((int)(alpha * 255), ((SolidBrush)b).Color));
                    g.FillEllipse(translucentBrush, d.Position.X, d.Position.Y, d.Size, d.Size);
                }
                continue;
            }

            if (texture != null)
            {
                g.DrawImage(texture, d.Position.X, d.Position.Y, d.Size, d.Size);
            }
            else
            {
                Brush b = d.IsCollectible ? (d.Type switch
                {
                    DebrisType.Gold => Brushes.Gold,
                    DebrisType.Diamond => Brushes.Cyan,
                    DebrisType.Energy => Brushes.Green,
                    DebrisType.Fuel => Brushes.Orange,
                    _ => Brushes.Gray
                }) : Brushes.Red;
                g.FillEllipse(b, d.Position.X, d.Position.Y, d.Size, d.Size);
            }
        }

        var font = new Font("Arial", 14); 

        var barWidth = 300;   
        var barHeight = 15;   
        var startX = 15;    

        int maxHealth = _state.GetDifficultyForLevel(_state.CurrentLevel) switch
        {
            GameDifficulty.Easy => 10,
            GameDifficulty.Normal => 3,
            GameDifficulty.Hard => 2,
            GameDifficulty.Extreme => 1,
            _ => 3
        };

        float healthPercent = (float)_state.Player.Health / maxHealth;
        var healthBarRect = new Rectangle(startX, 50, barWidth, barHeight);

        DrawProgressBar(g, healthBarRect, healthPercent, Color.LightSeaGreen, "ПРОЧНОСТЬ КОРАБЛЯ", font, true);

        float timePercent = 0f;
        float maxTimeForDisplay = 60f;

        if (_state.CurrentLevel == GameLevel.Survival)
        {
            timePercent = Math.Min(1f, _state.TimeLeft / maxTimeForDisplay);
        }
        else
        {
            float maxTime = new LevelLogic().GetTimeForLevel(_state.CurrentLevel);
            timePercent = maxTime == float.PositiveInfinity ? 1f : Math.Max(0f, _state.TimeLeft / maxTime);
        }

        var timeBarRect = new Rectangle(startX, 100, barWidth, barHeight);
        DrawProgressBar(g, timeBarRect, timePercent, Color.Red, "ТОПЛИВО", font, true);

        if (_state.CurrentLevel == GameLevel.Survival)
        {
            string resourcesText = $"РЕСУРСЫ: {_state.ResourceScore}"; 
            g.DrawString(resourcesText, font, Brushes.White, startX, 150);
        }

        else if (_state.UsesResourceGoals)
        {
            float metalPercent = _state.RequiredMetal == 0 ? 1f : Math.Min(1f, (float)_state.CollectedMetal / _state.RequiredMetal);
            var metalBarRect = new Rectangle(startX, 150, barWidth, barHeight);
            DrawProgressBar(g, metalBarRect, metalPercent, Color.LightGray, "МЕТАЛЛ", font, true);

            float goldPercent = _state.RequiredGold == 0 ? 1f : Math.Min(1f, (float)_state.CollectedGold / _state.RequiredGold);
            var goldBarRect = new Rectangle(startX, 200, barWidth, barHeight);
            DrawProgressBar(g, goldBarRect, goldPercent, Color.Gold, "ЗОЛОТО", font, true);

            if (_state.RequiredDiamond > 0)
            {
                float diamondPercent = _state.RequiredDiamond == 0 ? 1f : Math.Min(1f, (float)_state.CollectedDiamond / _state.RequiredDiamond);
                var diamondBarRect = new Rectangle(startX, 250, barWidth, barHeight);
                DrawProgressBar(g, diamondBarRect, diamondPercent, Color.Cyan, "АЛМАЗЫ", font, true);
            }
        }
        else
        {
            float scorePercent = 0f;
            int required = _state.CurrentLevel switch
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
                GameLevel.RichHuntPlus => 800,
                GameLevel.RichHuntPlusDark => 800,
                GameLevel.RichHuntPlusChaos => 1500,
                _ => 100
            };

            if (required > 0)
            {
                scorePercent = Math.Min(1f, (float)_state.Score / required);
            }

            var scoreBarRect = new Rectangle(startX, 150, barWidth, barHeight);
            DrawProgressBar(g, scoreBarRect, scorePercent, Color.Purple, "ОСТАЛОСЬ СОБРАТЬ", font, true);
        }
    }

    private void DrawProgressBar(Graphics g, Rectangle rect, float percent, Color barColor, string text, Font font, bool textAbove)
    {
        using var bgBrush = new SolidBrush(Color.FromArgb(100, 50, 50, 50));
        g.FillRectangle(bgBrush, rect);
        g.DrawRectangle(Pens.White, rect);

        int fillWidth = (int)(rect.Width * percent);
        if (fillWidth > 0)
        {
            using var fillBrush = new SolidBrush(barColor);
            g.FillRectangle(fillBrush, rect.X, rect.Y, fillWidth, rect.Height);
        }

        var textSize = g.MeasureString(text, font);
        int textX = rect.X + (rect.Width - (int)textSize.Width) / 2;
        int textY = textAbove ? rect.Y - (int)textSize.Height - 5 : rect.Y - (int)textSize.Height - 5;

        using var textBrush = new SolidBrush(Color.White);
        g.DrawString(text, font, textBrush, textX, textY);
    }

    private void RenderCustomizationMenu(Graphics g)
    {
        DrawBackground(g);
        var titleFont = new Font("Arial", 28, FontStyle.Bold);
        var titleSize = g.MeasureString("НАСТРОЙКИ", titleFont);
        g.DrawString("НАСТРОЙКИ", titleFont, Brushes.White, _width / 2 - titleSize.Width / 2, 60);

        var font = new Font("Arial", 18);
        var y = 150; 
        var labelX = _width / 2 - 450; 
        var leftButtonX = _width / 2 + 100; 
        var rightButtonX = _width / 2 + 400; 
        var btnSize = 40;

        g.DrawString("СЛОЖНОСТЬ ИГРЫ:", font, Brushes.White, labelX, y);
        DrawArrowButton(g, new Rectangle(leftButtonX, y, btnSize, btnSize), true);
        DrawArrowButton(g, new Rectangle(rightButtonX, y, btnSize, btnSize), false);
        string diffStr = _state.GetDifficultyForLevel(_state.CurrentLevel) switch
        {
            GameDifficulty.Easy => "EASY",
            GameDifficulty.Normal => "NORMAL",
            GameDifficulty.Hard => "HARD",  
            GameDifficulty.Extreme => "EXTREME",  
            _ => "NORMAL"
        };

        g.DrawString(diffStr, font, Brushes.Yellow, _width / 2 + 150, 155);

        y += 60;

        g.DrawString("ТИП КОРАБЛЯ:", font, Brushes.White, labelX, y);
        DrawArrowButton(g, new Rectangle(leftButtonX, y, btnSize, btnSize), true);
        DrawArrowButton(g, new Rectangle(rightButtonX, y, btnSize, btnSize), false);
        string shipStr = _state.SelectedShipType == ShipType.Cargo ? "ГРУЗОВОЙ" : "ТРАНСПОРТНЫЙ";
        g.DrawString(shipStr, font, Brushes.Yellow, _width / 2 + 150, 215);

        y += 60;

        g.DrawString("ЦВЕТ КОРАБЛЯ:", font, Brushes.White, labelX, y);
        DrawArrowButton(g, new Rectangle(leftButtonX, y, btnSize, btnSize), true);
        DrawArrowButton(g, new Rectangle(rightButtonX, y, btnSize, btnSize), false);
        string colorStr = _state.SelectedColor switch
        {
            PlayerColor.Blue => "СИНИЙ",
            PlayerColor.Red => "КРАСНЫЙ",
            PlayerColor.Green => "ЗЕЛЕНЫЙ",
            _ => "ОРАНЖЕВЫЙ"
        };
        g.DrawString(colorStr, font, Brushes.Yellow, _width / 2 + 150, 275);

        y += 60;

        g.DrawString("ФОН ИГРЫ:", font, Brushes.White, labelX, y);
        DrawArrowButton(g, new Rectangle(leftButtonX, y, btnSize, btnSize), true);
        DrawArrowButton(g, new Rectangle(rightButtonX, y, btnSize, btnSize), false);
        string bgStr = _state.SelectedBackground switch
        {
            BackgroundStyle.Default => "ПО УМОЛЧАНИЮ",
            BackgroundStyle.DarkSpace => "ТЕМНЫЙ КОСМОС",
            BackgroundStyle.MultiSpace => "МУЛЬТИ КОСМОС",
            BackgroundStyle.PixelSpace => "ПИКСЕЛЬ КОСМОС",
            _ => "ТУМАННОСТЬ"
        };
        g.DrawString(bgStr, font, Brushes.Yellow, _width / 2 + 150, 335);

        var backButtonWidth = 240;
        var backButtonHeight = 60;
        var backButtonY = _height - 100;
        var backButton = new Rectangle(_width / 2 - backButtonWidth / 2, backButtonY, backButtonWidth, backButtonHeight);
        DrawButton(g, backButton, "НАЗАД");
    }

    private void DrawArrowButton(Graphics g, Rectangle rect, bool isLeft)
    {
        using var brush = new SolidBrush(Color.FromArgb(128, 100, 100, 100)); 
        g.FillRectangle(brush, rect);
        using var pen = new Pen(Color.White, 2);
        g.DrawRectangle(pen, rect);

        Point[] points = isLeft
            ? new Point[]
            {
                new Point(rect.X + 10, rect.Y + rect.Height / 2),
                new Point(rect.X + rect.Width - 10, rect.Y + 10),
                new Point(rect.X + rect.Width - 10, rect.Y + rect.Height - 10)
            }
            : new Point[]
            {
                new Point(rect.X + rect.Width - 10, rect.Y + rect.Height / 2),
                new Point(rect.X + 10, rect.Y + 10),
                new Point(rect.X + 10, rect.Y + rect.Height - 10)
            };
        g.FillPolygon(Brushes.White, points);
    }

    private void DrawButton(Graphics g, Rectangle rect, string text)
    {
        using var brush = new SolidBrush(Color.FromArgb(128, 100, 100, 100));
        g.FillRectangle(brush, rect);
        using var pen = new Pen(Color.White, 2);
        g.DrawRectangle(pen, rect);
        var textSize = g.MeasureString(text, SystemFonts.DefaultFont);
        g.DrawString(text, SystemFonts.DefaultFont, Brushes.White,
                     rect.X + (rect.Width - textSize.Width) / 2,
                     rect.Y + (rect.Height - textSize.Height) / 2);
    }

    private void DrawBackground(Graphics g)
    {
        if (_backgroundTexture != null)
        {
            g.DrawImage(_backgroundTexture, 0, 0, _width, _height);
        }
        else
        {
            g.Clear(Color.Black);
        }
    }
}