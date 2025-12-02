using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonChef.Src.Utils;
using DungeonChef.Src.ECS;

namespace DungeonChef.Src.Entities
{
    public class Player : Entity
    {
        public AnimationController AnimationController { get; set; }
        public bool IsMoving { get; set; }

        public float Speed = 5f;

        public Player(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            AnimationController = new AnimationController();
        }

        public void Update(float deltaTime)
        {
            // Handle 8-way movement
            if (IsMoving)
            {
                // Example: handle 8 directions
                if (Velocity.X > 0 && Velocity.Y < 0) // Up-right
                    AnimationController.PlayAnimation("WalkUpRight");
                else if (Velocity.X > 0 && Velocity.Y > 0) // Down-right
                    AnimationController.PlayAnimation("WalkDownRight");
                else if (Velocity.X < 0 && Velocity.Y > 0) // Down-left
                    AnimationController.PlayAnimation("WalkDownLeft");
                else if (Velocity.X < 0 && Velocity.Y < 0) // Up-left
                    AnimationController.PlayAnimation("WalkUpLeft");
                else if (Velocity.X > 0) // Right
                    AnimationController.PlayAnimation("WalkRight");
                else if (Velocity.X < 0) // Left
                    AnimationController.PlayAnimation("WalkLeft");
                else if (Velocity.Y < 0) // Up
                    AnimationController.PlayAnimation("WalkUp");
                else if (Velocity.Y > 0) // Down
                    AnimationController.PlayAnimation("WalkDown");
            }
            else
            {
                AnimationController.PlayAnimation("Idle");
            }

            AnimationController.Update(deltaTime);

            Position += Velocity * deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var texture = AnimationController.GetCurrentTexture();
            var sourceRect = AnimationController.GetCurrentSourceRectangle();
            
            if (texture != null && sourceRect != Rectangle.Empty)
            {
                spriteBatch.Draw(
                    texture,
                    Position,
                    sourceRect,
                    Color.White
                );
            }
        }
    }
}