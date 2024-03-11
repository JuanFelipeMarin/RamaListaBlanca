using Sibo.WhiteList.IngresoMonitor.Classes;
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

namespace Sibo.WhiteList.IngresoMonitor
{
    public partial class FrmOpenDoor : Form
    {
        public string entryType = string.Empty, ipAddress = string.Empty;

        public FrmOpenDoor(string title)
        {
            InitializeComponent();
            this.Text = title;
        }

        private void FrmOpenDoor_Load(object sender, EventArgs e)
        {
            lblActualDate.Text = DateTime.Now.ToString("D");
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                clsAction act = new clsAction();
                int resp = 0;
                
                if (!string.IsNullOrEmpty(txtReason.Text))
                {
                    resp = act.Insert(ipAddress, clsEnum.ServiceActions.AdditionalEntryDisabled);

                    if (resp > 0)
                    {
                        clsActionParameters aParameters = new clsActionParameters();
                        List<eActionParameters> aParametersList = new List<eActionParameters>();
                        eActionParameters aParamEntityEntryReason = new eActionParameters();
                        eActionParameters aParamEntityEntryType = new eActionParameters();
                        aParamEntityEntryType.actionId = resp;
                        aParamEntityEntryType.parameterName = clsEnum.ActionParameters.EntryReason.ToString();
                        aParamEntityEntryType.parameterValue = txtReason.Text;
                        aParametersList.Add(aParamEntityEntryType);
                        aParamEntityEntryReason.actionId = resp;
                        aParamEntityEntryReason.parameterName = clsEnum.ActionParameters.EntryType.ToString();
                        aParamEntityEntryReason.parameterValue = entryType;
                        aParametersList.Add(aParamEntityEntryReason);
                        aParameters.Insert(aParametersList);

                        this.DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    clsShowMessage.Show("Debe ingresar un motivo.", clsEnum.MessageType.Informa);
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
