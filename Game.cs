using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SpaceWarProject
{
    public class Game
    {
        private static Game _current = new Game();
        public static Game Current => _current;

        private Spaceship player = new Spaceship(100, 300);
        public Spaceship PlayerShip => player;
        public double PlayerX => player.spawnX;
        public double PlayerY => player.spawnY;
        
        private List<Enemy> enemies = new List<Enemy>();
        private bool isGameOver = false;
        private int score = 0;
        private double spawnTimer = 0;
        private Random random = new Random();
        private DateTime gameStartTime;
        private DateTime lastUpdateTime;
        private const double SPAWN_INTERVAL = 3.0;
        private const int MAX_ENEMIES = 7;
        private const string SCORES_FILE = "scores.txt";
        private int highScore = 0;

        public bool IsGameOver => isGameOver;
        public int Score => score;
        public IReadOnlyList<Enemy> Enemies => enemies;
        public IEnumerable<Bullet> PlayerBullets => player.Bullets;
        public int HighScore => highScore;

        public Game()
        {
            Reset();
        }

        public void StartGame()
        {
            // Oyunu tamamen sıfırla
            Reset();
            
            // Zamanı sıfırla
            gameStartTime = DateTime.Now;
            lastUpdateTime = DateTime.Now;
            spawnTimer = SPAWN_INTERVAL;
            
            // Oyuncuyu başlangıç pozisyonuna getir
            player = new Spaceship(100, 300);
        }

        private void Reset()
        {
            // Yüksek skoru yükle ve mevcut skoru kaydet
            LoadHighScore();
            if (isGameOver)
            {
                SaveScore();
            }
            
            // Oyun durumunu sıfırla
            isGameOver = false;
            score = 0;
            
            // Tüm düşmanları ve mermileri temizle
            enemies.Clear();
            if (player != null)
            {
                player.Bullets.Clear();
            }
            
            // Zamanlamayı sıfırla
            spawnTimer = SPAWN_INTERVAL;
            gameStartTime = DateTime.Now;
            lastUpdateTime = DateTime.Now;
        }

        public void Update(double deltaTime)
        {
            if (isGameOver) return;

            // Düşman oluşturma zamanını güncelle
            spawnTimer -= deltaTime;
            if (spawnTimer <= 0 && enemies.Count < MAX_ENEMIES)
            {
                SpawnEnemy();
                spawnTimer = SPAWN_INTERVAL;
            }

            // Düşmanları güncelle
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var enemy = enemies[i];
                enemy.Update(deltaTime);

                // Ekrandan çıkan düşmanları kaldır
                if (enemy.spawnX < -enemy.Width)
                {
                    enemies.RemoveAt(i);
                    continue;
                }

                // Düşman mermilerini kontrol et
                foreach (var bullet in enemy.Bullets.ToList())
                {
                    if (CheckCollision(bullet, player))
                    {
                        player.Health -= 10;
                        enemy.Bullets.Remove(bullet);
                        if (player.Health <= 0)
                        {
                            isGameOver = true;
                            SaveScore(); // Oyun bittiğinde skoru kaydet
                            return;
                        }
                    }
                }

                // Düşman gemisiyle çarpışma kontrolü
                if (CheckCollision(enemy, player))
                {
                    // Düşman tipine göre hasar ver
                    int collisionDamage = enemy.EnemyType switch
                    {
                        "Boss" => 40,
                        "Strong" => 25,
                        "Fast" => 15,
                        _ => 20
                    };
                    
                    player.Health -= collisionDamage;
                    enemies.RemoveAt(i); // Çarpışan düşmanı yok et
                    
                    if (player.Health <= 0)
                    {
                        isGameOver = true;
                        SaveScore(); // Oyun bittiğinde skoru kaydet
                        return;
                    }
                }
            }

            // Oyuncu mermilerini kontrol et
            foreach (var bullet in player.Bullets.ToList())
            {
                foreach (var enemy in enemies.ToList())
                {
                    if (CheckCollision(bullet, enemy))
                    {
                        enemy.Health -= 20;
                        player.Bullets.Remove(bullet);
                        if (enemy.Health <= 0)
                        {
                            enemies.Remove(enemy);
                            score += enemy.ScoreValue;
                        }
                        break;
                    }
                }
            }

            // Mermileri güncelle
            UpdateBullets();
        }

        private void UpdateBullets()
        {
            // Oyuncu mermilerini güncelle
            for (int i = player.Bullets.Count - 1; i >= 0; i--)
            {
                var bullet = player.Bullets[i];
                bullet.Move();
                if (bullet.spawnX > 800 || bullet.spawnX < 0 ||
                    bullet.spawnY > 600 || bullet.spawnY < 0)
                {
                    player.Bullets.RemoveAt(i);
                }
            }

            // Düşman mermilerini güncelle
            foreach (var enemy in enemies)
            {
                for (int i = enemy.Bullets.Count - 1; i >= 0; i--)
                {
                    var bullet = enemy.Bullets[i];
                    bullet.Move();
                    if (bullet.spawnX > 800 || bullet.spawnX < 0 ||
                        bullet.spawnY > 600 || bullet.spawnY < 0)
                    {
                        enemy.Bullets.RemoveAt(i);
                    }
                }
            }
        }

        private void SpawnEnemy()
        {
            double y = random.Next(50, 550);
            Enemy enemy;

            // Rastgele düşman tipi seç
            double randomValue = random.NextDouble();
            if (randomValue < 0.25)
            {
                enemy = new BossEnemy(800, y);
            }
            else if (randomValue < 0.5)
            {
                enemy = new StrongEnemy(800, y);
            }
            else if (randomValue < 0.75)
            {
                enemy = new FastEnemy(800, y);
            }
            else
            {
                enemy = new BasicEnemy(800, y);
            }

            enemies.Add(enemy);
        }

        private bool CheckCollision(GameObject obj1, GameObject obj2)
        {
            return obj1.spawnX < obj2.spawnX + obj2.Width &&
                   obj1.spawnX + obj1.Width > obj2.spawnX &&
                   obj1.spawnY < obj2.spawnY + obj2.Height &&
                   obj1.spawnY + obj1.Height > obj2.spawnY;
        }

        public void MovePlayer(string direction, double deltaTime)
        {
            if (isGameOver) return;

            double deltaX = 0;
            double deltaY = 0;
            const double MOVE_SPEED = 300;

            switch (direction)
            {
                case "up":
                    deltaY = -MOVE_SPEED * deltaTime;
                    break;
                case "down":
                    deltaY = MOVE_SPEED * deltaTime;
                    break;
                case "left":
                    deltaX = -MOVE_SPEED * deltaTime;
                    break;
                case "right":
                    deltaX = MOVE_SPEED * deltaTime;
                    break;
            }

            player.Move(deltaX, deltaY);
        }

        public void PlayerShoot()
        {
            if (!isGameOver)
            {
                player.Shoot();
            }
        }

        private void SaveScore()
        {
            try
            {
                // Mevcut yüksek skoru kontrol et
                if (score > highScore)
                {
                    highScore = score;
                    File.WriteAllText(SCORES_FILE, highScore.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving score: {ex.Message}");
            }
        }

        private void LoadHighScore()
        {
            try
            {
                if (File.Exists(SCORES_FILE))
                {
                    string scoreText = File.ReadAllText(SCORES_FILE);
                    if (int.TryParse(scoreText, out int savedScore))
                    {
                        highScore = savedScore;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading score: {ex.Message}");
            }
        }
    }
}