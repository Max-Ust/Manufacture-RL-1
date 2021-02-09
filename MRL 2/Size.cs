using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MRL_2
{
    public partial class Size : Form
    {
        int M;
        int N;

        public Size()
        {
            M = 3;
            N = 3;

            InitializeComponent();
        }

        public Size(int m, int n)
        {
            InitializeComponent();

            M = m;
            N = n;

            trackBar1.Value = m;
            trackBar2.Value = n;

            textBox1.Text = "Ширина поля равна " + M;
            textBox2.Text = "Высота поля равна " + N;
        }

        public int GetM
        {
            get
            {
                return M;
            }
        }

        public int GetN
        {
            get
            {
                return N;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            M = trackBar1.Value;
            textBox1.Text = "Ширина поля равна " + M;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            N = trackBar2.Value;
            textBox2.Text = "Высота поля равна " + N;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
