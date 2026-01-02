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

            // фон
            g.Clear(Color.FromArgb(18, 18, 24));

            // ====== ЭКРАН ЗАСТАВКИ ======
            if (world.State == GameState.Title)
            {
                DrawCenteredText(g, clientSize,
                    "TURBO HEDGEHOG",
                    _titleFont,
                    Brushes.DeepSkyBlue,
                    yOffset: -50);

                DrawCenteredText(g, clientSize,
                    "Enter — Start    R — Back to Title",
                    _bigFont,
                    Brushes.WhiteSmoke,
                    yOffset: 20);

                DrawCenteredText(g, clientSize,
                    "A/D (или ←/→) — движение, Space — прыжок\nСобери кольца и добеги до флага",
                    _hudFont,
                    Brushes.Gainsboro,
                    yOffset: 90);

                return;
            }

            float camX = world.Camera.Position.X;
            float camY = world.Camera.Position.Y;

            // платформы
            using (var brush = new SolidBrush(Color.FromArgb(50, 200, 120)))
            {
                foreach (var p in world.Platforms)
                {
                    g.FillRectangle(brush,
                        p.Position.X - camX,
                        p.Position.Y - camY,
                        p.Size.X,
                        p.Size.Y);
                }
            }

            // финиш
            using (var pole = new SolidBrush(Color.Silver))
            using (var flag = new SolidBrush(Color.Gold))
            {
                var f = world.Finish;
                // древко
                g.FillRectangle(pole, f.Position.X - camX + 10, f.Position.Y - camY, 4, f.Size.Y);
                // флаг
                g.FillRectangle(flag, f.Position.X - camX + 14, f.Position.Y - camY + 6, 18, 12);
            }

            // кольца
            using (var pen = new Pen(Color.Gold, 3))
            {
                foreach (var r in world.Rings)
                {
                    if (!r.IsActive) continue;
                    g.DrawEllipse(pen, r.Position.X - camX, r.Position.Y - camY, r.Size.X, r.Size.Y);
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

            // игрок
            var pl = world.Player;
            using (var brush = new SolidBrush(Color.FromArgb(80, 160, 255)))
            {
                g.FillRectangle(brush, pl.Position.X - camX, pl.Position.Y - camY, pl.Size.X, pl.Size.Y);
            }

            // HUD
            string hud = $"Score: {pl.Score}    Lives: {pl.Lives}    Vx: {pl.Velocity.X:0}";
            g.DrawString(hud, _hudFont, Brushes.White, 10, 10);
            g.DrawString("R: Title", _hudFont, Brushes.WhiteSmoke, 10, 30);

            // ====== ОВЕРЛЕИ (конец/гейм овер) ======
            if (world.State == GameState.LevelCompleted)
            {
                DrawOverlay(g, clientSize, "LEVEL COMPLETED!", $"Score: {pl.Score}\nEnter — Back to Title");
            }
            else if (world.State == GameState.GameOver)
            {
                DrawOverlay(g, clientSize, "GAME OVER", $"Score: {pl.Score}\nEnter — Back to Title");
            }
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
            var rect = new RectangleF(0, size.Height / 2f + yOffset, size.Width, 200);
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
            g.DrawString(text, font, brush, rect, format);
        }
    }
}