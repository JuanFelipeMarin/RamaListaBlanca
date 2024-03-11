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
    public class ClientBLL
    {
        public gim_clientes GetActiveClient(int gymId, string clientId)
        {
            try
            {
                ClientDAL clientDAL = new ClientDAL();
                Exception ex;
                gim_clientes client = new gim_clientes();

                if (gymId == 0 || string.IsNullOrEmpty(gymId.ToString()))
                {
                    ex = new Exception(Exceptions.nullGym);
                    throw ex;
                }

                if (string.IsNullOrEmpty(clientId.ToString()))
                {
                    ex = new Exception(Exceptions.nullId);
                    throw ex;
                }

                client = clientDAL.GetActiveClient(gymId, clientId);
                return client;
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
