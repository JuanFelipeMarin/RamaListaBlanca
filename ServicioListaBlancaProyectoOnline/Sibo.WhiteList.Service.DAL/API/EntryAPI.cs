﻿using Newtonsoft.Json;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Sibo.WhiteList.Service.DAL.API
{
    public class EntryAPI
    {
        string apiRoute = "api/Entry/";

        /// <summary>
        /// Método que se comunica con la API (Entradas) y permite insertar los ingresos al gimnasio de los clientes.
        /// Getulio Vargas - 2018-04-03 - OD 1307
        /// </summary>
        /// <param name="entryList"></param>
        /// <returns></returns>
        public bool AddEntries(List<eEntries> entryList)
        {
            try
            {
                string result = string.Empty, json = string.Empty;
                bool res = false;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                //Convertimos la lista que llega como parámetro en una lista dynamic
                var dynamicList = new List<dynamic>();
                
                foreach (eEntries item in entryList)
                {
                    dynamic dyn = item;
                    dynamicList.Add(dyn);
                }

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

        public List<eEntries> GetListEntradasUsuarios(int gymId, string branchId, string id, bool TipoPlan = false)
        {
            try
            {
                List<eEntries> responseList = new List<eEntries>();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetListEntradasUsuarios/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId + "/" + id + "/" + TipoPlan);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<List<eEntries>>(result);
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