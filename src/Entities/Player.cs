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
        private Texture2D _playerTexture;

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

        public void LoadPlayerAnimations(Texture2D texture)
        {
            _playerTexture = texture;

            // Create animations - adjust these values based on your sprite sheet
            // Assuming 32x32 pixel frames (adjust as needed)
            int frameWidth = 32;
            int frameHeight = 32;

            // Create animations for different directions
            // You'll need to organize your sprite sheet with frames laid out properly
            var idleAnimation = new Animation(_playerTexture, frameWidth, frameHeight, 0.2f, false);
            var walkUpAnimation = new Animation(_playerTexture, frameWidth, frameHeight, 0.1f, true);
            var walkDownAnimation = new Animation(_playerTexture, frameWidth, frameHeight, 0.1f, true);
            var walkLeftAnimation = new Animation(_playerTexture, frameWidth, frameHeight, 0.1f, true);
            var walkRightAnimation = new Animation(_playerTexture, frameWidth, frameHeight, 0.1f, true);
            var walkUpRightAnimation = new Animation(_playerTexture, frameWidth, frameHeight, 0.1f, true);
            var walkUpLeftAnimation = new Animation(_playerTexture, frameWidth, frameHeight, 0.1f, true);
            var walkDownRightAnimation = new Animation(_playerTexture, frameWidth, frameHeight, 0.1f, true);
            var walkDownLeftAnimation = new Animation(_playerTexture, frameWidth, frameHeight, 0.1f, true);

            // Add animations to controller
            this.AnimationController.AddAnimation("Idle", idleAnimation);
            this.AnimationController.AddAnimation("WalkUp", walkUpAnimation);
            this.AnimationController.AddAnimation("WalkDown", walkDownAnimation);
            this.AnimationController.AddAnimation("WalkLeft", walkLeftAnimation);
            this.AnimationController.AddAnimation("WalkRight", walkRightAnimation);
            this.AnimationController.AddAnimation("WalkUpRight", walkUpRightAnimation);
            this.AnimationController.AddAnimation("WalkUpLeft", walkUpLeftAnimation);
            this.AnimationController.AddAnimation("WalkDownRight", walkDownRightAnimation);
            this.AnimationController.AddAnimation("WalkDownLeft", walkDownLeftAnimation);

            // Play idle animation initially
            this.AnimationController.PlayAnimation("Idle");
        }
    }
}