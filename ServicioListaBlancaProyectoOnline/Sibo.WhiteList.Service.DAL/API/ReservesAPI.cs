using Newtonsoft.Json;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Sibo.WhiteList.Service.DAL.API
{
    public class ReservesAPI
    {
        string apiRoute = "api/Reserves/";

        /// <summary>
        /// Método que se comunica con la API (Lista blanca) y permite actualizar la lista blanca de GSW a partir del listado enviado.
        /// Getulio Vargas - 2018-04-05 - OD 1304
        /// </summary>
        /// <param name="responseList"></param>
        /// <returns></returns>
        public bool UpdateWhiteList(List<eWhiteList> responseList)
        {
            try
            {
                string result = string.Empty, json = string.Empty;
                bool res = false;
                var dynamicList = new List<dynamic>();
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "UpdateWhiteList/";

                //Convertimos la lista que llega como parámetro en una lista dynamic
                foreach (eWhiteList item in responseList)
                {
                    dynamic dyn = item;
                    dynamicList.Add(dyn);
                }

                //Convertimos la lista dynamic en un json
                json = JsonConvert.SerializeObject(dynamicList);

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

                    res = JsonConvert.DeserializeObject<bool>(result);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Actualiza las reservas de un cliente, dado que este ya entro al gimnasio ese día
        /// </summary>
        /// <param name="reserves"></param>
        /// <returns></returns>
        public bool UpdateAsistedClasesAPI(eReservesToUpdate reserves)
        {
            try
            {
                string result = string.Empty, json = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "UpdateAsistedClases/";
                bool res = false;


                //Convertimos la lista dynamic en un json
                json = JsonConvert.SerializeObject(reserves);
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

                    res = JsonConvert.DeserializeObject<bool>(result);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<esDatoReserva> GetListReservaID(int gymId, int branchId, int idreserva)
        {
            try
            {
                List<esDatoReserva> responseList = new List<esDatoReserva>();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetListReservaID/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<List<esDatoReserva>>(result);
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
