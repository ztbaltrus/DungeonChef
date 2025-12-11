using DungeonChef.Src.ECS;
using DungeonChef.Src.ECS.Components;
using Microsoft.Xna.Framework;
using Serilog;

namespace DungeonChef.Src.ECS.Systems
{
    public sealed class EnemySystem
    {
        public static EnemySystem Instance { get; } = new EnemySystem();

        private EnemySystem()
        {
        }

        public void Update(World world, GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            var player = world.FindWith<PlayerTagComponent>();
            if (player == null)
                return;

            var playerTransform = player.GetComponent<TransformComponent>();
            var playerHealth = player.GetComponent<HealthComponent>();

            foreach (var entity in world.With<EnemyComponent, TransformComponent>())
            {
                var enemyTransform = entity.GetComponent<TransformComponent>();
                var enemyComponent = entity.GetComponent<EnemyComponent>();

                Vector2 toPlayer = playerTransform.Position - enemyTransform.Position;
                float dist2 = toPlayer.LengthSquared();
                float attackRangeSq = enemyComponent.AttackRange * enemyComponent.AttackRange;

                if (dist2 > attackRangeSq)
                {
                    enemyComponent.MoveTimer += dt;
                    if (enemyComponent.MoveTimer < enemyComponent.MoveCooldown)
                        continue;

                    if (toPlayer.LengthSquared() <= float.Epsilon)
                        continue;

                    Vector2 dir = Vector2.Normalize(toPlayer);
                    float stopDistSq = enemyComponent.StopDistance * enemyComponent.StopDistance;
                    if (dist2 > stopDistSq)
                    {
                        enemyTransform.Position += dir * enemyComponent.Speed * dt;
                        enemyTransform.Position = new Vector2(
                            MathHelper.Clamp(enemyTransform.Position.X, 0f, 9f),
                            MathHelper.Clamp(enemyTransform.Position.Y, 0f, 9f));
                        enemyTransform.Grid = enemyTransform.Position;
                    }

                    enemyComponent.MoveTimer = 0f;
                }
                else
                {
                    enemyComponent.AttackTimer += dt;
                    if (enemyComponent.AttackTimer < enemyComponent.AttackCooldown)
                        continue;

                    playerHealth.Damage(enemyComponent.Damage);
                    if (playerHealth.IsDead)
                    {
                        Log.Information("Player has been defeated by an enemy.");
                    }

                    enemyComponent.AttackTimer = 0f;
                }
            }
        }
    }
}
