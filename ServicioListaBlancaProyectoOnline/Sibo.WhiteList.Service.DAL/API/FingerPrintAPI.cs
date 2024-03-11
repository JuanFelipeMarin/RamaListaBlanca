﻿using Newtonsoft.Json;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Sibo.WhiteList.Service.DAL.API
{
    public class FingerPrintAPI
    {
        string apiRoute = "api/Fingerprint/";

        /// <summary>
        /// Método que se comunica con la API (Huellas) y permite validar si una persona puede o no grabar la huella.
        /// Getulio Vargas - 2018-04-03 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public eResponse ValidatePerson(int gymId, int branchId, string personId)
        {
            try
            {
                string result = string.Empty;
                eResponse res = new eResponse();
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "ValidatePersonToSaveFingerprint/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId + "/" + personId);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    res = JsonConvert.DeserializeObject<eResponse>(result);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que se comunica con la API (Huellas) y permite descargar la huella de un cliente en otra sucursal.
        /// Getulio Vargas - 2018-04-03 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public eWhiteList DownloadFingerprint(int gymId, int branchId, string personId)
        {
            try
            {
                string result = string.Empty;
                eWhiteList res = new eWhiteList();
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "DownloadFingerprintToLocalWhiteList/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId + "/" + personId);
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

                    res = JsonConvert.DeserializeObject<eWhiteList>(result);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que se comunica con la API (Huellas) y permite grabar la huella de un cliente en la BD de GSW.
        /// Getulio Vargas - 2018-04-03 - OD 1307
        /// </summary>
        /// <param name="fingerList"></param>
        /// <returns></returns>
        //public List<eFingerprint> AddFingerprint(List<eFingerprint> fingerList)
        //{
        //    try
        //    {
        //        string result = string.Empty, json = string.Empty;
        //        List<eFingerprint> res = new List<eFingerprint>();
        //        string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
        //        //Convertimos la lista que llega como parámetro en una lista dynamic
        //        var dynamicList = new List<dynamic>();

        //        foreach (eFingerprint item in fingerList)
        //        {
        //            dynamic dyn = item;
        //            dynamicList.Add(dyn);
        //        }

        //        //Convertimos la lista dynamic en un json
        //        json = JsonConvert.SerializeObject(dynamicList);
        //        var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute);
        //        webRequest.Method = "POST";
        //        webRequest.ContentType = "application/json";

        //        using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
        //        {
        //            streamWriter.Write(json);
        //            streamWriter.Flush();
        //            streamWriter.Close();
        //        }

        //        var response = webRequest.GetResponse();

        //        if (response != null)
        //        {
        //            using (var reader = new StreamReader(response.GetResponseStream()))
        //            {
        //                result = reader.ReadToEnd();
        //            }

        //            res = JsonConvert.DeserializeObject<List<eFingerprint>>(result);
        //        }

        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public eResponse ValidateFingerprintToRecord(eFingerprint fingerEntity)
        {
            try
            {
                string result = string.Empty, json = string.Empty;
                eResponse res = new eResponse();
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "ValidateAndSaveFingerprint/";
                dynamic dyn = fingerEntity;

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

                    res = JsonConvert.DeserializeObject<eResponse>(result);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ValidateFingerprint(eRequestFingerprint requestFingerprint, int timeout)
        {
            string result = string.Empty, json = string.Empty, strResponse = string.Empty;
            string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
            string methodName = "ValidateFingerprint/";
            dynamic dyn = requestFingerprint;

            //Convertimos la lista dynamic en un json
            json = JsonConvert.SerializeObject(dyn);
            var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.Timeout = timeout;

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

                strResponse = JsonConvert.DeserializeObject<string>(result);
            }

            return strResponse;
        }
        
        public eSendClientContract SaveSignedContract(eSaveSignedContract essc)
        {
            try
            {
                string result = string.Empty, json = string.Empty;
                eSendClientContract res = new eSendClientContract();
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "SaveSignedContract/";
                dynamic dyn = essc;

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

                    res = JsonConvert.DeserializeObject<eSendClientContract>(result);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public eResponse firmarContratoEnrolarGSW(eSaveSignedContract fingerEntity)
        {
            try
            {
                string result = string.Empty, json = string.Empty;
                eResponse res = new eResponse();
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "SaveSignedContract/";
                dynamic dyn = fingerEntity;
                //string newApiRoute = "api/Contratos/";
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

                    res = JsonConvert.DeserializeObject<eResponse>(result);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<eFingerprint> GetAllFingerprintsPerson(int gymId, string id)
        {
            try
            {
                List<eFingerprint> responseList = new List<eFingerprint>();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetAllFingerprintsPerson/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + id );
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<List<eFingerprint>>(result);
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
