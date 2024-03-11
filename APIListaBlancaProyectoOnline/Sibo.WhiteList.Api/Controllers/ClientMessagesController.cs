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
    [RoutePrefix("api/ClientMessages")]
    public class ClientMessagesController : ApiController
    {
        ClientMessagesBLL cm = new ClientMessagesBLL();

        /// <summary>
        /// Método encargado de consultar la lista de mensajes al cliente.
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetClientMessages/{gymId}/{branchId}")]
        [ResponseType(typeof(List<eClientMessages>))]
        public async Task<IHttpActionResult> GetClientMessages(int gymId, string branchId)
        {
            try
            {
                List<eClientMessages> responseList = new List<eClientMessages>();
                responseList = await Task.Run(() => cm.GetClientMessages(gymId, branchId.ToString()));
                return Ok(responseList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("GetClientMessagesDownload/{gymId}/{branchId}/{ipTerminal}")]
        [ResponseType(typeof(List<eClientMessages>))]
        public async Task<IHttpActionResult> GetClientMessagesDownload(int gymId, string branchId,string ipTerminal)
        {
            try
            {
                List<eClientMessages> responseList = new List<eClientMessages>();
                responseList = await Task.Run(() => cm.GetClientMessagesDownload(gymId, branchId, ipTerminal));
                return Ok(responseList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("GetClientMessagesInserReplicate/{gymId}/{branchId}/{IDImg}/{ipTerminal}")]
        public async Task<IHttpActionResult> GetClientMessagesInserReplicate(int gymId, string branchId, int IDImg, string ipTerminal)
        {
            try
            {
                bool resp = false;
                resp = await Task.Run(() => cm.GetClientMessagesInserReplicate(gymId, branchId, IDImg, ipTerminal));
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
