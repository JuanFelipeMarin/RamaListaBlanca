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
    public class PlanAPI
    {
        private static string apiRoute = "api/Plan/";
        public static List<ePlan> GetPlansByIdUSer(int id,int idEmpresa)
        {
            try
            {
                List<ePlan> responseList = new List<ePlan>();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "getByIdUser/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + id + "/" + idEmpresa);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<List<ePlan>>(result);
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
