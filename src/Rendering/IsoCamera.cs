using DungeonChef.Src.Core;
using DungeonChef.Src.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DungeonChef.Src.Rendering
{
    public sealed class IsoCamera
    {
        // Position is a screen-space offset in pixels
        public Vector2 Position;
        public float Zoom = 1f;

        // How quickly camera catches up to target
        public float FollowLerp = 10f;

        // Optional manual pan speed if you still want arrow-key nudge
        public float PanSpeed = 600f;

        public void UpdateFollow(GameTime gt, Vector2 playerWorldPos)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            // Convert world → iso screen coords
            Vector2 playerIso = IsoMath.ToScreen(playerWorldPos.X, playerWorldPos.Y);

            // Where we want the camera centered
            Vector2 screenCenter = new Vector2(
                GameRoot.VirtualWidth / 2f,
                GameRoot.VirtualHeight / 2f
            );

            // Desired camera offset so player appears in center
            Vector2 desired = playerIso - screenCenter;

            // Smooth follow
            Position = Vector2.Lerp(Position, desired, 10f * dt);
        }
    }
}
