using Sibo.WhiteList.BLL.Classes;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Sibo.WhiteList.DAL;
using System.Threading.Tasks;
using Sibo.WhiteList.Classes;
using Sibo.WhiteList.Service.Classes;
using Sibo.WhiteList.Entities.Classes;

namespace Sibo.WhiteList.Api.Controllers
{
    [RoutePrefix("api/Reserves")]
    public class ReservesController : ApiController
    {
        WhiteListBLL whiteList = new WhiteListBLL();
        ReservesBLL reservesBLL = new ReservesBLL();


        /// <summary>
        /// Método que se encarga de actualizar los registros de las reservas del cliente que acaba de ingresar
        /// </summary>
        /// <param name="reserves">Recibe un objeto con 3 propiedades: userId, gymId, reserveIds (los pk de las reservas a actualizar como asistido)</param>
        /// <returns>Retorna un valor 'True' cuando los datos se actualizan de forma correcta, un 'False' cuando no.</returns>
        [HttpPost]
        [Route("UpdateAsistedClases")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> UpdateAsistedClases(eReservesToUpdate reserves)
        {
            bool responseBool = false;
            responseBool = await Task.Run(() => reservesBLL.UpdateReserves(reserves));
            return Ok(responseBool);
        }

        [HttpGet]
        [Route("GetListReservaID/{gymId}/{branchId}/{idreserva}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetListReservaID(int gymId, int branchId, int idreserva)
        {
            List<eDatosReserva> listDato = new List<eDatosReserva>();
            listDato = await Task.Run(() => reservesBLL.GetListReservaID(gymId, branchId, idreserva));
            return Ok(listDato);
        }

    }
}
