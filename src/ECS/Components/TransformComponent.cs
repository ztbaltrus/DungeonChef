using Microsoft.Xna.Framework;

namespace DungeonChef.Src.ECS.Components
{
    public sealed class TransformComponent : IComponent
    {
        public Vector2 Position;
        public Vector2 Grid;
        public Vector2 Velocity;
        public float Radius = 0.3f;
    }
}
