using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.BLL.Helpers;
using Sibo.WhiteList.Service.Entities.Classes;
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
    public partial class FrmEmployeeLogin : Form
    {
        public string employeeId = string.Empty;

        public FrmEmployeeLogin()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                EmployeeBLL employeeBll = new EmployeeBLL();
                eResponse resp = new eResponse();
                int gymId = 0;
                string strGymId = string.Empty;
                strGymId = System.Configuration.ConfigurationManager.AppSettings["gymId"].ToString();

                if (!string.IsNullOrEmpty(strGymId)) { gymId = Convert.ToInt32(strGymId); }

                resp = employeeBll.ValidateUserEmployee(gymId, txtUserName.Text, txtPwd.Text);

                if (resp != null)
                {
                    if (resp.state)
                    {
                        employeeId = resp.messageOne;
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        switch (resp.message)
                        {
                            case "NoData":
                                clsShowMessage.Show("Debe ingresar el nombre de usuario y/o la contraseña.", clsEnum.MessageType.Informa);
                                break;
                            case "NoGym":
                                clsShowMessage.Show("No se encontró el gimnasio en la base de datos de GSW o no fue posible consultar este.", clsEnum.MessageType.Informa);
                                break;
                            case "NoUserOrPassword":
                                clsShowMessage.Show("Nombre de usuario y/o contraseña incorrectos.", clsEnum.MessageType.Informa);
                                break;
                            case "NoId":
                                clsShowMessage.Show("No fue posible obtener la identificación del usuario de SiboPaw.", clsEnum.MessageType.Informa);
                                break;
                            case "NoEmployee":
                                clsShowMessage.Show("No fue posible consultar al usuario como empleado del gimnasio.", clsEnum.MessageType.Informa);
                                break;
                            default:
                                clsShowMessage.Show("Nombre de usuario y/o contraseña incorrectos.", clsEnum.MessageType.Informa);
                                break;
                        }

                        return;
                    }
                }
                else
                {
                    clsShowMessage.Show("No fue posible validar el usuario y/o la contraseña ingresados.", clsEnum.MessageType.Informa);
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    txtPwd.Focus();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void txtPwd_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    btnOk.PerformClick();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
