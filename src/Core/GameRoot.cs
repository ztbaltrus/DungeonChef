using System;
using System.Linq;
using DungeonChef.Src.ECS;
using DungeonChef.Src.Gameplay;
using DungeonChef.Src.Gameplay.Items;
using DungeonChef.Src.Gameplay.Rooms;
using DungeonChef.Src.Rendering;
using DungeonChef.Src.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;

namespace DungeonChef.Src.Core
{
    public sealed class GameRoot : Game
    {
        private readonly GraphicsDeviceManager _gdm;
        private SpriteBatch _sb = null!;

        private readonly IsoCamera _camera = new();
        private readonly Input _input = new();
        private readonly MovementSystem _movementSystem = new();
        private readonly EnemySystem _enemySystem = new();
        private readonly CombatSystem _combatSystem = new();


        private World _world = null!;

        private LevelGenerator _levelGen = null!;
        private RoomController _roomController = null!;
        private Minimap _minimap = null!;
        private InteractPrompt _interactPrompt = null!;
        private Hud _hud = null!;

        private int _currentRunSeed;

        public static readonly int VirtualWidth = 1600;
        public static readonly int VirtualHeight = 900;
        private int _coins;

        public GameRoot()
        {
            _gdm = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = VirtualWidth,
                PreferredBackBufferHeight = VirtualHeight,
                SynchronizeWithVerticalRetrace = true
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Window.Title = "Dungeon Chef (FNA)";
        }

        protected override void Initialize()
        {
            base.Initialize();

            _sb = new SpriteBatch(GraphicsDevice);
            _world = new World();
            _hud = new Hud();

            StartNewRun();
        }

        private void StartNewRun()
        {
            _world.Entities.Clear();

            _currentRunSeed = NewRunSeed();

            _levelGen = new LevelGenerator(_currentRunSeed);
            var rooms = _levelGen.GenerateFloor();
            _roomController = new RoomController(rooms);
            _minimap = new Minimap(_roomController);
            _interactPrompt = new InteractPrompt();

            // Spawn player in middle of room space
            var startPos = new Vector2(4.5f, 4.5f);
            var player = _world.CreateEntity(startPos);
            player.IsPlayer = true;

            // Spawn enemies for starting room? For now, skip start.
            SpawnEnemiesForCurrentRoom();
            SpawnPickupsForCurrentRoom();

            CenterCameraOn(player.Position);
        }

        private static int NewRunSeed()
        {
            unchecked
            {
                return Environment.TickCount ^ Guid.NewGuid().GetHashCode();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Escape))
                Exit();

            // Optional: F5 for new run
            if (kb.IsKeyDown(Keys.F5))
            {
                StartNewRun();
                base.Update(gameTime);
                return;
            }

            _input.Update();
            _movementSystem.Update(_world, gameTime, _input.State);
            _enemySystem.Update(_world, gameTime);
            var currentRoom = _roomController?.CurrentRoom;
            _combatSystem.Update(_world, gameTime, _camera, currentRoom!);

            UpdateRoomClearState();

            HandlePickups();

            var player = _world.Entities.FirstOrDefault(e => e.IsPlayer);
            if (player != null)
            {
                _camera.UpdateFollow(gameTime, player.Position);
            }

            HandleDoorTransitions();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(18, 18, 22));

            // WORLD
            _sb.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: Matrix.Identity
            );

            IsoRenderer.DrawWorld(_sb, _camera, _world);
            DrawDoors(_sb);

            // 🔹 draw attack slash on top of world
            _combatSystem.DrawDebug(_sb, _camera);

            _sb.End();

            // UI / MINIMAP
            _sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: Matrix.Identity);

            var uiArea = new Rectangle(0, 0, VirtualWidth, VirtualHeight);
            _minimap.Draw(_sb, uiArea);

            _interactPrompt.Draw(_sb);
            _hud.Draw(_sb, _world, _coins);

            _sb.End();

            base.Draw(gameTime);
        }

        // ------------------------
        // ROOM / DOOR HANDLING
        // ------------------------

        private void HandleDoorTransitions()
        {
            var player = _world.Entities.FirstOrDefault(e => e.IsPlayer);
            if (player == null || _roomController == null)
            {
                _interactPrompt.SetPrompt(false, Vector2.Zero);
                return;
            }

            var room = _roomController.CurrentRoom;
            if (room == null)
            {
                _interactPrompt.SetPrompt(false, Vector2.Zero);
                return;
            }

            // 🔒 Doors are locked in ANY room while enemies are alive
            bool enemiesAliveHere = _world.Entities.Any(e => e.IsEnemy);
            if (enemiesAliveHere)
            {
                _interactPrompt.SetPrompt(false, Vector2.Zero);
                return;
            }

            // No enemies → doors are unlocked; prompt appears only in door trigger
            _interactPrompt.SetPrompt(false, Vector2.Zero);

            var playerPos = player.Position;
            var playerPoint = new Point((int)playerPos.X, (int)playerPos.Y);
            var kb = Keyboard.GetState();

            foreach (var door in _roomController.CurrentDoors)
            {
                if (!door.TriggerWorld.Contains(playerPoint))
                    continue;

                // Door world center → screen for prompt
                var centerWorld = new Vector2(
                    door.TriggerWorld.X + door.TriggerWorld.Width / 2f,
                    door.TriggerWorld.Y + door.TriggerWorld.Height / 2f
                );

                var doorIso = IsoMath.ToScreen(centerWorld.X, centerWorld.Y);
                var doorScreen = doorIso - _camera.Position;

                var promptScreenPos = doorScreen + new Vector2(0f, -24f);

                // Show "E" prompt above this door
                _interactPrompt.SetPrompt(true, promptScreenPos);

                // Only transition if E is pressed while in the door trigger
                if (kb.IsKeyDown(Keys.E))
                {
                    TransitionToRoom(door);
                }

                break; // only handle one door at a time
            }
        }


        private void TransitionToRoom(RoomDoor door)
        {
            if (_roomController == null)
                return;

            bool ok = _roomController.TryMoveTo(door.TargetRoom);
            if (!ok)
                return;

            _world.Entities.Clear();

            var player = _world.CreateEntity(door.NewRoomSpawn);
            player.IsPlayer = true;

            SpawnEnemiesForCurrentRoom();
            SpawnPickupsForCurrentRoom();

            CenterCameraOn(player.Position);
        }

        private void SpawnEnemiesForCurrentRoom()
        {
            if (_roomController == null)
                return;

            var room = _roomController.CurrentRoom;
            if (room == null)
                return;

            // No enemies in Start or Boss rooms
            if (room.Type == RoomType.Start || room.Type == RoomType.Boss)
                return;

            // If this room was cleared before, do NOT respawn enemies
            if (room.Cleared)
                return;

            // If we already spawned enemies once and player left mid-fight,
            // do not spawn a second wave on re-entry.
            if (room.EnemiesSpawned)
                return;

            // Simple seeded RNG so same run has deterministic layout per room
            int seed = _currentRunSeed ^ room.GridPos.GetHashCode();
            var rng = new Random(seed);

            int enemyCount = rng.Next(2, 5); // 2–4 enemies per room

            for (int i = 0; i < enemyCount; i++)
            {
                float x = 1f + (float)rng.NextDouble() * 7f; // 1..8
                float y = 1f + (float)rng.NextDouble() * 7f;

                var enemy = _world.CreateEntity(new Vector2(x, y));
                enemy.IsEnemy = true;
                enemy.HP = 5f;
            }

            room.EnemiesSpawned = true;
        }


        private void CenterCameraOn(Vector2 worldPos)
        {
            var playerIso = IsoMath.ToScreen(worldPos.X, worldPos.Y);
            var screenCenter = new Vector2(VirtualWidth / 2f, VirtualHeight / 2f);
            _camera.Position = playerIso - screenCenter;
        }

        private void DrawDoors(SpriteBatch sb)
        {
            if (_roomController == null || _roomController.CurrentDoors == null)
                return;

            var tex = IsoRenderer.DummyHero;
            if (tex == null)
                return;

            foreach (var door in _roomController.CurrentDoors)
            {
                var centerWorld = new Vector2(
                    door.TriggerWorld.X + door.TriggerWorld.Width / 2f,
                    door.TriggerWorld.Y + door.TriggerWorld.Height / 2f
                );

                var screenPos = IsoMath.ToScreen(centerWorld.X, centerWorld.Y) - _camera.Position;

                sb.Draw(
                    tex,
                    screenPos,
                    sourceRectangle: null,
                    color: Color.Goldenrod,
                    rotation: 0f,
                    origin: new Vector2(tex.Width / 2f, tex.Height),
                    scale: 0.6f,
                    effects: SpriteEffects.None,
                    layerDepth: 0.4f
                );
            }
        }

        private void UpdateRoomClearState()
        {
            if (_roomController == null)
                return;

            var room = _roomController.CurrentRoom;
            if (room == null)
                return;

            // Only care about rooms that actually have enemies
            if (!room.EnemiesSpawned || room.Cleared)
                return;

            bool anyEnemiesAlive = _world.Entities.Any(e => e.IsEnemy);
            if (!anyEnemiesAlive)
            {
                room.Cleared = true;
                // (Optional: trigger door opening VFX/SFX here later)
            }
        }

        private void SpawnPickupsForCurrentRoom()
        {
            if (_roomController == null)
                return;

            var room = _roomController.CurrentRoom;
            if (room == null)
                return;

            foreach (var p in room.Pickups)
            {
                var pickup = _world.CreateEntity(p.Position);
                pickup.IsPickup = true;
                pickup.ItemId = p.ItemId;
                pickup.Speed = 0f;
            }
        }

        private void HandlePickups()
        {
            if (_roomController == null)
                return;

            var room = _roomController.CurrentRoom;
            if (room == null)
                return;

            var player = _world.Entities.FirstOrDefault(e => e.IsPlayer);
            if (player == null)
                return;

            const float pickupRange = 0.6f;
            float pickupRangeSq = pickupRange * pickupRange;

            var pickupsToRemove = new List<Entity>();

            foreach (var e in _world.Entities)
            {
                if (!e.IsPickup)
                    continue;

                Vector2 toPickup = e.Position - player.Position;
                if (toPickup.LengthSquared() > pickupRangeSq)
                    continue;

                // We are close enough to pick this up
                if (!string.IsNullOrEmpty(e.ItemId))
                {
                    var def = ItemCatalog.GetById(e.ItemId);
                    if (def != null)
                    {
                        switch (def.Type)
                        {
                            case "Currency":
                                _coins += def.Value;
                                // Debug: you can log or print this for now
                                Serilog.Log.Information("Picked up {ItemId}, coins now = {Coins}", def.Id, _coins);
                                break;

                            case "Consumable":
                                // For now, simple heal on player.HP (if you want that)
                                player.HP += def.Heal;
                                // You can clamp to a max later (e.g., 10f)
                                Serilog.Log.Information("Picked up {ItemId}, healed {Heal}", def.Id, def.Heal);
                                break;

                            default:
                                // Unknown type, still remove it from world/state
                                Serilog.Log.Information("Picked up {ItemId} (unknown type {Type})", def.Id, def.Type);
                                break;
                        }
                    }
                }

                pickupsToRemove.Add(e);
            }

            if (pickupsToRemove.Count == 0)
                return;

            // Remove from world
            foreach (var ent in pickupsToRemove)
            {
                _world.Entities.Remove(ent);
            }

            // Remove from room.Pickups so they don't respawn next time
            foreach (var ent in pickupsToRemove)
            {
                if (string.IsNullOrEmpty(ent.ItemId))
                    continue;

                // Find the first matching pickup in this room by id + position
                var match = room.Pickups.FirstOrDefault(p =>
                    p.ItemId == ent.ItemId &&
                    Vector2.DistanceSquared(p.Position, ent.Position) < 0.0001f
                );

                if (match != null)
                {
                    room.Pickups.Remove(match);
                }
            }
        }
    }
}
