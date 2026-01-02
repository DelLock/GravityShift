using System.Numerics;

namespace TurboHedgehogForms.Entities
{
    public enum PlatformKind
    {
        Ground,
        Floating
    }

    public sealed class Platform : Entity
    {
        public PlatformKind Kind { get; }

        public Platform(Vector2 position, Vector2 size, PlatformKind kind = PlatformKind.Ground)
            : base(position, size)
        {
            Kind = kind;
        }
    }
}