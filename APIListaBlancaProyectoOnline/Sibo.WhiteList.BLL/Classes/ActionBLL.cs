using Sibo.WhiteList.DAL;
using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.BLL.Classes
{
   public class ActionBLL
    {
        ActionDAL actiondAL = new ActionDAL();
        public List<tblAction> GetAction(int gymId, string branchId, string IpAddress)
        {
            return actiondAL.GetAction(gymId, branchId, IpAddress);

        }

        public bool GetActionUpdate(int id, string ipAddress, string intIdSucursales, int cdgimnasio)
        {
            bool resp = actiondAL.GetActionUpdate(id, ipAddress, intIdSucursales, cdgimnasio);
            return resp;

        }

        public int InsertAction(int gymId, string branchId, string IpAddress, string enrolar)
        {
           return actiondAL.InsertAction(gymId, branchId, IpAddress, enrolar); 
        }

        public bool Insert(List<eActionParameters> aParametersList)
        {
            try
            {
                 bool resp = false;

                if (aParametersList != null && aParametersList.Count > 0)
                {
                    foreach (eActionParameters item in aParametersList)
                    {
                        resp = actiondAL.Insert(item);
                    }
                }

                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
