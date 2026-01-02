using System.Numerics;

namespace TurboHedgehogForms.Entities
{
    /// <summary>
    /// Скат/склон. Не AABB-коллайдер: даёт поверхность Y=f(x) на интервале X.
    /// Реализация упрощённая для Sonic-like ощущения.
    /// </summary>
    public sealed class RampPlatform
    {
        public Vector2 Start { get; }
        public float Width { get; }
        public float Height { get; }

        // + accel по X, если стоим на склоне (имитация гравитации вдоль поверхности)
        public float SlopeAccelX { get; }

        // true: вверх вправо, false: вниз вправо
        public bool UpRight { get; }

        private RampPlatform(Vector2 start, float width, float height, bool upRight)
        {
            Start = start;
            Width = width;
            Height = height;
            UpRight = upRight;

            // Небольшая "помощь моментуму"
            // вниз вправо => ускоряет вправо, вверх вправо => тормозит вправо
            SlopeAccelX = upRight ? -14f : +14f;
        }

        public static RampPlatform Up(Vector2 start, float width, float height) => new(start, width, height, upRight: true);
        public static RampPlatform Down(Vector2 start, float width, float height) => new(start, width, height, upRight: false);

        public bool ContainsX(float x) => x >= Start.X && x <= Start.X + Width;

        public float GetSurfaceY(float x)
        {
            float t = (x - Start.X) / Width; // 0..1

            // Start.Y — низ рампы (внизу). SurfaceY — это Y поверхности.
            // Для UpRight: слева ниже, справа выше (Y меньше = выше на экране? в WinForms Y вниз)
            // В нашей системе Y вниз, значит "выше" = меньше Y.
            // Поэтому: upRight => surfaceY = Start.Y - t*Height
            // downRight => surfaceY = Start.Y - (1-t)*Height (в начале выше, затем ниже)
            if (UpRight)
                return Start.Y - t * Height;
            else
                return (Start.Y - Height) + t * Height;
        }
    }
}