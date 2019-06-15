using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raft
{
    public partial class Form1 : Form
    {
        private RaftState state;

        public Form1()
        {
            InitializeComponent();
            state = new RaftState(
                (str) => label1.Text = str,
                (str) => label2.Text = str,
                (str) => label5.Text = str,
                (dbl) => progressBar1.Value = (int)(dbl * progressBar1.Maximum),
                () => trackBar2.Value / (double)trackBar2.Maximum,
                () => trackBar1.Value / (double)trackBar1.Maximum,
                () =>
                {
                    if(int.TryParse(textBox1.Text, out int output) && output > 0)
                        return output;
                    return 20;
                });
        }

        private void Tick_timerLogic(object sender, EventArgs e)
            => state.UpdateLogic();

        private void Tick_timerVisual(object sender, EventArgs e)
            => state.UpdateVisual();

    }
}
