namespace TurboHedgehogForms
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Timer gameTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gameTimer = new System.Windows.Forms.Timer(this.components);

            // 
            // gameTimer
            // 
            this.gameTimer.Interval = 16; // ~60 FPS
            this.gameTimer.Tick += new System.EventHandler(this.GameLoop_Tick);

            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.ClientSize = new System.Drawing.Size(960, 540);
            this.Text = "TurboHedgehog Forms";
            this.BackColor = System.Drawing.Color.Black;

            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
        }
    }
}