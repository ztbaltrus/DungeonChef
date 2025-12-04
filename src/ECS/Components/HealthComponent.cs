namespace DungeonChef.Src.ECS.Components
{
    public sealed class HealthComponent : IComponent
    {
        public float Current;
        public float Max;

        public float Normalized => Max <= 0f ? 0f : Current / Max;
        public bool IsDead => Current <= 0f;

        public void Heal(float amount)
        {
            Current = System.MathF.Min(Max, Current + amount);
        }

        public void Damage(float amount)
        {
            Current = System.MathF.Max(0f, Current - amount);
        }
    }
}
