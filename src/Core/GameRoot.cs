using System;
using System.Linq;
using DungeonChef.Src.ECS;
using DungeonChef.Src.ECS.Components;
using DungeonChef.Src.Entities;
using DungeonChef.Src.Gameplay;
using DungeonChef.Src.Gameplay.Rooms;
using DungeonChef.Src.Rendering;
using DungeonChef.Src.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonChef.Src.Core
{
    public sealed class GameRoot : Game
    {
        private readonly GraphicsDeviceManager _gdm;
        private SpriteBatch _sb = null!;

        private readonly IsoCamera _camera = new();
        private readonly Input _input = new();
        private readonly MovementSystem _movementSystem = MovementSystem.Instance;
        private readonly EnemySystem _enemySystem = EnemySystem.Instance;
        private readonly CombatSystem _combatSystem = CombatSystem.Instance;
        private readonly AnimationSystem _animationSystem = AnimationSystem.Instance;
        private RoomEnemySpawner _roomEnemySpawner = null!;

        private World _world = null!;

        private LevelGenerator _levelGen = null!;
        private RoomController _roomController = null!;
        private Minimap _minimap = null!;
        private InteractPrompt _interactPrompt = null!;
        private Hud _hud = null!;

        private int _currentRunSeed;
        private Texture2D _playerTexture = null!;

        public static readonly int VirtualWidth = 1600;
        public static readonly int VirtualHeight = 900;
        private int _coins;

        // ---------------------------------------------------------------------
        // Game state handling (start screen vs running)
        // ---------------------------------------------------------------------
        private enum GameState { StartScreen, Running }
        private GameState _state = GameState.StartScreen;
        private UI.StartScreen? _startScreen;

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

            Window.Title = "Dungeon Chef (FNA)";
        }

        protected override void Initialize()
        {
            base.Initialize();

            _sb = new SpriteBatch(GraphicsDevice);
            _world = new World();
            _hud = new Hud();

            // Initialise start screen UI (covers the whole window)
            _startScreen = new UI.StartScreen(GraphicsDevice);
            _state = GameState.StartScreen;
        }

        protected override void LoadContent()
        {
            // Load the player texture here
            _playerTexture = Content.Load<Texture2D>("Sprites/Player/player_spritesheet");
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
            _roomEnemySpawner = new RoomEnemySpawner(_roomController, () => _currentRunSeed);

            // Spawn player in middle of room space
            var startPos = new Vector2(4.5f, 4.5f);
            var player = _world.CreatePlayer(startPos);

            player.LoadPlayerAnimations(_playerTexture);

            // Spawn enemies and pickups for starting room
            _roomEnemySpawner.SpawnEnemiesForCurrentRoom(_world);
            PickupSystem.Instance.SpawnPickupsForRoom(_world, _roomController);

            var transform = player.GetComponent<TransformComponent>();
            CenterCameraOn(transform.Position);
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

            if (_state == GameState.StartScreen)
            {
                // Press Enter to begin the game
                if (kb.IsKeyDown(Keys.Enter))
                {
                    _state = GameState.Running;
                    StartNewRun();
                }
                // Skip the rest of the update loop while on the start screen
                base.Update(gameTime);
                return;
            }

            // Optional: F5 for new run (debug)
            if (kb.IsKeyDown(Keys.F5))
            {
                StartNewRun();
                base.Update(gameTime);
                return;
            }

            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _input.Update();
            _movementSystem.Update(_world, gameTime, _input.State);
            _enemySystem.Update(_world, gameTime);
            _animationSystem.Update(_world, deltaSeconds);

            var currentRoom = _roomController?.CurrentRoom;
            if (currentRoom != null)
            {
                _combatSystem.Update(_world, gameTime, _camera, currentRoom);
            }

            RoomClearSystem.Instance.Update(_world, _roomController);
            PickupSystem.Instance.Update(_world, _roomController, ref _coins);

            var player = _world.FindWith<PlayerTagComponent>();
            if (player != null)
            {
                var transform = player.GetComponent<TransformComponent>();
                _camera.UpdateFollow(gameTime, transform.Position);
            }

            HandleDoorTransitions();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(18, 18, 22));

            if (_state == GameState.StartScreen)
            {
                // Draw only the start screen UI
                _sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                    depthStencilState: null,
                    rasterizerState: null,
                    effect: null,
                    transformMatrix: Matrix.Identity);
                _startScreen?.Draw(_sb, VirtualWidth, VirtualHeight);
                _sb.End();
                // Remove the base.Draw(gameTime) call here - it was causing issues
                return;
            }

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
            _combatSystem.DrawDebug(_sb, _camera, _world);

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
        }

        // ------------------------
        // ROOM / DOOR HANDLING
        // ------------------------

        private void HandleDoorTransitions()
        {
            var player = _world.FindWith<PlayerTagComponent>();
            if (player == null || _roomController == null)
            {
                _interactPrompt.SetPrompt(false, Vector2.Zero);
                return;
            }

            var playerTransform = player.GetComponent<TransformComponent>();
            var room = _roomController.CurrentRoom;
            if (room == null)
            {
                _interactPrompt.SetPrompt(false, Vector2.Zero);
                return;
            }

            // 🔒 Doors are locked in ANY room while enemies are alive
            bool enemiesAliveHere = _world.With<EnemyComponent>().Any();
            if (enemiesAliveHere)
            {
                _interactPrompt.SetPrompt(false, Vector2.Zero);
                return;
            }

            // No enemies → doors are unlocked; prompt appears only in door trigger
            _interactPrompt.SetPrompt(false, Vector2.Zero);

            var playerPos = playerTransform.Position;
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

            // Preserve the existing player entity (and its HP) across rooms.
            var existingPlayer = _world.FindWith<PlayerTagComponent>();
            if (existingPlayer == null)
                return;

            bool ok = _roomController.TryMoveTo(door.TargetRoom);
            if (!ok)
                return;

            // Remove all non-player entities (enemies, pickups, etc.) but keep the player.
            _world.Entities.RemoveAll(e => !e.HasComponent<PlayerTagComponent>());

            // Move the existing player to the new room's spawn position.
            var transform = existingPlayer.GetComponent<TransformComponent>();
            transform.Position = door.NewRoomSpawn;
            transform.Grid = door.NewRoomSpawn;
            transform.Velocity = Vector2.Zero;

            _roomEnemySpawner.SpawnEnemiesForCurrentRoom(_world);
            PickupSystem.Instance.SpawnPickupsForRoom(_world, _roomController);

            CenterCameraOn(transform.Position);
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

    }
}
