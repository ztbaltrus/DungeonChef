using System.Collections.Generic;
using System.Linq;
using DungeonChef.Src.ECS;
using DungeonChef.Src.ECS.Components;
using DungeonChef.Src.Gameplay.Items;
using DungeonChef.Src.Gameplay.Rooms;
using Microsoft.Xna.Framework;
using Serilog;

namespace DungeonChef.Src.ECS.Systems
{
    public sealed class PickupSystem
    {
        public static PickupSystem Instance { get; } = new PickupSystem();

        private PickupSystem()
        {
        }

        public void SpawnPickupsForRoom(World world, RoomController roomController)
        {
            var room = roomController?.CurrentRoom;
            if (room == null)
                return;

            foreach (var pickup in room.Pickups)
            {
                world.CreateLoot(pickup.Position, pickup.ItemId);
            }
        }

        public void Update(World world, RoomController roomController, ref int coins)
        {
            var room = roomController?.CurrentRoom;
            if (room == null)
                return;

            var player = world.FindWith<PlayerTagComponent>();
            if (player == null)
                return;

            var playerTransform = player.GetComponent<TransformComponent>();
            var playerHealth = player.GetComponent<HealthComponent>();

            const float pickupRange = 0.6f;
            float pickupRangeSq = pickupRange * pickupRange;

            var pickupsToRemove = new List<Entity>();

            foreach (var entity in world.With<LootComponent, TransformComponent>())
            {
                var transform = entity.GetComponent<TransformComponent>();
                Vector2 toPickup = transform.Position - playerTransform.Position;
                if (toPickup.LengthSquared() > pickupRangeSq)
                    continue;

                var loot = entity.GetComponent<LootComponent>();
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
                            playerHealth.Heal(def.Heal);
                            Log.Information("Picked up {ItemId}, healed {Heal}", def.Id, def.Heal);
                            break;
                        default:
                            Log.Information("Picked up {ItemId} (type {Type})", def.Id, def.Type);
                            break;
                    }
                }

                pickupsToRemove.Add(entity);
            }

            if (pickupsToRemove.Count == 0)
                return;

            foreach (var ent in pickupsToRemove)
            {
                world.Entities.Remove(ent);
            }

            foreach (var ent in pickupsToRemove)
            {
                var loot = ent.GetComponent<LootComponent>();
                var transform = ent.GetComponent<TransformComponent>();
                var match = room.Pickups.FirstOrDefault(p =>
                    p.ItemId == loot.ItemId &&
                    Vector2.DistanceSquared(p.Position, transform.Position) < 0.0001f);

                if (match != null)
                {
                    room.Pickups.Remove(match);
                }
            }
        }
    }
}
