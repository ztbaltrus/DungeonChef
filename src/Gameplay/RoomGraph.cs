using System.Collections.Generic;
using DungeonChef.Src.Gameplay.Rooms;

namespace DungeonChef.Src.Gameplay
{
    public enum RoomType
    {
        Start,
        Combat,
        CookStation,
        Treasure,
        Shop,
        Event,
        MiniBoss,
        Boss,
    }
    public sealed class RoomGraph
    {
        public readonly int Seed;
        public List<RoomNode> Nodes { get; } = new();

        public RoomGraph(int seed)
        {
            Seed = seed;
            GenerateLinearPath(6);
        }

        private void GenerateLinearPath(int rooms)
        {
            for (int i = 0; i < rooms; i++)
            {
                var type = RoomType.Combat;
                if (i == rooms - 1)
                    type = RoomType.MiniBoss;
                else if (i == 2)
                    type = RoomType.CookStation;

                Nodes.Add(new RoomNode { Id = i, Type = type });
            }
        }
    }
}
