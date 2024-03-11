using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.BLL.Classes
{
    public class CourtesyBLL
    {
        public gim_planes_usuario_especiales GetVigentCourtesy(int gymId, string id)
        {
            try
            {
                Exception ex;
                CourtesyDAL courtesyDAL = new CourtesyDAL();

                if (gymId == 0 || string.IsNullOrEmpty(gymId.ToString()))
                {
                    ex = new Exception(Exceptions.nullGym);
                    throw ex;
                }

                if (string.IsNullOrEmpty(id.ToString()))
                {
                    ex = new Exception(Exceptions.nullId);
                    throw ex;
                }

                return courtesyDAL.GetVigentCourtesy(gymId, id);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    throw ex;
                }
                else
                {
                    throw new Exception(Exceptions.serverError);
                }
            }
        }
    }
}
