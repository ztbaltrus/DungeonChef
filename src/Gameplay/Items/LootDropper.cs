using DungeonChef.Src.ECS;
using DungeonChef.Src.Gameplay.Rooms;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Gameplay.Items
{
    public static class LootDropper
    {
        // rng can be Random.Shared for now; you can swap to run-seeded RNG later
        private static readonly System.Random _rng = new System.Random();

        public static void TryDropLoot(World world, Vector2 position, RoomNode room)
        {
            var def = ItemCatalog.RollItem(_rng);
            if (def == null)
                return;

            // Spawn pickup entity
            var pickup = world.CreateEntity(position);
            pickup.IsPickup = true;
            pickup.ItemId = def.Id;
            pickup.Speed = 0f;

            // Persist it on the room
            room.Pickups.Add(new RoomPickupState
            {
                ItemId = def.Id,
                Position = position
            });
        }
    }
}
