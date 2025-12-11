using DungeonChef.Src.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DungeonChef.Src.Utils
{
    public class AnimationController
    {
        private readonly Dictionary<string, Animation> _animations = new();
        private string? _currentAnimation;
        private Animation? _currentAnimationInstance;
        private bool _isPlaying;

        public void AddAnimation(string name, Animation animation)
        {
            _animations[name] = animation;
        }

        public void PlayAnimation(string name)
        {
            if (!_animations.TryGetValue(name, out var animation))
                return;

            if (_isPlaying && _currentAnimation == name)
                return;

            _currentAnimation = name;
            _currentAnimationInstance = animation;
            _currentAnimationInstance.Reset();
            _isPlaying = true;
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

        public Texture2D? GetCurrentTexture()
        {
            return _currentAnimationInstance?.Texture;
        }

        public string? CurrentAnimation => _currentAnimation;
        public bool IsPlaying => _isPlaying;
    }
}
