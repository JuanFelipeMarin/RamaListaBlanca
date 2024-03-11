﻿using Newtonsoft.Json;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.DAL.API
{
    public class WhiteListAPI
    {
        string apiRoute = "api/WhiteList/";
        
        /// <summary>
        /// Método que se comunica con la API (Lista blanca) y permite obtener la lista blanca de una sucursal.
        /// Getulio Vargas - 2018-04-03 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public List<eWhiteList> GetWhiteList(int gymId, int branchId)
        {
            try
            {
                List<eWhiteList> responseList = new List<eWhiteList>();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetWhiteList/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<List<eWhiteList>>(result);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que se comunica con la API (Lista blanca) y permite actualizar la lista blanca de GSW a partir del listado enviado.
        /// Getulio Vargas - 2018-04-05 - OD 1304
        /// </summary>
        /// <param name="responseList"></param>
        /// <returns></returns>
        public bool UpdateWhiteList(List<eWhiteList> responseList, int idGimnasio)
        {
            try
            {
                string result = string.Empty, json = string.Empty;
                string url = ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "UpdateWhiteList/";

                List<int> idMarcados = new List<int>();
                foreach (eWhiteList item in responseList)
                {
                    idMarcados.Add(item.intPkId);
                }

                var dataSend = new
                {
                    entities = idMarcados,
                    idEmpresa = idGimnasio
                };

                //Convertimos la lista dynamic en un json
                json = JsonConvert.SerializeObject(dataSend);

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

                    return JsonConvert.DeserializeObject<bool>(result);
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetListPlanesSucursalPorPlan(int gymId, int branchId, int planId)
        {
            try
            {
                string responseList = "";
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetListPlanesSucursalPorPlan/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId +"/"+ planId);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<string>(result);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetZonasAcceso(int idEmpresa, int idSucursal, int idPlan, int idReserva)
        {
            try
            {
                string responseList = "";
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetZonasAcceso/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + idEmpresa + "/" + idSucursal + "/" + idPlan + "/" + idReserva);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<string>(result);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GetActualizarCantidadTiquetes(int id, int invoiceId, int dianId,string documentType, int availableEntries ,int gymId,string branchId, int planId)
        {
            try
            {
                bool responseList = false;
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetActualizarCantidadTiquetes/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + id + "/" + invoiceId + "/" + dianId + "/" + documentType.Replace('í', 'i') + "/" + availableEntries + "/" + gymId + "/" + branchId + "/" + planId);
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

        public string GetListSucursalPorClase(int gymId, int branchId, int cdreserva)
        {
            try
            {
                string responseList = "";
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetListSucursalPorClase/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId + "/" + cdreserva);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<string>(result);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<eWhiteList> GetListPersonaConsultar(int gymId, string branchId, string id , bool option = false)
        {
            try
            {
                List<eWhiteList> responseList = new List<eWhiteList>();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetListPersonaConsultar/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId + "/" + id + "/" + option);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<List<eWhiteList>>(result);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetCantidadPersonasGrupoFamiliar(int gymId, string branchId, string idGrupo)
        {
            try
            {
                string responseList = "";
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetCantidadPersonasGrupoFamiliar/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId + "/" + idGrupo );
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<string>(result);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetConsultarEliminacionContratos(int gymId, string branchId, string id)
        {
            try
            {
                string responseList = "";
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetConsultarEliminacionContratos/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId + "/" + id);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<string>(result);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GetRespuestaIngresosVisitantes(int gymId, string id)
        {
            try
            {
                bool responseList = false;
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetRespuestaIngresosVisitantes/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + id );
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