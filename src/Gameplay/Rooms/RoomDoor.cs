using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Gameplay.Rooms
{
    public sealed class RoomDoor
    {
        public required RoomNode TargetRoom;
        public Rectangle TriggerWorld; // world-space AABB
        public Vector2 NewRoomSpawn;   // where player appears in target room
    }
}
