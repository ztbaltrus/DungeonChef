using Newtonsoft.Json;
using System;
using System.IO;

namespace DungeonChef.Src.ContentLoading
{
    public sealed class TilemapData
    {
        public int Width;
        public int Height;
        public int[] Tiles = Array.Empty<int>();
    }

    public static class TilemapLoader
    {
        public static TilemapData Load(string path)
        {
            if (!File.Exists(path))
                return new TilemapData { Width = 10, Height = 10, Tiles = new int[100] };

            var json = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<TilemapData>(json);
            return data ?? new TilemapData { Width = 10, Height = 10, Tiles = new int[100] };
        }
    }
}
