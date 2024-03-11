using Sibo.WhiteList.Service.BLL;
using System;

namespace Sibo.WhiteList.Service.Classes
{
    public class clsClients
    {
        public bool GetClientMessages(int gymId, int branchId)
        {
            try
            {
                ClientMessagesBLL msgBLL = new ClientMessagesBLL();
                return msgBLL.GetClientsMessages(gymId, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
