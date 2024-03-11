using Newtonsoft.Json;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Sibo.WhiteList.Service.DAL.API
{
    public class ClientMessagesAPI
    {
        string apiRoute = "api/ClientMessages/";

        /// <summary>
        /// Método que se comunica con la API (Mensajes cliente) y permite consultar los mensajes que serán mostrados al cliente al momento de ingresar.
        /// Getulio Vargas - 2018-04-03 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public List<eClientMessages> GetClientMessages(int gymId, string branchId)
        {
            try
            {
                List<eClientMessages> responseList = new List<eClientMessages>();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetClientMessages/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<List<eClientMessages>>(result);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<eClientMessages> GetClientMessagesDownload(int gymId, string branchId, string ipTerminal)
        {
            try
            {
                List<eClientMessages> responseList = new List<eClientMessages>();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetClientMessagesDownload/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId+ "/" + ipTerminal.Replace('.', ','));
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<List<eClientMessages>>(result);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GetClientMessagesInserReplicate(int gymId, string branchId, int IDImg, string ipTerminal)
        {
            try
            {
                bool responseList = false;
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetClientMessagesInserReplicate/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId + "/" + IDImg + "/" + ipTerminal);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<bool>(result);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
