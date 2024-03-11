using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.IngresoTouch.Classes
{
    public class clsActionParameters
    {
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
