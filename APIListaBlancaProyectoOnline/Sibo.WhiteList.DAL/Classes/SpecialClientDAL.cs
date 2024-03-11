using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class SpecialClientDAL
    {
        public gim_clientes_especiales GetSpecialClient(int gymId, string id)
        {
            double scId = Convert.ToDouble(id);
            gim_clientes_especiales scEntity = new gim_clientes_especiales();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                scEntity = (from sc in context.gim_clientes_especiales
                            where sc.cdgimnasio == gymId && sc.cli_identifi == scId
                            select sc).FirstOrDefault();
            }

            return scEntity;
        }
    }
}
