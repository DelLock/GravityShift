namespace TurboHedgehogForms.Game
{
    /// <summary>Утилиты математики для старых версий .NET.</summary>
    public static class MathUtil
    {
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}