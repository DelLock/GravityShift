using System.Drawing;
using System.Numerics;

namespace TurboHedgehogForms.Game
{
    public sealed class Camera2D
    {
        public Vector2 Position { get; private set; }
        public Size ViewSize { get; set; }

        public bool IsFrozen { get; private set; }
        private Vector2 _frozenPos;

        public void Freeze()
        {
            IsFrozen = true;
            _frozenPos = Position;
        }

        public void Unfreeze()
        {
            IsFrozen = false;
        }

        public void SetPosition(Vector2 pos)
        {
            Position = pos;
            if (IsFrozen) _frozenPos = pos;
        }

        public void Follow(Vector2 targetCenter)
        {
            if (IsFrozen)
            {
                Position = _frozenPos;
                return;
            }

            Position = targetCenter - new Vector2(ViewSize.Width / 2f, ViewSize.Height / 2f);

            if (Position.X < 0) Position = new Vector2(0, Position.Y);
            if (Position.Y < 0) Position = new Vector2(Position.X, 0);
        }
    }
}