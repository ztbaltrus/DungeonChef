using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonChef.Src.Utils
{
    public class SpriteSheetLoader
    {
        public static Texture2D LoadSpriteSheet(ContentManager content, string assetName)
        {
            return content.Load<Texture2D>(assetName);
        }

        public static Rectangle GetSpriteRectangle(int x, int y, int width, int height)
        {
            return new Rectangle(x, y, width, height);
        }
    }
}