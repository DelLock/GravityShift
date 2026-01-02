using System.Drawing;
using TurboHedgehogForms.Game;

namespace TurboHedgehogForms.Rendering
{
    /// <summary>Простой отрисовщик примитивами GDI+.</summary>
    public sealed class Renderer2D
    {
        private readonly Font _hudFont = new Font("Consolas", 12, FontStyle.Bold);

        public void Draw(Graphics g, Size clientSize, GameWorld world)
        {
            world.Camera.ViewSize = clientSize;

            // фон
            g.Clear(Color.FromArgb(18, 18, 24));

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

            g.DrawString("A/D или ←/→ движение, Space прыжок, R рестарт",
                _hudFont, Brushes.WhiteSmoke, 10, 30);
        }
    }
}