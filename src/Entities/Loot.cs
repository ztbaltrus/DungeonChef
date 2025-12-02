using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonChef.Src.Utils;
using DungeonChef.Src.ECS;

namespace DungeonChef.Src.Entities
{
    public class Loot : Entity
    {
        public AnimationController AnimationController { get; set; }
        public string ItemId { get; set; }
        public bool IsPickup { get; set; }
        public float PickupTimer { get; set; }
        public float PickupDuration { get; set; } = 2.0f; // Duration in seconds before pickup disappears

        public Loot(Vector2 position, string itemId)
        {
            Position = position;
            Velocity = Vector2.Zero;
            AnimationController = new AnimationController();
            ItemId = itemId;
            IsPickup = true;
            Radius = 0.2f; // Smaller radius for pickup items
            MaxHP = 100f;
            HP = MaxHP;
        }

        public void Update(float deltaTime)
        {
            // Update animation
            AnimationController.Update(deltaTime);
            
            // Update pickup timer
            if (PickupTimer > 0)
            {
                PickupTimer -= deltaTime;
                if (PickupTimer <= 0)
                {
                    // Item disappears after pickup duration
                    HP = 0;
                }
            }
            
            // Add any pickup-specific logic here
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

        public void Collect()
        {
            // Mark item as collected
            PickupTimer = PickupDuration;
        }
    }
}