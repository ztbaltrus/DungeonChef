using System.Collections.Generic;
using System.Linq;
using DungeonChef.Src.ECS;
using DungeonChef.Src.Rendering;
using DungeonChef.Src.Gameplay.Items;
using DungeonChef.Src.Gameplay.Rooms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DungeonChef.Src.Entities;
using System.Runtime.Serialization;

namespace DungeonChef.Src.Gameplay
{
    public sealed class CombatSystem
    {
        private const float AttackCooldown = 0.25f;  // seconds between swings
        private const float AttackRange = 2.5f;      // world units
        private const float AttackAngleDeg = 60f;    // cone half-angle

        private float _attackTimer;

        // --- Visual debug slash state ---
        private float _swingVisibleTimer;            // how long to show the slash
        private Vector2 _lastAttackOriginWorld;
        private Vector2 _lastAttackDirWorld;

        private Texture2D? _pixel;                   // 1x1 texture for drawing the line

        public void Update(World world, GameTime gt, IsoCamera camera, RoomNode currentRoom)
        {
            if (currentRoom == null)
                return;

            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            _attackTimer -= dt;
            if (_attackTimer < 0f) _attackTimer = 0f;

            _swingVisibleTimer -= dt;
            if (_swingVisibleTimer < 0f) _swingVisibleTimer = 0f;

            var player = world.Entities.FirstOrDefault(e => e.GetType() == typeof(Player));
            if (player == null)
                return;

            var mouse = Mouse.GetState();
            bool leftDown = mouse.LeftButton == ButtonState.Pressed;

            if (!leftDown || _attackTimer > 0f)
                return;

            // --- Perform attack in direction of mouse pointer ---

            // Player screen position
            Vector2 playerIso = IsoMath.ToScreen(player.Position.X, player.Position.Y);
            Vector2 playerScreen = playerIso - camera.Position;

            // Mouse screen position
            Vector2 mouseScreen = new(mouse.X, mouse.Y);

            Vector2 screenDir = mouseScreen - playerScreen;
            if (screenDir.LengthSquared() < 0.001f)
                return;

            screenDir.Normalize();

            // Convert screen direction -> world direction (inverse iso transform)
            float a = IsoMath.TileW / 2f;
            float b = IsoMath.TileH / 2f;

            float dx = 0.5f * (screenDir.X / a + screenDir.Y / b);
            float dy = 0.5f * (screenDir.Y / b - screenDir.X / a);

            Vector2 worldDir = new(dx, dy);
            if (worldDir.LengthSquared() < 0.0001f)
                return;
            worldDir.Normalize();

            // Save for debug rendering
            _lastAttackOriginWorld = player.Position;
            _lastAttackDirWorld = worldDir;
            _swingVisibleTimer = 0.12f; // show slash for 120ms

            // Attack cone
            float maxAngleCos = (float)Math.Cos(MathHelper.ToRadians(AttackAngleDeg));

            var toRemove = new List<Entity>();

            foreach (var e in world.Entities)
            {
                if (e.GetType() != typeof(Enemy))
                    continue;

                Vector2 toEnemy = e.Position - player.Position;
                float dist = toEnemy.Length();
                if (dist < 0.0001f || dist > AttackRange)
                    continue;

                Vector2 toEnemyDir = toEnemy / dist;

                float dot = Vector2.Dot(worldDir, toEnemyDir);
                if (dot >= maxAngleCos)
                {
                    // Hit!
                    e.HP -= 5f;
                    if (e.HP <= 0f)
                        toRemove.Add(e);
                }
            }

            foreach (var dead in toRemove)
            {
                // Chance-based item/coin drop
                LootDropper.TryDropLoot(world, dead.Position, currentRoom);
                world.Entities.Remove(dead);
            }

            _attackTimer = AttackCooldown;
        }

        // -------- VISUALIZATION --------

        private void EnsurePixel(GraphicsDevice gd)
        {
            if (_pixel != null) return;
            _pixel = new Texture2D(gd, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void DrawDebug(SpriteBatch sb, IsoCamera camera)
        {
            if (_swingVisibleTimer <= 0f)
                return;

            EnsurePixel(sb.GraphicsDevice);

            // Start/end in world
            Vector2 startWorld = _lastAttackOriginWorld;
            Vector2 endWorld = _lastAttackOriginWorld + _lastAttackDirWorld * AttackRange;

            // Convert to screen using iso
            Vector2 startIso = IsoMath.ToScreen(startWorld.X, startWorld.Y);
            Vector2 endIso = IsoMath.ToScreen(endWorld.X, endWorld.Y);

            Vector2 startScreen = startIso - camera.Position;
            Vector2 endScreen = endIso - camera.Position;

            Vector2 segment = endScreen - startScreen;
            float length = segment.Length();
            if (length < 1f)
                return;

            float angle = (float)System.Math.Atan2(segment.Y, segment.X);

            // Slight thickness for visibility
            float thickness = 4f;

            sb.Draw(
                _pixel!,
                position: startScreen,
                sourceRectangle: null,
                color: Color.Gold,
                rotation: angle,
                origin: new Vector2(0f, 0.5f),
                scale: new Vector2(length, thickness),
                effects: SpriteEffects.None,
                layerDepth: 0.6f
            );
        }
    }
}
