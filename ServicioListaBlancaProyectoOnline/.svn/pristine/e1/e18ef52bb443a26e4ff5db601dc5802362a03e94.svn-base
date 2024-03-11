using Sibo.WhiteList.Service.DAL.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.BLL.BLL
{
    public class ScheduleBLL
    {
        /// <summary>
        /// Método para consultar los horarios para borrado de huellas de las terminales.
        /// Getulio Vargas - 2018-08-31 - OD 1307
        /// </summary>
        /// <returns></returns>
        public DataTable GetFingerprintSchedules()
        {
            dSchedule srfData = new dSchedule();
            return srfData.GetFingerprintSchedules();
        }

        public DataTable GetUserSchedules()
        {
            dSchedule srfData = new dSchedule();
            return srfData.GetUserSchedules();
        }

        public DataTable GetEventSchedules()
        {
            dSchedule srfData = new dSchedule();
            return srfData.GetEventSchedules();
        }

        public DataTable GetReplicateUserSchedules()
        {
            dSchedule srfData = new dSchedule();
            return srfData.GetReplicateUserSchedules();
        }
    }
}
