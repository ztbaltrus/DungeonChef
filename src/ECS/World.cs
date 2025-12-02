using System.Collections.Generic;
using DungeonChef.Src.Entities;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.ECS
{
    public sealed class World
    {
        public List<Entity> Entities { get; } = new();

        public void Update(GameTime gt)
        {
            // systems run elsewhere (GameRoot)
        }

        public Player CreatePlayer(Vector2 grid)
        {
            var player = new Player(grid)
            {
                Grid = grid,
                Position = grid
            };
            Entities.Add(player);
            return player; 
        }

        public Enemy CreateEnemy(Vector2 grid, string enemyId)
        {
            var enemy = new Enemy(grid)
            {
                Grid = grid,
                Position = grid,
                EnemyId = enemyId
            };
            Entities.Add(enemy);
            return enemy;
        }

        public Loot CreateLoot(Vector2 grid, string itemId)
        {
            var loot = new Loot(grid, itemId);
            Entities.Add(loot);
            return loot;
        }
    }
}
