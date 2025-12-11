using System;
using System.IO;
using DungeonChef.Src.ECS;
using DungeonChef.Src.Gameplay.Rooms;
using Microsoft.Xna.Framework;
using DungeonChef.Src.Gameplay;
using System.ComponentModel; // For EnemyFactory

namespace DungeonChef.Src.Gameplay
{
    public sealed class RoomEnemySpawner
    {
        private readonly RoomController _roomController;
        private readonly Func<int> _seedProvider;

        public RoomEnemySpawner(RoomController roomController, Func<int> seedProvider)
        {
            _roomController = roomController;
            _seedProvider = seedProvider;
        }

        public void SpawnEnemiesForCurrentRoom(World world)
        {
            if (_roomController == null)
                return;

            var room = _roomController.CurrentRoom;
            if (room == null)
                return;

            if (room.Type == RoomType.Start || room.Type == RoomType.Boss)
                return;

            if (room.Cleared)
                return;

            if (room.EnemiesSpawned)
                return;

            int seed = _seedProvider() ^ room.GridPos.GetHashCode();
            var rng = new Random(seed);

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string jsonPath = Path.Combine(baseDir, "content", "Data", "enemies.json");
            EnemyFactory.Load(jsonPath);

            int enemyCount = rng.Next(2, 5);

            for (int i = 0; i < enemyCount; i++)
            {
                float x = 1f + (float)rng.NextDouble() * 7f;
                float y = 1f + (float)rng.NextDouble() * 7f;

                var def = EnemyFactory.GetRandom(rng);
                world.CreateEnemy(new Vector2(x, y), def);
            }

            room.EnemiesSpawned = true;
        }
    }
}
