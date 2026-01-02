using System.Numerics;

namespace TurboHedgehogForms.Entities
{
    public sealed class Spring : Entity
    {
        public float LaunchY { get; } = 920f;
        public Spring(Vector2 pos) : base(pos, new Vector2(28, 18)) { }
    }
}