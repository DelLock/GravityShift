using System.Numerics;

namespace TurboHedgehogForms.Entities
{
    public sealed class Boss : Entity
    {
        public int Hp { get; private set; } = 8;

        private float _aiTimer;
        private int _dir = 1;

        public bool Defeated => Hp <= 0;

        public Boss(Vector2 position) : base(position, new Vector2(64, 48))
        {
            Velocity = new Vector2(80, 0);
        }

        public override void Update(float dt)
        {
            _aiTimer += dt;

            // простая ИИ: ходит туда-сюда + иногда ускоряется
            float speed = (_aiTimer % 3.2f < 0.8f) ? 190f : 90f;
            Velocity.X = _dir * speed;

            Position += Velocity * dt;

            // границы “арены” (примерно)
            if (Position.X < 2940) { Position.X = 2940; _dir = 1; }
            if (Position.X > 3620) { Position.X = 3620; _dir = -1; }
        }

        public void TakeHit()
        {
            if (Hp <= 0) return;
            Hp--;
        }
    }
}