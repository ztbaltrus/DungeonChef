using System.Collections.Generic;
using System.Linq;
using DungeonChef.Src.ECS;
using DungeonChef.Src.Entities;
using DungeonChef.Src.Gameplay.Items;
using DungeonChef.Src.Gameplay.Rooms;
using Microsoft.Xna.Framework;
using Serilog;

namespace DungeonChef.Src.Gameplay
{
    public sealed class PickupSystem
    {
        private readonly RoomController _roomController;

        public PickupSystem(RoomController roomController)
        {
            _roomController = roomController;
        }

        public void SpawnPickupsForCurrentRoom(World world)
        {
            if (_roomController == null)
                return;

            var room = _roomController.CurrentRoom;
            if (room == null)
                return;

            foreach (var p in room.Pickups)
            {
                var loot = world.CreateLoot(p.Position, p.ItemId);
            }
        }

        public void Update(World world, ref int coins)
        {
            if (_roomController == null)
                return;

            var room = _roomController.CurrentRoom;
            if (room == null)
                return;

            var player = world.Entities.FirstOrDefault(e => e.GetType() == typeof(Player));
            if (player == null)
                return;

            const float pickupRange = 0.6f;
            float pickupRangeSq = pickupRange * pickupRange;

            var pickupsToRemove = new List<Entity>();

            foreach (var e in world.Entities)
            {
                if (e.GetType() != typeof(Loot) && e != null)
                    continue;

                Loot loot = e as Loot;
                Vector2 toPickup = loot.Position - player.Position;
                if (toPickup.LengthSquared() > pickupRangeSq)
                    continue;

                if (!string.IsNullOrEmpty(loot.ItemId))
                {
                    var def = ItemCatalog.GetById(loot.ItemId);
                    if (def != null)
                    {
                        switch (def.Type)
                        {
                            case "Currency":
                                coins += def.Value;
                                Log.Information("Picked up {ItemId}, coins now = {Coins}", def.Id, coins);
                                break;

                            case "Consumable":
                                player.HP += def.Heal;
                                Log.Information("Picked up {ItemId}, healed {Heal}", def.Id, def.Heal);
                                break;

                            default:
                                Log.Information("Picked up {ItemId} (unknown type {Type})", def.Id, def.Type);
                                break;
                        }
                    }
                }

                pickupsToRemove.Add(loot);
            }

            if (pickupsToRemove.Count == 0)
                return;

            foreach (var ent in pickupsToRemove)
            {
                world.Entities.Remove(ent);
            }

            foreach (var ent in pickupsToRemove)
            {
                Loot loot = ent as Loot;
                if (string.IsNullOrEmpty(loot.ItemId))
                    continue;

                var match = room.Pickups.FirstOrDefault(p =>
                    p.ItemId == loot.ItemId &&
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

