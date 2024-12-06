using System;
using System.Collections.Generic;

namespace SpaceWarProject
{
    public class BossEnemy : Enemy
    {
        private const double MOVE_SPEED = 2;
        private const int ENEMY_HEALTH = 120;
        private double moveTimer = 0;
        private double attackPhase = 0;
        private Random random = new Random();

        public BossEnemy(double startX, double startY) 
            : base(startX, startY, 60, 60, "Boss")
        {
            health = ENEMY_HEALTH;
            speedX = -MOVE_SPEED;
            ScoreValue = 50;
        }

        public override void Move()
        {
            // Karmaşık hareket paterni
            moveTimer += 0.02;
            
            // Yatay hareket (yavaş ileri-geri)
            speedX = -MOVE_SPEED + Math.Sin(moveTimer * 0.5) * 2;
            
            // Dikey hareket (8 şeklinde)
            double verticalAmplitude = 150;
            spawnY = 300 + Math.Sin(moveTimer) * verticalAmplitude * Math.Cos(moveTimer * 0.5);
            
            spawnX += speedX;

            // Ekran sınırlarını kontrol et
            if (spawnX < 400) spawnX = 400; // Boss ekranın ortasından daha ileriye geçemez
            if (spawnY < 0) spawnY = 0;
            if (spawnY > 600 - Height) spawnY = 600 - Height;
        }

        public override void Attack()
        {
            attackPhase += 0.1;
            
            // Farklı saldırı paternleri
            switch ((int)(attackPhase * 2) % 3)
            {
                case 0: // Dairesel ateş
                    CircularAttack();
                    break;
                case 1: // Dalga ateşi
                    WaveAttack();
                    break;
                case 2: // Hedefli ateş
                    TargetedAttack();
                    break;
            }
        }

        private void CircularAttack()
        {
            // 360 derece ateş
            int bulletCount = 12;
            for (int i = 0; i < bulletCount; i++)
            {
                double angle = (360.0 / bulletCount) * i;
                double radians = angle * Math.PI / 180.0;
                double bulletSpeedX = -BULLET_SPEED * Math.Cos(radians);
                double bulletSpeedY = BULLET_SPEED * Math.Sin(radians);
                var bullet = new Bullet(spawnX + Width/2, spawnY + Height/2, bulletSpeedX, bulletSpeedY, 20, true);
                Bullets.Add(bullet);
            }
        }

        private void WaveAttack()
        {
            // Dalga şeklinde ateş
            double baseAngle = Math.Sin(attackPhase * 2) * 30;
            double[] angles = { baseAngle - 20, baseAngle - 10, baseAngle, baseAngle + 10, baseAngle + 20 };
            foreach (var angle in angles)
            {
                double radians = angle * Math.PI / 180.0;
                double bulletSpeedX = -BULLET_SPEED * Math.Cos(radians);
                double bulletSpeedY = BULLET_SPEED * Math.Sin(radians);
                var bullet = new Bullet(spawnX, spawnY + Height/2, bulletSpeedX, bulletSpeedY, 15, true);
                Bullets.Add(bullet);
            }
        }

        private void TargetedAttack()
        {
            // Oyuncuya doğru ateş
            var player = Game.Current.PlayerShip;
            if (player != null)
            {
                double deltaX = player.spawnX - spawnX;
                double deltaY = player.spawnY - spawnY;
                double angle = Math.Atan2(deltaY, -deltaX);
                
                // Ana atış
                double bulletSpeedX = -BULLET_SPEED * Math.Cos(angle);
                double bulletSpeedY = BULLET_SPEED * Math.Sin(angle);
                var bullet = new Bullet(spawnX, spawnY + Height/2, bulletSpeedX, bulletSpeedY, 25, true);
                Bullets.Add(bullet);

                // Yan atışlar
                for (int i = -1; i <= 1; i++)
                {
                    if (i == 0) continue;
                    double spreadAngle = angle + (i * Math.PI / 6);
                    bulletSpeedX = -BULLET_SPEED * Math.Cos(spreadAngle);
                    bulletSpeedY = BULLET_SPEED * Math.Sin(spreadAngle);
                    bullet = new Bullet(spawnX, spawnY + Height/2, bulletSpeedX, bulletSpeedY, 15, true);
                    Bullets.Add(bullet);
                }
            }
        }
    }
}
