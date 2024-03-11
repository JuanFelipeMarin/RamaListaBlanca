using Sibo.WhiteList.BLL.Classes;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;

namespace Sibo.WhiteList.Api.Controllers
{
    [System.Web.Http.RoutePrefix("api/Contratos")]
    public class contratosController : ApiController
    {
        // GET: contratos
        //public ActionResult Index()
        //{
        //    return View();
        //}
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("FirmarContratoGSW")]
        [ResponseType(typeof(eResponse))]
        public async Task<IHttpActionResult> FirmarContratoGSW(eContratos contrato)
        {
            try
            {
                eResponse response = new eResponse();
                SupremaBLL suprema = new SupremaBLL();
                response = await Task.Run(() => suprema.firmarContratoGSW(contrato));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}