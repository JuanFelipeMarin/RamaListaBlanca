﻿using Newtonsoft.Json;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Sibo.WhiteList.Service.DAL.API
{
    public class AccessControlSettingsAPI
    {
        string apiRoute = "api/AccessControlSettings/";
        
        /// <summary>
        /// Método que se comunica con la API (Configuración de ingreso) y permite consultar la configuración del ingreso.
        /// Getulio Vargas - 2018-04-03 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public List<eConfiguration> GetAccessControlSettings(int gymId, string branchId)
        {
            try
            {
                List<eConfiguration> config = new List<eConfiguration>();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetAccessControlConfiguration/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    config = JsonConvert.DeserializeObject<List<eConfiguration>>(result);
                }

                return config;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite traer desde la API la configuración del parámetro para saber si se limpia o no la lista blanca local
        /// Getulio Vargas - 2018-04-04 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public bool GetConfigToResetLocalWhiteList(int gymId, int branchId)
        {
            try
            {
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetConfigToResetLocalWhiteList/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId);
                var response = webRequest.GetResponse();
                bool resp = false;

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

        /// <summary>
        /// Método que permite actualizar en la BD de GSW el parámetro para saber si se limpia o no la lista blanca local.
        /// es decir, cuando ya se haya depurado la lista blanca local.
        /// Getulio Vargas - 2018-04-04 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public bool UpdateConfigToResetLocalWhiteList(int gymId, int branchId)
        {
            try
            {
                string result = string.Empty;
                bool res = false;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "UpdateConfigToResetLocalWhiteList/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = 0;
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

        public string ValidateEntryData(int gymId, int branchId, string personId)
        {
            try
            {
                string result = string.Empty, strResponse = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "ValidateEntryData/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId + "/" + Convert.ToInt64(personId).ToString());
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = 0;
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }

                    strResponse = JsonConvert.DeserializeObject<string>(result);
                }

                return strResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
