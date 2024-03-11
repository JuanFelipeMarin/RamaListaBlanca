using Newtonsoft.Json;
using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.Data;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Sibo.WhiteList.Service.BLL
{
    public class ClientMessagesBLL
    {
        #region Public Methods
        /// <summary>
        /// Método encargado de consultar los mensajes al cliente por medio de la API y enviarlos ya sea a insertar o actualizar en la BD local.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public bool GetClientsMessages(int gymId, string branchId)
        {
            ServiceLog log = new ServiceLog();
            leerJson lg = new leerJson();

            try
            {
                ClientMessagesAPI clientMessagesAPI = new ClientMessagesAPI();
                List<eClientMessages> clientMessagesList = new List<eClientMessages>();
                ClientMessagesBLL cmProcess = new ClientMessagesBLL();
                bool resp = false;
                log.WriteProcess("Consumo de API para descargar los mensajes al cliente.");
                clientMessagesList = clientMessagesAPI.GetClientMessages(gymId, branchId);

                if (clientMessagesList != null && clientMessagesList.Count > 0)
                {
                    log.WriteProcess("Se procede a insertar o actualizar los mensajes al cliente en la BD local.");
                    string jsonString = JsonConvert.SerializeObject(clientMessagesList);
                    resp = lg.DescargaConfiguraciones(jsonString, 5);

                    //resp = cmProcess.InsertOrUpdateClientMessages(clientMessagesList);
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

        /// <summary>
        /// Método donde se verifica que items de la lista de mensajes se debe actualizar y cuales se deben insertar.
        /// Getulio Vargas - 2018-04-09 - OD 1307
        /// </summary>
        /// <param name="clientMessagesList"></param>
        /// <returns></returns>
        public bool InsertOrUpdateClientMessages(List<eClientMessages> clientMessagesList)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dClientMessages cmData = new dClientMessages();
                bool resp = false;

                foreach (eClientMessages item in clientMessagesList)
                {
                    eClientMessages cm = new eClientMessages();
                    cm = cmData.GetClientMessage(item.messageId);

                    if (cm != null)
                    {
                        resp = cmData.InsertOrUpdate(item, "Update");
                    }
                    else
                    {
                        resp = cmData.InsertOrUpdate(item, "Insert");
                    }
                }

                if (resp)
                {
                    log.WriteProcess("Los mensajes se actualizaron o insertaron correctamente en la BD local.");
                }
                else
                {
                    log.WriteProcess("No fue posible actualizar o insertar los mensajes en la BD local.");
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

        public List<eClientMessages> GetLocalCLientMessages()
        {
            dClientMessages cmData = new dClientMessages();
            return cmData.GetClientMessages();
        }

        public DataTable GetMessagesWithoutReplicate(string ipAddress)
        {
            dClientMessages cmData = new dClientMessages();
            return cmData.GetMessagesWithoutReplicate(ipAddress);
        }

        public bool InsertReplicatedImage(int iDImg, string ipAddress)
        {
            dClientMessages cmData = new dClientMessages();
            return cmData.InsertReplicatedImage(iDImg, ipAddress);
        }
        #endregion
    }
}
