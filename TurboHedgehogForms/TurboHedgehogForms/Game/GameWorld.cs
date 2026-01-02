using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TurboHedgehogForms.Entities;
using TurboHedgehogForms.Input;
using TurboHedgehogForms.Physics;

namespace TurboHedgehogForms.Game
{
    /// <summary>Хранит уровень, обновляет сущности, обрабатывает столкновения.</summary>
    public sealed class GameWorld
    {
        public Player Player { get; private set; } = new Player(new Vector2(80, 200));
        public Camera2D Camera { get; } = new();

        public List<Platform> Platforms { get; } = new();
        public List<Ring> Rings { get; } = new();
        public List<EnemyPatrol> Enemies { get; } = new();

        private Vector2 _spawnPoint = new(80, 200);

        public void BuildDemoLevel()
        {
            Platforms.Clear();
            Rings.Clear();
            Enemies.Clear();

            Player = new Player(_spawnPoint);

            // Земля
            Platforms.Add(new Platform(new Vector2(0, 460), new Vector2(2200, 80)));

            // Платформы
            Platforms.Add(new Platform(new Vector2(220, 380), new Vector2(220, 20)));
            Platforms.Add(new Platform(new Vector2(520, 330), new Vector2(220, 20)));
            Platforms.Add(new Platform(new Vector2(860, 290), new Vector2(260, 20)));
            Platforms.Add(new Platform(new Vector2(1250, 360), new Vector2(240, 20)));

            // "Ступеньки" чтобы было чувство скорости
            Platforms.Add(new Platform(new Vector2(1550, 420), new Vector2(90, 20)));
            Platforms.Add(new Platform(new Vector2(1650, 400), new Vector2(90, 20)));
            Platforms.Add(new Platform(new Vector2(1750, 380), new Vector2(90, 20)));

            // Кольца
            for (int i = 0; i < 10; i++)
                Rings.Add(new Ring(new Vector2(260 + i * 30, 340)));

            for (int i = 0; i < 6; i++)
                Rings.Add(new Ring(new Vector2(900 + i * 30, 250)));

            // Враги
            Enemies.Add(new EnemyPatrol(new Vector2(600, 310), leftX: 520, rightX: 740));
            Enemies.Add(new EnemyPatrol(new Vector2(1350, 340), leftX: 1250, rightX: 1490));
        }

        public void Update(GameTime time, InputState input)
        {
            float dt = time.DeltaTime;

            if (input.Restart)
                BuildDemoLevel();

            // обновление врагов (патруль)
            foreach (var e in Enemies.Where(x => x.IsActive))
                e.Update(dt);

            // ввод игрока
            Player.ApplyInput(dt, input.Left, input.Right, input.Jump);

            // физика игрока
            Player.ApplyGravity(dt);
            MoveAndCollidePlayer(dt);

            // сбор колец
            foreach (var r in Rings.Where(x => x.IsActive))
            {
                if (Player.Bounds.Intersects(r.Bounds))
                {
                    r.IsActive = false;
                    Player.Score += r.ScoreValue;
                }
            }

            // столкновения с врагами
            foreach (var enemy in Enemies.Where(x => x.IsActive))
            {
                if (!Player.Bounds.Intersects(enemy.Bounds)) continue;

                // если игрок падает сверху — уничтожаем врага и подпрыгиваем
                bool hitFromAbove = Player.Velocity.Y > 0 && Player.Bounds.Bottom - enemy.Bounds.Top < 14f;
                if (hitFromAbove)
                {
                    enemy.IsActive = false;
                    Player.Score += 100;
                    Player.Velocity.Y = -340;
                }
                else
                {
                    // урон
                    Player.Lives--;
                    Respawn();
                    break;
                }
            }

            // камера
            Camera.Follow(Player.Position + Player.Size / 2f);
        }

        private void Respawn()
        {
            if (Player.Lives <= 0)
            {
                BuildDemoLevel();
                return;
            }

            Player.Position = _spawnPoint;
            Player.Velocity = Vector2.Zero;
        }

        private void MoveAndCollidePlayer(float dt)
        {
            Player.IsOnGround = false;

            // Двигаем по X
            Player.Position.X += Player.Velocity.X * dt;
            ResolvePlatforms(axisY: false);

            // Двигаем по Y
            Player.Position.Y += Player.Velocity.Y * dt;
            ResolvePlatforms(axisY: true);

            // если упал ниже мира
            if (Player.Position.Y > 900)
            {
                Player.Lives--;
                Respawn();
            }
        }

        private void ResolvePlatforms(bool axisY)
        {
            foreach (var p in Platforms)
            {
                var hit = Collision.Solve(Player.Bounds, p.Bounds);
                if (hit.Side == HitSide.None) continue;

                Player.Position += hit.Correction;

                if (axisY)
                {
                    if (hit.Side == HitSide.Top)
                    {
                        Player.Velocity.Y = 0;
                        Player.IsOnGround = true;
                    }
                    else if (hit.Side == HitSide.Bottom)
                    {
                        Player.Velocity.Y = 0;
                    }
                }
                else
                {
                    if (hit.Side == HitSide.Left || hit.Side == HitSide.Right)
                        Player.Velocity.X = 0;
                }
            }
        }
    }
}