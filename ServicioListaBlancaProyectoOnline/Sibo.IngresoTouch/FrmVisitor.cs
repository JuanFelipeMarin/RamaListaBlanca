using Sibo.WhiteList.Service.BLL;
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
    public partial class FrmVisitor : Form
    {
        public int gymId = 0, branchId = 0;
        int idPK = 0;
        public string employeeId = string.Empty;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FrmVisitor_Load(object sender, EventArgs e)
        {
            try
            {
                clsShowMessage.Show("Por favor ingrese la identificación del visitante y presione la tecla Enter.", clsEnum.MessageType.Informa);
                EnableDisableControls(false);
            }
            catch (Exception ex)
            {
                clsShowMessage.Show("Ocurrió un error al abrir el formulario. Error real: " + ex.Message, clsEnum.MessageType.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Método para habilitar o deshabilitar los controles de la pantalla de visitantes.
        /// GEtulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="enable"></param>
        private void EnableDisableControls(bool enable)
        {
            try
            {
                cmbIdType.Enabled = enable;
                txtName.Enabled = enable;
                txtFirstLastName.Enabled = enable;
                txtSecondLastName.Enabled = enable;
                dtpBornDate.Enabled = enable;
                cmbGenre.Enabled = enable;
                txtPhone.Enabled = enable;
                cmbEPS.Enabled = enable;
                txtAddress.Enabled = enable;
                txtEmail.Enabled = enable;
                chkEntryFingerprint.Enabled = enable;
                cmbVisitedPerson.Enabled = enable;
                cmbWhoAuthorized.Enabled = enable;
                txtReason.Enabled = enable;
                txtElements.Enabled = enable;
                numVisitTime.Enabled = enable;
                btnCancel.Enabled = enable;
                btnSave.Enabled = enable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método encargado de consultar los datos maestros necesarios para crear visitantes y/o visitas, y enviar a llenar los combos correspondientes.
        /// Getulio Vargas - 2018-08-29 - OD 1307
        /// </summary>
        private void GetMasterData(string visitorId)
        {
            try
            {
                VisitorBLL visitorBll = new VisitorBLL();
                eVisitorData visitorData = new eVisitorData();
                //Consultamos los datos maestros en GSW por medio de la API.
                visitorData = visitorBll.GetVisitorData(gymId, visitorId);
                //Creamos la lista de géneros para llenar el combo de estos
                List<eGeneric> genreList = new List<eGeneric>();
                genreList.Add(new eGeneric { key = "0", value = "--Seleccione--" });
                genreList.Add(new eGeneric { key = "1", value = "Masculino" });
                genreList.Add(new eGeneric { key = "2", value = "Femenino" });

                if (visitorData != null && visitorData.visitedPerson.Count > 0 && visitorData.eps.Count > 0 && visitorData.idType.Count > 0)
                {
                    //Enviamos a llenar todos los combos
                    FillCombo(genreList, cmbGenre);
                    FillCombo(visitorData.visitedPerson, cmbVisitedPerson);
                    FillCombo(visitorData.whoAuthorized, cmbWhoAuthorized);
                    FillCombo(visitorData.eps, cmbEPS);
                    FillCombo(visitorData.idType, cmbIdType);

                    //Habilitamos los controles de la pantalla.
                    EnableDisableControls(true);

                    if (visitorData.visitor != null)
                    {
                        //Llenamos los controles en caso de existir el visitante.
                        FillControls(visitorData.visitor);
                    }
                }
                else
                {
                    clsShowMessage.Show("No es posible crear visitantes y/o visitas en este momento; no se consultaron correctamente los datos maestros (EPS, EMPLEADOS, TIPOS DE IDENTIFICACIÓN).", clsEnum.MessageType.Informa);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite llenar los controles en caso de que el visitante exista.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="visitor"></param>
        private void FillControls(eVisitor visitor)
        {
            try
            {
                cmbIdType.SelectedValue = visitor.idType.ToString();
                txtName.Text = visitor.name;
                txtFirstLastName.Text = visitor.firstLastName;
                txtSecondLastName.Text = visitor.secondLastName;
                dtpBornDate.Value = visitor.bornDate;
                cmbGenre.SelectedValue = visitor.genre.ToString();
                txtPhone.Text = visitor.phone;
                cmbEPS.SelectedValue = visitor.eps.ToString();
                txtAddress.Text = visitor.address;
                txtEmail.Text = visitor.email;
                chkEntryFingerprint.Checked = visitor.entryWithFingerprint;
                idPK = visitor.idPK;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite llenar un combobox a partir de una lista genérica.
        /// Getulio Vargas - 2018-08-29 - OD 1307
        /// </summary>
        /// <param name="list"></param>
        /// <param name="cmb"></param>
        private void FillCombo(List<eGeneric> list, ComboBox cmb)
        {
            try
            {
                cmb.DataSource = list;
                cmb.DisplayMember = "value";
                cmb.ValueMember = "key";
                cmb.AutoCompleteCustomSource.Clear();
                cmb.AutoCompleteSource = AutoCompleteSource.ListItems;
                cmb.SelectedValue = "0";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void txtId_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    if (string.IsNullOrEmpty(txtId.Text))
                    {
                        clsShowMessage.Show("Debe ingresar la identificación del visitante.", clsEnum.MessageType.Informa);
                        return;
                    }
                    else
                    {
                        GetMasterData(txtId.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                VisitorBLL visBll = new VisitorBLL();
                WhiteListBLL wlBll = new WhiteListBLL();
                eWhiteList wlEntity = new eWhiteList();
                eVisitor visEntity = new eVisitor();
                eVisit visit = new eVisit();

                if (ValidateFields())
                {
                    visEntity.address = txtAddress.Text;
                    visEntity.bornDate = dtpBornDate.Value;
                    visEntity.branchId = branchId;
                    visEntity.email = txtEmail.Text;
                    visEntity.entryWithFingerprint = chkEntryFingerprint.Checked;
                    visEntity.eps = Convert.ToInt32(cmbEPS.SelectedValue);
                    visEntity.firstLastName = txtFirstLastName.Text;
                    visEntity.genre = Convert.ToInt32(cmbGenre.SelectedValue);
                    visEntity.gymId = gymId;
                    visEntity.idPK = idPK;
                    visEntity.idType = Convert.ToInt32(cmbIdType.SelectedValue);
                    visEntity.name = txtName.Text;
                    visEntity.phone = txtPhone.Text;
                    visEntity.secondLastName = txtSecondLastName.Text;
                    visEntity.userId = employeeId;
                    visEntity.visitorId = txtId.Text;
                    visit.elements = txtElements.Text;
                    visit.reason = txtReason.Text;
                    visit.time = Convert.ToInt32(numVisitTime.Value);
                    visit.visitedPerson = cmbVisitedPerson.SelectedValue.ToString();
                    visit.whoAuthorized = cmbWhoAuthorized.SelectedValue.ToString();
                    visEntity.visit = visit;

                    wlEntity = visBll.InsertVisit(visEntity);

                    if (wlEntity != null && !string.IsNullOrEmpty(wlEntity.id))
                    {
                        wlBll.Insert(wlEntity);

                        if (!wlEntity.withoutFingerprint)
                        {
                            if (wlEntity.fingerprint == null)
                            {
                                clsShowMessage.Show("La visita ha sido registrada con éxito, para que el visitante pueda ingresar es necesario grabar la huella de este.",
                                                clsEnum.MessageType.Informa);
                                this.DialogResult = DialogResult.OK;
                            }
                            else
                            {
                                clsShowMessage.Show("La visita ha sido registrada con éxito, el visitante ya puede ingresar debido a que tiene huella registrada.",
                                                clsEnum.MessageType.Informa);
                                this.DialogResult = DialogResult.OK;
                            }
                        }
                        else
                        {
                            clsShowMessage.Show("La visita ha sido registrada con éxito, el visitante puede ingresar con la identificación registrada.",
                                                clsEnum.MessageType.Informa);
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite validar los campos necesarios para guardar la visita.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <returns></returns>
        private bool ValidateFields()
        {
            try
            {
                if (string.IsNullOrEmpty(txtId.Text))
                {
                    clsShowMessage.Show("Debe ingresar la identificaión del visitante.", clsEnum.MessageType.Informa);
                    txtId.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(cmbIdType.Text) || cmbIdType.SelectedValue.ToString() == "0")
                {
                    clsShowMessage.Show("Debe seleccionar el tipo de identificación del visitante.", clsEnum.MessageType.Informa);
                    cmbIdType.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(txtName.Text))
                {
                    clsShowMessage.Show("Debe ingresar el nombre del visitante.", clsEnum.MessageType.Informa);
                    txtName.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(txtFirstLastName.Text))
                {
                    clsShowMessage.Show("Debe ingresar el primer apellido del visitante.", clsEnum.MessageType.Informa);
                    txtFirstLastName.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(dtpBornDate.Value.ToString()))
                {
                    clsShowMessage.Show("Debe ingresar la fecha de nacimiento del visitante.", clsEnum.MessageType.Informa);
                    dtpBornDate.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(cmbGenre.Text) || cmbGenre.SelectedValue.ToString() == "0")
                {
                    clsShowMessage.Show("Debe seleccionar el género del visitante.", clsEnum.MessageType.Informa);
                    cmbGenre.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(txtEmail.Text))
                {
                    clsShowMessage.Show("Debe ingresar el email del visitante.", clsEnum.MessageType.Informa);
                    txtEmail.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(cmbVisitedPerson.Text) || cmbVisitedPerson.SelectedValue.ToString() == "0")
                {
                    clsShowMessage.Show("Debe seleccionar la persona visitada.", clsEnum.MessageType.Informa);
                    cmbVisitedPerson.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(cmbWhoAuthorized.Text) || cmbWhoAuthorized.SelectedValue.ToString() == "0")
                {
                    clsShowMessage.Show("Debe seleccionar quien autoriza la visita.", clsEnum.MessageType.Informa);
                    cmbWhoAuthorized.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(numVisitTime.Value.ToString()) || numVisitTime.Value <= 0)
                {
                    clsShowMessage.Show("Debe indicar cual es el tiempo de la visita.", clsEnum.MessageType.Informa);
                    numVisitTime.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(txtReason.Text))
                {
                    clsShowMessage.Show("Debe ingresar el motivo de la visita.", clsEnum.MessageType.Informa);
                    txtReason.Focus();
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FrmVisitor()
        {
            InitializeComponent();
        }
    }
}
