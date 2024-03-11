using Sibo.WhiteList.BLL.Classes;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Sibo.WhiteList.Api.Controllers
{
    [RoutePrefix("api/Holiday")]
    public class HolidayController : ApiController
    {
        HolidayBLL hdBll = new HolidayBLL();

        /// <summary>
        /// Método encargado de consultar los días festivos asociados a un gimnasio en la BD de GSW.
        /// </summary>
        /// <param name="gymId">Id del gimnasio (cdgimnasio o gymId)</param>
        /// <returns>Retorna una lista de festivos en formato entidad.</returns>
        [Route("GetHolidays/{gymId}")]
        [ResponseType(typeof(List<eHoliday>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetHolidays(int gymId)
        {
            try
            {
                List<eHoliday> responseList = new List<eHoliday>();
                responseList = await Task.Run(() => hdBll.GetHolidays(gymId));
                return Ok(responseList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}