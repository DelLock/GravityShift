using System.Drawing;
using TurboHedgehogForms.Game;

namespace TurboHedgehogForms.Rendering
{
    public sealed class Renderer2D
    {
        private readonly Font _hudFont = new Font("Consolas", 12, FontStyle.Bold);
        private readonly Font _titleFont = new Font("Consolas", 36, FontStyle.Bold);
        private readonly Font _bigFont = new Font("Consolas", 24, FontStyle.Bold);

        public void Draw(Graphics g, Size clientSize, GameWorld world)
        {
            world.Camera.ViewSize = clientSize;
            g.Clear(Color.FromArgb(18, 18, 24));

            if (world.State == GameState.Title)
            {
                DrawCenteredText(g, clientSize, "TURBO HEDGEHOG", _titleFont, Brushes.DeepSkyBlue, -50);
                DrawCenteredText(g, clientSize, "Enter — Start", _bigFont, Brushes.WhiteSmoke, 20);
                DrawCenteredText(g, clientSize,
                    "A/D (←/→) — движение\nSpace — прыжок\nS/↓ + Space — заряд спиндэша, отпусти S/↓ чтобы выстрелить\nR — в меню",
                    _hudFont, Brushes.Gainsboro, 95);
                return;
            }

            float camX = world.Camera.Position.X;
            float camY = world.Camera.Position.Y;

            // платформы
            using (var brush = new SolidBrush(Color.FromArgb(50, 200, 120)))
            {
                foreach (var p in world.Platforms)
                    g.FillRectangle(brush, p.Position.X - camX, p.Position.Y - camY, p.Size.X, p.Size.Y);
            }

            // финиш/капсула
            using (var pole = new SolidBrush(Color.Silver))
            using (var flag = new SolidBrush(Color.Gold))
            {
                var f = world.Finish;
                g.FillRectangle(pole, f.Position.X - camX + 10, f.Position.Y - camY, 4, f.Size.Y);
                g.FillRectangle(flag, f.Position.X - camX + 14, f.Position.Y - camY + 6, 18, 12);
            }

            // кольца на уровне
            using (var pen = new Pen(Color.Gold, 3))
            {
                foreach (var r in world.Rings)
                {
                    if (!r.IsActive) continue;
                    g.DrawEllipse(pen, r.Position.X - camX, r.Position.Y - camY, r.Size.X, r.Size.Y);
                }

                foreach (var rp in world.RingParticles)
                {
                    if (!rp.IsActive) continue;
                    g.DrawEllipse(pen, rp.Position.X - camX, rp.Position.Y - camY, rp.Size.X, rp.Size.Y);
                }
            }

            // враги
            using (var brush = new SolidBrush(Color.FromArgb(220, 70, 70)))
            {
                foreach (var e in world.Enemies)
                {
                    if (!e.IsActive) continue;
                    g.FillRectangle(brush, e.Position.X - camX, e.Position.Y - camY, e.Size.X, e.Size.Y);
                }
            }

            // босс
            if (world.Boss != null && !world.Boss.Defeated)
            {
                using var bossBrush = new SolidBrush(Color.FromArgb(200, 120, 255));
                var b = world.Boss;
                g.FillRectangle(bossBrush, b.Position.X - camX, b.Position.Y - camY, b.Size.X, b.Size.Y);

                // HP полоска над боссом
                float hpW = 70;
                float hpH = 8;
                float x = b.Position.X - camX + (b.Size.X - hpW) / 2f;
                float y = b.Position.Y - camY - 14;
                g.FillRectangle(Brushes.DimGray, x, y, hpW, hpH);
                float fill = hpW * (b.Hp / 8f);
                g.FillRectangle(Brushes.LimeGreen, x, y, fill, hpH);
            }

            // игрок (мигает при инвизе)
            var pl = world.Player;

            bool drawPlayer = true;
            if (pl.IsInvulnerable)
            {
                // мигание ~ 10 раз/сек
                // без доступа к TotalTime: сделаем от скорости X, но лучше привязать к Time.
                // Здесь простая имитация: если invuln активен — мигаем по кадрам.
                drawPlayer = (System.Environment.TickCount / 80) % 2 == 0;
            }

            if (drawPlayer)
            {
                Color c = pl.IsRolling ? Color.FromArgb(80, 255, 180) : Color.FromArgb(80, 160, 255);
                using var brush = new SolidBrush(c);
                g.FillRectangle(brush, pl.Position.X - camX, pl.Position.Y - camY, pl.Size.X, pl.Size.Y);
            }

            // HUD
            string act = world.CurrentLevel switch
            {
                LevelId.Act1 => "ACT 1",
                LevelId.Act2 => "ACT 2",
                LevelId.Act3_Boss => "ACT 3 (BOSS)",
                _ => "ACT ?"
            };

            g.DrawString($"{act}   Score: {pl.Score}   Rings: {pl.RingCount}   Lives: {pl.Lives}",
                _hudFont, Brushes.White, 10, 10);

            if (pl.IsChargingSpinDash)
                g.DrawString($"SpinDash: {(int)(pl.SpinDashCharge * 100)}%", _hudFont, Brushes.Gold, 10, 30);

            // Оверлеи
            if (world.State == GameState.LevelCompleted)
                DrawOverlay(g, clientSize, "YOU WIN!", "Enter — Back to Title");
            else if (world.State == GameState.GameOver)
                DrawOverlay(g, clientSize, "GAME OVER", "Enter — Back to Title");
        }

        private static void DrawOverlay(Graphics g, Size size, string title, string subtitle)
        {
            using var overlay = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
            g.FillRectangle(overlay, 0, 0, size.Width, size.Height);

            using var titleFont = new Font("Consolas", 32, FontStyle.Bold);
            using var subFont = new Font("Consolas", 18, FontStyle.Bold);

            DrawCenteredText(g, size, title, titleFont, Brushes.White, -30);
            DrawCenteredText(g, size, subtitle, subFont, Brushes.Gainsboro, 35);
        }

        private static void DrawCenteredText(Graphics g, Size size, string text, Font font, Brush brush, int yOffset)
        {
            var rect = new RectangleF(0, size.Height / 2f + yOffset, size.Width, 250);
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
            g.DrawString(text, font, brush, rect, format);
        }
    }
}