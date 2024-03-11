﻿using Sibo.WhiteList.Service.BLL.BLL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Service.Entities.Classes;
using System.Net.NetworkInformation;
using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.BLL.Log;
using System.Data;
using System.Timers;
using Sibo.WhiteList.Service.BLL.Helpers;
using System.Threading;
using Sibo.WhiteList.Service.Classes;
using ImagenTCAM7000;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace Sibo.WhiteList.Service.Classes
{
    public class clsTerminal
    {
        #region Attributes and Objects
        private const int maxNumberTry = 5;
        public clsWinSockCliente winSocketClient;
        public List<clsWinSockCliente> terminalObjectList = new List<clsWinSockCliente>();
        private byte[] objData;
        private byte[][] bdTemplate, arrayImagen;
        private States actualState = States.esperandoIDCliente;
        private States anteriorState;
        bool swRespuestaTramaOK, blnBaseDatosSQL = true, blnMensajeCumpleanos = false, blnComplices_CortxIngs = false, blnComplices_DescuentoTiq = false,
             blnMultiplesPlanesVig = false, blnSolo1HuellaxCliente = false, blnValideContrato = false, blnValideContPorFactura = false,
             blnReplicaImgsTCAM7000 = false, blnValidarMSW = false, blnAccesoDiscapacitados = false,
             blnTrabajarConDBEnOtroEquipo = false, blnPermitirIngresoAdicional = true, blnImagenSIBO = true, blnBloqueoNoCitaMSW = false, blnBloqueoNoAptoMSW = false,
             blnBloqueoNoConsentimiento = false, blnBloqueoNoAutorizacionMenor = false, blnAntipassbackEntrada = false, blnImprimirHoraReserva = false,
             blnTiqueteClaseAsistido_alImprimir = false, blnAccesoXReservaWeb = false, blnAccesoDUAL_Tradicional_y_XReservaWeb = false, blnNoValidarEntradaSalida = true,
             blnEsperarHuellaActualizar = true, blnIngresoAbreDesdeTouch = false, blnNoValidarHuella = false, signContracteToEnroll = false, recordingFromterminal = false,
             generatePDFContractAndSend = false, swIngresoTarjeta = false, blnSirveParaSalida = false, swReplicaImgs = false, bitImagenValida = false,
             allowWhiteListTCAM = false;
        byte[] fingerprintBytes = new byte[1024];
        string strImageFingerprint = string.Empty, strTextoMensajeCumpleanos = "FELIZ CUMPLEAÑOS", strTextoMensajeCortxIngs = "Acumula ingresos. Reclame a la administración una cortesía",
               strRutaImagen = "C:\\GYMSOFT\\Fotos\\", strRutaBanner = "C:\\GYMSOFT\\~BannerForma.swf", strClave = "admingym", strRutaLogo = "C:\\Proyectos\\ingresoTouch\\forma.jpg",
               strColorPpal = "#527BE7", strRutaGuiaMarcacion = "C:\\Proyectos\\ingresoTouch\\forma.jpg",
               generalUserIdToSigningContract = string.Empty, fingerprintsUsed = string.Empty, userUsed = string.Empty, fingerprintDeleteUsed = string.Empty,
               usersDeleteUsed = string.Empty, ipAddressDownload = string.Empty, events = string.Empty;
        public bool swMultiplesPlanesVigTCAM7000;
        int pingTime = 3000, timeTerminalConnections = 5000, intComplices_Plan_CortxIngs = 0, intTiempoReplicaImgsTCAM7000 = 3600000,
            intTiempoEspaciadoTramasReplicaImgsTCAM7000 = 5, intTiempoMaximoEnvioImgsTCAM7000 = 120, tmlimpiarPantalla = 7000, intTIEMPO_ACTUALIZA_INGRESOS = 5000,
            intTiempoRestarTiquetes = 2, intTiempoPulso = 9, intTiempoAntesAccesoXReservaWeb = 0, intTiempoDespuesAccesoXReservaWeb = 0,
            fingerId = 0, gymId = 0, branchId = 0, intTiempoReplicaHuellasTCAM7000 = 30000,
            intTiempoEsperaRespuestaReplicaHuellasTCAM7000 = 6000, intTamanoImagen = 1024, index = 0, intIndiceHuellaRepAct = 0, indexUsersWhiteList = 0, huellaContinua = 0,
            intHuellasPorTrama = 3, timeWaitResponseReplicateUsers = 6000, indexFingerprintsDelete = 0, timeWaitResponseDeleteFingerprint = 6000,
            intCantidadComandosEnviar = 0, intLineasImagenEnviadas = 0, timeGetPendingActions = 2000, timeRemoveFingerprints = 60000, oneSecond = 1000,
            timeWhitelistTCAM = 60000, timeRemoveUsers = 60000, indexUsersWhiteListDelete = 0, timeWaitResponseDeleteUser = 6000,
            terminalCounterReplicateFingerprint = 0, terminalCounterDeleteFingerprint = 0, terminalCounterReplicateUser = 0,
            terminalCounterDeleteUser = 0, timeDowndloadEvents = 60000, terminalCounterDownloadEvents = 0, timeResetDownloadEvents = 15000,
            timeHourSync = 32400000, terminalCounterHourSync = 0, indexReplicateUsers = 0, startIndexReplicateUsers = 0, indexRemoveUsers = 0, startIndexRemoveUsers = 0,
            indexRemoveFingerprints = 0, startIndexRemoveFingerprints = 0, indexReplicateFingerprints = 0, startIndexReplicateFingerprints = 0,
            indexDownloadEvents = 0, startIndexDownloadEvents = 0;
        List<eTerminal> termList = new List<eTerminal>();
        private System.Timers.Timer tmPingContinuoTCAM7000, timerGetPendingActions, timerTerminalConnections, tmReplicaHuellasTCAM7000, tmEsperaRespuestaTCAM7000,
                                    timerRemoveFingerprints, tmReplicaImgsTCAM7000, tmWhiteListTCAM, tmWaitResponseReplicateUsers, tmWaitResponseDeleteFingerprints,
                                    timerRemoveUsers, tmWaitResponseDeleteUser, tmDownloadEvents, tmResetDownloadEvents, tmHourSync;
        const string terminalConnected = "TERMINAL CONECTADA";
        const string terminalDisconnected = "TERMINAL DESCONECTADA";
        ServiceLog log = new ServiceLog();
        int[] dbTemplateSize;
        DataTable dtHuellas = new DataTable();
        clsEnum.ServiceActions actionService = clsEnum.ServiceActions.WaitingClientId, lastActionService;
        int timeoutOpening = 12000;
        Suprema.UFMatcher usComparadorUSB = new Suprema.UFMatcher();
        List<eFingerprint> fingerList = new List<eFingerprint>();
        List<eFingerprint> fingerListDelete = new List<eFingerprint>();
        List<eUser> userList = new List<eUser>();
        List<eUser> userListDelete = new List<eUser>();
        DataTable dtImgs = new DataTable();
        DataTable dtReplicatedUsers = new DataTable();
        DataTable dtDeletedUsers = new DataTable();
        List<int> listFingerprintsDelete = new List<int>();
        public const int CANTIDAD_TOTAL_COMANDOS_IMAGEN_TCAM7000_480X320 = 640;
        public const int CANTIDAD_TOTAL_COMANDOS_IMAGEN_TCAM7000_480X640 = 1280;
        #endregion

        public class eTerminalAction
        {
            public string ipAddress { get; set; }
            public int actionToMake { get; set; }
            public int actionfinished { get; set; }
        }

        public class eUserRecordFingerprint
        {
            public string userId { get; set; }
            public int quality { get; set; }
            public int fingerprintId { get; set; }
            public string ipAddress { get; set; }
        }

        List<eTerminalAction> listExecutionTerminal = new List<eTerminalAction>();
        List<eUserRecordFingerprint> listUserRecord = new List<eUserRecordFingerprint>();

        public enum SocketState
        {
            Ninguno,
            Ping,
            ConectandoSocket,
            LiberandoSocket,
        }

        public enum States
        {
            ninguno,
            esperandoIDCliente,
            esperandoConfirmacion,
            replicandoApertura,
            replicandoNoApertura,
            esperandoConfirmacionHuella,
            esperandoHuella,
            esperandoImagenHuella,
            esperandoConfirmacionPlanesVig,
            esperandoPlanVigente,
            esperandoConfirmacionIngSinHuella,
            esperandoConfirmacionReplicaHuella,
            esperandoConfirmacionReplicaImgs,
            esperandoTramaConexion,
            consultandoReloj,
            consultandoCantidadHuella,
            espera,
            entradaRegistrada,
            salidaRegistrada,
            leQuedanDias,
            LeQuedanTiquetes,
            desbloqueoTerminal,
            errorHuella,
            errorIdentificacion,
            errorIdentificacionRegistro,
            registro,
            registrarNuevoUsuario,
            registrarOtroUsuario,
            identificacion,
            reemplazarHuella,
            noTienePlanAdquirido,
            accesoDenegado,
            desactivarModoLibre,
            ActivarModoLibre,
            NoContemplaEsteDia,
            NoContemplaEstaHora,
            BorrandoHuellaReplicacion,
            ReplicandoHuella,
            CerrandoPuente,
            esperandoHuellaTerminal,
            IngresandoMenuTelnet,
            esperandoHuellaActualizar,
            esperandoCodigoError
        }

        /// <summary>
        /// Método que se encarca de consultar las terminales registradas para una sucursal específica en GSW e ingresarlas en la BD local "Lista blanca".
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public bool GetTerminal(int gymId, int branchId)
        {
            TerminalBLL termBLL = new TerminalBLL();
            return termBLL.GetTerminals(gymId, branchId);
        }

        /// <summary>
        /// Método encargado de consultar las terminales registradas en la BD local para pasar a conectarlas.
        /// Getulio Vargas - OD 1307
        /// </summary>
        public void OpenSockets()
        {
            try
            {
                string strGymId = System.Configuration.ConfigurationManager.AppSettings["gymId"].ToString();
                string strBranchId = System.Configuration.ConfigurationManager.AppSettings["branchId"].ToString();

                if (!string.IsNullOrEmpty(strGymId) && !string.IsNullOrEmpty(strBranchId))
                {
                    gymId = Convert.ToInt32(strGymId);
                    branchId = Convert.ToInt32(strBranchId);
                }
                else
                {
                    log.WriteProcess("No se ha configurado de forma correcta el código del gimnasio y/o la sucursal.");
                }

                timeoutOpening = 12000;

                TerminalBLL termBLL = new TerminalBLL();
                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                eConfiguration config = new eConfiguration();
                log.WriteProcess("Inicia el proceso de consulta de configuración en BD local, para fijar valor de parámetros en ejecución.");
                config = acsBll.GetLocalAccessControlSettings();

                if (config != null)
                {
                    FillConfigurationParameters(config);
                }
                else
                {
                    log.WriteProcess("No se encontró configuración guardada u ocurrió alguna novedad en el proceso de consulta.");
                }

                log.WriteProcess("Finaliza proceso de consulta de configuración en BD local, para fijar valor de parámetros en ejecución.");
                log.WriteProcess("Inicia el proceso de consulta de terminales en BD local.");
                termList = termBLL.GetTerminals();
                log.WriteProcess("Finaliza proceso de consulta de terminales en BD local.");
                log.WriteProcess("Inicia el proceso de de conexión de las terminales.");
                ConnectTCAM();
                log.WriteProcess("Finaliza el proceso de de conexión de las terminales.");
                //tmPingContinuoTCAM7000 = new System.Timers.Timer(pingTime);
                //tmPingContinuoTCAM7000.Elapsed += TmPingContinuoTCAM7000_Elapsed;
                //tmPingContinuoTCAM7000.Enabled = true;
                //tmPingContinuoTCAM7000.Start();

                timerGetPendingActions = new System.Timers.Timer(timeGetPendingActions);
                timerGetPendingActions.Elapsed += timerGetPendingActions_Elapsed;
                timerGetPendingActions.AutoReset = true;
                timerGetPendingActions.Enabled = true;
                timerGetPendingActions.Start();

                //Timer que se encargará de revisar qué terminales no están conectadas y las vuelve a conectar.
                timerTerminalConnections = new System.Timers.Timer(timeTerminalConnections);
                timerTerminalConnections.Elapsed += TimerTerminalConnections_Elapsed;
                timerTerminalConnections.Enabled = true;
                timerTerminalConnections.Start();

                //Iniciamos el timer de réplica de huellas a las terminales.
                tmReplicaHuellasTCAM7000 = new System.Timers.Timer(intTiempoReplicaHuellasTCAM7000);
                tmReplicaHuellasTCAM7000.Elapsed += TmReplicaHuellasTCAM7000_Elapsed;
                tmReplicaHuellasTCAM7000.Enabled = true;
                tmReplicaHuellasTCAM7000.Start();

                tmEsperaRespuestaTCAM7000 = new System.Timers.Timer(intTiempoEsperaRespuestaReplicaHuellasTCAM7000);
                tmEsperaRespuestaTCAM7000.Elapsed += TmEsperaRespuestaTCAM7000_Elapsed;

                tmWaitResponseReplicateUsers = new System.Timers.Timer(timeWaitResponseReplicateUsers);
                tmWaitResponseReplicateUsers.Elapsed += TmWaitResponseReplicateUsers_Elapsed;

                tmWaitResponseDeleteFingerprints = new System.Timers.Timer(timeWaitResponseDeleteFingerprint);
                tmWaitResponseDeleteFingerprints.Elapsed += TmWaitResponseDeleteFingerprints_Elapsed;

                tmWaitResponseDeleteUser = new System.Timers.Timer(timeWaitResponseDeleteUser);
                tmWaitResponseDeleteUser.Elapsed += TmWaitResponseDeleteUser_Elapsed;

                tmResetDownloadEvents = new System.Timers.Timer(timeResetDownloadEvents);
                tmResetDownloadEvents.Elapsed += TmResetDownloadEvents_Elapsed;

                //Iniciamos el timer de borrado de huellas de las terminales
                timerRemoveFingerprints = new System.Timers.Timer(timeRemoveFingerprints);
                timerRemoveFingerprints.Elapsed += TimerRemoveFingerprints_Elapsed;
                timerRemoveFingerprints.Enabled = true;
                timerRemoveFingerprints.Start();

                //INICIAMOS EL TIMER DE BORRADO DE USUARIOS DE LAS TERMINALES
                timerRemoveUsers = new System.Timers.Timer(timeRemoveUsers);
                timerRemoveUsers.Elapsed += TimerRemoveUsers_Elapsed;
                timerRemoveUsers.Enabled = true;
                timerRemoveUsers.Start();

                tmWhiteListTCAM = new System.Timers.Timer(timeWhitelistTCAM);
                tmWhiteListTCAM.Elapsed += TmWhiteListTCAM_Elapsed;
                tmWhiteListTCAM.Enabled = true;
                tmWhiteListTCAM.Start();

                tmDownloadEvents = new System.Timers.Timer(timeDowndloadEvents);
                tmDownloadEvents.Elapsed += TmDownloadEvents_Elapsed;
                tmDownloadEvents.Enabled = true;
                tmDownloadEvents.Start();

                tmHourSync = new System.Timers.Timer(timeHourSync);
                tmHourSync.Elapsed += TmHourSync_Elapsed;
                tmHourSync.Enabled = true;
                tmHourSync.Start();

                //Validamos si se debe iniciar el timer de réplica de imágenes
                if (blnReplicaImgsTCAM7000)
                {
                    tmReplicaImgsTCAM7000 = new System.Timers.Timer(intTiempoReplicaImgsTCAM7000);
                    tmReplicaImgsTCAM7000.Elapsed += TmReplicaImgsTCAM7000_Elapsed;
                    tmReplicaImgsTCAM7000.Enabled = true;
                    tmReplicaImgsTCAM7000.Start();
                }

                InitialHourSync();
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores de las terminales.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                actionService = clsEnum.ServiceActions.WaitingClientId;
            }
        }

        private void InitialHourSync()
        {
            if (terminalObjectList.Count > 0)
            {
                foreach (clsWinSockCliente item in terminalObjectList)
                {
                    if (item.terminalTypeId == 1)
                    {
                        Thread syncThread = new Thread(() => HourSync(item.IpAddress));
                        syncThread.Start();
                    }
                }
            }
        }

        private void TmResetDownloadEvents_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (terminalObjectList.Find(tol => tol.replicating == true) != null)
            {
                DownloadEvents(terminalObjectList.Find(tol => tol.replicating == true).IpAddress, true, false, false);
            }
            else
            {
                tmDownloadEvents.Start();
                actionService = clsEnum.ServiceActions.WaitingClientId;
            }
        }

        private void TmWaitResponseDeleteUser_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (terminalObjectList.Find(tol => tol.replicating == true) != null)
            {
                DeleteUsers(terminalObjectList.Find(tol => tol.replicating == true).IpAddress, true, false);
            }
            else
            {
                timerRemoveUsers.Start();
                actionService = clsEnum.ServiceActions.WaitingClientId;
            }
        }

        private void TmWaitResponseDeleteFingerprints_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (terminalObjectList.Find(tol => tol.replicating == true) != null)
            {
                DeleteFingerprints(terminalObjectList.Find(tol => tol.replicating == true).IpAddress, true, false);
            }
            else
            {
                tmWaitResponseDeleteFingerprints.Stop();
                timerRemoveFingerprints.Start();
            }
        }

        private void TmWaitResponseReplicateUsers_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (terminalObjectList.Find(tol => tol.replicating == true) != null)
            {
                ReplicateUsersWhitelist(terminalObjectList.Find(tol => tol.replicating == true).IpAddress, true, false);
            }
            else
            {
                tmWaitResponseReplicateUsers.Stop();
                tmWhiteListTCAM.Start();
            }
        }

        private void TmHourSync_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (allowWhiteListTCAM)
            {
                ValidateHourSync();
            }
        }

        private void ValidateHourSync()
        {
            if (terminalObjectList.Count > 0)
            {
                foreach (clsWinSockCliente item in terminalObjectList)
                {
                    if (!item.replicating && item.terminalTypeId == 1)
                    {
                        bool whitelistTerminal = terminalObjectList.Find(tol => tol.IpAddress == item.IpAddress).withWithelist;

                        if (whitelistTerminal)
                        {
                            terminalCounterHourSync++;
                            terminalObjectList.Find(tol => tol.IpAddress == item.IpAddress).replicating = true;
                            Task.Run(() => HourSync(item.IpAddress));
                        }
                        else
                        {
                            terminalCounterHourSync = 0;
                        }
                    }
                }
            }
        }

        private void HourSync(string ipAddress)
        {
            tmHourSync.Stop();
            string command = "@T" + DateTime.Now.ToString("HH:mm:ss dd/MM/yy") + "$";
            log.WriteProcessByTerminal("PROCEDEMOS A SINCRONIZAR LA HORA - DATO: " + command, ipAddress);
            actionService = clsEnum.ServiceActions.waitingConfirmHourSync;
            ExecuteCommand(ipAddress, command);
        }

        private void TmDownloadEvents_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (allowWhiteListTCAM)
            {
                if (terminalObjectList.Count > 0)
                {
                    int i = 0;
                    foreach (clsWinSockCliente item in terminalObjectList)
                    {
                        if (ValidateTerminalStateDownloadEvents(item, true) && item.terminalTypeId == 1)
                        {
                            startIndexReplicateUsers = i;
                            indexReplicateUsers = i;
                            terminalCounterDownloadEvents++;
                            item.replicating = true;
                            ControlDownloadEvents(item, false, true);
                            break;
                        }

                        i++;
                    }
                }
            }
        }

        private void ControlDownloadEvents(clsWinSockCliente actualTerminal, bool validateHour, bool firstTime)
        {
            if (firstTime)
            {
                log.WriteProcess("INICIA EL PROCESO DE DESCARGA DE MARCACIONES DE LAS TERMINALES.");
                DownloadEvents(actualTerminal.IpAddress, false, false, false);
            }
            else
            {
                if (indexDownloadEvents == startIndexDownloadEvents)
                {
                    log.WriteProcess("FINALIZA EL PROCESO DE DESCARGA DE MARCACIONES DE LAS TERMINALES.");
                    terminalCounterDownloadEvents = 0;
                    startIndexDownloadEvents = 0;
                    indexDownloadEvents = 0;
                    events = string.Empty;
                    quantityEvents = 0;
                    tmDownloadEvents.Enabled = true;
                    tmDownloadEvents.Start();
                }
                else
                {
                    if (ValidateTerminalStateDownloadEvents(actualTerminal, true))
                    {
                        terminalCounterDownloadEvents++;
                        DownloadEvents(actualTerminal.IpAddress, false, false, false);
                    }
                    else
                    {
                        if ((indexDownloadEvents + 1) < terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                        {
                            indexDownloadEvents++;
                            ControlDownloadEvents(terminalObjectList[indexDownloadEvents], true, false);
                            return;
                        }
                        else if (startIndexDownloadEvents > 0)
                        {
                            indexDownloadEvents = 0;
                            ControlDownloadEvents(terminalObjectList[indexDownloadEvents], true, false);
                            return;
                        }
                        else if ((indexDownloadEvents + 1) == terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                        {
                            log.WriteProcess("FINALIZA EL PROCESO DE DESCARGA DE MARCACIONES DE LAS TERMINALES.");
                            terminalCounterDownloadEvents = 0;
                            startIndexDownloadEvents = 0;
                            indexDownloadEvents = 0;
                            events = string.Empty;
                            quantityEvents = 0;
                            tmDownloadEvents.Enabled = true;
                            tmDownloadEvents.Start();
                        }
                    }
                }
            }
        }

        private bool ValidateTerminalStateDownloadEvents(clsWinSockCliente actualTerminal, bool validateHour)
        {
            bool response = false;

            if (actualTerminal.withWithelist)
            {
                if ((validateHour && ValidateHourDownloadEvents(actualTerminal.IpAddress)) || !validateHour)
                {
                    if (!actualTerminal.replicating)
                    {
                        response = true;
                    }
                }
            }

            return response;
        }

        private bool ValidateHourDownloadEvents(string ipAddress)
        {
            ScheduleBLL srfBll = new ScheduleBLL();
            DataTable dtSchedules = new DataTable();
            dtSchedules = srfBll.GetEventSchedules();
            bool execute = false;

            if (dtSchedules != null)
            {
                if (dtSchedules.Rows.Count > 0)
                {
                    foreach (DataRow row in dtSchedules.Rows)
                    {
                        if (Convert.ToDateTime(row["executionHour"]).Hour == DateTime.Now.Hour && Convert.ToDateTime(row["executionHour"]).Minute == DateTime.Now.Minute &&
                            row["ipAddress"].ToString() == ipAddress)
                        {
                            execute = true;
                        }
                    }
                }
            }

            return execute;
        }

        public bool ValidateHourReplicateUsers(string ipAddress)
        {
            ScheduleBLL srfBll = new ScheduleBLL();
            DataTable dtSchedules = new DataTable();
            dtSchedules = srfBll.GetReplicateUserSchedules();
            bool execute = false;

            if (dtSchedules != null)
            {
                if (dtSchedules.Rows.Count > 0)
                {
                    foreach (DataRow row in dtSchedules.Rows)
                    {
                        if (Convert.ToDateTime(row["executionHour"]).Hour == DateTime.Now.Hour && Convert.ToDateTime(row["executionHour"]).Minute == DateTime.Now.Minute &&
                            row["ipAddress"].ToString() == ipAddress)
                        {
                            execute = true;
                        }
                    }
                }
            }

            return execute;
        }

        private void DownloadEvents(string ipAddress, bool downloadState, bool downloadOk, bool confirmDeleteTerminalEvents)
        {
            string[] eventsArray = null, arrayEvent = null;
            bool isExit = termList.Find(t => t.ipAddress == ipAddress).servesToOutput;

            if (!confirmDeleteTerminalEvents)
            {
                if (!downloadState)
                {
                    log.WriteProcessByTerminal("INICIA LA DESCARGA DE MARCACIONES DE LA TERMINAL.", ipAddress);
                    tmDownloadEvents.Stop();
                }
                else
                {
                    Thread.Sleep(600);

                    if (downloadOk && !string.IsNullOrEmpty(events))
                    {
                        DataTable dtTerminalEvents = new DataTable();
                        EventBLL eventBLL = new EventBLL();

                        foreach (PropertyInfo info in typeof(eTerminalEvents).GetProperties())
                        {
                            dtTerminalEvents.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
                        }

                        log.WriteProcessByTerminal("SE DESCARGARON " + quantityEvents.ToString() + " EVENTOS DE LA TERMINAL", ipAddress);
                        eventsArray = events.Split('\n');

                        if (eventsArray != null)
                        {
                            foreach (string item in eventsArray)
                            {
                                if (item.Length == 31)
                                {
                                    arrayEvent = item.Split(',');
                                    CultureInfo us = new CultureInfo("en-US");

                                    if (arrayEvent != null)
                                    {
                                        DataRow row = dtTerminalEvents.NewRow();
                                        row["ipAddres"] = ipAddress;
                                        row["date"] = DateTime.ParseExact(arrayEvent[0].ToString(), "yy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                        row["userId"] = Convert.ToInt64(arrayEvent[1].ToString().Substring(0, arrayEvent[1].ToString().Length - 1)).ToString();
                                        row["isExit"] = isExit;
                                        dtTerminalEvents.Rows.Add(row);
                                    }
                                }
                            }
                        }

                        if (dtTerminalEvents != null)
                        {
                            if (dtTerminalEvents.Rows.Count > 0)
                            {
                                log.WriteProcessByTerminal("SE PROCEDE A INSERTAR LOS " + quantityEvents.ToString() + " EVENTOS DE LA TERMINAL EN LA BASE DE DATOS", ipAddress);

                                if (eventBLL.InsertTerminalEvents(dtTerminalEvents))
                                {
                                    log.WriteProcessByTerminal("SE INSERTARON CORRECTAMENTE LOS EVENTOS DE LA TERMINAL EN LA BASE DE DATOS.", ipAddress);
                                    log.WriteProcessByTerminal("PROCEDEMOS A BORRAR LOS EVENTOS DE LA TERMINAL", ipAddress);
                                    GenerateCommandToDeleteEvents(ipAddress);
                                    events = string.Empty;
                                    return;
                                }
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(events))
                {
                    if (GenerateCommandToDownloadEvents(ipAddress) != -1)
                    {
                        tmResetDownloadEvents.Enabled = true;
                        tmResetDownloadEvents.Start();
                    }

                    return;
                }
            }
            else
            {
                log.WriteProcessByTerminal("FINALIZA DESCARGA DE MARCACIONES DE LA TERMINAL.", ipAddress);
                terminalObjectList.Find(tol => tol.IpAddress == ipAddress).replicating = false;

                if (terminalCounterDownloadEvents == terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                {
                    terminalCounterDownloadEvents = 0;
                    startIndexDownloadEvents = 0;
                    indexDownloadEvents = 0;
                }
                else
                {
                    if ((indexDownloadEvents + 1) < terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                    {
                        indexDownloadEvents++;
                    }
                    else if (startIndexDownloadEvents > 0)
                    {
                        indexDownloadEvents = 0;
                    }

                    ControlDownloadEvents(terminalObjectList[indexDownloadEvents], true, false);
                    return;
                }
            }

            tmDownloadEvents.Start();
        }

        private void GenerateCommandToDeleteEvents(string ipAddress)
        {
            string command = "@MB$";
            actionService = clsEnum.ServiceActions.waitingConfirmDeleteEvents;
            SendDataAndLogs(command, ipAddress);
        }

        private int GenerateCommandToDownloadEvents(string ipAddress)
        {
            if (!string.IsNullOrEmpty(ipAddress))
            {
                string command = "@MD$";
                actionService = clsEnum.ServiceActions.waitingConfirmDownloadEvents;
                Thread.Sleep(oneSecond);
                SendDataAndLogs(command, ipAddress);
                return 0;
            }
            else
            {
                log.WriteProcessByTerminal("NO SE ENVIÓ LA DIRECCIÓN DE LA TERMINAL. NO SERÁ POSIBLE DESCARGAR LOS EVENTOS.", ipAddress);
                return -1;
            }
        }

        private void TmWhiteListTCAM_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (allowWhiteListTCAM)
            {
                if (terminalObjectList.Count > 0)
                {
                    dtReplicatedUsers = new DataTable();

                    foreach (PropertyInfo info in typeof(eReplicatedUser).GetProperties())
                    {
                        dtReplicatedUsers.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
                    }

                    int i = 0;
                    foreach (clsWinSockCliente item in terminalObjectList)
                    {
                        if (ValidateTerminalStateReplicateUsers(item, true) && item.terminalTypeId == 1)
                        {
                            startIndexReplicateUsers = i;
                            indexReplicateUsers = i;
                            terminalCounterReplicateUser++;
                            item.replicating = true;
                            ControlReplicateUsers(item, false, true);
                            break;
                        }

                        i++;
                    }
                }
            }
        }

        public bool ValidateTerminalStateReplicateUsers(clsWinSockCliente actualTerminal, bool validateHour)
        {
            bool response = false;

            if (actualTerminal.withWithelist)
            {
                if ((validateHour && ValidateHourReplicateUsers(actualTerminal.IpAddress)) || !validateHour)
                {
                    if (!actualTerminal.replicating)
                    {
                        response = true;
                    }
                    //else
                    //{
                    //    log.WriteProcessByTerminal("NO ES POSIBLE REPLICAR USUARIOS A LA TERMINAL YA QUE LA TERMINAL ESTÁ REALIZANDO UN PROCESO DE SINCRONIZACIÓN.", actualTerminal.IpAddress);
                    //}
                }
            }
            //else
            //{
            //    log.WriteProcessByTerminal("LA TERMINAL NO ESTÁ HABILITADA PARA ALMACENAR USUARIOS DE LISTA BLANCA.", actualTerminal.IpAddress);
            //}

            return response;
        }

        private void ControlReplicateUsers(clsWinSockCliente actualTerminal, bool validateHour, bool firstTime)
        {
            if (firstTime)
            {
                log.WriteProcess("INICIA EL PROCESO DE RÉPLICA DE USUARIOS A LAS TERMINALES.");
                ReplicateUsersWhitelist(actualTerminal.IpAddress, false, false);
            }
            else
            {
                if (indexReplicateUsers == startIndexReplicateUsers)
                {
                    log.WriteProcess("FINALIZA EL PROCESO DE RÉPLICA DE USUARIOS A LAS TERMINALES.");
                    terminalCounterReplicateUser = 0;
                    startIndexReplicateUsers = 0;
                    indexReplicateUsers = 0;
                    indexUsersWhiteList = 0;
                    tmWhiteListTCAM.Enabled = true;
                    tmWhiteListTCAM.Start();
                }
                else
                {
                    if (ValidateTerminalStateReplicateUsers(actualTerminal, true))
                    {
                        terminalCounterReplicateUser++;
                        ReplicateUsersWhitelist(actualTerminal.IpAddress, false, false);
                    }
                    else
                    {
                        if ((indexReplicateUsers + 1) < terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                        {
                            indexReplicateUsers++;
                            ControlReplicateUsers(terminalObjectList[indexReplicateUsers], true, false);
                            return;
                        }
                        else if (startIndexReplicateUsers > 0)
                        {
                            indexReplicateUsers = 0;
                            ControlReplicateUsers(terminalObjectList[indexReplicateUsers], true, false);
                            return;
                        }
                        else if ((indexReplicateUsers + 1) == terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                        {
                            log.WriteProcess("FINALIZA EL PROCESO DE RÉPLICA DE USUARIO EN LAS TERMINALES.");
                            terminalCounterReplicateUser = 0;
                            startIndexReplicateUsers = 0;
                            indexReplicateUsers = 0;
                            indexUsersWhiteList = 0;
                            tmWhiteListTCAM.Enabled = true;
                            tmWhiteListTCAM.Start();
                        }
                    }
                }
            }
        }

        private void ReplicateUsersWhitelist(string ipAddress, bool replyState, bool replyOk)
        {
            int quantityUsers = 0, counterUsers = 0, quantityAux = 0, genericQuantity = 20;
            string userId = string.Empty, userName = string.Empty;
            bool wWithList = false;
            UserBLL userBLL = new UserBLL();
            ReplicatedUserBLL replicatedUserBLL = new ReplicatedUserBLL();

            if (!replyState)
            {
                log.WriteProcessByTerminal("INICIA RÉPLICA DE USUARIOS DE LISTA BLANCA A LA TERMINAL", ipAddress);
                tmWhiteListTCAM.Stop();
                userList = new List<eUser>();
                userList = userBLL.GetUsersToReplicate(ipAddress);
            }
            else
            {
                Thread.Sleep(600);

                if (replyOk && dtReplicatedUsers != null)
                {
                    if (dtReplicatedUsers.Rows.Count > 0)
                    {
                        string userIds = string.Empty;

                        foreach (DataRow row in dtReplicatedUsers.Rows)
                        {
                            if (string.IsNullOrEmpty(userIds))
                            {
                                userIds = row["userId"].ToString();
                            }
                            else
                            {
                                userIds += "," + row["userId"].ToString();
                            }
                        }

                        log.WriteProcessByTerminal("RÉPLICA DE USUARIOS: " + userIds, ipAddress);

                        if (replicatedUserBLL.InsertTable(dtReplicatedUsers))
                        {
                            log.WriteProcessByTerminal("INSERTA EN BD RÉPLICA DE LOS USUARIOS: " + userIds, ipAddress);
                            dtReplicatedUsers = new DataTable();

                            foreach (PropertyInfo info in typeof(eReplicatedUser).GetProperties())
                            {
                                dtReplicatedUsers.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
                            }

                            counterUsers++;
                        }
                    }
                }
            }

            if (userList != null && userList.Count > 0)
            {
                if (!replyState)
                {
                    quantityUsers = userList.Count;
                    log.WriteProcessByTerminal("CANTIDAD DE USUARIOS SIN REPLICAR A TERMINAL: " + quantityUsers.ToString(), ipAddress);
                }

                if (indexUsersWhiteList < userList.Count)
                {
                    List<eUsersToReplicate> usersToReplicateList = new List<eUsersToReplicate>();
                    int counterAux = 0;

                    while (indexUsersWhiteList < userList.Count && counterAux < genericQuantity)
                    {
                        eUsersToReplicate usersToReplicate = new eUsersToReplicate()
                        {
                            delete = userList[indexUsersWhiteList].delete,
                            fingerprintId = userList[indexUsersWhiteList].fingerprintId,
                            insert = userList[indexUsersWhiteList].insert,
                            ipAddress = ipAddress,
                            recordId = userList[indexUsersWhiteList].id,
                            userId = userList[indexUsersWhiteList].userId,
                            userName = userList[indexUsersWhiteList].userName,
                            withoutFingerprint = userList[indexUsersWhiteList].withoutFingerprint
                        };

                        if (!userUsed.Contains("," + usersToReplicate.recordId.ToString() + ","))
                        {
                            if (string.IsNullOrEmpty(userUsed))
                            {
                                userUsed = usersToReplicate.recordId.ToString();
                            }
                            else
                            {
                                userUsed += "," + usersToReplicate.recordId.ToString();
                            }
                        }

                        DataRow rowUser = dtReplicatedUsers.NewRow();
                        rowUser = GetRowUser(usersToReplicate);
                        dtReplicatedUsers.Rows.Add(rowUser);
                        usersToReplicateList.Add(usersToReplicate);
                        indexUsersWhiteList++;
                        counterAux++;
                    }

                    if (GenerateCommandToReplicateUsers(usersToReplicateList, ipAddress) != -1)
                    {
                        tmWaitResponseReplicateUsers.Enabled = true;
                        tmWaitResponseReplicateUsers.Start();
                    }
                    else
                    {
                        log.WriteProcessByTerminal("DESCARTA RÉPLICA DEl USUARIO ID " + userId, ipAddress);
                        counterUsers++;
                        ReplicateUsersWhitelist(ipAddress, true, false);
                    }

                    quantityAux++;
                    return;
                }
                else
                {
                    log.WriteProcessByTerminal("FINALIZA RÉPLICA DE USUARIOS DE LISTA BLANCA A LA TERMINAL.", ipAddress);
                    terminalObjectList.Find(tol => tol.IpAddress == ipAddress).replicating = false;
                    actionService = clsEnum.ServiceActions.WaitingClientId;

                    if (terminalCounterReplicateUser == terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                    {
                        if (!string.IsNullOrEmpty(userUsed))
                        {
                            if (userBLL.UpdateUsedUsers(userUsed))
                            {
                                userUsed = string.Empty;
                            }
                        }

                        terminalCounterReplicateUser = 0;
                        startIndexReplicateUsers = 0;
                        indexReplicateUsers = 0;
                    }
                    else
                    {
                        if ((indexReplicateUsers + 1) < terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                        {
                            indexReplicateUsers++;
                            ControlReplicateUsers(terminalObjectList[indexReplicateUsers], true, false);
                            return;
                        }
                        else if (startIndexReplicateUsers > 0)
                        {
                            indexReplicateUsers = 0;
                            ControlReplicateUsers(terminalObjectList[indexReplicateUsers], true, false);
                            return;
                        }
                    }
                }
            }
            else
            {
                log.WriteProcessByTerminal("NO EXISTEN USUARIOS SIN REPLICAR A LA TERMINAL ", ipAddress);
                terminalObjectList.Find(tol => tol.IpAddress == ipAddress).replicating = false;
                actionService = clsEnum.ServiceActions.WaitingClientId;

                if (terminalCounterReplicateUser == terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                {
                    if (!string.IsNullOrEmpty(userUsed))
                    {
                        if (userBLL.UpdateUsedUsers(userUsed))
                        {
                            userUsed = string.Empty;
                        }
                    }

                    terminalCounterReplicateUser = 0;
                    startIndexReplicateUsers = 0;
                    indexReplicateUsers = 0;
                }
                else
                {
                    if ((indexReplicateUsers + 1) < terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                    {
                        indexReplicateUsers++;
                        ControlReplicateUsers(terminalObjectList[indexReplicateUsers], true, false);
                        return;
                    }
                    else if (startIndexReplicateUsers > 0)
                    {
                        indexReplicateUsers = 0;
                        ControlReplicateUsers(terminalObjectList[indexReplicateUsers], true, false);
                        return;
                    }
                }
            }

            if (quantityAux > 0)
            {
                log.WriteProcessByTerminal("CANTIDAD DE USUARIOS REPLICADOS A TERMINAL: " + quantityAux.ToString(), ipAddress);
            }

            indexUsersWhiteList = 0;
            tmWhiteListTCAM.Start();
        }

        private DataRow GetRowUser(eUsersToReplicate usersToReplicate)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                DataRow row = dtReplicatedUsers.NewRow();
                row["userId"] = usersToReplicate.userId;
                row["fingerprintId"] = usersToReplicate.fingerprintId;
                row["ipAddress"] = usersToReplicate.ipAddress;
                return row;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        private int GenerateCommandToReplicateUsers(List<eUsersToReplicate> userList, string ipAddress)
        {
            string command = string.Empty;

            if (userList.Count > 0)
            {
                command = "@L00" + userList.Count.ToString().PadLeft(2, '0') + ";";

                foreach (eUsersToReplicate item in userList)
                {
                    if (command.Length > 7)
                    {
                        command += ";";
                    }

                    if (item.insert)
                    {
                        command += "I-";
                    }
                    else
                    {
                        command += "E-";
                    }

                    command += item.userId.PadLeft(12, '0') + "-" + item.fingerprintId.ToString().PadLeft(10, '0') + "-" + item.userName.ToUpper().PadRight(20, ' ') + "-" +
                               (item.withoutFingerprint == true ? "1" : "0");
                }

                command += ";$";
                actionService = clsEnum.ServiceActions.waitingConfirmUserReplicate;
                Thread.Sleep(1000);
                SendDataAndLogs(command, ipAddress);
                log.WriteProcessByTerminal("Usuarios a replicar - " + command, ipAddress);
                return 0;
            }
            else
            {
                return -1;
            }
        }

        private int GenerateCommandToDeleteUsers(List<eUsersToReplicate> userList, string ipAddress)
        {
            string command = string.Empty;

            if (userList.Count > 0)
            {
                command = "@L00" + userList.Count.ToString().PadLeft(2, '0') + ";";

                foreach (eUsersToReplicate item in userList)
                {
                    if (command.Length > 7)
                    {
                        command += ";";
                    }

                    if (item.delete)
                    {
                        command += "E-";
                    }
                    else
                    {
                        command += "I-";
                    }

                    command += item.userId.PadLeft(12, '0') + "-" + item.fingerprintId.ToString().PadLeft(10, '0') + "-" + item.userName.ToUpper().PadRight(20, ' ') + "-" +
                               (item.withoutFingerprint == true ? "0" : "1");
                }

                command += ";$";
                actionService = clsEnum.ServiceActions.waitingConfirmUserDelete;
                SendDataAndLogs(command, ipAddress);
                log.WriteProcessByTerminal("USUARIOS A ELIMINAR - " + command, ipAddress);
                return 0;
            }
            else
            {
                return -1;
            }
        }

        private void TmReplicaImgsTCAM7000_Elapsed(object sender, ElapsedEventArgs e)
        {
            ValidateConnectedTerminalsToReplicateImgs();
        }

        private void ValidateConnectedTerminalsToReplicateImgs()
        {
            if (terminalObjectList.Count > 0)
            {
                foreach (clsWinSockCliente item in terminalObjectList)
                {
                    if (item.terminalTypeId == 1)
                    {
                        ReplicateImagesToTCAM(item);
                    }
                }
            }
        }

        private void ReplicateImagesToTCAM(clsWinSockCliente item)
        {
            ClientMessagesBLL cmBll = new ClientMessagesBLL();

            if ((actionService != clsEnum.ServiceActions.WaitingConfirm || actionService != clsEnum.ServiceActions.WaitingFingerprintConfirm) && !item.replicating)
            {
                int cantimgs = 0, IDImg, cantidadImgs, contImgs = 0;
                string TipoImagen, RutaImagen;
                swReplicaImgs = true;

                log.WriteProcess("INICIAR RÉPLICA DE IMAGENES ");

                if (tmReplicaHuellasTCAM7000 != null)
                {
                    tmReplicaHuellasTCAM7000.Stop();

                    if (blnReplicaImgsTCAM7000)
                    {
                        tmReplicaImgsTCAM7000.Stop();
                    }
                }

                dtImgs = cmBll.GetMessagesWithoutReplicate(item.IpAddress);

                if (dtImgs != null && dtImgs.Rows.Count > 0)
                {
                    cantidadImgs = dtImgs.Rows.Count;
                    log.WriteProcess("LA CANTIDAD DE IMÁGENES SIN REPLICAR A LA TERMINAL " + item.IpAddress + " ES: " + cantidadImgs);

                    foreach (DataRow row in dtImgs.Rows)
                    {
                        byte[] tmpImagen = (byte[])row["messageImage"];
                        TipoImagen = row["messageType"].ToString();
                        IDImg = Convert.ToInt32(row["messageId"].ToString());
                        RutaImagen = System.Windows.Forms.Application.StartupPath + "\\ImagenReplica.jpg";
                        FileInfo archivoIngreso = new FileInfo(RutaImagen);

                        if (archivoIngreso.Exists)
                        {
                            archivoIngreso.Delete();
                        }

                        MemoryStream ms = new MemoryStream(tmpImagen);
                        FileStream fs = new FileStream(RutaImagen, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                        ms.WriteTo(fs);
                        fs.Flush();
                        fs.Close();
                        ms.Close();

                        swReplicaImgs = false;

                        LoadImage(IDImg, TipoImagen, RutaImagen);

                        if (bitImagenValida)
                        {
                            UploadImage(IDImg, TipoImagen);
                            intLineasImagenEnviadas = 0;
                            DateTime dtmHoraInicio = DateTime.Now;

                            while (intLineasImagenEnviadas <= intCantidadComandosEnviar)
                            {
                                if ((DateTime.Now - dtmHoraInicio).Seconds > intTiempoMaximoEnvioImgsTCAM7000)
                                {
                                    log.WriteProcess("SUPERADO TIEMPO MAXIMO EN RÉPLICA IMAGEN: " + (DateTime.Now - dtmHoraInicio).Seconds.ToString() + " SEGUNDOS");
                                    SendDataAndLogs("@I;C$", item.IpAddress);
                                    break;
                                }

                                if (swRespuestaTramaOK || intLineasImagenEnviadas == 0)
                                {
                                    GeneraComandoReplicaImgTCAM7000(item);

                                    if (intLineasImagenEnviadas == intCantidadComandosEnviar)
                                    {
                                        log.WriteProcess("COMPLETADA LA RÉPLICA IMAGEN: " + IDImg.ToString());
                                    }
                                }
                                else
                                {
                                    swRespuestaTramaOK = false;
                                }

                                Thread.Sleep(intTiempoEspaciadoTramasReplicaImgsTCAM7000);
                            }
                        }

                        if (swReplicaImgs)
                        {
                            log.WriteProcess("RÉPLICA IMAGEN: " + IDImg.ToString());

                            if (cmBll.InsertReplicatedImage(IDImg, item.IpAddress))
                            {
                                log.WriteProcess("INSERTA EN BD RÉPLICA IMAGEN: " + IDImg.ToString());
                                contImgs++;
                            }
                        }

                        cantimgs++;
                    }
                }
                else
                {
                    log.WriteProcess("NO EXISTEN IMAGENES SIN REPLICAR A TERMINAL");
                }

                if (cantimgs > 0)
                {
                    log.WriteProcess("CANTIDAD IMAGENES REPLICADAS A TERMINAL: " + contImgs.ToString());
                }

                tmReplicaHuellasTCAM7000.Start();

                if (blnReplicaImgsTCAM7000)
                {
                    tmReplicaImgsTCAM7000.Start();
                }
            }
        }

        private void GeneraComandoReplicaImgTCAM7000(clsWinSockCliente item)
        {
            string command = string.Empty;

            if (intLineasImagenEnviadas == intCantidadComandosEnviar)
            {
                arrayImagen[intLineasImagenEnviadas][2] = 73;
                swReplicaImgs = false;
            }

            intLineasImagenEnviadas += 1;

            if (intLineasImagenEnviadas <= intCantidadComandosEnviar)
            {
                actionService = clsEnum.ServiceActions.WaitingConfirmReplicationImg;
                swRespuestaTramaOK = false;
                SendDataAndLogs(arrayImagen[intLineasImagenEnviadas], item.IpAddress);
            }
        }

        private void UploadImage(int iDImg, string tipoImagen)
        {
            char[] valores = new char[2];
            int intIndiceComandoGuardarImagen = 0;

            if (tipoImagen == "1")
            {
                intIndiceComandoGuardarImagen = CANTIDAD_TOTAL_COMANDOS_IMAGEN_TCAM7000_480X320;
            }
            else if (tipoImagen == "2")
            {
                intIndiceComandoGuardarImagen = CANTIDAD_TOTAL_COMANDOS_IMAGEN_TCAM7000_480X640;
            }

            valores = iDImg.ToString("000").ToCharArray();
            arrayImagen[intIndiceComandoGuardarImagen][3] = Convert.ToByte(valores[0]);
            arrayImagen[intIndiceComandoGuardarImagen][4] = Convert.ToByte(valores[1]);
            arrayImagen[intIndiceComandoGuardarImagen][5] = Convert.ToByte(valores[2]);
        }

        private void LoadImage(int iDImg, string tipoImagen, string rutaImagen)
        {
            string strMsgError = string.Empty;
            ImagenTCAM imgTCAM = new ImagenTCAM();

            if (tipoImagen == "1")
            {
                arrayImagen = imgTCAM.Convertir_Imagen_Gymsoft_480x320(rutaImagen);

                if (arrayImagen != null)
                {
                    intCantidadComandosEnviar = CANTIDAD_TOTAL_COMANDOS_IMAGEN_TCAM7000_480X320;
                }
            }
            else if (tipoImagen == "2")
            {
                arrayImagen = imgTCAM.Convertir_Imagen_Gymsoft_480x640(rutaImagen);

                if (arrayImagen != null)
                {
                    intCantidadComandosEnviar = CANTIDAD_TOTAL_COMANDOS_IMAGEN_TCAM7000_480X640;
                }
            }

            if (arrayImagen == null)
            {
                log.WriteProcess("ERROR: La imagen " + iDImg.ToString() + " es invalida y no puede ser enviada a la terminal");
                bitImagenValida = false;
            }
            else
            {
                bitImagenValida = true;
            }
        }

        private void TimerRemoveUsers_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (allowWhiteListTCAM)
            {
                if (terminalObjectList.Count > 0)
                {
                    dtDeletedUsers = new DataTable();

                    foreach (PropertyInfo info in typeof(eReplicatedUser).GetProperties())
                    {
                        dtDeletedUsers.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
                    }

                    int i = 0;
                    foreach (clsWinSockCliente item in terminalObjectList)
                    {
                        if (ValidateTerminalStateRemoveUsers(item, true) && item.terminalTypeId == 1)
                        {
                            indexRemoveUsers = i;
                            startIndexRemoveUsers = i;
                            terminalCounterDeleteUser++;
                            item.replicating = true;
                            ControlRemoveUsers(item, false, true);
                        }
                        else
                        {
                            terminalCounterDeleteUser = 0;
                        }

                        i++;
                    }
                }
            }
        }

        private bool ValidateTerminalStateRemoveUsers(clsWinSockCliente actualTerminal, bool validateHour)
        {
            bool response = false;

            if (actualTerminal.withWithelist)
            {
                if ((validateHour && ValidateHourRemoveUsers(actualTerminal.IpAddress)) || !validateHour)
                {
                    if (!actualTerminal.replicating)
                    {
                        response = true;
                    }
                    //else
                    //{
                    //    log.WriteProcessByTerminal("NO ES POSIBLE ELIMINAR USUARIOS DE LA TERMINAL YA QUE LA TERMINAL ESTÁ REALIZANDO UN PROCESO DE SINCRONIZACIÓN.", actualTerminal.IpAddress);
                    //}
                }
            }
            //else
            //{
            //    log.WriteProcessByTerminal("LA TERMINAL NO ESTÁ HABILITADA PARA BORRAR USUARIOS DE LISTA BLANCA.", actualTerminal.IpAddress);
            //}

            return response;
        }

        private bool ValidateHourRemoveUsers(string ipAddress)
        {
            ScheduleBLL srfBll = new ScheduleBLL();
            DataTable dtSchedules = new DataTable();
            dtSchedules = srfBll.GetUserSchedules();
            bool execute = false;

            if (dtSchedules != null)
            {
                if (dtSchedules.Rows.Count > 0)
                {
                    foreach (DataRow row in dtSchedules.Rows)
                    {
                        if (Convert.ToDateTime(row["executionHour"]).Hour == DateTime.Now.Hour && Convert.ToDateTime(row["executionHour"]).Minute == DateTime.Now.Minute &&
                            row["ipAddress"].ToString() == ipAddress)
                        {
                            execute = true;
                        }
                    }
                }
            }

            return execute;
        }

        private void ControlRemoveUsers(clsWinSockCliente actualTerminal, bool validateHour, bool firstTime)
        {
            if (firstTime)
            {
                log.WriteProcess("INICIA EL PROCESO DE ELIMINACIÓN DE USUARIOS DE LAS TERMINALES.");
                DeleteUsers(actualTerminal.IpAddress, false, false);
            }
            else
            {
                if (indexRemoveUsers == startIndexRemoveUsers)
                {
                    log.WriteProcess("FINALIZA EL PROCESO DE ELIMINACIÓN DE USUARIOS DE LAS TERMINALES.");
                    terminalCounterDeleteUser = 0;
                    startIndexRemoveUsers = 0;
                    indexRemoveUsers = 0;
                    indexUsersWhiteListDelete = 0;
                    timerRemoveUsers.Enabled = true;
                    timerRemoveUsers.Start();
                }
                else
                {
                    if (ValidateTerminalStateRemoveUsers(actualTerminal, true))
                    {
                        terminalCounterDeleteUser++;
                        DeleteUsers(actualTerminal.IpAddress, false, false);
                    }
                    else
                    {
                        if ((indexRemoveUsers + 1) < terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                        {
                            indexRemoveUsers++;
                            ControlRemoveUsers(terminalObjectList[indexRemoveUsers], true, false);
                            return;
                        }
                        else if (startIndexRemoveUsers > 0)
                        {
                            indexRemoveUsers = 0;
                            ControlRemoveUsers(terminalObjectList[indexRemoveUsers], true, false);
                            return;
                        }
                        else if ((indexRemoveUsers + 1) == terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                        {
                            log.WriteProcess("FINALIZA EL PROCESO DE ELIMINACIÓN DE USUARIO DE LAS TERMINALES.");
                            terminalCounterDeleteUser = 0;
                            startIndexRemoveUsers = 0;
                            indexRemoveUsers = 0;
                            indexUsersWhiteListDelete = 0;
                            timerRemoveUsers.Enabled = true;
                            timerRemoveUsers.Start();
                        }
                    }
                }
            }
        }

        private void DeleteUsers(string ipAddress, bool actionState, bool actionOk)
        {
            int quantityUsers = 0, counterUsers = 0, quantityAux = 0, genericQuantity = 20;
            string userId = string.Empty, userName = string.Empty;
            UserBLL userBLL = new UserBLL();
            ReplicatedUserBLL replicatedUserBLL = new ReplicatedUserBLL();

            if (!actionState)
            {
                log.WriteProcessByTerminal("INICIA ELIMINACIÓN DE USUARIOS DE LISTA BLANCA EN LA TERMINAL.", ipAddress);
                timerRemoveUsers.Stop();
                userListDelete = new List<eUser>();
                userListDelete = userBLL.GetUsersToDelete(ipAddress);
            }
            else
            {
                Thread.Sleep(600);

                if (actionOk && dtDeletedUsers != null)
                {
                    if (dtDeletedUsers.Rows.Count > 0)
                    {
                        string userIds = string.Empty;

                        foreach (DataRow row in dtDeletedUsers.Rows)
                        {
                            if (string.IsNullOrEmpty(userIds))
                            {
                                userIds = row["userId"].ToString();
                            }
                            else
                            {
                                userIds += "," + row["userId"].ToString();
                            }
                        }

                        replicatedUserBLL.DeleteUsersDeleted(ipAddress, userIds);
                        log.WriteProcessByTerminal("ELIMINACIÓN DE USUARIOS: " + userIds, ipAddress);
                        dtDeletedUsers = new DataTable();

                        foreach (PropertyInfo info in typeof(eReplicatedUser).GetProperties())
                        {
                            dtDeletedUsers.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
                        }

                        counterUsers++;
                    }
                }
            }

            if (userListDelete != null && userListDelete.Count > 0)
            {
                if (!actionState)
                {
                    quantityUsers = userListDelete.Count;
                    log.WriteProcessByTerminal("CANTIDAD DE USUARIOS SIN ELIMINAR DE LA TERMINAL: " + quantityUsers.ToString(), ipAddress);
                }

                if (indexUsersWhiteListDelete < userListDelete.Count)
                {
                    List<eUsersToReplicate> usersToDeleteList = new List<eUsersToReplicate>();
                    int counterAux = 0;

                    while (indexUsersWhiteListDelete < userListDelete.Count && counterAux < genericQuantity)
                    {
                        eUsersToReplicate usersToDelete = new eUsersToReplicate()
                        {
                            delete = userListDelete[indexUsersWhiteListDelete].delete,
                            fingerprintId = userListDelete[indexUsersWhiteListDelete].fingerprintId,
                            insert = userListDelete[indexUsersWhiteListDelete].insert,
                            ipAddress = ipAddress,
                            recordId = userListDelete[indexUsersWhiteListDelete].id,
                            userId = userListDelete[indexUsersWhiteListDelete].userId,
                            userName = userListDelete[indexUsersWhiteListDelete].userName,
                            withoutFingerprint = userListDelete[indexUsersWhiteListDelete].withoutFingerprint
                        };

                        if (string.IsNullOrEmpty(usersDeleteUsed))
                        {
                            usersDeleteUsed = usersToDelete.recordId.ToString();
                        }
                        else
                        {
                            usersDeleteUsed += "," + usersToDelete.recordId.ToString();
                        }

                        DataRow rowUser = dtDeletedUsers.NewRow();
                        rowUser = GetRowUserDelete(usersToDelete);
                        dtDeletedUsers.Rows.Add(rowUser);
                        usersToDeleteList.Add(usersToDelete);
                        indexUsersWhiteListDelete++;
                        counterAux++;
                    }

                    if (GenerateCommandToDeleteUsers(usersToDeleteList, ipAddress) != -1)
                    {
                        tmWaitResponseDeleteUser.Enabled = true;
                        tmWaitResponseDeleteUser.Start();
                    }
                    else
                    {
                        counterUsers++;
                        DeleteUsers(ipAddress, true, false);
                    }

                    quantityAux++;
                    return;
                }
                else
                {
                    log.WriteProcessByTerminal("FINALIZA ELIMINACIÓN DE USUARIOS DE LISTA BLANCA EN LA TERMINAL.", ipAddress);
                    terminalObjectList.Find(tol => tol.IpAddress == ipAddress).replicating = false;
                    actionService = clsEnum.ServiceActions.WaitingClientId;

                    if (terminalCounterDeleteUser == terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length && !string.IsNullOrEmpty(usersDeleteUsed))
                    {
                        if (userBLL.UpdateUsedUsers(usersDeleteUsed))
                        {
                            usersDeleteUsed = string.Empty;
                        }

                        terminalCounterDeleteUser = 0;
                        indexRemoveUsers = 0;
                        startIndexRemoveUsers = 0;
                    }
                    else
                    {
                        if ((indexRemoveUsers + 1) < terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                        {
                            indexRemoveUsers++;
                        }
                        else if (startIndexRemoveUsers > 0)
                        {
                            indexRemoveUsers = 0;
                        }

                        ControlRemoveUsers(terminalObjectList[indexRemoveUsers], true, false);
                        return;
                    }
                }
            }
            else
            {
                log.WriteProcessByTerminal("NO EXISTEN USUARIOS SIN ELIMINAR EN LA TERMINAL", ipAddress);
                terminalObjectList.Find(tol => tol.IpAddress == ipAddress).replicating = false;
                actionService = clsEnum.ServiceActions.WaitingClientId;

                if (terminalCounterDeleteUser == terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length && !string.IsNullOrEmpty(usersDeleteUsed))
                {
                    if (userBLL.UpdateUsedUsers(usersDeleteUsed))
                    {
                        usersDeleteUsed = string.Empty;
                    }

                    terminalCounterDeleteUser = 0;
                    indexRemoveUsers = 0;
                    startIndexRemoveUsers = 0;
                }
                else
                {
                    if ((indexRemoveUsers + 1) < terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                    {
                        indexRemoveUsers++;
                    }
                    else if (startIndexRemoveUsers > 0)
                    {
                        indexRemoveUsers = 0;
                    }

                    ControlRemoveUsers(terminalObjectList[indexRemoveUsers], true, false);
                    return;
                }
            }

            if (quantityAux > 0)
            {
                log.WriteProcessByTerminal("CANTIDAD DE USUARIOS ELIMINADOS EN LA TERMINAL: " + quantityAux.ToString(), ipAddress);
            }

            indexUsersWhiteListDelete = 0;
            timerRemoveUsers.Start();
        }

        private DataRow GetRowUserDelete(eUsersToReplicate usersToDelete)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                DataRow row = dtDeletedUsers.NewRow();
                row["userId"] = usersToDelete.userId;
                row["fingerprintId"] = usersToDelete.fingerprintId;
                row["ipAddress"] = usersToDelete.ipAddress;
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
        /// Evento del timer de borrado de huella de terminales.
        /// Getulio Vargas - 2018-08-31 - OD 1307
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerRemoveFingerprints_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (terminalObjectList.Count > 0)
            {
                int i = 0;
                foreach (clsWinSockCliente item in terminalObjectList)
                {
                    if (ValidateTerminalStateRemoveFingerprints(item, true) && item.terminalTypeId == 1)
                    {
                        indexRemoveFingerprints = i;
                        startIndexRemoveFingerprints = i;
                        terminalCounterDeleteFingerprint++;
                        item.replicating = true;
                        ControlRemoveFingerprints(item, false, true);
                    }

                    i++;
                }
            }
        }

        private void ControlRemoveFingerprints(clsWinSockCliente actualTerminal, bool validateHour, bool firstTime)
        {
            if (firstTime)
            {
                log.WriteProcess("INICIA EL PROCESO DE ALIMINACIÓN DE HUELLAS DE LAS TERMINALES.");
                DeleteFingerprints(actualTerminal.IpAddress, false, false);
            }
            else
            {
                if (indexRemoveFingerprints == startIndexRemoveFingerprints)
                {
                    log.WriteProcess("FINALIZA EL PROCESO DE ELIMINACIÓN DE HUELLAS DE LAS TERMINALES.");
                    terminalCounterDeleteFingerprint = 0;
                    startIndexRemoveFingerprints = 0;
                    indexRemoveFingerprints = 0;
                    indexFingerprintsDelete = 0;
                    timerRemoveFingerprints.Enabled = true;
                    timerRemoveFingerprints.Start();
                }
                else
                {
                    if (ValidateTerminalStateRemoveFingerprints(actualTerminal, true))
                    {
                        terminalCounterDeleteFingerprint++;
                        DeleteFingerprints(actualTerminal.IpAddress, false, false);
                    }
                    else
                    {
                        if ((indexRemoveFingerprints + 1) < terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                        {
                            indexRemoveFingerprints++;
                            ControlRemoveFingerprints(terminalObjectList[indexRemoveFingerprints], true, false);
                            return;
                        }
                        else if (startIndexRemoveFingerprints > 0)
                        {
                            indexRemoveFingerprints = 0;
                            ControlRemoveFingerprints(terminalObjectList[indexRemoveFingerprints], true, false);
                            return;
                        }
                        else if ((indexRemoveFingerprints + 1) == terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                        {
                            log.WriteProcess("FINALIZA EL PROCESO DE ELIMINACIÓN DE HUELLAS EN LAS TERMINALES.");
                            terminalCounterDeleteFingerprint = 0;
                            startIndexRemoveFingerprints = 0;
                            indexRemoveFingerprints = 0;
                            indexFingerprintsDelete = 0;
                            timerRemoveFingerprints.Enabled = true;
                            timerRemoveFingerprints.Start();
                        }
                    }
                }
            }
        }

        private bool ValidateTerminalStateRemoveFingerprints(clsWinSockCliente actualTerminal, bool validateHour)
        {
            bool response = false;

            if ((validateHour && ValidateHourRemoveFingerprints(actualTerminal.IpAddress)) || !validateHour)
            {
                if (!actualTerminal.replicating)
                {
                    response = true;
                }
                //else
                //{
                //    log.WriteProcessByTerminal("NO ES POSIBLE ELIMINAR HUELLAS DE LA TERMINAL YA QUE LA TERMINAL ESTÁ REALIZANDO UN PROCESO DE SINCRONIZACIÓN.", actualTerminal.IpAddress);
                //}
            }

            return response;
        }

        private bool ValidateHourRemoveFingerprints(string ipAddress)
        {
            ScheduleBLL srfBll = new ScheduleBLL();
            DataTable dtSchedules = new DataTable();
            dtSchedules = srfBll.GetFingerprintSchedules();
            bool execute = false;

            if (dtSchedules != null)
            {
                if (dtSchedules.Rows.Count > 0)
                {
                    foreach (DataRow row in dtSchedules.Rows)
                    {
                        if (Convert.ToDateTime(row["executionHour"]).Hour == DateTime.Now.Hour && Convert.ToDateTime(row["executionHour"]).Minute == DateTime.Now.Minute &&
                            row["ipAddress"].ToString() == ipAddress)
                        {
                            execute = true;
                        }
                    }
                }
            }

            return execute;
        }

        private void DeleteFingerprints(string ipAddress, bool actionState, bool actionOk)
        {
            int quantityFingerprintDelete = 0, counterFingerprint = 0, quantityAux = 0, genericQuantity = 100;
            FingerprintBLL fingerBll = new FingerprintBLL();
            ReplicatedFingerprintBLL replicatedFingerprintBLL = new ReplicatedFingerprintBLL();

            if (!actionState)
            {
                log.WriteProcessByTerminal("INICIA LA ELIMINACIÓN DE HUELLAS EN LA TERMINAL.", ipAddress);
                timerRemoveFingerprints.Stop();
                fingerListDelete = new List<eFingerprint>();
                fingerListDelete = fingerBll.GetFingerprintsToDelete(ipAddress);
            }
            else
            {
                Thread.Sleep(600);

                if (actionOk)
                {
                    string fingerprintsDeleted = string.Empty, fingerprintDeletedIds = string.Empty;

                    foreach (int item in listFingerprintsDelete)
                    {
                        if (string.IsNullOrEmpty(fingerprintsDeleted))
                        {
                            fingerprintsDeleted = item.ToString();
                        }
                        else
                        {
                            fingerprintsDeleted += "," + item.ToString();
                        }
                    }

                    replicatedFingerprintBLL.DeleteFingerprintsDeleted(ipAddress, fingerprintsDeleted);
                    log.WriteProcessByTerminal("HUELLAS ELIMINADAS: " + fingerprintsDeleted, ipAddress);
                    listFingerprintsDelete = new List<int>();
                }
            }

            if (fingerListDelete != null && fingerListDelete.Count > 0)
            {
                if (!actionState)
                {
                    quantityFingerprintDelete = fingerListDelete.Count;
                    log.WriteProcessByTerminal("CANTIDAD DE HUELLAS SIN ELIMINAR EN LA TERMINAL: " + quantityFingerprintDelete.ToString(), ipAddress);
                }

                if (indexFingerprintsDelete < fingerListDelete.Count)
                {
                    listFingerprintsDelete = new List<int>();
                    int counterAux = 0;

                    while (indexFingerprintsDelete < fingerListDelete.Count && counterAux < genericQuantity)
                    {
                        if (!fingerprintDeleteUsed.Contains("," + fingerListDelete[indexFingerprintsDelete].recordId.ToString() + ","))
                        {
                            if (string.IsNullOrEmpty(fingerprintDeleteUsed))
                            {
                                fingerprintDeleteUsed = fingerListDelete[indexFingerprintsDelete].recordId.ToString();
                            }
                            else
                            {
                                fingerprintDeleteUsed += "," + fingerListDelete[indexFingerprintsDelete].recordId.ToString();
                            }
                        }

                        listFingerprintsDelete.Add(fingerListDelete[indexFingerprintsDelete].id);
                        indexFingerprintsDelete++;
                        counterAux++;
                    }

                    if (GenerateCommandToRemoveFingerprints(listFingerprintsDelete, ipAddress) != -1)
                    {
                        tmWaitResponseDeleteFingerprints.Enabled = true;
                        tmWaitResponseDeleteFingerprints.Start();
                    }
                    else
                    {
                        counterFingerprint++;
                        DeleteFingerprints(ipAddress, true, false);
                    }

                    quantityAux++;
                    return;
                }
                else
                {
                    log.WriteProcessByTerminal("FINALIZA ELIMINACIÓN DE HUELLAS EN LA TERMINAL.", ipAddress);
                    terminalObjectList.Find(tol => tol.IpAddress == ipAddress).replicating = false;
                    actionService = clsEnum.ServiceActions.WaitingClientId;

                    if (terminalCounterDeleteFingerprint == terminalObjectList.Count && !string.IsNullOrEmpty(fingerprintDeleteUsed))
                    {
                        if (fingerBll.UpdateUsedFingerprints(fingerprintDeleteUsed))
                        {
                            fingerprintDeleteUsed = string.Empty;
                        }

                        terminalCounterDeleteFingerprint = 0;
                        startIndexRemoveFingerprints = 0;
                        indexRemoveFingerprints = 0;
                        log.WriteProcess("FINALIZA EL PROCESO DE ELIMINACIÓN DE HUELLAS EN LAS TERMINALES.");
                    }
                    else
                    {
                        if ((indexRemoveFingerprints + 1) < terminalObjectList.Count)
                        {
                            indexRemoveFingerprints++;
                        }
                        else if (startIndexRemoveFingerprints > 0)
                        {
                            indexRemoveFingerprints = 0;
                        }

                        ControlRemoveFingerprints(terminalObjectList[indexRemoveFingerprints], true, false);
                        return;
                    }
                }
            }
            else
            {
                log.WriteProcessByTerminal("NO EXISTEN HUELLAS POR ELIMINAR EN LA TERMINAL.", ipAddress);
                terminalObjectList.Find(tol => tol.IpAddress == ipAddress).replicating = false;
                actionService = clsEnum.ServiceActions.WaitingClientId;

                if (terminalCounterDeleteFingerprint == terminalObjectList.Count && !string.IsNullOrEmpty(fingerprintDeleteUsed))
                {
                    if (fingerBll.UpdateUsedFingerprints(fingerprintDeleteUsed))
                    {
                        fingerprintDeleteUsed = string.Empty;
                    }

                    terminalCounterDeleteFingerprint = 0;
                    startIndexRemoveFingerprints = 0;
                    indexRemoveFingerprints = 0;
                    log.WriteProcess("FINALIZA EL PROCESO DE ELIMINACIÓN DE HUELLAS EN LAS TERMINALES.");
                }
                else
                {
                    if ((indexRemoveFingerprints + 1) < terminalObjectList.Count)
                    {
                        indexRemoveFingerprints++;
                    }
                    else if (startIndexRemoveFingerprints > 0)
                    {
                        indexRemoveFingerprints = 0;
                    }

                    ControlRemoveFingerprints(terminalObjectList[indexRemoveFingerprints], true, false);
                    return;
                }
            }

            indexFingerprintsDelete = 0;
            timerRemoveFingerprints.Start();
        }

        private int GenerateCommandToRemoveFingerprints(List<int> listFingerprintsDelete, string ipAddress)
        {
            string command = string.Empty;

            if (listFingerprintsDelete != null && listFingerprintsDelete.Count > 0)
            {
                command = "@B";

                foreach (int item in listFingerprintsDelete)
                {
                    if (command.Length > 2)
                    {
                        command += ";" + item.ToString().PadLeft(6, '0');
                    }
                    else
                    {
                        command += item.ToString().PadLeft(6, '0');
                    }
                }

                command += "$";
                actionService = clsEnum.ServiceActions.WaitingRemoveFingerprints;
                SendDataAndLogs(command, ipAddress);
                log.WriteProcessByTerminal("HUELLAS A ELIMINAR - " + command, ipAddress);
                return 0;
            }
            else
            {
                return -1;
            }
        }

        private void TmReplicaHuellasTCAM7000_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (terminalObjectList.Count > 0)
            {
                int i = 0;
                foreach (clsWinSockCliente item in terminalObjectList)
                {
                    //OD 1689 - Getulio Vargas - 2020/01/28
                    //Se realiza validación para el proceso de réplica de huellas, para que sólo realice esto para las terminales de tipo 1 (TCAM7000)
                    if (ValidateTerminalStateReplicateFingerprints(item, true) && item.terminalTypeId == 1)
                    {
                        startIndexReplicateFingerprints = i;
                        indexReplicateFingerprints = i;
                        terminalCounterReplicateFingerprint++;
                        item.replicating = true;
                        ControlReplicateFingerprints(item, true);
                        break;
                    }

                    i++;
                }
            }
        }

        private void ControlReplicateFingerprints(clsWinSockCliente actualTerminal, bool firstTime)
        {
            if (firstTime)
            {
                log.WriteProcess("INICIA EL PROCESO DE RÉPLICA DE HUELLAS A LAS TERMINALES.");
                ReplicateFingerprints(actualTerminal.IpAddress, false, false);
            }
            else
            {
                if (indexReplicateFingerprints == startIndexReplicateFingerprints)
                {
                    log.WriteProcess("FINALIZA EL PROCESO DE RÉPLICA DE HUELLAS A LAS TERMINALES.");
                    terminalCounterReplicateFingerprint = 0;
                    startIndexReplicateFingerprints = 0;
                    indexReplicateFingerprints = 0;
                    intIndiceHuellaRepAct = 0;
                    tmReplicaHuellasTCAM7000.Enabled = true;
                    tmReplicaHuellasTCAM7000.Start();
                }
                else
                {
                    if (ValidateTerminalStateReplicateFingerprints(actualTerminal, true))
                    {
                        terminalCounterReplicateFingerprint++;
                        intIndiceHuellaRepAct = 0;
                        ReplicateFingerprints(actualTerminal.IpAddress, false, false);
                    }
                    else
                    {
                        if ((indexReplicateFingerprints + 1) < terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                        {
                            indexReplicateFingerprints++;
                            ControlReplicateFingerprints(terminalObjectList[indexReplicateFingerprints], false);
                            return;
                        }
                        else if (startIndexReplicateFingerprints > 0)
                        {
                            indexReplicateFingerprints = 0;
                            ControlReplicateFingerprints(terminalObjectList[indexReplicateFingerprints], false);
                            return;
                        }
                        else if ((indexReplicateFingerprints + 1) == terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                        {
                            log.WriteProcess("FINALIZA EL PROCESO DE RÉPLICA DE HUELLAS EN LAS TERMINALES.");
                            terminalCounterReplicateFingerprint = 0;
                            startIndexReplicateFingerprints = 0;
                            indexReplicateFingerprints = 0;
                            intIndiceHuellaRepAct = 0;
                            tmReplicaHuellasTCAM7000.Enabled = true;
                            tmReplicaHuellasTCAM7000.Start();
                        }
                    }
                }
            }
        }

        private bool ValidateTerminalStateReplicateFingerprints(clsWinSockCliente actualTerminal, bool validateHour)
        {
            bool response = false;

            if (!actualTerminal.replicating)
            {
                response = true;
            }
            //else
            //{
            //    log.WriteProcessByTerminal("NO ES POSIBLE REPLICAR HUELLAS A LA TERMINAL YA QUE LA TERMINAL ESTÁ REALIZANDO UN PROCESO DE SINCRONIZACIÓN.", actualTerminal.IpAddress);
            //}

            return response;
        }

        private void ReplicateFingerprints(string ipAddress, bool replicaState, bool replicaOk)
        {
            int quantityFingerprint = 0, counterFingerprint = 0, quantityAux = 0, recordId = 0, id = 0;
            string clientId = string.Empty;
            byte[] tmpFinger;
            FingerprintBLL fingerBll = new FingerprintBLL();
            ReplicatedFingerprintBLL rfBll = new ReplicatedFingerprintBLL();

            if (!replicaState)
            {
                log.WriteProcessByTerminal("INICIA RÉPLICA DE HUELLAS A LA TERMINAL", ipAddress);
                tmReplicaHuellasTCAM7000.Stop();

                if (blnReplicaImgsTCAM7000)
                {
                    tmReplicaImgsTCAM7000.Stop();
                }

                fingerList = new List<eFingerprint>();
                fingerList = fingerBll.GetFingerprintsToReplicate(ipAddress);
            }
            else
            {
                Thread.Sleep(600);

                if (replicaOk)
                {
                    clientId = fingerList[intIndiceHuellaRepAct].personId;
                    recordId = fingerList[intIndiceHuellaRepAct].recordId;
                    id = fingerList[intIndiceHuellaRepAct].id;
                    log.WriteProcessByTerminal("RÉPLICA HUELLA CLIENTE: " + clientId, ipAddress);
                    eReplicatedFingerprint rfEntity = new eReplicatedFingerprint();
                    rfEntity.fingerprintId = id;
                    rfEntity.ipAddress = ipAddress;
                    rfEntity.replicationDate = DateTime.Now;
                    rfEntity.userId = clientId;

                    if (string.IsNullOrEmpty(fingerprintsUsed))
                    {
                        fingerprintsUsed = recordId.ToString();
                    }
                    else
                    {
                        fingerprintsUsed += "," + recordId.ToString();
                    }

                    if (rfBll.Insert(rfEntity))
                    {
                        log.WriteProcessByTerminal("INSERTA EN BD RÉPLICA LA HUELLA DEL CLIENTE: " + clientId + ", ID Huella: " + id.ToString(), ipAddress);
                        counterFingerprint++;
                    }
                }

                intIndiceHuellaRepAct++;
            }

            if (fingerList != null && fingerList.Count > 0)
            {
                if (!replicaState)
                {
                    quantityFingerprint = fingerList.Count;
                    log.WriteProcessByTerminal("CONSULTA CANTIDAD HUELLAS SIN REPLICAR A TERMINAL: " + quantityFingerprint.ToString(), ipAddress);
                }

                if (intIndiceHuellaRepAct < fingerList.Count)
                {
                    tmpFinger = fingerList[intIndiceHuellaRepAct].fingerPrint;
                    clientId = fingerList[intIndiceHuellaRepAct].personId;
                    id = fingerList[intIndiceHuellaRepAct].id;

                    if (GenerateCommandToReplicateFingerprint(clientId, id, tmpFinger, ipAddress) != -1)
                    {
                        tmEsperaRespuestaTCAM7000.Enabled = true;
                        tmEsperaRespuestaTCAM7000.Start();
                    }
                    else
                    {
                        log.WriteProcessByTerminal("DESCARTA RÉPLICA DE HUELLA INCONSISTENTE DEL USUARIO CON ID " + clientId + ", ID Huella: " + id.ToString(), ipAddress);
                        counterFingerprint++;
                        ReplicateFingerprints(ipAddress, true, false);
                    }

                    quantityAux++;
                    return;
                }
                else
                {
                    //if (terminalCounterReplicateFingerprint == terminalObjectList.Count && !string.IsNullOrEmpty(fingerprintsUsed))
                    //{
                    //    FingerprintBLL fingerprintBLL = new FingerprintBLL();

                    //    if (fingerprintBLL.UpdateUsedFingerprints(fingerprintsUsed))
                    //    {
                    //        fingerprintsUsed = string.Empty;
                    //    }

                    //    terminalCounterReplicateFingerprint = 0;
                    //}

                    log.WriteProcessByTerminal("FINALIZA RÉPLICA DE HUELLAS A LA TERMINAL", ipAddress);
                    terminalObjectList.Find(tol => tol.IpAddress == ipAddress).replicating = false;
                    actionService = clsEnum.ServiceActions.WaitingClientId;

                    if (terminalCounterReplicateFingerprint == terminalObjectList.Count)
                    {
                        terminalCounterReplicateFingerprint = 0;
                        startIndexReplicateFingerprints = 0;
                        indexReplicateFingerprints = 0;
                        log.WriteProcess("FINALIZA EL PROCESO DE RÉPLICA DE HUELLAS EN LAS TERMINALES.");
                    }
                    else
                    {
                        if ((indexReplicateFingerprints + 1) < terminalObjectList.Count)
                        {
                            indexReplicateFingerprints++;
                        }
                        else if (startIndexReplicateFingerprints > 0)
                        {
                            indexReplicateFingerprints = 0;
                        }

                        ControlReplicateFingerprints(terminalObjectList[indexReplicateFingerprints], false);
                        return;
                    }
                }
            }
            else
            {
                log.WriteProcessByTerminal("NO EXISTEN HUELLAS SIN REPLICAR A LA TERMINAL", ipAddress);
                terminalObjectList.Find(tol => tol.IpAddress == ipAddress).replicating = false;
                actionService = clsEnum.ServiceActions.WaitingClientId;

                if (terminalCounterReplicateFingerprint == terminalObjectList.Count)
                {
                    terminalCounterReplicateFingerprint = 0;
                    startIndexReplicateFingerprints = 0;
                    indexReplicateFingerprints = 0;
                    log.WriteProcess("FINALIZA EL PROCESO DE RÉPLICA DE HUELLAS EN LAS TERMINALES.");
                }
                else
                {
                    if ((indexReplicateFingerprints + 1) < terminalObjectList.Count)
                    {
                        indexReplicateFingerprints++;
                    }
                    else if (startIndexReplicateFingerprints > 0)
                    {
                        indexReplicateFingerprints = 0;
                    }

                    ControlReplicateFingerprints(terminalObjectList[indexReplicateFingerprints], false);
                    return;
                }
            }

            if (quantityAux > 0)
            {
                log.WriteProcessByTerminal("CANTIDAD HUELLAS REPLICADAS A TERMINAL: " + quantityAux.ToString(), ipAddress);
            }

            intIndiceHuellaRepAct = 0;
            tmReplicaHuellasTCAM7000.Start();

            if (blnReplicaImgsTCAM7000)
            {
                tmReplicaImgsTCAM7000.Start();
            }
        }

        private int GenerateCommandToReplicateFingerprint(string clientId, int recordId, byte[] tmpFinger, string ipAddress)
        {
            byte[] fingerData = new byte[768], dsDatoTmp = new byte[768];
            Array.Copy(tmpFinger, fingerData, 767);
            string strData = string.Empty;

            if (fingerData[fingerData.Length - 1] != Encoding.ASCII.GetBytes("A")[0])
            {
                int i = 0;

                foreach (byte item in fingerData)
                {
                    strData = item.ToString("X");

                    if (strData.ToString().Length == 1)
                    {
                        dsDatoTmp[i] = Encoding.ASCII.GetBytes("0")[0];
                        dsDatoTmp[i + 1] = Encoding.ASCII.GetBytes(strData.ToString().Substring(0, 1))[0];
                    }
                    else
                    {
                        dsDatoTmp[i] = Encoding.ASCII.GetBytes(strData.ToString().Substring(0, 1))[0];
                        dsDatoTmp[i + 1] = Encoding.ASCII.GetBytes(strData.ToString().Substring(1, 1))[0];
                    }

                    i += 2;

                    if (i >= 767)
                    {
                        break;
                    }
                }
            }
            else
            {
                dsDatoTmp = fingerData;
            }

            if (huellaContinua == intHuellasPorTrama)
            {
                huellaContinua = 0;
                Thread.Sleep(300);
            }
            else
            {
                huellaContinua++;
            }

            string command = string.Empty;
            command = "@R";
            command += recordId.ToString().PadLeft(12, '0');

            if (dsDatoTmp[0] != 52)
            {
                return -1;
            }

            command += ";" + Encoding.ASCII.GetString(dsDatoTmp).Substring(0, 768);
            command += "$";

            actionService = clsEnum.ServiceActions.WaitingConfirmFingerprintReplicate;
            SendDataAndLogs(command, ipAddress);
            log.WriteProcessByTerminal("Huella a replicar - " + command, ipAddress);
            return 0;
        }

        private void TmEsperaRespuestaTCAM7000_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (terminalCounterReplicateFingerprint > 0)
            {
                ReplicateFingerprints(terminalObjectList[terminalCounterReplicateFingerprint - 1].IpAddress, true, false);
            }
        }

        /// <summary>
        /// Método donde se realiza la carga de los parámetros de configuración generales del ingreso.
        /// Getulio Vargas - 2018-05-30 - OD 1314
        /// </summary>
        /// <param name="config"></param>
        private void FillConfigurationParameters(eConfiguration config)
        {
            try
            {
                if (config.intTiempoPingContinuoTCAM7000 > 0)
                {
                    pingTime = config.intTiempoPingContinuoTCAM7000 * oneSecond;
                }

                blnBaseDatosSQL = config.bitBaseDatosSQLServer;
                blnMensajeCumpleanos = config.bitMensajeCumpleanos;

                if (!string.IsNullOrEmpty(config.strTextoMensajeCumpleanos))
                {
                    strTextoMensajeCumpleanos = config.strTextoMensajeCumpleanos;
                }

                blnComplices_CortxIngs = config.bitComplices_CortxIngs;
                blnComplices_DescuentoTiq = config.bitComplices_DescuentoTiq;

                if (!string.IsNullOrEmpty(config.strTextoMensajeCortxIngs))
                {
                    strTextoMensajeCortxIngs = config.strTextoMensajeCortxIngs;
                }

                intComplices_Plan_CortxIngs = config.intComplices_Plan_CortxIngs;
                blnMultiplesPlanesVig = config.bitMultiplesPlanesVig;
                blnSolo1HuellaxCliente = config.bitSolo1HuellaxCliente;
                blnValideContrato = config.bitValideContrato;
                blnValideContPorFactura = config.bitValideContratoPorFactura;
                blnReplicaImgsTCAM7000 = config.bitReplicaImgsTCAM7000;

                if (config.intTiempoReplicaImgsTCAM7000 > 0)
                {
                    intTiempoReplicaImgsTCAM7000 = config.intTiempoReplicaImgsTCAM7000 * oneSecond;
                }

                if (config.intTiempoEspaciadoTramasReplicaImgsTCAM7000 > 0)
                {
                    intTiempoEspaciadoTramasReplicaImgsTCAM7000 = config.intTiempoEspaciadoTramasReplicaImgsTCAM7000;
                }

                if (config.intTiempoMaximoEnvioImgsTCAM7000 > 0)
                {
                    intTiempoMaximoEnvioImgsTCAM7000 = config.intTiempoMaximoEnvioImgsTCAM7000;
                }

                blnValidarMSW = config.bitConsultaInfoCita;
                blnAccesoDiscapacitados = config.bitAccesoDiscapacitados;
                blnTrabajarConDBEnOtroEquipo = config.bitTrabajarConDBEnOtroEquipo;

                if (config.intTiempoParaLimpiarPantalla > 0)
                {
                    tmlimpiarPantalla = config.intTiempoParaLimpiarPantalla * oneSecond;
                }

                if (config.intTiempoActualizaIngresos > 0)
                {
                    intTIEMPO_ACTUALIZA_INGRESOS = config.intTiempoActualizaIngresos;
                }

                if (config.intMinutosDescontarTiquetes > 0)
                {
                    intTiempoRestarTiquetes = config.intMinutosDescontarTiquetes;
                }

                if (!string.IsNullOrEmpty(strRutaBanner))
                {
                    strRutaBanner = config.strRutaNombreBanner;
                }

                if (config.intTiempoPulso > 0)
                {
                    intTiempoPulso = config.intTiempoPulso;
                }

                if (!string.IsNullOrEmpty(strClave))
                {
                    strClave = config.strClave;
                }

                blnPermitirIngresoAdicional = config.blnPermiteIngresosAdicionales;

                if (!string.IsNullOrEmpty(strRutaLogo))
                {
                    strRutaLogo = config.strRutaNombreLogo;
                }

                blnImagenSIBO = config.bitImagenSIBO;

                if (!string.IsNullOrEmpty(strColorPpal))
                {
                    strColorPpal = config.strColorPpal;
                }

                if (!string.IsNullOrEmpty(strRutaGuiaMarcacion))
                {
                    strRutaGuiaMarcacion = config.strRutaGuiaMarcacion;
                }

                blnBloqueoNoCitaMSW = config.bitBloqueoCita;
                blnBloqueoNoAptoMSW = config.bitBloqueoClienteNoApto;
                blnBloqueoNoConsentimiento = config.bitBloqueoNoDisentimento;
                blnBloqueoNoAutorizacionMenor = config.bitBloqueoNoAutorizacionMenor;
                blnAntipassbackEntrada = config.bitAntipassbackEntrada;
                blnImprimirHoraReserva = config.bitImprimirHoraReserva;
                blnTiqueteClaseAsistido_alImprimir = config.bitTiqueteClaseAsistido_alImprimir;
                blnAccesoXReservaWeb = config.bitAccesoPorReservaWeb;
                intTiempoAntesAccesoXReservaWeb = config.intMinutosAntesReserva;
                intTiempoDespuesAccesoXReservaWeb = config.intMinutosDespuesReserva;
                blnAccesoDUAL_Tradicional_y_XReservaWeb = config.bitValidarPlanYReservaWeb;
                blnNoValidarEntradaSalida = config.bitNo_Validar_Entrada_En_Salida;
                blnEsperarHuellaActualizar = config.bitEsperarHuellaActualizar;
                blnIngresoAbreDesdeTouch = config.bitIngresoAbreDesdeTouch;
                blnNoValidarHuella = config.bitNoValidarHuella;
                signContracteToEnroll = config.bitFirmarContratoAlEnrolar;
                generatePDFContractAndSend = config.bitGenerarContratoPDFyEnviar;

                if (config.intTiempoReplicaHuellasTCAM7000 > 0)
                {
                    intTiempoReplicaHuellasTCAM7000 = config.intTiempoReplicaHuellasTCAM7000 * oneSecond;
                }

                if (config.timeGetPendingActions > 0)
                {
                    timeGetPendingActions = config.timeGetPendingActions * oneSecond;
                }

                if (config.timeRemoveFingerprints > 0)
                {
                    timeRemoveFingerprints = config.timeRemoveFingerprints * oneSecond;
                }

                allowWhiteListTCAM = config.allowWhiteListTCAM;

                if (config.timeInsertWhiteListTCAM > 0)
                {
                    timeWhitelistTCAM = config.timeInsertWhiteListTCAM * oneSecond;
                }

                //Nuevos parámetros
                if (config.timeTerminalConnections > 0)
                {
                    timeTerminalConnections = config.timeTerminalConnections * oneSecond;
                }

                if (config.intTiempoEsperaRespuestaReplicaHuellasTCAM7000 > 0)
                {
                    intTiempoEsperaRespuestaReplicaHuellasTCAM7000 = config.intTiempoEsperaRespuestaReplicaHuellasTCAM7000 * oneSecond;
                }

                if (config.timeWaitResponseReplicateUsers > 0)
                {
                    timeWaitResponseReplicateUsers = config.timeWaitResponseReplicateUsers * oneSecond;
                }

                if (config.timeWaitResponseDeleteFingerprint > 0)
                {
                    timeWaitResponseDeleteFingerprint = config.timeWaitResponseDeleteFingerprint * oneSecond;
                }

                if (config.timeWaitResponseDeleteUser > 0)
                {
                    timeWaitResponseDeleteUser = config.timeWaitResponseDeleteUser * oneSecond;
                }

                if (config.timeResetDownloadEvents > 0)
                {
                    timeResetDownloadEvents = config.timeResetDownloadEvents * oneSecond;
                }

                if (config.timeRemoveUsers > 0)
                {
                    timeRemoveUsers = config.timeRemoveUsers * oneSecond;
                }

                if (config.timeDowndloadEvents > 0)
                {
                    timeDowndloadEvents = config.timeDowndloadEvents * oneSecond;
                }

                if (config.timeHourSync > 0)
                {
                    timeHourSync = config.timeHourSync * oneSecond;
                }
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores de las terminales.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                actionService = clsEnum.ServiceActions.WaitingClientId;
            }
        }

        private void TmPingContinuoTCAM7000_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //ContinuousPing();
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        private void ConnectTCAM()
        {
            try
            {
                ConnectSocket();
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        private void ContinuousPing()
        {
            Ping p = new Ping();
            string ipAddressAux = string.Empty;

            try
            {
                tmPingContinuoTCAM7000.Stop();

                if (terminalObjectList != null && terminalObjectList.Count > 0)
                {
                    foreach (clsWinSockCliente item in terminalObjectList)
                    {
                        ipAddressAux = item.IpAddress;
                        PingReply pr = p.Send(item.IpAddress, 800);

                        if (pr.Status.ToString().Equals("Success"))
                        {
                            if (!item.connected)
                            {
                                item.connected = true;
                            }
                        }
                        else
                        {
                            if (item.connected)
                            {
                                item.connected = false;
                                log.WriteProcess(terminalDisconnected + " - Dirección Ip: " + item.IpAddress);
                            }
                        }
                    }
                }

                tmPingContinuoTCAM7000.Start();
            }
            catch (Exception ex)
            {
                tmPingContinuoTCAM7000.Start();
                throw ex;
            }
        }

        private void ConnectSocket()
        {
            string lastPart = string.Empty, ipAddressAux = string.Empty;

            if (termList != null && termList.Count > 0)
            {
                foreach (eTerminal item in termList)
                {
                    if (item.state)
                    {
                        ipAddressAux = item.ipAddress;

                        if (!string.IsNullOrEmpty(item.ipAddress))
                        {
                            log.WriteProcess("Inicia el proceso de conexión de la terminal " + item.ipAddress);

                            clsWinSockCliente exist = terminalObjectList.Find(tol => tol.IpAddress == ipAddressAux);

                            if (exist != null && exist.connected)
                            {
                                log.WriteProcess("La terminal " + item.ipAddress + " ya se encuentra conectada.");
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.port))
                                {
                                    winSocketClient = new clsWinSockCliente(item.ipAddress, Convert.ToInt32(item.port), item.withWhiteList, false, item.terminalTypeId);

                                    if (winSocketClient.connected)
                                    {
                                        terminalObjectList.Add(winSocketClient);
                                    }

                                    winSocketClient.ReceivedData += WinSocketClient_ReceivedData;
                                }
                                else
                                {
                                    log.WriteProcess("No es posible realizar la conexión de la terminal " + item.ipAddress + ", debido a que no está configurado el puerto de esta.");
                                }
                            }

                            log.WriteProcess("Finaliza el proceso de conexión de la terminal " + item.ipAddress);
                        }
                    }
                    else
                    {
                        log.WriteProcess("No es posible realizar la conexión de la terminal: " + item.ipAddress + ", debido a que no está activa en la BD.");
                    }
                }
            }
            else
            {
                log.WriteProcess("No es posible conectar terminales debido a que no se encontraron en la BD local o no fue posible consultarlas.");
            }
        }

        int quantityEvents = 0;

        private async void WinSocketClient_ReceivedData(string ipAddress, byte[] data)
        {
            try
            {
                if (data != null)
                {
                    log.WriteTerminals("-> DATA: " + Encoding.ASCII.GetString(data), ipAddress);
                }
                else
                {
                    log.WriteTerminals("-> LLEGA DATO NULO.", ipAddress);
                }

                EventBLL entryBll = new EventBLL();
                objData = null;
                string strDatosRecibidos = string.Empty, userId = string.Empty;
                string[] datosSeparados;
                int intUserId = 0;

                if (actualState != States.esperandoImagenHuella)
                {
                    if (data.Length == 1025 && Encoding.ASCII.GetString(data).Trim().Length == 0)
                    {
                        await Task.Run(() => FinalizeConnection(ipAddress));
                        return;
                    }
                }

                if (objData == null)
                {
                    objData = data;
                }
                else
                {
                    byte[] aux;
                    aux = new byte[objData.Length + data.Length];

                    if (objData != null)
                    {
                        //log.WriteProcess("Getulio Vargas " + objData.Length.ToString() + " - " + aux.Length.ToString());
                        Array.Copy(objData, aux, Math.Min(objData.Length, aux.Length));
                    }

                    objData = aux;
                    //log.WriteProcess("Getulio Vargas " + (objData.Length - data.Length).ToString());
                    Array.Copy(data, 0, objData, (objData.Length - data.Length), data.Length);
                }

                //Application.DoEvents();
                //Dim err As New clsEscribirErrorLog("<" & Encoding.ASCII.GetString(datos) & " fecha: " & Now, IP_TCAM7000)

                if ((objData.Length == 16 || objData.Length == 17 || objData.Length == 20) &&
                    (actualState == States.esperandoConfirmacion || actualState == States.esperandoConfirmacionHuella || actualState == States.esperandoHuella ||
                     actualState == States.esperandoConfirmacionPlanesVig || actualState == States.esperandoPlanVigente || actualState == States.esperandoConfirmacionIngSinHuella ||
                     actualState == States.esperandoConfirmacionReplicaHuella))
                {
                    actualState = States.esperandoIDCliente;
                    tmReplicaHuellasTCAM7000.Start();

                    if (blnReplicaImgsTCAM7000)
                    {
                        tmReplicaImgsTCAM7000.Start();
                    }
                }

                if (objData.Length == 10)
                {
                    if (Encoding.ASCII.GetString(objData).IndexOf("@ERROR$") >= 0)
                    {
                        actualState = States.esperandoIDCliente;
                        objData = null;
                        return;
                    }
                }

                if (objData.Length == 17 && Encoding.ASCII.GetString(objData).Substring(1, 1) == "U")
                {
                    intUserId = Convert.ToInt32(Encoding.ASCII.GetString(objData).Substring(2, 12).ToString());
                    userId = intUserId.ToString();
                    actionService = clsEnum.ServiceActions.EnrollFromTerminal;
                }

                switch (actionService)
                {
                    //Acción que se ejecuta cuando se da la orden de grabar una huella en la terminal TCAM7000.
                    //Se cambia el estado del servicio a "WaitingFingerprint", para que luego de la grabación reciba la huella para grabarla.
                    case clsEnum.ServiceActions.WaitingFingerprintConfirm:
                        if (objData.Length == 6 && Encoding.ASCII.GetString(objData).Substring(0, 1) == "@" && Encoding.ASCII.GetString(objData).Substring(objData.Length - 3, 1) == "$")
                        {
                            log.WriteProcessByTerminal("LLEGA CONFIRMACIÓN DE ID DE HUELLA", ipAddress);
                            actionService = clsEnum.ServiceActions.WaitingFingerprint;
                        }

                        objData = null;
                        break;
                    //Acción que se ejecuta cuando la huella fue grabada en la terminal en la TCAM7000.
                    case clsEnum.ServiceActions.WaitingFingerprint:
                        if (objData.Length == 771 && Encoding.ASCII.GetString(objData).Substring(objData.Length - 3, 1) == "$")
                        {
                            byte[] fingerprintBytes = new byte[1024];
                            string temp = string.Empty;
                            log.WriteProcessByTerminal("LLEGA HUELLA DEL CLIENTE", ipAddress);
                            temp = Encoding.ASCII.GetString(objData).Substring(0, 768);
                            log.WriteProcessByTerminal("Huella a grabar", ipAddress);
                            log.WriteProcessByTerminal(temp, ipAddress);
                            fingerprintBytes = Encoding.ASCII.GetBytes(temp);
                            RecordFingerprint(fingerprintBytes, ipAddress);

                            if (!signContracteToEnroll)
                            {
                                actionService = clsEnum.ServiceActions.WaitingClientId;
                            }
                            else
                            {
                                actionService = clsEnum.ServiceActions.WaitingFingerprintImage;
                            }

                            tmReplicaHuellasTCAM7000.Start();

                            if (blnReplicaImgsTCAM7000)
                            {
                                tmReplicaImgsTCAM7000.Start();
                            }
                        }
                        break;
                    case clsEnum.ServiceActions.EnrollFromTerminal:
                        recordingFromterminal = true;
                        ValidateRecordFingerprint(userId, ipAddress);
                        break;
                    case clsEnum.ServiceActions.WaitingFingerprintImage:
                        if (objData != null)
                        {
                            strImageFingerprint += Encoding.ASCII.GetString(objData);
                        }

                        log.WriteProcessByTerminal(strImageFingerprint, ipAddress);

                        if (strImageFingerprint.IndexOf("IMAGEN") >= 0 && strImageFingerprint.IndexOf("$") >= 0)
                        {
                            log.WriteProcessByTerminal("LLEGA LA IMAGEN DE LA HUELLA DEL CLIENTE", ipAddress);
                            string fingerprintInAscii = strImageFingerprint.Substring(strImageFingerprint.LastIndexOf(";") + 2, 20736);
                            GenerateFingerprintImage(fingerprintInAscii, generalUserIdToSigningContract);

                            tmReplicaHuellasTCAM7000.Start();

                            if (blnReplicaImgsTCAM7000)
                            {
                                tmReplicaImgsTCAM7000.Start();
                            }
                        }

                        actionService = clsEnum.ServiceActions.WaitingClientId;

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.WaitingClientId:
                        if (objData.Length == 786)
                        {
                            bool existFingerprint = false;
                            string strReceivedData = Encoding.ASCII.GetString(objData);
                            string[] separatedData = strReceivedData.Split(';');
                            GetTemplates(separatedData[0].Replace("@V", ""));
                            AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                            eConfiguration config = new eConfiguration();
                            config = acsBll.GetLocalAccessControlSettings();
                            fingerprintBytes = Encoding.ASCII.GetBytes(separatedData[1].Replace("$", ""));
                            fingerprintBytes = ConvertFingerprint(fingerprintBytes, ipAddress);

                            //Probar con terminal, MT Incidente 0005921
                            //Falta agregar el timeout desde: Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver
                            if (config.Validación_de_huella_de_marcación_con_SDK_del_webserver)
                            {
                                if (dtHuellas != null && dtHuellas.Rows.Count > 0)
                                {
                                    string id = dtHuellas.Rows[index]["id"].ToString();
                                    string strGymId = System.Configuration.ConfigurationManager.AppSettings["gymId"].ToString();

                                    int gymId = Convert.ToInt32(strGymId);

                                    wsGSW.Generic1SoapClient wsGymsoft = new wsGSW.Generic1SoapClient();
                                    string idcliente = string.Empty;
                                    idcliente = wsGymsoft.validatefootprint(gymId, id, fingerprintBytes);

                                    if (!string.IsNullOrEmpty(idcliente))
                                    {
                                        if (idcliente.Length < 12)
                                        {
                                            id = "@" + (new string(' ', (12 - idcliente.Length)) + id).Replace(" ", "0") + "$" + (char)0 + (char)0;
                                        }
                                        else
                                        {
                                            id = "@" + idcliente + "$" + (char)0 + (char)0;
                                        }

                                        objData = Encoding.ASCII.GetBytes(idcliente);

                                    }
                                    else
                                    {
                                        SendDataAndLogs("@DHUELLA NO COINCIDE                                                                                                                          $", ipAddress);
                                    }
                                }
                                else
                                {
                                    SendDataAndLogs("@DHUELLA NO COINCIDE                                                                                                                          $", ipAddress);

                                }
                            }
                            //Probar con terminal, MT Incidente 0005921
                            //Si tiene errores salir del if y quitar el else de abajo
                            else
                            {
                                if (!config.bitValidarHuellaMarcacionSDKAPI)
                                {
                                    if (dtHuellas != null && dtHuellas.Rows.Count > 0)
                                    {
                                        existFingerprint = IdentifyExistFingerprint(fingerprintBytes);

                                        if (existFingerprint)
                                        {
                                            string id = dtHuellas.Rows[index]["id"].ToString();

                                            if (id.Length < 12)
                                            {
                                                id = "@" + (new string(' ', (12 - id.Length)) + id).Replace(" ", "0") + "$" + (char)0 + (char)0;
                                            }
                                            else
                                            {
                                                id = "@" + id + "$" + (char)0 + (char)0;
                                            }

                                            objData = Encoding.ASCII.GetBytes(id);
                                        }
                                        else
                                        {
                                            SendDataAndLogs("@DHUELLA NO COINCIDE                                                                                                                          $", ipAddress);
                                        }
                                    }
                                    else
                                    {
                                        SendDataAndLogs("@DHUELLA NO COINCIDE                                                                                                                          $", ipAddress);
                                    }
                                }
                                else
                                {
                                    string personId = string.Empty;
                                    FingerprintBLL fingerBLL = new FingerprintBLL();
                                    personId = fingerBLL.ValidateFingerprint(gymId, personId, fingerprintBytes, (config.intTimeoutValidarHuellaMarcacionSDKAPI * oneSecond));

                                    if (string.IsNullOrEmpty(personId))
                                    {
                                        SendDataAndLogs("@DHUELLA NO COINCIDE                                                                                                                          $", ipAddress);
                                        return;
                                    }

                                    actionService = clsEnum.ServiceActions.WaitingClientId;
                                }
                            }


                        }

                        if ((objData.Length == 16 || objData.Length == 17 || objData.Length == 20) && Encoding.ASCII.GetString(objData).Substring(0, 1) == "@" &&
                            Encoding.ASCII.GetString(objData).Substring(objData.Length - 3, 1) == "$" && Encoding.ASCII.GetString(objData).Substring(1, 1) != "Q")
                        {
                            log.WriteProcessByTerminal("LLEGA ID CLIENTE", ipAddress);
                            lastActionService = clsEnum.ServiceActions.WaitingClientId;
                            actionService = clsEnum.ServiceActions.WaitingConfirm;

                            switch (objData.Length)
                            {
                                case 16:
                                    ValidateEntryByUserId(Encoding.ASCII.GetString(objData).Substring(1, 12), ipAddress);
                                    break;
                                case 17:
                                    ValidateEntryByUserId(Encoding.ASCII.GetString(objData).Substring(1, 13), ipAddress);
                                    break;
                                case 20:
                                    if (Encoding.ASCII.GetString(objData).Substring(3, 1) == "I" || Encoding.ASCII.GetString(objData).Substring(3, 1) == "O")
                                    {
                                        ValidateEntryByUserId("T-" + Encoding.ASCII.GetString(objData).Substring(5, 12), ipAddress);
                                    }
                                    break;
                            }
                        }
                        else if ((objData.Length == 6 || objData.Length == 23) && Encoding.ASCII.GetString(objData).Substring(0, 1) == "@" &&
                                 Encoding.ASCII.GetString(objData).Substring(objData.Length - 3, 1) == "$")
                        {
                            log.WriteProcessByTerminal("LLEGA OK CONFIRMACION APERTURA/NO APERTURA", ipAddress);
                            actionService = clsEnum.ServiceActions.WaitingClientId;

                            tmReplicaHuellasTCAM7000.Start();

                            if (blnReplicaImgsTCAM7000)
                            {
                                tmReplicaImgsTCAM7000.Start();
                            }
                        }
                        //MToro: Caso de Qr y temperatura
                        //Qr y temperatura => EJ: @Q0000000M3T4k;35.4$
                        else if (objData.Length == 22 && Encoding.ASCII.GetString(objData).Substring(0, 1) == "@" && Encoding.ASCII.GetString(objData).Substring(1, 1) == "Q")
                        {
                            string strGymId = System.Configuration.ConfigurationManager.AppSettings["gymId"].ToString();
                            if (!string.IsNullOrEmpty(strGymId))
                            {
                                gymId = Convert.ToInt32(strGymId);
                            }
                            string qr = Encoding.ASCII.GetString(objData).Substring(objData.Length - 13 , 5).ToString();
                            string temperature = Encoding.ASCII.GetString(objData).Substring(objData.Length - 7 , 4).ToString();
                            WhiteListBLL wlBll = new WhiteListBLL();
                            string userIdentifi = wlBll.GetClientByQr(qr, gymId);

                            ValidateEntryByUserId(userIdentifi, ipAddress, "qrAndTemp", qr, temperature);
                            actionService = clsEnum.ServiceActions.WaitingClientId;
                            wlBll.InactivateQrCode(qr, true);
                        }
                        //QR Con temperatura ANORMAL => EJ: @Q0000000M3T4k;39.4$
                        else if (objData.Length == 22 && Encoding.ASCII.GetString(objData).Substring(0, 1) == "@" && Encoding.ASCII.GetString(objData).Substring(1, 1) == "D")
                        {
                            string qr = Encoding.ASCII.GetString(objData).Substring(objData.Length - 13, 5).ToString();
                            string temperature = Encoding.ASCII.GetString(objData).Substring(objData.Length - 7, 4).ToString();
                            WhiteListBLL wlBll = new WhiteListBLL();
                            string userIdentifi = wlBll.GetClientByQr(qr, gymId, true);
                            ValidateEntryByUserId(userIdentifi, ipAddress, "DeniedBcsTemp", qr, temperature);
                            actionService = clsEnum.ServiceActions.WaitingClientId;
                            wlBll.InactivateQrCode(qr, true);                            

                        }
                        //QR Sin Temperatura => EJ: @Q############$
                        else if (objData.Length == 17 && Encoding.ASCII.GetString(objData).Substring(0, 1) == "@" && Encoding.ASCII.GetString(objData).Substring(1, 1) == "Q")
                        {
                            string qr = Encoding.ASCII.GetString(objData).Substring(objData.Length - 8, 5).ToString();
                            WhiteListBLL wlBll = new WhiteListBLL();
                            string userIdentifi = wlBll.GetClientByQr(qr, gymId);
                            ValidateEntryByUserId(userIdentifi, ipAddress, "justQr", qr);
                            actionService = clsEnum.ServiceActions.WaitingClientId;
                            wlBll.InactivateQrCode(qr, true);
                        }
                        //FIN MToro: Caso de Qr y temperatura

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.WaitingConfirm:
                        if ((objData.Length == 6 || objData.Length == 23) && Encoding.ASCII.GetString(objData).Substring(0, 1) == "@" &&
                            Encoding.ASCII.GetString(objData).Substring(objData.Length - 3, 1) == "$")
                        {
                            log.WriteProcessByTerminal("LLEGA OK CONFIRMACION APERTURA/NO APERTURA", ipAddress);
                            actionService = clsEnum.ServiceActions.WaitingClientId;

                            tmReplicaHuellasTCAM7000.Start();

                            if (blnReplicaImgsTCAM7000)
                            {
                                tmReplicaImgsTCAM7000.Start();
                            }
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.WaitingConfirmFingerprintReplicate:
                        if (objData.Length == 6 || objData.Length == 10)
                        {
                            if (Encoding.ASCII.GetString(objData).Substring(0, 4) == "@OK$")
                            {
                                tmEsperaRespuestaTCAM7000.Stop();
                                log.WriteProcessByTerminal("LLEGA OK CONFIRMACIÓN RÉPLICA HUELLA", ipAddress);
                                ReplicateFingerprints(ipAddress, true, true);
                            }
                            else
                            {
                                actionService = clsEnum.ServiceActions.WaitingErrorCode;
                            }
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.waitingConfirmDownloadEvents:
                        string val = Encoding.ASCII.GetString(objData);

                        if (!val.Contains("NO HAY MARCACIONES"))
                        {
                            events += Encoding.ASCII.GetString(objData);

                            if (quantityEvents == 0)
                            {
                                quantityEvents = Convert.ToInt32(events.Substring(0, 5));
                            }

                            if (events.Contains("$"))
                            {
                                tmResetDownloadEvents.Stop();
                                log.WriteProcessByTerminal("LLEGA OK DESCARGA DE MARCACIONES", ipAddress);
                                DownloadEvents(ipAddress, true, true, false);
                            }
                        }
                        else
                        {
                            log.WriteProcessByTerminal("NO HAY MARCACIONES POR DESCARGAR DE LA TERMINAL", ipAddress);
                            actionService = clsEnum.ServiceActions.WaitingErrorCodeDownloadEvents;
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.waitingConfirmDeleteEvents:
                        if (objData.Length == 6 || objData.Length == 10)
                        {
                            if (Encoding.ASCII.GetString(objData).Substring(0, 4) == "@OK$")
                            {
                                log.WriteProcessByTerminal("LLEGA OK CONFIRMACIÓN DE BORRADO DE EVENTOS DE LA TERMINAL", ipAddress);
                                DownloadEvents(terminalObjectList.Find(tol => tol.IpAddress == ipAddress).IpAddress, true, true, true);
                            }
                        }
                        else
                        {
                            log.WriteProcessByTerminal("NO SE BORRARON LAS MARCACIONES DE LA TERMINAL", ipAddress);
                            actionService = clsEnum.ServiceActions.WaitingErrorCodeConfirmDeleteEvents;
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.waitingConfirmUserReplicate:
                        if (objData.Length == 6 || objData.Length == 10)
                        {
                            if (Encoding.ASCII.GetString(objData).Substring(0, 4) == "@OK$")
                            {
                                tmWaitResponseReplicateUsers.Stop();
                                log.WriteProcessByTerminal("LLEGA OK CONFIRMACIÓN RÉPLICA USUARIOS", ipAddress);
                                actionService = clsEnum.ServiceActions.WaitingClientId;
                                ReplicateUsersWhitelist(terminalObjectList.Find(tol => tol.replicating == true).IpAddress, true, true);
                            }
                            else
                            {
                                actionService = clsEnum.ServiceActions.WaitingErrorCodeUsers;
                            }
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.waitingConfirmUserDelete:
                        if (objData.Length == 6 || objData.Length == 10)
                        {
                            if (Encoding.ASCII.GetString(objData).Substring(0, 4) == "@OK$")
                            {
                                tmWaitResponseDeleteUser.Stop();
                                log.WriteProcessByTerminal("LLEGA OK CONFIRMACIÓN ELIMINACIÓN USUARIOS", ipAddress);
                                actionService = clsEnum.ServiceActions.WaitingClientId;
                                DeleteUsers(terminalObjectList.Find(tol => tol.replicating == true).IpAddress, true, true);
                            }
                            else
                            {
                                actionService = clsEnum.ServiceActions.waitingErrorCodeUserDelete;
                            }
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.WaitingErrorCode:
                        tmEsperaRespuestaTCAM7000.Stop();

                        if (terminalObjectList.Find(tol => tol.replicating == true) != null)
                        {
                            ReplicateFingerprints(terminalObjectList.Find(tol => tol.replicating == true).IpAddress, true, false);
                        }
                        else
                        {
                            tmReplicaHuellasTCAM7000.Start();
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.WaitingErrorCodeDownloadEvents:
                        tmResetDownloadEvents.Stop();

                        if (terminalObjectList.Find(tol => tol.replicating == true) != null)
                        {
                            DownloadEvents(terminalObjectList.Find(tol => tol.replicating == true).IpAddress, true, false, false);
                        }
                        else
                        {
                            tmDownloadEvents.Start();
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.WaitingErrorCodeConfirmDeleteEvents:

                        if (terminalObjectList.Find(tol => tol.replicating == true) != null)
                        {
                            DownloadEvents(terminalObjectList.Find(tol => tol.replicating == true).IpAddress, true, false, false);
                        }
                        else
                        {
                            tmDownloadEvents.Start();
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.WaitingErrorCodeUsers:
                        tmWaitResponseReplicateUsers.Stop();

                        if (terminalObjectList.Find(tol => tol.replicating == true) != null)
                        {
                            ReplicateUsersWhitelist(terminalObjectList.Find(tol => tol.replicating == true).IpAddress, true, false);
                        }
                        else
                        {
                            tmWhiteListTCAM.Start();
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.waitingErrorCodeUserDelete:
                        tmWaitResponseDeleteUser.Stop();

                        if (terminalObjectList.Find(tol => tol.replicating == true) != null)
                        {
                            DeleteUsers(terminalObjectList.Find(tol => tol.replicating == true).IpAddress, true, false);
                        }
                        else
                        {
                            timerRemoveUsers.Start();
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.WaitingErrorCodeFingerprintsDelete:
                        tmWaitResponseDeleteFingerprints.Stop();

                        if (terminalObjectList.Find(tol => tol.replicating == true) != null)
                        {
                            DeleteFingerprints(terminalObjectList.Find(tol => tol.replicating == true).IpAddress, true, false);
                        }
                        else
                        {
                            timerRemoveFingerprints.Start();
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.WaitingRemoveFingerprints:
                        if (objData.Length == 6 || objData.Length == 10)
                        {
                            if (Encoding.ASCII.GetString(objData).Substring(0, 4) == "@OK$")
                            {
                                tmWaitResponseDeleteFingerprints.Stop();
                                log.WriteProcessByTerminal("LLEGA OK CONFIRMACIÓN DE BORRADO DE HUELLAS DE LA TERMINAL", ipAddress);
                                actionService = clsEnum.ServiceActions.WaitingClientId;
                                DeleteFingerprints(terminalObjectList.Find(tol => tol.replicating == true).IpAddress, true, true);
                            }
                            else
                            {
                                log.WriteProcessByTerminal("NO FUE POSIBLE REALIZAR EL BORRADO DE LAS HUELLAS EN LA TERMINAL", ipAddress);
                                actionService = clsEnum.ServiceActions.WaitingErrorCodeFingerprintsDelete;
                            }
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.WaitingConfirmReplicationImg:
                        if (objData.Length == 6 && Encoding.ASCII.GetString(objData).Substring(0, 1) == "@" && Encoding.ASCII.GetString(objData).Substring(objData.Length - 3, 1) == "$")
                        {
                            log.WriteProcessByTerminal("LLEGA OK CONFIRMACIÓN REÉLICA TRAMA DE IMAGEN", ipAddress);
                            actionService = clsEnum.ServiceActions.WaitingClientId;
                            swRespuestaTramaOK = true;
                        }

                        objData = null;
                        break;
                    case clsEnum.ServiceActions.waitingConfirmHourSync:
                        if (objData.Length == 6 || objData.Length == 10)
                        {
                            if (Encoding.ASCII.GetString(objData).Substring(0, 4) == "@OK$")
                            {
                                log.WriteProcessByTerminal("LLEGA OK CONFIRMACIÓN DE SINCRONIZACIÓN DE HORA.", ipAddress);
                            }
                            else
                            {
                                log.WriteProcessByTerminal("NO FUE POSIBLE SINCRONIZAR LA HORA.", ipAddress);
                            }
                        }
                        else
                        {
                            log.WriteProcessByTerminal("NO FUE POSIBLE SINCRONIZAR LA HORA. ", ipAddress);
                        }

                        if (terminalCounterHourSync == terminalObjectList.FindAll(tol => tol.withWithelist == true).ToArray().Length)
                        {
                            terminalCounterHourSync = 0;
                            tmHourSync.Start();
                        }

                        actionService = clsEnum.ServiceActions.WaitingClientId;
                        objData = null;
                        break;
                    case clsEnum.ServiceActions.turnOnLector:
                        if (objData.Length == 6 || objData.Length == 10)
                        {
                            if (Encoding.ASCII.GetString(objData).Substring(0, 4) == "@OK$")
                            {
                                log.WriteProcessByTerminal("LLEGA OK CONFIRMACIÓN DE ENCENDIDO DE LECTOR PARA LA TERMINAL.", ipAddress);
                            }
                            else
                            {
                                log.WriteProcessByTerminal("NO FUE POSIBLE ENCENDER EL LECTOR DE LA TERMINAL.", ipAddress);
                            }
                        }
                        else
                        {
                            log.WriteProcessByTerminal("NO FUE POSIBLE ENCENDER EL LECTOR DE LA TERMINAL.", ipAddress);
                        }

                        break;
                    case clsEnum.ServiceActions.turnOffLector:
                        if (objData.Length == 6 || objData.Length == 10)
                        {
                            if (Encoding.ASCII.GetString(objData).Substring(0, 4) == "@OK$")
                            {
                                log.WriteProcessByTerminal("LLEGA OK CONFIRMACIÓN DE APAGADO DE LECTOR PARA LA TERMINAL", ipAddress);
                            }
                            else
                            {
                                log.WriteProcessByTerminal("NO FUE POSIBLE APAGAR EL LECTOR DE LA TERMINAL.", ipAddress);
                            }
                        }
                        else
                        {
                            log.WriteProcessByTerminal("NO FUE POSIBLE APAGAR EL LECTOR DE LA TERMINAL.", ipAddress);
                        }

                        break;
                    default:
                        actionService = clsEnum.ServiceActions.WaitingClientId;
                        break;
                }
            }
            catch (Exception ex)
            {
                log.WriteErrorsByTerminals(ex.Message + " " + ex.StackTrace.ToString(), ipAddress);
                actionService = clsEnum.ServiceActions.WaitingClientId;
            }
        }
        

        private void ValidateEntryByUserId(
            string id, 
            string ipAddress, 
            string qrType = null, 
            string qrCode = null, 
            string temperature = null)
        {
            try
            {
                bool resp = false, swWithoutFingerprint = false, isId = false, isExit = false;
                string originalId = string.Empty;
                clsWhiteList wlCLS = new clsWhiteList(qrCode, temperature);
                List<eClientMessages> cmList = new List<eClientMessages>();
                ClientMessagesBLL cmBll = new ClientMessagesBLL();
                eConfiguration entryConfig = new eConfiguration();
                AccessControlSettingsBLL acsBLL = new AccessControlSettingsBLL();
                entryConfig = acsBLL.GetLocalAccessControlSettings();

                if (termList.Find(t => t.ipAddress == ipAddress) != null)
                {
                    isExit = termList.Find(t => t.ipAddress == ipAddress).servesToOutput;
                }

                log.WriteProcessByTerminal("Aceptar ID: " + id, ipAddress);

                if(qrType == "DeniedBcsTemp")
                {
                    WhiteListBLL whiteListBLL = new WhiteListBLL();
                    whiteListBLL.InsertDeniedEventByHightTemperature(id, qrCode, temperature, true, ipAddress);
                    return;
                }
                else if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(qrCode))
                {
                    wlCLS.InsertDeniedEventByNotQr(qrCode, temperature, true, ipAddress);
                    GenerateOpeningCommand(false, ipAddress, id, "USUARIO NO ENCONTRADO O QR VENCIDO.", isExit);
                }

                if (id.StartsWith("T-"))
                {
                    originalId = id;
                    swIngresoTarjeta = true;
                    id = originalId.Substring(2);
                    id = GetClientIdByCardId(id, ipAddress);

                    if (string.IsNullOrEmpty(id))
                    {
                        id = originalId.Substring(2);
                        id = wlCLS.GetEmployeeIdByCardId(id, ipAddress);
                    }
                }
                else
                {
                    swIngresoTarjeta = false;
                }

                if (id.Length == 13)
                {
                    swWithoutFingerprint = true;
                    id = id.Substring(1);
                    isId = true;
                }
                else if (id.Length == 12)
                {
                    swWithoutFingerprint = false;
                    id = wlCLS.GetClientIdByFingerprintId(Convert.ToInt32(id), ipAddress);
                    isId = false;
                }

                if (!isId && string.IsNullOrEmpty(id))
                {
                    GenerateOpeningCommand(resp, ipAddress, id, "EL USUARIO NO EXISTE EN LA LISTA BLANCA.", isExit);
                }
                else
                {
                    cmList = cmBll.GetLocalCLientMessages();
                    string messageToClient = string.Empty, messageClientId = string.Empty, messageType = "0", messageClientOrder = string.Empty;
                    int timeMessageClient = 0;

                    if (cmList != null && cmList.Count > 0)
                    {
                        foreach (eClientMessages item in cmList)
                        {
                            if (!string.IsNullOrEmpty(item.messageText))
                            {
                                messageToClient = item.messageText;
                            }

                            timeMessageClient = item.messageDurationTime;
                            messageClientId = item.messageId.ToString();
                            messageType = item.messageType;
                            messageClientOrder = item.messageImgOrder;
                        }
                    }

                    if (id.Trim().Length > 0)
                    {
                        if (entryConfig.bitValidarConfiguracionIngresoWeb)
                        {
                            string validationResponse = acsBLL.ValidateEntryData(gymId, branchId, id);

                            if (validationResponse != "OK")
                            {
                                log.WriteProcessByTerminal("No se permitirá el ingreso de la persona con identificación: " + id + "Debido a que: " + validationResponse, ipAddress);
                                actionService = clsEnum.ServiceActions.ValidationDataError;
                                resp = false;
                                GenerateOpeningCommand(resp, ipAddress, id, string.Empty, isExit);
                                return;
                            }
                        }

                        //Validar el parámetro blnComplices_DescuentoTiq
                        if (swIngresoTarjeta)
                        {
                            swWithoutFingerprint = false;
                        }

                        if (!isExit)
                        {
                            if (!blnNoValidarEntradaSalida)
                            {
                                //Validamos si la persona tiene una entrada anterior SIN SALIDA, en caso de ser así se bloquea el ingreso de esta.
                                //Funcionalidad basada en el parámetro "blnAntipassbackEntrada"
                                if (!wlCLS.ValidateEntryWithoutExit(id, ipAddress))
                                {
                                    actionService = clsEnum.ServiceActions.WaitingClientId;
                                    resp = false;
                                    GenerateOpeningCommand(resp, ipAddress, id, string.Empty, isExit);
                                    return;
                                }
                            }

                            if (!blnAntipassbackEntrada || (blnAntipassbackEntrada && !resp))
                            {
                                resp = wlCLS.ValidateEntryByUserId(id, ipAddress, isId);
                            }
                        }
                        else
                        {
                            //Validamos que el usuario pueda salir sin problema.
                            resp = wlCLS.ValidateExitById(id, ipAddress, isId);
                        }

                        GenerateOpeningCommand(resp, ipAddress, id, string.Empty, isExit);
                    }
                    else
                    {
                        GenerateOpeningCommand(resp, ipAddress, id, "DEBE INGRESAR LA IDENTIFICACIÓN", isExit);
                    }
                }
            }
            catch (Exception ex)
            {
                actionService = clsEnum.ServiceActions.WaitingClientId;
                log.WriteProcessByTerminal(ex.Message, ipAddress);
            }
        }


        /// <summary>
        /// Método encargado de consultar la identificación del cliente de acuerdo con el número de tarjeta de este.
        /// Getulio Vargas - 2018-08-28 - OD 1307
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        private string GetClientIdByCardId(string cardId, string ipAddress)
        {
            clsWhiteList wlCLS = new clsWhiteList();
            string clientId = string.Empty;
            clientId = wlCLS.GetClientIdByCardId(cardId, ipAddress);
            return clientId;
        }

        private void GenerateOpeningCommand(bool resp, string ipAddress, string userId, string message, bool isExit)
        {
            int longLine = 20;
            eEvent eventEntity = new eEvent();
            EventBLL eventBll = new EventBLL();
            string msg1 = string.Empty, strName = string.Empty, lastName = string.Empty, plan = string.Empty,
                   lastEntry = string.Empty, command = string.Empty, expiration = string.Empty;
            string[] name;

            if (!string.IsNullOrEmpty(userId))
            {
                eventEntity = eventBll.GetLastEntryByUserId(userId);

                if (!isExit)
                {
                    msg1 = eventEntity.secondMessage.Length > (longLine * 2) ? eventEntity.secondMessage.Substring(0, (longLine * 2)) : eventEntity.secondMessage.PadRight((longLine * 2), ' ').TrimStart().TrimEnd();
                    //Código para dividir el nombre del cliente
                    name = eventEntity.clientName.Split(' ');

                    if (name.Length > 0)
                    {
                        foreach (string str in name)
                        {
                            if (strName.Length <= longLine)
                            {
                                if ((strName.Length + str.Length) <= longLine)
                                {
                                    strName += " " + str;
                                }
                                else
                                {
                                    strName += str;
                                }
                            }
                            else
                            {
                                if ((lastName.Length + str.Length) <= longLine)
                                {
                                    lastName += " " + str;
                                }
                                else
                                {
                                    lastName += " " + str.Substring(0, (longLine - lastName.Length));
                                }
                            }
                        }
                    }

                    strName = strName.Length > longLine ? strName.Substring(0, 20) : strName.PadRight(longLine, ' ').TrimStart();
                    lastName = lastName.Length > longLine ? lastName.Substring(0, 20) : lastName.PadRight(longLine, ' ').TrimStart();

                    //Capturamos las variables adicionales para mostrar los mensajes correspondientes en la TCAM
                    plan = (eventEntity.planName.Length > longLine) ? eventEntity.planName.Substring(0, longLine).TrimStart() : eventEntity.planName.PadRight(longLine, ' ').TrimStart();
                    lastEntry = (eventEntity.dateLastEntry.Length > longLine) ? eventEntity.dateLastEntry.Substring(0, longLine).TrimStart() : eventEntity.dateLastEntry.PadRight(longLine, ' ').TrimStart();
                    expiration = "VENCE: " + eventEntity.expirationDate;
                    expiration = (expiration.Length > longLine) ? expiration.Substring(0, longLine).TrimStart() : expiration.PadRight(longLine, ' ').TrimStart();
                }
                else
                {
                    msg1 = eventEntity.thirdMessage.Length > (longLine * 2) ? eventEntity.thirdMessage.Substring(0, (longLine * 2)) : eventEntity.thirdMessage.PadRight((longLine * 2), ' ').TrimStart().TrimEnd();
                }
            }
            else
            {
                msg1 = message.Length > (longLine * 2) ? message.Substring(0, (longLine * 2)) : message.PadRight((longLine * 2), ' ').TrimStart().TrimEnd();
            }

            if (resp)
            {
                command = "@A";
            }
            else
            {
                command = "@D";
            }

            command += String.Format("{0,-" + (longLine * 2).ToString() + "}", msg1);
            command += String.Format("{0,-" + longLine.ToString() + "}", strName);
            command += String.Format("{0,-" + longLine.ToString() + "}", lastName);
            command += String.Format("{0,-" + longLine.ToString() + "}", plan);
            command += String.Format("{0,-" + longLine.ToString() + "}", lastEntry);
            command += String.Format("{0,-" + longLine.ToString() + "}", expiration);

            //Se debe agregar el campo para mostrar la imagen publicitaria
            command += "$";
            command = command.ToUpper();
            log.WriteProcessByTerminal("COMANDO DE APERTURA: " + command, ipAddress);
            actionService = clsEnum.ServiceActions.WaitingConfirm;
            SendDataAndLogs(command, ipAddress);

            if (tmReplicaHuellasTCAM7000 != null)
            {
                tmReplicaHuellasTCAM7000.Start();

                if (blnReplicaImgsTCAM7000)
                {
                    tmReplicaImgsTCAM7000.Start();
                }
            }
        }

        private bool IdentifyExistFingerprint(byte[] fingerprintBytes)
        {
            Suprema.UFM_STATUS ufm_res = new Suprema.UFM_STATUS();

            if (bdTemplate != null)
            {
                ufm_res = usComparadorUSB.Identify(fingerprintBytes, intTamanoImagen, bdTemplate, dbTemplateSize, bdTemplate.Length, 5000, out index);

                if (ufm_res == Suprema.UFM_STATUS.OK)
                {
                    if (index != -1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        private void GenerateFingerprintImage(string fingerprintInAscii, string userId)
        {
            ImagenTCAM imageTCAM = new ImagenTCAM();
            byte[] image = imageTCAM.FingerprintImage_Suprema_Bytes(fingerprintInAscii);
            //string bmpFile = System.Windows.Forms.Application.StartupPath + "\\Finger.bmp";
            //imageTCAM.FingerprintImage_Suprema_BMP_File(fingerprintInAscii, bmpFile);
            SaveSignedContract(userId, image);
        }

        private void SaveSignedContract(string userId, byte[] image)
        {
            FingerprintBLL fingerBll = new FingerprintBLL();
            fingerBll.SaveSignedContract(gymId, branchId, userId, image, generatePDFContractAndSend);
        }

        /// <summary>
        /// Método para validar si al usuario ingresado se le puede grabar la huella.
        /// Getulio Vargas - 2018-08-22 - OD 1307
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private void ValidateRecordFingerprint(string userId, string ipAddress)
        {
            EventBLL entryBll = new EventBLL();
            string strGymId = string.Empty, strBranchId = string.Empty, message = string.Empty;
            int gymId = 0, branchId = 0;
            bool state = false;
            FingerprintBLL fingerprintBll = new FingerprintBLL();
            eResponse response = new eResponse();
            strGymId = System.Configuration.ConfigurationSettings.AppSettings["gymId"].ToString();
            strBranchId = System.Configuration.ConfigurationSettings.AppSettings["branchId"].ToString();

            if (!string.IsNullOrEmpty(strGymId) && !string.IsNullOrEmpty(strBranchId))
            {
                gymId = Convert.ToInt32(strGymId);
                branchId = Convert.ToInt32(strBranchId);
            }

            if (gymId > 0 && branchId > 0)
            {
                response = fingerprintBll.ValidatePerson(gymId, branchId, userId);
            }

            if (response != null)
            {
                state = response.state;
                message = response.message;
                fingerId = Convert.ToInt32(response.messageTwo);

                //Código para validar si el registro de huella se está realizando desde la terminal; es decir, desde la opción de la terminal "REGISTRO DE USUARIO",
                // y verificar que el cliente no tenga huella registrada; en caso de que el cliente no tenga huella el sistema le permite el registro, en caso contrario no se permite.
                if (recordingFromterminal)
                {
                    if (response.messageThree == "ExistFingerprint")
                    {
                        //Comando para que la terminal no se quede esperando a que termine el tiempo de espera.
                        SendDataAndLogs("@N000000000000$", ipAddress);
                        entryBll.Insert("", userId, "", 0, 0, "", false, 0, null, null, false, "No es posible enrolar el usuario", "",
                                        "El cliente ya tiene huella registrada; para actualizarla, el proceso debe ser realizado por el empleado",
                                        false, ipAddress, "Enroll", string.Empty);
                        recordingFromterminal = false;
                        actionService = clsEnum.ServiceActions.WaitingClientId;
                        return;
                    }
                }

                if (state)
                {
                    actionService = clsEnum.ServiceActions.WaitingFingerprint;
                    eUserRecordFingerprint userRecord = new eUserRecordFingerprint();
                    userRecord.fingerprintId = fingerId;
                    userRecord.ipAddress = ipAddress;
                    userRecord.quality = 100;
                    userRecord.userId = userId;
                    listUserRecord.Add(userRecord);

                    GenerateCommandToEnroll(userId, fingerId, ipAddress);
                }
                else
                {
                    string msg = string.Empty;
                    //Comando para que la terminal no se quede esperando a que termine el tiempo de espera.
                    SendDataAndLogs("@N000000000000$", ipAddress);
                    actionService = clsEnum.ServiceActions.WaitingClientId;

                    switch (message)
                    {
                        case "BlackList":
                            msg = "No es posible grabar la huella del usuario porque se encuentra registrado en la lista blanca.";
                            break;
                        case "NoActiveClient":
                            msg = "No es posible grabar la huella del usuario porque no se encuentra como empleado(a) activo(a) o cliente activo(a).";
                            break;
                        case "NoConfiguration":
                            msg = "No es posible grabar la huella del usuario porque no se encontró la configuración del ingreso para esta sucursal.";
                            break;
                        case "NoVigentPlan":
                            msg = "No es posible grabar la huella del usuario porque no tiene plan vigente en el gimnasio.";
                            break;
                        default:
                            msg = "No es posible grabar la huella del usuario.\nEs posible que no haya conexión con el servidor para validar si se puede grabar la huella de este usuario.";
                            break;
                    }

                    entryBll.Insert("", userId, "", 0, 0, "", false, 0, null, null, false, "No es posible enrolar el usuario", "", msg, false, ipAddress, "Enroll", string.Empty);
                }
            }
        }

        private void SendDataAndLogs(string msg, string ipAddress)
        {
            if (terminalObjectList.Find(tol => tol.IpAddress == ipAddress).connected)
            {
                log.WriteTerminals("<- DATA: " + msg, ipAddress);
                Thread thread = new Thread(() => terminalObjectList.Find(tol => tol.IpAddress == ipAddress).SendData(msg));
                thread.Start();
            }
        }

        private void SendDataAndLogs(byte[] data, string ipAddress)
        {
            if (terminalObjectList.Find(tol => tol.IpAddress == ipAddress).connected)
            {
                log.WriteTerminals("<- DATA: " + Encoding.ASCII.GetString(data), ipAddress);
                terminalObjectList.Find(tol => tol.IpAddress == ipAddress).SendData(data);
            }
        }

        private void RecordFingerprint(byte[] fingerprintBytes, string ipAddress)
        {
            try
            {
                EventBLL entryBll = new EventBLL();
                clsWhiteList wlCLS = new clsWhiteList();
                FingerprintBLL fpBll = new FingerprintBLL();
                ReplicatedFingerprintBLL replicatedBll = new ReplicatedFingerprintBLL();
                FingerprintBLL fingerBll = new FingerprintBLL();
                bool resp = false;
                byte[] tmpFingerprint;
                tmpFingerprint = ConvertFingerprint(fingerprintBytes, ipAddress);

                if (string.IsNullOrEmpty(fingerprintBytes.ToString().Trim()) || tmpFingerprint == null)
                {
                    log.WriteProcessByTerminal("HUELLA VACÍA. NO SE ALMACENA LA HUELLA CLIENTE", ipAddress);
                }
                else
                {
                    eUserRecordFingerprint userRecordToSave = new eUserRecordFingerprint();

                    if (listUserRecord.Count > 0)
                    {
                        userRecordToSave = listUserRecord.Find(ur => ur.ipAddress == ipAddress);
                    }

                    if (userRecordToSave != null)
                    {
                        if (fingerBll.ValidateFingerprintToRecord(gymId, branchId, userRecordToSave.userId, userRecordToSave.fingerprintId, tmpFingerprint, userRecordToSave.quality, true))
                        {
                            entryBll.Insert("", userRecordToSave.userId, "", 0, 0, "", false, 0, null, null, true, "Huella grabada", "",
                                            "La huella del usuario fue grabada de forma correcta", true, ipAddress, "Enroll", string.Empty);
                            resp = fpBll.Insert(userRecordToSave.userId, userRecordToSave.ipAddress, userRecordToSave.fingerprintId, tmpFingerprint, userRecordToSave.quality);
                        }

                        if (resp)
                        {
                            wlCLS.UpdateFingerprint(userRecordToSave.userId, userRecordToSave.fingerprintId, tmpFingerprint, ipAddress);
                            log.WriteProcessByTerminal("SE ALMACENA LA HUELLA CLIENTE", ipAddress);

                            resp = replicatedBll.DeleteFingerprintReplicated(userRecordToSave.fingerprintId, userRecordToSave.userId);

                            if (resp)
                            {
                                log.WriteProcessByTerminal("ELIMINA HUELLAS REPLICADAS DEL USUARIO: " + userRecordToSave.userId, ipAddress);
                            }
                            else
                            {
                                log.WriteProcessByTerminal("NO FUE POSIBLE ELIMINAR LAS HUELLAS REPLICADAS DEL USUARIO " + userRecordToSave.userId, ipAddress);
                            }

                            eReplicatedFingerprint replicatedEntity = new eReplicatedFingerprint();
                            replicatedEntity.fingerprintId = userRecordToSave.fingerprintId;
                            replicatedEntity.ipAddress = userRecordToSave.ipAddress;
                            replicatedEntity.userId = userRecordToSave.userId;
                            resp = replicatedBll.Insert(replicatedEntity);

                            if (resp)
                            {
                                log.WriteProcessByTerminal("SE INSERTÓ EN LA BASE DE DATOS LA RÉPLICA DE LA HUELLA DEL USUARIO: " + userRecordToSave.userId + ", DE LA TERMINAL.", ipAddress);
                            }
                            else
                            {
                                log.WriteProcessByTerminal("NO SE INSERTÓ EN LA BASE DE DATOS LA RÉPLICA DE LA HUELLA DEL USUARIO: " + userRecordToSave.userId + ", DE LA TERMINAL.", ipAddress);
                            }

                            //ACTUALIZAMOS EL ID DE HUELLA DEL USUARIO EN CASO  DE NO TENER
                            eUser user = new eUser();
                            UserBLL userBLL = new UserBLL();
                            ReplicatedUserBLL replicatedUserBLL = new ReplicatedUserBLL();
                            user = userBLL.GetUser(userRecordToSave.userId);

                            if (user != null && !string.IsNullOrEmpty(user.userId))
                            {
                                if (user.fingerprintId <= 0 || (user.fingerprintId > 0 && user.fingerprintId != userRecordToSave.fingerprintId))
                                {
                                    replicatedUserBLL.DeleteUserReplicated(user.fingerprintId, user.userId);
                                    userBLL.UpdateFingerprintId(userRecordToSave.userId, userRecordToSave.fingerprintId);
                                }
                            }
                            else
                            {
                                WhiteListBLL whiteListBLL = new WhiteListBLL();
                                DataTable dt = whiteListBLL.GetFingerprintsById(userRecordToSave.userId);
                                bool withoutFinger = false;
                                string userName = string.Empty;

                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    withoutFinger = Convert.IsDBNull(dt.Rows[0]["withoutFingerprint"]) ? false : Convert.ToBoolean(dt.Rows[0]["withoutFingerprint"]);
                                    userName = dt.Rows[0]["userName"].ToString();
                                    userBLL.Insert(userRecordToSave.userId, userName, userRecordToSave.fingerprintId, withoutFinger, true, false);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteErrorsByTerminals(ex.Message + " " + ex.StackTrace.ToString(), ipAddress);
                actionService = clsEnum.ServiceActions.WaitingClientId;
            }
        }

        private void GetTemplates(string id)
        {
            string strId = string.Empty;
            byte[] tmpHuella, tmpHuellaCAPASITIVO;
            bdTemplate = null;
            dbTemplateSize = null;
            dtHuellas = GetDatosHuella(id);
            int i = 0;

            if (dtHuellas != null && dtHuellas.Rows.Count > 0)
            {
                foreach (DataRow row in dtHuellas.Rows)
                {
                    strId = row["id"].ToString();

                    if (bdTemplate == null)
                    {
                        bdTemplate = new byte[0][];
                        dbTemplateSize = new int[0];
                    }
                    else
                    {
                        bdTemplate = new byte[bdTemplate.Length][];
                        dbTemplateSize = new int[dbTemplateSize.Length];
                    }

                    tmpHuellaCAPASITIVO = (byte[])row["fingerprint"];

                    if (tmpHuellaCAPASITIVO.Length > 770 && tmpHuellaCAPASITIVO[769] != 0)
                    {
                        //tmpHuella = ConvertFingerprint(tmpHuellaCAPASITIVO,);
                    }
                    else
                    {
                        tmpHuella = (byte[])row["fingerprint"];
                    }

                    i++;
                }
            }
        }

        private byte[] ConvertFingerprint(byte[] fingerprint, string ipAddress)
        {
            try
            {
                byte[] testFingerprint = fingerprint, tmpFingerprint = new byte[384];
                string strData = string.Empty;
                int i = 0;

                if (fingerprint.Length > 384)
                {
                    foreach (byte item in testFingerprint)
                    {
                        strData += Convert.ToChar(item);

                        if (strData.Length == 2)
                        {
                            tmpFingerprint[i] = byte.Parse(strData, System.Globalization.NumberStyles.HexNumber);
                            strData = string.Empty;

                            if (i == 383)
                            {
                                break;
                            }

                            i++;
                        }
                    }
                }

                return tmpFingerprint;
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " Terminal: " + ipAddress + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        private DataTable GetDatosHuella(string id)
        {
            FingerprintBLL fingerprintBll = new FingerprintBLL();
            return fingerprintBll.GetFingerprints(id);
        }

        public void FinalizeConnection(string ipAddress)
        {
            try
            {
                if (terminalObjectList != null && terminalObjectList.Count > 0)
                {
                    foreach (clsWinSockCliente item in terminalObjectList)
                    {
                        if (item.IpAddress == ipAddress)
                        {
                            item.CloseConnection();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " Terminal " + ipAddress + " " + ex.StackTrace.ToString());
            }
        }

        //Validar - aún no se ha completado.
        private void winSocketClient_FinalizeConnection()
        {

        }

        private void timerGetPendingActions_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                timerGetPendingActions.Enabled = false;
                timerGetPendingActions.Stop();
                ActionBLL actionBll = new ActionBLL();
                listExecutionTerminal = new List<eTerminalAction>();

                foreach (clsWinSockCliente item in terminalObjectList)
                {
                    //OD 1689 - Getulio Vargas - 2020/01/28
                    //Se realiza validación para el proceso de ejecución de acciones pendientes, para que sólo realice esto para las terminales de tipo 1 (TCAM7000)
                    eActionToExecute terminalAction = new eActionToExecute();
                    terminalAction.ipAddress = item.IpAddress;
                    terminalAction.actionLists = actionBll.GetPendingActionsByTerminal(item.IpAddress);
                    eTerminalAction ta = new eTerminalAction();
                    ta.ipAddress = item.IpAddress;
                    ta.actionToMake = terminalAction.actionLists.Count;
                    ta.actionfinished = 0;
                    listExecutionTerminal.Add(ta);
                    Task.Run(() => ProcessActionTerminal(terminalAction));
                }

                if (terminalObjectList.Count == 0)
                {
                    if (!timerGetPendingActions.Enabled)
                    {
                        timerGetPendingActions.Enabled = true;
                        timerGetPendingActions.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                actionService = clsEnum.ServiceActions.WaitingClientId;
            }
        }

        private async void ProcessActionTerminal(eActionToExecute itemActionTerminal)
        {
            try
            {
                int terminalFinished = 0;
                bool finishedProcess = false;

                foreach (eAction itemAction in itemActionTerminal.actionLists)
                {
                    listExecutionTerminal.Find(let => let.ipAddress == itemAction.ipAddress).actionfinished += 1;
                    await Task.Run(() => ExecuteAction(itemAction));
                }

                foreach (eTerminalAction item in listExecutionTerminal)
                {
                    if (item.actionToMake == item.actionfinished)
                    {
                        terminalFinished++;
                    }
                }

                if (terminalFinished > 0 && (terminalFinished == listExecutionTerminal.Count))
                {
                    finishedProcess = true;
                }

                if (finishedProcess)
                {
                    timerGetPendingActions.Enabled = true;
                    timerGetPendingActions.Start();
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                actionService = clsEnum.ServiceActions.WaitingClientId;
            }
        }

        private void ExecuteAction(eAction item)
        {
            try
            {
                ActionBLL actionBll = new ActionBLL();

                if (item != null)
                {
                    switch (item.strAction)
                    {
                        case "AdditionalEntryDisabled":
                            //actionService = clsEnum.ServiceActions.AdditionalEntryDisabled;
                            //Se envía el commando "Z" para que se abra la puerta de discapacitados
                            if (ExecuteCommand(item.ipAddress, "Z"))
                            {
                                item.stateAction = true;
                                actionBll.Update(item);
                                System.Threading.Thread.Sleep(timeoutOpening);
                            }

                            break;
                        case "Restart":
                            //actionService = clsEnum.ServiceActions.Restart;
                            if (RestartTerminal(item.ipAddress, "7"))
                            {
                                item.stateAction = true;
                                actionBll.Update(item);
                                System.Threading.Thread.Sleep(oneSecond);
                            }

                            break;
                        case "AutorizeEntryClientById":
                        case "AutorizeExitClientById":
                            //actionService = clsEnum.ServiceActions.AutorizeEntryClientById;

                            //Se envía el commando "Z" para que se abra la puerta de discapacitados
                            if (ExecuteCommand(item.ipAddress, "X"))
                            {
                                item.stateAction = true;
                                actionBll.Update(item);
                                System.Threading.Thread.Sleep(timeoutOpening);
                            }

                            if (tmReplicaHuellasTCAM7000 != null)
                            {
                                tmReplicaHuellasTCAM7000.Start();
                            }

                            break;
                        case "Enroll":
                            List<eActionParameters> apList = new List<eActionParameters>();
                            ActionParametersBLL actionParamBll = new ActionParametersBLL();
                            string userId = string.Empty;
                            int fingerId = 0;
                            apList = actionParamBll.GetParameters(item.id);

                            if (apList != null && apList.Count > 0)
                            {
                                foreach (eActionParameters param in apList)
                                {
                                    if (param.parameterName == "UserId")
                                    {
                                        userId = param.parameterValue;
                                    }

                                    if (param.parameterName == "FingerPrintId")
                                    {
                                        fingerId = Convert.ToInt32(param.parameterValue);
                                    }
                                }

                                eUserRecordFingerprint userRecord = new eUserRecordFingerprint();
                                userRecord.fingerprintId = fingerId;
                                userRecord.ipAddress = item.ipAddress;
                                userRecord.quality = 100;
                                userRecord.userId = userId;
                                generalUserIdToSigningContract = userId;
                                listUserRecord.Add(userRecord);

                                GenerateCommandToEnroll(userId, fingerId, item.ipAddress);
                                item.stateAction = true;
                                actionBll.Update(item);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                actionService = clsEnum.ServiceActions.WaitingClientId;
            }
        }

        private void GenerateCommandToEnroll(string userId, int fingerId, string ipAddress)
        {
            try
            {
                string command = string.Empty;

                if (tmReplicaHuellasTCAM7000 != null)
                {
                    tmReplicaHuellasTCAM7000.Stop();

                    if (blnReplicaImgsTCAM7000)
                    {
                        tmReplicaImgsTCAM7000.Stop();
                    }
                }

                if (!signContracteToEnroll)
                {
                    //Inicio de comando para grabación "normal" de huellas.
                    command = "@E";
                }
                else
                {
                    //Inicio de comando para grabación de huella con firma de contrato.
                    command = "@F";
                }

                command += fingerId.ToString().PadLeft(12, '0');
                command += "$";
                log.WriteProcessByTerminal("COMANDO PARA ENROLAR HUELLA: " + command, ipAddress);
                ExecuteCommand(ipAddress, command);
                actionService = clsEnum.ServiceActions.WaitingFingerprintConfirm;
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                actionService = clsEnum.ServiceActions.WaitingClientId;
            }
        }

        /// <summary>
        /// Método que se encarga de reiniciar una terminal.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private bool RestartTerminal(string ipAddress, string command)
        {
            if (terminalObjectList.Exists(tol => tol.IpAddress == ipAddress) && ValidatePing(ipAddress))
            {
                bool withWhitelist = terminalObjectList.Find(tol => tol.IpAddress == ipAddress).withWithelist;
                clsWinSockCliente restartSocket = new clsWinSockCliente(ipAddress, 9999, withWhitelist, false, 0);
                Thread.Sleep(2000);

                if (restartSocket.connected)
                {
                    restartSocket.SendData(command);

                    if (restartSocket != null)
                    {
                        restartSocket.CloseConnection();
                        restartSocket = null;
                    }

                    foreach (clsWinSockCliente item in terminalObjectList)
                    {
                        if (item.IpAddress == ipAddress)
                        {
                            item.connected = false;
                            item.CloseConnection();
                        }
                    }

                    //Eliminamos el registro de la lista de terminales conectadas.
                    //terminalObjectList.Remove(terminalObjectList.Find(tol => tol.IpAddress == ipAddress));
                    //Thread.Sleep(10000);
                    //ConnectSocketSpecificTerminal(ipAddress, 3001);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Método que sirve de puente para invocar el envío de parámetros a las terminales.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private bool ExecuteCommand(string ipAddress, string command)
        {
            return SendData(command.ToUpper(), ipAddress);
        }

        /// <summary>
        /// Método por medio del cual se envían los comandos de acción a las terminales.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private bool SendData(string command, string ipAddress)
        {
            if (terminalObjectList.Exists(tol => tol.IpAddress == ipAddress) && ValidatePing(ipAddress) && terminalObjectList.Find(tol => tol.IpAddress == ipAddress).connected)
            {
                Thread.Sleep(oneSecond);
                log.WriteTerminals("<- DATA: " + command, ipAddress);
                terminalObjectList.Find(tol => tol.IpAddress == ipAddress).SendData(command);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Método por medio del cual se valida que una terminal esté respondiendo correctamente al ping.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private bool ValidatePing(string ipAddress)
        {
            Ping p = new Ping();
            PingReply pr = p.Send(ipAddress, 500);

            if (pr.Status.ToString().Equals("Success"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void TimerTerminalConnections_Elapsed(object sender, ElapsedEventArgs e)
        {
            ValidateTerminalConnections();
        }

        private void ValidateTerminalConnections()
        {
            try
            {
                timerTerminalConnections.Enabled = false;
                timerTerminalConnections.Stop();

                if (termList != null && termList.Count > 0)
                {
                    foreach (eTerminal item in termList)
                    {
                        if (!terminalObjectList.Exists(tol => tol.IpAddress == item.ipAddress))
                        {
                            if (!string.IsNullOrEmpty(item.ipAddress))
                            {
                                Ping p = new Ping();
                                PingReply pr = p.Send(item.ipAddress, 1000);

                                if (pr.Status.ToString().Equals("Success"))
                                {
                                    log.WriteProcess("Inicia el proceso de conexión de la terminal " + item.ipAddress);
                                    winSocketClient = new clsWinSockCliente(item.ipAddress, Convert.ToInt32(item.port), item.withWhiteList, false, item.terminalTypeId);

                                    if (winSocketClient.connected)
                                    {
                                        terminalObjectList.Add(winSocketClient);
                                    }

                                    winSocketClient.ReceivedData += WinSocketClient_ReceivedData;
                                    log.WriteProcess("Finaliza el proceso de conexión de la terminal " + item.ipAddress);
                                }
                            }
                        }
                        else
                        {
                            if (!terminalObjectList.Find(tol => tol.IpAddress == item.ipAddress).connected)
                            {
                                FinalizeConnection(item.ipAddress);
                                terminalObjectList.Remove(terminalObjectList.Find(tol => tol.IpAddress == item.ipAddress));
                                Ping p = new Ping();
                                PingReply pr = p.Send(item.ipAddress, 800);

                                if (pr.Status.ToString().Equals("Success"))
                                {
                                    log.WriteProcess("Inicia el proceso de conexión de la terminal " + item.ipAddress);
                                    winSocketClient = new clsWinSockCliente(item.ipAddress, Convert.ToInt32(item.port), item.withWhiteList, false, item.terminalTypeId);

                                    if (winSocketClient.connected)
                                    {
                                        terminalObjectList.Add(winSocketClient);
                                    }

                                    winSocketClient.ReceivedData += WinSocketClient_ReceivedData;
                                    log.WriteProcess("Finaliza el proceso de conexión de la terminal " + item.ipAddress);
                                }
                            }
                            else
                            {
                                Ping p = new Ping();
                                PingReply pr = p.Send(item.ipAddress, 800);

                                if (!pr.Status.ToString().Equals("Success"))
                                {
                                    if (terminalObjectList.Find(tol => tol.IpAddress == item.ipAddress).connected)
                                    {
                                        terminalObjectList.Find(tol => tol.IpAddress == item.ipAddress).connected = false;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //log.WriteProcess("No es posible conectar terminales debido a que no se encontraron en la BD local o no fue posible consultarlas.");
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message);
                actionService = clsEnum.ServiceActions.WaitingClientId;
            }
            finally
            {
                timerTerminalConnections.Start();
            }
        }

        private bool turnOnLector(string ipAddress)
        {
            log.WriteProcess("ENCENDEMOS EL LECTOR.");
            actionService = clsEnum.ServiceActions.turnOnLector;
            string command = "@R;0$";
            ExecuteCommand(ipAddress, command);
            return true;
        }

        private bool turnOffLector(string ipAddress)
        {
            log.WriteProcess("APAGAMOS EL LECTOR.");
            actionService = clsEnum.ServiceActions.turnOffLector;
            string command = "@R;1$";
            ExecuteCommand(ipAddress, command);
            return true;
        }
    }
}