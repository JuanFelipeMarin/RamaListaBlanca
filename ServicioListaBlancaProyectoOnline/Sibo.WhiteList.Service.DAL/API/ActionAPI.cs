using Newtonsoft.Json;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

//using Sibo.WhiteList.Service.BLL.Helpers;

namespace Sibo.WhiteList.Service.DAL.API
{
   public class ActionAPI
    {
        string apiRoute = "api/Action/";

        public List<eAction> GetAction(int gymId, string branchId, string IpAddress)
        {
            try
            {
                List<eAction> responseList = new List<eAction>();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetAction/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + "/"+   gymId + "/"+ branchId + "/"+ IpAddress.Replace('.', ','));
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<List<eAction>>(result);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GetActionUpdate(eAction item)
        {
            try
            {
                bool responseList = false;
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetActionUpdate/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + "/" + item.id + "/" + item.ipAddress.Replace('.', ',') + "/" + item.intIdSucursales+"/"+ item.cdgimnasio);
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

        public int InsertAction(int gymId, string branchId, string IpAddress, string enrolar)
        {
            try
            {
                int responseList = 0;
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "InsertAction/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + "/" + gymId + "/" + branchId + "/" + IpAddress.Replace('.', ',') + "/" + enrolar);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<int>(result);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AddActionParameters(List<eActionParameters> entryList)
        {
            try
            {
                string result = string.Empty, json = string.Empty;
                bool res = false;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                //Convertimos la lista que llega como parámetro en una lista dynamic
                var dynamicList = new List<dynamic>();

                foreach (eActionParameters item in entryList)
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
    }
}
