using DungeonChef.Src.ECS;
using DungeonChef.Src.ECS.Components;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Entities
{
    public sealed class Loot : Entity
    {
        public Loot(Vector2 position, string itemId) : base("Loot")
        {
            var transform = AddComponent(new TransformComponent());
            transform.Position = position;
            transform.Grid = position;

            AddComponent(new LootComponent(itemId));
            AddComponent(new RenderComponent(RenderArchetype.Loot));
        }
    }
}
