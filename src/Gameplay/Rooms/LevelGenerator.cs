using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonChef.Src.Gameplay.Rooms
{
    public sealed class LevelGenerator
    {
        private readonly Random _rng;
        private const int GridSize = 11; // 11×11 room grid

        public LevelGenerator(int seed)
        {
            _rng = new Random(seed);
        }

        public Dictionary<Point, RoomNode> GenerateFloor()
        {
            var rooms = new Dictionary<Point, RoomNode>();

            Point start = new(GridSize / 2, GridSize / 2);
            var startRoom = new RoomNode { GridPos = start, Type = RoomType.Start, Discovered = true };
            rooms[start] = startRoom;

            // Build a simple random walk path first
            Point current = start;
            int steps = _rng.Next(6, 10); // number of rooms in the floor

            for (int i = 0; i < steps; i++)
            {
                var dirs = new[]
                {
                    new Point(1,0),
                    new Point(-1,0),
                    new Point(0,1),
                    new Point(0,-1)
                };

                Point d = dirs[_rng.Next(dirs.Length)];
                Point next = new(current.X + d.X, current.Y + d.Y);

                // Clamp inside grid
                if (next.X < 0 || next.X >= GridSize || next.Y < 0 || next.Y >= GridSize)
                    continue;

                if (!rooms.ContainsKey(next))
                {
                    rooms[next] = new RoomNode
                    {
                        GridPos = next,
                        Type = RoomType.Combat
                    };
                }

                current = next;
            }

            // Add neighbor connections
            foreach (var kvp in rooms)
            {
                var pos = kvp.Key;
                var rn = kvp.Value;

                var neighbors = new[]
                {
                    new Point(pos.X + 1, pos.Y),
                    new Point(pos.X - 1, pos.Y),
                    new Point(pos.X, pos.Y + 1),
                    new Point(pos.X, pos.Y - 1),
                };

                foreach (var n in neighbors)
                {
                    if (rooms.TryGetValue(n, out var nn))
                    {
                        if (!rn.Neighbors.Contains(nn))
                            rn.Neighbors.Add(nn);
                    }
                }
            }

            AssignSpecialRooms(rooms);

            return rooms;
        }

        private void AssignSpecialRooms(Dictionary<Point, RoomNode> rooms)
        {
            var start = GetStart(rooms);

            // All non-start rooms
            var candidates = rooms.Values
                                  .Where(r => r != start)
                                  .ToList();

            if (!candidates.Any())
                return;

            // --- 1) BOSs: farthest room from start ---
            var boss = candidates
                .OrderByDescending(r => Manhattan(r.GridPos, start.GridPos))
                .First();

            boss.Type = RoomType.Boss;

            // Remove boss from pool for other guaranteed types
            candidates = candidates.Where(r => r != boss).ToList();

            // Ensure we have at least something left
            if (!candidates.Any())
                return;

            // --- 2) TREASURE: random non-boss, non-start ---
            var treasure = candidates[_rng.Next(candidates.Count)];
            treasure.Type = RoomType.Treasure;

            candidates = candidates.Where(r => r != treasure).ToList();

            // --- 3) COOK: random non-boss, non-treasure, non-start ---
            if (!candidates.Any())
            {
                // edge case: very tiny map, reuse a combat if we somehow ran out
                candidates = rooms.Values
                    .Where(r => r != start && r != boss && r != treasure)
                    .ToList();
            }

            RoomNode? cook = null;
            if (candidates.Any())
            {
                cook = candidates[_rng.Next(candidates.Count)];
                cook.Type = RoomType.CookStation;
                candidates = candidates.Where(r => r != cook).ToList();
            }

            // --- 4) Extra random specials on remaining combat rooms ---
            foreach (var rm in rooms.Values)
            {
                if (rm == start || rm == boss || rm == treasure || rm == cook)
                    continue;

                if (rm.Type != RoomType.Combat)
                    continue;

                int roll = _rng.Next(100);

                if (roll < 10)
                    rm.Type = RoomType.Shop;
                else if (roll < 20)
                    rm.Type = RoomType.CookStation;
                else if (roll < 30)
                    rm.Type = RoomType.Treasure;
            }
        }

        private static RoomNode GetStart(Dictionary<Point, RoomNode> rooms)
            => rooms.Values.First(r => r.Type == RoomType.Start);

        private static int Manhattan(Point a, Point b)
            => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
}
