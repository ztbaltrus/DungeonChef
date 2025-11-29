using System.Linq;
using DungeonChef.Src.Gameplay;
using DungeonChef.Src.Gameplay.Rooms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonChef.Src.UI
{
    public sealed class Minimap
    {
        private readonly RoomController _roomController;
        private Texture2D? _pixel;

        // Mini-iso tile size for the minimap (in pixels)
        private const int MiniTileW = 16; // width of a diamond
        private const int MiniTileH = 16; // height of a diamond

        public Minimap(RoomController roomController)
        {
            _roomController = roomController;
        }

        private void EnsurePixel(GraphicsDevice gd)
        {
            if (_pixel != null) return;

            _pixel = new Texture2D(gd, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void Draw(SpriteBatch sb, Rectangle area)
        {
            if (_roomController == null || _roomController.Rooms == null || _roomController.Rooms.Count == 0)
                return;

            EnsurePixel(sb.GraphicsDevice);

            var rooms = _roomController.Rooms.Values
                .Where(r => r.Discovered || r.Visited)
                .ToList();

            if (rooms.Count == 0)
                return;

            // --- Compute iso positions for each discovered/visited room ---
            // We'll remap them so the whole cluster fits nicely in a panel.
            var isoPositions = rooms.ToDictionary(
                r => r,
                r => ToMiniIso(r.GridPos.X, r.GridPos.Y)
            );

            float minX = isoPositions.Values.Min(v => v.X);
            float maxX = isoPositions.Values.Max(v => v.X);
            float minY = isoPositions.Values.Min(v => v.Y);
            float maxY = isoPositions.Values.Max(v => v.Y);

            float mapIsoWidth = maxX - minX + MiniTileW;
            float mapIsoHeight = maxY - minY + MiniTileH;

            int padding = 8;

            // Anchor minimap in the top-right corner of the given UI area
            int panelWidth = (int)mapIsoWidth + padding * 2;
            int panelHeight = (int)mapIsoHeight + padding * 2;

            int panelX = area.Right - panelWidth - 16;
            int panelY = area.Top + 16;

            var panelRect = new Rectangle(panelX, panelY, panelWidth, panelHeight);

            // Background
            DrawRect(sb, panelRect, new Color(0, 0, 0, 200));
            DrawBorder(sb, panelRect, new Color(255, 255, 255, 40));

            // Panel-space origin so the iso cluster is centered inside
            Vector2 panelOrigin = new Vector2(
                panelRect.X + panelRect.Width / 2f,
                panelRect.Y + panelRect.Height / 2f
            );

            // Center of the iso cluster in iso-space
            Vector2 isoCenter = new Vector2(
                (minX + maxX) / 2f,
                (minY + maxY) / 2f
            );

            // --- Draw rooms as mini isometric diamonds ---
            foreach (var room in rooms)
            {
                var isoPos = isoPositions[room];

                // Normalize so that iso cluster is centered in panel
                Vector2 localIso = isoPos - isoCenter;

                // Final screen pos on the minimap
                Vector2 screenPos = panelOrigin + localIso;

                Color baseColor;
                if (room == _roomController.CurrentRoom)
                {
                    baseColor = Color.White;
                }
                else if (room.Visited)
                {
                    baseColor = new Color(200, 200, 200);
                }
                else
                {
                    baseColor = new Color(90, 90, 90);
                }

                baseColor = ApplyTypeTint(baseColor, room.Type, room.Visited);

                DrawDiamond(sb, screenPos, baseColor);
            }
        }

        private static Vector2 ToMiniIso(int x, int y)
        {
            // Classic iso projection but at minimap scale
            float sx = (x - y) * (MiniTileW / 2f);
            float sy = (x + y) * (MiniTileH / 2f);
            return new Vector2(sx, sy);
        }

        private Color ApplyTypeTint(Color baseColor, RoomType type, bool visited)
        {
            // If you want room type hidden until visited, gate on 'visited'
            if (!visited)
                return baseColor;

            return type switch
            {
                RoomType.Boss => Color.Lerp(baseColor, Color.Red, 0.6f),
                RoomType.Shop => Color.Lerp(baseColor, Color.Cyan, 0.4f),
                RoomType.CookStation => Color.Lerp(baseColor, Color.LimeGreen, 0.4f),
                RoomType.Treasure => Color.Lerp(baseColor, Color.Yellow, 0.5f),
                _ => baseColor
            };
        }

        private void DrawRect(SpriteBatch sb, Rectangle rect, Color color)
        {
            if (_pixel == null) return;
            sb.Draw(_pixel, rect, color);
        }

        private void DrawBorder(SpriteBatch sb, Rectangle rect, Color color)
        {
            if (_pixel == null) return;

            // top
            sb.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, 1), color);
            // bottom
            sb.Draw(_pixel, new Rectangle(rect.X, rect.Bottom - 1, rect.Width, 1), color);
            // left
            sb.Draw(_pixel, new Rectangle(rect.X, rect.Y, 1, rect.Height), color);
            // right
            sb.Draw(_pixel, new Rectangle(rect.Right - 1, rect.Y, 1, rect.Height), color);
        }

        private void DrawDiamond(SpriteBatch sb, Vector2 center, Color color)
        {
            if (_pixel == null) return;

            // Draw a rotated/squished square to look like a small diamond
            float scaleX = MiniTileW / 2f;
            float scaleY = MiniTileH / 2f;

            sb.Draw(
                _pixel,
                center,
                sourceRectangle: null,
                color: color,
                rotation: MathHelper.PiOver4, // 45 degrees -> diamond
                origin: new Vector2(0.5f, 0.5f),
                scale: new Vector2(scaleX, scaleY),
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
        }
    }
}
