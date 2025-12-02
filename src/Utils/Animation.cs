using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonChef.Src.Utils
{
    public class Animation
    {
        public Texture2D Texture { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int FrameCount { get; set; }
        public float FrameTime { get; set; } // Time in seconds per frame
        public bool IsLooping { get; set; } = true;

        private float _timer;
        private int _currentFrame;
        private bool _isPlaying;

        public Animation(Texture2D texture, int frameWidth, int frameHeight, int frameCount, float frameTime)
        {
            Texture = texture;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            FrameCount = frameCount;
            FrameTime = frameTime;
        }

        public void Play()
        {
            _isPlaying = true;
        }

        public void Stop()
        {
            _isPlaying = false;
        }

        public void Reset()
        {
            _timer = 0;
            _currentFrame = 0;
        }

        public void Update(float deltaTime)
        {
            if (!_isPlaying) return;

            _timer += deltaTime;

            if (_timer >= FrameTime)
            {
                _timer = 0;
                _currentFrame++;

                if (_currentFrame >= FrameCount)
                {
                    if (IsLooping)
                        _currentFrame = 0;
                    else
                        _currentFrame = FrameCount - 1;
                }
            }
        }

        public Rectangle GetSourceRectangle()
        {
            int column = _currentFrame % (Texture.Width / FrameWidth);
            int row = _currentFrame / (Texture.Width / FrameWidth);

            return new Rectangle(
                column * FrameWidth,
                row * FrameHeight,
                FrameWidth,
                FrameHeight
            );
        }

        public int CurrentFrame => _currentFrame;
    }
}