using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class ConfigurationDAL
    {
        public tblConfiguracion GetConfiguration(int gymId)
        {
            tblConfiguracion config = new tblConfiguracion();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                config = (from con in context.tblConfiguracion
                          where con.cdgimnasio == gymId
                          select con).FirstOrDefault();
            }

            return config;
        }
    }
}
