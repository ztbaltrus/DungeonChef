namespace DungeonChef.Src.ECS.Components
{
    public enum MovementBehavior
    {
        None,
        PlayerInput,
        SeekTarget
    }

    public sealed class MovementComponent : IComponent
    {
        public MovementComponent(MovementBehavior behavior, float speed)
        {
            Behavior = behavior;
            Speed = speed;
        }

        public MovementBehavior Behavior { get; }
        public float Speed { get; }
        public bool IsMoving { get; set; }
    }
}
