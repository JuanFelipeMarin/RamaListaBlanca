using Sibo.WhiteList.Service.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Classes
{
    public class WhiteListProcess
    {
        public void GetWhiteList()
        {
            try
            {
                dWhiteList whiteList = new dWhiteList();
                DataTable dt = new DataTable();
                dt = whiteList.GetWhiteList(); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
