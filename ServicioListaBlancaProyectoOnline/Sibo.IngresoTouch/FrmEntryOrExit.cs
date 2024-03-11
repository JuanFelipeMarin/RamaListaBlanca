using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sibo.WhiteList.IngresoTouch
{
    public partial class FrmEntryOrExit : Form
    {
        public string entry = null;
        public FrmEntryOrExit()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            entry = "Entry";
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            entry = "Exit";
            this.Close();
        }
    }
}
