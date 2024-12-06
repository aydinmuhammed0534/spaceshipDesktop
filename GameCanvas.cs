using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using System;
using System.Globalization;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;

namespace SpaceWarProject
{
    public class GameCanvas : Control
    {
        private Game game;

        public static readonly StyledProperty<IBrush> BackgroundProperty =
            AvaloniaProperty.Register<GameCanvas, IBrush>(nameof(Background));

        public IBrush Background
        {
            get => GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public GameCanvas()
        {
            game = Game.Current;
            ClipToBounds = true;
            Background = Brushes.Black;
        }

        public override void Render(DrawingContext context)
        {
            if (game == null) return;

            // Draw background
            context.FillRectangle(Background, new Rect(0, 0, Bounds.Width, Bounds.Height));

            try
            {
                // Draw player
                if (game.PlayerShip != null)
                {
                    context.FillRectangle(Brushes.Green, new Rect(game.PlayerX, game.PlayerY, game.PlayerShip.Width, game.PlayerShip.Height));
                }

                // Draw player bullets
                if (game.PlayerBullets != null)
                {
                    foreach (var bullet in game.PlayerBullets)
                    {
                        context.FillRectangle(Brushes.Yellow, new Rect(bullet.spawnX, bullet.spawnY, bullet.Width, bullet.Height));
                    }
                }

                // Draw enemies and their bullets
                if (game.Enemies != null)
                {
                    foreach (var enemy in game.Enemies)
                    {
                        // Düşman tipine göre farklı renkler ve şekiller
                        var enemyBrush = enemy.EnemyType switch
                        {
                            "Basic" => Brushes.Red,
                            "Fast" => Brushes.Yellow,
                            "Strong" => Brushes.Purple,
                            "Boss" => Brushes.DarkRed,
                            _ => Brushes.Gray
                        };

                        // Düşman çizimi
                        var enemyRect = new Rect(enemy.spawnX, enemy.spawnY, enemy.Width, enemy.Height);
                        
                        // Düşman tipine göre özel efektler
                        switch (enemy.EnemyType)
                        {
                            case "Basic":
                                // Temel düşman - basit dikdörtgen
                                context.FillRectangle(enemyBrush, enemyRect);
                                break;

                            case "Fast":
                                // Hızlı düşman - üçgen şekli
                                var geometry = new StreamGeometry();
                                using (var context2 = geometry.Open())
                                {
                                    context2.BeginFigure(
                                        new Point(enemy.spawnX + enemy.Width, enemy.spawnY + enemy.Height/2),
                                        true);
                                    context2.LineTo(new Point(enemy.spawnX, enemy.spawnY));
                                    context2.LineTo(new Point(enemy.spawnX, enemy.spawnY + enemy.Height));
                                    context2.EndFigure(true);
                                }
                                context.DrawGeometry(enemyBrush, new Pen(enemyBrush, 2), geometry);
                                break;

                            case "Strong":
                                // Güçlü düşman - kalın kenarlıklı dikdörtgen
                                context.DrawRectangle(new Pen(Brushes.White, 3), enemyRect);
                                context.FillRectangle(enemyBrush, enemyRect);
                                break;

                            case "Boss":
                                // Boss - kompleks şekil
                                context.DrawRectangle(new Pen(Brushes.Orange, 4), enemyRect);
                                context.FillRectangle(enemyBrush, enemyRect);
                                // İç detaylar
                                var innerRect = new Rect(
                                    enemy.spawnX + 10, 
                                    enemy.spawnY + 10, 
                                    enemy.Width - 20, 
                                    enemy.Height - 20
                                );
                                context.DrawRectangle(new Pen(Brushes.Yellow, 2), innerRect);
                                break;
                        }

                        // Can barı
                        var healthBarWidth = enemy.Width;
                        var healthBarHeight = 5;
                        var healthPercentage = enemy.Health / (enemy.EnemyType == "Boss" ? 200.0 : 
                                                             enemy.EnemyType == "Strong" ? 120.0 : 
                                                             enemy.EnemyType == "Fast" ? 30.0 : 50.0);
                        
                        var healthBarRect = new Rect(
                            enemy.spawnX,
                            enemy.spawnY - healthBarHeight - 2,
                            healthBarWidth * healthPercentage,
                            healthBarHeight
                        );
                        context.FillRectangle(Brushes.LightGreen, healthBarRect);

                        // Düşman mermileri
                        foreach (var bullet in enemy.Bullets)
                        {
                            var bulletBrush = enemy.EnemyType == "Boss" ? Brushes.Orange :
                                            enemy.EnemyType == "Strong" ? Brushes.Purple :
                                            enemy.EnemyType == "Fast" ? Brushes.Yellow :
                                            Brushes.Red;
                            context.FillRectangle(bulletBrush, new Rect(bullet.spawnX, bullet.spawnY, bullet.Width, bullet.Height));
                        }
                    }
                }

                // Draw score and health
                var scoreText = new FormattedText(
                    $"Score: {game.Score}",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyle.Normal, FontWeight.Bold),
                    24,
                    Brushes.White);
                context.DrawText(scoreText, new Point(20, 20));

                var highScoreText = new FormattedText(
                    $"High Score: {game.HighScore}",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyle.Normal, FontWeight.Bold),
                    24,
                    Brushes.Yellow);
                context.DrawText(highScoreText, new Point(20, 50));

                var healthText = new FormattedText(
                    $"Health: {game.PlayerShip?.Health ?? 0}",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyle.Normal, FontWeight.Bold),
                    24,
                    Brushes.LightGreen);
                context.DrawText(healthText, new Point(Bounds.Width - healthText.Width - 20, 20));

                // Draw game over text if applicable
                if (game.IsGameOver)
                {
                    var gameOverText = new FormattedText(
                        "Game Over! Press R to restart",
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(new FontFamily("Arial"), FontStyle.Normal, FontWeight.Normal),
                        30,
                        Brushes.Red);
                    context.DrawText(gameOverText, new Point(
                        (Bounds.Width - gameOverText.Width) / 2,
                        (Bounds.Height - gameOverText.Height) / 2));
                }
            }
            catch (Exception ex)
            {
                // Draw error message if something goes wrong
                var errorText = new FormattedText(
                    $"Error: {ex.Message}",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyle.Normal, FontWeight.Normal),
                    16,
                    Brushes.Red);
                context.DrawText(errorText, new Point(10, Bounds.Height - 30));
            }
        }

        protected void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);
            InvalidateVisual();
        }
    }
}
