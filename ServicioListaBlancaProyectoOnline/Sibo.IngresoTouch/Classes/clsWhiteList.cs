using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.BLL.Helpers;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sibo.WhiteList.IngresoTouch.Classes
{
    public class clsWhiteList
    {
        FrmSeleccionarMultiplePlan frmMultiplePLan = new FrmSeleccionarMultiplePlan();

        PrintDocument printDoc = new PrintDocument();
        public eResponseWhiteList rWlEntity_Global = new eResponseWhiteList();

        public eResponseWhiteList ValidateEntryByUserId(string id, bool isId)
        {
            try
            {
                eResponseWhiteList rWlEntity = new eResponseWhiteList();
                WhiteListBLL wlBll = new WhiteListBLL();
                EventBLL entryBll = new EventBLL();
                eResponseWhiteList returnResponse = new eResponseWhiteList();
                eResponse response = new eResponse();
                WhiteListAPI api = new WhiteListAPI();
                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                eConfiguration config = new eConfiguration();
                string msg = string.Empty;
                config = acsBll.GetLocalAccessControlSettings();

                rWlEntity = wlBll.ValidateEntryByUserId(id, false, isId, config);

                if (rWlEntity.response.state)
                {
                    int idEmpresa = Convert.ToInt32(ConfigurationManager.AppSettings["gymId"]);
                    int idNumeroFactura = 0;
                    idNumeroFactura = rWlEntity.whiteList.intPkId;

                    //SECCION DE MULTIPLANES DE PARA CLIENTES
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
                    //                        return null;
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
                    //                idNumeroFactura = rWlEntity.whiteList.intPkId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //Si no es un clientes se toma el id de la factura en base al registro de whitelist que se tomo
                    //            idNumeroFactura = rWlEntity.whiteList.intPkId;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //Si no esta habilitado multiplanes
                    //        idNumeroFactura = rWlEntity.whiteList.intPkId;
                    //    }
                    //}
                    //else
                    //{
                    //    //Si no hay configuracion de ingreso sigue con el proceso como lo hacia anteriormente
                    //    idNumeroFactura = rWlEntity.whiteList.intPkId;
                    //}

                    //Validar entradas grupo familiar
                    response = wlBll.ValidateEntriesFamilyGroup(rWlEntity.whiteList, false);

                    if (response.state)
                    {
                        //Validar restricciones horarias incluyendo los días festivos
                        response = wlBll.ValidateRestrictions(rWlEntity.whiteList, false);

                        if (response.state)
                        {
                            if (response.state)
                            {
                                //Validar minutos no re-ingreso
                                string enableDays = string.Empty;

                                if (rWlEntity.whiteList.availableEntries == 1)
                                {
                                    //enableDays = "Le queda " + rWlEntity.whiteList.availableEntries.ToString() + " día disponible.";
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
                                    if (wlBll.DiscountTicket(idNumeroFactura, id))
                                    {
                                        DataTable dataDescontar = wlBll.DiscountTicketWeb(idNumeroFactura, id);
                                        if (dataDescontar.Rows.Count > 0)
                                        {
                                            ////peNDIENTE
                                            //api.GetActualizarCantidadTiquetes(Convert.ToInt32(dataDescontar.Rows[0]["id"]), Convert.ToInt32(dataDescontar.Rows[0]["invoiceId"]), Convert.ToInt32(dataDescontar.Rows[0]["dianId"]), dataDescontar.Rows[0]["documentType"].ToString(), Convert.ToInt32(dataDescontar.Rows[0]["availableEntries"]), Convert.ToInt32(dataDescontar.Rows[0]["gymId"]), Convert.ToInt32(dataDescontar.Rows[0]["branchId"]));
                                        }

                                    }
                                }

                                entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, rWlEntity.whiteList.name, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId,
                                                rWlEntity.whiteList.documentType, discountTicket, 0, null, rWlEntity.whiteList.expirationDate, false, "Puede ingresar",
                                                rWlEntity.whiteList.planName, enableDays, true, "IngresoTouch", "Entry", string.Empty);

                                returnResponse.whiteList = rWlEntity.whiteList;
                                response.messageTwo = "Puede ingresar";
                                response.messageThree = enableDays;
                                returnResponse.response = response;

                                return returnResponse;
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

                                clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                                entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, "IngresoTouch",
                                                "Entry", string.Empty);
                                response.messageTwo = "No puede ingresar";
                                response.messageThree = msg;
                                returnResponse.whiteList = rWlEntity.whiteList;
                                returnResponse.response = response;
                                return returnResponse;
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
                            entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, "IngresoTouch",
                                            "Entry", string.Empty);
                            response.messageTwo = "No puede ingresar";
                            response.messageThree = msg;
                            returnResponse.whiteList = rWlEntity.whiteList;
                            returnResponse.response = response;
                            return returnResponse;
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
                        entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, "IngresoTouch",
                                        "Entry", string.Empty);
                        response.messageTwo = "No puede ingresar";
                        response.messageThree = msg;
                        returnResponse.whiteList = rWlEntity.whiteList;
                        returnResponse.response = response;
                        return returnResponse;
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

                    entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, "IngresoTouch",
                                    "Entry", string.Empty);
                    response.messageTwo = "No puede ingresar";
                    response.messageThree = msg;
                    returnResponse.whiteList = rWlEntity.whiteList;
                    returnResponse.response = response;
                    return returnResponse;
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


        public bool UpdateFingerprint(int gymId,int branchId,string clientId, int fingerId, byte[] fingerprintImage)
        {
            try
            {
                WhiteListBLL wlBll = new WhiteListBLL();
                return wlBll.UpdateFingerprint(gymId,branchId,clientId, fingerId, fingerprintImage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public eResponseWhiteList ValidateExitById(string id, bool isId)
        {
            try
            {
                WhiteListBLL wlBll = new WhiteListBLL();
                EventBLL entryBll = new EventBLL();
                eResponseWhiteList rWlEntity = new eResponseWhiteList();
                eResponseWhiteList returnResponse = new eResponseWhiteList();
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
                        response.messageTwo = "Puede salir";
                        response.messageThree = "Vuelva pronto";
                        response.state = true;
                        returnResponse.whiteList = rWlEntity.whiteList;
                        returnResponse.response = response;
                        return returnResponse;
                    }
                    else
                    {
                        if (dtEntry != null && dtEntry.Rows.Count > 0)
                        {
                            entryBll.UpdateExit(id, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId, false, "Vuelva pronto.");
                            response.messageTwo = "Puede salir";
                            response.messageThree = "Vuelva pronto";
                            response.state = true;
                            returnResponse.whiteList = rWlEntity.whiteList;
                            returnResponse.response = response;
                        }
                        else
                        {
                            entryBll.Insert(clsEnum.EntryType.Exit.ToString(), id, rWlEntity.whiteList.name, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId,
                                        rWlEntity.whiteList.documentType, false, 0, null, null, false, "Puede salir", rWlEntity.whiteList.planName,
                                        "Vuelva pronto", true, "IngresoTouch", "Entry", string.Empty);
                            response.messageTwo = "Puede salir";
                            response.messageThree = "Vuelva pronto";
                            response.state = true;
                            returnResponse.whiteList = rWlEntity.whiteList;
                            returnResponse.response = response;
                        }

                        return returnResponse;
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

                    entryBll.Insert(clsEnum.EntryType.Exit.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede salir", "", msg, false, "IngresoTouch", "Entry",
                                    string.Empty);
                    response.messageTwo = "No puede salir";
                    response.messageThree = msg;
                    returnResponse.whiteList = rWlEntity.whiteList;
                    response.state = false;
                    returnResponse.response = response;
                    return returnResponse;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite validar si el usuario puede ingresar sin huella.
        /// Getulio Vargas - 2017-09-04
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool EntryWithoutFingerprint(string id)
        {
            try
            { 
                //COPIARSE EL DE XPACIOS QUE YA ESTA BIEN CONFIGURADO
                WhiteListBLL wlBll = new WhiteListBLL();
                eResponseWhiteList rWlEntity = new eResponseWhiteList();
                DataTable dt = new DataTable();
                string msg = string.Empty;

                rWlEntity = wlBll.ValidateEntryByUserId(id, false, true, null);

                if (rWlEntity.response.state)
                {
                    if (rWlEntity.whiteList.withoutFingerprint)
                    {
                        rWlEntity_Global = rWlEntity;
                        return true;
                    }
                    else
                    {
                        clsShowMessage.Show("El usuario debe ingresar con huella.", clsEnum.MessageType.Informa);
                        return false;
                    }
                }
                else if (rWlEntity.response.message == "NoEntryWithoutFingerprint")
                {
                    clsShowMessage.Show("El usuario debe ingresar con huella.", clsEnum.MessageType.Informa);
                    return false;
                }
                else if (rWlEntity.response.message == "NoTieneReservadiaActual")
                {
                    clsShowMessage.Show("Su plan o reserva no contempla este horario.", clsEnum.MessageType.Error);
                    return false;
                }
                else if (rWlEntity.response.message == "IngresosMaxTerminadosVisitantes")
                {
                    clsShowMessage.Show("El visitante no tiene más ingresos permitidos, por favor compre un plan.", clsEnum.MessageType.Error);
                    return false;
                }
                else if (rWlEntity.response.message == "NoTieneContratoFirmado")
                {
                    clsShowMessage.Show("No puede ingresar, Debe firmar el contrato.", clsEnum.MessageType.Error);
                    return false;
                }
                else if (rWlEntity.response.message == "NoTieneContratoFirmadoPorPlan")
                {
                    clsShowMessage.Show("No puede ingresar, Debe firmar el contrato por plan.", clsEnum.MessageType.Error);
                    return false;
                }
                else
                {
                    clsShowMessage.Show("El usuario no existe en la lista blanca, por lo tanto no podrá ingresar.", clsEnum.MessageType.Informa);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Validamos si la persona tiene una entrada anterior SIN SALIDA, en caso de ser así se bloquea el ingreso de esta.
        /// Funcionalidad basada en el parámetro "blnAntipassbackEntrada"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ValidateEntryWithoutExit(string id)
        {
            try
            {
                EventBLL entryBll = new EventBLL();
                eEvent eventEntity = new eEvent();
                eventEntity = entryBll.GetLastEntryByUserId(id);

                if (eventEntity != null)
                {
                    if (eventEntity.outDate == null || eventEntity.outDate == DateTime.MinValue || eventEntity.outDate.Date == new DateTime(1900, 01, 01))
                    {
                        entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "",
                                        "No tiene salida de la entrada anterior.", false, "IngresoTouch", "Entry", string.Empty);
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
                throw ex;
            }
        }

        public int CargaReservasWeb(Double dbvalorGenerado, String IDGymsoftWeb, String intRamgoHoraPrint)
        {
            printDoc = new System.Drawing.Printing.PrintDocument();

            DataTable dttReservasWeb = new DataTable();
            String FechaActual;
            String HoraInicial;
            String HoraFinal;
            DateTime FechaIni;
            DateTime FechaFin;

            FechaIni = DateTime.Now;
            FechaActual = FechaIni.ToString("yyyy/MM/dd");
            FechaFin = FechaIni.AddMinutes(double.Parse(intRamgoHoraPrint));

            HoraInicial = FechaIni.Hour.ToString() + ":" + FechaIni.Minute.ToString() + ":" + FechaIni.Second.ToString();
            HoraFinal = FechaFin.Hour.ToString() + ":" + FechaFin.Minute.ToString() + ":" + FechaFin.Second.ToString();

            CultureInfo culture = new CultureInfo("es-ES");

            WSComunicaNetVigente.WSComunicaNetVigenteSoapClient WS = new WSComunicaNetVigente.WSComunicaNetVigenteSoapClient();
            //Boolean swReservaImpresa;

            if (dbvalorGenerado > 0)
            {
                //Consulta las reservas activas del cliente
                //dttReservasWeb = WS.ConsultarReservasClienteTiqueteNET(dbvalorGenerado, IDGymsoftWeb, intCodigoSucursalActual);
                if (dttReservasWeb.Rows.Count > 0)
                {

                    //       For Each drwReservasWeb As DataRow In dttReservasWeb.Rows
                    //           If Convert.ToDateTime(drwReservasWeb("fecha_clase")).ToString("dd\MMMM\yyyy") = Now.Date.ToString("dd\MMMM\yyyy") Then
                    //               intReserva = drwReservasWeb("cdreserva")

                    //                If DateTime.Now >= Convert.ToDateTime(drwReservasWeb("fecha_clase")).AddMinutes(-intTiempoAntesAccesoXReservaWeb) And DateTime.Now <= Convert.ToDateTime(drwReservasWeb("fecha_clase")).AddMinutes(intTiempoDespuesAccesoXReservaWeb) Then
                    //                    dtmHoraPrimeraReservaWeb = Convert.ToDateTime(drwReservasWeb("fecha_clase"))
                    //                    blnAutorizaEntradaXReservaWeb = True
                    //                End If

                    //                'Valida si fue impreso el tiquete
                    //                If((drwReservasWeb("estadoImpreso") Is DBNull.Value) OrElse(drwReservasWeb("estadoImpreso") Is Nothing)) Then
                    //                  swReservaImpresa = False
                    //                Else
                    //                    If drwReservasWeb("estadoImpreso").ToString().Trim() = 1 Then
                    //                        swReservaImpresa = True
                    //                    Else
                    //                        swReservaImpresa = False
                    //                    End If
                    //                End If

                    //                If blnImprimirHoraReserva And swReservaImpresa = False Then
                    //                    strClienteReserva = drwReservasWeb("cli_nombres").ToString() + " " + drwReservasWeb("cli_primer_apellido").ToString() + " " + drwReservasWeb("cli_segundo_apellido").ToString()
                    //                    strNombreClaReserva = drwReservasWeb("nombre")
                    //                    If(Convert.ToInt32(drwReservasWeb("megas_utilizadas") = 0)) And(Convert.ToInt32(drwReservasWeb("tiq_utilizados")) = 0) Then
                    //                      intCantMegas = 0
                    //                    ElseIf(Convert.ToInt32(drwReservasWeb("megas_utilizadas")) = 0) Then
                    //                       intCantMegas = Convert.ToInt32(drwReservasWeb("tiq_utilizados"))
                    //                    ElseIf(Convert.ToInt32(drwReservasWeb("tiq_utilizados")) = 0) Then
                    //                       intCantMegas = Convert.ToInt32(drwReservasWeb("megas_utilizadas"))
                    //                    End If
                    //                    strFechaReserva = Convert.ToDateTime(drwReservasWeb("fecha_clase")).ToString("d", culture)
                    //                    strDia = culture.DateTimeFormat.DayNames(Convert.ToDateTime(drwReservasWeb("fecha_clase")).DayOfWeek)
                    //                    strHoraReserva = Convert.ToDateTime(drwReservasWeb("fecha_clase")).TimeOfDay.ToString()
                    //                    strProfesor = drwReservasWeb("emp_nombre").ToString() + " " + drwReservasWeb("emp_primer_apellido").ToString() + " " + drwReservasWeb("emp_segundo_apellido").ToString()
                    //                    strIntensidad = drwReservasWeb("intensidad")
                    //                    strEstado = drwReservasWeb("estado")
                    //                    AddHandler printDoc.PrintPage, AddressOf ImprimirReservasWeb


                    //                    'indicamos que queremos imprimir
                    //                    printDoc.Print()
                    //                    'ds = objAccesoDatos.upImpresionReservasWeb(strRutaBaseDatosMySqlServer, "Actualiza", IDGymsoftWeb, dbvalorGenerado, FechaActual, HoraInicial, HoraFinal, intReserva)
                    //                    'Cambia el estado para que no se imprima nuevamente
                    //                    WS.CambiarEstadoTiqueteImpresoNET(intReserva, IDGymsoftWeb, 1)
                    //                    blnImpresion = True
                    //                End If

                    //                If blnTiqueteClaseAsistido_alImprimir Then
                    //                    WS.CambiarEstadoTiqueteAsistenciaNET(intReserva, IDGymsoftWeb, "Asistio")
                    //                End If
                    //            End If
                    //        Next
                }
            }

            return 0;
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
