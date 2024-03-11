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
    public partial class FrmSeleccionarMultiplePlan : Form
    {
        public DataTable dtPlanesVigentes = new DataTable();
        public int planSeleccionado = 0;

        public FrmSeleccionarMultiplePlan()
        {
            InitializeComponent();

            if (dtPlanesVigentes.Rows.Count > 0)
            {
                DgvPlanes.AutoGenerateColumns = false;
                DgvPlanes.DataSource = dtPlanesVigentes;
            }
        }

        private void DgvPlanes_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            planSeleccionado = Convert.ToInt32(DgvPlanes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            this.Close();
        }
    }
}
