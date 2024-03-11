﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sibo.WhiteList.Service.Entities.Classes;

namespace Sibo.WhiteList.Service.DAL.API
{
 public   class ZonaAPI
    {
        string apiRoute = "api/Zonas/";
        public List<eZonas> GetZonasList(int gymId, string branchId)
        {
            try
            {
                List<eZonas> responseList = new List<eZonas>();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetZonas/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId + "/" + branchId);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<List<eZonas>>(result);
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