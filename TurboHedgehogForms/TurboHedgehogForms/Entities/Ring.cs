using System.Numerics;

namespace TurboHedgehogForms.Entities
{
    /// <summary>Кольцо (коллектабл). При касании — исчезает и дает очки.</summary>
    public sealed class Ring : Entity
    {
        public int ScoreValue { get; } = 10;

        public Ring(Vector2 position) : base(position, new Vector2(16, 16)) { }
    }
}