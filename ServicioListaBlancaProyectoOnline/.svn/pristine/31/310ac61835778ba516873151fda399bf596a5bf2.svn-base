using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sibo.WhiteList.IngresoMonitor
{
    public partial class FrmSeleccionarMultiplePlan : Form
    {
        public int planSeleccionado = 0;

        public FrmSeleccionarMultiplePlan()
        {
            InitializeComponent();
        }

        private void DgvPlanes_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            planSeleccionado = Convert.ToInt32(DgvPlanes.Rows[e.RowIndex].Cells[0].Value);
            this.Close();
        }
    }
}
