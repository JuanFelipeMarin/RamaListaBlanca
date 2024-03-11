using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.DAL.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.BLL.Classes
{
    public class QrCodesBLL
    {
        /// <summary>
        /// Obtiene la identificación del usuario a través del código QR
        /// MToro
        /// </summary>
        /// <param name="qrCode">código QR</param>
        /// <param name="gymId">gim Id</param>
        /// <returns></returns>
        public string GetClientByQr(string qrCode, int gymId, bool getUserAlways = false)
        {
            try
            {
                string userId = string.Empty;
                QrCodesDAL qrCodesDAL = new QrCodesDAL();
                gim_codigosQr qrCodeInfo = new gim_codigosQr();

                gim_codigosQr qrRegister = qrCodesDAL.GetClientByQr(qrCode, gymId, getUserAlways);


                if(qrRegister != null && !string.IsNullOrEmpty(qrRegister.identifi))
                {
                    return qrRegister.identifi;
                }

                return userId;
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    return string.Empty;
                }
                else
                {
                    throw new Exception(Exceptions.serverError);
                }
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
            try
            {
                bool resp = false;
                QrCodesDAL qrCodesDAL = new QrCodesDAL();

                resp = qrCodesDAL.InactivateQrCode(qrCode, gymId);

                return resp;
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    return false;
                }
                else
                {
                    throw new Exception(Exceptions.serverError);
                }
            }
        }
    }
}
