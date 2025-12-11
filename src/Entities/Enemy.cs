using System.ComponentModel;
using System.Linq;
using DungeonChef.Src.ECS;
using DungeonChef.Src.ECS.Components;
using DungeonChef.Src.Gameplay;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Entities
{
    public sealed class Enemy : Entity
    {
        public Enemy(Vector2 position, EnemyDefinition definition) : base(definition?.Id ?? "Enemy")
        {
            var transform = AddComponent(new TransformComponent());
            transform.Position = position;
            transform.Grid = position;

            float maxHp = definition?.Hp > 0 ? definition.Hp : 30f;
            AddComponent(new HealthComponent { Max = maxHp, Current = maxHp });

            float speed = definition?.Speed > 0 ? definition.Speed : 2f;
            AddComponent(new MovementComponent(MovementBehavior.SeekTarget, speed));

            float damage = definition?.Attacks?.FirstOrDefault()?.Damage ?? 1f;

            AddComponent(new EnemyComponent(
                enemyId: definition?.Id ?? "generic",
                speed: speed,
                attackRange: 2f,
                damage: damage,
                attackCooldown: 1f,
                stopDistance: 1f));

            string? spritePath = null;
            if (!string.IsNullOrEmpty(definition?.Texture))
            {
                spritePath = $"Content/{definition.Texture}";
            }
            else if (!string.IsNullOrEmpty(definition?.Id))
            {
                spritePath = $"Content/Sprites/Enemies/{definition.Id}.png";
            }

            AddComponent(new RenderComponent(RenderArchetype.Enemy, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, spritePath)));
        }
    }
}
