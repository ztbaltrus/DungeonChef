using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DungeonChef.Src.Gameplay
{
    // Represents a single attack that an enemy can perform.
    public sealed class EnemyAttack
    {
        public string? Type { get; set; }
        public float Damage { get; set; }
    }

    // Represents the data for an enemy type loaded from JSON.
    public sealed class EnemyDefinition
    {
        public string? Id { get; set; }
        public float Hp { get; set; }
        public float Speed { get; set; }
        public List<EnemyAttack>? Attacks { get; set; }
        // Optional texture filename (e.g., "angry_tomato.png") relative to Content/Sprites/Enemies/
        public string? Texture { get; set; }
    }

    // Simple factory that loads enemy definitions once and provides random selections.
    public static class EnemyFactory
    {
        private static List<EnemyDefinition>? _definitions;

        /// <summary>
        /// Loads enemy definitions from the given JSON file. Subsequent calls are ignored unless the file changes.
        /// </summary>
        public static void Load(string jsonPath)
        {
            if (_definitions != null)
                return; // Already loaded

            if (!File.Exists(jsonPath))
            {
                // Fallback to an empty list to avoid null checks elsewhere.
                _definitions = new List<EnemyDefinition>();
                return;
            }

            var json = File.ReadAllText(jsonPath);
            // The JSON file contains an array of enemy objects.
            _definitions = JsonConvert.DeserializeObject<List<EnemyDefinition>>(json) ?? new List<EnemyDefinition>();
        }

        /// <summary>
        /// Returns a random enemy definition using the provided Random instance.
        /// </summary>
        public static EnemyDefinition? GetRandom(Random rng)
        {
            if (_definitions == null || _definitions.Count == 0)
                return null;

            int index = rng.Next(_definitions.Count);
            return _definitions[index];
        }

        /// <summary>
        /// Retrieve a definition by its Id.
        /// </summary>
        public static EnemyDefinition? GetById(string id)
        {
            if (_definitions == null)
                return null;
            return _definitions.Find(d => d.Id == id);
        }
    }
}
