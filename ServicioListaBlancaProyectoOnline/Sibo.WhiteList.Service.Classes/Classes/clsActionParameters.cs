using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Classes
{
    public class clsActionParameters
    {
        public List<eActionParameters> GetParameters(int actionId)
        {
            try
            {
                ActionParametersBLL actionParamBll = new ActionParametersBLL();
                return actionParamBll.GetParameters(actionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Insert(List<eActionParameters> aParametersList)
        {
            try
            {
                ActionParametersBLL aParametersBll = new ActionParametersBLL();
                return aParametersBll.Insert(aParametersList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
