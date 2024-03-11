using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.Servicio.ListaBlanca.Classes
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
    }
}
