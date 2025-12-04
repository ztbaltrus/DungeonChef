using DungeonChef.Src.Core;
using DungeonChef.Src.ECS;
using DungeonChef.Src.ECS.Components;
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

            var ordered = world.With<TransformComponent>().OrderBy(e =>
            {
                var transform = e.GetComponent<TransformComponent>();
                var p = IsoMath.ToScreen(transform.Position.X, transform.Position.Y);
                return p.Y;
            });

            foreach (var entity in ordered)
            {
                var transform = entity.GetComponent<TransformComponent>();
                var isoPos = IsoMath.ToScreen(transform.Position.X, transform.Position.Y);
                var screenPos = isoPos - cam.Position;

                if (entity.TryGetComponent(out AnimationComponent animation))
                {
                    var texture = animation.Controller.GetCurrentTexture();
                    var sourceRect = animation.Controller.GetCurrentSourceRectangle();
                    if (texture != null && sourceRect != Rectangle.Empty)
                    {
                        sb.Draw(
                            texture,
                            screenPos,
                            sourceRect,
                            Color.White,
                            0f,
                            new Vector2(sourceRect.Width / 2f, sourceRect.Height),
                            1f,
                            SpriteEffects.None,
                            layerDepth: 0.6f);
                        continue;
                    }
                }

                Texture2D sprite = DummyHero!;
                float layer = 0.5f;
                if (entity.TryGetComponent(out RenderComponent render))
                {
                    switch (render.Archetype)
                    {
                        case RenderArchetype.Player:
                            sprite = DummyHero!;
                            layer = 0.6f;
                            break;
                        case RenderArchetype.Enemy:
                            sprite = EnemySpriteAtlas.GetTexture(sb.GraphicsDevice, render.SpriteId) ?? DummyEnemy!;
                            layer = 0.5f;
                            break;
                        case RenderArchetype.Loot:
                            sprite = ResolveLootTexture(sb.GraphicsDevice, entity) ?? DummyPickup!;
                            layer = 0.4f;
                            break;
                        default:
                            sprite = DummyHero!;
                            layer = 0.5f;
                            break;
                    }
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
                    layerDepth: layer);
            }

        }

        private static Texture2D? ResolveLootTexture(GraphicsDevice gd, Entity entity)
        {
            return entity.TryGetComponent(out LootComponent loot)
                ? ItemSpriteAtlas.GetTexture(gd, loot.ItemId)
                : null;
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
