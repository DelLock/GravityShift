using System.Numerics;

namespace TurboHedgehogForms.Entities
{
    /// <summary>Неподвижная платформа/земля.</summary>
    public sealed class Platform : Entity
    {
        public Platform(Vector2 position, Vector2 size) : base(position, size) { }
    }
}