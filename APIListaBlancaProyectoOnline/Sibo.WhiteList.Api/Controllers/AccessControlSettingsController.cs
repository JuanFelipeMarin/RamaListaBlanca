﻿using Sibo.WhiteList.BLL.Classes;
using Sibo.WhiteList.Classes;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace Sibo.WhiteList.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/AccessControlSettings")]
    [AllowAnonymous]
    public class AccessControlSettingsController : ApiController
    {
        /// <summary>
        /// Método que se encarga de retornar la configuración del ingreso de una sucursal.
        /// </summary>
        /// <param name="gymId">Id del gimnasio (cdgimnasio o gymId)</param>
        /// <param name="branchId">Id de la sucursal</param>
        /// <returns>Retorna una entidad que tiene todos los parámetros de configuración</returns>
        [HttpGet]
        [Route("GetAccessControlConfiguration/{gymId}/{branchId}")]
        [ResponseType(typeof(eConfiguration))]
        public async Task<IHttpActionResult> GetAccessControlConfiguration(int gymId, string branchId)
        {
            try
            {
                AccessControlSettingsBLL acs = new AccessControlSettingsBLL();
                List<eConfiguration> config = new List<eConfiguration>();
                config = await Task.Run(() => acs.GetAccessControlConfigurationSpecialEntity(gymId, branchId));
                return Ok(config);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Método encargado de consultar la configuración de lista blanca.
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("GetConfigToResetLocalWhiteList/{gymId}/{branchId}")]
        //[ResponseType(typeof(bool))]
        //public async Task<IHttpActionResult> GetConfigToResetLocalWhiteList(int gymId, string branchId)
        //{
        //    try
        //    {
        //        AccessControlSettingsBLL acs = new AccessControlSettingsBLL();
        //        bool response = false;
                
        //        response = await Task.Run(() => acs.GetConfigToResetLocalWhiteList(gymId, branchId));
                
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
                
        //        return InternalServerError(ex);
        //    }
        //}

        /// <summary>
        /// Método encargado de actualizar el parámetro de reset de la lista blanca, este parámetro es muy importante ya que es quien dice si la lista blanca local se debe actualizar de forma general o no.
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("UpdateConfigToResetLocalWhiteList/{gymId}/{branchId}")]
        //[ResponseType(typeof(bool))]
        //public async Task<IHttpActionResult> UpdateConfigToResetLocalWhiteList(int gymId, string branchId)
        //{
        //    AccessControlSettingsBLL acs = new AccessControlSettingsBLL();
        //    bool response = false;
        //    response = await Task.Run(() => acs.UpdateConfigToResetLocalWhiteList(gymId, branchId));
        //    return Ok(response);
        //}

        /// <summary>
        /// Método encargado de validar los datos obligatorios del ingreso.
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("ValidateEntryData/{gymId}/{branchId}/{clientId}")]
        //[ResponseType(typeof(string))]
        //public async Task<IHttpActionResult> ValidateEntryData(int gymId, string branchId, string clientId)
        //{
        //    try
        //    {
        //        string response = string.Empty;
        //        AccessControlSettingsBLL acsBLL = new AccessControlSettingsBLL();
        //        response = await Task.Run(() => acsBLL.ValidateEntryConfigurationData(gymId, branchId, clientId));
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}
    }
}