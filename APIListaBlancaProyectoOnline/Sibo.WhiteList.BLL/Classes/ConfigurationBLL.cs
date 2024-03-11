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
    public class ConfigurationBLL
    {
        public tblConfiguracion GetConfiguration(int gymId)
        {
            ConfigurationDAL configDAL = new ConfigurationDAL();
            Validation val = new Validation();
            val.ValidateGym(gymId);
            return configDAL.GetConfiguration(gymId);
        }
    }
}
