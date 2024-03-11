using Newtonsoft.Json;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.DAL.API
{
    public class EmployeeAPI
    {
        string apiRoute = "api/Employee/";

        /// <summary>
        /// Validamos por medio de la API si el empleado puede iniciar sesión; es decir, tiene usuario y contraseña en SiboPaw.
        /// Getulio Vargas - 2018-07-05 - OD 1307
        /// </summary>
        /// <param name="userResponse"></param>
        /// <returns></returns>
        public eResponse ValidateUserEmployee(eUserResponse userResponse)
        {
            try
            {
                string result = string.Empty, json = string.Empty;
                eResponse resp = new eResponse();
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                dynamic dyn = userResponse;

                //Convertimos la lista dynamic en un json
                json = JsonConvert.SerializeObject(dyn);
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute);
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

                    resp = JsonConvert.DeserializeObject<eResponse>(result);
                }

                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
