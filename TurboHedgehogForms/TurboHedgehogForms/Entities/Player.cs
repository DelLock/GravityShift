using System.Numerics;

namespace TurboHedgehogForms.Entities
{
    /// <summary>
    /// »грок с ускорением/инерцией "как у соника" (упрощенно).
    /// </summary>
    public sealed class Player : Entity
    {
        public bool IsOnGround { get; set; }
        public int Lives { get; set; } = 3;
        public int Score { get; set; } = 0;

        // Sonic-like параметры
        public float Accel { get; } = 1400f;
        public float Decel { get; } = 1800f;
        public float Friction { get; } = 2200f;
        public float MaxRunSpeed { get; } = 420f;
        public float JumpSpeed { get; } = 560f;
        public float Gravity { get; } = 1800f;

        private bool _jumpWasDown;

        public Player(Vector2 position) : base(position, new Vector2(26, 34)) { }

        public void ApplyInput(float dt, bool left, bool right, bool jumpDown)
        {
            float target = 0;
            if (left) target -= 1;
            if (right) target += 1;

            if (target != 0)
            {
                // ускор€емс€ в направлении нажати€
                Velocity.X += target * Accel * dt;
                Velocity.X = Game.MathUtil.Clamp(Velocity.X, -MaxRunSpeed, MaxRunSpeed);
            }
            else
            {
                // трение: если на земле Ч сильнее
                float fr = IsOnGround ? Friction : (Friction * 0.25f);
                Velocity.X = MoveToward(Velocity.X, 0, fr * dt);
            }

            // если мен€ем направление Ч небольша€ "декораци€" ощущением торможени€
            if (left && Velocity.X > 0) Velocity.X = MoveToward(Velocity.X, 0, Decel * dt);
            if (right && Velocity.X < 0) Velocity.X = MoveToward(Velocity.X, 0, Decel * dt);

            // прыжок по нажатию (edge)
            bool jumpPressed = jumpDown && !_jumpWasDown;
            _jumpWasDown = jumpDown;

            if (jumpPressed && IsOnGround)
            {
                Velocity.Y = -JumpSpeed;
                IsOnGround = false;
            }
        }

        public void ApplyGravity(float dt)
        {
            Velocity.Y += Gravity * dt;
            if (Velocity.Y > 1200) Velocity.Y = 1200;
        }

        private static float MoveToward(float value, float target, float maxDelta)
        {
            if (value < target) return System.MathF.Min(value + maxDelta, target);
            return System.MathF.Max(value - maxDelta, target);
        }
    }
}