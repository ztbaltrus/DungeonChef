using System.Collections.Generic;
using System.Linq;
using DungeonChef.Src.ECS.Components;
using DungeonChef.Src.Entities;
using DungeonChef.Src.Gameplay;
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
            var player = new Player(grid);
            Entities.Add(player);
            return player;
        }

        public Enemy CreateEnemy(Vector2 grid, EnemyDefinition? definition)
        {
            var enemy = new Enemy(grid, definition);
            Entities.Add(enemy);
            return enemy;
        }

        public Loot CreateLoot(Vector2 grid, string itemId)
        {
            var loot = new Loot(grid, itemId);
            Entities.Add(loot);
            return loot;
        }

        public Entity? FindWith<TComponent>() where TComponent : class, IComponent
        {
            return Entities.FirstOrDefault(e => e.HasComponent<TComponent>());
        }

        public IEnumerable<Entity> With<TComponent>() where TComponent : class, IComponent
        {
            return Entities.Where(e => e.HasComponent<TComponent>());
        }

        public IEnumerable<Entity> With<TComponent1, TComponent2>()
            where TComponent1 : class, IComponent
            where TComponent2 : class, IComponent
        {
            return Entities.Where(e => e.HasComponent<TComponent1>() && e.HasComponent<TComponent2>());
        }
    }
}
