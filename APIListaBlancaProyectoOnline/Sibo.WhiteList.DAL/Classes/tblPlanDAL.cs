using System;
using System.Collections.Generic;
using System.Linq;

namespace Sibo.WhiteList.DAL.Classes
{
    public class tblPlanDAL
    {
        public List<spGetPlanesByIdUser_Result> getPlanByIdUser(int idEmpresa, int id)
        {
            try
            {
                List<spGetPlanesByIdUser_Result> res = new List<spGetPlanesByIdUser_Result>();
                using (var context = new dbWhiteListModelEntities(idEmpresa))
                {
                    res = context.spGetPlanesByIdUser(idEmpresa, id.ToString()).ToList();
                }
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
