using System.Numerics;

namespace TurboHedgehogForms.Entities
{
    public enum MonitorType { Rings10, OneUp }

    public sealed class Monitor : Entity
    {
        public MonitorType Type { get; }
        public Monitor(Vector2 pos, MonitorType type) : base(pos, new Vector2(26, 26))
        {
            Type = type;
        }
    }
}