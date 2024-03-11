using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.DAL.API
{
    public class QrCodesAPI
    {
        string apiRoute = "api/QrCodes/";

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
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetClientByQr/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + qrCode + "/" + gymId + "/" + getUserAlways);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    userId = JsonConvert.DeserializeObject<string>(result);
                }

                return userId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }//fin method

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
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "InactivateQrCode/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + qrCode + "/" + gymId);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    resp = JsonConvert.DeserializeObject<bool>(result);
                }

                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }//fin class
}
