using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonChef.Src.Utils
{
    /// <summary>
    /// Reusable pixel-based text rendering system for drawing text without font assets.
    /// </summary>
    public static class TextRenderer
    {
        private static Texture2D? _pixel;
        
        private static void EnsurePixel(GraphicsDevice gd)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(gd, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }
        }

        /// <summary>
        /// Draws text using a simple pixel-based approach
        /// </summary>
        public static void DrawText(SpriteBatch sb, string text, Vector2 position, Color color, float scale = 1.0f)
        {
            if (string.IsNullOrEmpty(text)) return;
            
            EnsurePixel(sb.GraphicsDevice);
            
            // Simple character mapping - basic ASCII characters
            var charWidth = 6;
            var charHeight = 8;
            var spacing = 2;
            
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                Vector2 charPos = new Vector2(position.X + i * (charWidth + spacing) * scale, position.Y);
                
                DrawCharacter(sb, c, charPos, color, scale);
            }
        }

        private static void DrawCharacter(SpriteBatch sb, char c, Vector2 position, Color color, float scale)
        {
            if (c == ' ') return; // Skip spaces
            
            EnsurePixel(sb.GraphicsDevice);
            
            // Simple character patterns (basic ASCII)
            var pattern = GetCharacterPattern(c);
            var charWidth = 5;
            var charHeight = 7;
            
            for (int y = 0; y < charHeight; y++)
            {
                for (int x = 0; x < charWidth; x++)
                {
                    if (pattern[y, x] == 1)
                    {
                        var rect = new Rectangle(
                            (int)(position.X + x * scale),
                            (int)(position.Y + y * scale),
                            (int)scale,
                            (int)scale
                        );
                        sb.Draw(_pixel!, rect, color);
                    }
                }
            }
        }

        private static int[,] GetCharacterPattern(char c)
        {
            // Simple character patterns - you can expand this
            switch (c)
            {
                case 'A':
                case 'a':
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,1,1,1,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1}
                    };
                case 'B':
                case 'b':
                    return new int[,]
                    {
                        {1,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,1,1,1,0}
                    };
                case 'C':
                case 'c':
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,0},
                        {1,0,0,0,0},
                        {1,0,0,0,0},
                        {1,0,0,0,1},
                        {0,1,1,1,0}
                    };
                case 'D':
                case 'd':
                    return new int[,]
                    {
                        {1,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,1,1,1,0}
                    };
                case 'E':
                case 'e':
                    return new int[,]
                    {
                        {1,1,1,1,1},
                        {1,0,0,0,0},
                        {1,0,0,0,0},
                        {1,1,1,1,0},
                        {1,0,0,0,0},
                        {1,0,0,0,0},
                        {1,1,1,1,1}
                    };
                case 'F':
                case 'f':
                    return new int[,]
                    {
                        {1,1,1,1,1},
                        {1,0,0,0,0},
                        {1,0,0,0,0},
                        {1,1,1,1,0},
                        {1,0,0,0,0},
                        {1,0,0,0,0},
                        {1,0,0,0,0}
                    };
                case 'G':
                case 'g':
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,0},
                        {1,0,0,1,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,1,1,0}
                    };
                case 'H':
                case 'h':
                    return new int[,]
                    {
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,1,1,1,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1}
                    };
                case 'I':
                case 'i':
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0},
                        {0,1,1,1,0}
                    };
                case 'J':
                case 'j':
                    return new int[,]
                    {
                        {0,0,1,1,1},
                        {0,0,0,1,0},
                        {0,0,0,1,0},
                        {0,0,0,1,0},
                        {0,0,0,1,0},
                        {1,0,0,1,0},
                        {0,1,1,0,0}
                    };
                case 'K':
                case 'k':
                    return new int[,]
                    {
                        {1,0,0,0,1},
                        {1,0,0,1,0},
                        {1,0,1,0,0},
                        {1,1,0,0,0},
                        {1,0,1,0,0},
                        {1,0,0,1,0},
                        {1,0,0,0,1}
                    };
                case 'L':
                case 'l':
                    return new int[,]
                    {
                        {1,0,0,0,0},
                        {1,0,0,0,0},
                        {1,0,0,0,0},
                        {1,0,0,0,0},
                        {1,0,0,0,0},
                        {1,0,0,0,0},
                        {1,1,1,1,1}
                    };
                case 'M':
                case 'm':
                    return new int[,]
                    {
                        {1,0,0,0,1},
                        {1,1,0,1,1},
                        {1,0,1,0,1},
                        {1,0,1,0,1},
                        {1,0,1,0,1},
                        {1,0,1,0,1},
                        {1,0,1,0,1}
                    };
                case 'N':
                case 'n':
                    return new int[,]
                    {
                        {1,0,0,0,1},
                        {1,1,0,0,1},
                        {1,0,1,0,1},
                        {1,0,0,1,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1}
                    };
                case 'O':
                case 'o':
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,1,1,0}
                    };
                case 'P':
                case 'p':
                    return new int[,]
                    {
                        {1,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,1,1,1,0},
                        {1,0,0,0,0},
                        {1,0,0,0,0},
                        {1,0,0,0,0}
                    };
                case 'Q':
                case 'q':
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,1,1},
                        {1,0,0,0,1},
                        {0,1,1,1,0}
                    };
                case 'R':
                case 'r':
                    return new int[,]
                    {
                        {1,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,1,1,1,0},
                        {1,0,1,0,0},
                        {1,0,0,1,0},
                        {1,0,0,0,1}
                    };
                case 'S':
                case 's':
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,0},
                        {0,1,1,1,0},
                        {0,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,1,1,0}
                    };
                case 'T':
                case 't':
                    return new int[,]
                    {
                        {1,1,1,1,1},
                        {0,0,1,0,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0}
                    };
                case 'U':
                case 'u':
                    return new int[,]
                    {
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,1,1,0}
                    };
                case 'V':
                case 'v':
                    return new int[,]
                    {
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,0,1,0},
                        {0,0,1,0,0}
                    };
                case 'W':
                case 'w':
                    return new int[,]
                    {
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,1,0,1},
                        {1,0,1,0,1},
                        {1,1,0,1,1},
                        {1,0,0,0,1}
                    };
                case 'X':
                case 'x':
                    return new int[,]
                    {
                        {1,0,0,0,1},
                        {0,1,0,1,0},
                        {0,0,1,0,0},
                        {0,1,0,1,0},
                        {1,0,0,0,1},
                        {0,0,0,0,0},
                        {0,0,0,0,0}
                    };
                case 'Y':
                case 'y':
                    return new int[,]
                    {
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,0,1,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0}
                    };
                case 'Z':
                case 'z':
                    return new int[,]
                    {
                        {1,1,1,1,1},
                        {0,0,0,0,1},
                        {0,0,0,1,0},
                        {0,0,1,0,0},
                        {0,1,0,0,0},
                        {1,0,0,0,0},
                        {1,1,1,1,1}
                    };
                case '0':
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,1,1,0}
                    };
                case '1':
                    return new int[,]
                    {
                        {0,0,1,0,0},
                        {0,1,1,0,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0},
                        {0,0,1,0,0},
                        {1,1,1,1,1}
                    };
                case '2':
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {0,0,0,0,1},
                        {0,0,0,1,0},
                        {0,0,1,0,0},
                        {0,1,0,0,0},
                        {1,1,1,1,1}
                    };
                case '3':
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {0,0,0,0,1},
                        {0,0,1,1,0},
                        {0,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,1,1,0}
                    };
                case '4':
                    return new int[,]
                    {
                        {0,0,0,1,0},
                        {0,0,1,1,0},
                        {0,1,0,1,0},
                        {1,0,0,1,0},
                        {1,1,1,1,1},
                        {0,0,0,1,0},
                        {0,0,0,1,0}
                    };
                case '5':
                    return new int[,]
                    {
                        {1,1,1,1,1},
                        {1,0,0,0,0},
                        {1,0,0,0,0},
                        {1,1,1,1,0},
                        {0,0,0,0,1},
                        {0,0,0,0,1},
                        {1,1,1,1,0}
                    };
                case '6':
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,0},
                        {1,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,1,1,0}
                    };
                case '7':
                    return new int[,]
                    {
                        {1,1,1,1,1},
                        {0,0,0,0,1},
                        {0,0,0,1,0},
                        {0,0,1,0,0},
                        {0,1,0,0,0},
                        {1,0,0,0,0},
                        {1,0,0,0,0}
                    };
                case '8':
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,1,1,0}
                    };
                case '9':
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,1,1,1},
                        {0,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,1,1,0}
                    }; 
                default:
                    return new int[,]
                    {
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,1,1,0},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {0,1,1,1,0}
                    };
            }
        }
    }
}