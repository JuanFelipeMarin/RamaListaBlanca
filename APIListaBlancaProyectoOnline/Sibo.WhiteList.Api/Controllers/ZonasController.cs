﻿using Sibo.WhiteList.BLL.Classes;
using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace Sibo.WhiteList.Api.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    //[Authorize]
    [RoutePrefix("api/Zonas")]

    public class ZonasController : ApiController
    {
        //ZonasBLL termBLL = new ZonasBLL();

        [HttpGet]
        [Route("GetZonas/{gymId}/{branchId}")]
        //[ResponseType(typeof(List<eZonas>))]
        public IHttpActionResult GetZonas(int gymId , string branchId)
        {
            try
            {
                ZonasBLL zonasbll = new ZonasBLL();

                List<tbl_Maestro_Zonas> list = new List<tbl_Maestro_Zonas>();

                list = zonasbll.GetZonas(gymId, branchId);

                return Ok(list);



            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}