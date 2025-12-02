using DungeonChef.Src.Rendering;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.ECS
{
    public class Entity
    {
        public Vector2 Grid;        // logical grid position
        public Vector2 Position;    // world space (optional)
        public Vector2 Velocity;

        public float Radius = 0.3f;
        public float HP = 100f;
        public float MaxHP = 100f;

        // Simple collision box in world/grid space
        public Rectangle Bounds;

        // Animation
        public SpriteAnimator? Animator;
    }
}