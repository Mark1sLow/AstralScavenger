using AstralScavenger.Models.Logic;
using AstralScavenger.Models.States;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Forms;

namespace AstralScavenger.Views.Rendering;

public class Renderer
{
    private GameState _state;
    private int _width, _height;

    private Image _playerTexture;
    private Image _collectibleTexture;      // металл
    private Image _rareCollectibleTexture;  // золото
    private Image _diamondCollectibleTexture; // алмаз
    private Image _energyTexture;           // +1 жизнь
    private Image _fuelTexture;             // +20 сек
    private Image _dangerTexture;           // метеорит
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
            _currentBackgroundStyle = backgroundStyle; // Обновляем внутреннее состояние Renderer
            LoadBackgroundTexture(backgroundStyle);    // Перезагружаем текстуру
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
            DrawButton(g, rect, $"УРОВЕНЬ {i + 1}");
        }

        var backButton = new Rectangle(_width / 2 - buttonWidth / 2, startY + rows * (buttonHeight + spacing) + 20, buttonWidth, buttonHeight);
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
        bool isDarkLevel = _state.CurrentLevel == GameLevel.DarkZone ||
                   _state.CurrentLevel == GameLevel.DarkStatic ||
                   _state.CurrentLevel == GameLevel.DarkInverted ||
                   (_state.CurrentLevel == GameLevel.Survival && _state.TimeLeft <= 300);

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
                    continue; // Полностью невидим
                }

                // Плавное появление: чем ближе к центру, тем больше прозрачность
                float alpha = (float)(1.0 - distance / lightRadius); // 0..1
                alpha = Math.Max(0.1f, alpha); // Минимальная видимость

                if (texture != null)
                {
                    // Для текстуры используем ImageAttributes
                    using var attr = new ImageAttributes();
                    attr.SetColorMatrix(new ColorMatrix { Matrix00 = 1, Matrix11 = 1, Matrix22 = 1, Matrix33 = alpha, Matrix44 = 1 });
                    g.DrawImage(texture, new Rectangle(d.Position.X, d.Position.Y, d.Size, d.Size), 0, 0, texture.Width, texture.Height, GraphicsUnit.Pixel, attr);
                }
                else
                {
                    // Для эллипсов
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
                continue; // Переходим к следующему объекту
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

        var font = new Font("Arial", 16);
        g.DrawString($"УРОВЕНЬ: {(int)_state.CurrentLevel}", font, Brushes.White, 10, 10);
        g.DrawString($"ЖИЗНИ: {_state.Player.Health}", font, Brushes.White, 10, 40);

        if (_state.CurrentLevel != GameLevel.Survival)
        {
            g.DrawString($"ТОПЛИВО ЗАКОНЧИТСЯ ЧЕРЕЗ {(int)_state.TimeLeft} СЕКУНД", font, Brushes.White, 10, 70);
        }
        else
        {
            g.DrawString("РЕЖИМ ВЫЖИВАНИЯ", font, Brushes.Yellow, 10, 70);
        }

        if (_state.UsesResourceGoals)
        {
            g.DrawString("ОСТАЛОСЬ СОБРАТЬ:", font, Brushes.White, 10, 110);
            g.DrawString($"МЕТАЛЛ: {_state.CollectedMetal} / {_state.RequiredMetal}", font, Brushes.White, 10, 140);
            g.DrawString($"ЗОЛОТО: {_state.CollectedGold} / {_state.RequiredGold}", font, Brushes.White, 10, 170);
            if (_state.RequiredDiamond > 0)
                g.DrawString($"АЛМАЗ: {_state.CollectedDiamond} / {_state.RequiredDiamond}", font, Brushes.White, 10, 200);
        }
        else if (_state.CurrentLevel != GameLevel.Survival)
        {
            int required = _state.CurrentLevel switch
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
            g.DrawString($"ОСТАЛОСЬ СОБРАТЬ: {Math.Max(0, required - _state.Score)}", font, Brushes.White, 10, 110);
        }
    }

    private void RenderCustomizationMenu(Graphics g)
    {
        DrawBackground(g);
        var titleFont = new Font("Arial", 28, FontStyle.Bold);
        var titleSize = g.MeasureString("НАСТРОЙКИ", titleFont);
        g.DrawString("НАСТРОЙКИ", titleFont, Brushes.White, _width / 2 - titleSize.Width / 2, 60);

        var font = new Font("Arial", 18);
        var y = 150; // Начальная Y-координата
        var labelX = _width / 2 - 450; // Отступ слева для текста
        var leftButtonX = _width / 2 + 100; // Левая кнопка
        var rightButtonX = _width / 2 + 400; // Правая кнопка
        var btnSize = 40;

        // Сложность
        g.DrawString("СЛОЖНОСТЬ ИГРЫ:", font, Brushes.White, labelX, y);
        DrawArrowButton(g, new Rectangle(leftButtonX, y, btnSize, btnSize), true);
        DrawArrowButton(g, new Rectangle(rightButtonX, y, btnSize, btnSize), false);
        string diffStr = _state.Difficulty switch
        {
            GameDifficulty.Easy => "EASY",
            GameDifficulty.Normal => "NORMAL",
            GameDifficulty.Hard => "HARD",
            _ => "EXTREME"
        };
        g.DrawString(diffStr, font, Brushes.Yellow, _width / 2 + 150, 155);

        y += 60; // Увеличенный отступ

        // Тип корабля
        g.DrawString("ТИП КОРАБЛЯ:", font, Brushes.White, labelX, y);
        DrawArrowButton(g, new Rectangle(leftButtonX, y, btnSize, btnSize), true);
        DrawArrowButton(g, new Rectangle(rightButtonX, y, btnSize, btnSize), false);
        string shipStr = _state.SelectedShipType == ShipType.Cargo ? "ГРУЗОВОЙ" : "ТРАНСПОРТНЫЙ";
        g.DrawString(shipStr, font, Brushes.Yellow, _width / 2 + 150, 215);

        y += 60;

        // Цвет
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

        // Фон
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

        // Кнопка НАЗАД
        var backButtonWidth = 240;
        var backButtonHeight = 60;
        var backButtonY = _height - 100;
        var backButton = new Rectangle(_width / 2 - backButtonWidth / 2, backButtonY, backButtonWidth, backButtonHeight);
        DrawButton(g, backButton, "НАЗАД");
    }

    private void DrawArrowButton(Graphics g, Rectangle rect, bool isLeft)
    {
        using var brush = new SolidBrush(Color.FromArgb(128, 100, 100, 100)); // 50% прозрачность, светло-серый
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