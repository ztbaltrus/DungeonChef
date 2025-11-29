using System.Collections.Generic;
using System.IO;
using DungeonChef.Src.Gameplay.Items;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonChef.Src.Rendering
{
    public static class ItemSpriteAtlas
    {
        private static readonly Dictionary<string, Texture2D> _cache = new();

        public static Texture2D? GetTexture(GraphicsDevice gd, string? itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return null;

            var def = ItemCatalog.GetById(itemId);
            if (def == null || string.IsNullOrEmpty(def.Texture))
                return null;

            string key = def.Texture;

            if (_cache.TryGetValue(key, out var tex))
                return tex;

            // Load from disk: Content/<def.Texture>
            string path = Path.Combine("Content", def.Texture);
            if (!File.Exists(path))
                return null;

            using (var fs = File.OpenRead(path))
            {
                tex = Texture2D.FromStream(gd, fs);
            }

            _cache[key] = tex;
            return tex;
        }
    }
}
