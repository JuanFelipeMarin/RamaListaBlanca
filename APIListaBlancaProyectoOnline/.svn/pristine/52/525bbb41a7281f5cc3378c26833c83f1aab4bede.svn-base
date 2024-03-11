using Sibo.WhiteList.BLL.Classes;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Sibo.WhiteList.Api.Controllers
{
    [RoutePrefix("api/Visitor")]
    public class VisitorController : ApiController
    {
        /// <summary>
        /// Método encargado de consultar y retornar la lista de maestros necesarios para poder utilizar la pantalla de visitantes en el ingreso.
        /// </summary>
        /// <param name="gymId">Id del gimnasio (cdgimnasio o gymId)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDataToVisitor/{gymId}/{visitorId}")]
        [ResponseType(typeof(eVisitorData))]
        public async Task<IHttpActionResult> GetDataToVisitor(int gymId, string visitorId)
        {
            try
            {
                VisitorBLL visitorBll = new VisitorBLL();
                eVisitorData master = new eVisitorData();
                master = await Task.Run(() => visitorBll.GetDataToVisitor(gymId, visitorId));
                return Ok(master);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Método encargado de insertar un visitante en la BD de GSW.
        /// </summary>
        /// <param name="visitor">Recibe la entidad 'eVisitor' como parámetro.</param>
        /// <returns>Retorna un registro de lista blanca para insertar en la BD local.</returns>
        [HttpPost]
        [Route("InsertVisitor")]
        [ResponseType(typeof(DAL.WhiteList))]
        public async Task<IHttpActionResult> InsertVisitor(eVisitor visitor)
        {
            VisitorBLL visBll = new VisitorBLL();
            DAL.WhiteList response = new DAL.WhiteList();
            response = await Task.Run(() => visBll.InsertUpdateVisitor(visitor));
            return Ok(response);
        }
    }
}