using System.Numerics;

namespace TurboHedgehogForms.Entities
{
    /// <summary>¬раг, который ходит туда-сюда между двум€ точками.</summary>
    public sealed class EnemyPatrol : Entity
    {
        private readonly float _leftX;
        private readonly float _rightX;

        public EnemyPatrol(Vector2 position, float leftX, float rightX) : base(position, new Vector2(28, 20))
        {
            _leftX = leftX;
            _rightX = rightX;
            Velocity = new Vector2(60, 0);
        }

        public override void Update(float dt)
        {
            Position += Velocity * dt;

            if (Position.X < _leftX)
            {
                Position.X = _leftX;
                Velocity.X = System.MathF.Abs(Velocity.X);
            }
            else if (Position.X + Size.X > _rightX)
            {
                Position.X = _rightX - Size.X;
                Velocity.X = -System.MathF.Abs(Velocity.X);
            }
        }
    }
}