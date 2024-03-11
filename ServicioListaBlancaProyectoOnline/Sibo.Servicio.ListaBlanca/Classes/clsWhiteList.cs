﻿using Newtonsoft.Json;
using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.BLL.Helpers;
using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Sibo.Servicio.ListaBlanca.Classes
{
    public class clsWhiteList
    {
        private string _qrCode = null;
        private string _temperature = null;
        private clsEnum.ServiceActions actionService = clsEnum.ServiceActions.WaitingClientId;
        public clsWhiteList()
        {
        }
        public clsWhiteList(string qrCode, string temperature)
        {
            _qrCode = qrCode;
            _temperature = temperature;
        }

        public bool GetWhiteList(int gymId, int branchId)
        {
            try
            {
                WhiteListBLL wlBLL = new WhiteListBLL();
                return wlBLL.GetWhiteListAsync(gymId, branchId);
            }
            catch (Exception ex)
            {
                ServiceLog log = new ServiceLog();
                log.WriteProcess("Ocurrió un error en el proceso de consulta de la lista blanca");
                log.WriteError($"{ex.Message} - {ex.StackTrace}");
                return false;
            }
        }


        public bool UpdateFingerprint(int gymid, int idbranch, string clientId, int fingerId, byte[] fingerprintImage, string ipAddress)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                WhiteListBLL wlBll = new WhiteListBLL();
                return wlBll.UpdateFingerprint(gymid, idbranch, clientId, fingerId, fingerprintImage);
            }
            catch (Exception ex)
            {
                log.WriteProcessByTerminal("Ocurrió un error en el proceso de actualización de huella del usuario " + clientId + ", huella " + fingerId, ipAddress);
                log.WriteErrorsByTerminals(ex.Message + " Terminal " + ipAddress + " - " + ex.StackTrace, ipAddress);
                return false;
            }
        }

        public string ValidateEntryByUserId(string userId, string ipAddress, bool noEsHuella, eConfiguration config, bool esAceptada = false, bool esBiolite = false)
        {
            ServiceLog log = new ServiceLog();
            string id = Convert.ToInt64(userId).ToString();

            try
            {
                WhiteListBLL wlBll = new WhiteListBLL();
                eWhiteList whiteListEntity = new eWhiteList();
                EventBLL entryBll = new EventBLL();
                eResponseWhiteList rWlEntity = new eResponseWhiteList();
                eResponse response = new eResponse();
                clsTerminal terminal = new clsTerminal();
                WhiteListAPI api = new WhiteListAPI();
                string msg = string.Empty;
                bool discountTicket = false;

                rWlEntity = wlBll.ValidateEntryByUserId(id, false, noEsHuella, config, esAceptada, esBiolite);

                if (rWlEntity.response.state)
                {
                    int idEmpresa = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["gymId"]);
                    int idNumeroFactura = 0;
                    idNumeroFactura = rWlEntity.whiteList.intPkId;

                   //Validar entradas grupo familiar

                    response = wlBll.ValidateEntriesFamilyGroup(rWlEntity.whiteList, false);

                    if (response.state)
                    {
                        //Validar restricciones horarias
                        response = wlBll.ValidateRestrictions(rWlEntity.whiteList, false);
                        if (response.state)
                        {
                            if (!string.IsNullOrEmpty(rWlEntity.whiteList.className)
                                && !string.IsNullOrEmpty(rWlEntity.whiteList.classSchedule)
                                && rWlEntity.whiteList.dateClass != null)
                            {
                             
                                /////////////////////Pendiente revisar con QA
                                //FingerprintBLL fingerprintBLL = new FingerprintBLL();
                                //fingerprintBLL.GetCabiarEstadoElimarHuellas(rWlEntity.whiteList.id, ipAddress);

                                //WhiteListBLL whiteListBLL = new WhiteListBLL();
                                //whiteListBLL.fingerprintReplitDeleteA(rWlEntity.whiteList);
                            }

                            //validar si esta dentro del rango de tiempo establecido
                            if (response.state)
                            {
                                //Validar minutos no re-ingreso
                                string enableDays = string.Empty, entryType = string.Empty;

                                //validamos si se debe descontar tiquetes al usuario en esta entrada
                               // discountTicket = ValidateDiscountTicket(rWlEntity.whiteList, ipAddress);
                                discountTicket = api.GetActualizarCantidadTiquetes(Convert.ToInt32(rWlEntity.whiteList.id), Convert.ToInt32(rWlEntity.whiteList.invoiceId), Convert.ToInt32(rWlEntity.whiteList.dianId), rWlEntity.whiteList.documentType.ToString(), Convert.ToInt32(rWlEntity.whiteList.availableEntries), Convert.ToInt32(rWlEntity.whiteList.gymId), rWlEntity.whiteList.branchId.ToString(), Convert.ToInt32(rWlEntity.whiteList.planId));
                        
                                if (discountTicket)
                                {
                                    ////Actualizar tiquetes en web tabla facturas o cortesias
                                    //if (wlBll.DiscountTicket(idNumeroFactura, id))
                                    //{
                                    //    DataTable dataDescontar = wlBll.DiscountTicketWeb(idNumeroFactura, id);
                                    //    if (dataDescontar.Rows.Count > 0)
                                    //    {
                                    //        api.GetActualizarCantidadTiquetes(Convert.ToInt32(dataDescontar.Rows[0]["id"]), Convert.ToInt32(dataDescontar.Rows[0]["invoiceId"]), Convert.ToInt32(dataDescontar.Rows[0]["dianId"]), dataDescontar.Rows[0]["documentType"].ToString(), Convert.ToInt32(dataDescontar.Rows[0]["availableEntries"]), Convert.ToInt32(dataDescontar.Rows[0]["gymId"]), Convert.ToInt32(dataDescontar.Rows[0]["branchId"]));
                                    //        log.WriteProcess("SE ACTUALIZARON LOS TIQUETES PARA LA PERSONA " + dataDescontar.Rows[0]["id"].ToString() + " CANTIDAD: " + dataDescontar.Rows[0]["availableEntries"]);
                                    //    }
                                    //}
                                    log.WriteProcess("SE ACTUALIZARON LOS TIQUETES PARA LA PERSONA " + rWlEntity.whiteList.id.ToString() + " CANTIDAD: " + rWlEntity.whiteList.availableEntries);
                                }

                                try
                                {
                                    if (rWlEntity.whiteList.planType == "T")
                                    {
                                        entryType = "tiquete";
                                    }
                                    else
                                    {
                                        entryType = "dia";
                                    }

                                    if (rWlEntity.whiteList.planType == "M")
                                    {
                                        if (rWlEntity.whiteList.availableEntries == 1)
                                        {
                                            enableDays = "Le queda " + rWlEntity.whiteList.availableEntries.ToString() + " " + entryType + " disponible.";
                                        }
                                        else if (rWlEntity.whiteList.typePerson == "Prospecto" && rWlEntity.whiteList.documentType == "Cortesía" && rWlEntity.whiteList.invoiceId == 0)
                                        {
                                            enableDays = "Te quedan pocas entradas disponibles, compra un plan para poder acceder mas veces";
                                        }
                                        else
                                        {
                                            enableDays = "Le quedan " + rWlEntity.whiteList.availableEntries.ToString() + " " + entryType + "s disponibles.";
                                        }
                                    }
                                    else if (rWlEntity.whiteList.planType == "T")
                                    {
                                        if (rWlEntity.whiteList.availableEntries == 1)
                                        {
                                            enableDays = "Le queda " + rWlEntity.whiteList.availableEntries.ToString() + " " + entryType + " disponible.";
                                        }
                                        else if (rWlEntity.whiteList.typePerson == "Prospecto" && rWlEntity.whiteList.documentType == "Cortesía" && rWlEntity.whiteList.invoiceId == 0)
                                        {
                                            enableDays = "Te quedan pocas entradas disponibles, compra un plan para poder acceder mas veces";
                                        }
                                        else
                                        {
                                            enableDays = "Le quedan " + rWlEntity.whiteList.availableEntries.ToString() + " " + entryType + "s disponibles.";
                                        }
                                    }

                                    ///////////////////////////////AQUI VA EL INSERT A LA BASE DE DATOS WEB CON LOS ULTIMOS RESULTADOS 

                                    entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, rWlEntity.whiteList.name, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId,
                                            rWlEntity.whiteList.documentType, discountTicket, 0, null, rWlEntity.whiteList.expirationDate, false, "Puede ingresar", rWlEntity.whiteList.planName,
                                            enableDays, true, ipAddress, "Entry", string.Empty, _qrCode, _temperature);
                                    return "true^" + msg;
                                }
                                catch
                                {
                                    return "false^" + msg;
                                }
                            }
                            else
                            {
                                try
                                {
                                    switch (response.message)
                                    {
                                        case "DifferentDay":
                                            msg = "Hoy no es el día de su clase.";
                                            break;
                                        case "OutOfSchedule":
                                            msg = "Está por fuera del horario de ingreso de esta clase. " + response.messageOne;
                                            break;
                                        case "NoConfig":
                                            msg = "No se pudo acceder a la configuración de la empresa, por favor vuelva a intentarlo";
                                            break;
                                        case "Ex":
                                            msg = "Ocurrió un error en el proceso de validación de restricciones horarias de la reserva. Error: " + response.messageOne;
                                            break;
                                        default:
                                            msg = "Ocurrió un error en el proceso de validación de las restricciones horarias de la reserva, por favor vuelva a intentarlo.";
                                            break;
                                    }

                                    log.WriteProcessByTerminal(msg, ipAddress);
                                    //clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                                    entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, ipAddress, "Entry",
                                                    string.Empty, _qrCode, _temperature);
                                    return "false^" + msg;
                                }
                                catch
                                {

                                    return "false^" + msg;
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                switch (response.message)
                                {
                                    case "NoWL":
                                        msg = "No se pudo obtener la información del usuario que desea ingresar.";
                                        break;
                                    case "Ex":
                                        msg = "Ocurrió un error en el proceso de validación de restricciones horarias. Error: " + response.messageOne;
                                        break;
                                    case "NoEntryByRestriction":
                                        msg = "Su plan no contempla esta hora.";
                                        break;
                                    default:
                                        msg = "Ocurrió un error en el proceso de validación de las restricciones horarias, por favor vuelva a intentarlo.";
                                        break;
                                }

                                log.WriteProcessByTerminal(msg, ipAddress);
                                //clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                                entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, ipAddress, "Entry",
                                                string.Empty, _qrCode, _temperature);
                                return "false^" + msg;
                            }
                            catch
                            {

                                return "false^" + msg;
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            switch (response.message)
                            {
                                case "NoWL":
                                    msg = "No se pudo obtener la información del usuario que desea ingresar.";
                                    break;
                                case "Ex":
                                    msg = "Ocurró un error en el proceso de validación de restricciones horarias. Error: " + response.messageOne;
                                    break;
                                case "NoEntryByRestriction":
                                    msg = "Superado Limite de ingresos del Grupo Familiar.";
                                    break;
                                default:
                                    msg = "Ocurrió un error en el proceso de validación de las entradas del grupo familiar, por favor vuelva a intentarlo.";
                                    break;
                            }

                            log.WriteProcessByTerminal(msg, ipAddress);
                            entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, ipAddress, "Entry",
                                            string.Empty, _qrCode, _temperature);
                            return "false^" + msg;
                        }
                        catch
                        {
                            return "false^" + msg;
                        }
                    }
                }
                else
                {
                    try
                    {
                        switch (rWlEntity.response.message)
                        {
                            case "Ex":
                                msg = "Ocurrió un error en el proceso de consulta en la lista blanca. Error: " + rWlEntity.response.messageOne;
                                break;
                            case "NoId":
                                msg = "No es posible validar si el usuario puede ingresar debido a que la identificación no fue ingresada.";
                                break;
                            case "NoEntityWL":
                                msg = "El usuario existe en la lista blanca pero ocurrió un error en el proceso de conversión de los datos (Entidad). " + rWlEntity.response.messageOne;
                                break;
                            case "NoWhiteList":
                                msg = "El usuario no existe en la lista blanca por lo tanto no es posible su ingreso. " + rWlEntity.response.messageOne;
                                break;
                            case "NoAvailableEntries":
                                msg = "El usuario no tiene tiquetes disponibles para ingresar.";
                                break;
                            case "NoReEntry":
                                msg = "El usuario no puede re-ingresar.";
                                break;
                            case "NoEntryWithoutFingerprint":
                                msg = "El usuario debe ingresar con huella.";
                                break;
                            case "NoTieneContratoFirmadoPorPlan":
                                msg = "No puede ingresar, Debe firmar el contrato por plan.";
                                break;
                            case "NoTieneContratoFirmado":
                                msg = "No puede ingresar, Debe firmar el contrato.";
                                break;
                            case "IngresosMaxTerminadosVisitantes":
                                msg = "El visitante no tiene más ingresos permitidos, por favor compre un plan.";
                                break;
                            case "NoTieneReservadiaActual":
                                msg = "El cliente no cuenta con una reserva activa para la hora.";
                                break;
                            default:
                                msg = "Ocurrió un error en el proceso de validación del ingreso del usuario.";
                                break;
                        }

                        entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, ipAddress, "Entry",
                                        string.Empty);


                        return "false^" + msg;
                    }
                    catch
                    {
                        return "false^" + msg;
                    }
                }

            }
            catch (Exception ex)
            {
                log.WriteProcessByTerminal("Ocurrió un error en el proceso de consulta de validación de entrada de usuario " + id + " y terminal ", ipAddress);
                log.WriteErrorsByTerminals(ex.Message + " - " + ex.StackTrace, ipAddress);
                return "false^" + ex.Message;
            }
        }

        /// <summary>
        /// Método que permite validar si al ingreso de este cliente se le debe descontar el tiquete en caso de tener plan tipo TIQUETERA.
        /// Getulio Vargas - 2018-09-02 - OD 1307
        /// </summary>
        /// <param name="whiteList"></param>
        /// <returns></returns>
        private bool ValidateDiscountTicket(eWhiteList whiteList, string ipAddress)
        {
            try
            {
                if (whiteList.planType == "T")
                {
                    AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                    eConfiguration config = acsBll.GetLocalAccessControlSettings();

                    if (config != null)
                    {
                        EventBLL eventBll = new EventBLL();
                        eEvent eventEntity = eventBll.GetLastEntryByUserIdAndInvoiceIdAndPlanId(whiteList.id, whiteList.invoiceId, whiteList.planId);

                        if (eventEntity != null && (DateTime.Now - eventEntity.entryHour).Minutes < config.intMinutosDescontarTiquetes)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ServiceLog log = new ServiceLog();
                log.WriteProcess("Ocurrió un error en el proceso de consulta de descuento de tiquetes para el usuario " + whiteList.id + " y terminal " + ipAddress);
                log.WriteError(ex.Message + " Terminal " + ipAddress + " - " + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Método para retornar la identificación de un cliente partiendo del código de la tarjeta asociado.
        /// Getulio Vargas - 2018-08-27 - OD 1307
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public string GetClientIdByCardId(string cardId, string ipAddress)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                string clientId = string.Empty;
                WhiteListBLL wlBll = new WhiteListBLL();
                DataTable dt = new DataTable();
                dt = wlBll.GetClientWhiteListByCardId(cardId);

                if (dt != null && dt.Rows.Count > 0)
                {
                    clientId = dt.Rows[0]["id"].ToString();
                }

                return clientId;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso de consulta de identificación de cliente por id de tarjeta para la tarjeta " + cardId + " y terminal " + ipAddress);
                log.WriteError(ex.Message + " Terminal " + ipAddress + " - " + ex.StackTrace);
                return "";
            }
        }

        /// <summary>
        /// Método para retornar la identificación de un cliente partiendo del código de la tarjeta asociado.
        /// Getulio Vargas - 2018-08-27 - OD 1307
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public string GetEmployeeIdByCardId(string cardId, string ipAddress)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                string clientId = string.Empty;
                WhiteListBLL wlBll = new WhiteListBLL();
                DataTable dt = new DataTable();
                dt = wlBll.GetEmployeeWhiteListByCardId(cardId);

                if (dt != null && dt.Rows.Count > 0)
                {
                    clientId = dt.Rows[0]["id"].ToString();
                }

                return clientId;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso de consulta de identificación de empleado por id de tarjeta para la tarjeta " + cardId + " y terminal " + ipAddress);
                log.WriteError(ex.Message + " Terminal " + ipAddress + " - " + ex.StackTrace);
                return "";
            }
        }

        public string GetClientIdByFingerprintId(int id, string ipAddress)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                string clientId = string.Empty;
                WhiteListBLL wlBll = new WhiteListBLL();
                DataTable dt = new DataTable();
                dt = wlBll.GetClientIdByFingerprintId(id);

                if (dt != null && dt.Rows.Count > 0)
                {
                    clientId = dt.Rows[0]["id"].ToString();
                }

                return clientId;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso de consulta de identificación del cliente por id de huella " + id + " terminal " + ipAddress);
                log.WriteError(ex.Message + " Terminal " + ipAddress + " - " + ex.StackTrace);
                return "";
            }
        }

        public DataTable GetClientId(string id, string ipAddress)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                string clientId = string.Empty;
                WhiteListBLL wlBll = new WhiteListBLL();
                DataTable dt = new DataTable();
                dt = wlBll.GetClientIdBioentry(id);


                return dt;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso de consulta de identificación del cliente por id de huella " + id + " terminal " + ipAddress);
                log.WriteError(ex.Message + " Terminal " + ipAddress + " - " + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Validamos si la persona tiene una entrada anterior SIN SALIDA, en caso de ser así se bloquea el ingreso de esta.
        /// Funcionalidad basada en el parámetro "blnAntipassbackEntrada"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ValidateEntryWithoutExit(int gymId, string branchId, string id, string ipAddress)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                EventBLL entryBll = new EventBLL();
                //eEvent eventEntity = new eEvent();
                //eventEntity = entryBll.GetLastEntryByUserId(id);

                EntryAPI entryAPI2 = new EntryAPI();
                List<eEntries> eventEntity = new List<eEntries>();
                eventEntity = entryAPI2.GetListEntradasUsuarios(gymId,branchId, id);

                if (eventEntity[0] != null)
                {
                    //if (eventEntity.thirdMessage == "HightTemp")
                    //{
                    //    return true;
                    //}
                    //else 
                    if (eventEntity[0].outDate == null || eventEntity[0].outDate == DateTime.MinValue || eventEntity[0].outDate.Date == new DateTime(1900, 01, 01))
                    {
                        entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "",
                                        "No tiene salida de la entrada anterior.", false, ((ipAddress == null || ipAddress == "" || ipAddress == " ") ? "IngresoTouch" : ipAddress), "Entry", string.Empty, _qrCode, _temperature);

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso de consulta de entradas anteriores para el usuario " + id);
                log.WriteError(ex.Message + " Terminal " + ipAddress + " - " + ex.StackTrace);
                return false;
            }
        }

        public bool ValidateExitById(string id, string ipAddress, bool isId)
        {
            ServiceLog log = new ServiceLog();
            leerJson lg = new leerJson();
            try
            {
                WhiteListBLL wlBll = new WhiteListBLL();
                EventBLL entryBll = new EventBLL();
                eResponseWhiteList rWlEntity = new eResponseWhiteList();
                eResponse response = new eResponse();
                List<eConfiguration> config = new List<eConfiguration>();
                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                string msg = string.Empty;
                bool validateEntryInOutput = false;

                /// se cambia consulta de configuracion a archivo json
                string json = lg.cargarArchivos(1);
                if (json != "")
                {
                    config = JsonConvert.DeserializeObject<List<eConfiguration>>(json);
                }

                //config = acsBll.GetLocalAccessControlSettings();

                if (config != null)
                {
                    validateEntryInOutput = !config[0].bitNo_Validar_Entrada_En_Salida;
                }

                rWlEntity = wlBll.ValidateExitByUserId(id, false, isId);
                DataTable dtEntry = null;

                if (rWlEntity.whiteList != null)
                {
                    dtEntry = entryBll.GetEntryByUserWithoutOutput(id, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId, true);
                }

                if (rWlEntity.response.state)
                {
                    if (validateEntryInOutput)
                    {
                        ///////////  Pendiente insertar registro salida en la base de datos web
                        entryBll.UpdateExit(id, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId, false, "Vuelva pronto.");
                        return true;
                    }
                    else
                    {
                        if (dtEntry != null && dtEntry.Rows.Count > 0)
                        {
                            ///////////  Pendiente insertar registro salida en la base de datos web
                            entryBll.UpdateExit(id, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId, false, "Vuelva pronto.");
                        }
                        else
                        {
                            ///////////  Pendiente insertar registro salida en la base de datos web
                            entryBll.Insert(clsEnum.EntryType.Exit.ToString(), id, rWlEntity.whiteList.name, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId,
                                            rWlEntity.whiteList.documentType, false, 0, null, null, false, "Puede salir", rWlEntity.whiteList.planName,
                                            string.Empty, true, "IngresoTouch", "Entry", "Vuelva pronto.", _qrCode, _temperature);
                        }

                        return true;
                    }
                }
                else
                {
                    switch (rWlEntity.response.message)
                    {
                        case "Ex":
                            msg = "Ocurrió un error en el proceso de consulta en la lista blanca.\nError: " + rWlEntity.response.messageOne;
                            break;
                        case "NoId":
                            msg = "No es posible validar si el usuario puede ingresar debido a que la identificación no fue ingresada.";
                            break;
                        case "NoEntityWL":
                            msg = "El usuario existe en la lista blanca pero ocurrió un error en el proceso de conversión de los datos (Entidad)." + rWlEntity.response.messageOne;
                            break;
                        case "NoWhiteList":
                            msg = "El usuario no existe en la lista blanca por lo tanto no es posible su ingreso." + rWlEntity.response.messageOne;
                            break;
                        case "NoEntry":
                            msg = "El usuario no tiene entrada previa." + rWlEntity.response.messageOne;
                            break;
                        case "NoExitWithoutFingerprint":
                            msg = "El usuario debe salir con huella.";
                            break;
                        default:
                            msg = "Ocurrió un error en el proceso de validación de la salida del usuario.";
                            break;
                    }

                    entryBll.Insert(clsEnum.EntryType.Exit.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede salir", "", string.Empty, false, "IngresoTouch",
                                    "Entry", msg, _qrCode, _temperature);
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso de validación de la salida del usuario " + id + " terminal " + ipAddress);
                log.WriteError(ex.Message + " Terminal " + ipAddress + " - " + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Inserta el evento cuando no se pudo ingresar (o salir) porque no se encontro el id de la persona
        /// probablemente debido a que el Qr se ha vencido o usado
        /// </summary>
        /// <param name="qr"></param>
        /// <param name="temperature"></param>
        /// <param name="isService">en caso de enviarse desde el sevicio se envia en true</param>
        public void InsertDeniedEventByNotQr(string qr, string temperature, bool isService, string ipAddress)
        {
            EventBLL entryBll = new EventBLL();
            entryBll.Insert(clsEnum.EntryType.Entry.ToString(), "", "", 0, 0, "", false, 0, null,
                null, isService, "No se pudo encontrar su identificación por medio del código qr ó este ya se venció.", "", "", false, ipAddress, "Entry", "", qr, temperature);

        }

    }
}
