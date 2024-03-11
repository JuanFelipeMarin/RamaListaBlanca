using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.BLL.Helpers;
using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Data;

namespace Sibo.WhiteList.Service.Classes
{
    public class clsWhiteList
    {
        private string _qrCode = null;
        private string _temperature = null;

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
            ServiceLog log = new ServiceLog();

            try
            {
                WhiteListBLL wlBLL = new WhiteListBLL();
                return wlBLL.GetWhiteList(gymId, branchId);
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso de consulta de la lista blanca");
                log.WriteError(ex.Message + " - " + ex.StackTrace);
                return false;
            }
        }

        public bool UpdateFingerprint(string clientId, int fingerId, byte[] fingerprintImage, string ipAddress)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                WhiteListBLL wlBll = new WhiteListBLL();
                return wlBll.UpdateFingerprint(clientId, fingerId, fingerprintImage);
            }
            catch (Exception ex)
            {
                log.WriteProcessByTerminal("Ocurrió un error en el proceso de actualización de huella del usuario " + clientId + ", huella " + fingerId, ipAddress);
                log.WriteErrorsByTerminals(ex.Message + " Terminal " + ipAddress + " - " + ex.StackTrace, ipAddress);
                return false;
            }
        }

        public bool ValidateEntryByUserId(string userId, string ipAddress, bool isId)
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
                string msg = string.Empty;
                bool discountTicket = false;
                rWlEntity = wlBll.ValidateEntryByUserId(id, false, isId);

                if (rWlEntity.response.state)
                {
                    //Validar entradas grupo familiar
                    response = wlBll.ValidateEntriesFamilyGroup(rWlEntity.whiteList, false);

                    if (response.state)
                    {
                        //Validar restricciones horarias
                        response = wlBll.ValidateRestrictions(rWlEntity.whiteList, false);

                        if (response.state)
                        {
                            //Mtoro Resolución Incidente 5671 de la OD 1307
                            // en caso de haber reservado para una clase
                            if (!string.IsNullOrEmpty(rWlEntity.whiteList.className)
                                && !string.IsNullOrEmpty(rWlEntity.whiteList.classSchedule)
                                && rWlEntity.whiteList.dateClass != null)
                            {
                                response = wlBll.ValidateClassSchedule(rWlEntity.whiteList, false);

                                if (response.state)
                                {
                                    wlBll.UpdateAsistedClasesClient(rWlEntity.whiteList, false);
                                }
                            }
                            //validar si esta dentro del rango de tiempo establecido ↑


                            if (response.state)
                            {
                                //Validar minutos no re-ingreso
                                string enableDays = string.Empty, entryType = string.Empty;

                                //validamos si se debe descontar tiquetes al usuario en esta entrada
                                discountTicket = ValidateDiscountTicket(rWlEntity.whiteList, ipAddress);

                                if (discountTicket)
                                {
                                    //Validamos si se debe descontar tiquete lo descontamos de la lista blanca.
                                    wlBll.DiscountTicket(rWlEntity.whiteList.intPkId);
                                    DataTable dataWL = wlBll.GetWhiteListByUserId(userId);

                                    if (dataWL != null)
                                    {
                                        whiteListEntity = wlBll.ConvertToEntity(dataWL);
                                        rWlEntity.whiteList = whiteListEntity;
                                    }
                                }

                                if (rWlEntity.whiteList.planType == "T")
                                {
                                    entryType = "tiquete";
                                }
                                else
                                {
                                    entryType = "dia";
                                }

                                if (rWlEntity.whiteList.availableEntries == 1)
                                {
                                    enableDays = "Le queda " + rWlEntity.whiteList.availableEntries.ToString() + " " + entryType + " disponible.";
                                }
                                else
                                {
                                    enableDays = "Le quedan " + rWlEntity.whiteList.availableEntries.ToString() + " " + entryType + "s disponibles.";
                                }

                                entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, rWlEntity.whiteList.name, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId,
                                            rWlEntity.whiteList.documentType, discountTicket, 0, null, rWlEntity.whiteList.expirationDate, false, "Puede ingresar", rWlEntity.whiteList.planName,
                                            enableDays, true, ipAddress, "Entry", string.Empty, _qrCode, _temperature);
                                return true;
                            }
                            else
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
                                return false;
                            }
                            //FIN //Mtoro Resolución Incidente 5671 de la OD 1307
                        }
                        else
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
                            return false;
                        }
                    }
                    else
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
                        //clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                        entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, ipAddress, "Entry",
                                        string.Empty, _qrCode, _temperature);
                        return false;
                    }
                }
                else
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
                        default:
                            msg = "Ocurrió un error en el proceso de validación del ingreso del usuario.";
                            break;
                    }

                    log.WriteProcessByTerminal(msg, ipAddress);
                    entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, ipAddress, "Entry",
                                    string.Empty, _qrCode, _temperature);

                    return false;
                }
            }
            catch (Exception ex)
            {
                log.WriteProcessByTerminal("Ocurrió un error en el proceso de consulta de validación de entrada de usuario " + id + " y terminal ", ipAddress);
                log.WriteErrorsByTerminals(ex.Message + " - " + ex.StackTrace, ipAddress);
                return false;
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
            ServiceLog log = new ServiceLog();

            try
            {
                bool response = true;
                eConfiguration config = new eConfiguration();
                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                eEvent eventEntity = new eEvent();
                EventBLL eventBll = new EventBLL();

                if (whiteList.planType == "T")
                {
                    config = acsBll.GetLocalAccessControlSettings();
                    eventEntity = eventBll.GetLastEntryByUserIdAndInvoiceIdAndPlanId(whiteList.id, whiteList.invoiceId, whiteList.planId);

                    if (config != null)
                    {
                        if (eventEntity != null)
                        {
                            if ((DateTime.Now - eventEntity.entryHour).Minutes >= config.intMinutosDescontarTiquetes)
                            {
                                response = true;
                            }
                            else
                            {
                                response = false;
                            }
                        }
                        else
                        {
                            response = true;
                        }
                    }
                    else
                    {
                        response = true;
                    }
                }
                else
                {
                    response = false;
                }

                return response;
            }
            catch (Exception ex)
            {
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

        /// <summary>
        /// Validamos si la persona tiene una entrada anterior SIN SALIDA, en caso de ser así se bloquea el ingreso de esta.
        /// Funcionalidad basada en el parámetro "blnAntipassbackEntrada"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ValidateEntryWithoutExit(string id, string ipAddress)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                EventBLL entryBll = new EventBLL();
                eEvent eventEntity = new eEvent();
                eventEntity = entryBll.GetLastEntryByUserId(id);

                if (eventEntity != null)
                {
                    if(eventEntity.thirdMessage == "HightTemp")
                    {
                        return true;
                    }
                    else if(eventEntity.outDate == null || eventEntity.outDate == DateTime.MinValue || eventEntity.outDate.Date == new DateTime(1900, 01, 01))
                    {
                        entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "",
                                        "No tiene salida de la entrada anterior.", false, "IngresoTouch", "Entry", string.Empty, _qrCode, _temperature);
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

            try
            {
                WhiteListBLL wlBll = new WhiteListBLL();
                EventBLL entryBll = new EventBLL();
                eResponseWhiteList rWlEntity = new eResponseWhiteList();
                eResponse response = new eResponse();
                eConfiguration config = new eConfiguration();
                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                string msg = string.Empty;
                bool validateEntryInOutput = false;

                config = acsBll.GetLocalAccessControlSettings();

                if (config != null)
                {
                    validateEntryInOutput = !config.bitNo_Validar_Entrada_En_Salida;
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
                        entryBll.UpdateExit(id, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId, false, "Vuelva pronto.");
                        return true;
                    }
                    else
                    {
                        if (dtEntry != null && dtEntry.Rows.Count > 0)
                        {
                            entryBll.UpdateExit(id, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId, false, "Vuelva pronto.");
                        }
                        else
                        {
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
