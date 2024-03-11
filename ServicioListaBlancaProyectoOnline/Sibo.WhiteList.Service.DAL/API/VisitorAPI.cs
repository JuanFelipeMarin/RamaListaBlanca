using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Service.Entities.Classes;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Sibo.WhiteList.Service.DAL.API
{
    public class VisitorAPI
    {
        string apiRoute = "api/Visitor/";

        /// <summary>
        /// Método que permite conectarse a la API y consultar los datos maestros necesarios para abrir la ventana de visitantes.
        /// Getulio Vargas - 2018-08-29 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public eVisitorData GetVisitorData(int gymId, string visitorId)
        {
            try
            {
                eVisitorData resp = new eVisitorData();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetDataToVisitor/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + visitorId);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    resp = JsonConvert.DeserializeObject<eVisitorData>(result);
                }

                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite insertar un visitante en la BD de GSW por medio de la API.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public eWhiteList InsertVisitor(eVisitor visitor)
        {
            try
            {
                string result = string.Empty, json = string.Empty;
                eWhiteList res = new eWhiteList();
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "InsertVisitor/";
                dynamic dyn = visitor;

                //Convertimos la lista dynamic en un json
                json = JsonConvert.SerializeObject(dyn);

                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }

                    res = JsonConvert.DeserializeObject<eWhiteList>(result);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
