﻿using System;
using System.Collections.Generic;
using Sibo.WhiteList.Service.Entities.Classes;
using Sibo.WhiteList.Service.Data;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.BLL.Log;
using System.Data;
using Sibo.WhiteList.Service.BLL.Helpers;

namespace Sibo.WhiteList.Service.BLL
{
    public class EventBLL
    {
        #region Public Methods
        /// <summary>
        /// Método encargado de consultar las entradas en la BD local e insertarlas por medio de la API en la BD de GSW, luego envía a actualizar el estado de estas en la BD local.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public bool AddEntry(int gymId, int branchId)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                List<eEvent> entryList = new List<eEvent>();
                EventBLL entryProcess = new EventBLL();
                EventAPI entryAPI = new EventAPI();
                eEvent entry = new eEvent();
                bool response = false, aux = false;

                entryList = entryProcess.GetEntries();

                if (entryList != null && entryList.Count > 0)
                {
                    foreach (eEvent item in entryList)
                    {
                        item.gymId = gymId;
                        item.branchId = branchId;
                    }

                    log.WriteProcess("Consumo de API para guardar las entradas, se procesarán " + entryList.Count.ToString() + " registros.");
                    response = entryAPI.AddEntries(entryList);

                    if (response)
                    {
                        log.WriteProcess("Se procede a actualizar las entradas en la BD local.");
                        aux = entryProcess.UpdateState(entryList);
                    }
                }
                else
                {
                    log.WriteProcess("No se encontraron registros de entradas en la BD local para insertar en GSW.");
                }

                return aux;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return false;
            }
        }

        /// <summary>
        /// Método que permite consultar las entradas en la BD local.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <returns></returns>
        private List<eEvent> GetEntries()
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dEvent entryData = new dEvent();
                return entryData.GetEntries();
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        public bool AdicionarEntradasIngresoAdicional(int gymId, int branchId)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                List<eEvent> entryList = new List<eEvent>();
                EventBLL entryProcess = new EventBLL();
                EventAdicionalAPI entryAPI = new EventAdicionalAPI();
                eEvent entry = new eEvent();
                bool response = false, aux = false;

                entryList = entryProcess.GetEntriesAdicional();

                if (entryList != null && entryList.Count > 0)
                {
                    foreach (eEvent item in entryList)
                    {
                        item.gymId = gymId;
                        item.branchId = branchId;
                    }

                    log.WriteProcess("Consumo de API para guardar las entradas adicionales, se procesarán " + entryList.Count.ToString() + " registros.");
                    response = entryAPI.AddEntries(entryList);

                    if (response)
                    {
                        log.WriteProcess("Se procede a actualizar las entradas adicionales en la BD local.");
                        aux = entryProcess.UpdateState(entryList);
                    }
                }
                else
                {
                    log.WriteProcess("No se encontraron registros de entradas en la BD local para insertar en GSW.");
                }

                return aux;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return false;
            }
        }

        private List<eEvent> GetEntriesAdicional()
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dEvent entryData = new dEvent();
                return entryData.GetEntriesAdicional();
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }


        /// <summary>
        /// Método que permite actualizar el estado de las entradas en la BD local.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <param name="entryList"></param>
        /// <returns></returns>
        public bool UpdateState(List<eEvent> entryList)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dEvent entryData = new dEvent();
                bool resp = false;

                foreach (eEvent item in entryList)
                {
                    resp = entryData.UpdateState(item);
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

        public DataTable GetEntriesToShow(string ipAddress, bool blnSirveParaSalida, bool blnSirveParaEntradaySalida)
        {
            dEvent entryData = new dEvent();
            return entryData.GetEntriesToShow(ipAddress, blnSirveParaSalida, blnSirveParaEntradaySalida);
        }

        /// <summary>
        /// Método que permite consultar los eventos registrados en la sucursal para ingreso touch.
        /// Getulio Vargas - 2018-09-04 - OD 1307
        /// </summary>
        /// <returns></returns>
        public DataTable GetEntriesToShowMonitor(bool blnSirveParaSalida, bool blnSirveParaEntradaySalida)
        {
            dEvent entryData = new dEvent();
            return entryData.GetEntriesToShowMonitor(blnSirveParaSalida, blnSirveParaEntradaySalida);
        }

        public eResponse Insert(string entryType, string clientId, string clientName, int planId, int invoiceId, string documentType, 
                                bool discountTicket, int visitId, DateTime? dateLastEntry, DateTime? expirationDate, bool isService,
                                string firstMessage, string planName, string secondMessage, bool successEntry, string ipAddress, string eventType, string thirdMessage, string qr = null, string temperature = null, string usuario = null, string strEmpresaIngresoAdicional = null)
        {
            ServiceLog log = new ServiceLog();
            eResponse resp = new eResponse();
            leerJson lg = new leerJson();
            try
            {
                dEvent entryData = new dEvent();
                eEvent entryEntity = new eEvent();
                bool respBool = false;
                string strLinea = "";

                entryEntity.clientId = clientId; 
                entryEntity.clientName = clientName; 
                entryEntity.dateLastEntry = dateLastEntry.ToString();
                entryEntity.discountTicket = discountTicket;
                entryEntity.documentType = documentType;
                entryEntity.expirationDate = expirationDate.ToString();
                entryEntity.firstMessage = firstMessage;
                entryEntity.invoiceId = invoiceId;
                entryEntity.planId = planId;
                entryEntity.planName = planName;
                entryEntity.secondMessage = secondMessage;
                entryEntity.visitId = visitId;
                entryEntity.successEntry = successEntry;
                entryEntity.ipAddress = ipAddress;
                entryEntity.eventType = eventType;
                entryEntity.thirdMessage = thirdMessage;
                entryEntity.qrCode = qr;
                entryEntity.temperature = temperature;
                entryEntity.usuario = usuario;
                entryEntity.strEmpresaIngresoAdicional = strEmpresaIngresoAdicional;

                if (entryType == clsEnum.EntryType.Entry.ToString())
                {
                    entryEntity.entryDate = DateTime.Now;
                    entryEntity.entryHour = DateTime.Now;
                }
                else if (entryType == clsEnum.EntryType.Exit.ToString())
                {
                    entryEntity.outDate = DateTime.Now;
                    entryEntity.outHour = DateTime.Now;
                }
                else if (entryType == "")
                {
                    entryEntity.entryDate = DateTime.Now;
                    entryEntity.entryHour = DateTime.Now;
                }

                strLinea = "clientId: " + entryEntity.clientId;
                strLinea = strLinea + " clientName: " +entryEntity.clientName + ";";
                strLinea = strLinea + " planId: " + entryEntity.planId + ";";
                strLinea = strLinea + " planName: " + entryEntity.planName + ";";
                strLinea = strLinea + " entryDate: " + entryEntity.entryDate + ";";
                strLinea = strLinea + " entryHour: " + entryEntity.entryHour + ";";
                strLinea = strLinea + " outDate: " + entryEntity.outDate + ";";
                strLinea = strLinea + " outHour: " + entryEntity.outHour + ";";
                strLinea = strLinea + " modifiedDate: " + DateTime.Now + ";";
                strLinea = strLinea + " branchId: " + entryEntity.branchId + ";";
                strLinea = strLinea + " invoiceId: " + entryEntity.invoiceId + ";";
                strLinea = strLinea + " documentType: " + entryEntity.documentType + ";";
                strLinea = strLinea + " discountTicket: " + entryEntity.discountTicket + ";";
                strLinea = strLinea + " gymId: " + entryEntity.gymId + ";";
                strLinea = strLinea + " visitId: " + entryEntity.visitId + ";";
                strLinea = strLinea + " firstMessage: " + entryEntity.firstMessage + ";";
                strLinea = strLinea + " secondMessage: " + entryEntity.secondMessage + ";";
                strLinea = strLinea + " thirdMessage: " + entryEntity.thirdMessage + ";";
                strLinea = strLinea + " expirationDate: " + entryEntity.expirationDate + ";";
                strLinea = strLinea + " dateLastEntry: " + entryEntity.dateLastEntry + ";";
                strLinea = strLinea + " successEntry: " + entryEntity.successEntry + ";";
                strLinea = strLinea + " ipAddress: " + entryEntity.ipAddress + ";";
                strLinea = strLinea + " eventType: " + entryEntity.eventType + ";";
                strLinea = strLinea + " qrCode: " + entryEntity.qrCode + ";";
                strLinea = strLinea + " temperature: " + entryEntity.temperature + ";";
                strLinea = strLinea + " usuario: " + entryEntity.usuario + ";";
                strLinea = strLinea + " strEmpresaIngresoAdicional: " + entryEntity.strEmpresaIngresoAdicional + ";";

                respBool = lg.guardadoDeEntradasTXT(strLinea,1);
                //respBool = entryData.Insert(entryEntity);

                if (respBool)
                {
                    resp.state = true;
                    resp.message = "Ok";
                }
                else
                {
                    resp.state = false;
                    resp.message = "ErrorInsert";
                }

                return resp;
            }
            catch (Exception ex)
            {
                if (isService)
                {
                    log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                    log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                    return null;
                }
                else
                {
                    resp.state = false;
                    resp.message = "Ex";
                    resp.messageOne = ex.Message;
                    return resp;
                }
            }
        }

        public DataTable GetEntriesByIdFamilyGroup(string idList, bool isService)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                DataTable dt = new DataTable();
                dEvent entryData = new dEvent();

                if (!string.IsNullOrEmpty(idList))
                {
                    dt = entryData.GetEntriesByIdFamilyGroup(idList);
                }

                return dt;
            }
            catch (Exception ex)
            {
                if (isService)
                {
                    log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                    log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }

        public DataTable GetEntryByUserWithoutOutput(string userId, int planId, int invoiceId, bool isService)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                DataTable dt = new DataTable();
                dEvent entryData = new dEvent();

                if (!string.IsNullOrEmpty(userId))
                {
                    dt = entryData.GetEntryByUserWithoutOutput(userId, planId, invoiceId);
                }

                return dt;
            }
            catch (Exception ex)
            {
                if (isService)
                {
                    log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                    log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool UpdateExit(string userId, int planId, int invoiceId, bool isService, string thirdMessage)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dEvent entryData = new dEvent();

                if (!string.IsNullOrEmpty(userId))
                {
                    return entryData.Update(userId, planId, invoiceId, DateTime.Now, DateTime.Now, thirdMessage);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (isService)
                {
                    log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                    log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public eEvent GetLastEntryByUserId(string userId)
        {
            ServiceLog log = new ServiceLog();
            dEvent eventData = new dEvent();

            if (string.IsNullOrEmpty(userId))
            {                
                log.WriteProcess("No es posible consultar la información del usuario ya que la identificación de este no se envió correctamente.");
                return null;
            }

            return eventData.GetLastEntryByUserId(userId);
        }

        /// <summary>
        /// Método que permite retornar la última entrada de un usuario conociendo la factura y el plan de esa entrada.
        /// Getulio Vargas - 2018-09-02 - OD 1307
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        public eEvent GetLastEntryByUserIdAndInvoiceIdAndPlanId(string userId, int invoiceId, int planId)
        {
            dEvent eventData = new dEvent();

            if (!string.IsNullOrEmpty(userId) && invoiceId > 0 && planId > 0)
            {
                return eventData.GetLastEntryByUserIdAndInvoiceIdAndPlanId(userId, invoiceId, planId);
            }
            else
            {
                return null;
            }
        }

        public bool InsertTerminalEvents(DataTable dt)
        {
            dEvent eventData = new dEvent();
            bool response = false;

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    response = eventData.InsertTerminalEvents(dt);
                }
            }

            return response;
        }
        #endregion
    }
}
