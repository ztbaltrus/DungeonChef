using System.Linq;
using DungeonChef.Src.ECS;
using DungeonChef.Src.ECS.Components;
using DungeonChef.Src.Gameplay;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Entities
{
    public sealed class Enemy : Entity
    {
        public Enemy(Vector2 position, EnemyDefinition? definition) : base(definition?.Id ?? "Enemy")
        {
            var transform = AddComponent(new TransformComponent());
            transform.Position = position;
            transform.Grid = position;

            float maxHp = definition?.Hp > 0 ? definition.Hp : 30f;
            AddComponent(new HealthComponent { Max = maxHp, Current = maxHp });

            float speed = definition?.Speed > 0 ? definition.Speed : 2f;
            AddComponent(new MovementComponent(MovementBehavior.SeekTarget, speed));

            float damage = definition?.Attacks?.FirstOrDefault()?.Damage ?? 1f;
            var enemyComponent = AddComponent(new EnemyComponent(
                enemyId: definition?.Id ?? "generic",
                speed: speed,
                attackRange: 2f,
                damage: damage,
                attackCooldown: 1f,
                stopDistance: 1f));

            enemyComponent.MoveCooldown = 0.2f;

            string? spritePath = null;
            if (!string.IsNullOrEmpty(definition?.Texture))
            {
                spritePath = $"Sprites/Enemies/{definition.Texture}";
            }
            else if (!string.IsNullOrEmpty(definition?.Id))
            {
                spritePath = $"Sprites/Enemies/{definition.Id}.png";
            }

            AddComponent(new RenderComponent(RenderArchetype.Enemy, spritePath));
        }
    }
}
