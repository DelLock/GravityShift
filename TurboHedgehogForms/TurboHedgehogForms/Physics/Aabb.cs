using System.Numerics;

namespace TurboHedgehogForms.Physics
{
    /// <summary>Axis-Aligned Bounding Box.</summary>
    public readonly struct Aabb
    {
        public Vector2 Pos { get; }
        public Vector2 Size { get; }

        public float Left => Pos.X;
        public float Right => Pos.X + Size.X;
        public float Top => Pos.Y;
        public float Bottom => Pos.Y + Size.Y;

        public Aabb(Vector2 pos, Vector2 size)
        {
            Pos = pos;
            Size = size;
        }

        public bool Intersects(Aabb other)
        {
            return !(Right <= other.Left || Left >= other.Right || Bottom <= other.Top || Top >= other.Bottom);
        }
    }
}