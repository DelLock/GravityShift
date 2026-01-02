using System.Collections.Generic;
using System.Windows.Forms;

namespace TurboHedgehogForms.Input
{
    public sealed class InputState
    {
        private readonly HashSet<Keys> _down = new();

        public bool Left => IsDown(Keys.A) || IsDown(Keys.Left);
        public bool Right => IsDown(Keys.D) || IsDown(Keys.Right);
        public bool Up => IsDown(Keys.W) || IsDown(Keys.Up);
        public bool Down => IsDown(Keys.S) || IsDown(Keys.Down);

        public bool Jump => IsDown(Keys.Space) || Up;
        public bool Restart => IsDown(Keys.R);
        public bool Enter => IsDown(Keys.Enter);

        public void SetKey(Keys key, bool pressed)
        {
            if (pressed) _down.Add(key);
            else _down.Remove(key);
        }

        public bool IsDown(Keys key) => _down.Contains(key);
    }
}