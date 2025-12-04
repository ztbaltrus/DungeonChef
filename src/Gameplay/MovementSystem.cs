using DungeonChef.Src.ECS;
using DungeonChef.Src.Entities;
using DungeonChef.Src.Rendering;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Gameplay
{
    public sealed class MovementSystem
    {
        private const float Speed = 3f;

        // World bounds in "iso world" units (matching your 10x10 grid)
        private const float WorldMinX = 0f;
        private const float WorldMinY = 0f;
        private const float WorldMaxX = 9f; // GridWidth - 1
        private const float WorldMaxY = 9f; // GridHeight - 1

        public void Update(World world, GameTime gt, Src.Core.InputState input)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            // Basis vectors in world space:
            // ex_world -> horizontal on screen
            // ey_world -> vertical on screen (scaled so speed matches)
            Vector2 exWorld = Vector2.Normalize(new Vector2(1f, -1f)); // horizontal
            Vector2 eyWorld = Vector2.Normalize(new Vector2(1f,  1f)); // vertical

            // Compute how long they appear on screen
            Vector2 exScreen = ToScreen(exWorld);
            Vector2 eyScreen = ToScreen(eyWorld);

            float exLen = exScreen.Length();
            float eyLen = eyScreen.Length();

            // Scale vertical basis so its screen length matches horizontal
            if (eyLen > 0.0001f)
            {
                float scale = exLen / eyLen;
                eyWorld *= scale;
            }

            foreach (var e in world.Entities)
            {
                if (e.GetType() != typeof(Player))
                    continue;

                Vector2 move = input.Move; // screen-space input from WASD

                Player player = e as Player;
                player.IsMoving = true;

                if (move.LengthSquared() > 0f)
                {
                    move.Normalize();

                    // Build world direction from screen X/Y axes
                    Vector2 worldDelta =
                        move.X * exWorld +   // left/right part
                        move.Y * eyWorld;    // up/down part

                    // IMPORTANT: do NOT normalize worldDelta again.
                    // We want screen speed to be uniform based on our basis scaling.
                    player.Position += worldDelta * Speed * dt;

                    // Clamp to world bounds so player stays on the tile area
                    player.Position = new Vector2(
                        MathHelper.Clamp(player.Position.X, WorldMinX, WorldMaxX),
                        MathHelper.Clamp(player.Position.Y, WorldMinY, WorldMaxY)
                    );
                }

                // Grid = approximate tile under the player
                player.Grid = player.Position;
            }
        }

        private static Vector2 ToScreen(Vector2 world)
        {
            float sx = (world.X - world.Y) * (IsoMath.TileW / 2f);
            float sy = (world.X + world.Y) * (IsoMath.TileH / 2f);
            return new Vector2(sx, sy);
        }
    }
}
