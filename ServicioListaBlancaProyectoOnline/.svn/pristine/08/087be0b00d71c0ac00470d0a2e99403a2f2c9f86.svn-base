using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Service.Entities.Classes;
using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.BLL.Helpers;

namespace Sibo.WhiteList.IngresoMonitor.Classes
{
    public class clsAction
    {
        public int Insert(string ipAddress, clsEnum.ServiceActions enrolar)
        {
            try
            {
                eAction action = new eAction();
                ActionBLL actionBll = new ActionBLL();
                action.ipAddress = ipAddress;
                action.strAction = enrolar.ToString();
                return actionBll.Insert(action);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
