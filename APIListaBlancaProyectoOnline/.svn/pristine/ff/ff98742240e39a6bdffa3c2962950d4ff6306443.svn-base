using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class ClientDAL
    {
        public gim_clientes GetActiveClient(int gymId, string clientId)
        {
            gim_clientes client = new gim_clientes();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                client = context.gim_clientes.FirstOrDefault(cli => cli.cdgimnasio == gymId && cli.cli_identifi == clientId && cli.cli_estado == true);
                return client;
            }
        }
    }
}
