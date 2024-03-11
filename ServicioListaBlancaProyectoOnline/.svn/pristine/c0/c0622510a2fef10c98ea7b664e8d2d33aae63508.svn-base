using Sibo.WhiteList.Service.BLL.Helpers;
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
    public partial class FrmEntryOrExitById : Form
    {
        public string id = string.Empty;

        public FrmEntryOrExitById()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtId.Text))
            {
                id = txtId.Text;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                clsShowMessage.Show("Debe ingresar la identificación del usuario.", clsEnum.MessageType.Informa);
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                btnOk.PerformClick();
            }
        }
    }
}
