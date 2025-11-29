using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DungeonChef.Src.Gameplay.Rooms
{
    public class RoomNode
    {
        public int Id;
        public Point GridPos;                   // position in room-grid
        public RoomType Type;
        public bool Discovered = false;         // appears on minimap
        public bool Visited = false;            // player entered
        public List<RoomNode> Neighbors = new();
        public bool EnemiesSpawned = false;  // did we ever spawn enemies here?
        public bool Cleared = false;
        public List<RoomPickupState> Pickups { get; } = new();
    }
}
