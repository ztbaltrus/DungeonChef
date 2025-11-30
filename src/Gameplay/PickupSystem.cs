using System.Collections.Generic;
using System.Linq;
using DungeonChef.Src.ECS;
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
                var pickup = world.CreateEntity(p.Position);
                pickup.IsPickup = true;
                pickup.ItemId = p.ItemId;
                pickup.Speed = 0f;
            }
        }

        public void Update(World world, ref int coins)
        {
            if (_roomController == null)
                return;

            var room = _roomController.CurrentRoom;
            if (room == null)
                return;

            var player = world.Entities.FirstOrDefault(e => e.IsPlayer);
            if (player == null)
                return;

            const float pickupRange = 0.6f;
            float pickupRangeSq = pickupRange * pickupRange;

            var pickupsToRemove = new List<Entity>();

            foreach (var e in world.Entities)
            {
                if (!e.IsPickup)
                    continue;

                Vector2 toPickup = e.Position - player.Position;
                if (toPickup.LengthSquared() > pickupRangeSq)
                    continue;

                if (!string.IsNullOrEmpty(e.ItemId))
                {
                    var def = ItemCatalog.GetById(e.ItemId);
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

                pickupsToRemove.Add(e);
            }

            if (pickupsToRemove.Count == 0)
                return;

            foreach (var ent in pickupsToRemove)
            {
                world.Entities.Remove(ent);
            }

            foreach (var ent in pickupsToRemove)
            {
                if (string.IsNullOrEmpty(ent.ItemId))
                    continue;

                var match = room.Pickups.FirstOrDefault(p =>
                    p.ItemId == ent.ItemId &&
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

