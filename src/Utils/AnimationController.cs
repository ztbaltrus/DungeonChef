using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonChef.Src.Utils
{
    public class AnimationController
    {
        private readonly Dictionary<string, Animation> _animations;
        private string _currentAnimation;
        private Animation _currentAnimationInstance;
        private bool _isPlaying;

        public AnimationController()
        {
            _animations = new Dictionary<string, Animation>();
        }

        public void AddAnimation(string name, Animation animation)
        {
            _animations[name] = animation;
        }

        public void PlayAnimation(string name)
        {
            if (_animations.ContainsKey(name))
            {
                _currentAnimation = name;
                _currentAnimationInstance = _animations[name];
                _currentAnimationInstance.Reset();
                _isPlaying = true;
            }
        }

        public void Stop()
        {
            _isPlaying = false;
        }

        public void Update(float deltaTime)
        {
            if (_isPlaying && _currentAnimationInstance != null)
            {
                _currentAnimationInstance.Update(deltaTime);
            }
        }

        public Rectangle GetCurrentSourceRectangle()
        {
            return _currentAnimationInstance?.GetSourceRectangle() ?? Rectangle.Empty;
        }

        public Texture2D GetCurrentTexture()
        {
            return _currentAnimationInstance?.Texture;
        }

        public string CurrentAnimation => _currentAnimation;
        public bool IsPlaying => _isPlaying;
    }
}