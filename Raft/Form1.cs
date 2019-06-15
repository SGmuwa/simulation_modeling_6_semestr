using System;
using System.Drawing;
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
                (dbl) => panelPlot.Location = new Point((int)((panelWater.Width - panelPlot.Width) * dbl), panelPlot.Location.Y),
                () => trackBarChance.Value / (double)trackBarChance.Maximum,
                () => 8 * trackBarSpeed.Value / (double)trackBarSpeed.Maximum,
                () =>
                {
                    if (int.TryParse(textBox1.Text, out int output) && output > 0)
                    {
                        textBox1.BackColor = SystemColors.Window;
                        return output;
                    }
                    textBox1.BackColor = Color.Red;
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

        /// <summary>
        /// Вызывается, когда пользователь меняет интервал логического таймера.
        /// </summary>
        private void TextChanged_textBoxLogicalInterval(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxLogicalInterval.Text, out int output) && output > 24)
            {
                textBoxLogicalInterval.BackColor = SystemColors.Window;
                timerLogic.Interval = output;
            }
            else
            {
                textBoxLogicalInterval.BackColor = Color.Red;
                timerLogic.Interval = 400;
            }
        }
    }
}
