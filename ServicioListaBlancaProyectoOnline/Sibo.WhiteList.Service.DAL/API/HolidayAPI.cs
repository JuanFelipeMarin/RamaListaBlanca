using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Service.Entities.Classes;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Sibo.WhiteList.Service.DAL.API
{
    public class HolidayAPI
    {
        string apiRoute = "api/Holiday/";

        public List<eHoliday> GetHolidays(int gymId)
        {
            try
            {
                List<eHoliday> responseList = new List<eHoliday>();
                string result = string.Empty;
                string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
                string methodName = "GetHolidays/";
                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName + gymId);
                var response = webRequest.GetResponse();

                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }

                    responseList = JsonConvert.DeserializeObject<List<eHoliday>>(result);
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
