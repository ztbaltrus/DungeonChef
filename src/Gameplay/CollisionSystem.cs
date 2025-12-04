using DungeonChef.Src.ECS;
using DungeonChef.Src.ECS.Components;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Gameplay
{
    public sealed class CollisionSystem
    {
        private const int WorldMinX = 0;
        private const int WorldMinY = 0;
        private const int WorldMaxX = 9;
        private const int WorldMaxY = 9;

        public void Update(World world)
        {
            foreach (var entity in world.With<TransformComponent>())
            {
                var transform = entity.GetComponent<TransformComponent>();
                var grid = transform.Grid;
                grid.X = MathHelper.Clamp(grid.X, WorldMinX, WorldMaxX);
                grid.Y = MathHelper.Clamp(grid.Y, WorldMinY, WorldMaxY);
                transform.Grid = grid;
                transform.Position = grid;
            }
        }
    }
}
