using System;
using System.Drawing;
using System.Windows.Forms;
using TurboHedgehogForms.Game;
using TurboHedgehogForms.Input;
using TurboHedgehogForms.Rendering;

namespace TurboHedgehogForms
{
    public partial class Form1 : Form
    {
        private readonly InputState _input = new();
        private readonly GameWorld _world = new();
        private readonly Renderer2D _renderer = new();
        private readonly GameTime _time = new();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _world.BuildDemoLevel();
            _time.Reset();
            gameTimer.Start();
        }

        private void GameLoop_Tick(object sender, EventArgs e)
        {
            _time.Step();
            _world.Update(_time, _input);

            Invalidate(); // גûחמגוע Paint
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            _renderer.Draw(e.Graphics, ClientSize, _world);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            _input.SetKey(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            _input.SetKey(e.KeyCode, false);
        }
    }
}