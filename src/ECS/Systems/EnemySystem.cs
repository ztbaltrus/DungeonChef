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
                if (dist2 <= float.Epsilon)
                {
                    enemyTransform.Velocity = Vector2.Zero;
                    continue;
                }

                float distance = System.MathF.Sqrt(dist2);
                bool withinStopRange = distance <= enemyComponent.StopDistance;
                bool withinAttackRange = distance <= enemyComponent.AttackRange;

                if (!withinStopRange)
                {
                    Vector2 dir = toPlayer / distance;
                    float remaining = distance - enemyComponent.StopDistance;
                    float maxStep = enemyComponent.Speed * dt;
                    float actualStep = System.MathF.Min(remaining, maxStep);
                    Vector2 frameDelta = dir * actualStep;

                    enemyTransform.Position += frameDelta;
                    enemyTransform.Position = new Vector2(
                        MathHelper.Clamp(enemyTransform.Position.X, 0f, 9f),
                        MathHelper.Clamp(enemyTransform.Position.Y, 0f, 9f));
                    enemyTransform.Grid = enemyTransform.Position;
                    enemyTransform.Velocity = frameDelta / dt;
                }
                else
                {
                    enemyTransform.Velocity = Vector2.Zero;
                }

                if (withinAttackRange)
                {
                    enemyComponent.AttackTimer += dt;
                    if (enemyComponent.AttackTimer >= enemyComponent.AttackCooldown)
                    {
                        playerHealth.Damage(enemyComponent.Damage);
                        if (playerHealth.IsDead)
                        {
                            Log.Information("Player has been defeated by an enemy.");
                        }

                        enemyComponent.AttackTimer = 0f;
                    }
                }
                else
                {
                    enemyComponent.AttackTimer = 0f;
                }
            }
        }
    }
}
