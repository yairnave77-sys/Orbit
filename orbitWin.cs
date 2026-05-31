using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Orbit
{
    public partial class orbitWin : Form
    {
        public orbitWin()
        {
            InitializeComponent();
            aiTrainingButton.Click += button1_Click;
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            this.BackColor = Color.FromArgb(13, 13, 28);
            this.Paint += orbitWin_Paint;

            StyleButton(twoPlayersButton, Color.FromArgb(35, 110, 65),  Color.FromArgb(55, 145, 90),  "Two Players");
            StyleButton(onePlayerbutton,  Color.FromArgb(35, 70,  140), Color.FromArgb(55, 100, 180), "One Player");
            StyleButton(aiTrainingButton, Color.FromArgb(90, 40,  90),  Color.FromArgb(120, 60, 120), "AI Training");
        }

        private static void StyleButton(Button btn, Color bg, Color border, string text)
        {
            btn.Text = text;
            btn.BackColor = bg;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = border;
            btn.FlatAppearance.BorderSize = 1;
            btn.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            btn.Cursor = Cursors.Hand;
            btn.Size = new Size(140, 52);
        }

        private void orbitWin_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int W = this.ClientSize.Width, H = this.ClientSize.Height;

            // Background gradient
            using (var bg = new LinearGradientBrush(
                new Point(0, 0), new Point(0, H),
                Color.FromArgb(13, 13, 28), Color.FromArgb(20, 10, 40)))
                g.FillRectangle(bg, 0, 0, W, H);

            // Decorative orbit rings behind the title
            float cx = W / 2f, cy = 88f;
            using (Pen p = new Pen(Color.FromArgb(30, 80, 140, 230), 1.5f))
            {
                g.DrawEllipse(p, cx - 160, cy - 55, 320, 110);
                g.DrawEllipse(p, cx - 90,  cy - 30, 180, 60);
            }
            // Orbiting dot
            g.FillEllipse(new SolidBrush(Color.FromArgb(160, 100, 160, 255)), cx + 152, cy - 8, 10, 10);
            g.FillEllipse(new SolidBrush(Color.FromArgb(120, 80,  200, 120)), cx - 86,  cy + 24, 8,  8);

            // Title
            using (Font titleFont = new Font("Segoe UI", 48, FontStyle.Bold))
            using (var brush = new LinearGradientBrush(
                new PointF(cx - 120, 38), new PointF(cx + 120, 38 + 60),
                Color.FromArgb(110, 170, 255),
                Color.FromArgb(200, 140, 255)))
            {
                string title = "ORBIT";
                SizeF sz = g.MeasureString(title, titleFont);
                g.DrawString(title, titleFont, brush, cx - sz.Width / 2, 38);
            }

            // Subtitle
            using (Font subFont = new Font("Segoe UI", 12, FontStyle.Regular))
            using (SolidBrush subBrush = new SolidBrush(Color.FromArgb(130, 130, 175)))
            {
                string sub = "Strategy Board Game  —  4×4 Rotating Board";
                SizeF sz = g.MeasureString(sub, subFont);
                g.DrawString(sub, subFont, subBrush, cx - sz.Width / 2, 112);
            }

            // Label above buttons
            using (Font lf = new Font("Segoe UI", 10, FontStyle.Regular))
            using (SolidBrush lb = new SolidBrush(Color.FromArgb(100, 100, 145)))
            {
                string l = "Choose game mode";
                SizeF sz = g.MeasureString(l, lf);
                g.DrawString(l, lf, lb, cx - sz.Width / 2, 145);
            }

            // Bottom rule
            using (Pen rule = new Pen(Color.FromArgb(35, 80, 80, 120), 1))
                g.DrawLine(rule, 60, H - 38, W - 60, H - 38);

            // Footer
            using (Font ff = new Font("Segoe UI", 9))
            using (SolidBrush fb = new SolidBrush(Color.FromArgb(70, 70, 105)))
            {
                string footer = "Each turn: place a piece, then the board rotates. First to 4 in a row wins.";
                SizeF sz = g.MeasureString(footer, ff);
                g.DrawString(footer, ff, fb, cx - sz.Width / 2, H - 28);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("AI Training mode coming soon!", "Coming Soon",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("AI opponent coming soon!\nOpening two-player mode.", "One Player",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LaunchGame();
        }

        private void button3_Click(object sender, EventArgs e) => LaunchGame();

        private void LaunchGame()
        {
            var gameForm = new Gui();
            gameForm.FormClosed += (s, a) => this.Close();
            gameForm.Show();
            this.Hide();
        }
    }
}
