using DungeonChef.Src.ECS;
using DungeonChef.Src.ECS.Components;

namespace DungeonChef.Src.Gameplay
{
    public sealed class AnimationSystem
    {
        public static AnimationSystem Instance { get; } = new AnimationSystem();

        private AnimationSystem()
        {
        }

        public void Update(World world, float deltaTime)
        {
            foreach (var entity in world.With<AnimationComponent>())
            {
                var animation = entity.GetComponent<AnimationComponent>();
                animation.Controller.Update(deltaTime);
            }
        }
    }
}
