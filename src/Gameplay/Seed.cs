using DungeonChef.Src.ECS;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Gameplay
{
    public static class Seed
    {
        public static void SpawnTestPlayer(World world)
        {
            // Center of the 10x10 world (0..9)
            var center = new Vector2(4.5f, 4.5f);

            var e = world.CreatePlayer(center);
            var transform = e.GetComponent<Src.ECS.Components.TransformComponent>();
            transform.Position = center;
            transform.Grid = center;
        }
    }
}
