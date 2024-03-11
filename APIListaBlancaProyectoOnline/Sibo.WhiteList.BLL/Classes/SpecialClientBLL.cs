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
    public class SpecialClientBLL
    {
        public gim_clientes_especiales GetSpecialClient(int gymId, string id)
        {
            SpecialClientDAL scDAL = new SpecialClientDAL();

            if (gymId <= 0)
            {
                throw new Exception(Exceptions.nullGym);
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new Exception(Exceptions.nullId);
            }

            return scDAL.GetSpecialClient(gymId, id);
        }
    }
}
