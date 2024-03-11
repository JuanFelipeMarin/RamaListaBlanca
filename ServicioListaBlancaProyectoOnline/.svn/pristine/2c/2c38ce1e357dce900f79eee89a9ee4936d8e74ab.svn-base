using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Service.Entities.Classes;
using Sibo.WhiteList.Service.DAL.Data;

namespace Sibo.WhiteList.Service.BLL.BLL
{
    public class ActionParametersBLL
    {
        public bool Insert(List<eActionParameters> aParametersList)
        {
            dActionParameters aParametersData = new dActionParameters();
            bool resp = false;

            if (aParametersList != null && aParametersList.Count > 0)
            {
                foreach (eActionParameters item in aParametersList)
                {
                    resp = aParametersData.Insert(item);
                }
            }

            return resp;
        }

        public List<eActionParameters> GetParameters(int actionId)
        {
            dActionParameters actionParametersBll = new dActionParameters();

            if (actionId > 0)
            {
                return actionParametersBll.GetParameters(actionId);
            }
            else
            {
                return null;
            }
        }
    }
}
