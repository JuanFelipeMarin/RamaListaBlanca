﻿using Sibo.WhiteList.BLL.Classes;
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
    [RoutePrefix("api/WhiteList")]
    public class WhiteListController : ApiController
    {
        WhiteListBLL whiteList = new WhiteListBLL();

        /// <summary>
        /// Método que retorna la lista de los registros pendientes y a eliminar de la lista blanca.
        /// </summary>
        /// <param name="gymId">Id del gimnasio (cdgimnasio o gymId)</param>
        /// <param name="branchId">Id de la sucursal</param>
        /// <returns>Retorna una lista de los registros pendientes por actualizar en la BD local.</returns>
        //[Route("GetWhiteList/{gymId}/{branchId}")]
        //[ResponseType(typeof(List<eWhiteList>))]
        //[HttpGet]
        //public async Task<IHttpActionResult> GetWhiteList(int gymId, string branchId)
        //{
        //    try
        //    {
        //        List<eWhiteList> responseList = new List<eWhiteList>();
        //        responseList = await Task.Run(() => whiteList.GetWhiteList(gymId, branchId));
        //        return Ok(responseList);
        //    }
        //    catch (Exception ex)
        //    {
                
        //        return InternalServerError(ex);
        //    }
        //}

        [Route("GetListPlanesSucursalPorPlan/{gymId}/{branchId}/{planId}")]
        [ResponseType(typeof(string))]
        [HttpGet]
        public async Task<IHttpActionResult> GetListPlanesSucursalPorPlan(int gymId, string branchId, int planId)
        {
            try
            {
                string strReturn = await Task.Run(() => whiteList.GetListPlanesSucursalPorPlan(gymId, branchId, planId));
                return Ok(strReturn);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("getZonasAcceso/{idEmpresa}/{idSucursal}/{idPlan}/{idReserva}")]
        [ResponseType(typeof(string))]
        [HttpGet]
        public async Task<IHttpActionResult> getZonasAcceso(int idEmpresa, string idSucursal, int idPlan, int idReserva)
        {
            try
            {
                return Ok(await Task.Run(() => whiteList.getZonasAcceso(idEmpresa, idSucursal, idPlan, idReserva)));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("GetActualizarCantidadTiquetes/{id}/{invoiceId}/{dianId}/{documentType}/{availableEntries}/{gymId}/{branchId}/ {planId}")]
        [ResponseType(typeof(string))]
        [HttpGet]
        public async Task<IHttpActionResult> GetActualizarCantidadTiquetes(int id, int invoiceId, int dianId, string documentType, int availableEntries, int gymId, string branchId, int planId)
        {
            try
            {
                bool strReturn = await Task.Run(() => whiteList.GetActualizarCantidadTiquetes(id, invoiceId, dianId, documentType, availableEntries, gymId, branchId, planId));
                return Ok(strReturn);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Método que se encarga de actualizar los registros de la lista blanca insertados en la lista blanca local.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Retorna un valor 'True' cuando los datos se actualizan de forma correcta, un 'False' cuando no.</returns>
        [HttpPost]
        [Route("UpdateWhiteList")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> UpdateWhiteList(eMarcarListaBlanca entity)
        {
            try
            {
                return Ok(await Task.Run(() => whiteList.UpdateWhiteListRecords(entity)));
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("GetListSucursalPorClase/{gymId}/{branchId}/{cdreserva}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetListSucursalPorClase(int gymId, int branchId, int cdreserva)
        {
            string responseBool = "";
            responseBool = await Task.Run(() => whiteList.GetListSucursalPorClase(gymId, branchId, cdreserva));
            return Ok(responseBool);
        }

        [HttpGet]
        [Route("GetListPersonaConsultar/{gymId}/{branchId}/{id}/{option}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetListPersonaConsultar(int gymId, string branchId, string id, bool option)
        {
            List<eWhiteList> responseBool = new List<eWhiteList>();
            responseBool = await Task.Run(() => whiteList.GetListPersonaConsultar(gymId, branchId, id, option));
            return Ok(responseBool);
        }

        [HttpGet]
        [Route("GetCantidadPersonasGrupoFamiliar/{gymId}/{branchId}/{idGrupo}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetCantidadPersonasGrupoFamiliar(int gymId, string branchId, string idGrupo)
        {
            string responseBool = "";
            responseBool = await Task.Run(() => whiteList.GetCantidadPersonasGrupoFamiliar(gymId, branchId, idGrupo));
            return Ok(responseBool);
        }

        [HttpGet]
        [Route("GetConsultarEliminacionContratos/{gymId}/{branchId}/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetConsultarEliminacionContratos(int gymId, string branchId, string id)
        {
            string responseBool = "";
            responseBool = await Task.Run(() => whiteList.GetConsultarEliminacionContratos(gymId, branchId, id));
            return Ok(responseBool);
        }

        [HttpGet]
        [Route("GetRespuestaIngresosVisitantes/{gymId}/{branchId}/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetRespuestaIngresosVisitantes(int gymId, string id)
        {
            bool responseBool = false;
            responseBool = await Task.Run(() => whiteList.GetRespuestaIngresosVisitantes(gymId, id));
            return Ok(responseBool);
        }

    }
}
