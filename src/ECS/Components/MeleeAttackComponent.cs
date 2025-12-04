namespace DungeonChef.Src.ECS.Components
{
    public sealed class MeleeAttackComponent : IComponent
    {
        public MeleeAttackComponent(float damage, float range, float angleDegrees, float cooldownSeconds)
        {
            Damage = damage;
            Range = range;
            AngleDegrees = angleDegrees;
            CooldownSeconds = cooldownSeconds;
        }

        public float Damage { get; }
        public float Range { get; }
        public float AngleDegrees { get; }
        public float CooldownSeconds { get; }
        public float CooldownTimer { get; set; }
    }
}
