using System.Collections.Generic;
using DungeonChef.Src.ECS;
using DungeonChef.Src.ECS.Components;
using DungeonChef.Src.Gameplay.Items;
using DungeonChef.Src.Gameplay.Rooms;
using DungeonChef.Src.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonChef.Src.Gameplay
{
    public sealed class CombatSystem
    {
        public static CombatSystem Instance { get; } = new CombatSystem();

        private CombatSystem()
        {
        }

        private float _swingVisibleTimer;
        private Vector2 _lastAttackOriginWorld;
        private Vector2 _lastAttackDirWorld;
        private Texture2D? _pixel;

        public void Update(World world, GameTime gt, IsoCamera camera, RoomNode currentRoom)
        {
            if (currentRoom == null)
                return;

            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            _swingVisibleTimer = MathHelper.Max(0f, _swingVisibleTimer - dt);

            var player = world.FindWith<PlayerTagComponent>();
            if (player == null)
                return;

            var attack = player.GetComponent<MeleeAttackComponent>();
            var playerTransform = player.GetComponent<TransformComponent>();
            attack.CooldownTimer = MathHelper.Max(0f, attack.CooldownTimer - dt);

            var mouse = Mouse.GetState();
            bool leftDown = mouse.LeftButton == ButtonState.Pressed;
            if (!leftDown || attack.CooldownTimer > 0f)
                return;

            Vector2 playerIso = IsoMath.ToScreen(playerTransform.Position.X, playerTransform.Position.Y);
            Vector2 playerScreen = playerIso - camera.Position;
            Vector2 mouseScreen = new Vector2(mouse.X, mouse.Y);

            Vector2 screenDir = mouseScreen - playerScreen;
            if (screenDir.LengthSquared() < 0.001f)
                return;

            screenDir.Normalize();

            float a = IsoMath.TileW / 2f;
            float b = IsoMath.TileH / 2f;

            float dx = 0.5f * (screenDir.X / a + screenDir.Y / b);
            float dy = 0.5f * (screenDir.Y / b - screenDir.X / a);

            Vector2 worldDir = new Vector2(dx, dy);
            if (worldDir.LengthSquared() < 0.0001f)
                return;

            worldDir.Normalize();

            _lastAttackOriginWorld = playerTransform.Position;
            _lastAttackDirWorld = worldDir;
            _swingVisibleTimer = 0.12f;

            float maxAngleCos = (float)System.Math.Cos(MathHelper.ToRadians(attack.AngleDegrees));
            var toRemove = new List<Entity>();

            foreach (var enemy in world.With<EnemyComponent, TransformComponent>())
            {
                var enemyTransform = enemy.GetComponent<TransformComponent>();
                var enemyHealth = enemy.GetComponent<HealthComponent>();

                Vector2 toEnemy = enemyTransform.Position - playerTransform.Position;
                float dist = toEnemy.Length();
                if (dist < 0.0001f || dist > attack.Range)
                    continue;

                Vector2 toEnemyDir = toEnemy / dist;
                float dot = Vector2.Dot(worldDir, toEnemyDir);
                if (dot < maxAngleCos)
                    continue;

                enemyHealth.Damage(attack.Damage);
                if (enemyHealth.IsDead)
                    toRemove.Add(enemy);
            }

            foreach (var dead in toRemove)
            {
                var deadTransform = dead.GetComponent<TransformComponent>();
                LootDropper.TryDropLoot(world, deadTransform.Position, currentRoom);
                world.Entities.Remove(dead);
            }

            attack.CooldownTimer = attack.CooldownSeconds;
        }

        private void EnsurePixel(GraphicsDevice gd)
        {
            if (_pixel != null)
                return;

            _pixel = new Texture2D(gd, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void DrawDebug(SpriteBatch sb, IsoCamera camera, World world)
        {
            if (_swingVisibleTimer <= 0f)
                return;

            EnsurePixel(sb.GraphicsDevice);

            var player = world.FindWith<PlayerTagComponent>();
            if (player == null)
                return;

            var attack = player.GetComponent<MeleeAttackComponent>();
            Vector2 startWorld = _lastAttackOriginWorld;
            Vector2 endWorld = _lastAttackOriginWorld + _lastAttackDirWorld * attack.Range;

            Vector2 startIso = IsoMath.ToScreen(startWorld.X, startWorld.Y);
            Vector2 endIso = IsoMath.ToScreen(endWorld.X, endWorld.Y);

            Vector2 startScreen = startIso - camera.Position;
            Vector2 endScreen = endIso - camera.Position;

            Vector2 segment = endScreen - startScreen;
            float length = segment.Length();
            if (length < 1f)
                return;

            float angle = (float)System.Math.Atan2(segment.Y, segment.X);

            sb.Draw(
                _pixel!,
                position: startScreen,
                sourceRectangle: null,
                color: Color.Gold,
                rotation: angle,
                origin: new Vector2(0f, 0.5f),
                scale: new Vector2(length, 4f),
                effects: SpriteEffects.None,
                layerDepth: 0.6f);
        }
    }
}
