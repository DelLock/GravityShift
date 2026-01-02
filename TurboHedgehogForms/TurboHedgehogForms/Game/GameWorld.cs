using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TurboHedgehogForms.Entities;
using TurboHedgehogForms.Input;
using TurboHedgehogForms.Physics;

namespace TurboHedgehogForms.Game
{
    public sealed class GameWorld
    {
        public GameState State { get; private set; } = GameState.Title;

        public LevelId CurrentLevel { get; private set; } = LevelId.Act1;

        public Player Player { get; private set; } = new Player(new Vector2(80, 200));
        public Camera2D Camera { get; } = new();

        public List<Platform> Platforms { get; } = new();
        public List<Ring> Rings { get; } = new();
        public List<EnemyPatrol> Enemies { get; } = new();
        public List<RingParticle> RingParticles { get; } = new();

        public FinishFlag Finish { get; private set; } = new FinishFlag(new Vector2(0, 0));
        public Boss? Boss { get; private set; }

        private Vector2 _spawnPoint;
        private float _stateTimer;

        // УкапсулаФ/финиш в акте 3 по€вл€етс€ только после победы над боссом
        private bool _finishUnlocked = false;

        public void GoToTitle()
        {
            State = GameState.Title;
            _stateTimer = 0;
        }

        public void StartNewGame()
        {
            CurrentLevel = LevelId.Act1;
            LoadLevel(CurrentLevel);

            Player.Lives = 3;
            Player.Score = 0;
            Player.RingCount = 0;

            State = GameState.Playing;
            _stateTimer = 0;
        }

        private void LoadLevel(LevelId id)
        {
            LevelFactory.Build(id, Platforms, Rings, Enemies, out var finish, out var boss, out var spawn);
            Finish = finish;
            Boss = boss;
            _spawnPoint = spawn;

            Player = new Player(_spawnPoint)
            {
                Lives = Player.Lives,
                Score = Player.Score,
                RingCount = Player.RingCount
            };

            RingParticles.Clear();
            _finishUnlocked = (id != LevelId.Act3_Boss);
        }

        public void Update(GameTime time, InputState input)
        {
            float dt = time.DeltaTime;
            _stateTimer += dt;

            if (input.Restart)
            {
                GoToTitle();
                return;
            }

            if (State == GameState.Title)
            {
                if (input.Enter) StartNewGame();
                return;
            }

            if (State == GameState.LevelCompleted || State == GameState.GameOver)
            {
                if (input.Enter) GoToTitle();
                return;
            }

            // ===== PLAYING =====

            // ќбновление таймеров игрока
            Player.UpdateTimers(dt);

            // Ѕосс »»
            if (Boss != null && !Boss.Defeated)
                Boss.Update(dt);

            // ¬раги
            foreach (var e in Enemies.Where(x => x.IsActive))
                e.Update(dt);

            // Ћет€щие кольца
            foreach (var rp in RingParticles.Where(x => x.IsActive))
            {
                rp.Velocity += new Vector2(0, 1900f) * dt; // гравитаци€
                rp.Position += rp.Velocity * dt;
                rp.Update(dt);

                // проста€ Уземл€Ф (чтобы подпрыгивали/останавливались чуть)
                foreach (var pl in Platforms)
                {
                    var hit = Collision.Solve(rp.Bounds, pl.Bounds);
                    if (hit.Side == HitSide.None) continue;

                    rp.Position += hit.Correction;
                    if (hit.Side == HitSide.Top)
                    {
                        rp.Velocity = new Vector2(rp.Velocity.X * 0.7f, -rp.Velocity.Y * 0.45f);
                        if (MathF.Abs(rp.Velocity.Y) < 120f) rp.Velocity = new Vector2(rp.Velocity.X, 0);
                    }
                }
            }

            RingParticles.RemoveAll(x => !x.IsActive);

            // ≈сли игрок умер Ч просто анимаци€ смерти
            if (Player.IsDead)
            {
                Player.ApplyGravity(dt);
                Player.Position += Player.Velocity * dt;

                // улетел за экран вниз Ч тер€ем жизнь и либо game over, либо рестарт уровн€
                if (Player.Position.Y > Camera.Position.Y + Camera.ViewSize.Height + 200)
                {
                    Player.Lives--;
                    if (Player.Lives <= 0)
                    {
                        State = GameState.GameOver;
                        return;
                    }

                    // перезагружаем текущий уровень
                    LoadLevel(CurrentLevel);
                }

                Camera.Follow(Player.Position + Player.Size / 2f);
                return;
            }

            // ¬вод/движение игрока
            Player.ApplyInput(dt, input.Left, input.Right, input.Down, input.Jump);

            Player.ApplyGravity(dt);
            MoveAndCollidePlayer(dt);

            // сбор обычных колец
            foreach (var ring in Rings.Where(x => x.IsActive))
            {
                if (Player.Bounds.Intersects(ring.Bounds))
                {
                    ring.IsActive = false;
                    Player.RingCount += 1;
                    Player.Score += ring.ScoreValue;
                }
            }

            // сбор УвылетевшихФ колец
            foreach (var rp in RingParticles.Where(x => x.IsActive))
            {
                if (Player.Bounds.Intersects(rp.Bounds))
                {
                    rp.IsActive = false;
                    Player.RingCount += 1;
                    Player.Score += 1; // можно 0 или 1, на вкус
                }
            }

            // столкновени€ с врагами (урон/атака)
            foreach (var enemy in Enemies.Where(x => x.IsActive))
            {
                if (!Player.Bounds.Intersects(enemy.Bounds)) continue;

                if (Player.IsInvulnerable) break;

                bool stomp = Player.Velocity.Y > 0 && Player.Bounds.Bottom - enemy.Bounds.Top < 14f;

                // ≈сли игрок в ролле/спин-атаке или падает сверху Ч уничтожаем
                if (stomp || Player.IsRolling)
                {
                    enemy.IsActive = false;
                    Player.Score += 100;
                    Player.Velocity.Y = -340;
                }
                else
                {
                    HurtFrom(enemy.Position);
                    break;
                }
            }

            // столкновени€ с боссом
            if (Boss != null && !Boss.Defeated && Player.Bounds.Intersects(Boss.Bounds))
            {
                if (!Player.IsInvulnerable)
                {
                    bool stompBoss = Player.Velocity.Y > 0 && Player.Bounds.Bottom - Boss.Bounds.Top < 18f;

                    if (stompBoss || Player.IsRolling)
                    {
                        Boss.TakeHit();
                        Player.Score += 500;
                        Player.Velocity.Y = -420;
                        Player.StartInvulnerability(0.6f);

                        if (Boss.Defeated)
                            _finishUnlocked = true;
                    }
                    else
                    {
                        HurtFrom(Boss.Position);
                    }
                }
            }

            // конец уровн€ (дл€ акта 3 Ч только если босс побежден)
            if (_finishUnlocked && Player.Bounds.Intersects(Finish.Bounds))
            {
                AdvanceLevelOrWin();
                return;
            }

            // камера
            Camera.Follow(Player.Position + Player.Size / 2f);
        }

        private void AdvanceLevelOrWin()
        {
            if (CurrentLevel == LevelId.Act1)
            {
                CurrentLevel = LevelId.Act2;
                LoadLevel(CurrentLevel);
                return;
            }

            if (CurrentLevel == LevelId.Act2)
            {
                CurrentLevel = LevelId.Act3_Boss;
                LoadLevel(CurrentLevel);
                return;
            }

            // победа в игре
            State = GameState.LevelCompleted;
        }

        private void HurtFrom(Vector2 hazardPos)
        {
            // ≈сли есть кольца Ч вылетают. »наче Ч смерть Укак в соникеФ.
            if (Player.RingCount > 0)
            {
                SpawnRingBurst(Player.Position + Player.Size / 2f, Player.RingCount);
                Player.RingCount = 0;

                // отбрасывание от врага
                float dir = (Player.Position.X + Player.Size.X / 2f) < hazardPos.X ? -1f : 1f;
                Player.Velocity = new Vector2(320f * dir, -420f);

                Player.StartInvulnerability(1.6f);
            }
            else
            {
                Player.DieSonicStyle();
                Player.StartInvulnerability(999f); // чтобы не триггерить повторно
            }
        }

        private void SpawnRingBurst(Vector2 center, int rings)
        {
            // как в Sonic: ограничим число вылетающих колец
            int count = Math.Min(rings, 20);

            float baseSpeed = 520f;
            float spread = 2.3f; // радианы УвееромФ
            float startAngle = -MathF.PI / 2f - spread / 2f;

            for (int i = 0; i < count; i++)
            {
                float t = (count == 1) ? 0.5f : i / (count - 1f);
                float ang = startAngle + t * spread;

                var v = new Vector2(MathF.Cos(ang), MathF.Sin(ang)) * baseSpeed;
                v.Y -= 120f; // чуть сильнее вверх

                RingParticles.Add(new RingParticle(center, v));
            }
        }

        private void MoveAndCollidePlayer(float dt)
        {
            Player.IsOnGround = false;

            // X
            Player.Position.X += Player.Velocity.X * dt;
            ResolvePlatforms(axisY: false);

            // Y
            Player.Position.Y += Player.Velocity.Y * dt;
            ResolvePlatforms(axisY: true);

            // падение ниже мира Ч в сонике это смерть
            if (Player.Position.Y > 1200)
            {
                Player.DieSonicStyle();
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