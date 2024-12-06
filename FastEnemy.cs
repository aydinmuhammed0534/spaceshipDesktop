using System;

namespace SpaceWarProject
{
    public class FastEnemy : Enemy
    {
        private const double MOVE_SPEED = 7;
        private const int ENEMY_HEALTH = 30;
        private double verticalDirection = 1;
        private double directionChangeTimer = 0;

        public FastEnemy(double startX, double startY) 
            : base(startX, startY, 25, 25, "Fast")
        {
            health = ENEMY_HEALTH;
            speedX = -MOVE_SPEED;
            ScoreValue = 20;
        }

        public override void Move()
        {
            // Hızlı zigzag hareketi
            directionChangeTimer += 0.05;
            if (directionChangeTimer >= 1.0)
            {
                verticalDirection *= -1;
                directionChangeTimer = 0;
            }

            speedY = verticalDirection * MOVE_SPEED;
            spawnX += speedX;
            spawnY += speedY;

            // Ekran sınırlarını kontrol et
            if (spawnY < 0)
            {
                spawnY = 0;
                verticalDirection = 1;
            }
            if (spawnY > 600 - Height)
            {
                spawnY = 600 - Height;
                verticalDirection = -1;
            }
        }

        public override void Attack()
        {
            // Hızlı düşman iki mermi atar
            double[] angles = { -10, 10 };
            foreach (var angle in angles)
            {
                double radians = angle * Math.PI / 180.0;
                double bulletSpeedX = -BULLET_SPEED * 1.2 * Math.Cos(radians);
                double bulletSpeedY = BULLET_SPEED * 1.2 * Math.Sin(radians);
                var bullet = new Bullet(spawnX, spawnY + Height / 2, bulletSpeedX, bulletSpeedY, 7, true);
                Bullets.Add(bullet);
            }
        }
    }
}