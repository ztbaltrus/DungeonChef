namespace DungeonChef.Src.ECS.Components
{
    public sealed class EnemyComponent : IComponent
    {
        public EnemyComponent(string enemyId, float speed, float attackRange, float damage, float attackCooldown, float stopDistance)
        {
            EnemyId = enemyId;
            Speed = speed;
            AttackRange = attackRange;
            Damage = damage;
            AttackCooldown = attackCooldown;
            StopDistance = stopDistance;
        }

        public string EnemyId { get; }
        public float Speed { get; }
        public float AttackRange { get; }
        public float Damage { get; }
        public float AttackCooldown { get; }
        public float StopDistance { get; }

        public float AttackTimer { get; set; }
    }
}
