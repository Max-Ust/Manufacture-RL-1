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
    public partial class Iterations : Form
    {
        int ITER;

        public Iterations()
        {
            InitializeComponent();
        }

        public Iterations(int iter)
        {
            InitializeComponent();

            ITER = iter;

            trackBar1.Value = ITER;
            textBox1.Text = ITER.ToString();
        }

        public int GetITER
        {
            get
            {
                return ITER;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            ITER = trackBar1.Value;

            textBox1.Text = ITER.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int i;

            if (int.TryParse(textBox1.Text, out i) == false || i > 10000000)
                MessageBox.Show("Введенное значение некорректно.");
            else
            {
                ITER = i;

                trackBar1.Value = ITER;
            }
        }
    }
}
