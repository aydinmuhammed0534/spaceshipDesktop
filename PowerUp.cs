using System.Collections.Generic;

namespace SpaceWarProject
{
    public class PowerUp : GameObject
    {
        public enum PowerUpType
        {
            Health,
            Shield,
            DoubleDamage,
            Speed
        }

        public PowerUpType Type { get; private set; }
        public bool IsActive { get; set; } = true;

        public PowerUp(double x, double y, PowerUpType type) 
            : base(x, y, 30, 30) // 30x30 piksel powerup boyutu
        {
            Type = type;
        }

        public void ApplyPowerUp(Spaceship player)
        {
            switch (Type)
            {
                case PowerUpType.Health:
                    player.TakeDamage(-50); // Can yenileme (-hasar = iyileştirme)
                    break;
                case PowerUpType.DoubleDamage:
                    // Hasar artırma özelliği Spaceship sınıfına eklenmeli
                    break;
                case PowerUpType.Shield:
                    // Kalkan özelliği Spaceship sınıfına eklenmeli
                    break;
                case PowerUpType.Speed:
                    // Hız artırma özelliği Spaceship sınıfına eklenmeli
                    break;
            }
            IsActive = false;
        }

        public void Update(double deltaTime)
        {
            if (!IsActive) return;

            // PowerUp'ı yavaşça aşağı hareket ettir
            spawnY += 1;

            // Ekrandan çıktıysa deaktif et
            if (spawnY > 600)
            {
                IsActive = false;
            }
        }
    }
}