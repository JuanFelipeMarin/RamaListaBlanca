using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class HolidayDAL
    {
        public List<gim_festivos> GetHolidays(int gymId)
        {
            List<gim_festivos> holidays = new List<gim_festivos>();
            int actualYear = DateTime.Today.Year;
            DateTime initialDateYear = new DateTime(actualYear, 01, 01).Date, finalDateYear = new DateTime(actualYear, 12, 31).Date, genericDate = new DateTime(1900, 01, 01);

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                holidays = (from hd in context.gim_festivos
                            where hd.cdgimnasio == gymId && hd.dia_festivo.Value >= initialDateYear
                                  && hd.dia_festivo.Value <= finalDateYear
                            select hd).ToList();
            }

            return holidays;
        }
    }
}
