using System;

namespace SpaceWarProject
{
    public class BasicEnemy : Enemy
    {
        private const double MOVE_SPEED = 4;
        private const int ENEMY_HEALTH = 30;
        private double moveTimer = 0;

        public BasicEnemy(double startX, double startY) 
            : base(startX, startY, 30, 30, "Basic")
        {
            health = ENEMY_HEALTH;
            speedX = -MOVE_SPEED;
            ScoreValue = 10;
        }

        public override void Move()
        {
            // Basit dalga hareketi
            moveTimer += 0.05;
            speedY = Math.Sin(moveTimer) * 2;
            spawnX += speedX;
            spawnY += speedY;

            // Ekran sınırlarını kontrol et
            if (spawnY < 0) spawnY = 0;
            if (spawnY > 600 - Height) spawnY = 600 - Height;
        }

        public override void Attack()
        {
            // Tek mermi at
            var bullet = new Bullet(spawnX, spawnY + Height / 2, -BULLET_SPEED, 0, 5, true);
            Bullets.Add(bullet);
        }
    }
}