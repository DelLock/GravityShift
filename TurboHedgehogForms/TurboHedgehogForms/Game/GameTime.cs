using System;
using System.Diagnostics;

namespace TurboHedgehogForms.Game
{
    /// <summary>ѕростой таймер: считает deltaTime дл€ игрового цикла.</summary>
    public sealed class GameTime
    {
        private readonly Stopwatch _sw = new();
        private long _lastTicks;

        public float DeltaTime { get; private set; } = 1f / 60f;
        public float TotalTime { get; private set; }

        public void Reset()
        {
            _sw.Restart();
            _lastTicks = _sw.ElapsedTicks;
            DeltaTime = 1f / 60f;
            TotalTime = 0f;
        }

        public void Step()
        {
            long now = _sw.ElapsedTicks;
            long diff = now - _lastTicks;
            _lastTicks = now;

            double seconds = (double)diff / Stopwatch.Frequency;
            // защита от огромных скачков
            seconds = Math.Clamp(seconds, 0.0, 0.05);

            DeltaTime = (float)seconds;
            TotalTime += DeltaTime;
        }
    }
}