using Sibo.WhiteList.BLL.Classes;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Sibo.WhiteList.Api.Controllers
{
    [RoutePrefix("api/Entry")]
    public class EntryController : ApiController
    {
        EntryBLL entryBLL = new EntryBLL();

        /// <summary>
        /// Método encargado de guardar entradas desde la BD local hasta la BD de GSW.
        /// </summary>
        /// <param name="entryList"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> Add(List<eEntry> entryList)
        {
            try
            {
                bool responseBool = false;
                responseBool = await Task.Run(() => entryBLL.AddEntry(entryList));
                return Ok(responseBool);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("GetListEntradasUsuarios/{gymId}/{branchId}/{id}/{TipoPlan}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetListEntradasUsuarios(int gymId, string branchId, string id, bool TipoPlan)
        {
            List<eEntry> responseBool = new List<eEntry>();
            responseBool = await Task.Run(() => entryBLL.GetListEntradasUsuarios(gymId, branchId, id, TipoPlan));
            return Ok(responseBool);
        }
    }
}
