namespace DungeonChef.Src.ECS.Components
{
    public sealed class LootComponent : IComponent
    {
        public LootComponent(string itemId)
        {
            ItemId = itemId;
        }

        public string ItemId { get; }
    }
}
