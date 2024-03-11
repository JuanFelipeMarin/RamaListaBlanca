﻿using Newtonsoft.Json;
using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.DAL.Data;
using Sibo.WhiteList.Service.Data;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Reflection;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.BLL
{
    public class WhiteListBLL
    {
        public string GymId = System.Configuration.ConfigurationManager.AppSettings["gymId"].ToString();
        public string branchId = System.Configuration.ConfigurationManager.AppSettings["branchId"].ToString();

        private dWhiteList obj = new dWhiteList();
        /// <summary>
        /// Método que permite consultar la lista blanca en GSW por medio de la API y enviar a actualizar o insertar en la BD local.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public bool GetWhiteListAsync(int gymId, int branchId)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                WhiteListAPI whiteListAPI = new WhiteListAPI();
                AccessControlSettingsAPI configAPI = new AccessControlSettingsAPI();

                log.WriteProcess("Consumo de API para descargar la lista blanca.");
                List<eWhiteList> whiteList = whiteListAPI.GetWhiteList(gymId, branchId);

                if (whiteList != null && whiteList.Count > 0)
                {
                    log.WriteProcess($"Se procede a guardar o actualizar la lista blanca en la BD local, se procesarán {whiteList.Count} registros.");

                    bool resetLocalWhiteList = configAPI.GetConfigToResetLocalWhiteList(gymId, branchId);
                    List<eWhiteList> responseList = SaveOrUpdateLocalWhiteList(whiteList, resetLocalWhiteList);

                    if (responseList != null && responseList.Count > 0)
                    {
                        foreach (eWhiteList itemEliminar in responseList)
                        {
                            obj.fingerprintReplitDelete(itemEliminar);

                            string[] restriccionesDias = itemEliminar.restrictions.Split('|');

                            foreach (string restriccionDia in restriccionesDias)
                            {
                                string[] restriccionesHoras = restriccionDia.Split(';');

                                foreach (string restriccionHora in restriccionesHoras)
                                {
                                    string[] horas = restriccionHora.Split('-');

                                    if (horas.Length == 2)
                                    {
                                        obj.agregarHorariosRestriccion(horas[0].Replace("SG", "").Replace(" ", ""), horas[1].Replace(" ", ""));
                                    }
                                }
                            }
                        }

                        bool resp = whiteListAPI.UpdateWhiteList(responseList, gymId);
                        configAPI.UpdateConfigToResetLocalWhiteList(gymId, branchId);
                        return resp;
                    }
                }
                else
                {
                    log.WriteProcess("No se encontraron registros de lista blanca en GSW para actualizar o insertar en BD local.");
                }
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError($"{ex.Message} {ex.StackTrace}");
            }

            return false;
        }

        public eResponse ValidateClassSchedule(eWhiteList whiteList, bool isService, eConfiguration config)
        {
            ServiceLog log = new ServiceLog();
            eResponse resp = new eResponse();

            try
            {
                if (config != null)
                {
                    if (config.bitAccesoPorReservaWeb && !config.bitValidarPlanYReservaWeb)
                    {
                        string stringSchedule = whiteList.classSchedule;

                        string startString = stringSchedule.Substring(0, stringSchedule.IndexOf("-"));
                        string endString = stringSchedule.Substring(stringSchedule.IndexOf("-") + 1);

                        string startHour = startString.Substring(0, startString.Length - 4);
                        string endHour = endString.Substring(0, endString.Length - 3);

                        int dateLength = DateTime.Now.Date.ToString().Length - 14;

                        startString = DateTime.Now.Date.ToString("dd/MM/yyyy") + " " + startString.Substring(0, startString.Length - 5);
                        endString = DateTime.Now.Date.ToString("dd/MM/yyyy") + " " + endString.Substring(0, endString.Length - 5);

                        DateTime startDate = DateTime.Parse(startString);
                        DateTime endDate = DateTime.Parse(endString);

                        DateTime currentDate = DateTime.Now;

                        if (startDate != null && endDate != null)
                        {
                            if (((DateTime)whiteList.dateClass).Date == currentDate.Date)
                            {
                                if (currentDate >= startDate && currentDate < endDate)
                                {
                                    resp.state = true;
                                    return resp;
                                }
                                else
                                {
                                    resp.state = false;
                                    resp.message = "OutOfSchedule";
                                    resp.messageOne = "HORARIO DE ENTRADA: \n" + startHour + " - " + endHour;
                                    return resp;
                                }
                            }
                            else
                            {
                                resp.state = false;
                                resp.message = "DifferentDay";
                                resp.messageOne = "Día de clase: " + whiteList.dateClass;
                                return resp;
                            }
                        }
                        else
                        {
                            resp.state = true;
                            return resp;
                        }
                    }
                    else
                    {
                        resp.state = true;
                        return resp;
                    }
                }
                else
                {
                    resp.state = false;
                    resp.message = "NoConfig";
                    return resp;
                }
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

        public async void UpdateAsistedClasesClient(eWhiteList whiteList, bool isService)
        {
            await Task.Run(() =>
            {
                string strGymId = System.Configuration.ConfigurationManager.AppSettings["gymId"].ToString();
                int gymId = 0;
                if (!string.IsNullOrEmpty(strGymId))
                    gymId = Convert.ToInt32(strGymId);

                if (gymId != 0)
                {
                    List<eWhiteList> respList = new List<eWhiteList>();
                    DataTable dt = new DataTable();
                    ReservesAPI reservesAPI = new ReservesAPI();

                    if (!string.IsNullOrEmpty(whiteList.id))
                        dt = obj.GetClientsReservesFromToday(whiteList.id);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        respList = ConvertToEntityWL(dt);
                    }

                    if (respList != null && respList.Count > 0)
                    {
                        List<int> reserveIds = new List<int>();
                        foreach (eWhiteList item in respList)
                            reserveIds.Add(item.reserveId);

                        eReservesToUpdate reserves = new eReservesToUpdate();
                        reserves.reserveIds = reserveIds;
                        reserves.userId = whiteList.id;
                        reserves.gymId = gymId;

                        reservesAPI.UpdateAsistedClasesAPI(reserves);

                    }
                }
            });
        }

        /// <summary>
        /// Método para inactivar el código qr usado
        /// Es asíncrono para que el rendimiento del servicio no se vea afectado
        /// </summary>
        /// <param name="qrCode"></param>
        /// <param name="isService"></param>
        public async void InactivateQrCode(string qrCode, bool isService)
        {
            await Task.Run(() =>
            {
                string strGymId = System.Configuration.ConfigurationManager.AppSettings["gymId"].ToString();
                int gymId = 0;
                if (!string.IsNullOrEmpty(strGymId))
                    gymId = Convert.ToInt32(strGymId);

                if (gymId != 0)
                {
                    QrCodesAPI qrCodesAPI = new QrCodesAPI();

                    qrCodesAPI.InactivateQrCode(qrCode, gymId);
                }
            });
        }

        public string GetQrFromGSW(eWhiteList whiteList, bool isService)
        {
            string qrCode = string.Empty;

            string strGymId = System.Configuration.ConfigurationManager.AppSettings["gymId"].ToString();
            int gymId = 0;
            if (!string.IsNullOrEmpty(strGymId))
                gymId = Convert.ToInt32(strGymId);

            if (gymId != 0)
            {
                List<eWhiteList> respList = new List<eWhiteList>();
                DataTable dt = new DataTable();
                ReservesAPI reservesAPI = new ReservesAPI();

                if (!string.IsNullOrEmpty(whiteList.id))
                    dt = obj.GetClientsReservesFromToday(whiteList.id);

                if (dt != null && dt.Rows.Count > 0)
                {
                    respList = ConvertToEntityWL(dt);
                }

                if (respList != null && respList.Count > 0)
                {
                    List<int> reserveIds = new List<int>();
                    foreach (eWhiteList item in respList)
                        reserveIds.Add(item.reserveId);

                    eReservesToUpdate reserves = new eReservesToUpdate();
                    reserves.reserveIds = reserveIds;
                    reserves.userId = whiteList.id;
                    reserves.gymId = gymId;
                    reservesAPI.UpdateAsistedClasesAPI(reserves);

                }
            }

            return qrCode;
        }

        public DataTable GetWhiteListByUserId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("No es posible consultar si el usuario existe en la base de datos local, la identificación no se envió correctamente.");
            }

            DataTable dt = new DataTable();
            dt = obj.GetUserById(id);
            return dt;
        }

        /// <summary>
        /// Método que permite obtener la lista blanca de la BD local.
        /// Getulio Vargas - 2018-04-03 - OD 1307
        /// </summary>
        /// <returns></returns>
        private List<eWhiteList> GetLocalWhiteList()
        {
            try
            {
                DataTable dt = new DataTable();
                List<eWhiteList> whiteList = new List<eWhiteList>();
                dt = obj.GetWhiteList();

                if (dt != null && dt.Rows.Count > 0)
                {
                    whiteList = ConvertToEntityWL(dt);
                }

                return whiteList;
            }
            catch (Exception ex)
            {
                ServiceLog log = new ServiceLog();
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        /// <summary>
        /// Método encargado de consultar los registros de la lista blanca que tienen huella por actualizar en GSW y retornarlos en una entidad WL.
        /// Getulio Vargas - 2018-08-22 - OD 1307
        /// </summary>
        /// <returns></returns>
        //public List<eWhiteList> GetFingerprintsToUpdate()
        //{
        //    try
        //    {
        //        return ConvertToEntityWL(obj.GetFingerprintsToUpdate());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// Método que se encarga de convertir un DataTable de la tabla tblWhiteList de la BD local en una lista de la entidad "eWhiteList".
        /// Getulio Vargas - 2018-04-04 - OD 1307
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<eWhiteList> ConvertToEntityWL(DataTable dt)
        {
            List<eWhiteList> responseList = new List<eWhiteList>();

            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    eWhiteList wl = new eWhiteList();

                    wl.availableEntries = row.Field<int>("availableEntries");
                    wl.branchId = row.Field<int>("branchId");
                    wl.branchName = row.Field<string>("branchName");
                    wl.cardId = row.Field<string>("cardId");
                    wl.classIntensity = row.Field<string>("classIntensity");
                    wl.className = row.Field<string>("className");
                    wl.classSchedule = row.Field<string>("classSchedule");
                    wl.classState = row.Field<string>("classState");
                    wl.courtesy = row.Field<bool>("courtesy");
                    wl.dateClass = row.Field<DateTime?>("dateClass") ?? DateTime.MinValue;
                    wl.dianId = row.Field<int>("dianId");
                    wl.documentType = row.Field<string>("documentType");
                    wl.employeeName = row.Field<string>("employeeName");
                    wl.fingerprintId = row.Field<int?>("fingerprintId") ?? 0;
                    wl.fingerprint = row.Field<byte[]>("fingerprint");
                    wl.updateFingerprint = row.Field<bool?>("updateFingerprint") ?? false;
                    wl.groupEntriesControl = row.Field<bool>("groupEntriesControl");
                    wl.groupEntriesQuantity = row.Field<int>("groupEntriesQuantity");
                    wl.groupId = row.Field<int>("groupId");
                    wl.gymId = row.Field<int>("gymId");
                    wl.id = row.Field<string>("id");
                    wl.intPkId = row.Field<int>("intPkId");
                    wl.invoiceId = row.Field<int>("invoiceId");
                    wl.isRestrictionClass = row.Field<bool>("isRestrictionClass");
                    wl.know = row.Field<bool>("know");
                    wl.lastEntry = row.Field<DateTime?>("lastEntry") ?? DateTime.MinValue;
                    wl.expirationDate = row.Field<DateTime?>("expirationDate") ?? DateTime.MinValue;
                    wl.name = row.Field<string>("name");
                    wl.personState = row.Field<string>("personState");
                    wl.photoPath = row.Field<string>("photoPath");
                    wl.planId = row.Field<int>("planId");
                    wl.planName = row.Field<string>("planName");
                    wl.planType = row.Field<string>("planType");
                    wl.reserveId = row.Field<int>("reserveId");
                    wl.restrictions = row.Field<string>("restrictions");
                    wl.strDatoFoto = row.Field<string>("strDatoFoto");
                    wl.subgroupId = row.Field<int>("subgroupId");
                    wl.typePerson = row.Field<string>("typePerson");
                    wl.utilizedMegas = row.Field<int>("utilizedMegas");
                    wl.utilizedTickets = row.Field<int>("utilizedTickets");
                    wl.withoutFingerprint = row.Field<bool>("withoutFingerprint");

                    responseList.Add(wl);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                ServiceLog log = new ServiceLog();
                log.WriteProcess("Ocurrio un error al procesar los registros de lista blanca, verifique el log.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        public eWhiteList ConvertToEntity(DataTable dt)
        {
            try
            {
                eWhiteList wl = new eWhiteList();

                if (dt != null)
                {
                    DataRow row = dt.Rows[0];
                    wl.availableEntries = Convert.ToInt32(row["availableEntries"].ToString());
                    wl.branchId = Convert.ToInt32(row["branchId"].ToString());
                    wl.branchName = row["branchName"].ToString();
                    wl.cardId = row["cardId"].ToString();
                    wl.classIntensity = row["classIntensity"].ToString();
                    wl.className = row["className"].ToString();
                    wl.classSchedule = row["classSchedule"].ToString();
                    wl.classState = row["classState"].ToString();
                    wl.courtesy = Convert.ToBoolean(row["courtesy"].ToString());
                    wl.dateClass = Convert.IsDBNull(row["dateClass"]) ? DateTime.MinValue : Convert.ToDateTime(row["dateClass"].ToString());
                    wl.dianId = Convert.ToInt32(row["dianId"].ToString());
                    wl.documentType = row["documentType"].ToString();
                    wl.employeeName = row["employeeName"].ToString();
                    wl.fingerprintId = Convert.IsDBNull(row["fingerprintId"]) ? 0 : Convert.ToInt32(row["fingerprintId"].ToString());
                    try
                    {
                        if (!Convert.IsDBNull(row["fingerprint"]))
                        {
                            wl.fingerprint = (byte[])row["fingerprint"];
                        }
                    }
                    catch (Exception)
                    {

                        wl.fingerprint = null;
                    }


                    wl.updateFingerprint = Convert.ToBoolean(row["updateFingerprint"].ToString());
                    wl.strDatoFoto = (row["strDatoFoto"].ToString());
                    wl.groupEntriesControl = Convert.ToBoolean(row["groupEntriesControl"].ToString());
                    wl.groupEntriesQuantity = Convert.ToInt32(row["groupEntriesQuantity"].ToString());
                    wl.groupId = Convert.ToInt32(row["groupId"].ToString());
                    wl.gymId = Convert.ToInt32(row["gymId"].ToString());
                    wl.id = row["id"].ToString();
                    wl.intPkId = Convert.ToInt32(row["intPkId"].ToString());
                    wl.invoiceId = Convert.ToInt32(row["invoiceId"].ToString());
                    wl.isRestrictionClass = Convert.ToBoolean(row["isRestrictionClass"].ToString());
                    wl.know = Convert.ToBoolean(row["know"].ToString());
                    wl.lastEntry = Convert.IsDBNull(row["lastEntry"]) ? DateTime.MinValue : Convert.ToDateTime(row["lastEntry"].ToString());
                    wl.expirationDate = Convert.IsDBNull(row["expirationDate"]) ? DateTime.MinValue : Convert.ToDateTime(row["expirationDate"].ToString());
                    wl.name = row["name"].ToString();
                    wl.personState = row["personState"].ToString();
                    wl.photoPath = row["photoPath"].ToString();
                    wl.planId = Convert.ToInt32(row["planId"].ToString());
                    wl.planName = row["planName"].ToString();
                    wl.planType = row["planType"].ToString();
                    wl.reserveId = Convert.ToInt32(row["reserveId"].ToString());
                    wl.restrictions = row["restrictions"].ToString();
                    wl.subgroupId = Convert.ToInt32(row["subgroupId"].ToString());
                    wl.typePerson = row["typePerson"].ToString();
                    wl.utilizedMegas = Convert.ToInt32(row["utilizedMegas"].ToString());
                    wl.utilizedTickets = Convert.ToInt32(row["utilizedTickets"].ToString());
                    wl.withoutFingerprint = Convert.ToBoolean(row["withoutFingerprint"].ToString());
                }

                return wl;
            }
            catch (Exception ex)
            {
                ServiceLog log = new ServiceLog();
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        /// <summary>
        /// Método para insertar un registro en la lista blanca local.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="wlEntity"></param>
        /// <returns></returns>
        public bool Insert(eWhiteList wlEntity)
        {
            return obj.Insert(wlEntity);
        }

        /// <summary>
        /// Método encargado de invocar la actualización de la lista blanca de una huella actualizada en GSW.
        /// Getulio Vargas - 2018-08-22 - OD 1307
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="fingerprintId"></param>
        /// <returns></returns>
        //public bool FingerprintUpdated(string personId, int fingerprintId)
        //{
        //    try
        //    {
        //        return obj.FingerprintUpdated(personId, fingerprintId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// Método que se encarga de actualizar la huella en los registros existentes de un usuario en la lista blanca local.
        /// Getulio Vargas - 2018-08-22 - OD 1307
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="fingerId"></param>
        /// <param name="fingerprintImage"></param>
        /// <returns></returns>
        public bool UpdateFingerprint(int gymId, int branchId, string clientId, int fingerId, byte[] fingerprintImage)
        {
            string msg = string.Empty;
            DataTable dt = new DataTable();

            if (string.IsNullOrEmpty(clientId))
            {
                msg = "Identificación del usuario";
            }

            if (fingerId <= 0)
            {
                if (string.IsNullOrEmpty(msg))
                {
                    msg = "Id de la huella";
                }
                else
                {
                    msg = " - Id de la huella";
                }
            }

            if (fingerprintImage == null)
            {
                if (string.IsNullOrEmpty(msg))
                {
                    msg = "Huella";
                }
                else
                {
                    msg = " - Huella";
                }
            }

            if (!string.IsNullOrEmpty(msg))
            {
                throw new Exception("Alguno de los parámetros enviados para actualizar la huella en la lista blanca no está correcto. Los parámetros son: " + msg);
            }
            else
            {
                dt = obj.GetUserById(clientId);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return obj.UpdateFingerprint(clientId, fingerId, fingerprintImage);
                }
                else if (dt == null && dt.Rows.Count == 0)
                {
                    return obj.UpdateFingerprint(clientId, fingerId, fingerprintImage);
                }
                else if (clientId != "" && fingerId > 0)
                {
                    FingerprintBLL fingerprintBll = new FingerprintBLL();

                    return fingerprintBll.ValidateFingerprintToRecord(gymId, branchId, clientId, fingerId, fingerprintImage, 99, false,"");

                }
                else
                {
                    throw new Exception("No es posible actualizar la huella del usuario debido a que no se encuentra registrado en la lista blanca local.");
                }
            }
        }

        /// <summary>
        /// Método que permite actualizar, insertar o eliminar registros en la lista blanca local; además, permite borrar o insertar huellas en las terminales.
        /// Getulio Vargas - 2018-04-05 - Od 1307
        /// </summary>
        /// <param name="whiteList"></param>
        /// <param name="resetLocalWhiteList"></param>
        /// <returns></returns>
        public List<eWhiteList> SaveOrUpdateLocalWhiteList(List<eWhiteList> whiteList, bool resetLocalWhiteList)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                log.WriteProcess("Inicia el proceso de inserción o actualización de lista blanca en BD local.");

                List<eWhiteList> localWhiteList = new List<eWhiteList>();
                FingerprintBLL fpProcess = new FingerprintBLL();
                List<eWhiteList> responseList = new List<eWhiteList>();
                dUser user = new dUser();
                dReplicatedUser replicatedUser = new dReplicatedUser();
                localWhiteList = GetLocalWhiteList();
                bool updateFingerprint = false, resWL = false;
                DataTable dtFingerprint = CreateDataTable<eFingerprintTable>();
                DataTable dtPerson = CreateDataTable<eLocalWhiteList>();
                DataTable dtUsers = CreateDataTable<eUsersTable>();
                DataTable dtCards = CreateDataTable<eClientCard>();
                ClientCardBLL clientCardBLL = new ClientCardBLL();
                string clientsToDeleteCards = string.Empty;

                if (localWhiteList != null && localWhiteList.Count > 0)
                {
                    bool exist = false;
                    foreach (eWhiteList item in whiteList)
                    {
                        eWhiteList compareItem = null;

                        foreach (eWhiteList localItem in localWhiteList)
                        {
                            if (item.id == localItem.id && item.typePerson == localItem.typePerson && item.gymId == localItem.gymId && item.branchId == localItem.branchId &&
                                item.documentType == localItem.documentType && item.invoiceId == localItem.invoiceId && item.dianId == localItem.dianId && item.planId == localItem.planId)
                            {
                                exist = true;
                                compareItem = localItem;
                                break;
                            }
                            else
                            {
                                exist = false;
                            }
                        }

                        DataRow rowFingerprint = dtFingerprint.NewRow();

                        if (exist)
                        {
                            if (item.personState == "Pendiente")
                            {
                                int fingerprintIdToUpdate = 0;

                                if (item.fingerprint == null && compareItem.fingerprint != null)
                                {
                                    rowFingerprint = GetRowFingerprint(compareItem, dtFingerprint, false, true);
                                    dtFingerprint.Rows.Add(rowFingerprint);
                                    fingerprintIdToUpdate = compareItem.fingerprintId ?? 0;
                                }
                                else if (item.fingerprint != null && compareItem.fingerprint != null)
                                {
                                    updateFingerprint = ValidateFingerprint(item.fingerprint, compareItem.fingerprint);

                                    if (updateFingerprint)
                                    {
                                        rowFingerprint = GetRowFingerprint(item, dtFingerprint, true, false);
                                        dtFingerprint.Rows.Add(rowFingerprint);
                                        fingerprintIdToUpdate = item.fingerprintId ?? 0;
                                    }
                                }
                                else if (item.fingerprint != null && compareItem.fingerprint == null)
                                {
                                    rowFingerprint = GetRowFingerprint(item, dtFingerprint, true, false);
                                    dtFingerprint.Rows.Add(rowFingerprint);
                                    fingerprintIdToUpdate = item.fingerprintId ?? 0;
                                }
                                else if (item.fingerprint == null && compareItem.fingerprint == null && item.tblPalmasId != null)
                                {
                                    if (item.tblPalmasId.Count > 2)
                                    {
                                        dtFingerprint = GetRowPalmas(item, dtFingerprint, true, false);
                                    }
                                    
                                }

                                DataRow rowPerson = dtPerson.NewRow();
                                rowPerson = GetRowPerson(item, dtPerson, compareItem.intPkId, false, true, false);
                                dtPerson.Rows.Add(rowPerson);

                                //ACTUALIZAMOS USUARIOS EN LA LISTA DE USUARIOS A REPLICAR A LA TERMINAL
                                if ((compareItem.fingerprintId != item.fingerprintId) || (compareItem.withoutFingerprint != item.withoutFingerprint))
                                {
                                    DataRow rowUser = dtUsers.NewRow();
                                    rowUser = GetRowUser(item.id, item.name, item.fingerprintId ?? 0, item.withoutFingerprint, true, false, dtUsers);
                                    dtUsers.Rows.Add(rowUser);
                                }

                                responseList.Add(item);

                                //Agregamos la identifición del cliente al que se le eliminará las tarjetas y serán agregadas las nuevas.
                                if (item.typePerson == "Cliente")
                                {
                                    if (string.IsNullOrEmpty(clientsToDeleteCards))
                                    {
                                        clientsToDeleteCards = item.id;
                                    }
                                    else
                                    {
                                        clientsToDeleteCards += "," + item.id;
                                    }
                                }

                                //Agregamos las tarjetas del cliente
                                if (item.typePerson == "Cliente" && item.tblCardId != null && item.tblCardId.Count > 0)
                                {
                                    foreach (eClientCard card in item.tblCardId)
                                    {
                                        DataRow rowCard = dtCards.NewRow();
                                        rowCard = GetRowCard(card, dtCards);
                                        dtCards.Rows.Add(rowCard);
                                    }
                                }
                            }
                            else if (item.personState == "Eliminar")
                            {
                                if (compareItem.fingerprint != null)
                                {
                                    rowFingerprint = GetRowFingerprint(compareItem, dtFingerprint, false, true);
                                    dtFingerprint.Rows.Add(rowFingerprint);
                                }

                                DataRow rowPerson = dtPerson.NewRow();
                                rowPerson = GetRowPerson(compareItem, dtPerson, compareItem.intPkId, false, false, true);
                                dtPerson.Rows.Add(rowPerson);
                                //ACTUALIZAMOS USUARIOS EN LA LISTA DE USUARIOS A REPLICAR A LA TERMINAL
                                DataRow rowUser = dtUsers.NewRow();
                                rowUser = GetRowUser(compareItem.id, compareItem.name, compareItem.fingerprintId ?? 0, compareItem.withoutFingerprint, false, true, dtUsers);
                                dtUsers.Rows.Add(rowUser);
                                responseList.Add(item);

                                //Agregamos las identificaciones de los clientes a los que se les eliminará las tarjetas.
                                if (item.typePerson == "Cliente")
                                {
                                    if (string.IsNullOrEmpty(clientsToDeleteCards))
                                    {
                                        clientsToDeleteCards = item.id;
                                    }
                                    else
                                    {
                                        clientsToDeleteCards += "," + item.id;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (item.personState == "Pendiente")
                            {
                                if (item.fingerprint != null)
                                {
                                    rowFingerprint = GetRowFingerprint(item, dtFingerprint, true, false);
                                    dtFingerprint.Rows.Add(rowFingerprint);
                                }

                                DataRow rowPerson = dtPerson.NewRow();
                                rowPerson = GetRowPerson(item, dtPerson, 0, true, false, false);
                                dtPerson.Rows.Add(rowPerson);

                                //INSERTAMOS USUARIOS EN LA LISTA DE USUARIOS A REPLICAR A LA TERMINAL
                                if ((item.fingerprintId != null && item.fingerprintId > 0 && item.fingerprint != null) || (item.fingerprint == null || item.withoutFingerprint))
                                {
                                    DataRow rowUser = dtUsers.NewRow();
                                    rowUser = GetRowUser(item.id, item.name, (item.fingerprintId ?? 0), item.withoutFingerprint,
                                                         true, false, dtUsers);
                                    dtUsers.Rows.Add(rowUser);
                                }

                                //Agregamos las tarjetas de los clientes
                                if (item.typePerson == "Cliente" && item.tblCardId != null && item.tblCardId.Count > 0)
                                {
                                    foreach (eClientCard card in item.tblCardId)
                                    {
                                        DataRow rowCard = dtCards.NewRow();
                                        rowCard = GetRowCard(card, dtCards);
                                        dtCards.Rows.Add(rowCard);
                                    }
                                }
                            }

                            responseList.Add(item);
                        }
                    }

                    if (resetLocalWhiteList)
                    {
                        log.WriteProcess("Inicia el proceso de depuración de la lista blanca local.");

                        foreach (eWhiteList localItem in localWhiteList)
                        {
                            exist = false;
                            DataRow rowFingerprint = dtFingerprint.NewRow();

                            foreach (eWhiteList item in whiteList)
                            {
                                //Validamos si el registro de la lista blanca local existe en la lista blanca nueva, para saber si se elimina o no
                                if (item.id == localItem.id && item.typePerson == localItem.typePerson && item.gymId == localItem.gymId && item.branchId == localItem.branchId &&
                                    item.documentType == localItem.documentType && item.invoiceId == localItem.invoiceId && item.dianId == localItem.dianId && item.planId == localItem.planId)
                                {
                                    exist = true;
                                    break;
                                }
                            }

                            if (!exist)
                            {
                                if (localItem.fingerprint != null)
                                {
                                    rowFingerprint = GetRowFingerprint(localItem, dtFingerprint, false, true);
                                    dtFingerprint.Rows.Add(rowFingerprint);
                                }

                                //ACTUALIZAMOS USUARIOS EN LA LISTA DE USUARIOS A REPLICAR A LA TERMINAL
                                DataRow rowUser = dtUsers.NewRow();
                                rowUser = GetRowUser(localItem.id, localItem.name, localItem.fingerprintId ?? 0, localItem.withoutFingerprint, false, true, dtUsers);
                                dtUsers.Rows.Add(rowUser);
                                DataRow rowPerson = dtPerson.NewRow();
                                rowPerson = GetRowPerson(localItem, dtPerson, localItem.intPkId, false, false, true);
                                dtPerson.Rows.Add(rowPerson);

                                //Agregamos las identificaciones de los clientes a los que se les eliminará las tarjetas.
                                if (localItem.typePerson == "Cliente")
                                {
                                    if (string.IsNullOrEmpty(clientsToDeleteCards))
                                    {
                                        clientsToDeleteCards = localItem.id;
                                    }
                                    else
                                    {
                                        clientsToDeleteCards += "," + localItem.id;
                                    }
                                }
                            }
                        }
                    }

                    if (dtPerson != null && dtPerson.Rows.Count > 0)
                    {
                        obj.InsertOrUpdateWhiteList(dtPerson);
                    }

                    if (dtUsers != null && dtUsers.Rows.Count > 0)
                    {
                        user.InsertOrUpdateUsers(dtUsers);
                    }
                }
                else
                {
                    //Como la lista local está vacía, procedemos a eliminar todas las tarjetas existentes en la BD y así poder insertar las nuevas
                    clientCardBLL.DeleteAll();

                    foreach (eWhiteList item in whiteList)
                    {
                        DataRow rowPerson = dtPerson.NewRow();
                        DataRow rowUser = dtUsers.NewRow();

                        if (item.personState == "Pendiente")
                        {
                            if (item.fingerprint != null)
                            {
                                DataRow rowFingerprint = dtFingerprint.NewRow();

                                rowFingerprint = GetRowFingerprint(item, dtFingerprint, true, false);
                                dtFingerprint.Rows.Add(rowFingerprint);
                            }

                            if (item.fingerprint == null && item.tblPalmasId != null)
                            {                                
                                dtFingerprint = GetRowPalmas(item, dtFingerprint, true, false);
                                
                            }

                            rowPerson = GetRowPerson(item, dtPerson, 0, true, false, false);

                            if ((item.fingerprintId != null && item.fingerprintId > 0 && item.fingerprint != null) || (item.fingerprint == null || item.withoutFingerprint))
                            {
                                rowUser = GetRowUser(item.id, item.name, (item.fingerprintId ?? 0), item.withoutFingerprint, true, false, dtUsers);
                                dtUsers.Rows.Add(rowUser);
                            }

                            dtPerson.Rows.Add(rowPerson);
                            responseList.Add(item);

                            //Agregamos las tarjetas de los clientes
                            if (item.typePerson == "Cliente" && item.tblCardId != null && item.tblCardId.Count > 0)
                            {
                                foreach (eClientCard card in item.tblCardId)
                                {
                                    DataRow rowCard = dtCards.NewRow();
                                    rowCard = GetRowCard(card, dtCards);
                                    dtCards.Rows.Add(rowCard);
                                }
                            }
                        }
                    }

                    if (dtPerson != null && dtPerson.Rows.Count > 0)
                    {
                        resWL = obj.InsertTable(dtPerson);
                    }

                    if (dtUsers != null && dtUsers.Rows.Count > 0)
                    {
                        user.InsertOrUpdateUsers(dtUsers);
                    }
                }

                log.WriteProcess("Finaliza proceso de inserción o actualización de lista blanca en BD local.");

                if (dtFingerprint != null && dtFingerprint.Rows.Count > 0)
                {
                    bool ret = fpProcess.InsertRecord(dtFingerprint);
                }

                //Eliminamos las tarjetas de los usuarios seleccionados para esta actividad
                if (!string.IsNullOrEmpty(clientsToDeleteCards))
                {
                    clientCardBLL.DeleteByClients(clientsToDeleteCards);
                }

                //Se insertan las tarjetas de los clientes en la BD
                if (dtCards != null && dtCards.Rows.Count > 0)
                {
                    //Elimino las tarjetas duplicadas MToro
                    DataTable uniqueCards = clientCardBLL.RemoveDuplicateRows(dtCards, "clientCardId");
                    clientCardBLL.InsertTable(uniqueCards);
                }

                return responseList;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        private DataTable CreateDataTable<T>()
        {
            DataTable dataTable = new DataTable();

            // Obtener las propiedades del tipo genérico T
            PropertyInfo[] properties = typeof(T).GetProperties();

            // Crear las columnas del DataTable
            foreach (PropertyInfo property in properties)
            {
                dataTable.Columns.Add(new DataColumn(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType));
            }

            return dataTable;
        }

        /// <summary>
        /// Método que permite consultar las huellas relacionadas según el id enviado (Con un like).
        /// Getulio Vargas - 2018-09-04 - OD 1307
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetFingerprintsById(string id)
        {
            return obj.GetFingerprintsById(id);
        }

        /// <summary>
        /// Método que permite armar un DataRow para el DataTable dtFingerprint.
        /// Getulio Vargas - 2018-04-05 - OD 1307
        /// </summary>
        /// <param name="localItem"></param>
        /// <param name="dtFingerprint"></param>
        /// <param name="insert"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        private DataRow GetRowFingerprint(eWhiteList localItem, DataTable dtFingerprint, bool insert, bool delete)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                DataRow row = dtFingerprint.NewRow();

                row["personId"] = localItem.id;
                row["fingerprintId"] = localItem.fingerprintId;
                row["fingerprint"] = localItem.fingerprint;
                row["strDatoFoto"] = "";
                row["insert"] = insert;
                row["delete"] = delete;
                row["registerDate"] = DateTime.Now;
                row["intIndiceHuellaActual"] = 0;

                return row;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        private DataTable GetRowPalmas(eWhiteList localItem, DataTable dtFingerprint, bool insert, bool delete)
        {
            ServiceLog log = new ServiceLog();

            try
            {
               

                foreach (var item in localItem.tblPalmasId)
                {
                    DataRow row = dtFingerprint.NewRow();

                    row["personId"] = item.hue_identifi.ToString();
                    row["fingerprintId"] = item.hue_dedo;
                    row["fingerprint"] = item.hue_dato;
                    row["strDatoFoto"] = "";
                    row["insert"] = insert;
                    row["delete"] = delete;
                    row["registerDate"] = DateTime.Now;
                    row["intIndiceHuellaActual"] = item.hue_id.ToString();

                    dtFingerprint.Rows.Add(row);

                }

                return dtFingerprint;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        /// <summary>
        /// Método que permite armar un DataRow para el DataTable dtPerson.
        /// Getulio Vargas - 2018-04-05 - OD 1307
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dtPerson"></param>
        /// <returns></returns>
        private DataRow GetRowPerson(eWhiteList item, DataTable dtPerson, int intPkId, bool insert, bool update, bool delete)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                DataRow row = dtPerson.NewRow();
                row["intPkId"] = intPkId;
                row["id"] = item.id;
                row["name"] = item.name;
                row["planId"] = item.planId;
                row["planName"] = item.planName;
                row["expirationDate"] = (item.expirationDate == null) ? new DateTime(1900, 1, 1) : item.expirationDate;
                row["lastEntry"] = (item.lastEntry == null) ? new DateTime(1900, 1, 1) : item.lastEntry;
                row["planType"] = item.planType;
                row["typePerson"] = item.typePerson;
                row["availableEntries"] = item.availableEntries;
                row["restrictions"] = item.restrictions;
                row["branchId"] = item.branchId;
                row["branchName"] = item.branchName;
                row["gymId"] = item.gymId;
                row["personState"] = item.personState;
                row["withoutFingerprint"] = item.withoutFingerprint;
                row["fingerprintId"] = (item.fingerprintId == null) ? 0 : item.fingerprintId;
                row["fingerprint"] = item.fingerprint;
                row["updateFingerprint"] = item.updateFingerprint;
                row["know"] = item.know;
                row["courtesy"] = item.courtesy;
                row["groupEntriesControl"] = item.groupEntriesControl;
                row["groupEntriesQuantity"] = item.groupEntriesQuantity;
                row["groupId"] = item.groupId;
                row["isRestrictionClass"] = item.isRestrictionClass;
                row["classSchedule"] = item.classSchedule;
                row["dateClass"] = (item.dateClass == null) ? new DateTime(1900, 1, 1) : item.dateClass;
                row["reserveId"] = item.reserveId;
                row["className"] = item.className;
                row["utilizedMegas"] = item.utilizedMegas;
                row["utilizedTickets"] = item.utilizedTickets;
                row["employeeName"] = item.employeeName;
                row["classIntensity"] = item.classIntensity;
                row["classState"] = item.classState;
                row["photoPath"] = item.photoPath;
                row["invoiceId"] = item.invoiceId;
                row["dianId"] = item.dianId;
                row["documentType"] = item.documentType;
                row["subgroupId"] = item.subgroupId;
                row["cardId"] = item.cardId;
                row["strDatoFoto"] = item.strDatoFoto;
                row["ins"] = insert;
                row["upd"] = update;
                row["del"] = delete;



                return row;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        private DataRow GetRowUser(string userId, string userName, int fingerprintId, bool withoutFingerprint,
                                   bool insert, bool delete, DataTable dtUsers)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                DataRow row = dtUsers.NewRow();
                row["userId"] = userId;
                row["userName"] = userName.Length > 20 ? userName.Substring(0, 20) : userName;
                row["fingerprintId"] = fingerprintId;
                row["withoutFingerprint"] = withoutFingerprint;
                row["bitInsert"] = insert;
                row["bitDelete"] = delete;

                return row;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        /// <summary>
        /// Método que se encarga de crear una fila para el DataTable de tarjetas de clientes.
        /// Getulio Vargas - OD 1689 - 2020/01/27
        /// </summary>
        /// <param name="clientCard"></param>
        /// <param name="dtCards"></param>
        /// <returns></returns>
        private DataRow GetRowCard(eClientCard clientCard, DataTable dtCards)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                DataRow row = dtCards.NewRow();
                row["clientCardId"] = clientCard.clientCardId;
                row["clientId"] = clientCard.clientId;
                row["cardCode"] = clientCard.cardCode;
                row["state"] = clientCard.state;
                return row;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        /// <summary>
        /// Método para validar si la huella que descarga la lista blanca de GSW es igual a la almacenada en la lista blanca local.
        /// Getulio Vargas - 2018-04-05 - OD 1307
        /// </summary>
        /// <param name="newFingerprint"></param>
        /// <param name="localFingerprint"></param>
        /// <returns></returns>
        private bool ValidateFingerprint(byte[] newFingerprint, byte[] localFingerprint)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                bool diff = false;

                for (int i = 0; i < newFingerprint.Length; i++)
                {
                    if (newFingerprint[i] != localFingerprint[i])
                    {
                        diff = true;
                        break;
                    }
                }

                if (diff)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return false;
            }
        }

        public eResponseWhiteList ValidateEntryByUserId(string id, bool isService, bool noEsHuella, eConfiguration config, bool esAceptada = false, bool esBiolite = false)
        {
            ServiceLog log = new ServiceLog();
            eResponseWhiteList rWlEntity = new eResponseWhiteList();
            eResponse resp = new eResponse();
            WhiteListAPI wsAPI = new WhiteListAPI();
            EntryAPI entryAPI2 = new EntryAPI();
            List<eEntries> eventEntity = new List<eEntries>();
            try
            {
                List<eWhiteList> respList = new List<eWhiteList>();

                if (!string.IsNullOrEmpty(id))
                {


                    //DataTable dt = new DataTable();
                    //if (config.bitValidarPlanYReservaWeb == true || (config.bitValidarPlanYReservaWeb == false && config.bitAccesoPorReservaWeb == false))
                    //{
                    //    dt = obj.GetUserByIdSinReservas(id);
                    //}
                    //else
                    //{
                    //    dt = obj.GetUserById(id);
                    //}

                    respList = wsAPI.GetListPersonaConsultar(Convert.ToInt32(GymId), branchId, id, true);
                    //if (dt != null && dt.Rows.Count > 0)
                    if (respList != null && respList.Count > 0)
                    {
                       // respList = ConvertToEntityWL(dt);

                        if (respList != null && respList.Count >= 1)
                        {
                            foreach (eWhiteList item in respList)
                            {
                                if ((item.withoutFingerprint && noEsHuella) || !noEsHuella && (esBiolite == true && esAceptada == true) || (esBiolite == false && esAceptada == false))
                                {
                                    //VALIDACION: SI EL REGISTRO PERTENECE A UN PLAN TIPO TIQUETERA SE DEBEN VALIDAR LA CANTIDA DE TIQUETES
                                    if (item.planType == "T" && item.availableEntries <= 0)
                                    {
                                        resp.state = false;
                                        resp.message = "NoAvailableEntries";
                                        rWlEntity.response = resp;
                                        return rWlEntity;
                                    }

                                    if (item.typePerson == "Cliente" || item.typePerson == "Empleado")
                                    {
                                        //VALIDACION: TIEMPO DE REINGRESO DE LA PERSONA DADOS LOS MINUTOS DE REINGRESO
                                        if (item.lastEntry != null && !string.IsNullOrEmpty(item.lastEntry.ToString()) && config.intMinutosNoReingreso > 0)
                                        {
                                            if (config.intMinutosNoReingreso > (DateTime.Now - Convert.ToDateTime(item.lastEntry)).Minutes)
                                            {
                                                resp.state = false;
                                                resp.message = "NoReEntry";
                                                rWlEntity.response = resp;
                                                return rWlEntity;
                                            }
                                        }
                                        else if (item.lastEntry != null && !string.IsNullOrEmpty(item.lastEntry.ToString()) && config.intMinutosNoReingresoDia > 0)
                                        {
                                            if (config.intMinutosNoReingresoDia > (DateTime.Now - Convert.ToDateTime(item.lastEntry)).Minutes && (DateTime.Now.Day == Convert.ToDateTime(item.lastEntry).Day) )
                                            {
                                                resp.state = false;
                                                resp.message = "NoReEntry";
                                                rWlEntity.response = resp;
                                                return rWlEntity;
                                            }
                                        }

                                        //se confirma que QA indican que la validacion para imprimir se debe de realizar cuando el check este activo independiente de la configuracion
                                        if (config.bitImprimirHoraReserva && item.isRestrictionClass == true/* && config.bitValidarPlanYReservaWeb == true*/)
                                        {
                                            if (item.reserveId != 0 )
                                            {
                                                List<esDatoReserva> listaReserva = new List<esDatoReserva>();
                                                ReservesAPI apiReservas = new ReservesAPI();
                                                listaReserva = apiReservas.GetListReservaID(item.gymId, item.branchId, item.reserveId);

                                                CargaDatosReservas(listaReserva);
                                                //CargaDatosReservas(item);
                                            }
                                            else
                                            {
                                                log.WriteProcess("La persona " + item.id + " no cuenta con reserva.");

                                            }

                                        }

                                        resp.state = true;
                                        resp.message = "Ok";
                                        rWlEntity.response = resp;
                                        rWlEntity.whiteList = item;
                                        return rWlEntity;
                                    }

                                    if (item.typePerson == "Prospecto" || item.typePerson == "Visitante")
                                    {
                                        if (item.planId == 0)
                                        {
                                            //VALIDACION: SI EL REGISTRO PERTENECE A UN VISITANTE O PROSPECTO Y NO TIENE ENTRADAS DISPONIBLES
                                            if (item.availableEntries <= 0)
                                            {
                                                resp.state = false;
                                                resp.message = "NoAvailableEntries";
                                                rWlEntity.response = resp;
                                                return rWlEntity;
                                            }

                                            //DataTable dtIngresos = new DataTable();
                                            //dtIngresos = obj.GetIngresosVisitantesTblEvents(item.id);
                                           
                                            eventEntity = entryAPI2.GetListEntradasUsuarios(Convert.ToInt32(GymId), branchId, id ,true);
                
                                            if (eventEntity.Count > 0)
                                            {
                                                if (eventEntity[0].cantidadREgistrosEntrada >= item.availableEntries)
                                                {
                                                    resp.state = false;
                                                    resp.message = "IngresosMaxTerminadosVisitantes";
                                                    rWlEntity.response = resp;
                                                    return rWlEntity;
                                                }
                                                else
                                                {
                                                    resp.state = true;
                                                    resp.message = "Ok";
                                                    rWlEntity.response = resp;
                                                    rWlEntity.whiteList = item;

                                                }
                                            }
                                            else
                                            {
                                                resp.state = true;
                                                resp.message = "Ok";
                                                rWlEntity.response = resp;
                                                rWlEntity.whiteList = item;
                                                return rWlEntity;
                                            }
                                        }

                                        resp.state = true;
                                        resp.message = "Ok";
                                        rWlEntity.response = resp;
                                        rWlEntity.whiteList = item;
                                        return rWlEntity;
                                    }
                                }
                                else
                                {
                                    resp.state = false;
                                    resp.message = "NoEntryWithoutFingerprint";
                                    rWlEntity.response = resp;
                                    return rWlEntity;
                                }
                            }

                            resp.state = false;
                            resp.message = "NoWhiteList";
                            rWlEntity.response = resp;
                            return rWlEntity;
                        }
                        else
                        {
                            resp.state = false;
                            resp.message = "NoWhiteList";
                            rWlEntity.response = resp;
                            return rWlEntity;
                        }
                    }
                    else
                    {
                        resp.state = false;
                        resp.message = "NoWhiteList";
                        rWlEntity.response = resp;
                        return rWlEntity;
                    }
                }
                else
                {
                    resp.state = false;
                    resp.message = "NoId";
                    rWlEntity.response = resp;
                    return rWlEntity;
                }
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
                    rWlEntity.response = resp;
                    return rWlEntity;
                }
            }
        }

        string strNombreGimnasio = "MI GYM";
        private PrintDocument printDoc;
        private string intReserva = "000002020";
        private string dbvalorGenerado = "321123";
        private string strClienteReserva = "Cliente Orlando";
        private string strNombreClaReserva = "Clase Tinton";
        private string intCantMegas = "0009";
        private string strFechaReserva = "";
        private string strHoraReserva = "";
        private string strProfesor = "";
        private string strUbicacion = "De primero y ultimo";
        private object intCodigoSucursalActual = "";


        public int CargaDatosReservas(List<esDatoReserva> lstDatosReserva)
        {
            string fechaClase = Convert.ToDateTime(lstDatosReserva[0].strFechaReserva).ToString("dd/MM/yyyy");
            string horaClase = Convert.ToDateTime(lstDatosReserva[0].strHoraReserva).ToString("hh:mm:ss");

            printDoc = new System.Drawing.Printing.PrintDocument();
            printDoc.PrinterSettings.PrinterName = System.Configuration.ConfigurationManager.AppSettings["NombreImpresora"].ToString();

            intReserva = lstDatosReserva[0].intReserva.ToString();
            dbvalorGenerado = lstDatosReserva[0].dbvalorGenerado.ToString();
            strClienteReserva = lstDatosReserva[0].strClienteReserva.ToString();
            strNombreClaReserva = lstDatosReserva[0].strNombreClaReserva.ToString();
            intCantMegas = lstDatosReserva[0].intCantMegas.ToString();
            strFechaReserva = fechaClase;
            strHoraReserva = horaClase;
            strProfesor = lstDatosReserva[0].strProfesor;
            strUbicacion = lstDatosReserva[0].strUbicacion.ToString();

            printDoc.PrintPage += ImprimirReservas;
            printDoc.Print();
            return 0;

            //string fechaClase = Convert.ToDateTime(lstDatosReserva.dateClass).ToString("dd/MM/yyyy");
            //string horaClase = Convert.ToDateTime(lstDatosReserva.dateClass).ToString("hh:mm:ss");

            //printDoc = new System.Drawing.Printing.PrintDocument();
            //printDoc.PrinterSettings.PrinterName = System.Configuration.ConfigurationManager.AppSettings["NombreImpresora"].ToString();

            //intReserva = lstDatosReserva.reserveId.ToString();
            //dbvalorGenerado = lstDatosReserva.id.ToString();
            //strClienteReserva = lstDatosReserva.name.ToString();
            //strNombreClaReserva = lstDatosReserva.className.ToString();
            //intCantMegas = lstDatosReserva.utilizedMegas.ToString();
            //strFechaReserva = fechaClase;
            //strHoraReserva = horaClase;
            //strProfesor = lstDatosReserva.employeeName;
            //strUbicacion = lstDatosReserva.className.ToString();

            //printDoc.PrintPage += ImprimirReservas;
            //printDoc.Print();
            //return 0;
        }

        private void ImprimirReservas(object sender, PrintPageEventArgs e)
        {
            float xPos;
            float yPos;
            Font prFontEmcabezado = new Font("Arial", 14, FontStyle.Bold);
            Font prFontTiquetes = new Font("Arial", 9, FontStyle.Bold);
            Font prFontDatos = new Font("Calibri (Cuerpo)", 9, FontStyle.Regular);
            Font prFontCodigoBarras = new Font("EAN-13", 48, FontStyle.Regular);

            strNombreGimnasio = "";// System.Convert.ToString(IIf(clsConfiguration.GetValue("NombreSucursalActual", "Gimnasio:") == string.Empty, "Gimnasio:", clsConfiguration.GetValue("NombreSucursalActual", "Gimnasio:")));
            // Posición del encabezado y tipo de  letra 
            // imprimimos la cadena en el margen izquierdo
            xPos = 1;
            // la posición superior
            yPos = prFontEmcabezado.GetHeight(e.Graphics);
            // imprimimos la cadena
            e.Graphics.DrawString(strNombreGimnasio, prFontEmcabezado, Brushes.Black, xPos, yPos);

            xPos = 1;
            // la posición superior
            yPos += 25;
            // imprimimos la cadena
            e.Graphics.DrawString("TIQUETE DE RESERVA", prFontTiquetes, Brushes.Black, xPos, yPos);

            // Nro de la reserva 
            xPos = 1;
            yPos += 22;
            e.Graphics.DrawString("No. reserva: ", prFontDatos, Brushes.Black, xPos, yPos);
            e.Graphics.DrawString(intReserva, prFontDatos, Brushes.Black, xPos + 74, yPos);

            // Cédula del Cliente reserva 
            xPos = 1;
            yPos += 14;
            e.Graphics.DrawString("ID Cliente: ", prFontDatos, Brushes.Black, xPos, yPos);
            e.Graphics.DrawString(dbvalorGenerado, prFontDatos, Brushes.Black, xPos + 74, yPos);

            // Nombre del cliente que reserva
            xPos = 1;
            yPos += 14;
            e.Graphics.DrawString("Nombre: ", prFontDatos, Brushes.Black, xPos, yPos);
            e.Graphics.DrawString(strClienteReserva, prFontDatos, Brushes.Black, xPos + 74, yPos);

            // Nombre de la Clase  
            xPos = 1;
            yPos += 14;
            e.Graphics.DrawString("Clase: ", prFontDatos, Brushes.Black, xPos, yPos);
            e.Graphics.DrawString(strNombreClaReserva, prFontDatos, Brushes.Black, xPos + 74, yPos);

            // Cantidad megas de la clase  
            xPos = 1;
            yPos += 14;
            e.Graphics.DrawString("Megas: ", prFontDatos, Brushes.Black, xPos, yPos);
            e.Graphics.DrawString(intCantMegas, prFontDatos, Brushes.Black, xPos + 74, yPos);

            // Fecha de la clase  
            xPos = 1;
            yPos += 14;
            e.Graphics.DrawString("Fecha: ", prFontDatos, Brushes.Black, xPos, yPos);
            e.Graphics.DrawString(strFechaReserva, prFontDatos, Brushes.Black, xPos + 74, yPos);

            // Hora de la clase  
            xPos = 1;
            yPos += 14;
            e.Graphics.DrawString("Hora: ", prFontDatos, Brushes.Black, xPos, yPos);
            e.Graphics.DrawString(strHoraReserva, prFontDatos, Brushes.Black, xPos + 74, yPos);

            // Profesor de la clase  
            xPos = 1;
            yPos += 14;
            e.Graphics.DrawString("Profesor: ", prFontDatos, Brushes.Black, xPos, yPos);
            e.Graphics.DrawString(strProfesor, prFontDatos, Brushes.Black, xPos + 74, yPos);

            // Ubicacion  
            xPos = 1;
            yPos += 14;
            e.Graphics.DrawString("Ubicación: ", prFontDatos, Brushes.Black, xPos, yPos);
            e.Graphics.DrawString(strUbicacion, prFontDatos, Brushes.Black, xPos + 74, yPos);

            //objAccesoDatos.UpEstadoImpresion(intReserva, intCodigoSucursalActual);

            // xPos = 1
            // yPos += 25
            // e.Graphics.DrawString(intReserva, prFontCodigoBarras, Brushes.Black, xPos + 74, yPos)



            // indicamos que ya no hay nada más que imprimir
            // (el valor predeterminado de esta propiedad es False)
            e.HasMorePages = false;
        }


        /// <summary>
        /// Método que permite validar si un usuario puede salir del gimnasio.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isService"></param>
        /// <returns></returns>
        public eResponseWhiteList ValidateExitByUserId(string id, bool isService, bool isId)
        {
            ServiceLog log = new ServiceLog();
            leerJson lg = new leerJson();
            eResponseWhiteList rWlEntity = new eResponseWhiteList();
            eResponse resp = new eResponse();

            try
            {
                DataTable dt = new DataTable();
                List<eWhiteList> respList = new List<eWhiteList>();
                List<eConfiguration> config = new List<eConfiguration>();
                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                EventBLL entryBll = new EventBLL();
                bool entryEmployeeWithoutPlan = false, validateEntryInOutput = false;

                //config = acsBll.GetLocalAccessControlSettings();

                string json = lg.cargarArchivos(1);
                if (json != "")
                {
                    config = JsonConvert.DeserializeObject<List<eConfiguration>>(json);
                }

                if (config != null)
                {
                    entryEmployeeWithoutPlan = config[0].bitIngresoEmpSinPlan;
                    validateEntryInOutput = !config[0].bitNo_Validar_Entrada_En_Salida;
                }

                if (!string.IsNullOrEmpty(id))
                {
                    dt = obj.GetUserById(id);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        respList = ConvertToEntityWL(dt);

                        if (respList != null && respList.Count > 0)
                        {
                            foreach (eWhiteList item in respList)
                            {
                                if ((item.withoutFingerprint && isId) || !isId)
                                {
                                    if (validateEntryInOutput)
                                    {
                                        dt = entryBll.GetEntryByUserWithoutOutput(id, item.planId, item.invoiceId, isService);

                                        if (dt != null && dt.Rows.Count > 0)
                                        {
                                            resp.state = true;
                                            resp.message = "Ok";
                                            rWlEntity.response = resp;
                                            rWlEntity.whiteList = item;
                                            break;
                                        }
                                        else
                                        {
                                            resp.state = false;
                                            resp.message = "NoEntry";
                                            rWlEntity.response = resp;
                                        }
                                    }
                                    else
                                    {
                                        resp.state = true;
                                        resp.message = "Ok";
                                        rWlEntity.response = resp;
                                        rWlEntity.whiteList = item;
                                        break;
                                    }
                                }
                                else
                                {
                                    resp.state = false;
                                    resp.message = "NoExitWithoutFingerprint";
                                    rWlEntity.response = resp;
                                    rWlEntity.whiteList = item;
                                    return rWlEntity;
                                }
                            }

                            return rWlEntity;
                        }
                        else
                        {
                            resp.state = false;
                            resp.message = "NoEntityWL";
                            rWlEntity.response = resp;
                            return rWlEntity;
                        }
                    }
                    else
                    {
                        resp.state = false;
                        resp.message = "NoWhiteList";
                        rWlEntity.response = resp;
                        return rWlEntity;
                    }
                }
                else
                {
                    resp.state = false;
                    resp.message = "NoId";
                    rWlEntity.response = resp;
                    return rWlEntity;
                }
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
                    rWlEntity.response = resp;
                    return rWlEntity;
                }
            }
        }

        /// <summary>
        /// Método para validar el ingreso de un usuario respecto a los ingresos del grupo familiar asociado.
        /// Getulio Vargas - 2018-07-09 - OD 1307
        /// </summary>
        /// <param name="whiteList"></param>
        /// <param name="isService"></param>
        /// <returns></returns>
        public eResponse ValidateEntriesFamilyGroup(eWhiteList whiteList, bool isService)
        {
            ServiceLog log = new ServiceLog();
            eResponse resp = new eResponse();

            try
            {
                DataTable dtUsersFamilyGroup = new DataTable();
                DataTable dtEntries = new DataTable();
                EventBLL entryBll = new EventBLL();
                string idList = string.Empty;
                WhiteListAPI ws = new WhiteListAPI();
                string contadorAPI = "";

                if (whiteList != null)
                {
                    if (whiteList.groupId > 0)
                    {
                        if (whiteList.groupEntriesControl)
                        {
                            ///se cambia por consulta a la api unificando la busqueda del grupo familiar y la cantidad de registros por los participantes del grupo
                            contadorAPI = ws.GetCantidadPersonasGrupoFamiliar(whiteList.gymId, whiteList.branchId.ToString(), whiteList.groupId.ToString());
                            
                            //dtUsersFamilyGroup = GetUsersFamilyGroup(whiteList.groupId, isService);

                            //if (dtUsersFamilyGroup != null && dtUsersFamilyGroup.Rows.Count > 0)
                            //{
                            //    foreach (DataRow row in dtUsersFamilyGroup.Rows)
                            //    {
                            //        if (string.IsNullOrEmpty(idList))
                            //        {
                            //            idList = "''" + row["id"].ToString() + "''";
                            //        }
                            //        else
                            //        {
                            //            idList += ", ''" + row["id"].ToString() + "''";
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    idList = "''" + whiteList.id + "''";
                            //}

                            //dtEntries = entryBll.GetEntriesByIdFamilyGroup(idList, isService);

                            if (contadorAPI != null)
                            {
                                if (whiteList.groupEntriesQuantity < Convert.ToInt32(contadorAPI))
                                {
                                    resp.state = false;
                                    resp.message = "NoEntryByRestriction";
                                }
                                else
                                {
                                    resp.state = true;
                                    resp.message = "Ok";
                                }
                            }
                            else
                            {
                                resp.state = true;
                                resp.message = "Ok";
                            }
                        }
                        else
                        {
                            resp.state = true;
                            resp.message = "NoValidateEntries";
                        }
                    }
                    else
                    {
                        resp.state = true;
                        resp.message = "Ok";
                    }
                }
                else
                {
                    resp.state = false;
                    resp.message = "NoWL";
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

        /// <summary>
        /// Método que permite consultar los usuarios asociados a un grupo familiar específico.
        /// Getulio Vargas - 2018-07-11 - OD 1307
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groupId"></param>
        /// <param name="isService"></param>
        /// <returns></returns>
        private DataTable GetUsersFamilyGroup(int groupId, bool isService)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                DataTable dt = new DataTable();

                if (groupId > 0)
                {
                    dt = obj.GetUsersFamilyGroup(groupId);
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

        /// <summary>
        /// Método principal para realizar las validaciones de restricciones horarias.
        /// Getulio Vargas - 2018-07-11 - OD 1307
        /// </summary>
        /// <param name="whiteList"></param>
        /// <param name="isService"></param>
        /// <returns></returns>
        public eResponse ValidateRestrictions(eWhiteList whiteList, bool isService)
        {
            ServiceLog log = new ServiceLog();
            eResponse resp = new eResponse();

            try
            {
                string[] restrictions;

                if (whiteList != null)
                {
                    if (!string.IsNullOrEmpty(whiteList.restrictions))
                    {
                        restrictions = whiteList.restrictions.ToString().Trim().Split('|');

                        if (restrictions.Length > 0)
                        {
                            return ValidateDays(restrictions, isService);
                        }
                        else
                        {
                            resp.state = true;
                            resp.message = "Ok";
                        }
                    }
                    else
                    {
                        resp.state = true;
                        resp.message = "Ok";
                    }
                }
                else
                {
                    resp.state = false;
                    resp.message = "NoWL";
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

        /// <summary>
        /// Método que permite realizar la validación de restricciones por día, de acuerdo con un array de rangos horarios.
        /// Getulio Vargas - 2018-07-11 - OD 1307
        /// </summary>
        /// <param name="restrictions"></param>
        /// <param name="isService"></param>
        /// <returns></returns>
        private eResponse ValidateDays(string[] restrictions, bool isService)
        {
            ServiceLog log = new ServiceLog();
            eResponse resp = new eResponse();

            try
            {
                int dayOfWeek = 0;
                dayOfWeek = Convert.ToInt32(DateTime.Today.DayOfWeek);

                //Hacer validación de festivos
                if (ValidateHoliday(DateTime.Today, isService))
                {
                    resp = ValidateHour(restrictions[8].ToString().Trim(), isService);
                }
                else
                {
                    switch (dayOfWeek)
                    {
                        case 0:
                            if (!string.IsNullOrEmpty(restrictions[6]))
                            {
                                resp = ValidateHour(restrictions[6].ToString().Trim(), isService);
                            }
                            else
                            {
                                resp.state = true;
                                resp.message = "Ok";
                            }
                            break;
                        case 1:
                            if (!string.IsNullOrEmpty(restrictions[0]))
                            {
                                resp = ValidateHour(restrictions[0].ToString().Trim(), isService);
                            }
                            else
                            {
                                resp.state = true;
                                resp.message = "Ok";
                            }
                            break;
                        case 2:
                            if (!string.IsNullOrEmpty(restrictions[1]))
                            {
                                resp = ValidateHour(restrictions[1].ToString().Trim(), isService);
                            }
                            else
                            {
                                resp.state = true;
                                resp.message = "Ok";
                            }
                            break;
                        case 3:
                            if (!string.IsNullOrEmpty(restrictions[2]))
                            {
                                resp = ValidateHour(restrictions[2].ToString().Trim(), isService);
                            }
                            else
                            {
                                resp.state = true;
                                resp.message = "Ok";
                            }
                            break;
                        case 4:
                            if (!string.IsNullOrEmpty(restrictions[3]))
                            {
                                resp = ValidateHour(restrictions[3].ToString().Trim(), isService);
                            }
                            else
                            {
                                resp.state = true;
                                resp.message = "Ok";
                            }
                            break;
                        case 5:
                            if (!string.IsNullOrEmpty(restrictions[4]))
                            {
                                resp = ValidateHour(restrictions[4].ToString().Trim(), isService);
                            }
                            else
                            {
                                resp.state = true;
                                resp.message = "Ok";
                            }
                            break;
                        case 6:
                            if (!string.IsNullOrEmpty(restrictions[5]))
                            {
                                resp = ValidateHour(restrictions[5].ToString().Trim(), isService);
                            }
                            else
                            {
                                resp.state = true;
                                resp.message = "Ok";
                            }
                            break;
                        case 7:
                            if (!string.IsNullOrEmpty(restrictions[6]))
                            {
                                resp = ValidateHour(restrictions[6].ToString().Trim(), isService);
                            }
                            else
                            {
                                resp.state = true;
                                resp.message = "Ok";
                            }
                            break;
                    }
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

        private bool ValidateHoliday(DateTime today, bool isService)
        {
            ServiceLog log = new ServiceLog();
            leerJson lg = new leerJson();
            try
            {
                List<eHoliday> holidayList = new List<eHoliday>();
                HolidayBLL holidayBll = new HolidayBLL();
                bool resp = false;
                //holidayList = holidayBll.GetLocalHolidays(isService);
                //config = acsData.GetConfiguration();

                string json = lg.cargarArchivos(4);
                if (json != "")
                {
                    holidayList = JsonConvert.DeserializeObject<List<eHoliday>>(json);
                }
                
                if (holidayList != null && holidayList.Count > 0)
                {
                    foreach (eHoliday item in holidayList)
                    {
                        if (today.Date == item.date.Date)
                        {
                            resp = true;
                            break;
                        }
                    }
                }
                else
                {
                    resp = false;
                }

                return resp;
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

        /// <summary>
        /// Método para validar las horas en las que puede ingresar un usuario con un plan específico y/o un subgrupo específico.
        /// Getulio Vargas - 2018-07-11 - OD 1307
        /// </summary>
        /// <param name="schedules"></param>
        /// <param name="isService"></param>
        /// <returns></returns>
        private eResponse ValidateHour(string schedules, bool isService)
        {
            ServiceLog log = new ServiceLog();
            eResponse resp = new eResponse();

            try
            {
                string[] scheduleList, scheduleDetailHours;
                List<string> scheduleDetailPlan = new List<string>();
                List<string> scheduleDetailSG = new List<string>();
                string initialHour = string.Empty, finalHour = string.Empty, actualHour = string.Empty;
                TimeSpan iHour, fHour, aHour;
                bool canEntryPlan = false, canEntrySG = false;

                if (schedules.Length > 0)
                {
                    scheduleList = schedules.ToString().Trim().Split(';');

                    if (scheduleList.Length > 0)
                    {
                        foreach (string item in scheduleList)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (item.Trim().Substring(0, 2) == "SG")
                                {
                                    string horas = item.Substring(3, item.Length - 3).Trim();
                                    scheduleDetailSG.Add(horas);
                                }
                                else
                                {
                                    scheduleDetailPlan.Add(item.Trim());
                                }
                            }
                        }

                        //Validamos Plan
                        if (scheduleDetailPlan.Count > 0)
                        {
                            foreach (string item in scheduleDetailPlan)
                            {
                                scheduleDetailHours = item.ToString().Trim().Split('-');

                                if (scheduleDetailHours.Length > 0)
                                {
                                    initialHour = scheduleDetailHours[0].ToString().Trim();
                                    finalHour = scheduleDetailHours[1].ToString().Trim();

                                    if (!string.IsNullOrEmpty(initialHour) && !string.IsNullOrEmpty(finalHour))
                                    {
                                        actualHour = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString().Trim();
                                        iHour = TimeSpan.Parse(initialHour.Substring(0, initialHour.Length - 2));
                                        fHour = TimeSpan.Parse(finalHour.Substring(0, initialHour.Length - 2));
                                        aHour = TimeSpan.Parse(actualHour);

                                        if (aHour >= iHour && aHour <= fHour)
                                        {
                                            canEntryPlan = true;
                                            break;
                                        }
                                        else
                                        {
                                            canEntryPlan = false;
                                        }
                                    }
                                    else
                                    {
                                        canEntryPlan = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            canEntryPlan = true;
                        }

                        //Validamos SubGrupo
                        if (scheduleDetailSG.Count > 0)
                        {
                            foreach (string item in scheduleDetailSG)
                            {
                                scheduleDetailHours = item.ToString().Trim().Split('-');

                                if (scheduleDetailHours.Length > 0)
                                {
                                    initialHour = scheduleDetailHours[0].ToString().Trim();
                                    finalHour = scheduleDetailHours[1].ToString().Trim();

                                    if (!string.IsNullOrEmpty(initialHour) && !string.IsNullOrEmpty(finalHour))
                                    {
                                        actualHour = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString().Trim();
                                        iHour = TimeSpan.Parse(initialHour.Substring(0, initialHour.Length - 2));
                                        fHour = TimeSpan.Parse(finalHour.Substring(0, initialHour.Length - 2));
                                        aHour = TimeSpan.Parse(actualHour);

                                        if (aHour >= iHour && aHour <= fHour)
                                        {
                                            canEntrySG = true;
                                            break;
                                        }
                                        else
                                        {
                                            canEntrySG = false;
                                        }
                                    }
                                    else
                                    {
                                        canEntrySG = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            canEntrySG = true;
                        }

                        if (canEntryPlan && canEntrySG)
                        {
                            resp.state = true;
                            resp.message = "Ok";
                        }
                        else
                        {
                            resp.state = false;
                            resp.message = "NoEntryByRestriction";
                        }
                    }
                    else
                    {
                        resp.state = true;
                        resp.message = "Ok";
                    }
                }
                else
                {
                    resp.state = true;
                    resp.message = "Ok";
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

        public DataTable GetClientWhiteListByCardId(string cardId)
        {
            if (string.IsNullOrEmpty(cardId))
            {
                throw new Exception("El código de la tarjeta no se envió correctamente. Por lo tanto, no es posible encontrar el id del cliente.");
            }

            return obj.GetClientWhiteListByCardId(cardId);
        }

        public DataTable GetEmployeeWhiteListByCardId(string cardId)
        {
            if (string.IsNullOrEmpty(cardId))
            {
                throw new Exception("El código de la tarjeta no se envió correctamente. Por lo tanto, no es posible encontrar el id del empleado.");
            }

            return obj.GetEmployeeWhiteListByCardId(cardId);
        }

        public DataTable GetClientIdByFingerprintId(int id)
        {
            if (id <= 0)
            {
                throw new Exception("El código de la huella no se envió correctamente. Por lo tanto, no es posible encontrar el id del usuario.");
            }

            return obj.GetClientIdByFingerprintId(id);
        }

        public DataTable GetClientIdBioentry(string id)
        {
            return obj.GetClientIdBioentry(id);
        }

        /// <summary>
        /// Método que permite actualizar los tiquetes disponibles de un usuario en la lista blanca.
        /// GEtulio Vargas - 2018-09-02 - OD 1307
        /// </summary>
        /// <param name="whiteListId"></param>
        /// <returns></returns>
        public bool DiscountTicket(int idNumeroFactura, string idPersona)
        {
            if (idNumeroFactura > 0)
                return obj.DiscountTicket(idNumeroFactura, idPersona);
            else
                return false;
        }
        public DataTable DiscountTicketWeb(int idNumeroFactura, string idPersona)
        {
            if (idNumeroFactura > 0)
                return obj.DiscountTicketWeb(idNumeroFactura, idPersona);
            else
                return null;
        }

        /// <summary>
        /// Obtiene la identificación del usuario a través del código QR
        /// MToro
        /// </summary>
        /// <param name="qrCode">código QR</param>
        /// <param name="gymId">gim Id</param>
        /// <param name="getUserAlways">Obtiene el usuario siempre, esto se usa cuando la temperatura es anormal para traerlo y reportarlo</param>
        /// <returns></returns>
        public string GetClientByQr(string qrCode, int gymId, bool getUserAlways = false)
        {
            QrCodesAPI qrCodesAPI = new QrCodesAPI();
            return qrCodesAPI.GetClientByQr(qrCode, gymId, getUserAlways);
        }

        /// Inserta el evento cuando no se pudo ingresar (o salir)
        /// debido a que la temperatura del usuario marcó mas de la establecida como límite
        /// </summary>
        public void InsertDeniedEventByHightTemperature(string id, string qr, string temperature, bool isService, string ipAddress)
        {
            DataTable dt = new DataTable();
            dt = obj.GetUserById(id);
            List<eWhiteList> respList = new List<eWhiteList>();
            EventBLL entryBll = new EventBLL();
            if (dt != null && dt.Rows.Count > 0)
            {
                respList = ConvertToEntityWL(dt);
                if (respList.Count > 0)
                {
                    eWhiteList user = respList[0];
                    entryBll.Insert("Entry", id, user.name, user.planId, user.invoiceId, user.documentType, false, 0, null, null, isService,
                        "No puede ingresar - Temperatura anormal: " + temperature + "°", user.planName, "", false, ipAddress, "Entry", "HightTemp", qr, temperature);
                }
                else
                {
                    entryBll.Insert("Entry", id, "", 0, 0, "", false, 0, null, null, isService,
                        "No puede ingresar - Temperatura anormal: " + temperature + "°", "", "", false, ipAddress, "Entry", "HightTemp", qr, temperature);
                }
            }
            else
            {
                entryBll.Insert("Entry", id, "", 0, 0, "", false, 0, null, null, isService,
                    "No puede ingresar - Temperatura anormal: " + temperature + "°", "", "", false, ipAddress, "Entry", "HightTemp", qr, temperature);
            }

        }

        public bool CumpleAniosCliente(string id)
        {
            WhitelistIngresoTouchMonitor ApiValidacionBll = new WhitelistIngresoTouchMonitor();
            int gym = Convert.ToInt32(ConfigurationManager.AppSettings["gymId"].ToString());
            sEntidadCumpleanios entidad = new sEntidadCumpleanios();
            entidad.id = id;
            entidad.gymId = gym;
            return ApiValidacionBll.Cumpleanios(entidad); ;

        }

        public bool UpdateFingerprintID(byte[] fingerprint, int fingerprintId, string personId)
        {
            return obj.UpdateFingerprintID(fingerprint, fingerprintId, personId);
        }

        public DataTable ConsultarZonas(string ids)
        {
            return obj.ConsultarZonas(ids);
        }

        public DataTable ConsultarHorariosRestriccion()
        {
            return obj.ConsultarHorariosRestriccion();
        }

        public string ConsultarHorariosRestriccionPorDiaHora(string horaEntrada, string horaSalida)
        {
            return obj.ConsultarHorariosRestriccionPorDiaHora(horaEntrada, horaSalida);
        }

        public string RemoverListaContratos(string id)
        {
            return obj.RemoverListaContratos(id);
        }

        public bool RemoverListaPersonas(string id)
        {
            return obj.RemoverListaPersonas(id);
        }

        public DataTable obtenerIdVisitanteEliminar(string id)
        {
            return obj.obtenerIdVisitanteEliminar(id);
        }

        public DataTable bllReplicaUsuariosBioLiteRestriccion()
        {
            return obj.dReplicaUsuariosBioLiteRestriccion();
        }

        public bool agregarHorariosRestriccion(string horaEntrada, string horaSalida)
        {
            return obj.agregarHorariosRestriccion(horaEntrada, horaSalida);
        }

        public bool fingerprintReplitDeleteA(eWhiteList itemEliminar)
        {
            return obj.fingerprintReplitDelete(itemEliminar);
        }

        public DataTable obtenerPlanesPersona(int id)
        {
            return obj.obtenerPlanesPersona(id);
        }
    }
}