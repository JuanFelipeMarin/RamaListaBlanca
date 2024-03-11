using Sibo.WhiteList.BLL.Classes;
using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Sibo.WhiteList.Api.Controllers
{
    [RoutePrefix("api/Plan")]
    public class PlanController : ApiController
    {
        private tblPlanBLL obj = new tblPlanBLL();

        [Route("getByIdUser/{id}/{idEmpresa}")]
        [ResponseType(typeof(List<spGetPlanesByIdUser_Result>))]
        [HttpGet]
        public List<spGetPlanesByIdUser_Result> getByIdUser(int id, int idEmpresa)
        {
            try
            {   
                List<spGetPlanesByIdUser_Result> res = new List<spGetPlanesByIdUser_Result>();
                res = obj.getPlanByIdUser(idEmpresa, id);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
