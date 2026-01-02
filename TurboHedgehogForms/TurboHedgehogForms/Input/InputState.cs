using System.Collections.Generic;
using System.Windows.Forms;

namespace TurboHedgehogForms.Input
{
    /// <summary>Хранит состояние клавиш. Подходит для WinForms без сторонних библиотек.</summary>
    public sealed class InputState
    {
        private readonly HashSet<Keys> _down = new();

        public bool Left => IsDown(Keys.A) || IsDown(Keys.Left);
        public bool Right => IsDown(Keys.D) || IsDown(Keys.Right);
        public bool Jump => IsDown(Keys.Space) || IsDown(Keys.W) || IsDown(Keys.Up);
        public bool Restart => IsDown(Keys.R);

        public void SetKey(Keys key, bool pressed)
        {
            if (pressed) _down.Add(key);
            else _down.Remove(key);
        }

        public bool IsDown(Keys key) => _down.Contains(key);
    }
}