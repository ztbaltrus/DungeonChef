using DungeonChef.Src.Rendering;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.ECS
{
    public sealed class Entity
    {
        public Vector2 Grid;        // logical grid position
        public Vector2 Position;    // world space (optional)
        public Vector2 Velocity;
        public float Speed = 5f;

        public bool IsPlayer;
        public bool IsEnemy;

        public bool IsPickup;
        public string? ItemId;
        // Identifier for the enemy type (matches the "id" field in enemies.json)
        public string? EnemyId;

        public float Radius = 0.3f;
        public float HP = 100f;
        public float MaxHP = 100f;

        // Simple collision box in world/grid space
        public Rectangle Bounds;

        // Animation
        public SpriteAnimator? Animator;
    }
}
