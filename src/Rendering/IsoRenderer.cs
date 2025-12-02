using DungeonChef.Src.Core;
using DungeonChef.Src.ECS;
using DungeonChef.Src.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace DungeonChef.Src.Rendering
{
    public static class IsoRenderer
    {
        public static Texture2D? DummyTile;
        public static Texture2D? DummyHero;
        public static Texture2D? DummyEnemy;
        public static Texture2D? DummyPickup;

        private const int GridWidth = 10;
        private const int GridHeight = 10;

        public static void DrawWorld(SpriteBatch sb, IsoCamera cam, World world)
        {
            EnsurePlaceholders(sb.GraphicsDevice);

            // 1) Draw tiles (background)
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    var isoPos = IsoMath.ToScreen(x, y);
                    var tileScreen = isoPos - cam.Position;

                    sb.Draw(
                        DummyTile!,
                        tileScreen,
                        sourceRectangle: null,
                        color: Color.White,
                        rotation: 0f,
                        // bottom-center of the tile: "feet" at world pos
                        origin: new Vector2(DummyTile!.Width / 2f, DummyTile!.Height - 1),
                        scale: 1f,
                        effects: SpriteEffects.None,
                        layerDepth: 0f // we rely on draw order, not depth
                    );
                }
            }

            // 2) Draw entities (hero etc.) on top of tiles
            var ordered = world.Entities.OrderBy(e =>
            {
                var p = IsoMath.ToScreen(e.Position.X, e.Position.Y);
                return p.Y;
            });

            foreach (var e in ordered)
            {
                var isoPos = IsoMath.ToScreen(e.Position.X, e.Position.Y);
                var screenPos = isoPos - cam.Position;

                Texture2D sprite;
                float layer = 0.5f;

                if (e.GetType() == typeof(Player))
                {
                    sprite = DummyHero!;
                    layer = 0.6f;
                }
                else if (e.GetType() == typeof(Enemy))
                {
                    // Try to load a specific enemy sprite based on the EnemyId field.
                    Texture2D? specific = null;
                    Enemy enemy = e as Enemy;
                    if (!string.IsNullOrEmpty(enemy.EnemyId))
                    {
                        // Expect enemy sprites to be located under Content/Sprites/Enemies/<id>.png
                        var texturePath = $"Sprites/Enemies/{enemy.EnemyId}.png"; // Full relative path inside Content
                        specific = EnemySpriteAtlas.GetTexture(sb.GraphicsDevice, texturePath);
                    }
                    sprite = specific ?? DummyEnemy!;
                    layer = 0.5f;
                }
                else if (e.GetType() == typeof(Loot))
                {
                    // Try to get the real item texture first
                    Loot loot = e as Loot;
                    var tex = ItemSpriteAtlas.GetTexture(sb.GraphicsDevice, loot.ItemId);
                    sprite = tex ?? DummyPickup!;
                    layer = 0.4f;
                }
                else
                {
                    sprite = DummyHero!;
                }

                sb.Draw(
                    sprite,
                    screenPos,
                    sourceRectangle: null,
                    color: Color.White,
                    rotation: 0f,
                    origin: new Vector2(sprite.Width / 2f, sprite.Height),
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: layer
                );
            }

        }

        private static void EnsurePlaceholders(GraphicsDevice gd)
        {
            // --- Diamond tile as before ---
            if (DummyTile == null)
            {
                int w = IsoMath.TileW;
                int h = IsoMath.TileH;
                DummyTile = new Texture2D(gd, w, h);

                var data = new Color[w * h];

                int centerX = w / 2;
                int midY = h / 2;
                int halfH = midY;

                for (int y = 0; y < h; y++)
                {
                    int dy = System.Math.Abs(y - midY);
                    float t = 1f - (dy / (float)halfH);
                    if (t < 0f) t = 0f;

                    int halfWidth = (int)(t * (w / 2f));
                    int minX = centerX - halfWidth;
                    int maxX = centerX + halfWidth;

                    for (int x = 0; x < w; x++)
                    {
                        int idx = y * w + x;
                        if (x >= minX && x <= maxX)
                            data[idx] = new Color(40, 50, 55, 255);
                        else
                            data[idx] = Color.Transparent;
                    }
                }

                DummyTile.SetData(data);
            }

            // --- Player sprite (blue square) ---
            if (DummyHero == null)
            {
                const int size = 32;
                DummyHero = new Texture2D(gd, size, size);
                var data = new Color[size * size];
                for (int i = 0; i < data.Length; i++)
                    data[i] = Color.CornflowerBlue;
                DummyHero.SetData(data);
            }

            // --- Enemy sprite (red square) ---
            if (DummyEnemy == null)
            {
                const int size = 28;
                DummyEnemy = new Texture2D(gd, size, size);
                var data = new Color[size * size];
                for (int i = 0; i < data.Length; i++)
                    data[i] = Color.IndianRed;
                DummyEnemy.SetData(data);
            }

            if (DummyPickup == null)
            {
                const int size = 20;
                DummyPickup = new Texture2D(gd, size, size);
                var data = new Color[size * size];
                for (int i = 0; i < data.Length; i++)
                    data[i] = Color.Gold;
                DummyPickup.SetData(data);
            }
        }
    }
}
