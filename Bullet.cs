namespace SpaceWarProject
{
    public class Bullet : GameObject
    {
        private double speedX;
        private double speedY;
        public int Damage { get; }
        public bool IsEnemyBullet { get; }

        public Bullet(double x, double y, double speedX, double speedY, int damage, bool isEnemyBullet) 
            : base(x, y, 10, 5)
        {
            this.speedX = speedX;
            this.speedY = speedY;
            Damage = damage;
            IsEnemyBullet = isEnemyBullet;
        }

        public void Move()
        {
            spawnX += speedX;
            spawnY += speedY;
        }

        public bool IsOutOfBounds()
        {
            return spawnX < -Width || spawnY > 700 || spawnY < -Height || spawnY > 500;
        }
    }
}