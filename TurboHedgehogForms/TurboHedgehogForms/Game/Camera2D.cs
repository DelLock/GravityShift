using System.Drawing;
using System.Numerics;

namespace TurboHedgehogForms.Game
{
    /// <summary>Простейшая камера: смещение мира относительно экрана.</summary>
    public sealed class Camera2D
    {
        public Vector2 Position { get; private set; }
        public Size ViewSize { get; set; }

        public void Follow(Vector2 targetCenter)
        {
            // центрируем камеру на игроке
            Position = targetCenter - new Vector2(ViewSize.Width / 2f, ViewSize.Height / 2f);

            // немного ограничений, чтобы не уходить в отрицательные координаты
            if (Position.X < 0) Position = new Vector2(0, Position.Y);
            if (Position.Y < 0) Position = new Vector2(Position.X, 0);
        }
    }
}