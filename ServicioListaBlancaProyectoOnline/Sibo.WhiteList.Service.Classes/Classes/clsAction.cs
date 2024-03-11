using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.BLL.Helpers;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Classes
{
    public class clsAction
    {
        public List<eAction> GetPendingActionsByTerminal(string ipAddress)
        {
            try
            {
                List<eAction> actionList = new List<eAction>();
                ActionBLL actionBll = new ActionBLL();
                actionList = actionBll.GetPendingActionsByTerminal(ipAddress);
                return actionList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Update(eAction action)
        {
            try
            {
                ActionBLL actionBll = new ActionBLL();
                return actionBll.Update(action);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
