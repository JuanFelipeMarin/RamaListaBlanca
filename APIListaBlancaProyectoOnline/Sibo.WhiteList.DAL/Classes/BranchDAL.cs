using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class BranchDAL
    {
        public gim_sucursales GetBranch(int gymId, string branchId)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                return context.gim_sucursales.FirstOrDefault(b => b.cdgimnasio == gymId && b.suc_intpkIdentificacion == Convert.ToInt32(branchId));
            }
        }
    }
}
