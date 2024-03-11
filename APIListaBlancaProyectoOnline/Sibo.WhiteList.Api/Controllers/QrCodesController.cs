using Sibo.WhiteList.BLL.Classes;
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
    [RoutePrefix("api/QrCodes")]
    public class QrCodesController : ApiController
    {
        [HttpGet]
        [Route("GetClientByQr/{qrCode}/{gymId}/{getUserAlways}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetClientByQr(string qrCode, int gymId, bool getUserAlways)
        {
            try
            {
                QrCodesBLL qrCodesBLL = new QrCodesBLL();
                string userId = string.Empty;
                userId = await Task.Run(() => qrCodesBLL.GetClientByQr(qrCode, gymId, getUserAlways));
                return Ok(userId);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("InactivateQrCode/{qrCode}/{gymId}")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> InactivateQrCode(string qrCode, int gymId)
        {
            try
            {
                QrCodesBLL qrCodesBLL = new QrCodesBLL();
                bool resp = false;
                resp = await Task.Run(() => qrCodesBLL.InactivateQrCode(qrCode, gymId));
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}