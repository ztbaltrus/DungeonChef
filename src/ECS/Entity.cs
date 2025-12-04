using System;
using System.Collections.Generic;
using DungeonChef.Src.ECS.Components;

namespace DungeonChef.Src.ECS
{
    public abstract class Entity
    {
        private readonly Dictionary<Type, IComponent> _components = new();

        protected Entity(string name)
        {
            Name = name;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; }

        public IReadOnlyCollection<IComponent> Components => _components.Values;

        public T AddComponent<T>(T component) where T : class, IComponent
        {
            _components[typeof(T)] = component;
            return component;
        }

        public bool HasComponent<T>() where T : class, IComponent
        {
            return _components.ContainsKey(typeof(T));
        }

        public bool TryGetComponent<T>(out T component) where T : class, IComponent
        {
            if (_components.TryGetValue(typeof(T), out var stored) && stored is T typed)
            {
                component = typed;
                return true;
            }

            component = null!;
            return false;
        }

        public T GetComponent<T>() where T : class, IComponent
        {
            if (!TryGetComponent(out T component))
            {
                throw new InvalidOperationException($"{Name} is missing required component {typeof(T).Name}.");
            }

            return component;
        }
    }
}
