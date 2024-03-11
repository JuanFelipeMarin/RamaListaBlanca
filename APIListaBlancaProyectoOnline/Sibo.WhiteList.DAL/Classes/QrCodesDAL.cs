using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class QrCodesDAL
    {
        /// <summary>
        /// Obtiene el registro de código Qr
        /// MToro
        /// </summary>
        /// <param name="qrCode">código QR</param>
        /// <param name="gymId">gim Id</param>
        /// <returns></returns>
        public gim_codigosQr GetClientByQr(string qrCode, int gymId, bool getUserAlways = false)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                return context.gim_codigosQr.FirstOrDefault(q => q.qr == qrCode
                                                && q.cdgimnasio == gymId
                                                && (q.bitActivo || getUserAlways));
            }

        }

        /// <summary>
        /// Inactiva uyn código QR que ya fue usado
        /// MToro
        /// </summary>
        /// <param name="qrCode">código QR</param>
        /// <param name="gymId">gim Id</param>
        /// <returns></returns>
        public bool InactivateQrCode(string qrCode, int gymId)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                gim_codigosQr code = context.gim_codigosQr.FirstOrDefault(q => q.qr == qrCode
                                                && q.cdgimnasio == gymId);

                if(code != null)
                {
                    code.bitActivo = false;
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }
    }
}
