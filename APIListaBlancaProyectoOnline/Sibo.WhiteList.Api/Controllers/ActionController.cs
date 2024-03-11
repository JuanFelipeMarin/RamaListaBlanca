using Sibo.WhiteList.BLL.Classes;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace Sibo.WhiteList.Api.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    //[Authorize]
    [RoutePrefix("api/Action")]


    public class ActionController : ApiController
    {
        ActionBLL actionbll = new ActionBLL();

        [HttpGet]
        [Route("Action/{gymId}/{branchId}/{IpAddress}")]
        public IHttpActionResult GetAction(int gymId , string branchId, string IpAddress)
        {
            try
            {
               List<tblAction> list = new List<tblAction>();
               list = actionbll.GetAction(gymId, branchId, IpAddress.Replace(',', '.'));

                return Ok(list);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        [Route("GetActionUpdate/{id}/{ipAddress}/{intIdSucursales}/{cdgimnasio}")]
        public IHttpActionResult GetActionUpdate(int id, string ipAddress, string intIdSucursales, int cdgimnasio)
        {
            try
            {
                bool resp = false;
                resp = actionbll.GetActionUpdate(id,ipAddress.Replace(',', '.'), intIdSucursales, cdgimnasio);
                return Ok(resp);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("InsertAction/{gymId}/{branchId}/{IpAddress}/{enrolar}")]
        public IHttpActionResult InsertAction(int gymId, string branchId, string IpAddress, string enrolar)
        {
            try
            {
                int list = 0;
                list = actionbll.InsertAction(gymId, branchId, IpAddress.Replace(',', '.'), enrolar);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> AddActionParameters(List<eActionParameters> entryList)
        {
            try
            {
                bool responseBool = false;
                responseBool = await Task.Run(() => actionbll.Insert(entryList));
                return Ok(responseBool);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
