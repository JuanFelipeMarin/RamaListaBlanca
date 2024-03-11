using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class BlackListDAL
    {
        public bool ValidateActiveRecord(int gymId, double id)
        {
            gim_listanegra blackList = new gim_listanegra();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                blackList = context.gim_listanegra.FirstOrDefault(bl => bl.cdgimnasio == gymId && bl.listneg_floatId == id && bl.listneg_bitEstado == true);
            }

            if (blackList == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
