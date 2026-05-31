using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Orbit
{
    public partial class Gui : Form
    {
        private ManageGame game;
        private Label turnLabel;
        private Button newGameBtn;
        private Location? hoveredCell = null;

        private static readonly Color C_BG       = Color.FromArgb(13, 13, 28);
        private static readonly Color C_PANEL     = Color.FromArgb(20, 20, 42);
        private static readonly Color C_BOARD     = Color.FromArgb(18, 18, 38);
        private static readonly Color C_RING      = Color.FromArgb(55, 95, 200);
        private static readonly Color C_DOT       = Color.FromArgb(65, 65, 105);
        private static readonly Color C_TEXT      = Color.FromArgb(195, 195, 235);

        public Gui()
        {
            InitializeComponent();
            game = new ManageGame();
            ApplyTheme();

            pix_board.Image = null;
            pix_board.BackColor = C_BOARD;
            pix_board.Paint       += pix_board_Paint;
            pix_board.MouseClick  += pix_board_MouseClick;
            pix_board.MouseMove   += pix_board_MouseMove;
            pix_board.MouseLeave  += pix_board_MouseLeave;
            pix_board.Cursor = Cursors.Hand;

            UpdateStatus();
            deleteFromStack();
        }

        // ─── Theme ────────────────────────────────────────────────────────────

        private void ApplyTheme()
        {
            this.BackColor = C_BG;
            mainPanel.BackColor = C_BOARD;
            tableLayoutPanel1.BackColor = C_BG;
            pictureBox1.Visible = false;

            blackStack.BackColor = C_PANEL;
            blackStack.Paint += (s, e) => DrawStackPanel(e.Graphics, blackStack, "BLACK", game.BlackPiecesInStack, false);

            whiteStack.BackColor = C_PANEL;
            whiteStack.Paint += (s, e) => DrawStackPanel(e.Graphics, whiteStack, "WHITE", game.WhitePiecesInStack, true);

            label1.Visible = false;
            label2.Visible = false;

            // Turn label pinned above the table
            turnLabel = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(255, 215, 90),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BackColor = Color.Transparent,
                Bounds = new Rectangle(0, 0, this.ClientSize.Width, 38)
            };
            this.Controls.Add(turnLabel);
            turnLabel.BringToFront();

            // Home button
            HomeBtn.BackColor = Color.FromArgb(45, 45, 80);
            HomeBtn.ForeColor = Color.White;
            HomeBtn.FlatStyle = FlatStyle.Flat;
            HomeBtn.FlatAppearance.BorderColor = Color.FromArgb(75, 75, 130);
            HomeBtn.Font = new Font("Segoe UI", 10);
            HomeBtn.Cursor = Cursors.Hand;

            // New Game button (same anchor region as HomeBtn, offset left)
            newGameBtn = new Button
            {
                Text = "New Game",
                BackColor = Color.FromArgb(35, 115, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand,
                Size = HomeBtn.Size,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            newGameBtn.FlatAppearance.BorderColor = Color.FromArgb(55, 150, 95);
            newGameBtn.Click += (s, e) => RestartGame();
            this.Controls.Add(newGameBtn);
        }

        // ─── Stack panel ──────────────────────────────────────────────────────

        private void DrawStackPanel(Graphics g, Panel panel, string name, int count, bool isWhite)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int pw = panel.Width, ph = panel.Height;

            using (Pen accent = new Pen(C_RING, 2))
                g.DrawLine(accent, isWhite ? pw - 1 : 0, 0, isWhite ? pw - 1 : 0, ph);

            using (Font f = new Font("Segoe UI", 12, FontStyle.Bold))
            using (SolidBrush b = new SolidBrush(C_TEXT))
            {
                SizeF sz = g.MeasureString(name, f);
                g.DrawString(name, f, b, (pw - sz.Width) / 2f, 18);
            }

            using (Font f = new Font("Segoe UI", 9))
            using (SolidBrush b = new SolidBrush(Color.FromArgb(130, 130, 170)))
            {
                string s = $"{count} / 8";
                SizeF sz = g.MeasureString(s, f);
                g.DrawString(s, f, b, (pw - sz.Width) / 2f, 44);
            }

            float r = Math.Min(pw * 0.11f, 15f);
            float gap = r * 2.6f;
            int cols = Math.Max(1, (int)((pw - 14) / gap));
            float startX = (pw - cols * gap + (gap - r * 2)) / 2f + r;

            using (SolidBrush shadow = new SolidBrush(Color.FromArgb(35, 0, 0, 0)))
            using (SolidBrush shine = new SolidBrush(Color.FromArgb(isWhite ? 150 : 50, 255, 255, 255)))
            using (Pen borderPen = new Pen(isWhite ? Color.FromArgb(165, 165, 185) : Color.FromArgb(140, 140, 140), 1f))
            {
                for (int i = 0; i < count; i++)
                {
                    int row = i / cols, col = i % cols;
                    float px = startX + col * gap;
                    float py = 78 + r + row * gap;

                    g.FillEllipse(shadow, px - r + 2, py - r + 2, r * 2, r * 2);

                    if (!isWhite)
                    {
                        using (var br = new LinearGradientBrush(
                            new PointF(px - r, py - r), new PointF(px + r, py + r),
                            Color.FromArgb(75, 75, 75), Color.FromArgb(12, 12, 12)))
                            g.FillEllipse(br, px - r, py - r, r * 2, r * 2);
                    }
                    else
                    {
                        using (var br = new LinearGradientBrush(
                            new PointF(px - r, py - r), new PointF(px + r, py + r),
                            Color.White, Color.FromArgb(195, 195, 215)))
                            g.FillEllipse(br, px - r, py - r, r * 2, r * 2);
                    }

                    g.DrawEllipse(borderPen, px - r, py - r, r * 2, r * 2);
                    g.FillEllipse(shine, px - r * 0.45f, py - r * 0.65f, r * 0.5f, r * 0.35f);
                }
            }

            bool myTurn = isWhite ? !game.IsBlackTurn : game.IsBlackTurn;
            if (myTurn && !game.GameOver)
            {
                using (SolidBrush b = new SolidBrush(Color.FromArgb(255, 215, 90)))
                {
                    g.FillRectangle(b, pw / 2f - 18, 8, 36, 4);
                    Point[] tri = {
                        new Point(pw / 2, 2),
                        new Point(pw / 2 - 6, 9),
                        new Point(pw / 2 + 6, 9)
                    };
                    g.FillPolygon(b, tri);
                }
            }
        }

        // ─── Board paint ─────────────────────────────────────────────────────

        private void pix_board_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int W = pix_board.Width, H = pix_board.Height;
            int SIZE = GameConsts.BOARD_SIZE;
            float cw = (float)W / SIZE, ch = (float)H / SIZE;
            float bcx = W / 2f, bcy = H / 2f;

            using (SolidBrush bgBrush = new SolidBrush(C_BOARD))
                g.FillRectangle(bgBrush, 0, 0, W, H);

            using (Pen gridPen = new Pen(Color.FromArgb(35, 255, 255, 255), 1))
                for (int i = 1; i < SIZE; i++)
                {
                    g.DrawLine(gridPen, i * cw, 0, i * cw, H);
                    g.DrawLine(gridPen, 0, i * ch, W, i * ch);
                }

            float outerR = cw * 1.54f;
            using (Pen p = new Pen(Color.FromArgb(140, C_RING), 2.5f))
                g.DrawEllipse(p, bcx - outerR, bcy - outerR, outerR * 2, outerR * 2);

            float innerR = cw * 0.70f;
            using (Pen p = new Pen(Color.FromArgb(100, C_RING), 2f))
                g.DrawEllipse(p, bcx - innerR, bcy - innerR, innerR * 2, innerR * 2);

            using (SolidBrush dotBrush = new SolidBrush(C_DOT))
                for (int r = 0; r < SIZE; r++)
                    for (int c = 0; c < SIZE; c++)
                    {
                        float px = (c + 0.5f) * cw, py = (r + 0.5f) * ch;
                        g.FillEllipse(dotBrush, px - 4, py - 4, 8, 8);
                    }

            float pieceR = Math.Min(cw, ch) * 0.36f;

            if (hoveredCell.HasValue && !game.GameOver)
            {
                int hr = hoveredCell.Value.Row, hc = hoveredCell.Value.Col;
                if (game.GetTroopAt(hoveredCell.Value) == GameConsts.NO_TROOP_INT)
                {
                    float px = (hc + 0.5f) * cw, py = (hr + 0.5f) * ch;
                    Color hCol = game.IsBlackTurn
                        ? Color.FromArgb(45, 200, 200, 220)
                        : Color.FromArgb(45, 255, 255, 240);
                    using (SolidBrush hb = new SolidBrush(hCol))
                        g.FillEllipse(hb, px - pieceR, py - pieceR, pieceR * 2, pieceR * 2);
                    using (Pen hp = new Pen(Color.FromArgb(80, 180, 180, 255), 1.5f))
                        g.DrawEllipse(hp, px - pieceR, py - pieceR, pieceR * 2, pieceR * 2);
                }
            }

            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
            using (Pen blackBorder = new Pen(Color.FromArgb(170, 170, 170), 1.5f))
            using (Pen whiteBorder = new Pen(Color.FromArgb(165, 165, 185), 1.5f))
            {
                for (int r = 0; r < SIZE; r++)
                    for (int c = 0; c < SIZE; c++)
                    {
                        int troop = game.GetTroopAt(new Location(r, c));
                        if (troop == GameConsts.NO_TROOP_INT) continue;

                        float px = (c + 0.5f) * cw, py = (r + 0.5f) * ch;

                        g.FillEllipse(shadowBrush, px - pieceR + 3, py - pieceR + 4, pieceR * 2, pieceR * 2);

                        if (troop == GameConsts.BLACK_TROOP_INT)
                        {
                            using (var br = new LinearGradientBrush(
                                new PointF(px - pieceR, py - pieceR), new PointF(px + pieceR, py + pieceR),
                                Color.FromArgb(85, 85, 85), Color.FromArgb(10, 10, 10)))
                                g.FillEllipse(br, px - pieceR, py - pieceR, pieceR * 2, pieceR * 2);
                            g.DrawEllipse(blackBorder, px - pieceR, py - pieceR, pieceR * 2, pieceR * 2);
                        }
                        else
                        {
                            using (var br = new LinearGradientBrush(
                                new PointF(px - pieceR, py - pieceR), new PointF(px + pieceR, py + pieceR),
                                Color.White, Color.FromArgb(195, 195, 215)))
                                g.FillEllipse(br, px - pieceR, py - pieceR, pieceR * 2, pieceR * 2);
                            g.DrawEllipse(whiteBorder, px - pieceR, py - pieceR, pieceR * 2, pieceR * 2);
                        }

                        int alpha = troop == GameConsts.BLACK_TROOP_INT ? 40 : 130;
                        using (SolidBrush shine = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255)))
                            g.FillEllipse(shine, px - pieceR * 0.48f, py - pieceR * 0.65f, pieceR * 0.52f, pieceR * 0.36f);
                    }
            }
        }

        // ─── Board interaction ────────────────────────────────────────────────

        private void pix_board_MouseClick(object sender, MouseEventArgs e)
        {
            if (game.GameOver) return;

            var loc = ScreenToBoard(e.X, e.Y);
            if (loc == null) return;

            if (game.PlacePiece(loc.Value))
            {
                pix_board.Invalidate();
                blackStack.Invalidate();
                whiteStack.Invalidate();
                UpdateStatus();

                if (game.GameOver)
                {
                    string winner = game.Winner == Troop.BLACK_TROOP ? "Black" : "White";
                    MessageBox.Show($"{winner} wins!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void pix_board_MouseMove(object sender, MouseEventArgs e)
        {
            var loc = ScreenToBoard(e.X, e.Y);
            bool changed = loc?.Row != hoveredCell?.Row || loc?.Col != hoveredCell?.Col
                           || loc.HasValue != hoveredCell.HasValue;
            if (changed) { hoveredCell = loc; pix_board.Invalidate(); }
        }

        private void pix_board_MouseLeave(object sender, EventArgs e)
        {
            if (hoveredCell.HasValue) { hoveredCell = null; pix_board.Invalidate(); }
        }

        private Location? ScreenToBoard(int x, int y)
        {
            int col = (int)(x / ((float)pix_board.Width  / GameConsts.BOARD_SIZE));
            int row = (int)(y / ((float)pix_board.Height / GameConsts.BOARD_SIZE));
            if (col < 0 || col >= GameConsts.BOARD_SIZE || row < 0 || row >= GameConsts.BOARD_SIZE)
                return null;
            return new Location(row, col);
        }

        // ─── Game control ─────────────────────────────────────────────────────

        private void RestartGame()
        {
            game = new ManageGame();
            hoveredCell = null;
            pix_board.Invalidate();
            blackStack.Invalidate();
            whiteStack.Invalidate();
            UpdateStatus();
        }

        private void HomeBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            var home = new orbitWin();
            home.FormClosed += (s, a) => this.Close();
            home.Show();
        }

        private void mainPanel_Resize(object sender, EventArgs e)
        {
            int sz = Math.Min(mainPanel.Width, mainPanel.Height) - 20;
            pix_board.Size = new Size(sz, sz);
            pix_board.Location = new Point((mainPanel.Width - sz) / 2, (mainPanel.Height - sz) / 2);
        }

        private void Game_Load(object sender, EventArgs e)
        {
            mainPanel_Resize(null, null);
            // Position New Game button to the left of Home button
            if (newGameBtn != null)
                newGameBtn.Location = new Point(HomeBtn.Left - newGameBtn.Width - 10, HomeBtn.Top);
            // Stretch turn label across top
            if (turnLabel != null)
                turnLabel.Bounds = new Rectangle(0, 0, this.ClientSize.Width, 38);
        }

        // ─── Status ───────────────────────────────────────────────────────────

        public void deleteFromStack()
        {
            blackStack.Invalidate();
            whiteStack.Invalidate();
        }

        private void UpdateStatus()
        {
            if (turnLabel == null) return;
            if (game.GameOver)
            {
                string w = game.Winner == Troop.BLACK_TROOP ? "Black" : "White";
                turnLabel.Text = $"★  {w} Wins!  ★";
                turnLabel.ForeColor = Color.FromArgb(255, 200, 60);
            }
            else
            {
                string cur = game.IsBlackTurn ? "Black" : "White";
                turnLabel.Text = $"●  {cur}'s Turn";
                turnLabel.ForeColor = game.IsBlackTurn
                    ? Color.FromArgb(190, 190, 230)
                    : Color.FromArgb(255, 255, 230);
            }
        }
    }
}
