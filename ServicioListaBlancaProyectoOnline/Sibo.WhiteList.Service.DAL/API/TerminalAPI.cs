﻿using System;
using System.Collections.Generic;
using System.Text;
using Sibo.WhiteList.Service.Entities.Classes;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Sibo.WhiteList.Service.DAL.API
{
    public class TerminalAPI
    {
        string apiRoute = "api/Terminal/";

        /// <summary>
        /// Método que se comunica con la API (Terminales) y permite obtener la lista de terminales asociadas a la sucursal.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public List<eTerminal> GetTerminals(int gymId, string branchId)
        {
            try
            {
                List<eTerminal> responseList = new List<eTerminal>();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetTerminals/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<List<eTerminal>>(result);
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