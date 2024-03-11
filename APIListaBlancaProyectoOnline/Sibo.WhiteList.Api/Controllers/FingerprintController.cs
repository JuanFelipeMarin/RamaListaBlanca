using Sibo.WhiteList.Api.Classes;
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
    [RoutePrefix("api/Fingerprint")]
    public class FingerprintController : ApiController
    {
        FingerprintBLL fingerprintBLL = new FingerprintBLL();
        WhiteListBLL whiteListBLL = new WhiteListBLL();

        /// <summary>
        /// Método para la APP (Por ejemplo).
        /// Método que permite descargar la huella de un usuario en una sucursal diferente siempre y cuando el plan activo tenga permitido entrar a la sucursal solicitada.
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <param name="clientId"></param>
        /// <returns>Es método retorna un parámetro booleano que en caso de ser 'true', 
        /// quiere decir que el registro se insertó en la lista blanca de GSW y se debe esperar que el servicio lo descargue a la base de datos local.</returns>
        [HttpPost]
        [Route("DownloadFingerprint/{gymId}/{branchId}/{clientId}")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> DownloadFingerprint(int gymId, int branchId, string clientId)
        {
            try
            {
                bool responseBool = false;
                responseBool = await Task.Run(() => whiteListBLL.DownloadClient(gymId, branchId.ToString(), clientId));

                return Ok(responseBool);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Método para el ingreso (Por ejemplo).
        /// Método que permite descargar la huella de un usuario en una sucursal diferente siempre y cuando el plan activo tenga permitido entrar a la sucursal solicitada.
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <param name="clientId"></param>
        /// <returns>Este método retorna un registro de la entidad WhiteList (Lista blanca), el cual será insertado en la base de datos local.</returns>
        [HttpPost]
        [Route("DownloadFingerprintToLocalWhiteList/{gymId}/{branchId}/{clientId}")]
        [ResponseType(typeof(DAL.WhiteList))]
        public async Task<IHttpActionResult> DownloadFingerprintToLocalWhiteList(int gymId, int branchId, string clientId)
        {
            try
            {
                DAL.WhiteList response = new DAL.WhiteList();
                response = await Task.Run(() => whiteListBLL.DownloadFingerprintToLocalWhiteList(gymId, branchId, clientId));

                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Método que permite validar si a un usuario específico se le puede grabar la huella.
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ValidatePersonToSaveFingerprint/{gymId}/{branchId}/{id}")]
        [ResponseType(typeof(eResponse))]
        public async Task<IHttpActionResult> ValidatePersonToSaveFingerprint(int gymId, int branchId, string id)
        {
            try
            {
                eResponse responseBool = new eResponse();
                responseBool = await Task.Run(() => fingerprintBLL.ValidatePersonToSaveFingerprint(gymId, branchId, id));
                return Ok(responseBool);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Método encargado de recibir una lista de huellas e insertar estas en la BD.
        /// </summary>
        /// <param name="fingerprintList"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(List<eFingerprint>))]
        public async Task<IHttpActionResult> Add(List<eFingerprint> fingerprintList)
        {
            try
            {
                List<eFingerprint> responseList = new List<eFingerprint>();
                responseList = await Task.Run(() => fingerprintBLL.AddUpdateFingerprint(fingerprintList));
                return Ok(responseList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Método encargado de validar si una huella ya existe en la base de datos "para no duplicar esta" y en caso de no existir se guarda la huella en la base de datos de GSW.
        /// </summary>
        /// <param name="fingerEntity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ValidateAndSaveFingerprint")]
        [ResponseType(typeof(eResponse))]
        public async Task<IHttpActionResult> ValidateAndSaveFingerprint(eFingerprint fingerEntity)
        {
            try
            {
                eResponse response = new eResponse();
                SupremaBLL suprema = new SupremaBLL();
                response = await Task.Run(() => suprema.ValidateAndSaveFingerprint(fingerEntity));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Método que permite validar si una huella existe en la BD.
        /// </summary>
        /// <param name="requestFingerprint"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ValidateFingerprint")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> ValidateFingerprint(eRequestFingerprint requestFingerprint)
        {
            try
            {
                string response = string.Empty;
                SupremaBLL suprema = new SupremaBLL();
                response = await Task.Run(() => suprema.ValidateFingerprint(requestFingerprint.gymId, requestFingerprint.strData, requestFingerprint.imageBytes));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        
        /// <summary>
        /// Método que permite guardar la firma del contrato realizada al momento de enrolar el usuario desde el ingreso.
        /// </summary>
        /// <param name="essc"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveSignedContract")]
        [ResponseType(typeof(eSendClientContract))]
        public async Task<IHttpActionResult> SaveSignedContract(eSaveSignedContract essc)
        {
            try
            {
                eSendClientContract response = new eSendClientContract();
                FingerprintBLL fingerBll = new FingerprintBLL();
                response = await Task.Run(() => fingerBll.SaveSignedContract(essc));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("GetAllFingerprintsPerson/{gymId}/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetAllFingerprintsPerson(int gymId, string id)
        {
            try
            {
                List <eFingerprintWL> response = new List<eFingerprintWL>();
                SupremaBLL suprema = new SupremaBLL();
                response = await Task.Run(() => suprema.GetAllFingerprintsPerson(gymId, id));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
