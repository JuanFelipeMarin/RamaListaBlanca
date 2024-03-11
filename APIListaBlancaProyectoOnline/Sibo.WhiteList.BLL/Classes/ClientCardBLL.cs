using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.BLL.Classes
{
    public class ClientCardBLL
    {
        public List<eClientCard> GetClientCards(int gymId, string clientId)
        {
            ClientCardDAL clientCardDAL = new ClientCardDAL();
            return clientCardDAL.GetClientCards(gymId, clientId);
        }
    }
}
