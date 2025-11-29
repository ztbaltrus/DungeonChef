using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace DungeonChef.Src.Meta
{
    public sealed class MetaProgression
    {
        public int SpiceDust;
        public HashSet<string> UnlockedRecipes { get; set; } = new();
        public HashSet<string> UnlockedHeroes { get; set; } = new();
        public HashSet<string> UnlockedUtensils { get; set; } = new();
        public Dictionary<string, int> ApplianceLevels { get; set; } = new();
    }

    public sealed class SaveService
    {
        private readonly string _metaPath;

        public SaveService()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DungeonChef");
            Directory.CreateDirectory(dir);
            _metaPath = Path.Combine(dir, "meta.json");
        }

        public void SaveMeta(MetaProgression meta)
        {
            try
            {
                var json = JsonConvert.SerializeObject(meta, Formatting.Indented);
                File.WriteAllText(_metaPath, json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save meta progression");
            }
        }

        public MetaProgression LoadMeta()
        {
            try
            {
                if (!File.Exists(_metaPath))
                    return new MetaProgression();

                var json = File.ReadAllText(_metaPath);
                var meta = JsonConvert.DeserializeObject<MetaProgression>(json);
                return meta ?? new MetaProgression();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load meta progression");
                return new MetaProgression();
            }
        }
    }
}
