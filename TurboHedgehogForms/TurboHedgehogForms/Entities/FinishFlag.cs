using System.Numerics;

namespace TurboHedgehogForms.Entities
{
    /// <summary>Финиш уровня. При касании игроком завершает уровень.</summary>
    public sealed class FinishFlag : Entity
    {
        public FinishFlag(Vector2 position) : base(position, new Vector2(24, 64)) { }
    }
}