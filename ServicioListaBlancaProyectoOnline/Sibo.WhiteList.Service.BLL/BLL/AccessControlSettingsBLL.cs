using Newtonsoft.Json;
using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.Data;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;

namespace Sibo.WhiteList.Service.BLL
{
    public class AccessControlSettingsBLL
    {
        #region Public Methods
        /// <summary>
        /// Método que permite consultar la configuración del ingreso en la API e insertar o actualizar la BD local.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public bool GetAccessControlSettings(int gymId, string branchId)
        {
            ServiceLog log = new ServiceLog();
            leerJson lg = new leerJson();

            try
            {
                AccessControlSettingsAPI acsAPI = new AccessControlSettingsAPI();
                List<eConfiguration> config = new List<eConfiguration>();
                bool resp = false;
                log.WriteProcess("Consumo de API para descargar la configuración.");
                config = acsAPI.GetAccessControlSettings(gymId, branchId);

                if (config != null)
                {
                    log.WriteProcess("Se procede a insertar o actualizar la configuración en la BD local.");

                    string jsonString = JsonConvert.SerializeObject(config);
                    resp = lg.DescargaConfiguraciones(jsonString, 1);
                    //resp = SaveOrUpdateConfiguration(config);
                }
                else
                {
                    log.WriteProcess("No se encontró la configuración del ingreso para esta sucursal.");
                }

                return resp;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return false;
            }
        }

        public string ValidateEntryData(int gymId, int branchId, string personId)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                AccessControlSettingsAPI acsAPI = new AccessControlSettingsAPI();
                string response = string.Empty;
                log.WriteProcess("Consumo de API para verificar si la persona con identificación: " + Convert.ToInt64(personId).ToString() + " puede ingresar.");
                response = acsAPI.ValidateEntryData(gymId, branchId, personId);

                return response;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return string.Empty;
            }
        }

        /// <summary>
        /// Método que se encarga de consultar los tiempos que utilizará el servicio durante su proceso de ejecución.
        /// Getulio Vargas - 2018-04-10 - OD 1307
        /// </summary>
        /// <returns></returns>
        public List<int> GetTimers()
        {
            ServiceLog log = new ServiceLog();
            leerJson lg = new leerJson();

            try
            {
                AccessControlSettingsAPI acsAPI = new AccessControlSettingsAPI();
                dAccessControlSettings acsData = new dAccessControlSettings();
                eConfiguration config = new eConfiguration();
                List<eConfiguration> product = new List<eConfiguration>();
                List<int> responseList = new List<int>();
                int timeConfig = 0, timeMsg = 0, timeIUF = 0, timeTerminal = 0;
                string json = lg.cargarArchivos(1);
                //config = acsData.GetConfiguration();

                if(json != "")
                {
                    product = JsonConvert.DeserializeObject<List<eConfiguration>>(json);
                }
                 

                if (product != null && product[0].intfkSucursal != 0)
                {
                    timeConfig = (product[0].timeGetConfiguration == null) ? 0 : product[0].timeGetConfiguration;
                    timeMsg = (product[0].timeGetClientMessages == null) ? 0 : product[0].timeGetClientMessages;
                    timeTerminal = (product[0].timeGetTerminals == null) ? 0 : product[0].timeGetTerminals;
                }

                responseList.Insert(0, timeConfig);
                responseList.Insert(1, timeMsg);
                responseList.Insert(1, timeTerminal);

                return responseList;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        public eConfiguration GetLocalAccessControlSettings()
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dAccessControlSettings acsData = new dAccessControlSettings();
                AccessControlSettingsAPI acsAPI = new AccessControlSettingsAPI();
                return acsData.GetConfiguration();
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Método que permite actualizar o insertar la configuración del ingreso.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private bool SaveOrUpdateConfiguration(eConfiguration config)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dAccessControlSettings acsData = new dAccessControlSettings();
                eConfiguration configEntity = new eConfiguration();
                bool resp = false;

                configEntity = acsData.GetConfiguration();

                if (configEntity != null)
                {
                    resp = acsData.Update(config);

                    if (resp)
                    {
                        log.WriteProcess("La configuración se actualizó correctamente en la BD local.");
                    }
                    else
                    {
                        log.WriteProcess("No fue posible actualizar la configuración en la BD local.");
                    }
                }
                else
                {
                    resp = acsData.Insert(config);

                    if (resp)
                    {
                        log.WriteProcess("La configuración se insertó correctamente en la BD local.");
                    }
                    else
                    {
                        log.WriteProcess("No fue posible insertar la configuración en la BD local.");
                    }
                }

                return resp;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return false;
            }
        } 
        #endregion
    }
}
