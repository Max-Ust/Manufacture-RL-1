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
    public partial class RulesForm : Form
    {
        bool[] R;

        public RulesForm()
        {
            InitializeComponent();
        }

        public RulesForm(bool[] r)
        {
            InitializeComponent();

            R = r;

            if (R[0])
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;

            if (R[1])
                checkBox2.Checked = true;
            else
                checkBox2.Checked = false;
        }

        public bool[] GetR
        {
            get
            {
                return R;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                R[0] = true;
            else
                R[0] = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                R[1] = true;
            else
                R[1] = false;
        }
    }
}
