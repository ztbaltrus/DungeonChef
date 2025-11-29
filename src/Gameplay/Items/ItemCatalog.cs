using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace DungeonChef.Src.Gameplay.Items
{
    public static class ItemCatalog
    {
        private static List<ItemDefinition>? _items;
        private static int _totalWeight;

        private static void EnsureLoaded()
        {
            if (_items != null)
                return;

            string path = Path.Combine("Content", "Data", "items.json");
            if (!File.Exists(path))
            {
                _items = new List<ItemDefinition>();
                _totalWeight = 0;
                return;
            }

            var json = File.ReadAllText(path);
            _items = JsonConvert.DeserializeObject<List<ItemDefinition>>(json) ?? new List<ItemDefinition>();
            _totalWeight = _items.Sum(i => i.DropWeight);
        }

        public static ItemDefinition? RollItem(System.Random rng)
        {
            EnsureLoaded();
            if (_items == null || _items.Count == 0 || _totalWeight <= 0)
                return null;

            int roll = rng.Next(0, _totalWeight);
            foreach (var item in _items)
            {
                if (roll < item.DropWeight)
                    return item;

                roll -= item.DropWeight;
            }

            return null;
        }

        public static ItemDefinition? GetById(string id)
        {
            EnsureLoaded();
            return _items?.FirstOrDefault(i => i.Id == id);
        }
    }
}
