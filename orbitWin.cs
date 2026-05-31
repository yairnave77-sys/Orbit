namespace Orbit
{
    public partial class orbitWin : Form
    {
        public orbitWin()
        {
            InitializeComponent();
            aiTrainingButton.Click += button1_Click;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("AI Training mode is coming soon!", "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("AI opponent coming soon!\nStarting two-player mode.", "One Player", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Gui gameForm = new Gui();
            gameForm.FormClosed += (s, args) => this.Close();
            gameForm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Gui gameForm = new Gui();
            gameForm.FormClosed += (s, args) => this.Close();
            gameForm.Show();
            this.Hide();
        }
    }
}
