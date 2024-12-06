using System;

namespace SpaceWarProject
{
    public class GameObject
    {
        public double spawnX { get; set; }
        public double spawnY { get; set; }
        public double Width { get; protected set; }
        public double Height { get; protected set; }

        public GameObject(double x, double y, double width, double height)
        {
            spawnX = x;
            spawnY = y;
            Width = width;
            Height = height;
        }
    }
}
