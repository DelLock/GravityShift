using System.Numerics;
using TurboHedgehogForms.Physics;

namespace TurboHedgehogForms.Entities
{
    /// <summary>
    /// Зона ускорения (псевдо "loop/spiral"). Пока игрок внутри — его подталкивает.
    /// </summary>
    public sealed class SpeedPad
    {
        public Vector2 Position { get; }
        public Vector2 Size { get; }
        public float PushX { get; }
        public float PushY { get; }

        public Aabb Bounds => new(Position, Size);

        public SpeedPad(Vector2 position, Vector2 size, float pushX, float pushY)
        {
            Position = position;
            Size = size;
            PushX = pushX;
            PushY = pushY;
        }
    }
}