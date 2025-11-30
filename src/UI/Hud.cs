using DungeonChef.Src.ECS;
using DungeonChef.Src.Gameplay;
using DungeonChef.Src.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;

namespace DungeonChef.Src.UI
{
    public sealed class Hud
    {
        private Texture2D? _pixel;
        private Texture2D? _coinIcon;

        public void Draw(SpriteBatch sb, World world, int coins)
        {
            EnsurePixel(sb.GraphicsDevice);
            EnsureCoinIcon(sb.GraphicsDevice);

            var player = world.Entities.Find(e => e.IsPlayer);
            if (player == null)
                return;

            // // Simple Aroma/Buff indicator bar stub at top-left
            // var buffRect = new Rectangle(20, 20, 200, 20);
            // sb.Draw(_pixel!, buffRect, Color.Black * 0.5f);

            // var buff = BuffSystem.Get(player);
            // if (buff != null && buff.Active.Count > 0)
            // {
            //     var activeRect = new Rectangle(20, 20, 200, 20);
            //     sb.Draw(_pixel!, activeRect, Color.Orange * 0.6f);
            // }

            DrawHealthBar(sb, player); // Move this line up
            DrawCoins(sb, coins);
        }

        private void DrawCoins(SpriteBatch sb, int coins)
        {
            // Background panel for coins
            var panelRect = new Rectangle(20, 52, 140, 32);
            sb.Draw(_pixel!, panelRect, Color.Black * 0.5f);

            // Coin icon on the left
            if (_coinIcon != null)
            {
                var iconPos = new Vector2(panelRect.X + 8, panelRect.Y + panelRect.Height / 2f);
                var origin = new Vector2(0f, _coinIcon.Height / 2f);

                sb.Draw(
                    _coinIcon,
                    iconPos,
                    sourceRectangle: null,
                    color: Color.White,
                    rotation: 0f,
                    origin: origin,
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: 0f);
            }

            // Simple blocky numeric display for the amount
            var numberOrigin = new Vector2(panelRect.X + 48, panelRect.Y + 8);
            DrawNumber(sb, coins, numberOrigin, 2, Color.Gold);
        }

        private void DrawHealthBar(SpriteBatch sb, Entity player)
        {
            // Health bar
            var healthBarRect = new Rectangle(20, 20, 200, 20);
            sb.Draw(_pixel!, healthBarRect, Color.Green);

            // Health percentage text
            var healthTextOrigin = new Vector2(healthBarRect.X + 8, healthBarRect.Y - 5);
            int healthPercentage = 0;
            if (player.HP > 0 && player.MaxHP > 0)
            {
                // Use floating‑point division to get a proper percentage before casting to int.
                healthPercentage = (int)(player.HP / player.MaxHP * 100f);
            }
            DrawNumber(sb, healthPercentage, healthTextOrigin, 1, Color.White);
        }

        private void DrawNumber(SpriteBatch sb, int value, Vector2 topLeft, int pixelSize, Color color)
        {
            if (value < 0)
                value = 0;

            var text = value.ToString();

            for (int digitIndex = 0; digitIndex < text.Length; digitIndex++)
            {
                char c = text[digitIndex];
                if (c < '0' || c > '9')
                    continue;

                int patternIndex = c - '0';
                var pattern = DigitPatterns[patternIndex];

                const int cols = 3;
                const int rows = 5;

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        int patternPos = row * cols + col;
                        if (pattern[patternPos] != '#')
                            continue;

                        int x = (int)topLeft.X + (digitIndex * (cols + 1) + col) * pixelSize;
                        int y = (int)topLeft.Y + row * pixelSize;

                        var rect = new Rectangle(x, y, pixelSize, pixelSize);
                        sb.Draw(_pixel!, rect, color);
                    }
                }
            }
        }

        private void EnsurePixel(GraphicsDevice gd)
        {
            if (_pixel != null)
                return;

            _pixel = new Texture2D(gd, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        private void EnsureCoinIcon(GraphicsDevice gd)
        {
            if (_coinIcon != null)
                return;

            _coinIcon = ItemSpriteAtlas.GetTexture(gd, "coin_large");
        }

        private static readonly string[] DigitPatterns =
        {
            // 0
            "###" +
            "#.#" +
            "#.#" +
            "#.#" +
            "###",
            // 1
            ".#." +
            "##." +
            ".#." +
            ".#." +
            "###",
            // 2
            "###" +
            "..#" +
            "###" +
            "#.." +
            "###",
            // 3
            "###" +
            "..#" +
            "###" +
            "..#" +
            "###",
            // 4
            "#.#" +
            "#.#" +
            "###" +
            "..#" +
            "..#",
            // 5
            "###" +
            "#.." +
            "###" +
            "..#" +
            "###",
            // 6
            "###" +
            "#.." +
            "###" +
            "#.#" +
            "###",
            // 7
            "###" +
            "..#" +
            "..#" +
            "..#" +
            "..#",
            // 8
            "###" +
            "#.#" +
            "###" +
            "#.#" +
            "###",
            // 9
            "###" +
            "#.#" +
            "###" +
            "..#" +
            "###"
        };
    }
}
