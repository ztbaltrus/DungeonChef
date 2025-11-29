using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonChef.Src.UI
{
    /// <summary>
    /// Simple reusable "Press E" interaction prompt.
    /// You pass a screen-space position each frame where it should appear.
    /// </summary>
    public sealed class InteractPrompt
    {
        private bool _active;
        private Vector2 _screenPos;

        private Texture2D? _pixel;
        private Texture2D? _glyphE;

        public void SetPrompt(bool active, Vector2 screenPos)
        {
            _active = active;
            _screenPos = screenPos;
        }

        private void EnsureTextures(GraphicsDevice gd)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(gd, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }

            if (_glyphE == null)
            {
                // Build a tiny "E" glyph procedurally, 5x7 pixels
                int w = 5;
                int h = 7;
                _glyphE = new Texture2D(gd, w, h);
                var data = new Color[w * h];

                // 1 = pixel on, 0 = off
                int[,] pattern =
                {
                    {1,1,1,1,1},
                    {1,0,0,0,0},
                    {1,0,0,0,0},
                    {1,1,1,1,0},
                    {1,0,0,0,0},
                    {1,0,0,0,0},
                    {1,1,1,1,1}
                };

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        int idx = y * w + x;
                        data[idx] = pattern[y, x] == 1 ? Color.White : Color.Transparent;
                    }
                }

                _glyphE.SetData(data);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (!_active)
                return;

            EnsureTextures(sb.GraphicsDevice);

            // Background "button" rectangle near the given screen position
            int bgWidth = 28;
            int bgHeight = 24;

            var bgRect = new Rectangle(
                (int)(_screenPos.X - bgWidth / 2f),
                (int)(_screenPos.Y - bgHeight / 2f),
                bgWidth,
                bgHeight
            );

            // Semi-transparent dark background
            sb.Draw(_pixel!, bgRect, new Color(0, 0, 0, 200));

            // Thin border
            // top
            sb.Draw(_pixel!, new Rectangle(bgRect.X, bgRect.Y, bgRect.Width, 1), new Color(255, 255, 255, 60));
            // bottom
            sb.Draw(_pixel!, new Rectangle(bgRect.X, bgRect.Bottom - 1, bgRect.Width, 1), new Color(255, 255, 255, 60));
            // left
            sb.Draw(_pixel!, new Rectangle(bgRect.X, bgRect.Y, 1, bgRect.Height), new Color(255, 255, 255, 60));
            // right
            sb.Draw(_pixel!, new Rectangle(bgRect.Right - 1, bgRect.Y, 1, bgRect.Height), new Color(255, 255, 255, 60));

            // Draw the "E" glyph centered in the button, scaled up
            if (_glyphE != null)
            {
                var center = new Vector2(bgRect.Center.X, bgRect.Center.Y);
                float scale = 2f;

                sb.Draw(
                    _glyphE,
                    center,
                    sourceRectangle: null,
                    color: Color.White,
                    rotation: 0f,
                    origin: new Vector2(_glyphE.Width / 2f, _glyphE.Height / 2f),
                    scale: scale,
                    effects: SpriteEffects.None,
                    layerDepth: 0f
                );
            }
        }
    }
}
