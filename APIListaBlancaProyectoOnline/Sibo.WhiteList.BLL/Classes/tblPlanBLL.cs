using Sibo.WhiteList.DAL;
using Sibo.WhiteList.DAL.Classes;
using System;
using System.Collections.Generic;

namespace Sibo.WhiteList.BLL.Classes
{
    public class tblPlanBLL
    {
        tblPlanDAL obj = new tblPlanDAL();
        public List<spGetPlanesByIdUser_Result> getPlanByIdUser(int idEmpresa, int id)
        {
            try
            {
                return obj.getPlanByIdUser(idEmpresa, id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

