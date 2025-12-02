using System.Linq;
using DungeonChef.Src.ECS;
using DungeonChef.Src.Entities;
using DungeonChef.Src.Gameplay.Rooms;

namespace DungeonChef.Src.Gameplay
{
    public sealed class RoomClearSystem
    {
        private readonly RoomController _roomController;

        public RoomClearSystem(RoomController roomController)
        {
            _roomController = roomController;
        }

        public void Update(World world)
        {
            if (_roomController == null)
                return;

            var room = _roomController.CurrentRoom;
            if (room == null)
                return;

            if (!room.EnemiesSpawned || room.Cleared)
                return;

            bool anyEnemiesAlive = world.Entities.Any(e => e.GetType() == typeof(Enemy));
            if (!anyEnemiesAlive)
            {
                room.Cleared = true;
            }
        }
    }
}

