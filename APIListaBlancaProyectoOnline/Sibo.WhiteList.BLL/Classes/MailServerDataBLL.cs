using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.BLL.Classes
{
    public class MailServerDataBLL
    {
        public eMailServerData GetMailServerData(int gymId)
        {
            MailServerDataDAL msdDAL = new MailServerDataDAL();
            eMailServerData response = new eMailServerData();
            response = msdDAL.GetMailServerData(gymId);

            if (response == null)
            {
                response = new eMailServerData();
            }

            return response;
        }
    }
}
