using System.Linq;
using DungeonChef.Src.ECS;
using DungeonChef.Src.ECS.Components;
using DungeonChef.Src.Gameplay.Rooms;

namespace DungeonChef.Src.Gameplay
{
    public sealed class RoomClearSystem
    {
        public static RoomClearSystem Instance { get; } = new RoomClearSystem();

        private RoomClearSystem()
        {
        }

        public void Update(World world, RoomController roomController)
        {
            var room = roomController?.CurrentRoom;
            if (room == null || !room.EnemiesSpawned || room.Cleared)
                return;

            bool anyEnemiesAlive = world.With<EnemyComponent>().Any();
            if (!anyEnemiesAlive)
            {
                room.Cleared = true;
            }
        }
    }
}
