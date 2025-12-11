using DungeonChef.Src.ECS;
using DungeonChef.Src.ECS.Components;
using DungeonChef.Src.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonChef.Src.Entities
{
    public sealed class Player : Entity
    {
        public Player(Vector2 position) : base("Player")
        {
            var transform = AddComponent(new TransformComponent());
            transform.Position = position;
            transform.Grid = position;

            AddComponent(new PlayerTagComponent());
            AddComponent(new MovementComponent(MovementBehavior.PlayerInput, 3f));
            AddComponent(new HealthComponent { Max = 100f, Current = 100f });
            AddComponent(new MeleeAttackComponent(damage: 5f, range: 2.5f, angleDegrees: 60f, cooldownSeconds: 0.25f));
            AddComponent(new RenderComponent(RenderArchetype.Player));
        }

        public void LoadPlayerAnimations(Texture2D texture)
        {
            var controller = new AnimationController();

            const int frameWidth = 72;
            const int frameHeight = 72;

            controller.AddAnimation("Idle", new Animation(texture, frameWidth, frameHeight, 0.2f, false));
            controller.AddAnimation("WalkUp", new Animation(texture, frameWidth, frameHeight, 0.1f, true));
            controller.AddAnimation("WalkDown", new Animation(texture, frameWidth, frameHeight, 0.1f, true));
            controller.AddAnimation("WalkLeft", new Animation(texture, frameWidth, frameHeight, 0.1f, true));
            controller.AddAnimation("WalkRight", new Animation(texture, frameWidth, frameHeight, 0.1f, true));
            controller.AddAnimation("WalkUpRight", new Animation(texture, frameWidth, frameHeight, 0.1f, true));
            controller.AddAnimation("WalkUpLeft", new Animation(texture, frameWidth, frameHeight, 0.1f, true));
            controller.AddAnimation("WalkDownRight", new Animation(texture, frameWidth, frameHeight, 0.1f, true));
            controller.AddAnimation("WalkDownLeft", new Animation(texture, frameWidth, frameHeight, 0.1f, true));

            controller.PlayAnimation("Idle");
            AddComponent(new AnimationComponent(controller));
        }
    }
}
