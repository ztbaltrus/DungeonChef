namespace DungeonChef.Src.Gameplay.Items
{
    public sealed class ItemDefinition
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "";      // "Currency", "Consumable", etc.

        // Optional numeric fields (coins, heals, etc.)
        public int Value { get; set; } = 0;
        public int Heal { get; set; } = 0;

        public int DropWeight { get; set; } = 0;    // relative chance
        public string Texture { get; set; } = "";   // e.g. "Sprites/ui/coin_small.png"
    }
}
