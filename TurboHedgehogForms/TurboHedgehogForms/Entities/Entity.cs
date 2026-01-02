using System.Numerics;
using TurboHedgehogForms.Physics;

namespace TurboHedgehogForms.Entities
{
    /// <summary>Базовая сущность уровня.</summary>
    public abstract class Entity
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Size;

        public bool IsActive { get; set; } = true;

        public Aabb Bounds => new(Position, Size);

        protected Entity(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public virtual void Update(float dt) { }
    }
}