using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonChef.Src.Utils
{
    public class Animation
    {
        public Texture2D Texture { get; set; }
        public Rectangle[] Frames { get; set; }
        public int CurrentFrame { get; set; }
        public float FrameTime { get; set; }
        public float ElapsedTime { get; set; }
        public bool Loop { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }

        private bool _isPlaying;

        public Animation(Texture2D texture, int frameWidth, int frameHeight, float frameTime = 0.1f, bool loop = true)
        {
            Texture = texture;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            FrameTime = frameTime;
            Loop = loop;
            CurrentFrame = 0;
            ElapsedTime = 0;
            _isPlaying = true;
            
            // Calculate number of frames based on texture size
            int frameCountX = texture.Width / frameWidth;
            int frameCountY = texture.Height / frameHeight;
            int totalFrames = frameCountX * frameCountY;
            
            Frames = new Rectangle[totalFrames];
            int index = 0;
            for (int y = 0; y < frameCountY; y++)
            {
                for (int x = 0; x < frameCountX; x++)
                {
                    Frames[index] = new Rectangle(x * frameWidth, y * frameHeight, frameWidth, frameHeight);
                    index++;
                }
            }
        }

        public void Update(float deltaTime)
        {
            if (!_isPlaying) return;
            
            ElapsedTime += deltaTime;
            
            if (ElapsedTime >= FrameTime)
            {
                ElapsedTime = 0;
                CurrentFrame++;
                
                if (CurrentFrame >= Frames.Length)
                {
                    if (Loop)
                    {
                        CurrentFrame = 0;
                    }
                    else
                    {
                        CurrentFrame = Frames.Length - 1;
                        _isPlaying = false;
                    }
                }
            }
        }

        public void Reset()
        {
            CurrentFrame = 0;
            ElapsedTime = 0;
            _isPlaying = true;
        }

        public Rectangle GetSourceRectangle()
        {
            if (Frames.Length > 0)
                return Frames[CurrentFrame];
            return Rectangle.Empty;
        }

        public void Play()
        {
            _isPlaying = true;
        }

        public void Stop()
        {
            _isPlaying = false;
        }
    }
}