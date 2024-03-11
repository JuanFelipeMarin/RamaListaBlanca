using Sibo.WhiteList.IngresoTouch.Classes;
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

namespace Sibo.WhiteList.IngresoMonitor
{
    public partial class FrmAbriPuerta : Form
    {
        public string motivos = string.Empty;
        public string terminal = string.Empty;

        public FrmAbriPuerta()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string strMensajeError = "";
            if (!string.IsNullOrEmpty(txtMotivos.Text))
            {
                motivos = txtMotivos.Text;
            }
            else
            {
                strMensajeError = "- Debe ingresar un motivo." + "\r\n";
                txtMotivos.Focus();
               
            }

            if (!string.IsNullOrEmpty(cmbTerminal.Text))
            {
                terminal = cmbTerminal.Text;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                strMensajeError = strMensajeError + "- Debe seleccionar una terminal." + "\r\n";
                cmbTerminal.Focus();
            }
            if (!string.IsNullOrEmpty(strMensajeError))
            {
                clsShowMessage.Show(strMensajeError, clsEnum.MessageType.Informa);
                txtMotivos.Text = motivos;
                cmbTerminal.Text = terminal;
                //return;
                this.DialogResult = DialogResult.None;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
            }


        }

        private void txtId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                btnOk.PerformClick();
            }
        }

        private void FrmAbriPuerta_Load(object sender, EventArgs e)
        {
            lblFechaActual.Text = DateTime.Now.Date.ToString("D");

           DataTable dttTerminal = new DataTable();
            clsWhiteList wl = new clsWhiteList();

            dttTerminal = wl.consultarTerminal();

            if (dttTerminal.Rows.Count > 0)
            {
                for (int i = 0; i < dttTerminal.Rows.Count; i++)
                {
                    cmbTerminal.Items.Add(dttTerminal.Rows[i]["terminalId"].ToString() + ". " + dttTerminal.Rows[i]["name"].ToString());
                }
            }
            else
            {
                cmbTerminal.Items.Add("Seleccione");
            }


        }
    }
}
