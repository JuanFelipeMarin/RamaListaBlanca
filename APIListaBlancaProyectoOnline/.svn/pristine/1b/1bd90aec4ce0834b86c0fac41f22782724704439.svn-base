using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class GymDAL
    {
        public gim_gimnasio GetGymById(int gymId)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                return context.gim_gimnasio.FirstOrDefault(g => g.cdgimnasio == gymId);
            }
        }
    }
}
