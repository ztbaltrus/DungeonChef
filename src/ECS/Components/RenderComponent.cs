namespace DungeonChef.Src.ECS.Components
{
    public enum RenderArchetype
    {
        Generic,
        Player,
        Enemy,
        Loot
    }

    public sealed class RenderComponent : IComponent
    {
        public RenderComponent(RenderArchetype archetype, string? spriteId = null)
        {
            Archetype = archetype;
            SpriteId = spriteId;
        }

        public RenderArchetype Archetype { get; }
        public string? SpriteId { get; }
    }
}
