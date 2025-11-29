using System.Collections.Generic;
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

        public Entity CreateEntity(Vector2 grid)
        {
            var e = new Entity
            {
                Grid = grid,
                Position = grid
            };
            Entities.Add(e);
            return e;
        }
    }
}
