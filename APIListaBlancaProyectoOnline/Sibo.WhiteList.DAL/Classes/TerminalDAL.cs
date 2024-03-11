using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.DAL;

namespace Sibo.WhiteList.DAL.Classes
{
    public class TerminalDAL
    {
        public List<Terminal> GetTerminals(int gymId, string branchId)
        {
            List<Terminal> list = new List<Terminal>();
            string[] ListabranchId = branchId.Split(',');

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                int sucursal = 0;
                for (int i = 0; i < ListabranchId.Length; i++)
                {
                    sucursal = Convert.ToInt32(ListabranchId[i]);

                   var listterminal = (from ter in context.Terminal
                            where ter.cdgimnasio == gymId && ter.ter_intBranchId == sucursal && ter.state == true
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
