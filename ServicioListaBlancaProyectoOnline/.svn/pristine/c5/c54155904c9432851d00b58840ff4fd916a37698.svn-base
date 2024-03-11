using System;
using System.Collections.Generic;
using Sibo.WhiteList.Service.Entities.Classes;
using Sibo.WhiteList.Service.Data;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.BLL.Log;
using System.Data;
using Sibo.WhiteList.Service.BLL.Helpers;

namespace Sibo.WhiteList.Service.BLL
{
    public class EntryBLL
    {
        #region Objects
        ServiceLog log = new ServiceLog();
        #endregion

        #region Public Methods
        /// <summary>
        /// Método encargado de condultar las entradas en la BD local e insertarlas por medio de la API en la BD de GSW, luego envía a actualizar el estado de estas en la BD local.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public bool AddEntry(int gymId, int branchId)
        {
            try
            {
                List<eEntries> entryList = new List<eEntries>();
                EntryBLL entryProcess = new EntryBLL();
                EntryAPI entryAPI = new EntryAPI();
                eEntries entry = new eEntries();
                bool response = false, aux = false;

                entryList = entryProcess.GetEntries();

                if (entryList != null && entryList.Count > 0)
                {
                    foreach (eEntries item in entryList)
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
        private List<eEntries> GetEntries()
        {
            try
            {
                dEntry entryData = new dEntry();
                return entryData.GetEntries();
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
        public bool UpdateState(List<eEntries> entryList)
        {
            try
            {
                dEntry entryData = new dEntry();
                bool resp = false;

                foreach (eEntries item in entryList)
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

        public DataTable GetEntriesToShow(string ipAddress)
        {
            try
            {
                dEntry entryData = new dEntry();
                return entryData.GetEntriesToShow(ipAddress);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public eResponse Insert(string entryType, string clientId, string clientName, int planId, int invoiceId, string documentType, 
                                bool discountTicket, int visitId, DateTime? dateLastEntry, DateTime? expirationDate, bool isService,
                                string firstMessage, string planName, string secondMessage, bool successEntry, string ipAddress)
        {
            eResponse resp = new eResponse();

            try
            {
                dEntry entryData = new dEntry();
                eEntries entryEntity = new eEntries();
                bool respBool = false;

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

                respBool = entryData.Insert(entryEntity);

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
            try
            {
                DataTable dt = new DataTable();
                dEntry entryData = new dEntry();

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
            try
            {
                DataTable dt = new DataTable();
                dEntry entryData = new dEntry();

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

        public bool UpdateExit(string userId, int planId, int invoiceId, bool isService)
        {
            try
            {
                dEntry entryData = new dEntry();

                if (!string.IsNullOrEmpty(userId))
                {
                    return entryData.Update(userId, planId, invoiceId, DateTime.Now, DateTime.Now);
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
        #endregion


    }
}
