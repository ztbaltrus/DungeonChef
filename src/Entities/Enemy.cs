using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonChef.Src.Utils;
using DungeonChef.Src.ECS;

namespace DungeonChef.Src.Entities
{
    public class Enemy : Entity
    {
        public AnimationController AnimationController { get; set; }
        public string EnemyId { get; set; }
        public bool IsMoving { get; set; }

        public float Speed = 2f;

        public Enemy(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            AnimationController = new AnimationController();
        }

        public void Update(float deltaTime)
        {
            // Handle enemy-specific movement logic
            AnimationController.Update(deltaTime);
            
            // Add enemy AI logic here
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