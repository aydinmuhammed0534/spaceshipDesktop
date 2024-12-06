using System;

namespace SpaceWarProject
{
    public class StrongEnemy : Enemy
    {
        private const double MOVE_SPEED = 3;
        private const int ENEMY_HEALTH = 100;
        private double oscillationTimer = 0;
        private double baseY;

        public StrongEnemy(double startX, double startY) 
            : base(startX, startY, 45, 45, "Strong")
        {
            health = ENEMY_HEALTH;
            speedX = -MOVE_SPEED;
            ScoreValue = 30;
            baseY = startY;
        }

        public override void Move()
        {
            // Yavaş ama geniş dairesel hareket
            oscillationTimer += 0.02;
            speedY = Math.Sin(oscillationTimer) * 3;
            
            spawnX += speedX;
            spawnY = baseY + Math.Sin(oscillationTimer) * 100; // Geniş dikey hareket

            // Ekran sınırlarını kontrol et
            if (spawnY < 0) spawnY = 0;
            if (spawnY > 600 - Height) spawnY = 600 - Height;
        }

        public override void Attack()
        {
            // Güçlü düşman çoklu mermi atar
            double[] angles = { -20, -10, 0, 10, 20 };
            foreach (var angle in angles)
            {
                double radians = angle * Math.PI / 180.0;
                double bulletSpeedX = -BULLET_SPEED * 0.8 * Math.Cos(radians);
                double bulletSpeedY = BULLET_SPEED * 0.8 * Math.Sin(radians);
                var bullet = new Bullet(spawnX, spawnY + Height / 2, bulletSpeedX, bulletSpeedY, 15, true);
                Bullets.Add(bullet);
            }
        }
    }
}