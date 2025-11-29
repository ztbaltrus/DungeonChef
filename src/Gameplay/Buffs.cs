using DungeonChef.Src.ECS;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DungeonChef.Src.Gameplay
{
    public sealed class ActiveBuff
    {
        public string Id = string.Empty;
        public float Remaining;
    }

    public sealed class BuffData
    {
        public string Id = string.Empty;
        public float AttackMultiplier;
    }

    public sealed class BuffComponent
    {
        public readonly List<ActiveBuff> Active = new();
    }

    public sealed class BuffSystem
    {
        private static readonly Dictionary<Entity, BuffComponent> _buffs = new();

        public static void AddBuff(Entity entity, string id, float duration)
        {
            if (!_buffs.TryGetValue(entity, out var comp))
            {
                comp = new BuffComponent();
                _buffs[entity] = comp;
            }

            comp.Active.Add(new ActiveBuff { Id = id, Remaining = duration });
        }

        public static BuffComponent? Get(Entity entity)
        {
            _buffs.TryGetValue(entity, out var comp);
            return comp;
        }

        public void Update(ECS.World world, GameTime gt)
        {
            float dt = (float)gt.ElapsedGameTime.TotalSeconds;

            foreach (var kvp in _buffs)
            {
                var list = kvp.Value.Active;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    list[i].Remaining -= dt;
                    if (list[i].Remaining <= 0f)
                        list.RemoveAt(i);
                }
            }
        }

        public static float GetAttackMultiplier(Entity entity)
        {
            if (!_buffs.TryGetValue(entity, out var comp))
                return 1f;

            float mult = 1f;
            foreach (var b in comp.Active)
            {
                if (b.Id == "cook_tmp_attack")
                    mult *= 1.1f;
            }

            return mult;
        }
    }
}
