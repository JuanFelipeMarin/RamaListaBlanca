using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.BLL.BLL
{
    public class VisitorBLL
    {
        public eVisitorData GetVisitorData(int gymId, string visitorId)
        {
            if (gymId <= 0)
            {
                throw new Exception("No es posible consultar los datos maestros, el id del gimnasio no se envió correctamente.");
            }

            VisitorAPI visAPI = new VisitorAPI();
            return visAPI.GetVisitorData(gymId, visitorId);
        }

        /// <summary>
        /// Método encargado de enviar a guardar por medio de la API una visita.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="visit"></param>
        /// <returns></returns>
        public eWhiteList InsertVisit(eVisitor visit)
        {
            VisitorAPI visAPI = new VisitorAPI();
            eWhiteList wlEntity = new eWhiteList();
            wlEntity = visAPI.InsertVisitor(visit);
            return wlEntity;
        }
    }
}
