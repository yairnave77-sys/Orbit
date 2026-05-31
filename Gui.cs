using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Orbit
{
    public partial class Gui : Form
    {
        private ManageGame game;

        public Gui()
        {
            InitializeComponent();
            game = new ManageGame();
            pix_board.Paint += pix_board_Paint;
            pix_board.MouseClick += pix_board_MouseClick;
            pix_board.Cursor = Cursors.Hand;
            UpdateStatus();
            deleteFromStack();
        }

        private void pix_board_Paint(object sender, PaintEventArgs e)
        {
            int size = GameConsts.BOARD_SIZE;
            float cellW = (float)pix_board.Width / size;
            float cellH = (float)pix_board.Height / size;
            float radius = Math.Min(cellW, cellH) * 0.32f;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    int troop = game.GetTroopAt(new Location(r, c));
                    if (troop == GameConsts.NO_TROOP_INT) continue;

                    float cx = (c + 0.5f) * cellW;
                    float cy = (r + 0.5f) * cellH;

                    if (troop == GameConsts.BLACK_TROOP_INT)
                    {
                        using (SolidBrush fill = new SolidBrush(Color.FromArgb(30, 30, 30)))
                            e.Graphics.FillEllipse(fill, cx - radius, cy - radius, radius * 2, radius * 2);
                        using (Pen border = new Pen(Color.DimGray, 2))
                            e.Graphics.DrawEllipse(border, cx - radius, cy - radius, radius * 2, radius * 2);
                    }
                    else
                    {
                        using (SolidBrush fill = new SolidBrush(Color.WhiteSmoke))
                            e.Graphics.FillEllipse(fill, cx - radius, cy - radius, radius * 2, radius * 2);
                        using (Pen border = new Pen(Color.DarkGray, 2))
                            e.Graphics.DrawEllipse(border, cx - radius, cy - radius, radius * 2, radius * 2);
                    }
                }
            }
        }

        private void pix_board_MouseClick(object sender, MouseEventArgs e)
        {
            if (game.GameOver) return;

            float cellW = (float)pix_board.Width / GameConsts.BOARD_SIZE;
            float cellH = (float)pix_board.Height / GameConsts.BOARD_SIZE;

            int col = (int)(e.X / cellW);
            int row = (int)(e.Y / cellH);

            if (col < 0 || col >= GameConsts.BOARD_SIZE || row < 0 || row >= GameConsts.BOARD_SIZE) return;

            Location clicked = new Location(row, col);

            if (game.PlacePiece(clicked))
            {
                deleteFromStack();
                pix_board.Invalidate();
                UpdateStatus();

                if (game.GameOver)
                {
                    string winnerName = game.Winner == Troop.BLACK_TROOP ? "Black" : "White";
                    MessageBox.Show($"{winnerName} wins!", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void HomeBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            orbitWin homeForm = new orbitWin();
            homeForm.FormClosed += (s, args) => this.Close();
            homeForm.Show();
        }

        private void mainPanel_Resize(object sender, EventArgs e)
        {
            int minSize = Math.Min(mainPanel.Width, mainPanel.Height);
            int w, h;
            w = h = minSize - 20;
            pix_board.Size = new Size(w, h);
            int left = (mainPanel.Width - w) / 2;
            int top = (mainPanel.Height - h) / 2;
            pix_board.Location = new Point(left, top);
        }

        private void Game_Load(object sender, EventArgs e)
        {
            mainPanel_Resize(null, null);
        }

        public void deleteFromStack()
        {
            label1.Text = $"Black\n{game.BlackPiecesInStack} left";
            label2.Text = $"White\n{game.WhitePiecesInStack} left";
        }

        private void UpdateStatus()
        {
            if (game.GameOver)
            {
                string winnerName = game.Winner == Troop.BLACK_TROOP ? "Black" : "White";
                this.Text = $"Orbit - {winnerName} Wins!";
            }
            else
            {
                string current = game.IsBlackTurn ? "Black" : "White";
                this.Text = $"Orbit - {current}'s Turn";
            }
        }
    }
}
