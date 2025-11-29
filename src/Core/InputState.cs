using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DungeonChef.Src.Core
{
    public struct InputState
    {
        public Vector2 Move;
        public bool DashPressed;
        public bool PrimaryPressed;
        public bool SpecialPressed;
        public bool InteractPressed;
        public bool ExitRequested;
    }

    public sealed class Input
    {
        public InputState State;

        public void Update()
        {
            var k = Keyboard.GetState();

            Vector2 move = Vector2.Zero;

            if (k.IsKeyDown(Keys.W)) move.Y -= 1f; // up on screen
            if (k.IsKeyDown(Keys.S)) move.Y += 1f; // down
            if (k.IsKeyDown(Keys.A)) move.X -= 1f; // left
            if (k.IsKeyDown(Keys.D)) move.X += 1f; // right

            State.Move = move;

            State.DashPressed    = k.IsKeyDown(Keys.Space);
            State.PrimaryPressed = k.IsKeyDown(Keys.J);
            State.SpecialPressed = k.IsKeyDown(Keys.K);
        }
    }
}
