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
        }

        public bool[] GetR
        {
            get
            {
                return R;
            }
        }
    }
}
