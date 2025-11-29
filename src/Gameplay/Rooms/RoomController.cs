using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Gameplay.Rooms
{
    public sealed class RoomController
    {
        public Dictionary<Point, RoomNode> Rooms;
        public RoomNode CurrentRoom;

        public List<RoomDoor> CurrentDoors { get; private set; } = new();

        public RoomController(Dictionary<Point, RoomNode> rooms)
        {
            Rooms = rooms;
            CurrentRoom = GetStart();
            CurrentRoom.Visited = true;
            BuildDoorsForCurrentRoom();
        }

        public bool TryMoveTo(RoomNode target)
        {
            if (!CurrentRoom.Neighbors.Contains(target))
                return false;

            target.Discovered = true;
            target.Visited = true;
            CurrentRoom = target;

            BuildDoorsForCurrentRoom();
            return true;
        }

        public RoomNode GetStart()
        {
            foreach (var r in Rooms.Values)
                if (r.Type == RoomType.Start) return r;
            return null!;
        }

        private void BuildDoorsForCurrentRoom()
        {
            CurrentDoors.Clear();

            var cur = CurrentRoom;
            foreach (var neighbor in cur.Neighbors)
            {
                // When you're in a room, its neighbors become "discovered" on the minimap
                neighbor.Discovered = true;

                var delta = neighbor.GridPos - cur.GridPos;

                RoomDoor door = new RoomDoor
                {
                    TargetRoom = neighbor
                };

                const int doorWidth = 2;
                const int doorHeight = 2;

                if (delta.X == 1 && delta.Y == 0)
                {
                    int x = 9;
                    int y = 4;
                    door.TriggerWorld = new Rectangle(x, y, doorWidth, doorHeight);
                    door.NewRoomSpawn = new Vector2(1f, 4.5f);
                }
                else if (delta.X == -1 && delta.Y == 0)
                {
                    int x = 0;
                    int y = 4;
                    door.TriggerWorld = new Rectangle(x - doorWidth + 1, y, doorWidth, doorHeight);
                    door.NewRoomSpawn = new Vector2(8f, 4.5f);
                }
                else if (delta.X == 0 && delta.Y == -1)
                {
                    int x = 4;
                    int y = 0;
                    door.TriggerWorld = new Rectangle(x, y - doorHeight + 1, doorWidth, doorHeight);
                    door.NewRoomSpawn = new Vector2(4.5f, 8f);
                }
                else if (delta.X == 0 && delta.Y == 1)
                {
                    int x = 4;
                    int y = 9;
                    door.TriggerWorld = new Rectangle(x, y, doorWidth, doorHeight);
                    door.NewRoomSpawn = new Vector2(4.5f, 1f);
                }
                else
                {
                    continue;
                }

                CurrentDoors.Add(door);
            }
        }
    }
}
