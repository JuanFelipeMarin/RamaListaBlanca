using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.DAL;

namespace Sibo.WhiteList.DAL.Classes
{
    public class MasterDataDAL
    {
        /// <summary>
        /// Método encargado de consultar los tipos de identificación registrados en un gimnasio y retornarlos en una lista.
        /// Getulio Vargas - 2018-08-29 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        public List<gim_tipo_identificacion> GetIdTypeByGym(int gymId)
        {
            List<gim_tipo_identificacion> idTypeList = new List<gim_tipo_identificacion>();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                idTypeList = (from it in context.gim_tipo_identificacion
                              where it.cdgimnasio == gymId
                              orderby it.tipoiden_descripc ascending
                              select it).ToList();
            }

            return idTypeList;
        }

        /// <summary>
        /// Método encargado de consultar las EPS registradas en un gimnasio y retornarlos en una lista.
        /// Getulio Vargas - 2018-08-29 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        public List<gim_eps> GetEPS(int gymId)
        {
            List<gim_eps> epsList = new List<gim_eps>();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                epsList = (from it in context.gim_eps
                           where it.cdgimnasio == gymId
                           orderby it.eps_nombre ascending
                           select it).ToList();
            }

            return epsList;
        }
    }
}
