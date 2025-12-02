using DungeonChef.Src.Core;
using DungeonChef.Src.ECS;
using DungeonChef.Src.Entities;
using Microsoft.Xna.Framework;

namespace DungeonChef.Src.Gameplay
{
    //TODO: Not implemented
    public enum CookResult
    {
        Perfect,
        Good,
        Ok,
        Fail
    }

    public sealed class CookingSession
    {
        public bool Active;
        public float Timer;
        public float TargetTime = 2.0f;
        public float WindowPerfect = 0.1f;
        public float WindowGood = 0.25f;
        public float WindowOk = 0.4f;
        public CookResult? Result;
    }

    public sealed class CookingMinigameSystem
    {
        private readonly CookingSession _session = new();

        public void Update(World world, GameTime gt, InputState input)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            if (!_session.Active)
            {
                // Minimal stub: start cooking when Interact is pressed
                if (input.InteractPressed)
                {
                    _session.Active = true;
                    _session.Timer = 0f;
                    _session.Result = null;
                }
                return;
            }

            _session.Timer += dt;

            if (input.PrimaryPressed && _session.Result == null)
            {
                float diff = System.MathF.Abs(_session.Timer - _session.TargetTime);
                if (diff <= _session.WindowPerfect) _session.Result = CookResult.Perfect;
                else if (diff <= _session.WindowGood) _session.Result = CookResult.Good;
                else if (diff <= _session.WindowOk) _session.Result = CookResult.Ok;
                else _session.Result = CookResult.Fail;

                // Apply a basic buff stub to first player entity
                ApplyDummyBuff(world, _session.Result.Value);
                _session.Active = false;
            }
        }

        private void ApplyDummyBuff(World world, CookResult result)
        {
            var player = world.Entities.Find(e => e.GetType() == typeof(Player));
            if (player == null) return;

            float duration = result switch
            {
                CookResult.Perfect => 20f,
                CookResult.Good => 15f,
                CookResult.Ok => 10f,
                _ => 5f
            };

            BuffSystem.AddBuff(player, "cook_tmp_attack", duration);
        }
    }
}
