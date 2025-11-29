using System.Linq;
using DungeonChef.Src.ECS;
using DungeonChef.Src.Rendering;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Gameplay
{
    public sealed class EnemySystem
    {
        private const float EnemySpeed = 2f;

        public void Update(World world, GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            var player = world.Entities.FirstOrDefault(e => e.IsPlayer);
            if (player == null)
                return;

            foreach (var e in world.Entities)
            {
                if (!e.IsEnemy)
                    continue;

                Vector2 toPlayer = player.Position - e.Position;
                float dist2 = toPlayer.LengthSquared();
                if (dist2 < 0.0001f)
                    continue;

                Vector2 dir = toPlayer / System.MathF.Sqrt(dist2);

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
    }
}
