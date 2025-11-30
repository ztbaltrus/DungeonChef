using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonChef.Src.Rendering
{
    public static class EnemySpriteAtlas
    {
        private static readonly Dictionary<string, Texture2D> _cache = new();

        /// <summary>
        /// Loads an enemy texture from the Content folder. The path should be relative to the Content root, e.g. "Sprites/Enemies/angry_tomato.png".
        /// </summary>
        public static Texture2D? GetTexture(GraphicsDevice gd, string? texturePath)
        {
            if (string.IsNullOrEmpty(texturePath))
                return null;

            if (_cache.TryGetValue(texturePath, out var tex))
                return tex;

            string fullPath = Path.Combine("Content", texturePath);
            if (!File.Exists(fullPath))
                return null;

            using var fs = File.OpenRead(fullPath);
            tex = Texture2D.FromStream(gd, fs);
            _cache[texturePath] = tex;
            return tex;
        }
    }
}
