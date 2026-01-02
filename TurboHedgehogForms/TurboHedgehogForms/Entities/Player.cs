using System;
using System.Numerics;

namespace TurboHedgehogForms.Entities
{
    public sealed class Player : Entity
    {
        public bool IsOnGround { get; set; }

        public int Lives { get; set; } = 3;
        public int Score { get; set; } = 0;

        // Sonic-like: кольца = защита
        public int RingCount { get; set; } = 0;

        // »нвиз после урона
        public bool IsInvulnerable => _invulnLeft > 0f;
        private float _invulnLeft = 0f;

        // —мерть
        public bool IsDead { get; private set; }
        private float _deadTime;

        // ƒвижение
        public float Accel { get; } = 1500f;
        public float Decel { get; } = 1900f;
        public float Friction { get; } = 2400f;
        public float MaxRunSpeed { get; } = 460f;

        public float JumpSpeed { get; } = 640f;   // было 580
        public float Gravity { get; } = 1850f;
        public bool IsHurtLocked => _hurtLockLeft > 0f;
        private float _hurtLockLeft = 0f;

        // –олл/спиндэш
        public bool IsChargingSpinDash { get; private set; }
        public bool IsRolling { get; private set; }

        public float SpinDashCharge { get; private set; } = 0f;
        private bool _downWasDown = false;
        private bool _jumpWasDown = false;
        private bool _downHeld = false;

        private const float SpinChargeMax = 1.25f;      // 0..1.25
        private const float SpinChargeRate = 1.85f;     // зар€д/сек при "пинке"
        private const float SpinLaunchMin = 520f;
        private const float SpinLaunchMax = 1180f;

        private const float GroundFriction = 2200f;     // обычное трение
        private const float RollFriction = 420f;        // меньше => больше инерции

        public Player(Vector2 position) : base(position, new Vector2(26, 34)) { }

        public void UpdateTimers(float dt)
        {
            if (_invulnLeft > 0) _invulnLeft = MathF.Max(0, _invulnLeft - dt);
            if (_hurtLockLeft > 0) _hurtLockLeft = MathF.Max(0, _hurtLockLeft - dt);
        }

        public void StartHurtLock(float seconds)
        {
            _hurtLockLeft = MathF.Max(_hurtLockLeft, seconds);
        }

        public void ApplyInput(float dt, bool left, bool right, bool down, bool jumpDown)
        {
            if (IsDead) return;

            // spin dash logic: удерживаем DOWN на земле, прыжок "пампит" зар€д
            bool downPressed = down && !_downWasDown;
            bool downReleased = !down && _downWasDown;
            _downWasDown = down;

            bool jumpPressed = jumpDown && !_jumpWasDown;
            _jumpWasDown = jumpDown;
            _downHeld = down;

            if (IsOnGround && down)
            {
                // если мы почти стоим Ч можем зар€дить спиндэш
                if (MathF.Abs(Velocity.X) < 45f)
                {
                    IsChargingSpinDash = true;
                    Velocity.X = 0;
                    IsRolling = true;

                    if (jumpPressed)
                        SpinDashCharge = MathF.Min(1f, SpinDashCharge + 0.22f);
                }
                else
                {
                    // ролл на скорости
                    IsRolling = true;
                    IsChargingSpinDash = false;
                }
            }
            else
            {
                // если отпустили down после зар€дки Ч выстреливаемс€
                if (IsChargingSpinDash && downReleased)
                {
                    float launch = 360f + 560f * SpinDashCharge; // 360..920
                    // направление берЄм из последнего движени€ (если стоим, то вправо)
                    float dir = Velocity.X < 0 ? -1 : 1;
                    Velocity.X = dir * launch;

                    IsChargingSpinDash = false;
                    SpinDashCharge = 0f;
                }

                if (!down) IsRolling = false;
                if (!IsOnGround) IsChargingSpinDash = false;
            }

            // если зар€жаем спиндэш Ч обычный ввод по X блокируем
            if (IsChargingSpinDash) return;

            float target = 0;
            if (left) target -= 1;
            if (right) target += 1;

            if (target != 0)
            {
                Velocity.X += target * Accel * dt;
                Velocity.X = Clamp(Velocity.X, -MaxRunSpeed, MaxRunSpeed);
            }
            else
            {
                float fr = IsOnGround ? Friction : (Friction * 0.25f);
                Velocity.X = MoveToward(Velocity.X, 0, fr * dt);
            }

            if (left && Velocity.X > 0) Velocity.X = MoveToward(Velocity.X, 0, Decel * dt);
            if (right && Velocity.X < 0) Velocity.X = MoveToward(Velocity.X, 0, Decel * dt);

            // прыжок
            if (jumpPressed && IsOnGround && !IsChargingSpinDash)
            {
                Velocity.Y = -JumpSpeed;
                IsOnGround = false;
            }
        }

        public void ApplyGravity(float dt)
        {
            if (IsDead)
            {
                // при смерти позвол€ем гравитации т€нуть вниз (вылет за экран)
                Velocity.Y += Gravity * dt;
                return;
            }

            Velocity.Y += Gravity * dt;
            if (Velocity.Y > 1300) Velocity.Y = 1300;
        }

        public void StartInvulnerability(float seconds)
        {
            _invulnLeft = MathF.Max(_invulnLeft, seconds);
        }

        public void DieSonicStyle()
        {
            if (IsDead) return;
            IsDead = true;
            _deadTime = 0;
            Velocity = new Vector2(0, -650); // подпрыгнул вверх
        }

        public void UpdateDeath(float dt)
        {
            if (!IsDead) return;
            _deadTime += dt;
        }

        private static float MoveToward(float value, float target, float maxDelta)
        {
            if (value < target) return MathF.Min(value + maxDelta, target);
            return MathF.Max(value - maxDelta, target);
        }

        private static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}