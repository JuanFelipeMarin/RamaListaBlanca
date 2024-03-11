﻿using Sibo.WhiteList.BLL.Classes;
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
    [RoutePrefix("api/Terminal")]
    public class TerminalController : ApiController
    {
        TerminalBLL termBLL = new TerminalBLL();

        /// <summary>
        /// Método encargado de consulta y retornar la lista de terminales registradas en la BD de GSW.
        /// </summary>
        /// <param name="gymId">Id del gimnasio (cdgimnasio o gymId)</param>
        /// <param name="branchId">Id de la sucursal</param>
        /// <returns>Retorna una lista de terminales en formato entidad.</returns>
        [HttpGet]
        [Route("GetTerminals/{gymId}/{branchId}")]
        [ResponseType(typeof(List<eTerminal>))]
        public async Task<IHttpActionResult> GetTerminals(int gymId, string branchId)
        {
            List<eTerminal> termList = new List<eTerminal>();
            termList = await Task.Run(() => termBLL.GetTerminals(gymId, branchId));
            return Ok(termList);
        }
    }
}