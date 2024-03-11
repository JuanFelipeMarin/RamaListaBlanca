using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.BLL.Helpers;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Configuration;
using System.Data;

namespace Sibo.WhiteList.IngresoMonitor.Classes
{
    public class clsWhiteList
    {
        FrmSeleccionarMultiplePlan frmMultiplePLan = new FrmSeleccionarMultiplePlan();

        public bool ValidateEntryByUserId(string id, string ipAddress, bool isId)
        {
            try
            {
                WhiteListBLL wlBll = new WhiteListBLL();
                EventBLL entryBll = new EventBLL();
                eResponseWhiteList rWlEntity = new eResponseWhiteList();
                eResponse response = new eResponse();

                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                eConfiguration config = new eConfiguration();
                string msg = string.Empty;
                config = acsBll.GetLocalAccessControlSettings();
                rWlEntity = wlBll.ValidateEntryByUserId(id, isId, false, config);

                if (rWlEntity.response.state)
                {
                    int idEmpresa = Convert.ToInt32(ConfigurationManager.AppSettings["gymId"]);
                    int idNumeroFactura = 0;
                    idNumeroFactura = rWlEntity.whiteList.intPkId;

                    ////SECCION DE MULTIPLANES DE PARA CLIENTES
                    //if (config != null)
                    //{
                    //    if (config.bitMultiplesPlanesVig)
                    //    {
                    //        //VALIDA SI LA PERSONA QUE MARCO ES UN CLIENTE
                    //        if (rWlEntity.whiteList.typePerson == "Cliente")
                    //        {
                    //            DataTable datosPlanes = new DataTable();
                    //            WhiteListBLL obj = new WhiteListBLL();
                    //            datosPlanes = obj.obtenerPlanesPersona(Convert.ToInt32(id));

                    //            //Se valida que la lista que contiene los planes de la persona no este vacia
                    //            if (datosPlanes != null)
                    //            {
                    //                if (datosPlanes.Rows.Count > 1)
                    //                {
                    //                    //Se crea una instancia del formulario que muestra los diferentes planes disponibles
                    //                    FrmSeleccionarMultiplePlan frmMultiplePLan = new FrmSeleccionarMultiplePlan();

                    //                    //Se le entregan los datos a la tabla del formulario, se actualiza y se muestra el formulario
                    //                    frmMultiplePLan.DgvPlanes.DataSource = datosPlanes;
                    //                    frmMultiplePLan.DgvPlanes.Refresh();
                    //                    frmMultiplePLan.ShowDialog();

                    //                    //Se captura el id de la factura que la persona selecciono
                    //                    idNumeroFactura = frmMultiplePLan.planSeleccionado;

                    //                    //Si el id que llego es cero significa que la persona no selecciono ningun plan o cerro la vista
                    //                    if (idNumeroFactura == 0)
                    //                    {
                    //                        msg = "Debes seleccionar un plan de la lista, realizar el ingreso nuevamente y selecciona un plan.";
                    //                        clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                    //                        return false;
                    //                    }
                    //                }
                    //                else if (datosPlanes.Rows.Count == 1)
                    //                {
                    //                    //Si la persona solo tiene un plan se toma el unico id de factura que tiene
                    //                    idNumeroFactura = Convert.ToInt32(datosPlanes.Rows[0][0]);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                //Si no se encuntran datos del los planes
                    //                idNumeroFactura = rWlEntity.whiteList.invoiceId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //Si no es un clientes se toma el id de la factura en base al registro de whitelist que se tomo
                    //            idNumeroFactura = rWlEntity.whiteList.invoiceId;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //Si no esta habilitado multiplanes                 |
                    //        idNumeroFactura = rWlEntity.whiteList.invoiceId;
                    //    }
                    //}
                    //else
                    //{
                    //    //Si no hay configuracion de ingreso sigue con el proceso como lo hacia anteriormente
                    //    idNumeroFactura = rWlEntity.whiteList.invoiceId;
                    //}

                    //Validar entradas grupo familiar
                    response = wlBll.ValidateEntriesFamilyGroup(rWlEntity.whiteList, false);

                    if (response.state)
                    {
                        //Validar restricciones horarias incluyendo los días festivos
                        response = wlBll.ValidateRestrictions(rWlEntity.whiteList, false);

                        if (response.state)
                        {
                            //validar si esta dentro del rango de tiempo establecido ↑
                            if (response.state)
                            {
                                //Validar minutos no re-ingreso
                                string enableDays = string.Empty;

                                if (rWlEntity.whiteList.availableEntries == 1)
                                {
                                    if (rWlEntity.whiteList.planType == "M")
                                    {
                                        enableDays = "Le queda " + rWlEntity.whiteList.availableEntries.ToString() + " día disponible.";
                                    }
                                    else if (rWlEntity.whiteList.planType == "T")
                                    {
                                        enableDays = "Le queda " + rWlEntity.whiteList.availableEntries.ToString() + " tiquete disponible.";
                                    }
                                    else if (rWlEntity.whiteList.typePerson == "Prospecto" && rWlEntity.whiteList.documentType == "Cortesía" && rWlEntity.whiteList.invoiceId == 0)
                                    {
                                        enableDays = "Te quedan pocas entradas disponibles, compra un plan para poder acceder mas veces";
                                    }
                                    else
                                    {
                                        enableDays = "Le queda " + rWlEntity.whiteList.availableEntries.ToString() + " día disponible.";
                                    }
                                }
                                else
                                {
                                    if (rWlEntity.whiteList.planType == "M")
                                    {
                                        enableDays = "Le quedan " + rWlEntity.whiteList.availableEntries.ToString() + " días disponibles.";
                                    }
                                    else if (rWlEntity.whiteList.typePerson == "Prospecto" && rWlEntity.whiteList.documentType == "Cortesía" && rWlEntity.whiteList.invoiceId == 0)
                                    {
                                        enableDays = "Te quedan pocas entradas disponibles, compra un plan para poder acceder mas veces";
                                    }
                                    else
                                    {
                                        enableDays = "Le quedan " + rWlEntity.whiteList.availableEntries.ToString() + " tiquetes disponibles.";
                                    }
                                }

                                //validamos si se debe descontar tiquetes al usuario en esta entrada
                                bool discountTicket = ValidateDiscountTicket(rWlEntity.whiteList);

                                if (discountTicket)
                                {
                                    //Validamos si se debe descontar tiquete lo descontamos de la lista blanca.
                                    //wlBll.DiscountTicket(idNumeroFactura, id);
                                    if (wlBll.DiscountTicket(idNumeroFactura, id) == true)
                                    {
                                        DataTable dataDescontar = wlBll.DiscountTicketWeb(idNumeroFactura, id);
                                        if (dataDescontar.Rows.Count > 0)
                                        {
                                            WhiteListAPI api = new WhiteListAPI();
                                            ///arreglar despues 
                                            //api.GetActualizarCantidadTiquetes(Convert.ToInt32(dataDescontar.Rows[0]["id"]), Convert.ToInt32(dataDescontar.Rows[0]["invoiceId"]), Convert.ToInt32(dataDescontar.Rows[0]["dianId"]), dataDescontar.Rows[0]["documentType"].ToString(), Convert.ToInt32(dataDescontar.Rows[0]["availableEntries"]), Convert.ToInt32(dataDescontar.Rows[0]["gymId"]), Convert.ToInt32(dataDescontar.Rows[0]["branchId"]));
                                        }
                                    }
                                }

                                entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, rWlEntity.whiteList.name, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId,
                                                rWlEntity.whiteList.documentType, discountTicket, 0, null, rWlEntity.whiteList.expirationDate, false, "Puede ingresar",
                                                rWlEntity.whiteList.planName, enableDays, true, ipAddress, "Entry", string.Empty);
                                return true;
                            }
                            else
                            {
                                switch (response.message)
                                {
                                    case "DifferentDay":
                                        msg = "Hoy no es el día de su clase.\n" + response.messageOne;
                                        break;
                                    case "OutOfSchedule":
                                        msg = "Está por fuera del horario de ingreso de esta clase.\n" + response.messageOne;
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

                                clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                                entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, ipAddress, "Entry",
                                                string.Empty);
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
                                    msg = "Ocurrió un error en el proceso de validación de restricciones horarias. Error: " + response.messageOne;
                                    break;
                                case "NoEntryByRestriction":
                                    msg = "Su plan o subgrupo no contempla esta hora.";
                                    break;
                                default:
                                    msg = "Ocurrió un error en el proceso de validación de las restricciones horarias, por favor vuelva a intentarlo.";
                                    break;
                            }

                            clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                            entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, ipAddress, "Entry",
                                            string.Empty);
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

                        clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                        entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, ipAddress, "Entry",
                                        string.Empty);
                        return false;
                    }
                }
                else
                {
                    switch (rWlEntity.response.message)
                    {
                        case "NoAvailableEntries":
                            msg = "No cuentas con tiquetes disponibles para ingresar.";
                            break;
                        case "NoReEntry":
                            msg = "No puedes re-ingresar en este momento.";
                            break;
                        case "IngresosMaxTerminadosVisitantes":
                            msg = "Has superado el limite de ingresos, por lo tanto no es posible su ingreso";
                            break;
                        case "NoEntryWithoutFingerprint":
                            msg = "No tienes permitido ingresar sin huella, por lo tanto no es posible su ingreso";
                            break;
                        case "NoWhiteList":
                            msg = "La persona no existe en la lista blanca por lo tanto no es posible su ingreso." + rWlEntity.response.messageOne;
                            clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                            break;
                        case "NoId":
                            msg = "No es posible validar si el usuario puede ingresar debido a que la identificación no fue ingresada.";
                            clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                            break;
                        case "Ex":
                            msg = "Ocurrió un error en el proceso de consulta en la lista blanca.\nError: " + rWlEntity.response.messageOne;
                            clsShowMessage.Show(msg, clsEnum.MessageType.Error);
                            break;
                        default:
                            msg = "Ocurrió un error en el proceso de validación del ingreso del usuario.";
                            clsShowMessage.Show(msg, clsEnum.MessageType.Error);
                            break;
                    }

                    entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, ipAddress, "Entry",
                                    string.Empty);

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite validar si al ingreso de este cliente se le debe descontar el tiquete en caso de tener plan tipo TIQUETERA.
        /// Getulio Vargas - 2018-09-02 - OD 1307
        /// </summary>
        /// <param name="whiteList"></param>
        /// <returns></returns>
        private bool ValidateDiscountTicket(eWhiteList whiteList)
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
                throw ex;
            }
        }

        public bool UpdateFingerprint(int gymId, int branchId, string clientId, int fingerId, byte[] fingerprintImage)
        {
            try
            {
                WhiteListBLL wlBll = new WhiteListBLL();
                return wlBll.UpdateFingerprint(gymId, branchId, clientId, fingerId, fingerprintImage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidateExitById(string id, string ipAddress, bool isId)
        {
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
                                        string.Empty, true, ipAddress, "Entry", "Vuelva pronto.");
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
                            clsShowMessage.Show(msg, clsEnum.MessageType.Error);
                            break;
                        case "NoId":
                            msg = "No es posible validar si el usuario puede ingresar debido a que la identificación no fue ingresada.";
                            clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                            break;
                        case "NoEntityWL":
                            msg = "El usuario existe en la lista blanca pero ocurrió un error en el proceso de conversión de los datos (Entidad)." + rWlEntity.response.messageOne;
                            clsShowMessage.Show(msg, clsEnum.MessageType.Error);
                            break;
                        case "NoWhiteList":
                            msg = "El usuario no existe en la lista blanca por lo tanto no es posible su ingreso." + rWlEntity.response.messageOne;
                            clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                            break;
                        case "NoEntry":
                            msg = "El usuario no tiene entrada previa." + rWlEntity.response.messageOne;
                            clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                            break;
                        default:
                            msg = "Ocurrió un error en el proceso de validación de la salida del usuario.";
                            clsShowMessage.Show(msg, clsEnum.MessageType.Error);
                            break;
                    }

                    entryBll.Insert(clsEnum.EntryType.Exit.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede salir", "", string.Empty, false, ipAddress, "Entry",
                                    msg);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable consultarTerminal()
        {
            TerminalBLL termina = new TerminalBLL();
            DataTable dtt = new DataTable();

            dtt = termina.consultarTerminal();
            return dtt;

        }
    }
}
