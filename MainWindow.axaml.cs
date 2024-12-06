using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;

namespace SpaceWarProject
{
    public partial class MainWindow : Window
    {
        private Game game;
        private DispatcherTimer gameTimer;
        private bool isLeftPressed;
        private bool isRightPressed;
        private bool isUpPressed;
        private bool isDownPressed;
        private bool isSpacePressed;
        private DateTime lastShootTime = DateTime.MinValue;

        public MainWindow()
        {
            InitializeComponent();
            game = Game.Current;
            
            gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
            };
            gameTimer.Tick += GameTimer_Tick;
            
            this.KeyDown += MainWindow_KeyDown;
            this.KeyUp += MainWindow_KeyUp;
            
            StartGame();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            // Handle movement based on pressed keys
            if (isLeftPressed) game.MovePlayer("left", 0.016);
            if (isRightPressed) game.MovePlayer("right", 0.016);
            if (isUpPressed) game.MovePlayer("up", 0.016);
            if (isDownPressed) game.MovePlayer("down", 0.016);

            // Handle shooting
            if (isSpacePressed && (DateTime.Now - lastShootTime).TotalSeconds >= 0.25)
            {
                game.PlayerShoot();
                lastShootTime = DateTime.Now;
            }

            // Update game state
            game.Update(0.016);

            // Update canvas
            GameCanvas?.InvalidateVisual();

            if (game.IsGameOver)
            {
                gameTimer.Stop();
                GameOverScreen.IsVisible = true;
                FinalScoreText.Text = $"Final Score: {game.Score}";
                HighScoreText.Text = $"High Score: {game.HighScore}";
            }
        }

        private void RestartButton_Click(object? sender, RoutedEventArgs e)
        {
            // Önce timer'ı durdur
            gameTimer.Stop();
            
            // Tüm tuş durumlarını sıfırla
            isLeftPressed = false;
            isRightPressed = false;
            isUpPressed = false;
            isDownPressed = false;
            isSpacePressed = false;
            
            // Oyunu yeniden başlat
            StartGame();
        }

        private void StartGame()
        {
            // Game Over ekranını gizle
            GameOverScreen.IsVisible = false;
            
            // Oyunu sıfırla
            game.StartGame();
            
            // Timer'ı yeniden başlat
            gameTimer.Stop(); // Önce durduralım
            gameTimer.Start();
            
            // Son atış zamanını sıfırla
            lastShootTime = DateTime.MinValue;
            
            // Canvas'ı güncelle
            GameCanvas?.InvalidateVisual();
        }

        private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    isLeftPressed = true;
                    break;
                case Key.Right:
                    isRightPressed = true;
                    break;
                case Key.Up:
                    isUpPressed = true;
                    break;
                case Key.Down:
                    isDownPressed = true;
                    break;
                case Key.Space:
                    isSpacePressed = true;
                    break;
                case Key.R when game.IsGameOver:
                    RestartButton_Click(this, new RoutedEventArgs());
                    break;
            }
        }

        private void MainWindow_KeyUp(object? sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    isLeftPressed = false;
                    break;
                case Key.Right:
                    isRightPressed = false;
                    break;
                case Key.Up:
                    isUpPressed = false;
                    break;
                case Key.Down:
                    isDownPressed = false;
                    break;
                case Key.Space:
                    isSpacePressed = false;
                    break;
            }
        }
    }
}