using System;
using System.Windows.Forms;

namespace Raft
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Состояние системы берег-плот-берег.
        /// </summary>
        private RaftState state;

        public Form1()
        {
            InitializeComponent();
            state = new RaftState(
                (str) => label1.Text = str,
                (str) => label2.Text = str,
                (str) => label5.Text = str,
                (dbl) => progressBar1.Value = (int)(dbl * progressBar1.Maximum),
                () => trackBarChance.Value / (double)trackBarChance.Maximum,
                () => 8 * trackBarSpeed.Value / (double)trackBarSpeed.Maximum,
                () =>
                {
                    if(int.TryParse(textBox1.Text, out int output) && output > 0)
                        return output;
                    return 20;
                });
        }

        /// <summary>
        /// Обновление логики.
        /// </summary>
        private void Tick_timerLogic(object sender, EventArgs e)
            => state.UpdateLogic();

        /// <summary>
        /// Обновление графики.
        /// </summary>
        private void Tick_timerVisual(object sender, EventArgs e)
            => state.UpdateVisual();

        private void TextChanged_textBoxLogicalInterval(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxLogicalInterval.Text, out int output) && output > 0)
                timerLogic.Interval = output;
            else
                timerLogic.Interval = 400;
        }
    }
}
