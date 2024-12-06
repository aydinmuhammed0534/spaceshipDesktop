using System;
using System.Collections.Generic;

namespace SpaceWarProject
{
    public class Spaceship : GameObject
    {
        private const double MOVE_SPEED = 5;
        private const int BULLET_SPEED = 10;
        private const double SHOOT_COOLDOWN = 0.25; // seconds

        public int Health { get; set; } = 100;
        public double Speed { get; } = 300; // pixels per second
        public List<Bullet> Bullets { get; } = new List<Bullet>();
        public bool IsAlive => Health > 0;

        public Spaceship(double x, double y) : base(x, y, 50, 50)
        {
        }

        public void Move(double deltaX, double deltaY)
        {
            spawnX += deltaX;
            spawnY += deltaY;

            // Ekran sınırlarını kontrol et
            if (spawnX < 0) spawnX = 0;
            if (spawnX > 800 - Width) spawnX = 800 - Width;
            if (spawnY < 0) spawnY = 0;
            if (spawnY > 600 - Height) spawnY = 600 - Height;
        }

        public void Shoot()
        {
            var bullet = new Bullet(
                spawnX + Width,
                spawnY + Height / 2,
                BULLET_SPEED,
                0,
                20,
                false
            );
            Bullets.Add(bullet);
        }

        public void UpdateBullets()
        {
            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                Bullets[i].Move();
                if (Bullets[i].spawnX > 850 || Bullets[i].spawnX < -50 ||
                    Bullets[i].spawnY > 650 || Bullets[i].spawnY < -50)
                {
                    Bullets.RemoveAt(i);
                }
            }
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
        }
    }
}