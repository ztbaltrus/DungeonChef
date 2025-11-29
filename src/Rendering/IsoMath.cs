using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Rendering
{
    public static class IsoMath
    {
        // Classic 2:1 isometric tile
        public const int TileW = 128;
        public const int TileH = 64;

        // World/grid (x,y) -> screen position
        public static Vector2 ToScreen(float x, float y)
        {
            float sx = (x - y) * (TileW / 2f);
            float sy = (x + y) * (TileH / 2f);
            return new Vector2(sx, sy);
        }

        public static Vector2 ToScreen(int x, int y)
            => ToScreen((float)x, (float)y);

        public static float DepthFromWorld(float x, float y)
        {
            float sy = (x + y) * (TileH / 2f);
            return 1f - (sy / 100000f);
        }
    }
}
