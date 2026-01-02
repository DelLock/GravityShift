using System.Numerics;

namespace TurboHedgehogForms.Physics
{
    public enum HitSide { None, Top, Bottom, Left, Right }

    public readonly struct Hit
    {
        public HitSide Side { get; }
        public Vector2 Correction { get; }

        public Hit(HitSide side, Vector2 correction)
        {
            Side = side;
            Correction = correction;
        }
    }

    /// <summary>—толкновение AABB с минимальным вектором разделени€.</summary>
    public static class Collision
    {
        public static Hit Solve(Aabb moving, Aabb solid)
        {
            if (!moving.Intersects(solid)) return new Hit(HitSide.None, Vector2.Zero);

            float moveLeft = solid.Left - moving.Right;
            float moveRight = solid.Right - moving.Left;
            float moveUp = solid.Top - moving.Bottom;
            float moveDown = solid.Bottom - moving.Top;

            // выбираем минимальную по модулю коррекцию
            float absX = System.MathF.Min(System.MathF.Abs(moveLeft), System.MathF.Abs(moveRight));
            float absY = System.MathF.Min(System.MathF.Abs(moveUp), System.MathF.Abs(moveDown));

            if (absX < absY)
            {
                // исправл€ем по X
                if (System.MathF.Abs(moveLeft) < System.MathF.Abs(moveRight))
                    return new Hit(HitSide.Left, new Vector2(moveLeft, 0));
                return new Hit(HitSide.Right, new Vector2(moveRight, 0));
            }
            else
            {
                // исправл€ем по Y
                if (System.MathF.Abs(moveUp) < System.MathF.Abs(moveDown))
                    return new Hit(HitSide.Top, new Vector2(0, moveUp));
                return new Hit(HitSide.Bottom, new Vector2(0, moveDown));
            }
        }
    }
}