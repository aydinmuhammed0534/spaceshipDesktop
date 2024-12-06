using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceWarProject
{
    public class CollisionDetector
    {
        public bool CheckCollision(double x1, double y1, double width1, double height1,
                                 double x2, double y2, double width2, double height2)
        {
            return x1 < x2 + width2 &&
                   x1 + width1 > x2 &&
                   y1 < y2 + height2 &&
                   y1 + height1 > y2;
        }

        public void CheckBulletEnemyCollisions(List<Bullet> bullets, List<Enemy> enemies)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                if (bullet.IsEnemyBullet) continue;  // Düşman mermilerini kontrol etme

                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    var enemy = enemies[j];
                    if (CheckCollision(bullet.spawnY, bullet.spawnY, bullet.Width, bullet.Height,
                                     enemy.spawnX, enemy.spawnY, enemy.Width, enemy.Height))
                    {
                        enemy.TakeDamage(bullet.Damage);
                        bullets.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public bool CheckPlayerEnemyCollision(Spaceship player, Enemy enemy)
        {
            return CheckCollision(player.spawnX, player.spawnY, player.Width, player.Height,
                                enemy.spawnX, enemy.spawnY, enemy.Width, enemy.Height);
        }

        public bool CheckBulletPlayerCollision(Bullet bullet, Spaceship player)
        {
            return CheckCollision(bullet.spawnX, bullet.spawnY, bullet.Width, bullet.Height,
                                player.spawnX, player.spawnY, player.Width, player.Height);
        }
    }
}