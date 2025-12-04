using DungeonChef.Src.ECS;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DungeonChef.Src.Rendering
{
    public sealed class AnimationClip
    {
        public string Name = string.Empty;
        public int FrameCount;
        public float FrameDuration;
    }

    public sealed class SpriteAnimator
    {
        public Dictionary<string, AnimationClip> Clips { get; } = new();
        public string Current = "idle";
        public float Time;
        public int Frame;

        public void Update(GameTime gt)
        {
            if (!Clips.TryGetValue(Current, out var clip)) return;

            Time += (float)gt.ElapsedGameTime.TotalSeconds;
            while (Time > clip.FrameDuration)
            {
                Time -= clip.FrameDuration;
                Frame = (Frame + 1) % clip.FrameCount;
            }
        }
    }
}
