using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.BLL.Helpers;
using Sibo.WhiteList.Service.Data;
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
    public partial class FrmDownloadFingerprint : Form
    {
        public int gymId = 0, branchId = 0;

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtId.Text))
                {
                    clsShowMessage.Show("Debe ingresar la identificación del usuario", clsEnum.MessageType.Informa);
                    return;
                }
                else
                {
                    FingerprintBLL fingerBll = new FingerprintBLL();
                    eWhiteList wlEntity = new eWhiteList();
                    DataTable dt = new DataTable();
                    dWhiteList wlData = new dWhiteList();

                    dt = wlData.GetInformacionZonas(txtId.Text);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        clsShowMessage.Show("El usuario está registrado en la base de datos local, no es necesario descargarlo nuevamente.", clsEnum.MessageType.Informa);
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        wlEntity = fingerBll.DownloadFingerprint(gymId, branchId, txtId.Text);

                        if (wlEntity != null && !string.IsNullOrEmpty(wlEntity.id))
                        {
                            //INGRESAMOS EL USUARIO QUE SE DESCARGÓ
                            UserBLL userBLL = new UserBLL();
                            eUser user = new eUser();
                            user = userBLL.GetUser(wlEntity.id);

                            if (user != null && !string.IsNullOrEmpty(user.userId))
                            {
                                if (user.fingerprintId != wlEntity.fingerprintId || user.withoutFingerprint != wlEntity.withoutFingerprint)
                                {
                                    userBLL.Update(wlEntity.id, wlEntity.withoutFingerprint, wlEntity.fingerprintId??0, true, false);
                                }
                            }
                            else
                            {
                                if ((wlEntity.fingerprintId != null && wlEntity.fingerprintId > 0) || ((wlEntity.fingerprintId == null || wlEntity.fingerprintId == 0) && wlEntity.withoutFingerprint))
                                {
                                    user = new eUser()
                                    {
                                        fingerprintId = wlEntity.fingerprintId ?? 0,
                                        userId = wlEntity.id,
                                        userName = wlEntity.name,
                                        withoutFingerprint = wlEntity.withoutFingerprint
                                    };

                                    userBLL.InsertUser(user);
                                }
                            }

                            //INGRESAMOS LA HUELLA DEL USUARIO QUE SE DESCARGÓ
                            FingerprintBLL fingerprintBLL = new FingerprintBLL();

                            if (wlEntity.fingerprintId > 0 && wlEntity.fingerprint != null)
                            {
                                fingerprintBLL.Insert(wlEntity.id, string.Empty, wlEntity.fingerprintId??0, wlEntity.fingerprint, 100);
                            }
                            else if (wlEntity.cantidadhuellas > 0)
                            {
                                foreach (eClientesPalmas itemHuella in wlEntity.tblPalmasId)
                                {
                                    //string userId, string ipAddress, int fingerprintId, byte[] fingerprint, int quality, string idhuellaActual = null
                                    fingerprintBLL.Insert(itemHuella.hue_identifi,"",Convert.ToInt32(itemHuella.hue_dedo),itemHuella.hue_dato,100, itemHuella.hue_id.ToString());
                                }
                            }
                            
                            clsShowMessage.Show("La huella fue descargada de forma correcta, ya puede ingresar a la sucursal.", clsEnum.MessageType.Informa);
                            this.DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            clsShowMessage.Show("No fue posible descargar la huella del cliente.", clsEnum.MessageType.Informa);
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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

        private void txtId_KeyPress(object sender, KeyPressEventArgs e)
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

        public FrmDownloadFingerprint()
        {
            InitializeComponent();
        }
    }
}
