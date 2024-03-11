using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.IngresoMonitor.Classes
{
    public class clsAccessControlSettings
    {
        public eConfiguration GetLocalAccessControlSettings()
        {
            try
            {
                AccessControlSettingsBLL acsBLL = new AccessControlSettingsBLL();
                return acsBLL.GetLocalAccessControlSettings();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
