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
    public class WhitelistIngresoTouchMonitor
    {
        string apiRoute = "api/WhitelistIngresoTouchMonitor/";

        public string reservasCliente(eReservesClient responseList)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
            string methodName = "ConsultarReservasCliente/";

            string json = string.Empty;
            var dynamicList = new List<dynamic>();
            string res = "0";
            string result = string.Empty;

            try
            {
                //Convertimos la lista que llega como parámetro en una lista dynamic
                //foreach (eReservesClient item in responseList)
                //{
                //    dynamic dyn = item;
                //    dynamicList.Add(dyn);
                //}
                dynamic dyn = responseList;

                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                //webRequest.Timeout = 0;

                json = JsonConvert.SerializeObject(dyn);

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

                    res = JsonConvert.DeserializeObject<string>(result);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return false;
        }

        public string ConsultarReservasClienteDatos(eReservesClient responseList)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
            string methodName = "ConsultarReservasClienteDatos/";

            string json = string.Empty;
            var dynamicList = new List<dynamic>();
            string res = "0";
            string result = string.Empty;

            try
            {
                //Convertimos la lista que llega como parámetro en una lista dynamic
                //foreach (eReservesClient item in responseList)
                //{
                //    dynamic dyn = item;
                //    dynamicList.Add(dyn);
                //}
                dynamic dyn = responseList;

                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                //webRequest.Timeout = 0;

                json = JsonConvert.SerializeObject(dyn);

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

                    res = JsonConvert.DeserializeObject<string>(result);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return false;
        }
        public bool ValidarContratoFirmado(eValidarContrato datos)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
            string methodName = "ValidarContratoFirmado/";

            string json = string.Empty;
            var dynamicList = new List<dynamic>();
            bool res = false;
            string result = string.Empty;

            try
            {

                dynamic dyn = datos;

                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                //webRequest.Timeout = 0;

                json = JsonConvert.SerializeObject(dyn);

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

            //return false;
        }
        public bool ValidarContratoFirmadoPorPlan(eValidarContrato datos)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
            string methodName = "ValidarContratoFirmadoPorPlan/";

            string json = string.Empty;
            var dynamicList = new List<dynamic>();
            bool res = false;
            string result = string.Empty;

            try
            {

                dynamic dyn = datos;

                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                //webRequest.Timeout = 0;

                json = JsonConvert.SerializeObject(dyn);

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

            //return false;
        }

        public bool Cumpleanios(sEntidadCumpleanios responseList)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["urlApi"].ToString();
            string methodName = "ConsultarCumpleaniosCliente/";

            string json = string.Empty;
            var dynamicList = new List<dynamic>();
            bool res = false;
            string result = string.Empty;

            try
            {
                //Convertimos la lista que llega como parámetro en una lista dynamic
                //foreach (eReservesClient item in responseList)
                //{
                //    dynamic dyn = item;
                //    dynamicList.Add(dyn);
                //}
                dynamic dyn = responseList;

                var webRequest = (HttpWebRequest)WebRequest.Create(url + apiRoute + methodName);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                //webRequest.Timeout = 0;

                json = JsonConvert.SerializeObject(dyn);

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

            //return false;
        }

    }
}
