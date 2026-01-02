using System.Numerics;

namespace TurboHedgehogForms.Entities
{
    /// <summary>Кольцо, которое вылетает при уроне (как в Sonic). Живет ограниченное время.</summary>
    public sealed class RingParticle : Entity
    {
        public float LifeLeft { get; private set; } = 2.5f;

        public RingParticle(Vector2 position, Vector2 initialVelocity)
            : base(position, new Vector2(14, 14))
        {
            Velocity = initialVelocity;
        }

        public override void Update(float dt)
        {
            LifeLeft -= dt;
            if (LifeLeft <= 0) IsActive = false;
        }
    }
}