using DungeonChef.Src.Core;
using DungeonChef.Src.ECS;
using DungeonChef.Src.ECS.Components;
using DungeonChef.Src.Rendering;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Gameplay
{
    public sealed class MovementSystem
    {
        public static MovementSystem Instance { get; } = new MovementSystem();

        private MovementSystem()
        {
        }

        private const float WorldMinX = 0f;
        private const float WorldMinY = 0f;
        private const float WorldMaxX = 9f;
        private const float WorldMaxY = 9f;

        public void Update(World world, GameTime gt, InputState input)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            Vector2 exWorld = Vector2.Normalize(new Vector2(1f, -1f));
            Vector2 eyWorld = Vector2.Normalize(new Vector2(1f, 1f));

            Vector2 exScreen = ToScreen(exWorld);
            Vector2 eyScreen = ToScreen(eyWorld);

            float exLen = exScreen.Length();
            float eyLen = eyScreen.Length();

            if (eyLen > 0.0001f)
            {
                float scale = exLen / eyLen;
                eyWorld *= scale;
            }

            foreach (var entity in world.With<MovementComponent, TransformComponent>())
            {
                var movement = entity.GetComponent<MovementComponent>();
                if (movement.Behavior != MovementBehavior.PlayerInput)
                    continue;

                var transform = entity.GetComponent<TransformComponent>();

                Vector2 move = input.Move;
                Vector2 worldDelta = Vector2.Zero;

                movement.IsMoving = move.LengthSquared() > 0f;

                if (movement.IsMoving)
                {
                    move.Normalize();
                    worldDelta = move.X * exWorld + move.Y * eyWorld;
                    transform.Position += worldDelta * movement.Speed * dt;
                    transform.Position = new Vector2(
                        MathHelper.Clamp(transform.Position.X, WorldMinX, WorldMaxX),
                        MathHelper.Clamp(transform.Position.Y, WorldMinY, WorldMaxY));
                    transform.Velocity = worldDelta * movement.Speed;
                }
                else
                {
                    transform.Velocity = Vector2.Zero;
                }

                transform.Grid = transform.Position;

                if (entity.TryGetComponent(out AnimationComponent animation))
                {
                    ApplyMovementAnimation(animation, transform.Velocity);
                }
            }
        }

        private static void ApplyMovementAnimation(AnimationComponent animation, Vector2 velocity)
        {
            if (velocity.LengthSquared() <= float.Epsilon)
            {
                animation.Controller.PlayAnimation("Idle");
                return;
            }

            float vx = velocity.X;
            float vy = velocity.Y;

            if (vx > 0 && vy < 0)
                animation.Controller.PlayAnimation("WalkUpRight");
            else if (vx > 0 && vy > 0)
                animation.Controller.PlayAnimation("WalkDownRight");
            else if (vx < 0 && vy > 0)
                animation.Controller.PlayAnimation("WalkDownLeft");
            else if (vx < 0 && vy < 0)
                animation.Controller.PlayAnimation("WalkUpLeft");
            else if (vx > 0)
                animation.Controller.PlayAnimation("WalkRight");
            else if (vx < 0)
                animation.Controller.PlayAnimation("WalkLeft");
            else if (vy < 0)
                animation.Controller.PlayAnimation("WalkUp");
            else
                animation.Controller.PlayAnimation("WalkDown");
        }

        private static Vector2 ToScreen(Vector2 world)
        {
            float sx = (world.X - world.Y) * (IsoMath.TileW / 2f);
            float sy = (world.X + world.Y) * (IsoMath.TileH / 2f);
            return new Vector2(sx, sy);
        }
    }
}
