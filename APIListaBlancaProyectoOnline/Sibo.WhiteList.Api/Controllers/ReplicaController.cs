using Sibo.WhiteList.BLL.Classes;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using System.Threading.Tasks;
using Sibo.WhiteList.Classes;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Entities.Classes;

namespace Sibo.WhiteList.Api.Controllers
{
    [System.Web.Http.RoutePrefix("api/Replica")]
    public class ReplicaController : ApiController
    {
        ReplicaBLL replicaBLL = new ReplicaBLL();


        [HttpGet]
        [Route("GetActualizarEstadoReplica/{gymId}/{branchId}/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetActualizarEstadoReplica(int gymId, string id,string ipTerminal)
        {
            bool responseBool = false;
            responseBool = await Task.Run(() => replicaBLL.GetActualizarEstadoReplica(gymId, id, ipTerminal.Replace(',', '.')));
            return Ok(responseBool);
        }

        [HttpGet]
        [Route("GetEliminarReplicaPersona/{gymId}/{intIdHuella_Tarjeta}/{idPersona}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetEliminarReplicaPersona(int gymId, string intIdHuella_Tarjeta, string idPersona)
        {
            bool responseBool = false;
            responseBool = await Task.Run(() => replicaBLL.GetEliminarReplicaPersona(gymId, intIdHuella_Tarjeta, idPersona));
            return Ok(responseBool);
        }

        [HttpPost]
        [Route("AddReplicaHuellas")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> AddReplicaHuellas(eReplicatedFingerprint entity)
        {
            try
            {
                return Ok(await Task.Run(() => replicaBLL.InsertReplicaHuellas(entity)));
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}