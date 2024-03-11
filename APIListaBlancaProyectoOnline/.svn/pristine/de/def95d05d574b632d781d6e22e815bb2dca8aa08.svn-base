using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.DAL;

namespace Sibo.WhiteList.BLL.Classes
{
    public class HolidayBLL
    {
        public List<eHoliday> GetHolidays(int gymId)
        {
            try
            {
                Exception ex;
                HolidayDAL hdDAL = new HolidayDAL();
                List<gim_festivos> holidays = new List<gim_festivos>();
                List<eHoliday> responseList = new List<eHoliday>();

                if (gymId == 0 || string.IsNullOrEmpty(gymId.ToString()))
                {
                    ex = new Exception(Exceptions.nullGym);
                    throw ex;
                }

                holidays = hdDAL.GetHolidays(gymId);

                if (holidays != null && holidays.Count > 0)
                {
                    foreach (gim_festivos item in holidays)
                    {
                        eHoliday hol = new eHoliday();

                        if (item.dia_festivo != null)
                        {
                            hol.id = item.nro_festivo;
                            hol.date = Convert.ToDateTime(item.dia_festivo);
                            responseList.Add(hol);
                        }
                    }
                }

                return responseList;
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
