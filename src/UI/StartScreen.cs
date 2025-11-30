using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonChef.Src.UI
{
    /// <summary>
    /// Simple start screen that shows the game title and a "Press Enter to Start" prompt.
    /// </summary>
    public sealed class StartScreen
    {
        private readonly SpriteFont _font;
        private readonly Texture2D _background;

        public StartScreen(GraphicsDevice gd, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            // Create a plain dark background texture.
            _background = new Texture2D(gd, 1, 1);
            _background.SetData(new[] { new Color(10, 10, 15) });

            // Load a default sprite font from the Content pipeline. If the font asset does not exist, we simply skip drawing text.
            try
            {
                _font = content.Load<SpriteFont>("DefaultFont");
            }
            catch
            {
                _font = null!;
            }
        }

        public void Draw(SpriteBatch sb, int screenWidth, int screenHeight)
        {
            // Fill background
            sb.Draw(_background, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);

            if (_font == null)
                return;

            const string title = "Dungeon Chef";
            const string prompt = "Press Enter to Start";

            var titleSize = _font.MeasureString(title);
            var promptSize = _font.MeasureString(prompt);

            var titlePos = new Vector2((screenWidth - titleSize.X) / 2f, screenHeight / 3f);
            var promptPos = new Vector2((screenWidth - promptSize.X) / 2f, screenHeight / 2f);

            sb.DrawString(_font, title, titlePos, Color.White);
            sb.DrawString(_font, prompt, promptPos, Color.LightGray);
        }
    }
}
