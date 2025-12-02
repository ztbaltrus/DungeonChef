using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DungeonChef.Src.Utils;

namespace DungeonChef.Src.UI
{
    /// <summary>
    /// Simple start screen that shows a start button.
    /// </summary>
    public sealed class StartScreen
    {
        private readonly Texture2D _background;
        private readonly Texture2D _buttonTexture;

        public StartScreen(GraphicsDevice gd)
        {
            // Create a solid dark background texture
            _background = new Texture2D(gd, 1, 1);
            _background.SetData(new[] { new Color(10, 10, 15) });
            
            // Create a simple button texture
            _buttonTexture = new Texture2D(gd, 1, 1);
            _buttonTexture.SetData(new[] { Color.Green });
        }

        public void Draw(SpriteBatch sb, int screenWidth, int screenHeight)
        {
            // Clear the screen with a dark background
            sb.GraphicsDevice.Clear(new Color(10, 10, 15));

            // Draw background
            sb.Draw(_background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            
            // Draw title text
            
            // Draw subtitle text
            TextRenderer.DrawText(sb, "The Queen is Hungry", new Vector2(screenWidth / 2 - 50, screenHeight / 2 - 40), Color.White, 1.0f);
            
            // Draw start text
            TextRenderer.DrawText(sb, "Press Enter to Start", new Vector2(screenWidth / 2 - 50, screenHeight / 2), Color.White, 1.0f);
        }
    }
}