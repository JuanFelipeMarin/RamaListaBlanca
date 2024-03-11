using Microsoft.VisualBasic;
using Sibo.WhiteList.IngresoMonitor.Classes;
using Sibo.WhiteList.IngresoMonitor.wsSiboPawSoap;
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
    public partial class FrmAbriPuertaLoginIngresoAdicional : Form
    {
        public string motivos = string.Empty;
        public string terminal = string.Empty;
        public string identificacion = string.Empty;
        public string nombre = string.Empty;
        public string empresa = string.Empty;
        public string usuario = string.Empty;
        public string contrasena = string.Empty;

        public FrmAbriPuertaLoginIngresoAdicional()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            int strGymId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["gymId"].ToString());
            string strMensajeError = "";

            if (!string.IsNullOrEmpty(cmbTerminalLoginIngresoAdicional.Text))
            {
                terminal = cmbTerminalLoginIngresoAdicional.Text;
                // this.DialogResult = DialogResult.OK;
            }
            else
            {
                strMensajeError = "- Debe seleccionar una terminal." + Constants.vbCrLf;
                cmbTerminalLoginIngresoAdicional.Focus();

            }

            if (!string.IsNullOrEmpty(TxtID.Text))
            {
                identificacion = TxtID.Text;
                //this.DialogResult = DialogResult.OK;
            }
            else
            {
                strMensajeError = strMensajeError + "- Debe ingresar una identificacion." + Constants.vbCrLf;
                TxtID.Focus();
                
            }

            if (!string.IsNullOrEmpty(txtNombre.Text))
            {
                nombre = txtNombre.Text;
               // this.DialogResult = DialogResult.OK;
            }
            else
            {
                strMensajeError = strMensajeError + "- Debe ingresar un nombre." + Constants.vbCrLf;
                txtNombre.Focus();
                
            }

            //campo opcional
            if (!string.IsNullOrEmpty(txtEmpresa.Text))
            {
                empresa = txtEmpresa.Text;
                //this.DialogResult = DialogResult.OK;
            }
            else
            {
                empresa = "";
            }

            if (!string.IsNullOrEmpty(txtMotivoLoginIngresoAdicional.Text))
            {
                motivos = txtMotivoLoginIngresoAdicional.Text;
                //this.DialogResult = DialogResult.OK;
            }
            else
            {
                strMensajeError = strMensajeError + "- Debe ingresar un motivo." + Constants.vbCrLf;
                txtMotivoLoginIngresoAdicional.Focus();
                

            }

            if (!string.IsNullOrEmpty(txtUsr.Text))
            {
                usuario = txtUsr.Text;
                //this.DialogResult = DialogResult.OK;
            }
            else
            {
                strMensajeError = strMensajeError + "- Debe ingresar el usuario." + Constants.vbCrLf;
                txtUsr.Focus();

            }

            if (!string.IsNullOrEmpty(txtPwd.Text))
            {
                contrasena = txtPwd.Text;
               // this.DialogResult = DialogResult.OK;
            }
            else
            {
                strMensajeError = strMensajeError + "- Debe ingresar la contraseña." + Constants.vbCrLf;
                txtPwd.Focus();
                
            }

            //Validacion si el usuario existe en Sibo Paw
            wsSiboPawSoapClient ws = new wsSiboPawSoapClient();
            DataTable strUsuario = new DataTable();
            strUsuario = ws.ValidarLogueoUsuarioPorEmpresa(usuario,contrasena, strGymId);

            if (Convert.ToBoolean(strUsuario.Rows[0]["Resultado"]) == false)
            {
                strMensajeError = strMensajeError + "- " + strUsuario.Rows[0]["MsjError"] + Constants.vbCrLf;
            }

            if (!string.IsNullOrEmpty(strMensajeError))
            {
                clsShowMessage.Show(strMensajeError, clsEnum.MessageType.Informa);
                txtEmpresa.Text = empresa;
                TxtID.Text = identificacion;
                txtMotivoLoginIngresoAdicional.Text = motivos;
                txtNombre.Text = nombre;
                txtUsr.Text = usuario;
                txtMotivoLoginIngresoAdicional.Text = motivos;
                cmbTerminalLoginIngresoAdicional.Text = terminal;
                txtPwd.Text = contrasena;
                this.DialogResult = DialogResult.None;
                //return;
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
            //fecha actual
            lblFechaActual.Text = DateTime.Now.Date.ToString("D");
            cmbTerminalLoginIngresoAdicional.Focus();
            cmbTerminalLoginIngresoAdicional.TabIndex = 0;
            TxtID.TabIndex = 1;
            txtNombre.TabIndex = 2;
            txtEmpresa.TabIndex = 3;
            txtMotivoLoginIngresoAdicional.TabIndex = 4;
            txtUsr.TabIndex = 5;
            txtPwd.TabIndex = 6;
            btnOk.TabIndex = 7;
            btnCancel.TabIndex = 8;

            //lista de terminales en lista blanca
            DataTable dttTerminal = new DataTable();
            clsWhiteList wl = new clsWhiteList();

            dttTerminal = wl.consultarTerminal();

            if (dttTerminal.Rows.Count > 0)
            {
                for (int i = 0; i < dttTerminal.Rows.Count; i++)
                {
                    cmbTerminalLoginIngresoAdicional.Items.Add(dttTerminal.Rows[i]["terminalId"].ToString() + ". " + dttTerminal.Rows[i]["name"].ToString());
                }
            }
            else
            {
                cmbTerminalLoginIngresoAdicional.Items.Add("Seleccione");
            }


        }
    }
}
