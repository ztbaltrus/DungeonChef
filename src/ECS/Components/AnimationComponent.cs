using DungeonChef.Src.Utils;

namespace DungeonChef.Src.ECS.Components
{
    public sealed class AnimationComponent : IComponent
    {
        public AnimationComponent(AnimationController controller)
        {
            Controller = controller;
        }

        public AnimationController Controller { get; }
        public string CurrentClip { get; set; } = "Idle";
    }
}
