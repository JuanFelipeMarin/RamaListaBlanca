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
  public  class ReplicaHuellasAPI
    {
        string apiRoute = "api/Replica/";
        public bool GetActualizarEstadoReplica(int gymId, string id, string ipTerminal)
        {
            try
            {
                bool responseList = false;
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetActualizarEstadoReplica/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + id + "/" + ipTerminal.Replace('.',',') );
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

        public bool GetEliminarReplicaPersona(int gymId, string intIdHuella_Tarjeta, string idPersona)
        {
            try
            {
                bool responseList = false;
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetActualizarEstadoReplica/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + intIdHuella_Tarjeta + "/" + idPersona);
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

        public bool AddReplicaHuellas(eReplicatedFingerprint entryList)
        {
            try
            {
                string result = string.Empty, json = string.Empty;
                bool res = false;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                //Convertimos la lista que llega como parámetro en una lista dynamic
                var dynamicList = new List<dynamic>();

               // foreach (eReplicatedFingerprint item in entryList)
               // {
                    dynamic dyn = entryList;
                    dynamicList.Add(dyn);
               // }

                //Convertimos la lista dynamic en un json
                json = JsonConvert.SerializeObject(dynamicList);
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

                    res = JsonConvert.DeserializeObject<bool>(result);
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
