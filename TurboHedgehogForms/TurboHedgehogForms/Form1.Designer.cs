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
            components = new System.ComponentModel.Container();
            gameTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // gameTimer
            // 
            gameTimer.Interval = 16;
            gameTimer.Tick += GameLoop_Tick;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Black;
            ClientSize = new Size(960, 540);
            DoubleBuffered = true;
            KeyPreview = true;
            Name = "Form1";
            Text = "TurboHedgehog Forms";
            Load += Form1_Load;
            Paint += Form1_Paint;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            ResumeLayout(false);
        }
    }
}