using DungeonChef.Src.ECS;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Gameplay
{
    public sealed class CollisionSystem
    {
        private const int WorldMinX = 0;
        private const int WorldMinY = 0;
        private const int WorldMaxX = 9;
        private const int WorldMaxY = 9;

        public void Update(World world, GameTime gt)
        {
            foreach (var e in world.Entities)
            {
                // Clamp to simple 10x10 grid for now
                var g = e.Grid;
                g.X = MathHelper.Clamp(g.X, WorldMinX, WorldMaxX);
                g.Y = MathHelper.Clamp(g.Y, WorldMinY, WorldMaxY);
                e.Grid = g;
                e.Position = g;
            }
        }
    }
}
