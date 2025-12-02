using System.Linq;
using DungeonChef.Src.ECS;
using DungeonChef.Src.Entities;
using DungeonChef.Src.Rendering;
using Microsoft.Xna.Framework;
using Serilog;

namespace DungeonChef.Src.Gameplay
{
    public sealed class EnemySystem
    {
        private const float EnemySpeed = 2f;
        private const float EnemyAttackRange = 2f; // World units
        private const float EnemyDamage = 1f;
        private const float AttackCooldown = 1f; // Seconds between attacks

        private float moveTimer = 0f;
        private float attackTimer = 0f; // Timer for attack cooldown

        public void Update(World world, GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            var player = world.Entities.FirstOrDefault(e => e.GetType() == typeof(Player));
            if (player == null)
                return;

            foreach (var e in world.Entities)
            {
                if (e.GetType() != typeof(Enemy))
                    continue;

                Vector2 toPlayer = player.Position - e.Position;
                float dist2 = toPlayer.LengthSquared();

                // Move towards the player
                if (dist2 > EnemyAttackRange * EnemyAttackRange) // Check distance to attack
                {
                    moveTimer += dt;
                    if (moveTimer >= 1f)
                    {
                        // Calculate direction towards the player from a certain distance away
                        Vector2 dir = Vector2.Normalize(player.Position - e.Position);

                        // Stop the enemy at a certain distance away from the player
                        float stopDistance = 1f; // You can adjust this value as needed
                        if (dist2 < stopDistance * stopDistance)
                        {
                            e.Position = new Vector2(
                                MathHelper.Clamp(e.Position.X, 0f, 9f),
                                MathHelper.Clamp(e.Position.Y, 0f, 9f)
                            );
                            e.Grid = e.Position;
                            moveTimer = 0f; // Reset timer
                            return; // Stop updating the enemy's position
                        }

                        e.Position += dir * EnemySpeed * dt;
                        e.Grid = e.Position;

                        // Optional: clamp enemies to room bounds (0..9 test room)
                        e.Position = new Vector2(
                            MathHelper.Clamp(e.Position.X, 0f, 9f),
                            MathHelper.Clamp(e.Position.Y, 0f, 9f)
                        );
                        e.Grid = e.Position;
                    }
                }
                else
                {
                    // Attack player with cooldown
                    attackTimer += dt;
                    if (attackTimer >= AttackCooldown)
                    {
                        // Check if player is still in attack range
                        if (dist2 <= EnemyAttackRange * EnemyAttackRange)
                        {
                            player.HP -= EnemyDamage;
                            if (player.HP <= 0f)
                            {
                                Log.Information("Player has been defeated by an enemy.");
                            }
                            attackTimer = 0f; // Reset attack timer
                        }
                    }
                }
            }
        }
    }
}
