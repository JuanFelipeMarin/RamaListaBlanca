using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.DAL;

namespace Sibo.WhiteList.DAL.Classes
{
   public class ZonasDAL
    {
        public List<tbl_Maestro_Zonas> GetZonas(int gymId, string branchId)
        {
            List<tbl_Maestro_Zonas> list = new List<tbl_Maestro_Zonas>();
            string[] ListabranchId = branchId.Split(',');

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                int sucursal = 0;
                for (int i = 0; i < ListabranchId.Length; i++)
                {
                    sucursal = Convert.ToInt32(ListabranchId[i]);

                    var listterminal = (from ter in context.tbl_Maestro_Zonas
                            where ter.cdgimnasio == gymId && ter.intSucursal == sucursal
                            select ter).ToList();

                    foreach (var item in listterminal)
                    {
                        list.Add(item);
                    }

                }
                    
            }

            return list;
        }

    }
}
