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
        public List<RampPlatform> Ramps { get; } = new();
        public List<SpeedPad> SpeedPads { get; } = new();

        public List<Ring> Rings { get; } = new();
        public List<EnemyPatrol> Enemies { get; } = new();

        public List<RingParticle> RingParticles { get; } = new();

        public FinishFlag Finish { get; private set; } = new FinishFlag(new Vector2(0, 0));
        public Boss? Boss { get; private set; }

        private Vector2 _spawnPoint;
        private bool _finishUnlocked;
        private readonly Random _rng = new();

        public void GoToTitle()
        {
            State = GameState.Title;
        }

        public void StartNewGame()
        {
            CurrentLevel = LevelId.Act1;

            // стартовые статы
            int lives = 3;
            int score = 0;
            int rings = 0;

            LoadLevel(CurrentLevel, lives, score, rings);

            State = GameState.Playing;
        }

        private void LoadLevel(LevelId id, int lives, int score, int ringCount)
        {
            LevelFactory.Build(id, Platforms, Ramps, SpeedPads, Rings, Enemies, out var finish, out var boss, out var spawn);
            Finish = finish;
            Boss = boss;
            _spawnPoint = spawn;

            Player = new Player(_spawnPoint)
            {
                Lives = lives,
                Score = score,
                RingCount = ringCount
            };

            RingParticles.Clear();

            _finishUnlocked = (id != LevelId.Act3_Boss);
        }

        public void Update(GameTime time, InputState input)
        {
            float dt = time.DeltaTime;

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
            Player.UpdateTimers(dt);

            // обновление босса/врагов
            if (Boss != null && !Boss.Defeated)
                Boss.Update(dt);

            foreach (var e in Enemies.Where(x => x.IsActive))
                e.Update(dt);

            // обновление "вылетевших колец"
            UpdateRingParticles(dt);

            // смерть: камера статична, респавн сразу при вылете
            if (Player.IsDead)
            {
                Player.ApplyGravity(dt);
                Player.Position += Player.Velocity * dt;

                // камера заморожена
                float screenBottom = Camera.Position.Y + Camera.ViewSize.Height;
                if (Player.Position.Y > screenBottom + 200)
                {
                    Player.Lives--;
                    if (Player.Lives <= 0)
                    {
                        State = GameState.GameOver;
                        Camera.Unfreeze();
                        return;
                    }

                    // мгновенный респавн
                    int lives = Player.Lives;
                    int score = Player.Score;
                    int rings = 0; // как в Sonic 1 — после смерти кольца теряются

                    LoadLevel(CurrentLevel, lives, score, rings);

                    Camera.Unfreeze();
                    Camera.Follow(Player.Position + Player.Size / 2f);
                }

                return;
            }

            // ввод
            Player.ApplyInput(dt, input.Left, input.Right, input.Down, input.Jump);

            // гравитация/движение
            Player.ApplyGravity(dt);
            MoveAndCollidePlayer(dt);

            // SpeedPads (псевдо-петля/спираль)
            ApplySpeedPads(dt);

            // Сбор колец запрещён пока HurtLock активен (отлет)
            if (!Player.IsHurtLocked)
            {
                CollectLevelRings();
                CollectBurstRings();
            }

            // враги
            HandleEnemyContacts();

            // босс
            HandleBossContacts();

            // конец уровня
            if (_finishUnlocked && Player.Bounds.Intersects(Finish.Bounds))
            {
                AdvanceLevelOrWin();
                return;
            }

            // камера
            Camera.Follow(Player.Position + Player.Size / 2f);
        }

        private void UpdateRingParticles(float dt)
        {
            foreach (var rp in RingParticles.Where(x => x.IsActive))
            {
                rp.Velocity += new Vector2(0, 1900f) * dt;
                rp.Position += rp.Velocity * dt;
                rp.Update(dt);

                // столкновения с обычными платформами
                foreach (var pl in Platforms)
                {
                    var hit = Collision.Solve(rp.Bounds, pl.Bounds);
                    if (hit.Side == HitSide.None) continue;

                    rp.Position += hit.Correction;

                    if (hit.Side == HitSide.Top)
                    {
                        // подпрыгивание/затухание
                        rp.Velocity = new Vector2(rp.Velocity.X * 0.72f, -rp.Velocity.Y * 0.45f);
                        if (MathF.Abs(rp.Velocity.Y) < 130f)
                            rp.Velocity = new Vector2(rp.Velocity.X, 0);
                    }
                }
            }

            RingParticles.RemoveAll(x => !x.IsActive);
        }

        private void ApplySpeedPads(float dt)
        {
            foreach (var pad in SpeedPads)
            {
                if (!Player.Bounds.Intersects(pad.Bounds)) continue;

                // Подталкивание: хорошо чувствуется при спиндэше/ролле
                Player.Velocity = new Vector2(
                    MathUtil.Clamp(Player.Velocity.X + pad.PushX * dt, -1400f, 1400f),
                    MathF.Min(Player.Velocity.Y, pad.PushY) // слегка “прижимает” вверх
                );
            }
        }

        private void CollectLevelRings()
        {
            foreach (var ring in Rings.Where(x => x.IsActive))
            {
                if (Player.Bounds.Intersects(ring.Bounds))
                {
                    ring.IsActive = false;
                    Player.RingCount += 1;
                    Player.Score += ring.ScoreValue;
                }
            }
        }

        private void CollectBurstRings()
        {
            foreach (var rp in RingParticles.Where(x => x.IsActive))
            {
                if (Player.Bounds.Intersects(rp.Bounds))
                {
                    rp.IsActive = false;
                    Player.RingCount += 1;
                    // обычно очки за поднятые выпавшие кольца не дают — оставим 0
                }
            }
        }

        private void HandleEnemyContacts()
        {
            foreach (var enemy in Enemies.Where(x => x.IsActive))
            {
                if (!Player.Bounds.Intersects(enemy.Bounds)) continue;

                if (Player.IsInvulnerable) break;

                bool stomp = Player.Velocity.Y > 0 && Player.Bounds.Bottom - enemy.Bounds.Top < 16f;

                if (stomp || Player.IsRolling)
                {
                    enemy.IsActive = false;
                    Player.Score += 100;
                    Player.Velocity.Y = -360;
                }
                else
                {
                    HurtFrom(enemy.Position);
                    break;
                }
            }
        }

        private void HandleBossContacts()
        {
            if (Boss == null || Boss.Defeated) return;
            if (!Player.Bounds.Intersects(Boss.Bounds)) return;
            if (Player.IsInvulnerable) return;

            bool stomp = Player.Velocity.Y > 0 && Player.Bounds.Bottom - Boss.Bounds.Top < 18f;

            if (stomp || Player.IsRolling)
            {
                Boss.TakeHit();
                Player.Score += 500;
                Player.Velocity.Y = -440;
                Player.StartInvulnerability(0.6f);

                if (Boss.Defeated)
                    _finishUnlocked = true;
            }
            else
            {
                HurtFrom(Boss.Position);
            }
        }

        private void HurtFrom(Vector2 hazardPos)
        {
            if (Player.RingCount > 0)
            {
                SpawnRingBurst360(Player.Position + Player.Size / 2f, Player.RingCount);
                Player.RingCount = 0;

                // отбрасывание
                float dir = (Player.Position.X + Player.Size.X / 2f) < hazardPos.X ? -1f : 1f;
                Player.Velocity = new Vector2(360f * dir, -460f);

                Player.StartInvulnerability(1.6f);
                Player.StartHurtLock(0.55f); // пока отлетает — нельзя подбирать
            }
            else
            {
                // death like Sonic: вверх и затем вниз
                Player.DieSonicStyle();
                Camera.Freeze();
                // invuln "навсегда" до респавна
                Player.StartInvulnerability(999f);
            }
        }

        private void SpawnRingBurst360(Vector2 center, int rings)
        {
            int count = Math.Min(rings, 24);

            float speedMin = 320f;
            float speedMax = 760f;

            for (int i = 0; i < count; i++)
            {
                float ang = (2f * MathF.PI) * (i / (float)count);
                ang += (float)(_rng.NextDouble() * 0.22 - 0.11);

                float spd = speedMin + (float)_rng.NextDouble() * (speedMax - speedMin);

                var v = new Vector2(MathF.Cos(ang), MathF.Sin(ang)) * spd;
                v.Y -= 200f; // чуть вверх как в Sonic

                RingParticles.Add(new RingParticle(center, v));
            }
        }

        private void AdvanceLevelOrWin()
        {
            if (CurrentLevel == LevelId.Act1)
            {
                CurrentLevel = LevelId.Act2;
                LoadLevel(CurrentLevel, Player.Lives, Player.Score, ringCount: 0); // как Sonic 1
                return;
            }

            if (CurrentLevel == LevelId.Act2)
            {
                CurrentLevel = LevelId.Act3_Boss;
                LoadLevel(CurrentLevel, Player.Lives, Player.Score, ringCount: 0);
                return;
            }

            State = GameState.LevelCompleted;
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

            // slope/ramp support (простая модель “прилипнуть к склону”)
            ResolveRamps();

            // падение за нижнюю границу "мира"
            if (Player.Position.Y > 1400)
            {
                Player.DieSonicStyle();
                Camera.Freeze();
                Player.StartInvulnerability(999f);
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

        private void ResolveRamps()
        {
            // Простая модель: если игрок "над" рампой по X, ставим его Y на поверхность рампы,
            // если он почти касается и падает вниз.
            foreach (var ramp in Ramps)
            {
                if (!ramp.ContainsX(Player.Position.X + Player.Size.X / 2f))
                    continue;

                float surfaceY = ramp.GetSurfaceY(Player.Position.X + Player.Size.X / 2f);
                float playerBottom = Player.Position.Y + Player.Size.Y;

                // Если игрок падает и почти на поверхности — "ставим" на склон
                if (Player.Velocity.Y >= 0 && playerBottom >= surfaceY - 6f && playerBottom <= surfaceY + 18f)
                {
                    Player.Position = new Vector2(Player.Position.X, surfaceY - Player.Size.Y);
                    Player.Velocity = new Vector2(Player.Velocity.X, 0);
                    Player.IsOnGround = true;

                    // Небольшая "физика момента": вниз по склону ускоряем, вверх чуть тормозим
                    Player.Velocity = new Vector2(Player.Velocity.X + ramp.SlopeAccelX * 0.9f, Player.Velocity.Y);
                }
            }
        }
    }
}