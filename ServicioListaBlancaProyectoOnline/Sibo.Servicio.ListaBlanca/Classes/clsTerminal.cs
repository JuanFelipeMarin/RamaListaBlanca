﻿using ImagenTCAM7000;
using libComunicacionBioEntry;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json;
using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.BLL.Helpers;
using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.Data;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using static libComunicacionBioEntry.clsComBioEntryPlus2;

namespace Sibo.Servicio.ListaBlanca.Classes
{
    public class clsTerminal
    {
        #region Attributes and Objects
        private const int maxNumberTry = 5;
        public clsWinSockCliente winSocketClient;
        public List<clsWinSockCliente> terminalObjectList = new List<clsWinSockCliente>();
        private static List<clsWinSockCliente> terminalObjectListAux = new List<clsWinSockCliente>();
        private static string planSeleccionado;
        public List<eListBiolite> terminalObjectListBioLite = new List<eListBiolite>();
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
             allowWhiteListTCAM = false, bitIngresoEmpSinPlan = false;
        byte[] fingerprintBytes = new byte[1024];
        string strImageFingerprint = string.Empty, strTextoMensajeCumpleanos = "FELIZ CUMPLEAÑOS", strTextoMensajeCortxIngs = "Acumula ingresos. Reclame a la administración una cortesía",
               strRutaBanner = "C:\\GYMSOFT\\~BannerForma.swf", strClave = "admingym", strRutaLogo = "C:\\Proyectos\\ingresoTouch\\forma.jpg",
               strColorPpal = "#527BE7", strRutaGuiaMarcacion = "C:\\Proyectos\\ingresoTouch\\forma.jpg",
               generalUserIdToSigningContract = string.Empty, fingerprintsUsed = string.Empty, userUsed = string.Empty, fingerprintDeleteUsed = string.Empty,
               usersDeleteUsed = string.Empty, ipAddressDownload = string.Empty, events = string.Empty,branchId = string.Empty;
        public bool swMultiplesPlanesVigTCAM7000;
        int pingTime = 3000, timeTerminalConnections = 5000, intComplices_Plan_CortxIngs = 0, intTiempoReplicaImgsTCAM7000 = 3600000,
            intTiempoEspaciadoTramasReplicaImgsTCAM7000 = 5, intTiempoMaximoEnvioImgsTCAM7000 = 120, tmlimpiarPantalla = 7000, intTIEMPO_ACTUALIZA_INGRESOS = 5000,
            intTiempoRestarTiquetes = 2, intTiempoPulso = 9, intTiempoAntesAccesoXReservaWeb = 0, intTiempoDespuesAccesoXReservaWeb = 0,
            fingerId = 0, gymId = 0, intTiempoReplicaHuellasTCAM7000 = 30000,
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
        List<eTerminal> termListBioLite = new List<eTerminal>();


        private System.Timers.Timer tmPingContinuoTCAM7000, timerGetPendingActions, timerTerminalConnections, tmReplicaHuellasTCAM7000, tmEsperaRespuestaTCAM7000,
                                    timerRemoveFingerprints, tmReplicaImgsTCAM7000, tmWhiteListTCAM, tmWaitResponseReplicateUsers, tmWaitResponseDeleteFingerprints,
                                    timerRemoveUsers, tmWaitResponseDeleteUser, tmDownloadEvents, tmResetDownloadEvents, tmHourSync, tmReplicaUsuariosBioLiteRestriccion;
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


        public class eListBiolite
        {
            public string ip { get; set; }
            public int codigo { get; set; }
            public string codigoReturn { get; set; }
            public bool replicating { get; set; } = false;
            public string zonas { get; set; }
            public int idTerminal { get; set; }
            public string snTerminal { get; set; }
            public clsComBioEntryPlus2 conexionBioEntry { get; set; }
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
        public bool GetTerminal(int gymId, string branchId)
        {
            TerminalBLL termBLL = new TerminalBLL();
            return termBLL.GetTerminals(gymId, branchId);
        }

        public string ASCIIExtendidoDevolver(int Numero)
        {
            object[] arrASCIIExtendido = new object[256];
            arrASCIIExtendido[128] = "Ç";
            arrASCIIExtendido[129] = "ü";
            arrASCIIExtendido[130] = "é";
            arrASCIIExtendido[131] = "â";
            arrASCIIExtendido[132] = "ä";
            arrASCIIExtendido[133] = "à";
            arrASCIIExtendido[134] = "å";
            arrASCIIExtendido[135] = "ç";
            arrASCIIExtendido[136] = "ê";
            arrASCIIExtendido[137] = "ë";
            arrASCIIExtendido[138] = "è";
            arrASCIIExtendido[139] = "ï";
            arrASCIIExtendido[140] = "î";
            arrASCIIExtendido[141] = "ì";
            arrASCIIExtendido[142] = "Ä";
            arrASCIIExtendido[143] = "Å";
            arrASCIIExtendido[144] = "É";
            arrASCIIExtendido[145] = "æ";
            arrASCIIExtendido[146] = "Æ";
            arrASCIIExtendido[147] = "ô";
            arrASCIIExtendido[148] = "ö";
            arrASCIIExtendido[149] = "ò";
            arrASCIIExtendido[150] = "û";
            arrASCIIExtendido[151] = "ù";
            arrASCIIExtendido[152] = "ÿ";
            arrASCIIExtendido[153] = "Ö";
            arrASCIIExtendido[154] = "Ü";
            arrASCIIExtendido[155] = "ø";
            arrASCIIExtendido[156] = "£";
            arrASCIIExtendido[157] = "Ø";
            arrASCIIExtendido[158] = "×";
            arrASCIIExtendido[159] = "ƒ";
            arrASCIIExtendido[160] = "á";
            arrASCIIExtendido[161] = "í";
            arrASCIIExtendido[162] = "ó";
            arrASCIIExtendido[163] = "ú";
            arrASCIIExtendido[164] = "ñ";
            arrASCIIExtendido[165] = "Ñ";
            arrASCIIExtendido[166] = "ª";
            arrASCIIExtendido[167] = "º";
            arrASCIIExtendido[168] = "¿";
            arrASCIIExtendido[169] = "®";
            arrASCIIExtendido[170] = "¬";
            arrASCIIExtendido[171] = "½";
            arrASCIIExtendido[172] = "¼";
            arrASCIIExtendido[173] = "¡";
            arrASCIIExtendido[174] = "«";
            arrASCIIExtendido[175] = "»";
            arrASCIIExtendido[176] = "░";
            arrASCIIExtendido[177] = "▒";
            arrASCIIExtendido[178] = "▓";
            arrASCIIExtendido[179] = "│";
            arrASCIIExtendido[180] = "┤";
            arrASCIIExtendido[181] = "Á";
            arrASCIIExtendido[182] = "Â";
            arrASCIIExtendido[183] = "À";
            arrASCIIExtendido[184] = "©";
            arrASCIIExtendido[185] = "╣";
            arrASCIIExtendido[186] = "║";
            arrASCIIExtendido[187] = "╗";
            arrASCIIExtendido[188] = "╝";
            arrASCIIExtendido[189] = "¢";
            arrASCIIExtendido[190] = "¥";
            arrASCIIExtendido[191] = "┐";
            arrASCIIExtendido[192] = "└";
            arrASCIIExtendido[193] = "┴";
            arrASCIIExtendido[194] = "┬";
            arrASCIIExtendido[195] = "├";
            arrASCIIExtendido[196] = "─";
            arrASCIIExtendido[197] = "┼";
            arrASCIIExtendido[198] = "ã";
            arrASCIIExtendido[199] = "Ã";
            arrASCIIExtendido[200] = "╚";
            arrASCIIExtendido[201] = "╔";
            arrASCIIExtendido[202] = "╩";
            arrASCIIExtendido[203] = "╦";
            arrASCIIExtendido[204] = "╠";
            arrASCIIExtendido[205] = "═";
            arrASCIIExtendido[206] = "╬";
            arrASCIIExtendido[207] = "¤";
            arrASCIIExtendido[208] = "ð";
            arrASCIIExtendido[209] = "Ð";
            arrASCIIExtendido[210] = "Ê";
            arrASCIIExtendido[211] = "Ë";
            arrASCIIExtendido[212] = "È";
            arrASCIIExtendido[213] = "ı";
            arrASCIIExtendido[214] = "Í";
            arrASCIIExtendido[215] = "Î";
            arrASCIIExtendido[216] = "Ï";
            arrASCIIExtendido[217] = "┘";
            arrASCIIExtendido[218] = "┌";
            arrASCIIExtendido[219] = "█";
            arrASCIIExtendido[220] = "▄";
            arrASCIIExtendido[221] = "¦";
            arrASCIIExtendido[222] = "Ì";
            arrASCIIExtendido[223] = "▀";
            arrASCIIExtendido[224] = "Ó";
            arrASCIIExtendido[225] = "ß";
            arrASCIIExtendido[226] = "Ô";
            arrASCIIExtendido[227] = "Ò";
            arrASCIIExtendido[228] = "õ";
            arrASCIIExtendido[229] = "Õ";
            arrASCIIExtendido[230] = "µ";
            arrASCIIExtendido[231] = "þ";
            arrASCIIExtendido[232] = "Þ";
            arrASCIIExtendido[233] = "Ú";
            arrASCIIExtendido[234] = "Û";
            arrASCIIExtendido[235] = "Ù";
            arrASCIIExtendido[236] = "ý";
            arrASCIIExtendido[237] = "Ý";
            arrASCIIExtendido[238] = "¯";
            arrASCIIExtendido[239] = "´";
            arrASCIIExtendido[240] = "­";
            arrASCIIExtendido[241] = "±";
            arrASCIIExtendido[242] = "‗";
            arrASCIIExtendido[243] = "¾";
            arrASCIIExtendido[244] = "¶";
            arrASCIIExtendido[245] = "§";
            arrASCIIExtendido[246] = "÷";
            arrASCIIExtendido[247] = "¸";
            arrASCIIExtendido[248] = "°";
            arrASCIIExtendido[249] = "¨";
            arrASCIIExtendido[250] = "·";
            arrASCIIExtendido[251] = "¹";
            arrASCIIExtendido[252] = "³";
            arrASCIIExtendido[253] = "²";
            arrASCIIExtendido[254] = "■";
            arrASCIIExtendido[255] = " ";

            return arrASCIIExtendido[Numero].ToString();
        }


        public double EnDecimal(string CadenaBinaria)
        {
            int intPosicion;
            string strBinario;
            double intDecimal = 0;

            for (intPosicion = 0; intPosicion <= CadenaBinaria.Length - 1; intPosicion++)
            {
                strBinario = CadenaBinaria.Substring(intPosicion, 1);
                intDecimal += Conversion.Val(strBinario) * (Math.Pow(2, (CadenaBinaria.Length - intPosicion - 1)));
            }

            return intDecimal;
        }

        /// <summary>
        /// Método encargado de consultar las terminales registradas en la BD local para pasar a conectarlas.
        /// Getulio Vargas - OD 1307
        /// </summary>
        public void OpenSockets()
        {
            leerJson lg = new leerJson();
            try
            {
                string strGymId = ConfigurationSettings.AppSettings["gymId"].ToString();
                string strBranchId = ConfigurationSettings.AppSettings["branchId"].ToString();

                if (!string.IsNullOrEmpty(strGymId) && !string.IsNullOrEmpty(strBranchId))
                {
                    gymId = Convert.ToInt32(strGymId);
                    branchId = strBranchId;
                }
                else
                {
                    log.WriteProcess("No se ha configurado de forma correcta el código del gimnasio y/o la sucursal.");
                }

                timeoutOpening = 12000;

                TerminalBLL termBLL = new TerminalBLL();
                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                List<eConfiguration> config = new List<eConfiguration>();
                log.WriteProcess("Inicia el proceso de consulta de configuración en BD local, para fijar valor de parámetros en ejecución.");
                //config = acsBll.GetLocalAccessControlSettings();

                string json = lg.cargarArchivos(1);
                //config = acsData.GetConfiguration();
                if (json != "")
                {
                    config = JsonConvert.DeserializeObject<List<eConfiguration>>(json);
                }

                if (config != null )
                {
                    FillConfigurationParameters(config);
                }
                else
                {
                    log.WriteProcess("No se encontró configuración guardada u ocurrió alguna novedad en el proceso de consulta.");
                }

                log.WriteProcess("Finaliza proceso de consulta de configuración en BD local, para fijar valor de parámetros en ejecución.");
                log.WriteProcess("Inicia el proceso de consulta de terminales en BD local.");
                ////llamar base de datos local
                //termList = termBLL.GetTerminals();
                //termListBioLite = termBLL.GetTerminalsBioLite();
                termList = listaTerminales(7,1);
                termListBioLite = listaTerminales(7, 2); 

                log.WriteProcess("Finaliza proceso de consulta de terminales en BD local.");
                log.WriteProcess("Inicia el proceso de de conexión de las terminales.");
                ConnectTCAM();
                ConnectBioLite();

                /////Pendiente
                //foreach (eListBiolite item in terminalObjectListBioLite)
                //{
                //    if (hacerping(item.ip))
                //    {
                //        DataTable dtvFila = new DataTable();
                //        dtvFila.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                //        DataRow drvFila;
                //        drvFila = dtvFila.NewRow();
                //        drvFila["strIpTerminal"] = item.ip;
                //        dtvFila.Rows.Add(drvFila);

                //        WhiteListBLL whiteListBLL = new WhiteListBLL();
                //        DataTable dtHorrariosRestriccion = new DataTable();



                //        dtHorrariosRestriccion = whiteListBLL.ConsultarHorariosRestriccion();

                //        for (int i = 0; i < dtHorrariosRestriccion.Rows.Count; i++)
                //        {
                //            string[] horaEntradaRestriccion = dtHorrariosRestriccion.Rows[i]["strHoraEntrada"].ToString().Split(':');
                //            string[] horaSalidaRestriccion = dtHorrariosRestriccion.Rows[i]["strHoraSalida"].ToString().Split(':');

                //            string SimboloAssciDiasSemana = "\u007f";

                //            item.conexionBioEntry.configurarHorario(drvFila, Convert.ToInt32(dtHorrariosRestriccion.Rows[i]["id"]), SimboloAssciDiasSemana, Convert.ToInt32(horaEntradaRestriccion[0]), Convert.ToInt32(horaEntradaRestriccion[1]), Convert.ToInt32(horaSalidaRestriccion[0]), Convert.ToInt32(horaSalidaRestriccion[0]), Convert.ToUInt32(item.snTerminal), item.idTerminal);
                //            int delayTerminate = 0;
                //            Thread.Sleep(delayTerminate);
                //        }
                //    }
                //}

                log.WriteProcess("Finaliza el proceso de de conexión de las terminales.");
                //tmPingContinuoTCAM7000 = new System.Timers.Timer(pingTime);
                //tmPingContinuoTCAM7000.Elapsed += TmPingContinuoTCAM7000_Elapsed;
                //tmPingContinuoTCAM7000.Enabled = true;
                //tmPingContinuoTCAM7000.Start();

                //descomentar esta parte 

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




                //tmEsperaRespuestaTCAM7000 = new System.Timers.Timer(intTiempoEsperaRespuestaReplicaHuellasTCAM7000);
                //tmEsperaRespuestaTCAM7000.Elapsed += TmEsperaRespuestaTCAM7000_Elapsed;

                //tmWaitResponseReplicateUsers = new System.Timers.Timer(timeWaitResponseReplicateUsers);
                //tmWaitResponseReplicateUsers.Elapsed += TmWaitResponseReplicateUsers_Elapsed;

                //tmWaitResponseDeleteFingerprints = new System.Timers.Timer(timeWaitResponseDeleteFingerprint);
                //tmWaitResponseDeleteFingerprints.Elapsed += TmWaitResponseDeleteFingerprints_Elapsed;

                //tmWaitResponseDeleteUser = new System.Timers.Timer(timeWaitResponseDeleteUser);
                //tmWaitResponseDeleteUser.Elapsed += TmWaitResponseDeleteUser_Elapsed;

                //tmResetDownloadEvents = new System.Timers.Timer(timeResetDownloadEvents);
                //tmResetDownloadEvents.Elapsed += TmResetDownloadEvents_Elapsed;

                //Iniciamos el timer de borrado de huellas de las terminales
                timerRemoveFingerprints = new System.Timers.Timer(timeRemoveFingerprints);
                timerRemoveFingerprints.Elapsed += TimerRemoveFingerprints_Elapsed;
                timerRemoveFingerprints.Enabled = true;
                timerRemoveFingerprints.Start();

                //INICIAMOS EL TIMER DE BORRADO DE USUARIOS DE LAS TERMINALES
                //timerRemoveUsers = new System.Timers.Timer(timeRemoveUsers);
                //timerRemoveUsers.Elapsed += TimerRemoveUsers_Elapsed;
                //timerRemoveUsers.Enabled = true;
                //timerRemoveUsers.Start();

                //tmWhiteListTCAM = new System.Timers.Timer(timeWhitelistTCAM);
                //tmWhiteListTCAM.Elapsed += TmWhiteListTCAM_Elapsed;
                //tmWhiteListTCAM.Enabled = true;
                //tmWhiteListTCAM.Start();

                //descomentar esta parte 

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

                string tiempoReplicaBioLite = ConfigurationManager.AppSettings["tiempoReplicaBioLite"];
                int tiempoReplica = 30;

                if (!string.IsNullOrEmpty(tiempoReplicaBioLite))
                {
                    tiempoReplica = Convert.ToInt32(tiempoReplicaBioLite);
                }

                //descomentar esta parte 
                //tmReplicaUsuariosBioLiteRestriccion = new System.Timers.Timer((tiempoReplica * 60000));
                //tmReplicaUsuariosBioLiteRestriccion.Elapsed += TimerReplicaUsuariosBioLiteRestriccion;
                //tmReplicaUsuariosBioLiteRestriccion.Enabled = true;
                //tmReplicaUsuariosBioLiteRestriccion.Start();

               // InitialHourSync();
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores de las terminales.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                actionService = clsEnum.ServiceActions.WaitingClientId;
            }
        }

        private List<eTerminal> listaTerminales(int tipoTerminal, int operador)
        {
            leerJson lg = new leerJson();
            List<eTerminal> config = new List<eTerminal>();
            List<eTerminal> personasCiudadA = new List<eTerminal>();

            string json = lg.cargarArchivos(2);
            if (json != "")
            {
                config = JsonConvert.DeserializeObject<List<eTerminal>>(json);
            }

            if (operador == 1)
            {
                personasCiudadA = config.Where(p => p.terminalTypeId != tipoTerminal).ToList();
            }
            else
            {
                personasCiudadA = config.Where(p => p.terminalTypeId == tipoTerminal).ToList();
            }

           return personasCiudadA;
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
                tmDownloadEvents.Enabled = true;
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

        public class lstDescargaTerminaleventos
        {
            public List<eTerminarBioLite> terminal { get; set; }
            public clsComBioEntryPlus2 conexionBioEntry { get; set; }
        }

        private void TmDownloadEvents_Elapsed(object sender, ElapsedEventArgs e)
        {
            WhiteListAPI wlAPI = new WhiteListAPI();
            ReplicaHuellasAPI repllicaAPI = new ReplicaHuellasAPI();
            bool respuesta = false;
            bool respuestaConsultaVisitante = false;
            //if (allowWhiteListTCAM)
            //{
            //    if (terminalObjectList.Count > 0)
            //    {
            //        int i = 0;
            //        foreach (clsWinSockCliente item in terminalObjectList)
            //        {
            //            if (ValidateTerminalStateDownloadEvents(item, true) && item.terminalTypeId == 1)
            //            {
            //                startIndexReplicateUsers = i;
            //                indexReplicateUsers = i;
            //                terminalCounterDownloadEvents++;
            //                item.replicating = true;
            //                ControlDownloadEvents(item, false, true);
            //                break;
            //            }

            //            i++;
            //        }
            //    }
            //}

            if (terminalObjectListBioLite.Count > 0)
            {
                try
                {
                    tmDownloadEvents.Enabled = false;
                    tmDownloadEvents.Stop();
                }
                catch (Exception)
                {

                }
                List<lstDescargaTerminaleventos> todosRegistrosTerminales = new List<lstDescargaTerminaleventos>();
                foreach (eListBiolite item in terminalObjectListBioLite)
                {
                    if (hacerping(item.ip))
                    {
                        lstDescargaTerminaleventos objDescargaTerminaleventos = new lstDescargaTerminaleventos();

                        objDescargaTerminaleventos.terminal = item.conexionBioEntry.descargarMarcaciones(item.ip, item.codigo);
                        int delayTerminate = 0;
                        Thread.Sleep(delayTerminate);
                        objDescargaTerminaleventos.conexionBioEntry = item.conexionBioEntry;

                        todosRegistrosTerminales.Add(objDescargaTerminaleventos);
                    }
                }

                foreach (lstDescargaTerminaleventos itemPrincipal in todosRegistrosTerminales)
                {
                    if (itemPrincipal != null)
                    {
                        foreach (eTerminarBioLite item in itemPrincipal.terminal)
                        {
                            ValidateEntryByUserIdBioLite(item.codigo, item.ip, item.esAceptada);
                        }
                    }
                }

                foreach (lstDescargaTerminaleventos itemPrincipal in todosRegistrosTerminales)
                {
                    if (itemPrincipal != null)
                    {
                        foreach (eTerminarBioLite RemoverListaContratos in itemPrincipal.terminal)
                                {
                            WhiteListBLL whiteListBLL = new WhiteListBLL();
                            // en caso de que la persona no tenga firmado contrato pero aya realizado una marcacion se elimina el registro de huella o tarjeta de la terminal
                            //string clienteId = whiteListBLL.RemoverListaContratos(RemoverListaContratos.codigo);

                            /////Se cambia consulta con metodo a la api para obtener si la persona que esta consultado tiene los ocntratos configurador firmados 
                            string clienteId = wlAPI.GetConsultarEliminacionContratos(gymId,branchId,RemoverListaContratos.codigo);

                            if (clienteId != null && clienteId != "")
                            {
                                if (hacerping(RemoverListaContratos.ip))
                                {
                                    FingerprintBLL bllEliminar = new FingerprintBLL();

                                    DataTable dtvFila = new DataTable();
                                    dtvFila.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                                    DataRow drvFila;
                                    drvFila = dtvFila.NewRow();
                                    drvFila["strIpTerminal"] = RemoverListaContratos.ip;
                                    dtvFila.Rows.Add(drvFila);

                                    itemPrincipal.conexionBioEntry.EliminarHuellaUsuario(drvFila, 1471, RemoverListaContratos.codigo, false);
                                    int delayTerminate = 0;
                                    Thread.Sleep(delayTerminate);

                                    //bllEliminar.GetCabiarEstadoElimarHuellas(clienteId, RemoverListaContratos.ip);
                                    //bllEliminar.GetCambiarEstadoElimarTarjetas(clienteId, RemoverListaContratos.ip);
                                    ///se crea nuevo metodo en la api para actualizar estado de las huellas que va actualizar 
                                    ///Evaluar con daniel si se llena una lista con los datos que se van a mandar a actualizar y hacerlo de manera masiva 
                                    respuesta = repllicaAPI.GetActualizarEstadoReplica(gymId, clienteId, RemoverListaContratos.ip);
                                 
                                }
                            }

                            //Eliminar visitantes por terminal perimetral
                            //DataTable dtCantidadEventosVisitante = new DataTable();

                            if (RemoverListaContratos.codigo != "\u0003")
                            {
                                //dWhiteList wlData = new dWhiteList();
                                //dtCantidadEventosVisitante = wlData.GetIngresosVisitantesTblEvents(RemoverListaContratos.codigo);
                                //dtCantidadEventosVisitante = whiteListBLL.obtenerIdVisitanteEliminar(RemoverListaContratos.codigo);
                                //if (dtCantidadEventosVisitante != null)
                                //{
                                //    if (dtCantidadEventosVisitante.Rows.Count > 0)
                                //    {
                                //        clienteId = dtCantidadEventosVisitante.Rows[0]["clientId"].ToString();

                                //        DataTable dt = new DataTable();
                                //        List<eWhiteList> respList = new List<eWhiteList>();
                                //        dt = wlData.GetInformacionZonas(clienteId);

                                //        int Ingresos = 0;

                                //        if (dt != null && dt.Rows.Count > 0)
                                //        {
                                //            Ingresos = Convert.ToInt32(dt.Rows[0]["availableEntries"].ToString());
                                //        }
                                //        else
                                //        {
                                //            Ingresos = 1;
                                //        }

                                //        if (dtCantidadEventosVisitante.Rows.Count >= Ingresos && (dt.Rows[0]["typePerson"].ToString() == "Prospecto" || dt.Rows[0]["typePerson"].ToString() == "Visitante"))
                                //        {
                                //            if (hacerping(RemoverListaContratos.ip))
                                //            {
                                //                FingerprintBLL bllEliminar = new FingerprintBLL();
                                //                DataTable dtvFila = new DataTable();
                                //                dtvFila.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                                //                DataRow drvFila;
                                //                drvFila = dtvFila.NewRow();
                                //                drvFila["strIpTerminal"] = RemoverListaContratos.ip;
                                //                dtvFila.Rows.Add(drvFila);

                                //                itemPrincipal.conexionBioEntry.EliminarHuellaUsuario(drvFila, 1471, RemoverListaContratos.codigo, false);
                                //                int delayTerminate = 0;
                                //                Thread.Sleep(delayTerminate);
                                //            }
                                //        }
                                //    }
                                //}

                                ////Se cambia logica de consulta centraliza en la api  para obtener respeus de si se elimina o no de la biolite
                                respuestaConsultaVisitante = wlAPI.GetRespuestaIngresosVisitantes(gymId, RemoverListaContratos.codigo);
                                if (respuestaConsultaVisitante == true)
                                {
                                    if (hacerping(RemoverListaContratos.ip))
                                    {
                                        FingerprintBLL bllEliminar = new FingerprintBLL();
                                        DataTable dtvFila = new DataTable();
                                        dtvFila.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                                        DataRow drvFila;
                                        drvFila = dtvFila.NewRow();
                                        drvFila["strIpTerminal"] = RemoverListaContratos.ip;
                                        dtvFila.Rows.Add(drvFila);

                                        itemPrincipal.conexionBioEntry.EliminarHuellaUsuario(drvFila, 1471, RemoverListaContratos.codigo, false);
                                        int delayTerminate = 0;
                                        Thread.Sleep(delayTerminate);
                                    }
                                }
                            }

                            ////// se elimina huella o tarjeta para ser replicado nuevamente.  Pendiente Revisar QA
                            ////whiteListBLL.RemoverListaPersonas(RemoverListaContratos.codigo);
                        }
                    }
                }

                tmDownloadEvents.Enabled = true;
                tmDownloadEvents.Start();
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
                    tmDownloadEvents.Enabled = false;
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
            tmDownloadEvents.Enabled = true;
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
            ClientMessagesAPI clibll = new ClientMessagesAPI();
            List<eClientMessages> responseList = new List<eClientMessages>();
            tmReplicaImgsTCAM7000.Stop();

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

                //dtImgs = cmBll.GetMessagesWithoutReplicate(item.IpAddress);

                responseList = clibll.GetClientMessagesDownload(gymId,branchId, item.IpAddress);

                if (responseList != null && responseList.Count > 0)
                {
                    //cantidadImgs = dtImgs.Rows.Count;
                    cantidadImgs = responseList.Count;
                    log.WriteProcess("LA CANTIDAD DE IMÁGENES SIN REPLICAR A LA TERMINAL " + item.IpAddress + " ES: " + cantidadImgs);

                    foreach (eClientMessages row in responseList)
                    {
                        byte[] tmpImagen = (byte[])row.messageImage;
                        TipoImagen = row.messageType.ToString();
                        IDImg = Convert.ToInt32(row.messageId.ToString());
                        //RutaImagen = System.Windows.Forms.Application.StartupPath + "\\ImagenReplica.jpg";
                        RutaImagen = AppDomain.CurrentDomain.BaseDirectory + "\\ImagenReplica.jpg";
                        //RutaImagen = "";
                        FileInfo archivoIngreso = new FileInfo(RutaImagen);

                        if (archivoIngreso.Exists)
                        {
                            archivoIngreso.Delete();
                        }

                        using (MemoryStream ms = new MemoryStream(tmpImagen))
                        {
                            //MemoryStream ms = new MemoryStream(tmpImagen);
                            FileStream fs = new FileStream(RutaImagen, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                            ms.WriteTo(fs);
                            fs.Flush();
                            fs.Close();
                            ms.Close();

                        }
                           

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

                            //if (cmBll.InsertReplicatedImage(IDImg, item.IpAddress))
                            if(clibll.GetClientMessagesInserReplicate(gymId, branchId, IDImg, item.IpAddress))
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

            if (terminalObjectListBioLite.Count > 0)
            {
                foreach (eListBiolite item in terminalObjectListBioLite)
                {
                    EliminarHellasTerminaVencidas(item);
                    EliminarTarjetasTerminalVencidas(item);


                }
            }

        }

        private void TimerReplicaUsuariosBioLiteRestriccion(object sender, ElapsedEventArgs e)
        {
            //System.Threading.Thread.Sleep(60 * oneSecond);
            tmReplicaUsuariosBioLiteRestriccion.Enabled = false;
            tmReplicaUsuariosBioLiteRestriccion.Stop();

            WhiteListBLL whiteListBLL = new WhiteListBLL();
            DataTable dttReplicaUsuariosBioLiteRestriccion = new DataTable();

            // consultar si hay terminales BioLite Conectadas
            if (terminalObjectListBioLite.Count > 0)
            {
                foreach (var itemListaTerminal in terminalObjectListBioLite)
                {
                    // Lista de Cientes con restriccion horario de sub grupo o plan o con reserva de clases 
                    dttReplicaUsuariosBioLiteRestriccion = whiteListBLL.bllReplicaUsuariosBioLiteRestriccion();


                    if (dttReplicaUsuariosBioLiteRestriccion.Rows.Count > 0)
                    {
                        dttReplicaUsuariosBioLiteRestriccion.Columns.Add("seElimino", typeof(Boolean));
                        // revisa si la terminal responde a ping dentro de la Red.
                        if (hacerping(itemListaTerminal.ip))
                        {
                            for (int i = 0; i < dttReplicaUsuariosBioLiteRestriccion.Rows.Count; i++)
                            {
                                if (Convert.ToBoolean(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["esClase"]))
                                {
                                    TimeSpan horaAcutal = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
                                    //EN EL METODO DE ARRIBA PONER QUE VALIDE CON 15 MINUTOS ANTES (ES LA CONDICION DEL IF DE ARRIBA), PARA LOS QUE LLEGAN TEMPRANO
                                    TimeSpan horaInicialRestrinccion = TimeSpan.Parse(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["HoraInicial"].ToString());
                                    TimeSpan horaFinalRestrinccion = TimeSpan.Parse(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["HoraFinal"].ToString());
                                    //if ((horaInicialRestrinccion >= horaInicial && horaInicialRestrinccion <= horaFinal) || (horaFinalRestrinccion >= horaInicial && horaFinalRestrinccion <= horaFinal))

                                    horaInicialRestrinccion = horaInicialRestrinccion.Add(TimeSpan.FromMinutes(-60));
                                    horaFinalRestrinccion = horaFinalRestrinccion.Add(TimeSpan.FromMinutes(60));

                                    if (horaInicialRestrinccion <= horaAcutal && horaFinalRestrinccion >= horaAcutal)
                                    {
                                        TimeSpan horaInicialHorarioFinal = TimeSpan.Parse(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["HoraInicial"].ToString());
                                        TimeSpan horaFinalHorarioFinal = TimeSpan.Parse(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["HoraFinal"].ToString());
                                        TimeSpan horaFinalSalida = TimeSpan.Parse(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["HoraFinal"].ToString());


                                        if (intTiempoDespuesAccesoXReservaWeb != 0)
                                        {
                                            horaFinalHorarioFinal = horaInicialHorarioFinal.Add(TimeSpan.FromMinutes(intTiempoDespuesAccesoXReservaWeb));
                                        }


                                        horaFinalSalida = horaFinalSalida.Add(TimeSpan.FromMinutes(-30));

                                        TimeSpan horaSalida = TimeSpan.Parse(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["HoraFinal"].ToString());

                                        // inserta los ahorarios en la tabla de restriccion
                                        whiteListBLL.agregarHorariosRestriccion(horaInicialHorarioFinal.Hours + ":" + horaInicialHorarioFinal.Minutes + ":00:0", horaFinalHorarioFinal.Hours + ":" + horaFinalHorarioFinal.Minutes + ":00:0");
                                        whiteListBLL.agregarHorariosRestriccion(horaFinalSalida.Hours + ":" + horaFinalSalida.Minutes + ":00:0", horaSalida.Hours + ":" + horaSalida.Minutes + ":00:0");

                                        //consulta el codigo de horario segun las horas que esten la clase 
                                        string idHorarioEntrada = whiteListBLL.ConsultarHorariosRestriccionPorDiaHora(horaInicialHorarioFinal.Hours + ":" + horaInicialHorarioFinal.Minutes + ":00:0", horaFinalHorarioFinal.Hours + ":" + horaFinalHorarioFinal.Minutes + ":00:0");
                                        string idHorarioSalida = whiteListBLL.ConsultarHorariosRestriccionPorDiaHora(horaFinalSalida.Hours + ":" + horaFinalSalida.Minutes + ":00:0", horaSalida.Hours + ":" + horaSalida.Minutes + ":00:0");
                                        DataTable dttHorarios = new DataTable();
                                        dttHorarios = whiteListBLL.ConsultarHorariosRestriccion();

                                        //consulta restriccion de zonas segun terminal
                                        string zonasTerminal = terminalObjectListBioLite.Find(tol => tol.ip == itemListaTerminal.ip).zonas;
                                        int idTerminal = terminalObjectListBioLite.Find(tol => tol.ip == itemListaTerminal.ip).idTerminal;
                                        string snTerminal = terminalObjectListBioLite.Find(tol => tol.ip == itemListaTerminal.ip).snTerminal;

                                        bool seEliminoNoZona = false;
                                        dttReplicaUsuariosBioLiteRestriccion.Rows[i]["seElimino"] = false;

                                        DataView dv = new DataView(dttHorarios);
                                        dv.RowFilter = "id = " + idHorarioEntrada + " or id = " + idHorarioSalida;

                                        DataTable dtvFila = new DataTable();
                                        dtvFila.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                                        DataRow drvFila;
                                        drvFila = dtvFila.NewRow();
                                        drvFila["strIpTerminal"] = itemListaTerminal.ip;
                                        dtvFila.Rows.Add(drvFila);

                                        int delayTerminate = 0;

                                        for (int iRow = 0; iRow < dv.Count; iRow++)
                                        {
                                            string[] horaEntradaRestriccion = dv[iRow]["strHoraEntrada"].ToString().Split(':');
                                            string[] horaSalidaRestriccion = dv[iRow]["strHoraSalida"].ToString().Split(':');

                                            string SimboloAssciDiasSemana = "\u007f";

                                            itemListaTerminal.conexionBioEntry.configurarHorario(drvFila, Convert.ToInt32(dv[iRow]["id"]), SimboloAssciDiasSemana, Convert.ToInt32(horaEntradaRestriccion[0]), Convert.ToInt32(horaEntradaRestriccion[1]), Convert.ToInt32(horaSalidaRestriccion[0]), Convert.ToInt32(horaSalidaRestriccion[1]), Convert.ToUInt32(snTerminal), idTerminal);

                                            Thread.Sleep(delayTerminate);
                                        }

                                        //if (zonasTerminal != null && zonasTerminal != "" && zonasTerminal != "0")
                                        //{
                                        WhiteListAPI whiteListAPI = new WhiteListAPI();

                                        //for (int o = 0; o < dttReplicaUsuariosBioLiteRestriccion.Rows.Count; o++)
                                        //{
                                        bool auxExist = false;
                                        string zonasPlan = "";
                                        string zonaPlanAux = "";
                                        if (dttReplicaUsuariosBioLiteRestriccion.Rows[i]["typePerson"].ToString() == "Prospecto" && Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["planId"]) == 0)
                                        {
                                            auxExist = true;
                                        }
                                        else
                                        {
                                            if (bitIngresoEmpSinPlan == true && dttReplicaUsuariosBioLiteRestriccion.Rows[i]["typePerson"].ToString() == "Empleado")
                                            {
                                                auxExist = true;
                                            }
                                            else
                                            {
                                                if (blnAccesoXReservaWeb == true && blnAccesoDUAL_Tradicional_y_XReservaWeb == false)
                                                {
                                                    zonasPlan = whiteListAPI.GetListSucursalPorClase(Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["gymId"]), Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["branchId"]), Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["reserveId"]));
                                                }
                                                else if (blnAccesoDUAL_Tradicional_y_XReservaWeb == true && blnAccesoXReservaWeb == false)
                                                {
                                                    zonasPlan = whiteListAPI.GetListSucursalPorClase(Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["gymId"]), Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["branchId"]), Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["reserveId"]));
                                                    zonaPlanAux = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["gymId"]), Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["branchId"]), Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["planId"]));

                                                    if (zonasPlan == "0" || zonasPlan == "" || zonasPlan == " " || zonasPlan == null)
                                                    {
                                                        zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["gymId"]), Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["branchId"]), Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["planId"]));
                                                    }
                                                }
                                                else if (zonasTerminal != null && zonasTerminal != "" && zonasTerminal != "0")
                                                {
                                                    zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["gymId"]), Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["branchId"]), Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["planId"]));
                                                }
                                            }
                                        }

                                        DataTable dtZonas = new DataTable();
                                        DataTable dtZonasAux = new DataTable();
                                        dtZonas = whiteListBLL.ConsultarZonas(zonasPlan);
                                        dtZonasAux = whiteListBLL.ConsultarZonas(zonaPlanAux);
                                        Boolean estadoauxiliar = false;

                                        for (int ii = 0; ii < dtZonas.Rows.Count; ii++)
                                        {
                                            string[] zonasPermitidasSplit = dtZonas.Rows[ii]["Terminales"].ToString().Split(',');

                                            foreach (string itemZonaPermitida in zonasPermitidasSplit)
                                            {
                                                if (Convert.ToInt32(itemZonaPermitida) == idTerminal)
                                                {
                                                    auxExist = true;
                                                    dttReplicaUsuariosBioLiteRestriccion.Rows[i]["seElimino"] = true;
                                                    estadoauxiliar = true;
                                                }
                                            }
                                        }

                                        if (blnAccesoDUAL_Tradicional_y_XReservaWeb == true && blnAccesoXReservaWeb == false && estadoauxiliar == false)
                                        {
                                            for (int ii = 0; ii < dtZonasAux.Rows.Count; ii++)
                                            {
                                                string[] zonasPermitidasSplit = dtZonasAux.Rows[ii]["Terminales"].ToString().Split(',');

                                                foreach (string itemZonaPermitida in zonasPermitidasSplit)
                                                {
                                                    if (Convert.ToInt32(itemZonaPermitida) == idTerminal)
                                                    {
                                                        auxExist = true;
                                                        dttReplicaUsuariosBioLiteRestriccion.Rows[i]["seElimino"] = true;

                                                    }
                                                }
                                            }

                                        }


                                        if (auxExist == false)
                                        {
                                            //DataRow dr = dttReplicaUsuariosBioLiteRestriccion.Rows[i];
                                            //dr.Delete();

                                            seEliminoNoZona = true;
                                            dttReplicaUsuariosBioLiteRestriccion.Rows[i]["seElimino"] = false;
                                        }
                                        //}

                                        dttReplicaUsuariosBioLiteRestriccion.AcceptChanges();
                                        //}

                                        if (seEliminoNoZona == false && Convert.ToBoolean(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["seElimino"]) == true)
                                        {
                                            string[] arrIdUsuario;
                                            object[] arrIdUsuarioObj;

                                            clsComBioEntryPlus2.tipoAccesoLector[] tipos;
                                            bool esCard = false;

                                            if (Convert.ToBoolean(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["withoutFingerprint"]) == true && dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString().Length > 2)
                                            {
                                                tipos = new clsComBioEntryPlus2.tipoAccesoLector[] { clsComBioEntryPlus2.tipoAccesoLector.TarjetaSolo };
                                                arrIdUsuario = new string[] { dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString() };
                                                arrIdUsuarioObj = new object[] { Array.Empty<byte>() };
                                                esCard = true;
                                            }
                                            else if (Convert.ToBoolean(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["withoutFingerprint"]) == true && dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString().Length < 2)
                                            {
                                                tipos = new clsComBioEntryPlus2.tipoAccesoLector[] { clsComBioEntryPlus2.tipoAccesoLector.ClaveSolo };
                                                arrIdUsuario = new string[] { dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString() };
                                                arrIdUsuarioObj = new object[] { Array.Empty<byte>() };
                                                esCard = true;

                                            }
                                            else
                                            {
                                                if (dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprint"] == null || dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprint"].ToString() == "")
                                                {
                                                    tipos = new clsComBioEntryPlus2.tipoAccesoLector[] { clsComBioEntryPlus2.tipoAccesoLector.soloHuella };
                                                    arrIdUsuario = new string[] { "0" };
                                                    arrIdUsuarioObj = new object[] { Array.Empty<byte>() };
                                                    esCard = false;
                                                }
                                                else
                                                {

                                                    tipos = new clsComBioEntryPlus2.tipoAccesoLector[] { clsComBioEntryPlus2.tipoAccesoLector.soloHuella };
                                                    arrIdUsuario = new string[] { dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprintId"].ToString() };
                                                    arrIdUsuarioObj = new object[] { (byte[])dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprint"] };
                                                    esCard = false;
                                                }

                                            }

                                            string[] arrayHorarioGenerico = new string[2];

                                            arrayHorarioGenerico[0] = idHorarioEntrada;
                                            arrayHorarioGenerico[1] = idHorarioSalida;

                                            bool[] arrayBoolGenerico = new bool[] { true };
                                            object[] byteNulo = new object[] { Array.Empty<byte>() };

                                            eTerminal dsTerminalesBio = new eTerminal();
                                            TerminalBLL terminal = new TerminalBLL();

                                            dsTerminalesBio = terminal.GetTerminalByIp(itemListaTerminal.ip);

                                            byte[] huella;

                                            if (Convert.IsDBNull(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprint"]))
                                            {
                                                huella = new byte[0];
                                            }
                                            else
                                            {
                                                huella = (byte[])dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprint"];
                                            }

                                            //if (itemListaTerminal.conexionBioEntry.EnrrolarMultiplesHuellasBioEntry(dtvFila.Rows[0], tipos, arrIdUsuario, arrIdUsuarioObj, byteNulo, byteNulo, byteNulo, arrIdUsuario, arrayHorarioGenerico, arrayHorarioGenerico, Convert.ToUInt32(dsTerminalesBio.snTerminal), arrayBoolGenerico, dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString()) == respuestaDescarga.huellaEnrroladaSatisfactoriamente)
                                            if (itemListaTerminal.conexionBioEntry.EnrrolarMultiplesHuellasBioEntryNuevo(dtvFila.Rows[0], dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString(), (dttReplicaUsuariosBioLiteRestriccion.Rows[i].IsNull("fingerprint") == true ? null : (byte[])dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprint"]), dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprintId"].ToString(), arrayHorarioGenerico, arrayHorarioGenerico, Convert.ToUInt32(dsTerminalesBio.snTerminal), true, dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString(), Convert.ToBoolean(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["withoutFingerprint"])) == respuestaDescarga.huellaEnrroladaSatisfactoriamente)
                                            {
                                                Thread.Sleep(delayTerminate);
                                                ReplicatedFingerprintBLL rfBll = new ReplicatedFingerprintBLL();
                                                eReplicatedFingerprint rfEntity = new eReplicatedFingerprint();
                                                try
                                                {
                                                    rfEntity.fingerprintId = Convert.ToInt32(arrIdUsuario[0]);
                                                }
                                                catch (Exception)
                                                {
                                                    rfEntity.fingerprintId = Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"]);
                                                }
                                                rfEntity.ipAddress = itemListaTerminal.ip;
                                                rfEntity.replicationDate = DateTime.Now;
                                                rfEntity.userId = dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString();


                                                FingerprintBLL fingerBll = new FingerprintBLL();
                                                if (esCard == true)
                                                {
                                                    string card = "";
                                                    try
                                                    {
                                                        if (dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString() == "" || dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString() == " " || dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString() == null)
                                                        {
                                                            card = dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString();
                                                        }
                                                        else
                                                        {
                                                            card = dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString();
                                                        }
                                                    }
                                                    catch (Exception)
                                                    {
                                                        card = dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString();
                                                    }

                                                    fingerBll.InsertReplicaTarjetas(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString(), card, false, itemListaTerminal.ip);
                                                    log.WriteProcessByTerminal("INSERTA EN BD RÉPLICA LA TARJETA DEL USUARIO: " + dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString(), itemListaTerminal.ip);

                                                }
                                                else if (rfBll.Insert(rfEntity))
                                                {
                                                    log.WriteProcessByTerminal("INSERTA EN BD RÉPLICA DEL USUARIO: " + dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString(), itemListaTerminal.ip);
                                                }

                                                log.WriteProcessByTerminal("SE REPLICO LA PERSONA: " + dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString(), itemListaTerminal.ip);
                                            }
                                            else
                                            {
                                                log.WriteProcessByTerminal("DESCARTA RÉPLICA INCONSISTENTE DE LA PERSONA CON ID: " + dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString(), itemListaTerminal.ip);
                                                continue;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        log.WriteProcessByTerminal("AUN NO ES HORARIO DE REPLICA DE LA CLASE PARA EL USUARIO " + dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString(), itemListaTerminal.ip);
                                    }
                                }
                                else
                                {
                                    string[] horariosRestriccion = dttReplicaUsuariosBioLiteRestriccion.Rows[i]["restrictions"].ToString().Split('|');

                                    //Cero es domingo
                                    DateTime diaActual = DateTime.Now;
                                    byte numDiaSemana = (byte)diaActual.DayOfWeek;

                                    DataTable dtvFila = new DataTable();
                                    dtvFila.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                                    DataRow drvFila;
                                    drvFila = dtvFila.NewRow();
                                    drvFila["strIpTerminal"] = itemListaTerminal.ip;
                                    dtvFila.Rows.Add(drvFila);

                                    for (int iDiaSemana = 0; iDiaSemana < 7; iDiaSemana++)
                                    {
                                        int iDiaSemanaAux = 0;
                                        //Se reorganizaron los numeros porque el registro de la tabla trae de lunes a domingo
                                        //Y el dayofweek trae domingo a lunes
                                        switch (iDiaSemana)
                                        {
                                            case 0:
                                                iDiaSemanaAux = 1;
                                                break;
                                            case 1:
                                                iDiaSemanaAux = 2;
                                                break;
                                            case 2:
                                                iDiaSemanaAux = 3;
                                                break;
                                            case 3:
                                                iDiaSemanaAux = 4;
                                                break;
                                            case 4:
                                                iDiaSemanaAux = 5;
                                                break;
                                            case 5:
                                                iDiaSemanaAux = 6;
                                                break;
                                            case 6:
                                                iDiaSemanaAux = 0;
                                                break;
                                        }

                                        if (iDiaSemanaAux == numDiaSemana)
                                        {

                                            string[] horasRestriccion = horariosRestriccion[iDiaSemana].Replace(" ", "").Split(';');

                                            string[] arrayHorarioGenerico = new string[horasRestriccion.Length];
                                            int contArrayGenrico = 0;

                                            foreach (string horas in horasRestriccion)
                                            {
                                                if (horas.Length > 2)
                                                {

                                                    string horasFinal = horas.Replace("SG", "");
                                                    string[] horaSeparada = horasFinal.Replace(" ", "").Split('-');

                                                    string idHorario = whiteListBLL.ConsultarHorariosRestriccionPorDiaHora(horaSeparada[0], horaSeparada[1]);
                                                    arrayHorarioGenerico[contArrayGenrico] = idHorario;

                                                    string[] horaEntradaRestriccion = horaSeparada[0].ToString().Split(':');
                                                    string[] horaSalidaRestriccion = horaSeparada[1].ToString().Split(':');
                                                    string SimboloAssciDiasSemana = "\u007f";

                                                    itemListaTerminal.conexionBioEntry.configurarHorario(drvFila, Convert.ToInt32(idHorario), SimboloAssciDiasSemana, Convert.ToInt32(horaEntradaRestriccion[0]), Convert.ToInt32(horaEntradaRestriccion[1]), Convert.ToInt32(horaSalidaRestriccion[0]), Convert.ToInt32(horaSalidaRestriccion[1]), Convert.ToUInt32(itemListaTerminal.snTerminal), itemListaTerminal.idTerminal);
                                                    int delayTerminate = 0;
                                                    Thread.Sleep(delayTerminate);

                                                    contArrayGenrico++;
                                                }
                                            }

                                            //consulta restriccion de zonas segun terminal
                                            string zonasTerminal = terminalObjectListBioLite.Find(tol => tol.ip == itemListaTerminal.ip).zonas;
                                            int idTerminal = terminalObjectListBioLite.Find(tol => tol.ip == itemListaTerminal.ip).idTerminal;

                                            bool seEliminoNoZona = false;

                                            if (zonasTerminal != null && zonasTerminal != "" && zonasTerminal != "0")
                                            {
                                                WhiteListAPI whiteListAPI = new WhiteListAPI();

                                                //for (int o = 0; o < dttReplicaUsuariosBioLiteRestriccion.Rows.Count; o++)
                                                //{
                                                bool auxExist = false;
                                                string zonasPlan = "";
                                                if (dttReplicaUsuariosBioLiteRestriccion.Rows[i]["typePerson"].ToString() == "Prospecto" && Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["planId"]) == 0)
                                                {
                                                    auxExist = true;
                                                }
                                                else
                                                {
                                                    if (bitIngresoEmpSinPlan == true && dttReplicaUsuariosBioLiteRestriccion.Rows[i]["typePerson"].ToString() == "Empleado")
                                                    {
                                                        auxExist = true;
                                                    }
                                                    else
                                                    {
                                                        zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["gymId"]), Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["branchId"]), Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["planId"]));
                                                    }
                                                }

                                                DataTable dtZonas = new DataTable();
                                                dtZonas = whiteListBLL.ConsultarZonas(zonasPlan);


                                                for (int ii = 0; ii < dtZonas.Rows.Count; ii++)
                                                {
                                                    string[] zonasPermitidasSplit = dtZonas.Rows[ii]["Terminales"].ToString().Split(',');

                                                    foreach (string itemZonaPermitida in zonasPermitidasSplit)
                                                    {
                                                        if (Convert.ToInt32(itemZonaPermitida) == idTerminal)
                                                        {
                                                            auxExist = true;
                                                        }
                                                    }
                                                }


                                                if (auxExist == false)
                                                {

                                                    DataRow dr = dttReplicaUsuariosBioLiteRestriccion.Rows[i];
                                                    dr.Delete();

                                                    seEliminoNoZona = true;
                                                }


                                                dttReplicaUsuariosBioLiteRestriccion.AcceptChanges();

                                            }

                                            if (seEliminoNoZona == false)
                                            {

                                                DataTable dtvFila2 = new DataTable();
                                                dtvFila2.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                                                DataRow drvFila2;
                                                drvFila2 = dtvFila2.NewRow();
                                                drvFila2["strIpTerminal"] = itemListaTerminal.ip;
                                                dtvFila2.Rows.Add(drvFila2);

                                                string[] arrIdUsuario;
                                                object[] arrIdUsuarioObj;

                                                clsComBioEntryPlus2.tipoAccesoLector[] tipos;
                                                bool esCard = false;

                                                if (Convert.ToBoolean(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["withoutFingerprint"]) == true && dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString().Length > 2)
                                                {
                                                    tipos = new clsComBioEntryPlus2.tipoAccesoLector[] { clsComBioEntryPlus2.tipoAccesoLector.TarjetaSolo };
                                                    arrIdUsuario = new string[] { dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString() };
                                                    arrIdUsuarioObj = new object[] { Array.Empty<byte>() };
                                                    esCard = true;
                                                }
                                                else if (Convert.ToBoolean(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["withoutFingerprint"]) == true && dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString().Length < 2)
                                                {
                                                    tipos = new clsComBioEntryPlus2.tipoAccesoLector[] { clsComBioEntryPlus2.tipoAccesoLector.ClaveSolo };
                                                    arrIdUsuario = new string[] { dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString() };
                                                    arrIdUsuarioObj = new object[] { Array.Empty<byte>() };
                                                    esCard = true;
                                                }
                                                else
                                                {
                                                    if (dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprint"] == null || dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprint"].ToString() == "")
                                                    {
                                                        tipos = new clsComBioEntryPlus2.tipoAccesoLector[] { clsComBioEntryPlus2.tipoAccesoLector.soloHuella };
                                                        arrIdUsuario = new string[] { "0" };
                                                        arrIdUsuarioObj = new object[] { Array.Empty<byte>() };
                                                        esCard = false;
                                                    }
                                                    else
                                                    {
                                                        tipos = new clsComBioEntryPlus2.tipoAccesoLector[] { clsComBioEntryPlus2.tipoAccesoLector.soloHuella };
                                                        arrIdUsuario = new string[] { dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprintId"].ToString() };
                                                        arrIdUsuarioObj = new object[] { (byte[])dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprint"] };
                                                        esCard = false;
                                                    }

                                                }

                                                bool[] arrayBoolGenerico = new bool[] { true };
                                                object[] byteNulo = new object[] { Array.Empty<byte>() };

                                                eTerminal dsTerminalesBio = new eTerminal();
                                                TerminalBLL terminal = new TerminalBLL();

                                                dsTerminalesBio = terminal.GetTerminalByIp(itemListaTerminal.ip);
                                                byte[] huella;

                                                if (Convert.IsDBNull(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprint"]))
                                                {
                                                    huella = new byte[0];
                                                }
                                                else
                                                {
                                                    huella = (byte[])dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprint"];
                                                }

                                                // if (itemListaTerminal.conexionBioEntry.EnrrolarMultiplesHuellasBioEntry(dtvFila2.Rows[0], tipos, arrIdUsuario, arrIdUsuarioObj, byteNulo, byteNulo, byteNulo, arrIdUsuario, arrayHorarioGenerico, arrayHorarioGenerico, Convert.ToUInt32(dsTerminalesBio.snTerminal), arrayBoolGenerico, dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString()) == respuestaDescarga.huellaEnrroladaSatisfactoriamente)
                                                if (itemListaTerminal.conexionBioEntry.EnrrolarMultiplesHuellasBioEntryNuevo(dtvFila.Rows[0], dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString(), huella, dttReplicaUsuariosBioLiteRestriccion.Rows[i]["fingerprintId"].ToString(), arrayHorarioGenerico, arrayHorarioGenerico, Convert.ToUInt32(dsTerminalesBio.snTerminal), true, dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString(), Convert.ToBoolean(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["withoutFingerprint"])) == respuestaDescarga.huellaEnrroladaSatisfactoriamente)
                                                {
                                                    int delayTerminate = 0;
                                                    Thread.Sleep(delayTerminate);
                                                    ReplicatedFingerprintBLL rfBll = new ReplicatedFingerprintBLL();
                                                    eReplicatedFingerprint rfEntity = new eReplicatedFingerprint();
                                                    try
                                                    {
                                                        rfEntity.fingerprintId = Convert.ToInt32(arrIdUsuario[0]);
                                                    }
                                                    catch (Exception)
                                                    {
                                                        rfEntity.fingerprintId = Convert.ToInt32(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"]);
                                                    }
                                                    rfEntity.ipAddress = itemListaTerminal.ip;
                                                    rfEntity.replicationDate = DateTime.Now;
                                                    rfEntity.userId = dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString();

                                                    FingerprintBLL fingerBll = new FingerprintBLL();
                                                    if (esCard == true)
                                                    {
                                                        string card = "";
                                                        try
                                                        {
                                                            if (dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString() == "" || dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString() == " " || dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString() == null)
                                                            {
                                                                card = dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString();
                                                            }
                                                            else
                                                            {
                                                                card = dttReplicaUsuariosBioLiteRestriccion.Rows[i]["cardId"].ToString();
                                                            }
                                                        }
                                                        catch (Exception)
                                                        {
                                                            card = dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString();
                                                        }
                                                                                                                    
                                                        fingerBll.InsertReplicaTarjetas(dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString(), card, false, itemListaTerminal.ip);
                                                        log.WriteProcessByTerminal("INSERTA EN BD RÉPLICA LA TARJETA DEL USUARIO: " + dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString(), itemListaTerminal.ip);

                                                    }
                                                    else if (rfBll.Insert(rfEntity))
                                                    {
                                                        log.WriteProcessByTerminal("INSERTA EN BD RÉPLICA DEL USUARIO: " + dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString(), itemListaTerminal.ip);
                                                    }

                                                    log.WriteProcessByTerminal("SE REPLICO EL USUARIO " + dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString(), itemListaTerminal.ip);
                                                }
                                                else
                                                {
                                                    log.WriteProcessByTerminal("DESCARTA RÉPLICA INCONSISTENTE DEL USUARIO CON ID " + dttReplicaUsuariosBioLiteRestriccion.Rows[i]["id"].ToString(), itemListaTerminal.ip);
                                                    continue;
                                                }
                                            }
                                        }
                                        else
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            tmReplicaUsuariosBioLiteRestriccion.Enabled = true;
            tmReplicaUsuariosBioLiteRestriccion.Start();

        }
        private void EliminarHellasTerminaVencidas(eListBiolite item)
        {
            DataTable dtt = new DataTable();
            FingerprintBLL bllEliminar = new FingerprintBLL();

            dtt = bllEliminar.GetEliminarHuellas(item.ip);

            if (dtt.Rows.Count > 0)
            {
                if (hacerping(item.ip))
                {
                    for (int i = 0; i < dtt.Rows.Count; i++)
                    {
                        DataTable dtvFila = new DataTable();
                        dtvFila.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                        DataRow drvFila;
                        drvFila = dtvFila.NewRow();
                        drvFila["strIpTerminal"] = item.ip;
                        dtvFila.Rows.Add(drvFila);

                        item.conexionBioEntry.EliminarHuellaUsuario(drvFila, 1471, dtt.Rows[i]["personId"].ToString(), false);
                        int delayTerminate = 0;
                        Thread.Sleep(delayTerminate);
                        bllEliminar.GetCabiarEstadoElimarHuellas(dtt.Rows[i]["personId"].ToString(), item.ip);
                    }
                }
            }
        }

        private void EliminarTarjetasTerminalVencidas(eListBiolite item)
        {
            DataTable dtt = new DataTable();
            FingerprintBLL bllEliminar = new FingerprintBLL();

            dtt = bllEliminar.GetEliminarTarjetas(item.ip);
            if (dtt != null)
            {
                if (dtt.Rows.Count > 0)
                {
                    if (hacerping(item.ip))
                    {
                        for (int e = 0; e < dtt.Rows.Count; e++)
                        {
                            DataTable dtvFila = new DataTable();
                            dtvFila.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                            DataRow drvFila;
                            drvFila = dtvFila.NewRow();
                            drvFila["strIpTerminal"] = item.ip;
                            dtvFila.Rows.Add(drvFila);

                            item.conexionBioEntry.EliminarHuellaUsuario(drvFila, 1471, dtt.Rows[e]["idPerson"].ToString(), false);
                            int delayTerminate = 0;
                            Thread.Sleep(delayTerminate);
                            bllEliminar.GetCambiarEstadoElimarTarjetas(dtt.Rows[e]["idPerson"].ToString(), item.ip);
                        }
                    }
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


            int i = 0;

            if (terminalObjectList.Count > 0)
            {
                foreach (clsWinSockCliente item in terminalObjectList)
                {
                    //OD 1689 - Getulio Vargas - 2020/01/28
                    //Se realiza validación para el proceso de réplica de huellas, para que sólo realice esto para las terminales de tipo 1 (TCAM7000)
                    //Se agrega el id 6 para que tambien realice a terminales de tipo 6 (XBIOS 3000GS)
                    if (ValidateTerminalStateReplicateFingerprints(item, true) && (item.terminalTypeId == 1 || item.terminalTypeId == 6))
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

            if (terminalObjectListBioLite.Count > 0)
            {
                try
                {
                    tmReplicaHuellasTCAM7000.Enabled = false;
                    tmReplicaHuellasTCAM7000.Stop();
                }
                catch (Exception)
                { }


                foreach (eListBiolite item in terminalObjectListBioLite)
                {

                    //Se realiza validación para el proceso de réplica de huellas, para BioLite N2 
                    if (hacerping(item.ip))
                    {
                        //ReplicaHuellasBioLite(item);
                        // ReplicaTarjetas(item);
                        ReplicaBioLiteNuevo(item);
                    }
                }

                tmReplicaHuellasTCAM7000.Enabled = true;
                tmReplicaHuellasTCAM7000.Start();
            }
        }

        private void ReplicaHuellasBioLite(eListBiolite item)
        {
            //System.Threading.Thread.Sleep(60 * oneSecond);
            fingerList = new List<eFingerprint>();
            FingerprintBLL fingerBll = new FingerprintBLL();
            ReplicatedFingerprintBLL rfBll = new ReplicatedFingerprintBLL();
            fingerList = fingerBll.GetFingerprintsToReplicate(item.ip);

            List<eFingerprint> listAuxFingerList = new List<eFingerprint>();

            string zonasTerminal = terminalObjectListBioLite.Find(tol => tol.ip == item.ip).zonas;
            int idTerminal = terminalObjectListBioLite.Find(tol => tol.ip == item.ip).idTerminal;

            if (zonasTerminal != null && zonasTerminal != "" && zonasTerminal != "0")
            {
                WhiteListAPI whiteListAPI = new WhiteListAPI();

                foreach (eFingerprint itemFinger in fingerList)
                {
                    bool auxExist = false;
                    string zonasPlan = "";
                    string zonaPlanAux = "";
                    if (itemFinger.typePerson == "Visitante" || itemFinger.typePerson == "Empleado" || itemFinger.typePerson == "Prospecto")
                    {
                        auxExist = true;
                    }
                    else
                    {
                        if (blnAccesoXReservaWeb == true && blnAccesoDUAL_Tradicional_y_XReservaWeb == false)
                        {
                            zonasPlan = whiteListAPI.GetListSucursalPorClase(Convert.ToInt32(itemFinger.gymId), Convert.ToInt32(itemFinger.branchId), Convert.ToInt32(itemFinger.reserveId));
                        }
                        else if (blnAccesoDUAL_Tradicional_y_XReservaWeb == true && blnAccesoXReservaWeb == false)
                        {
                            zonasPlan = whiteListAPI.GetListSucursalPorClase(Convert.ToInt32(itemFinger.gymId), Convert.ToInt32(itemFinger.branchId), Convert.ToInt32(itemFinger.reserveId));

                            zonaPlanAux = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(itemFinger.gymId), Convert.ToInt32(itemFinger.branchId), Convert.ToInt32(itemFinger.planId));

                            if (zonasPlan == "0" || zonasPlan == "" || zonasPlan == " " || zonasPlan == null)
                            {
                                zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(itemFinger.gymId), Convert.ToInt32(itemFinger.branchId), Convert.ToInt32(itemFinger.planId));
                            }
                        }
                        else if (zonasTerminal != null && zonasTerminal != "" && zonasTerminal != "0")
                        {
                            zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(itemFinger.gymId), Convert.ToInt32(itemFinger.branchId), Convert.ToInt32(itemFinger.planId));
                        }

                        //zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(itemFinger.gymId, itemFinger.branchId, Convert.ToInt32(itemFinger.planId));
                    }

                    WhiteListBLL whiteListBLL = new WhiteListBLL();
                    DataTable dtZonas = new DataTable();
                    DataTable dtZonasAux = new DataTable();
                    dtZonas = whiteListBLL.ConsultarZonas(zonasPlan);
                    dtZonasAux = whiteListBLL.ConsultarZonas(zonaPlanAux);
                    Boolean estadoauxiliar = false;

                    for (int i = 0; i < dtZonas.Rows.Count; i++)
                    {
                        string[] zonasPermitidasSplit = dtZonas.Rows[i]["Terminales"].ToString().Split(',');

                        foreach (string itemZonaPermitida in zonasPermitidasSplit)
                        {
                            if (Convert.ToInt32(itemZonaPermitida) == idTerminal)
                            {
                                auxExist = true;
                                estadoauxiliar = true;
                            }
                        }
                    }

                    if (blnAccesoDUAL_Tradicional_y_XReservaWeb == true && blnAccesoXReservaWeb == false && estadoauxiliar == false)
                    {
                        for (int ii = 0; ii < dtZonasAux.Rows.Count; ii++)
                        {
                            string[] zonasPermitidasSplit = dtZonasAux.Rows[ii]["Terminales"].ToString().Split(',');

                            foreach (string itemZonaPermitida in zonasPermitidasSplit)
                            {
                                if (Convert.ToInt32(itemZonaPermitida) == idTerminal)
                                {
                                    auxExist = true;

                                }
                            }
                        }

                    }


                    if (bitIngresoEmpSinPlan == true)
                    {
                        if (itemFinger.typePerson == "Empleado")
                        {
                            auxExist = true;
                        }
                    }

                    if (itemFinger.typePerson == "Prospecto" && Convert.ToInt32(itemFinger.planId) == 0)
                    {
                        auxExist = true;
                    }

                    if (auxExist == false)
                    {
                        listAuxFingerList.Add(itemFinger);
                    }
                }

                foreach (eFingerprint itemListAux in listAuxFingerList)
                {
                    fingerList.RemoveAll(x => x.personId == itemListAux.personId && x.id == itemListAux.id);
                }
            }

            //NO enviar visitantes o prospectos con mas de dos marcaciones
            listAuxFingerList = new List<eFingerprint>();
            foreach (eFingerprint itemFinger in fingerList)
            {
                WhiteListBLL whiteListBLL = new WhiteListBLL();
                DataTable dtCantidadEventosVisitante = new DataTable();
                dtCantidadEventosVisitante = whiteListBLL.obtenerIdVisitanteEliminar(itemFinger.id.ToString());
                if (dtCantidadEventosVisitante != null)
                {
                    if (dtCantidadEventosVisitante.Rows.Count > 0)
                    {
                        if (dtCantidadEventosVisitante.Rows.Count >= 2)
                        {
                            listAuxFingerList.Add(itemFinger);
                        }
                    }
                }
            }

            foreach (eFingerprint itemListAux in listAuxFingerList)
            {
                fingerList.RemoveAll(x => x.personId == itemListAux.personId && x.id == itemListAux.id);
            }

            foreach (eFingerprint itemFinger in fingerList)
            {
                DataTable dtvFila = new DataTable();
                dtvFila.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                DataRow drvFila;
                drvFila = dtvFila.NewRow();
                drvFila["strIpTerminal"] = item.ip;
                dtvFila.Rows.Add(drvFila);

                clsComBioEntryPlus2.tipoAccesoLector[] tipos = new clsComBioEntryPlus2.tipoAccesoLector[] { clsComBioEntryPlus2.tipoAccesoLector.soloHuella };

                string[] arrIdUsuario = new string[] { itemFinger.personId.ToString() };
                object[] arrIdUsuarioObj = new object[] { itemFinger.fingerPrint };

                List<string> listIdsHorarioRestrinccion = new List<string>();
                WhiteListBLL whiteListBLL = new WhiteListBLL();

                string[] arrayHorarioGenerico = new string[1];

                if (itemFinger.restrictions.Length <= 32)
                {
                    string idHorario = whiteListBLL.ConsultarHorariosRestriccionPorDiaHora("00:01:00:0", "23:59:00:0");

                    arrayHorarioGenerico[0] = idHorario;
                }
                else
                {
                    log.WriteProcessByTerminal("DESCARTA RÉPLICA DE HUELLA DEL USUARIO CON ID " + itemFinger.personId + " POR QUE SE VA A REALIZAR CON LA VALIDACION POR RESTRICCIÓN", item.ip);
                    continue;
                }

                bool[] arrayBoolGenerico = new bool[] { true };
                object[] byteNulo = new object[] { Array.Empty<byte>() };

                eTerminal dsTerminalesBio = new eTerminal();
                TerminalBLL terminal = new TerminalBLL();

                dsTerminalesBio = terminal.GetTerminalByIp(item.ip);

                //if(true)
                if (item.conexionBioEntry.EnrrolarMultiplesHuellasBioEntry(dtvFila.Rows[0], tipos, arrIdUsuario, arrIdUsuarioObj, byteNulo, byteNulo, byteNulo, arrIdUsuario, arrayHorarioGenerico, arrayHorarioGenerico, Convert.ToUInt32(dsTerminalesBio.snTerminal), arrayBoolGenerico, itemFinger.personId) == respuestaDescarga.huellaEnrroladaSatisfactoriamente)
                {
                    int delayTerminate = 0;
                    Thread.Sleep(delayTerminate);
                    log.WriteProcessByTerminal("RÉPLICA HUELLA CLIENTE: " + itemFinger.personId, item.ip);
                    eReplicatedFingerprint rfEntity = new eReplicatedFingerprint();
                    rfEntity.fingerprintId = itemFinger.id;
                    rfEntity.ipAddress = item.ip;
                    rfEntity.replicationDate = DateTime.Now;
                    rfEntity.userId = itemFinger.personId;

                    if (rfBll.Insert(rfEntity))
                    {
                        log.WriteProcessByTerminal("INSERTA EN BD RÉPLICA LA HUELLA DEL CLIENTE: " + itemFinger.personId + ", ID Huella: " + itemFinger.id.ToString(), item.ip);
                    }
                }
                else
                {
                    //NO INSERTO ESCRIBIR EN EL LOG
                    log.WriteProcessByTerminal("DESCARTA RÉPLICA DE HUELLA INCONSISTENTE DEL USUARIO CON ID " + itemFinger.personId, item.ip);
                    continue;
                }
            }
        }

        private void ReplicaTarjetas(eListBiolite item)
        {
            //System.Threading.Thread.Sleep(60 * oneSecond);
            FingerprintBLL fingerBll = new FingerprintBLL();

            DataTable dttTarjetas = new DataTable();
            dttTarjetas = fingerBll.GetTarjetasReplica(item.ip);

            if (dttTarjetas.Rows.Count > 0)
            {

                //log.GenerarArchivolOG("conectarTerminal", "PASO POR ACA " + item.ip, "errores", item.ip);
                //List<int> listAuxRowsList = new List<int>();

                string zonasTerminal = terminalObjectListBioLite.Find(tol => tol.ip == item.ip).zonas;
                int idTerminal = terminalObjectListBioLite.Find(tol => tol.ip == item.ip).idTerminal;

                if (zonasTerminal != null && zonasTerminal != "" && zonasTerminal != "0")
                {
                    WhiteListAPI whiteListAPI = new WhiteListAPI();

                    for (int e = 0; e < dttTarjetas.Rows.Count; e++)
                    {
                        bool auxExist = false;
                        string zonasPlan = "";
                        string zonaPlanAux = "";
                        if ((dttTarjetas.Rows[e]["typePerson"].ToString() == "Prospecto" || dttTarjetas.Rows[e]["typePerson"].ToString() == "Visitante") && Convert.ToInt32(dttTarjetas.Rows[e]["planId"]) == 0)
                        {
                            auxExist = true;
                        }
                        else
                        {
                            if (bitIngresoEmpSinPlan == true && dttTarjetas.Rows[e]["typePerson"].ToString() == "Empleado")
                            {
                                auxExist = true;
                            }
                            else
                            {
                                if (blnAccesoXReservaWeb == true && blnAccesoDUAL_Tradicional_y_XReservaWeb == false)
                                {
                                    zonasPlan = whiteListAPI.GetListSucursalPorClase(Convert.ToInt32(dttTarjetas.Rows[e]["gymId"]), Convert.ToInt32(dttTarjetas.Rows[e]["branchId"]), Convert.ToInt32(dttTarjetas.Rows[e]["reserveId"]));
                                }
                                else if (blnAccesoDUAL_Tradicional_y_XReservaWeb == true && blnAccesoXReservaWeb == false)
                                {
                                    zonasPlan = whiteListAPI.GetListSucursalPorClase(Convert.ToInt32(dttTarjetas.Rows[e]["gymId"]), Convert.ToInt32(dttTarjetas.Rows[e]["branchId"]), Convert.ToInt32(dttTarjetas.Rows[e]["reserveId"]));
                                    zonaPlanAux = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(dttTarjetas.Rows[e]["gymId"]), Convert.ToInt32(dttTarjetas.Rows[e]["branchId"]), Convert.ToInt32(dttTarjetas.Rows[e]["planId"]));

                                    if (zonasPlan == "0" || zonasPlan == "" || zonasPlan == " " || zonasPlan == null)
                                    {
                                        zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(dttTarjetas.Rows[e]["gymId"]), Convert.ToInt32(dttTarjetas.Rows[e]["branchId"]), Convert.ToInt32(dttTarjetas.Rows[e]["planId"]));
                                    }
                                }
                                else if (zonasTerminal != null && zonasTerminal != "" && zonasTerminal != "0")
                                {
                                    zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(dttTarjetas.Rows[e]["gymId"]), Convert.ToInt32(dttTarjetas.Rows[e]["branchId"]), Convert.ToInt32(dttTarjetas.Rows[e]["planId"]));
                                }

                                //zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(dttTarjetas.Rows[e]["gymId"]), Convert.ToInt32(dttTarjetas.Rows[e]["branchId"]), Convert.ToInt32(dttTarjetas.Rows[e]["planId"]));
                            }
                        }

                        WhiteListBLL whiteListBLL = new WhiteListBLL();
                        DataTable dtZonas = new DataTable();
                        DataTable dtZonasAux = new DataTable();

                        dtZonas = whiteListBLL.ConsultarZonas(zonasPlan);
                        dtZonasAux = whiteListBLL.ConsultarZonas(zonaPlanAux);
                        Boolean estadoauxiliar = false;

                        for (int i = 0; i < dtZonas.Rows.Count; i++)
                        {
                            string[] zonasPermitidasSplit = dtZonas.Rows[i]["Terminales"].ToString().Split(',');

                            foreach (string itemZonaPermitida in zonasPermitidasSplit)
                            {
                                if (Convert.ToInt32(itemZonaPermitida) == idTerminal)
                                {
                                    auxExist = true;
                                    estadoauxiliar = true;
                                }
                            }
                        }

                        if (blnAccesoDUAL_Tradicional_y_XReservaWeb == true && blnAccesoXReservaWeb == false && estadoauxiliar == false)
                        {
                            for (int ii = 0; ii < dtZonasAux.Rows.Count; ii++)
                            {
                                string[] zonasPermitidasSplit = dtZonasAux.Rows[ii]["Terminales"].ToString().Split(',');

                                foreach (string itemZonaPermitida in zonasPermitidasSplit)
                                {
                                    if (Convert.ToInt32(itemZonaPermitida) == idTerminal)
                                    {
                                        auxExist = true;

                                    }
                                }
                            }

                        }


                        if (dttTarjetas.Rows[e]["typePerson"].ToString() == "Prospecto" && Convert.ToInt32(dttTarjetas.Rows[e]["planId"]) == 0)
                        {
                            auxExist = true;
                        }

                        if (auxExist == false)
                        {
                            DataRow dr = dttTarjetas.Rows[e];
                            dr.Delete();
                        }
                    }

                    dttTarjetas.AcceptChanges();
                }

                //NO enviar visitantes o prospectos con mas de dos marcaciones
                for (int e = 0; e < dttTarjetas.Rows.Count; e++)
                {
                    WhiteListBLL whiteListBLL = new WhiteListBLL();
                    DataTable dtCantidadEventosVisitante = new DataTable();
                    dtCantidadEventosVisitante = whiteListBLL.obtenerIdVisitanteEliminar(dttTarjetas.Rows[e]["id"].ToString());
                    if (dtCantidadEventosVisitante != null)
                    {
                        if (dtCantidadEventosVisitante.Rows.Count > 0)
                        {
                            if (dtCantidadEventosVisitante.Rows.Count >= 2)
                            {
                                DataRow dr = dttTarjetas.Rows[e];
                                dr.Delete();
                            }
                        }
                    }
                }

                dttTarjetas.AcceptChanges();

                for (int i = 0; i < dttTarjetas.Rows.Count; i++)
                {
                    DataTable dtvFila = new DataTable();
                    dtvFila.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                    DataRow drvFila;
                    drvFila = dtvFila.NewRow();
                    drvFila["strIpTerminal"] = item.ip;
                    dtvFila.Rows.Add(drvFila);

                    clsComBioEntryPlus2.tipoAccesoLector[] tipos;

                    if (dttTarjetas.Rows[i]["cardId"].ToString().Length > 3)
                    {
                        tipos = new clsComBioEntryPlus2.tipoAccesoLector[] { clsComBioEntryPlus2.tipoAccesoLector.TarjetaSolo };
                    }
                    else
                    {
                        tipos = new clsComBioEntryPlus2.tipoAccesoLector[] { clsComBioEntryPlus2.tipoAccesoLector.ClaveSolo };
                    }


                    string[] arrIdUsuario = new string[] { dttTarjetas.Rows[i]["cardId"].ToString() };

                    WhiteListBLL whiteListBLL = new WhiteListBLL();

                    string[] arrayHorarioGenerico = new string[1];

                    if (dttTarjetas.Rows[i]["restrictions"].ToString().Length <= 32)
                    {
                        string idHorario = whiteListBLL.ConsultarHorariosRestriccionPorDiaHora("00:01:00:0", "23:59:00:0");

                        arrayHorarioGenerico[0] = idHorario;
                    }
                    else
                    {
                        log.WriteProcessByTerminal("DESCARTA RÉPLICA DE TARJETAS DEL USUARIO CON ID " + dttTarjetas.Rows[i]["id"].ToString() + ", CON TARJETA: " + dttTarjetas.Rows[i]["cardId"].ToString() + " POR QUE SE VA A REALIZAR CON LA VALIDACION POR RESTRICCIÓN", item.ip);
                        continue;
                    }

                    bool[] arrayBoolGenerico = new bool[] { true };
                    object[] byteNulo = new object[] { Array.Empty<byte>() };

                    eTerminal dsTerminalesBio = new eTerminal();
                    TerminalBLL terminal = new TerminalBLL();

                    dsTerminalesBio = terminal.GetTerminalByIp(item.ip);

                    //if(true)
                    if (item.conexionBioEntry.EnrrolarMultiplesHuellasBioEntry(dtvFila.Rows[0], tipos, arrIdUsuario, byteNulo, byteNulo, byteNulo, byteNulo, arrIdUsuario, arrayHorarioGenerico, arrayHorarioGenerico, Convert.ToUInt32(dsTerminalesBio.snTerminal), arrayBoolGenerico, dttTarjetas.Rows[i]["id"].ToString()) == respuestaDescarga.huellaEnrroladaSatisfactoriamente)
                    {
                        int delayTerminate = 0;
                        Thread.Sleep(delayTerminate);
                        if (dttTarjetas.Rows[i]["cardId"].ToString() == "" || dttTarjetas.Rows[i]["cardId"].ToString() == " " || dttTarjetas.Rows[i]["cardId"].ToString() == null)
                        {
                            dttTarjetas.Rows[i]["cardId"] = dttTarjetas.Rows[i]["id"].ToString();
                        }
                        fingerBll.InsertReplicaTarjetas(dttTarjetas.Rows[i]["id"].ToString(), dttTarjetas.Rows[i]["cardId"].ToString(), false, item.ip);
                        log.WriteProcessByTerminal("INSERTA EN BD RÉPLICA LA TARJETA DEL CLIENTE: " + dttTarjetas.Rows[i]["id"].ToString() + ", CON TARJETA: " + dttTarjetas.Rows[i]["cardId"].ToString(), item.ip);

                        log.WriteProcessByTerminal("SE REPLICO EL USUARIO " + dttTarjetas.Rows[i]["id"].ToString() + ", CON TARJETA: " + dttTarjetas.Rows[i]["cardId"].ToString(), item.ip);
                    }
                    else
                    {
                        //NO INSERTO ESCRIBIR EN EL LOG
                        log.WriteProcessByTerminal("DESCARTA RÉPLICA DE TARJETAS INCONSISTENTE DEL USUARIO CON ID " + dttTarjetas.Rows[i]["id"].ToString() + ", CON TARJETA: " + dttTarjetas.Rows[i]["cardId"].ToString(), item.ip);
                        continue;
                    }
                }
            }
        }

        private void ReplicaBioLiteNuevo(eListBiolite item)
        {
            fingerList = new List<eFingerprint>();
            FingerprintBLL fingerBll = new FingerprintBLL();
            ReplicatedFingerprintBLL rfBll = new ReplicatedFingerprintBLL();
            // tarjetas
            DataTable dttTarjetas = new DataTable();
            dttTarjetas = fingerBll.GetTarjetasReplica(item.ip);
            fingerList = fingerBll.GetFingerprintsToReplicate(item.ip);

            if (dttTarjetas.Rows.Count > 0)
            {
                string zonasTerminal = terminalObjectListBioLite.Find(tol => tol.ip == item.ip).zonas;
                int idTerminal = terminalObjectListBioLite.Find(tol => tol.ip == item.ip).idTerminal;

                if (zonasTerminal != null && zonasTerminal != "" && zonasTerminal != "0")
                {
                    WhiteListAPI whiteListAPI = new WhiteListAPI();

                    for (int e = 0; e < dttTarjetas.Rows.Count; e++)
                    {
                        bool auxExist = false;
                        string zonasPlan = "";
                        string zonaPlanAux = "";
                        if ((dttTarjetas.Rows[e]["typePerson"].ToString() == "Prospecto" || dttTarjetas.Rows[e]["typePerson"].ToString() == "Visitante") && Convert.ToInt32(dttTarjetas.Rows[e]["planId"]) == 0)
                        {
                            auxExist = true;
                        }
                        else
                        {
                            if (bitIngresoEmpSinPlan == true && dttTarjetas.Rows[e]["typePerson"].ToString() == "Empleado")
                            {
                                auxExist = true;
                            }
                            else
                            {
                                if (blnAccesoXReservaWeb == true && blnAccesoDUAL_Tradicional_y_XReservaWeb == false)
                                {
                                    zonasPlan = whiteListAPI.GetListSucursalPorClase(Convert.ToInt32(dttTarjetas.Rows[e]["gymId"]), Convert.ToInt32(dttTarjetas.Rows[e]["branchId"]), Convert.ToInt32(dttTarjetas.Rows[e]["reserveId"]));
                                }
                                else if (blnAccesoDUAL_Tradicional_y_XReservaWeb == true && blnAccesoXReservaWeb == false)
                                {
                                    zonasPlan = whiteListAPI.GetListSucursalPorClase(Convert.ToInt32(dttTarjetas.Rows[e]["gymId"]), Convert.ToInt32(dttTarjetas.Rows[e]["branchId"]), Convert.ToInt32(dttTarjetas.Rows[e]["reserveId"]));
                                    zonaPlanAux = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(dttTarjetas.Rows[e]["gymId"]), Convert.ToInt32(dttTarjetas.Rows[e]["branchId"]), Convert.ToInt32(dttTarjetas.Rows[e]["planId"]));

                                    if (zonasPlan == "0" || zonasPlan == "" || zonasPlan == " " || zonasPlan == null)
                                    {
                                        zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(dttTarjetas.Rows[e]["gymId"]), Convert.ToInt32(dttTarjetas.Rows[e]["branchId"]), Convert.ToInt32(dttTarjetas.Rows[e]["planId"]));
                                    }
                                }
                                else if (zonasTerminal != null && zonasTerminal != "" && zonasTerminal != "0")
                                {
                                    zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(dttTarjetas.Rows[e]["gymId"]), Convert.ToInt32(dttTarjetas.Rows[e]["branchId"]), Convert.ToInt32(dttTarjetas.Rows[e]["planId"]));
                                }
                            }
                        }

                        WhiteListBLL whiteListBLL = new WhiteListBLL();
                        DataTable dtZonas = new DataTable();
                        DataTable dtZonasAux = new DataTable();

                        dtZonas = whiteListBLL.ConsultarZonas(zonasPlan);
                        dtZonasAux = whiteListBLL.ConsultarZonas(zonaPlanAux);
                        Boolean estadoauxiliar = false;

                        for (int i = 0; i < dtZonas.Rows.Count; i++)
                        {
                            string[] zonasPermitidasSplit = dtZonas.Rows[i]["Terminales"].ToString().Split(',');

                            foreach (string itemZonaPermitida in zonasPermitidasSplit)
                            {
                                if (Convert.ToInt32(itemZonaPermitida) == idTerminal)
                                {
                                    auxExist = true;
                                    estadoauxiliar = true;
                                }
                            }
                        }

                        if (blnAccesoDUAL_Tradicional_y_XReservaWeb == true && blnAccesoXReservaWeb == false && estadoauxiliar == false)
                        {
                            for (int ii = 0; ii < dtZonasAux.Rows.Count; ii++)
                            {
                                string[] zonasPermitidasSplit = dtZonasAux.Rows[ii]["Terminales"].ToString().Split(',');

                                foreach (string itemZonaPermitida in zonasPermitidasSplit)
                                {
                                    if (Convert.ToInt32(itemZonaPermitida) == idTerminal)
                                    {
                                        auxExist = true;

                                    }
                                }
                            }

                        }


                        if (dttTarjetas.Rows[e]["typePerson"].ToString() == "Prospecto" && Convert.ToInt32(dttTarjetas.Rows[e]["planId"]) == 0)
                        {
                            auxExist = true;
                        }

                        if (auxExist == false)
                        {
                            DataRow dr = dttTarjetas.Rows[e];
                            dr.Delete();
                        }
                    }

                    dttTarjetas.AcceptChanges();
                }

                //NO enviar visitantes o prospectos con mas de dos marcaciones
                for (int e = 0; e < dttTarjetas.Rows.Count; e++)
                {
                    WhiteListBLL whiteListBLL = new WhiteListBLL();
                    DataTable dtCantidadEventosVisitante = new DataTable();
                    dtCantidadEventosVisitante = whiteListBLL.obtenerIdVisitanteEliminar(dttTarjetas.Rows[e]["id"].ToString());
                    if (dtCantidadEventosVisitante != null)
                    {
                        if (dtCantidadEventosVisitante.Rows.Count > 0)
                        {
                            if (dtCantidadEventosVisitante.Rows.Count >= 2)
                            {
                                DataRow dr = dttTarjetas.Rows[e];
                                dr.Delete();
                            }
                        }
                    }
                }

                dttTarjetas.AcceptChanges();

                for (int i = 0; i < dttTarjetas.Rows.Count; i++)
                {
                    DataTable dtvFila = new DataTable();
                    dtvFila.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                    DataRow drvFila;
                    drvFila = dtvFila.NewRow();
                    drvFila["strIpTerminal"] = item.ip;
                    dtvFila.Rows.Add(drvFila);

                    WhiteListBLL whiteListBLL = new WhiteListBLL();

                    string[] arrayHorarioGenerico = new string[1];

                    if (dttTarjetas.Rows[i]["restrictions"].ToString().Length <= 32)
                    {
                        if (Convert.ToBoolean(dttTarjetas.Rows[i]["isRestrictionClass"]) == true)
                        {
                            log.WriteProcessByTerminal("DESCARTA RÉPLICA DE TARJETAS DEL USUARIO CON ID " + dttTarjetas.Rows[i]["id"].ToString() + ", CON TARJETA: " + dttTarjetas.Rows[i]["cardId"].ToString() + " POR QUE SE VA A REALIZAR CON LA VALIDACION POR RESTRICCIÓN", item.ip);
                            continue;
                        }
                        else
                        {
                            string idHorario = whiteListBLL.ConsultarHorariosRestriccionPorDiaHora("00:01:00:0", "23:59:00:0");

                            arrayHorarioGenerico[0] = idHorario;
                        }
                    }
                    else
                    {
                        log.WriteProcessByTerminal("DESCARTA RÉPLICA DE TARJETAS DEL USUARIO CON ID " + dttTarjetas.Rows[i]["id"].ToString() + ", CON TARJETA: " + dttTarjetas.Rows[i]["cardId"].ToString() + " POR QUE SE VA A REALIZAR CON LA VALIDACION POR RESTRICCIÓN", item.ip);
                        continue;
                    }

                    eTerminal dsTerminalesBio = new eTerminal();
                    TerminalBLL terminal = new TerminalBLL();

                    dsTerminalesBio = terminal.GetTerminalByIp(item.ip);

                    if (item.conexionBioEntry.EnrrolarMultiplesHuellasBioEntryNuevo(dtvFila.Rows[0], dttTarjetas.Rows[i]["cardId"].ToString(), Array.Empty<byte>(), "", arrayHorarioGenerico, arrayHorarioGenerico, Convert.ToUInt32(dsTerminalesBio.snTerminal), true, dttTarjetas.Rows[i]["id"].ToString(), Convert.ToBoolean(dttTarjetas.Rows[i]["withoutFingerprint"])) == respuestaDescarga.huellaEnrroladaSatisfactoriamente)
                    {
                        int delayTerminate = 0;
                        Thread.Sleep(delayTerminate);
                        if (dttTarjetas.Rows[i]["cardId"].ToString() == "" || dttTarjetas.Rows[i]["cardId"].ToString() == " " || dttTarjetas.Rows[i]["cardId"].ToString() == null || dttTarjetas.Rows[i]["cardId"].ToString() == "0")
                        {
                            dttTarjetas.Rows[i]["cardId"] = dttTarjetas.Rows[i]["id"].ToString();
                        }
                        fingerBll.InsertReplicaTarjetas(dttTarjetas.Rows[i]["id"].ToString(), dttTarjetas.Rows[i]["cardId"].ToString(), false, item.ip);
                        log.WriteProcessByTerminal("INSERTA EN BD RÉPLICA LA TARJETA DEL USUARIO: " + dttTarjetas.Rows[i]["id"].ToString() + ", CON TARJETA: " + dttTarjetas.Rows[i]["cardId"].ToString(), item.ip);

                        log.WriteProcessByTerminal("SE REPLICO LA PERSONA CON IDENTIFICACION: " + dttTarjetas.Rows[i]["id"].ToString(), item.ip);
                    }
                    else
                    {
                        //NO INSERTO ESCRIBIR EN EL LOG
                        log.WriteProcessByTerminal("DESCARTA RÉPLICA DE TARJETAS INCONSISTENTE DEL USUARIO CON ID: " + dttTarjetas.Rows[i]["id"].ToString() + ", CON TARJETA: " + dttTarjetas.Rows[i]["cardId"].ToString(), item.ip);
                        continue;
                    }
                }
            }

            // huella


            if (fingerList != null && fingerList.Count > 0)
            {
                List<eFingerprint> listAuxFingerList = new List<eFingerprint>();

                string zonasTerminal = terminalObjectListBioLite.Find(tol => tol.ip == item.ip).zonas;
                int idTerminal = terminalObjectListBioLite.Find(tol => tol.ip == item.ip).idTerminal;

                if (zonasTerminal != null && zonasTerminal != "" && zonasTerminal != "0")
                {
                    WhiteListAPI whiteListAPI = new WhiteListAPI();

                    foreach (eFingerprint itemFinger in fingerList)
                    {
                        bool auxExist = false;
                        string zonasPlan = "";
                        string zonaPlanAux = "";
                        if (itemFinger.typePerson == "Visitante" || itemFinger.typePerson == "Empleado" || itemFinger.typePerson == "Prospecto")
                        {
                            auxExist = true;
                        }
                        else
                        {
                            if (blnAccesoXReservaWeb == true && blnAccesoDUAL_Tradicional_y_XReservaWeb == false)
                            {
                                zonasPlan = whiteListAPI.GetListSucursalPorClase(Convert.ToInt32(itemFinger.gymId), Convert.ToInt32(itemFinger.branchId), Convert.ToInt32(itemFinger.reserveId));
                            }
                            else if (blnAccesoDUAL_Tradicional_y_XReservaWeb == true && blnAccesoXReservaWeb == false)
                            {
                                zonasPlan = whiteListAPI.GetListSucursalPorClase(Convert.ToInt32(itemFinger.gymId), Convert.ToInt32(itemFinger.branchId), Convert.ToInt32(itemFinger.reserveId));

                                zonaPlanAux = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(itemFinger.gymId), Convert.ToInt32(itemFinger.branchId), Convert.ToInt32(itemFinger.planId));

                                if (zonasPlan == "0" || zonasPlan == "" || zonasPlan == " " || zonasPlan == null)
                                {
                                    zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(itemFinger.gymId), Convert.ToInt32(itemFinger.branchId), Convert.ToInt32(itemFinger.planId));
                                }
                            }
                            else if (zonasTerminal != null && zonasTerminal != "" && zonasTerminal != "0")
                            {
                                zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(Convert.ToInt32(itemFinger.gymId), Convert.ToInt32(itemFinger.branchId), Convert.ToInt32(itemFinger.planId));
                            }
                        }

                        WhiteListBLL whiteListBLL = new WhiteListBLL();
                        DataTable dtZonas = new DataTable();
                        DataTable dtZonasAux = new DataTable();
                        dtZonas = whiteListBLL.ConsultarZonas(zonasPlan);
                        dtZonasAux = whiteListBLL.ConsultarZonas(zonaPlanAux);
                        Boolean estadoauxiliar = false;

                        for (int i = 0; i < dtZonas.Rows.Count; i++)
                        {
                            string[] zonasPermitidasSplit = dtZonas.Rows[i]["Terminales"].ToString().Split(',');

                            foreach (string itemZonaPermitida in zonasPermitidasSplit)
                            {
                                if (Convert.ToInt32(itemZonaPermitida) == idTerminal)
                                {
                                    auxExist = true;
                                    estadoauxiliar = true;
                                }
                            }
                        }

                        if (blnAccesoDUAL_Tradicional_y_XReservaWeb == true && blnAccesoXReservaWeb == false && estadoauxiliar == false)
                        {
                            for (int ii = 0; ii < dtZonasAux.Rows.Count; ii++)
                            {
                                string[] zonasPermitidasSplit = dtZonasAux.Rows[ii]["Terminales"].ToString().Split(',');

                                foreach (string itemZonaPermitida in zonasPermitidasSplit)
                                {
                                    if (Convert.ToInt32(itemZonaPermitida) == idTerminal)
                                    {
                                        auxExist = true;

                                    }
                                }
                            }

                        }

                        if (bitIngresoEmpSinPlan == true)
                        {
                            if (itemFinger.typePerson == "Empleado")
                            {
                                auxExist = true;
                            }
                        }

                        if (itemFinger.typePerson == "Prospecto" && Convert.ToInt32(itemFinger.planId) == 0)
                        {
                            auxExist = true;
                        }

                        if (auxExist == false)
                        {
                            listAuxFingerList.Add(itemFinger);
                        }
                    }

                    foreach (eFingerprint itemListAux in listAuxFingerList)
                    {
                        fingerList.RemoveAll(x => x.personId == itemListAux.personId && x.id == itemListAux.id);
                    }
                }

                //NO enviar visitantes o prospectos con mas de dos marcaciones
                listAuxFingerList = new List<eFingerprint>();
                foreach (eFingerprint itemFinger in fingerList)
                {
                    WhiteListBLL whiteListBLL = new WhiteListBLL();
                    DataTable dtCantidadEventosVisitante = new DataTable();
                    dtCantidadEventosVisitante = whiteListBLL.obtenerIdVisitanteEliminar(itemFinger.id.ToString());
                    if (dtCantidadEventosVisitante != null)
                    {
                        if (dtCantidadEventosVisitante.Rows.Count > 0)
                        {
                            if (dtCantidadEventosVisitante.Rows.Count >= 2)
                            {
                                listAuxFingerList.Add(itemFinger);
                            }
                        }
                    }
                }

                foreach (eFingerprint itemListAux in listAuxFingerList)
                {
                    fingerList.RemoveAll(x => x.personId == itemListAux.personId && x.id == itemListAux.id);
                }

                foreach (eFingerprint itemFinger in fingerList)
                {
                    DataTable dtvFila = new DataTable();
                    dtvFila.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                    DataRow drvFila;
                    drvFila = dtvFila.NewRow();
                    drvFila["strIpTerminal"] = item.ip;
                    dtvFila.Rows.Add(drvFila);

                    string[] arrIdUsuario = new string[] { itemFinger.personId.ToString() };
                    object[] arrIdUsuarioObj = new object[] { itemFinger.fingerPrint };

                    List<string> listIdsHorarioRestrinccion = new List<string>();
                    WhiteListBLL whiteListBLL = new WhiteListBLL();

                    string[] arrayHorarioGenerico = new string[1];

                    if (itemFinger.restrictions.Length <= 32)
                    {
                        if (Convert.ToInt32(itemFinger.reserveId) == 0 || itemFinger.reserveId == null)
                        {
                            log.WriteProcessByTerminal("DESCARTA RÉPLICA DE TARJETAS DEL USUARIO CON ID " + itemFinger.personId.ToString() + ", "  + " POR QUE SE VA A REALIZAR CON LA VALIDACION POR RESTRICCIÓN", item.ip);
                            continue;
                        }
                        else
                        {
                            string idHorario = whiteListBLL.ConsultarHorariosRestriccionPorDiaHora("00:01:00:0", "23:59:00:0");

                            arrayHorarioGenerico[0] = idHorario;
                        }


                    }
                    else
                    {
                        log.WriteProcessByTerminal("DESCARTA RÉPLICA DE HUELLA DEL USUARIO CON ID " + itemFinger.personId + " POR QUE SE VA A REALIZAR CON LA VALIDACION POR RESTRICCIÓN", item.ip);
                        continue;
                    }

                    eTerminal dsTerminalesBio = new eTerminal();
                    TerminalBLL terminal = new TerminalBLL();

                    dsTerminalesBio = terminal.GetTerminalByIp(item.ip);

                    if (item.conexionBioEntry.EnrrolarMultiplesHuellasBioEntryNuevo(dtvFila.Rows[0], itemFinger.cardId, itemFinger.fingerPrint, (itemFinger.id == 0 ? "" : itemFinger.personId.ToString()), arrayHorarioGenerico, arrayHorarioGenerico, Convert.ToUInt32(dsTerminalesBio.snTerminal), true, itemFinger.personId, itemFinger.withoutFingerprint) == respuestaDescarga.huellaEnrroladaSatisfactoriamente)
                    {
                        int delayTerminate = 0;
                        Thread.Sleep(delayTerminate);
                        log.WriteProcessByTerminal("RÉPLICA HUELLA CLIENTE: " + itemFinger.personId, item.ip);
                        eReplicatedFingerprint rfEntity = new eReplicatedFingerprint();
                        rfEntity.fingerprintId = itemFinger.id;
                        rfEntity.ipAddress = item.ip;
                        rfEntity.replicationDate = DateTime.Now;
                        rfEntity.userId = itemFinger.personId;

                        if (rfBll.Insert(rfEntity))
                        {
                            log.WriteProcessByTerminal("INSERTA EN BD RÉPLICA DEL USUARIO CON IDENTIFICACION: " + itemFinger.personId, item.ip);
                        }
                    }
                    else
                    {
                        //NO INSERTO ESCRIBIR EN EL LOG
                        log.WriteProcessByTerminal("DESCARTA RÉPLICA DE HUELLA INCONSISTENTE DEL USUARIO CON ID " + itemFinger.personId, item.ip);
                        continue;
                    }
                }
            }
            //else
            //{

            //}
        }

        private void ControlReplicateFingerprints(clsWinSockCliente actualTerminal, bool firstTime)
        {
            if (firstTime)
            {
                log.WriteTerminals("INICIA EL PROCESO DE RÉPLICA DE HUELLAS A LAS TERMINALES.", actualTerminal.IpAddress);
                ReplicateFingerprints(actualTerminal.IpAddress, false, false);
            }
            else
            {
                if (indexReplicateFingerprints == startIndexReplicateFingerprints)
                {
                    log.WriteTerminals("FINALIZA EL PROCESO DE RÉPLICA DE HUELLAS A LAS TERMINALES.", actualTerminal.IpAddress);
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
                            log.WriteTerminals("FINALIZA EL PROCESO DE RÉPLICA DE HUELLAS EN LAS TERMINALES.", actualTerminal.IpAddress);
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
                List<eFingerprint> listAuxFingerList = new List<eFingerprint>();

                string zonasTerminal = terminalObjectList.Find(tol => tol.IpAddress == ipAddress).Zonas;
                int idTerminal = terminalObjectList.Find(tol => tol.IpAddress == ipAddress).IdTerminal;

                if (zonasTerminal != null && zonasTerminal != "" && zonasTerminal != "0")
                {
                    WhiteListAPI whiteListAPI = new WhiteListAPI();

                    foreach (eFingerprint item in fingerList)
                    {
                        bool auxExist = false;
                        string zonasPlan = "";
                        if (item.typePerson == "Visitante" || item.typePerson == "Empleado" || item.typePerson == "Prospecto")
                        {
                            auxExist = true;
                        }
                        else
                        {
                            zonasPlan = whiteListAPI.GetListPlanesSucursalPorPlan(item.gymId, item.branchId, Convert.ToInt32(item.planId));
                        }

                        WhiteListBLL whiteListBLL = new WhiteListBLL();
                        DataTable dtZonas = new DataTable();

                        dtZonas = whiteListBLL.ConsultarZonas(zonasPlan);

                        for (int i = 0; i < dtZonas.Rows.Count; i++)
                        {
                            string[] zonasPermitidasSplit = dtZonas.Rows[i]["Terminales"].ToString().Split(',');

                            foreach (string itemZonaPermitida in zonasPermitidasSplit)
                            {
                                if (Convert.ToInt32(itemZonaPermitida) == idTerminal)
                                {
                                    auxExist = true;
                                }
                            }
                        }

                        if (bitIngresoEmpSinPlan == true)
                        {
                            if (item.typePerson == "Empleado")
                            {
                                auxExist = true;
                            }
                        }

                        if (item.typePerson == "Prospecto" && Convert.ToInt32(item.planId) == 0)
                        {
                            auxExist = true;
                        }

                        if (auxExist == false)
                        {
                            listAuxFingerList.Add(item);
                        }
                    }

                    foreach (eFingerprint itemListAux in listAuxFingerList)
                    {
                        fingerList.RemoveAll(x => x.personId == itemListAux.personId && x.id == itemListAux.id);
                    }
                }
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
            tmReplicaHuellasTCAM7000.Enabled = true;
            tmReplicaHuellasTCAM7000.Start();

            if (blnReplicaImgsTCAM7000)
            {
                tmReplicaImgsTCAM7000.Start();
            }
        }

        private int GenerateCommandToReplicateFingerprint(string clientId, int recordId, byte[] tmpFinger, string ipAddress)
        {
            eTerminal objeTerminal = new eTerminal();
            TerminalBLL terminalBLL = new TerminalBLL();

            objeTerminal = terminalBLL.GetTerminalByIp(ipAddress);

            if (objeTerminal.terminalTypeId == 6)
            {
                byte[] fingerData = new byte[997], dsDatoTmp = new byte[997];
                Array.Copy(tmpFinger, fingerData, 997);
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

                        if (i >= 995)
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

                //if (dsDatoTmp[0] != 52)
                //{
                //    return -1;
                //}

                command += ";" + System.Text.Encoding.UTF8.GetString(dsDatoTmp).Substring(0, 996);
                command += "$";

                actionService = clsEnum.ServiceActions.WaitingConfirmFingerprintReplicate;
                SendDataAndLogs(command, ipAddress);
                log.WriteProcessByTerminal("Huella a replicar - " + command, ipAddress);
                return 0;
            }
            else
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
        private void FillConfigurationParameters(List<eConfiguration> config)
        {
            try
            {
                if (config[0].intTiempoPingContinuoTCAM7000 > 0)
                {
                    pingTime = config[0].intTiempoPingContinuoTCAM7000 * oneSecond;
                }

                blnBaseDatosSQL = config[0].bitBaseDatosSQLServer;
                blnMensajeCumpleanos = config[0].bitMensajeCumpleanos;

                if (!string.IsNullOrEmpty(config[0].strTextoMensajeCumpleanos))
                {
                    strTextoMensajeCumpleanos = config[0].strTextoMensajeCumpleanos;
                }

                blnComplices_CortxIngs = config[0].bitComplices_CortxIngs;
                blnComplices_DescuentoTiq = config[0].bitComplices_DescuentoTiq;

                if (!string.IsNullOrEmpty(config[0].strTextoMensajeCortxIngs))
                {
                    strTextoMensajeCortxIngs = config[0].strTextoMensajeCortxIngs;
                }

                intComplices_Plan_CortxIngs = config[0].intComplices_Plan_CortxIngs;
                blnMultiplesPlanesVig = config[0].bitMultiplesPlanesVig;
                blnSolo1HuellaxCliente = config[0].bitSolo1HuellaxCliente;
                blnValideContrato = config[0].bitValideContrato;
                blnValideContPorFactura = config[0].bitValideContratoPorFactura;
                blnReplicaImgsTCAM7000 = config[0].bitReplicaImgsTCAM7000;

                if (config[0].intTiempoReplicaImgsTCAM7000 > 0)
                {
                    intTiempoReplicaImgsTCAM7000 = config[0].intTiempoReplicaImgsTCAM7000 * oneSecond;
                }

                if (config[0].intTiempoEspaciadoTramasReplicaImgsTCAM7000 > 0)
                {
                    intTiempoEspaciadoTramasReplicaImgsTCAM7000 = config[0].intTiempoEspaciadoTramasReplicaImgsTCAM7000;
                }

                if (config[0].intTiempoMaximoEnvioImgsTCAM7000 > 0)
                {
                    intTiempoMaximoEnvioImgsTCAM7000 = config[0].intTiempoMaximoEnvioImgsTCAM7000;
                }

                blnValidarMSW = config[0].bitConsultaInfoCita;
                blnAccesoDiscapacitados = config[0].bitAccesoDiscapacitados;
                blnTrabajarConDBEnOtroEquipo = config[0].bitTrabajarConDBEnOtroEquipo;
                bitIngresoEmpSinPlan = config[0].bitIngresoEmpSinPlan;

                if (config[0].intTiempoParaLimpiarPantalla > 0)
                {
                    tmlimpiarPantalla = config[0].intTiempoParaLimpiarPantalla * oneSecond;
                }

                if (config[0].intTiempoActualizaIngresos > 0)
                {
                    intTIEMPO_ACTUALIZA_INGRESOS = config[0].intTiempoActualizaIngresos;
                }

                if (config[0].intMinutosDescontarTiquetes > 0)
                {
                    intTiempoRestarTiquetes = config[0].intMinutosDescontarTiquetes;
                }

                if (!string.IsNullOrEmpty(strRutaBanner))
                {
                    strRutaBanner = config[0].strRutaNombreBanner;
                }

                if (config[0].intTiempoPulso > 0)
                {
                    intTiempoPulso = config[0].intTiempoPulso;
                }

                if (!string.IsNullOrEmpty(strClave))
                {
                    strClave = config[0].strClave;
                }

                blnPermitirIngresoAdicional = config[0].blnPermiteIngresosAdicionales;

                if (!string.IsNullOrEmpty(strRutaLogo))
                {
                    strRutaLogo = config[0].strRutaNombreLogo;
                }

                blnImagenSIBO = config[0].bitImagenSIBO;

                if (!string.IsNullOrEmpty(strColorPpal))
                {
                    strColorPpal = config[0].strColorPpal;
                }

                if (!string.IsNullOrEmpty(strRutaGuiaMarcacion))
                {
                    strRutaGuiaMarcacion = config[0].strRutaGuiaMarcacion;
                }

                blnBloqueoNoCitaMSW = config[0].bitBloqueoCita;
                blnBloqueoNoAptoMSW = config[0].bitBloqueoClienteNoApto;
                blnBloqueoNoConsentimiento = config[0].bitBloqueoNoDisentimento;
                blnBloqueoNoAutorizacionMenor = config[0].bitBloqueoNoAutorizacionMenor;
                blnAntipassbackEntrada = config[0].bitAntipassbackEntrada;
                blnImprimirHoraReserva = config[0].bitImprimirHoraReserva;
                blnTiqueteClaseAsistido_alImprimir = config[0].bitTiqueteClaseAsistido_alImprimir;
                blnAccesoXReservaWeb = config[0].bitAccesoPorReservaWeb;
                intTiempoAntesAccesoXReservaWeb = config[0].intMinutosAntesReserva;
                intTiempoDespuesAccesoXReservaWeb = config[0].intMinutosDespuesReserva;
                blnAccesoDUAL_Tradicional_y_XReservaWeb = config[0].bitValidarPlanYReservaWeb;
                blnNoValidarEntradaSalida = config[0].bitNo_Validar_Entrada_En_Salida;
                blnEsperarHuellaActualizar = config[0].bitEsperarHuellaActualizar;
                blnIngresoAbreDesdeTouch = config[0].bitIngresoAbreDesdeTouch;
                blnNoValidarHuella = config[0].bitNoValidarHuella;
                signContracteToEnroll = config[0].bitFirmarContratoAlEnrolar;
                generatePDFContractAndSend = config[0].bitGenerarContratoPDFyEnviar;

                if (config[0].intTiempoReplicaHuellasTCAM7000 > 0)
                {
                    intTiempoReplicaHuellasTCAM7000 = config[0].intTiempoReplicaHuellasTCAM7000 * oneSecond;
                }

                if (config[0].timeGetPendingActions > 0)
                {
                    timeGetPendingActions = config[0].timeGetPendingActions * oneSecond;
                }

                if (config[0].timeRemoveFingerprints > 0)
                {
                    timeRemoveFingerprints = config[0].timeRemoveFingerprints * oneSecond;
                }

                allowWhiteListTCAM = config[0].allowWhiteListTCAM;

                if (config[0].timeInsertWhiteListTCAM > 0)
                {
                    timeWhitelistTCAM = config[0].timeInsertWhiteListTCAM * oneSecond;
                }

                //Nuevos parámetros
                if (config[0].timeTerminalConnections > 0)
                {
                    timeTerminalConnections = config[0].timeTerminalConnections * oneSecond;
                }

                if (config[0].intTiempoEsperaRespuestaReplicaHuellasTCAM7000 > 0)
                {
                    intTiempoEsperaRespuestaReplicaHuellasTCAM7000 = config[0].intTiempoEsperaRespuestaReplicaHuellasTCAM7000 * oneSecond;
                }

                if (config[0].timeWaitResponseReplicateUsers > 0)
                {
                    timeWaitResponseReplicateUsers = config[0].timeWaitResponseReplicateUsers * oneSecond;
                }

                if (config[0].timeWaitResponseDeleteFingerprint > 0)
                {
                    timeWaitResponseDeleteFingerprint = config[0].timeWaitResponseDeleteFingerprint * oneSecond;
                }

                if (config[0].timeWaitResponseDeleteUser > 0)
                {
                    timeWaitResponseDeleteUser = config[0].timeWaitResponseDeleteUser * oneSecond;
                }

                if (config[0].timeResetDownloadEvents > 0)
                {
                    timeResetDownloadEvents = config[0].timeResetDownloadEvents * oneSecond;
                }

                if (config[0].timeRemoveUsers > 0)
                {
                    timeRemoveUsers = config[0].timeRemoveUsers * oneSecond;
                }

                if (config[0].timeDowndloadEvents > 0)
                {
                    timeDowndloadEvents = config[0].timeDowndloadEvents * oneSecond;
                }

                if (config[0].timeHourSync > 0)
                {
                    timeHourSync = config[0].timeHourSync * oneSecond;
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

        private void ConnectBioLite()
        {
            try
            {
                ConnectSocketBioLite();
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
                        if (hacerping(item.ipAddress))
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
                                        winSocketClient = new clsWinSockCliente(item.ipAddress, Convert.ToInt32(item.port), item.withWhiteList, false, item.terminalTypeId, item.Zonas, item.terminalId);

                                        if (winSocketClient.connected)
                                        {
                                            terminalObjectList.Add(winSocketClient);
                                            terminalObjectListAux.Add(winSocketClient);
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
                            log.WriteProcess("No es posible realizar la conexión de la terminal: " + item.ipAddress + ", no responde a ping.");
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

        private void ConnectSocketBioLite()
        {
            string lastPart = string.Empty, ipAddressAux = string.Empty;

            if (termListBioLite != null && termListBioLite.Count > 0)
            {
                foreach (eTerminal item in termListBioLite)
                {
                    if (item.state)
                    {
                        ipAddressAux = item.ipAddress;

                        if (!string.IsNullOrEmpty(item.ipAddress))
                        {
                            log.WriteProcess("Inicia el proceso de conexión de la terminal " + item.ipAddress);

                            eListBiolite exist = terminalObjectListBioLite.Find(tol => tol.ip == ipAddressAux);

                            if (exist != null)
                            {
                                log.WriteProcess("La terminal " + item.ipAddress + " ya se encuentra conectada.");
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.port))
                                {
                                    if (hacerping(ipAddressAux))
                                    {
                                        clsComBioEntryPlus2 libcomBioPlus2 = new clsComBioEntryPlus2();
                                        bool bresultado = false;
                                        bresultado = libcomBioPlus2.conectarTerminal(ipAddressAux, Convert.ToUInt16(item.port.ToString()));

                                        if (bresultado)
                                        {
                                            terminalObjectListBioLite.Add(new eListBiolite { ip = ipAddressAux, codigo = item.terminalId, zonas = item.Zonas, idTerminal = item.terminalId, snTerminal = item.snTerminal, conexionBioEntry = libcomBioPlus2 });
                                            //libcomBioPlus2.bConectada = true;
                                            int delayTerminate = 0;
                                            Thread.Sleep(delayTerminate);
                                        }
                                        else
                                        {
                                            log.WriteProcess("No es posible realizar la conexión de la terminal " + item.ipAddress + ", debido a que no responde a conectividad.");
                                        }
                                    }
                                    else
                                    {
                                        log.WriteProcess("No es posible realizar la conexión de la terminal " + item.ipAddress + ", debido a que no responde a conectividad.");
                                    }
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
                string pruebas = Encoding.ASCII.GetString(data);
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

                if (objData.Length == 11)
                {
                    if (objData[0] != 0 && Encoding.ASCII.GetString(objData).Contains("@PLAN"))
                    {
                        planSeleccionado = Regex.Replace(Encoding.ASCII.GetString(objData), @"[^\d]+", "");
                    }
                    objData = null;
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
                        if (objData.Length == 16 && Encoding.ASCII.GetString(objData).Substring(0, 1) == "@" && Encoding.ASCII.GetString(objData).Substring(objData.Length - 3, 1) == "$")
                        {
                            actionService = clsEnum.ServiceActions.WaitingClientId;
                        }

                        if (objData.Length == 999 && Encoding.ASCII.GetString(objData).Substring(objData.Length - 3, 1) == "$")
                        {
                            byte[] fingerprintBytes = new byte[1024];
                            string temp = string.Empty;
                            log.WriteProcessByTerminal("LLEGA HUELLA DEL CLIENTE", ipAddress);
                            temp = Encoding.ASCII.GetString(objData).Substring(0, 498);
                            log.WriteProcessByTerminal("Huella a grabar", ipAddress);
                            log.WriteProcessByTerminal(temp, ipAddress);
                            fingerprintBytes = Encoding.ASCII.GetBytes(temp);
                            RecordFingerprint(fingerprintBytes, ipAddress,"cama");

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


                        String P = Encoding.ASCII.GetString(objData);
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
                            RecordFingerprint(fingerprintBytes, ipAddress, "suprema");

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
                        if (objData.Length == 999 && Encoding.ASCII.GetString(objData).Substring(objData.Length - 3, 1) == "$")
                        {
                            byte[] fingerprintBytes = new byte[1024];
                            string temp = string.Empty;
                            log.WriteProcessByTerminal("LLEGA HUELLA DEL CLIENTE", ipAddress);
                            temp = Encoding.ASCII.GetString(objData).Substring(0, 498);
                            log.WriteProcessByTerminal("Huella a grabar", ipAddress);
                            log.WriteProcessByTerminal(temp, ipAddress);
                            fingerprintBytes = Encoding.ASCII.GetBytes(temp);
                            RecordFingerprint(fingerprintBytes, ipAddress,"cama");

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
                                    string strGymId = ConfigurationSettings.AppSettings["gymId"].ToString();

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
                        else if (objData.Length == 22 && Encoding.ASCII.GetString(objData).Substring(0, 1) == "@" && Encoding.ASCII.GetString(objData).Substring(1, 1) == "Q" != Encoding.ASCII.GetString(objData).Contains("."))
                        {
                            string strGymId = ConfigurationSettings.AppSettings["gymId"].ToString();
                            if (!string.IsNullOrEmpty(strGymId))
                            {
                                gymId = Convert.ToInt32(strGymId);
                            }
                            string qr = Encoding.ASCII.GetString(objData).Substring(objData.Length - 13, 5).ToString();
                            string temperature = Encoding.ASCII.GetString(objData).Substring(objData.Length - 7, 4).ToString();
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
                        // OD 2285 criterio 2
                        // QR sin temperatura y con el parametro de QR Externo activo
                        else if (objData.Length == 22 && Encoding.ASCII.GetString(objData).Substring(0, 1) == "@" && Encoding.ASCII.GetString(objData).Substring(1, 1) == "Q" && Encoding.ASCII.GetString(objData).Contains("."))
                        {
                            eConfiguration entryConfig = new eConfiguration();
                            string valorGenerado = "";
                            string identificacionUsuario;
                            valorGenerado = Encoding.ASCII.GetString(objData).ToString();

                            valorGenerado = valorGenerado.Replace(valorGenerado.Substring(0, 2), "").Replace("$", "");
                            if (entryConfig.bitGenerarCodigoQRdeingresoparavalidarLocalmente)
                            {
                                identificacionUsuario = AceptarIDQRGeneral(valorGenerado, ipAddress);
                                ValidateEntryByUserId(identificacionUsuario, ipAddress, "justQr", "");
                            }
                            else
                            {

                                bool isExit2 = termList.Find(t => t.ipAddress == ipAddress).servesToOutput;
                                isExit2 = false;
                                GenerateOpeningCommand(false, ipAddress, "", "NO SE ENCUENTRA ACTIVO EL PARAMETRO DE QR EXTERNO.", isExit2);
                                //objData = null;

                            }

                        }
                        else
                        {
                            //jmarin
                            //impresion me mensaje de error en caso de que se pierda comunicacion con la API
                            bool isExit2 = termList.Find(t => t.ipAddress == ipAddress).servesToOutput;
                            isExit2 = false;
                            GenerateOpeningCommand(false, ipAddress, "", "USUARIO NO ENCONTRADO INTENTE NUEVAMENTE.", isExit2);
                            objData = null;

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
                        else
                        {
                            objData = null;
                        }

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
                            tmDownloadEvents.Enabled = true;
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
                            tmDownloadEvents.Enabled = true;
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
                WhiteListBLL whiteListBLL = new WhiteListBLL();
                entryConfig = acsBLL.GetLocalAccessControlSettings();

                //saca la zona que tiene la terminal y el Id de la terminal de base de datos
                var listTerminales = terminalObjectList.Find(tol => tol.IpAddress == ipAddress);

                string zonasTerminal = listTerminales?.Zonas;
                int idTerminal = listTerminales?.IdTerminal ?? 0;
                string res = "";

                var terminal = termList.Find(t => t.ipAddress == ipAddress);
                if (terminal != null)
                {
                    isExit = terminal.servesToOutput;
                }

                log.WriteProcessByTerminal($"Aceptar ID: {id}", ipAddress);

                if (qrType == "DeniedBcsTemp")
                {
                    whiteListBLL.InsertDeniedEventByHightTemperature(id, qrCode, temperature, true, ipAddress);
                    return;
                }
                else if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(qrCode))
                {
                    wlCLS.InsertDeniedEventByNotQr(qrCode, temperature, true, ipAddress);
                    GenerateOpeningCommand(false, ipAddress, id, "USUARIO NO ENCONTRADO O QR VENCIDO.", isExit);
                    return;
                }

                if (id.StartsWith("T-"))
                {
                    originalId = id;
                    swIngresoTarjeta = true;
                    id = originalId.Substring(2);
                    id = GetClientIdByCardId(id, ipAddress) ?? wlCLS.GetEmployeeIdByCardId(id, ipAddress);
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
                    GenerateOpeningCommand(resp, ipAddress, id, "EL USUARIO NO EXISTE EN LA LISTA BLANCA", isExit);
                    return;
                }

                cmList = cmBll.GetLocalCLientMessages();
                string messageToClient = string.Empty, messageClientId = string.Empty, messageType = "0", messageClientOrder = string.Empty;
                int timeMessageClient = 0;

                if (cmList != null && cmList.Count > 0)
                {
                    var item = cmList.Last();
                    if (!string.IsNullOrEmpty(item.messageText))
                    {
                        messageToClient = item.messageText;
                    }

                    timeMessageClient = item.messageDurationTime;
                    messageClientId = item.messageId.ToString();
                    messageType = item.messageType;
                    messageClientOrder = item.messageImgOrder;
                }

                if (id.Trim().Length > 0)
                {
                    if (entryConfig.bitValidarConfiguracionIngresoWeb)
                    {
                        string validationResponse = acsBLL.ValidateEntryData(gymId, Convert.ToInt32(branchId), id);

                        if (validationResponse != "OK")
                        {
                            log.WriteProcessByTerminal($"No se permitirá el ingreso de la persona con identificación: {id}. Debido a que: {validationResponse}", ipAddress);
                            actionService = clsEnum.ServiceActions.ValidationDataError;
                            GenerateOpeningCommand(resp, ipAddress, id, string.Empty, isExit);
                            return;
                        }
                    }

                    if (swIngresoTarjeta)
                    {
                        swWithoutFingerprint = false;
                    }

                    if (!isExit)
                    {
                        if (!blnNoValidarEntradaSalida)
                        {
                            //Validamos si la persona tiene una entrada anterior SIN SALIDA, en caso de ser así se bloquea el ingreso de esta, basado en el parametro "blnAntipassbackEntrada"
                            if (!wlCLS.ValidateEntryWithoutExit(gymId,branchId, id, ipAddress))
                            {
                                actionService = clsEnum.ServiceActions.WaitingClientId;
                                GenerateOpeningCommand(resp, ipAddress, id, string.Empty, isExit);
                                return;
                            }
                        }

                        if (!blnAntipassbackEntrada || (blnAntipassbackEntrada && !resp))
                        {
                            if (string.IsNullOrEmpty(zonasTerminal) || zonasTerminal == "0")
                            {
                                res = wlCLS.ValidateEntryByUserId(id, ipAddress, isId, entryConfig);
                                resp = Convert.ToBoolean(res.Split('^')[0]);
                            }
                            else
                            {
                                WhiteListAPI whiteListAPI = new WhiteListAPI();
                                dWhiteList wlData = new dWhiteList();
                                long idBusqueda = Convert.ToInt64(id);

                                DataTable tblWhiteList = wlData.GetInformacionZonas(idBusqueda.ToString());

                                bool auxExist = false;
                                string zonasPlan = "";

                                if (tblWhiteList.Rows.Count >= 1)
                                {
                                    string typePerson = tblWhiteList.Rows[0]["typePerson"].ToString();
                                    if (typePerson == "Visitante" || typePerson == "Empleado" || typePerson == "Prospecto")
                                    {
                                        auxExist = true;
                                    }
                                    else
                                    {
                                        zonasPlan = whiteListAPI.GetZonasAcceso(Convert.ToInt32(tblWhiteList.Rows[0]["gymId"]), Convert.ToInt32(tblWhiteList.Rows[0]["branchId"]), Convert.ToInt32(tblWhiteList.Rows[0]["planId"]), Convert.ToInt32(tblWhiteList.Rows[0]["reserveId"]));
                                    }

                                    if (auxExist == false)
                                    {
                                        DataTable dtZonas = whiteListBLL.ConsultarZonas(zonasPlan);

                                        if (dtZonas.Rows.Count > 0)
                                        {
                                            auxExist = dtZonas.AsEnumerable().Any(row => row["Terminales"].ToString().Split(',').Any(item => Convert.ToInt32(item) == idTerminal));
                                        }
                                        else
                                        {
                                            GenerateOpeningCommandMensaje("false^SIN ACCESO A LA ZONA DE ESTA TERMINAL", ipAddress, id, false);
                                        }
                                    }

                                    if (auxExist)
                                    {
                                        res = wlCLS.ValidateEntryByUserId(id, ipAddress, isId, entryConfig);
                                        resp = Convert.ToBoolean(res.Split('^')[0]);
                                    }
                                    else
                                    {
                                        GenerateOpeningCommandMensaje("false^SIN ACCESO A LA ZONA DE ESTA TERMINAL", ipAddress, id, false);
                                    }
                                }
                                else
                                {
                                    GenerateOpeningCommandMensaje("false^EL USUARIO NO EXISTE EN LA LISTA BLANCA.", ipAddress, id, false);
                                }
                            }
                        }
                    }
                    else
                    {
                        resp = wlCLS.ValidateExitById(id, ipAddress, isId);
                    }

                    GenerateOpeningCommandMensaje(res, ipAddress, id, isExit);
                }
                else
                {
                    GenerateOpeningCommand(resp, ipAddress, id, "DEBE INGRESAR LA IDENTIFICACIÓN", isExit);
                }
            }
            catch (Exception ex)
            {
                actionService = clsEnum.ServiceActions.WaitingClientId;
                log.WriteProcessByTerminal(ex.Message, ipAddress);
            }
        }

        private void GenerateOpeningCommandMensaje(string resp, string ipAddress, string userId, bool isExit)
        {
            try
            {
                int longLine = 20;
                eEvent eventEntity = new eEvent();
                EventBLL eventBll = new EventBLL();
                dWhiteList wlData = new dWhiteList();
                DataTable dt = new DataTable();
                string msg1 = string.Empty, strName = string.Empty, lastName = string.Empty, plan = string.Empty,
                       lastEntry = string.Empty, command = string.Empty, expiration = string.Empty;
                string[] name;
                string[] respuesta;
                respuesta = resp.Split('^');

                string strCamposMensajeRespuesta = respuesta[1];
                if (strCamposMensajeRespuesta == "")
                {
                    strCamposMensajeRespuesta = "PUEDE INGRESAR";
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    dt = wlData.GetInformacionZonas(userId);

                    if (dt.Rows.Count > 0 && Convert.ToBoolean(respuesta[0]) == true)
                    {
                        eventEntity = new eEvent();
                        eventEntity = eventBll.GetLastEntryByUserId(userId);
                        eventEntity.secondMessage = strCamposMensajeRespuesta;
                        eventEntity.planName = dt.Rows[0]["planName"].ToString();
                        eventEntity.clientName = dt.Rows[0]["name"].ToString();
                        eventEntity.dateLastEntry = "";
                        eventEntity.expirationDate = Convert.ToDateTime(dt.Rows[0]["expirationDate"]).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        eventEntity = new eEvent();
                        eventEntity.secondMessage = strCamposMensajeRespuesta;
                        eventEntity.planName = "";
                        eventEntity.clientName = "";
                        eventEntity.dateLastEntry = "";
                        eventEntity.expirationDate = Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd");
                    }

                    if (!isExit)
                    {
                        msg1 = eventEntity.secondMessage.Length > (longLine * 2) ? eventEntity.secondMessage.Substring(0, (longLine * 2)) : eventEntity.secondMessage.PadRight((longLine * 2), ' ').TrimStart().TrimEnd();
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
                    msg1 = strCamposMensajeRespuesta.Length > (longLine * 2) ? strCamposMensajeRespuesta.Substring(0, (longLine * 2)) : strCamposMensajeRespuesta.PadRight((longLine * 2), ' ').TrimStart().TrimEnd();
                }

                if (Convert.ToBoolean(respuesta[0]))
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
            catch (Exception)
            {
                log.WriteProcess("Ocurrió un error en el proceso de consulta de identificación del cliente por id de huella " + userId + " terminal " + ipAddress);
                log.WriteError(" Terminal " + ipAddress);
            }
        }

        private void ValidateEntryByUserIdBioLite(string id, string ipAddress, bool esAceptada = false, string qrType = null, string qrCode = null, string temperature = null)
        {
            try
            {
                leerJson lg = new leerJson();
                WhiteListAPI wsAPI = new WhiteListAPI();
                List<eWhiteList> eListWhite = new List<eWhiteList>();
                bool resp = false, swWithoutFingerprint = false, isId = false, isExit = false;
                string originalId = string.Empty;
                clsWhiteList wlCLS = new clsWhiteList(qrCode, temperature);
                List<eClientMessages> cmList = new List<eClientMessages>();
                ClientMessagesBLL cmBll = new ClientMessagesBLL();
                //eConfiguration entryConfig = new eConfiguration();
                List<eConfiguration> entryConfig = new List<eConfiguration>();
                AccessControlSettingsBLL acsBLL = new AccessControlSettingsBLL();
                // entryConfig = acsBLL.GetLocalAccessControlSettings();
                //config = acsData.GetConfiguration();
                string json = lg.cargarArchivos(1);
                if (json != "")
                {
                    entryConfig = JsonConvert.DeserializeObject<List<eConfiguration>>(json);
                }
               
                string res = "";

                if (termListBioLite.Find(t => t.ipAddress == ipAddress) != null)
                {
                    isExit = termListBioLite.Find(t => t.ipAddress == ipAddress).servesToOutput;
                }

                log.WriteProcessByTerminal("Aceptar ID: " + id, ipAddress);

                swIngresoTarjeta = false;

                /// inicio quitar y hacer consuta para identificar id o huellas o card con ID
                /// 
                //DataTable dtUsuarioFinal = new DataTable();
                //dtUsuarioFinal = wlCLS.GetClientId(id, ipAddress);
                //id = dtUsuarioFinal.Rows[0]["id"].ToString();
                /////// se cambia consulta para  validar en la base de datos web si el la persona que esta marcando existe en Whitelist Web 
                eListWhite = wsAPI.GetListPersonaConsultar(gymId, branchId , id);

                if (eListWhite[0].tipo == "tarjeta" || eListWhite[0].tipo == "identificacion")
                {
                    swWithoutFingerprint = true;
                    isId = true;
                }
                else
                {
                    swWithoutFingerprint = false;
                    isId = false;
                }


                if (!isId && string.IsNullOrEmpty(id))
                {
                    //GenerateOpeningCommand(resp, ipAddress, id, "EL USUARIO NO EXISTE EN LA LISTA BLANCA.", isExit);
                }
                else
                {
                    /////////////PEndiente 
                    //cmList = cmBll.GetLocalCLientMessages();
                    //string messageToClient = string.Empty, messageClientId = string.Empty, messageType = "0", messageClientOrder = string.Empty;
                    //int timeMessageClient = 0;

                    //if (cmList != null && cmList.Count > 0)
                    //{
                    //    foreach (eClientMessages item in cmList)
                    //    {
                    //        if (!string.IsNullOrEmpty(item.messageText))
                    //        {
                    //            messageToClient = item.messageText;
                    //        }

                    //        timeMessageClient = item.messageDurationTime;
                    //        messageClientId = item.messageId.ToString();
                    //        messageType = item.messageType;
                    //        messageClientOrder = item.messageImgOrder;
                    //    }
                    //}

                    if (id.Trim().Length > 0)
                    {

                        if (!isExit)
                        {
                            if (!blnNoValidarEntradaSalida)
                            {
                                //Validamos si la persona tiene una entrada anterior SIN SALIDA, en caso de ser así se bloquea el ingreso de esta.
                                //Funcionalidad basada en el parámetro "blnAntipassbackEntrada"
                                if (!wlCLS.ValidateEntryWithoutExit(gymId,branchId, id, ipAddress))
                                {
                                    actionService = clsEnum.ServiceActions.WaitingClientId;
                                    resp = false;
                                    // GenerateOpeningCommand(resp, ipAddress, id, string.Empty, isExit);
                                    return;
                                }
                            }

                            if (!blnAntipassbackEntrada || (blnAntipassbackEntrada && !resp))
                            {
                                res = wlCLS.ValidateEntryByUserId(id, ipAddress, esAceptada, entryConfig[0], true);
                                resp = Convert.ToBoolean(res.Split('^')[0]);
                            }
                        }
                        else
                        {
                            //Validamos que el usuario pueda salir sin problema.
                            resp = wlCLS.ValidateExitById(id, ipAddress, isId);
                        }

                        //GenerateOpeningCommand(resp, ipAddress, id, string.Empty, isExit);
                    }
                    else
                    {
                        //GenerateOpeningCommand(resp, ipAddress, id, "DEBE INGRESAR LA IDENTIFICACIÓN", isExit);
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

                if (eventEntity == null)
                {

                }

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
            fingerBll.SaveSignedContract(gymId, Convert.ToInt32(branchId), userId, image, generatePDFContractAndSend);
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

        public void enviarPlanesTerminal(string comando, string ipAdress)
        {
            actionService = clsEnum.ServiceActions.esperandoConfirmacionPlanesVig;
            SendDataAndLogsAux(comando, ipAdress);
        }

        public bool validarTerminalTcam(string ipAddress)
        {
            int tipoTerminal = terminalObjectListAux.Find(tol => tol.IpAddress == ipAddress).terminalTypeId;

            if (tipoTerminal == 1)
                return true;
            else
                return false;
        }

        public string obtenerPlanSeleccionado()
        {
            return planSeleccionado;
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
        private void SendDataAndLogsAux(string msg, string ipAddress)
        {
            if (terminalObjectListAux.Find(tol => tol.IpAddress == ipAddress).connected)
            {
                log.WriteTerminals("<- DATA: " + msg, ipAddress);
                Thread thread = new Thread(() => terminalObjectListAux.Find(tol => tol.IpAddress == ipAddress).SendData(msg));
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

        private void RecordFingerprint(byte[] fingerprintBytes, string ipAddress, string strModoGrabacion)
        {
            try
            {
                leerJson lg = new leerJson();
                EventBLL entryBll = new EventBLL();
                clsWhiteList wlCLS = new clsWhiteList();
                FingerprintBLL fpBll = new FingerprintBLL();
                ReplicatedFingerprintBLL replicatedBll = new ReplicatedFingerprintBLL();
                FingerprintBLL fingerBll = new FingerprintBLL();
                List<eConfiguration> config = new List<eConfiguration>();
                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                FingerPrintAPI acAPI = new FingerPrintAPI();
                ReplicaHuellasAPI rhAPI = new ReplicaHuellasAPI();
                //config = acsBll.GetLocalAccessControlSettings();
                bool resp = false;
                byte[] tmpFingerprint;
                tmpFingerprint = ConvertFingerprint(fingerprintBytes, ipAddress);
                string json = lg.cargarArchivos(1);
                //config = acsData.GetConfiguration();
                if (json != "")
                {
                    config = JsonConvert.DeserializeObject<List<eConfiguration>>(json);
                }


                if (string.IsNullOrEmpty(fingerprintBytes.ToString().Trim()) || tmpFingerprint == null)
                {
                    log.WriteProcessByTerminal("HUELLA VACÍA. NO SE ALMACENA LA HUELLA CLIENTE", ipAddress);
                }
                else
                {
                    eUserRecordFingerprint userRecordToSave = new eUserRecordFingerprint();

                    if (listUserRecord.Count > 0)
                    {
                        listUserRecord.Reverse();

                        userRecordToSave = listUserRecord.Find(ur => ur.ipAddress == ipAddress);
                    }

                    if (userRecordToSave != null)
                    {
                        if (fingerBll.ValidateFingerprintToRecord(gymId, Convert.ToInt32(branchId), userRecordToSave.userId, userRecordToSave.fingerprintId, tmpFingerprint, userRecordToSave.quality, true, strModoGrabacion))
                        {
                            entryBll.Insert("", userRecordToSave.userId, "", 0, 0, "", false, 0, null, null, true, "Huella grabada", "",
                                            "La huella del usuario fue grabada de forma correcta", true, ipAddress, "Enroll", string.Empty);
                            //resp = fpBll.Insert(userRecordToSave.userId, userRecordToSave.ipAddress, userRecordToSave.fingerprintId, tmpFingerprint, userRecordToSave.quality);
                            resp = true;
                        }

                        if (resp)
                        {
                            //wlCLS.UpdateFingerprint(gymId, Convert.ToInt32(branchId), userRecordToSave.userId, userRecordToSave.fingerprintId, tmpFingerprint, ipAddress);
                            log.WriteProcessByTerminal("SE ALMACENA LA HUELLA CLIENTE", ipAddress);

                            //resp = replicatedBll.DeleteFingerprintReplicated(userRecordToSave.fingerprintId, userRecordToSave.userId);
                            resp = rhAPI.GetEliminarReplicaPersona(gymId, userRecordToSave.fingerprintId.ToString(), userRecordToSave.userId);

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
                            replicatedEntity.bitDelete = false;
                            replicatedEntity.bitHuella = true;
                            replicatedEntity.bitTarje_Pin = false;
                            replicatedEntity.cdgimnacio = gymId;
    
                            //resp = replicatedBll.Insert(replicatedEntity);
                            resp = rhAPI.AddReplicaHuellas(replicatedEntity);

                            if (resp)
                            {
                                log.WriteProcessByTerminal("SE INSERTÓ EN LA BASE DE DATOS LA RÉPLICA DE LA HUELLA DEL USUARIO: " + userRecordToSave.userId + ", DE LA TERMINAL.", ipAddress);
                            }
                            else
                            {
                                log.WriteProcessByTerminal("NO SE INSERTÓ EN LA BASE DE DATOS LA RÉPLICA DE LA HUELLA DEL USUARIO: " + userRecordToSave.userId + ", DE LA TERMINAL.", ipAddress);
                            }

                            //ACTUALIZAMOS EL ID DE HUELLA DEL USUARIO EN CASO  DE NO TENER
                            //eUser user = new eUser();
                            //UserBLL userBLL = new UserBLL();
                            //ReplicatedUserBLL replicatedUserBLL = new ReplicatedUserBLL();
                            //user = userBLL.GetUser(userRecordToSave.userId);

                            //if (user != null && !string.IsNullOrEmpty(user.userId))
                            //{
                            //    if (user.fingerprintId <= 0 || (user.fingerprintId > 0 && user.fingerprintId != userRecordToSave.fingerprintId))
                            //    {
                            //        replicatedUserBLL.DeleteUserReplicated(user.fingerprintId, user.userId);
                            //        userBLL.UpdateFingerprintId(userRecordToSave.userId, userRecordToSave.fingerprintId);
                            //    }
                            //}
                            //else
                            //{
                            //    WhiteListBLL whiteListBLL = new WhiteListBLL();
                            //    DataTable dt = whiteListBLL.GetFingerprintsById(userRecordToSave.userId);
                            //    bool withoutFinger = false;
                            //    string userName = string.Empty;

                            //    if (dt != null && dt.Rows.Count > 0)
                            //    {
                            //        withoutFinger = Convert.IsDBNull(dt.Rows[0]["withoutFingerprint"]) ? false : Convert.ToBoolean(dt.Rows[0]["withoutFingerprint"]);
                            //        userName = dt.Rows[0]["name"].ToString();
                            //        userBLL.Insert(userRecordToSave.userId, userName, userRecordToSave.fingerprintId, withoutFinger, true, false);
                            //    }
                            //}

                            //validacion para firmar contrato al enrrolar huella desde tcam y cama 

                            if (config[0].bitFirmarContratoAlEnrolar == true)
                            {
                                //Firma de contratos
                                eSaveSignedContract eEnviarFirmaContractos = new eSaveSignedContract();
                                eEnviarFirmaContractos.gymId = gymId;
                                eEnviarFirmaContractos.branchId = Convert.ToInt32(branchId);
                                eEnviarFirmaContractos.userId = userRecordToSave.userId;
                                eEnviarFirmaContractos.fingerprintImage = tmpFingerprint;

                                acAPI.SaveSignedContract(eEnviarFirmaContractos);
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
                ActionAPI acttionAPI = new ActionAPI();
                listExecutionTerminal = new List<eTerminalAction>();

                foreach (clsWinSockCliente item in terminalObjectList)
                {
                    //OD 1689 - Getulio Vargas - 2020/01/28
                    //Se realiza validación para el proceso de ejecución de acciones pendientes, para que sólo realice esto para las terminales de tipo 1 (TCAM7000)
                    eActionToExecute terminalAction = new eActionToExecute();
                    terminalAction.ipAddress = item.IpAddress;
                    //terminalAction.actionLists = actionBll.GetPendingActionsByTerminal(item.IpAddress);
                    // se cambia consulta de acciones pendiente a la base de datos web 
                    terminalAction.actionLists = acttionAPI.GetAction(gymId,branchId ,item.IpAddress);
                    eTerminalAction ta = new eTerminalAction();
                    ta.ipAddress = item.IpAddress;
                    ta.actionToMake = terminalAction.actionLists.Count;
                    ta.actionfinished = 0;
                    listExecutionTerminal.Add(ta);
                    Task.Run(() => ProcessActionTerminal(terminalAction));
                }


                if (termListBioLite.Count > 0)
                {
                   
                    foreach (var item in terminalObjectListBioLite)
                    {
                        //DataTable dtt = new DataTable();
                        ActionBLL acionHuellaBioLite = new ActionBLL();
                        eActionToExecute terminalAction = new eActionToExecute();
                        bool resp = false;

                        try
                        {
                            //dtt = acionHuellaBioLite.ConsultarAccionPendiente(item.ip);
                            terminalAction.actionLists = acttionAPI.GetAction(gymId, branchId, item.ip);
                        }
                        catch (Exception)
                        {
                        }

                        if (terminalAction.actionLists.Count > 0)
                        {
                            if (hacerping(item.ip))
                            {
                                for (int i = 0; i < terminalAction.actionLists.Count; i++)
                                {
                                    //Obtenemos el byte de la huella
                                    byte[] byteHuela = item.conexionBioEntry.capturarHuellaTerminalBiolite(item.ip, item.snTerminal);
                                    int delayTerminate = 0;
                                    Thread.Sleep(delayTerminate);
                                    if (byteHuela != null)
                                    {
                                        //Actualizamos el estado
                                       // acionHuellaBioLite.ActualizarHuellasPersonas(byteHuela, Convert.ToInt32(dtt.Rows[i]["intID"]));
                                        resp = acttionAPI.GetActionUpdate(terminalAction.actionLists[0]);

                                        //Obtener el fingerprintid
                                        FingerPrintAPI acAPI = new FingerPrintAPI();
                                        eResponse response = new eResponse();
                                        response = acAPI.ValidatePerson(gymId, Convert.ToInt32(branchId), terminalAction.actionLists[0].strAction);
                                        int fingerPrintId = Convert.ToInt32(response.messageTwo);

                                        //Mandamos actualizar en web el byte de la huella
                                        eFingerprint eEnviarfingerprint = new eFingerprint();
                                        eEnviarfingerprint.id = fingerPrintId;
                                        eEnviarfingerprint.fingerPrint = byteHuela;
                                        eEnviarfingerprint.gymId = gymId;
                                        eEnviarfingerprint.finger = 1;
                                        eEnviarfingerprint.quality = 100;
                                        eEnviarfingerprint.branchId = Convert.ToInt32(branchId);
                                        eEnviarfingerprint.personId = terminalAction.actionLists[0].strAction;
                                        eEnviarfingerprint.modoGrabacion = "suprema";

                                        ///Respuesta de la api si la persona tiene las validaciones en web
                                        response = acAPI.ValidateFingerprintToRecord(eEnviarfingerprint);

                                        if (response.message == "ExistFingerprint")
                                        {
                                            log.WriteTerminals("YA EXISTE UNA HUELLA REGISTRADA PARA LA PERSONA" + fingerPrintId ,item.ip);
                                        }

                                        /// se guardara la huella en base de datos web 
                                        ////Actualizamos el fingerprintId en fingerprint
                                        //FingerprintBLL BLLfingerprint = new FingerprintBLL();
                                        //BLLfingerprint.Insert(terminalAction.actionLists[0].strAction, item.ip, fingerPrintId, byteHuela, 100);

                                        ////Actualizamos finerPrint de whiteList
                                        //WhiteListBLL BLLwhiteList = new WhiteListBLL();
                                        //BLLwhiteList.UpdateFingerprintID(byteHuela, fingerPrintId, terminalAction.actionLists[0].strAction);

                                        ////Eliminar Registro detalle Accion
                                        //acionHuellaBioLite.EliminarDetalleAccion(Convert.ToInt32(dtt.Rows[i]["intID"]));

                                        //Validamos firma de contratos al enrolar
                                        //eConfiguration config = new eConfiguration();
                                        //AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                                        //config = acsBll.GetLocalAccessControlSettings();

                                        string json = "";
                                        List<eConfiguration> config = new List<eConfiguration>();
                                        if (json != "")
                                        {
                                            config = JsonConvert.DeserializeObject<List<eConfiguration>>(json);
                                        }

                                        if (response.state == false)
                                        {
                                            if (config[0].bitFirmarContratoAlEnrolar == true)
                                            {
                                                //Firma de contratos
                                                eSaveSignedContract eEnviarFirmaContractos = new eSaveSignedContract();
                                                eEnviarFirmaContractos.gymId = gymId;
                                                eEnviarFirmaContractos.branchId = Convert.ToInt32(branchId);
                                                eEnviarFirmaContractos.userId = terminalAction.actionLists[0].strAction;
                                                eEnviarFirmaContractos.fingerprintImage = byteHuela;

                                                acAPI.SaveSignedContract(eEnviarFirmaContractos);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    timerGetPendingActions.Enabled = true;
                    timerGetPendingActions.Start();
                }

                if (terminalObjectList.Count == 0 || termListBioLite.Count == 0)
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
                ActionAPI acttionAPI = new ActionAPI();
                ActionBLL actionBll = new ActionBLL();
                bool resp = false;

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
                                //actionBll.Update(item);
                                resp= acttionAPI.GetActionUpdate(item);
                                System.Threading.Thread.Sleep(timeoutOpening);
                            }

                            break;
                        case "Restart":
                            //actionService = clsEnum.ServiceActions.Restart;
                            if (RestartTerminal(item.ipAddress, "7"))
                            {
                                item.stateAction = true;
                                //actionBll.Update(item);
                                resp = acttionAPI.GetActionUpdate(item);
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
                                //actionBll.Update(item);
                                resp = acttionAPI.GetActionUpdate(item);
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
                                //actionBll.Update(item);
                                resp = acttionAPI.GetActionUpdate(item);
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

                //eTerminal objeTerminal = new eTerminal();
                //TerminalBLL terminalBLL = new TerminalBLL();
                if (!signContracteToEnroll)
                {
                    command = "@E";
                }
                else
                {
                    //objeTerminal = terminalBLL.GetTerminalByIp(ipAddress);
                    termList = termList.Where(p => p.ipAddress == ipAddress).ToList();
                    //if (objeTerminal.terminalTypeId == 6)
                    if (termList[0].terminalTypeId == 6)
                    {
                        command = "@E";
                    }
                    else
                    {
                        command = "@F";
                    }
                    //Inicio de comando para grabación de huella con firma de contrato.

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
                string zonas = terminalObjectList.Find(tol => tol.IpAddress == ipAddress).Zonas;
                int idTerminal = terminalObjectList.Find(tol => tol.IpAddress == ipAddress).IdTerminal;
                clsWinSockCliente restartSocket = new clsWinSockCliente(ipAddress, 9999, withWhitelist, false, 0, zonas, idTerminal);
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
                                    winSocketClient = new clsWinSockCliente(item.ipAddress, Convert.ToInt32(item.port), item.withWhiteList, false, item.terminalTypeId, item.Zonas, item.terminalId);

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
                                    winSocketClient = new clsWinSockCliente(item.ipAddress, Convert.ToInt32(item.port), item.withWhiteList, false, item.terminalTypeId, item.Zonas, item.terminalId);

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
                try
                {
                    if (termListBioLite.Count > 0)
                    {
                        foreach (var item in termListBioLite)
                        {

                            if (!terminalObjectListBioLite.Exists(tol => tol.ip == item.ipAddress))
                            {
                                if (!string.IsNullOrEmpty(item.ipAddress))
                                {
                                    if (hacerping(item.ipAddress))
                                    {
                                        clsComBioEntryPlus2 libcomBioPlus2 = new clsComBioEntryPlus2();
                                        bool bresultado = false;
                                        bresultado = libcomBioPlus2.conectarTerminal(item.ipAddress, 51211);

                                        if (bresultado)
                                        {
                                            terminalObjectListBioLite.Add(new eListBiolite { ip = item.ipAddress, codigo = item.terminalId, zonas = item.Zonas, idTerminal = item.terminalId, snTerminal = item.snTerminal, conexionBioEntry = libcomBioPlus2 });

                                            int delayTerminate = 0;
                                            Thread.Sleep(delayTerminate);
                                        }
                                        else
                                        {
                                            log.WriteProcess("No es posible realizar la conexión de la terminal " + item.ipAddress + ", debido a que no responde a conectividad.");
                                        }

                                    }

                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.ipAddress))
                                {
                                    if (hacerping(item.ipAddress))
                                    {
                                        DataTable dtvFila = new DataTable();
                                        dtvFila.Columns.Add(new DataColumn("strIpTerminal", System.Type.GetType("System.String")));
                                        DataRow drvFila;
                                        drvFila = dtvFila.NewRow();
                                        drvFila["strIpTerminal"] = item.ipAddress;
                                        dtvFila.Rows.Add(drvFila);

                                        clsComBioEntryPlus2 libcomBioPlus2 = terminalObjectListBioLite.Find(tol => tol.ip == item.ipAddress).conexionBioEntry;

                                        if (libcomBioPlus2.configurarFechaHora(drvFila) == respuestaDescarga.fechaHoraNoPudoSerActualizada)
                                        {
                                            clsComBioEntryPlus2 libcomBioPlus2Nuevo = new clsComBioEntryPlus2();
                                            bool bresultado = false;
                                            bresultado = libcomBioPlus2Nuevo.conectarTerminal(item.ipAddress, 51211);

                                            if (bresultado)
                                            {
                                                terminalObjectListBioLite.Find(tol => tol.ip == item.ipAddress).conexionBioEntry = libcomBioPlus2Nuevo;
                                            }
                                            else
                                            {
                                                log.WriteProcess("No es posible realizar la conexión de la terminal " + item.ipAddress + ", debido a que no responde a conectividad.");
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    log.WriteProcess("No es posible realizar la reconexión de la terminal , debido a que no responde a conectividad.");
                }


            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message);
                actionService = clsEnum.ServiceActions.WaitingClientId;
            }
            finally
            {
                timerTerminalConnections.Enabled = true;
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

        public string AceptarIDQRGeneral(string valorGenerado, string ipAddress)
        {
            try
            {

                log.WriteProcess("Aceptar ID: " + valorGenerado);

                string codeQr = "";

                //colocar if validando que el paraemtro de lector QR externo este activo parametro traido de la api

                codeQr = valorGenerado;
                log.WriteProcess("valor: " + valorGenerado + " logitud: " + valorGenerado.Length.ToString());

                string identifi;
                string DD;
                string MM;
                string HH;
                string min;
                string[] QRSplit;
                QRSplit = valorGenerado.Split('.');

                if (QRSplit.Length > 2)
                {

                    DD = QRSplit[1].Substring(0, 2);
                    HH = QRSplit[1].Substring(2, 2);
                    MM = QRSplit[2].Substring(0, 2);
                    min = QRSplit[2].Substring(2, 2);
                    identifi = QRSplit[1].Substring(4, QRSplit[1].Length - 4);

                    //log.WriteProcess("Se genera DD : " + DD);
                    //log.WriteProcess("Se genera MM : " + MM);
                    //log.WriteProcess("Se genera HH : " + HH);
                    //log.WriteProcess("Se genera min : " + min);
                    //log.WriteProcess("Se genero identificacion : " + identifi);

                    DateTime FechayHoraActual = DateTime.Now;

                    if (FechayHoraActual.Date.Day == Convert.ToInt32(DD) & FechayHoraActual.Date.Month == Convert.ToInt32(MM) & (FechayHoraActual.Hour == Convert.ToInt32(HH) | FechayHoraActual.AddSeconds(30).Hour == Convert.ToInt32(HH)) & (FechayHoraActual.Minute == Convert.ToInt32(min) | FechayHoraActual.AddSeconds(30).Minute == Convert.ToInt32(min) | FechayHoraActual.AddSeconds(-30).Minute == Convert.ToInt32(min)))
                    {
                        identifi = InvertirManualmente(identifi);
                        log.WriteProcess("Se Valida identificacion : " + identifi);
                    }

                    else
                    {
                        log.WriteProcess("La fecha:" + DD + "/" + MM + " " + HH + min + " del codigo generado no es valida frente a la fecha actal : " + FechayHoraActual.ToString());
                        // mensaje = ("@D" + "FH CODIGO INVALIDO" + Space(116) + "$")

                        bool isExit2 = termList.Find(t => t.ipAddress == ipAddress).servesToOutput;
                        isExit2 = false;
                        GenerateOpeningCommand(false, ipAddress, "", "LA FECHA DEL QR NO ES VALIDA", isExit2);
                        objData = null;

                    }


                    if (identifi.Length > 3)
                    {
                        valorGenerado = identifi;
                        return valorGenerado;
                    }
                    else
                    {
                        bool isExit2 = termList.Find(t => t.ipAddress == ipAddress).servesToOutput;
                        isExit2 = false;
                        GenerateOpeningCommand(false, ipAddress, "", "IDENTIFICACION NO VALIDA.", isExit2);
                        objData = null;

                        return "";
                    }



                }
                return "";
            }
            catch (Exception)
            {

                throw;
            }

        }


        public static string InvertirManualmente(string cadena)
        {
            string cadenaInvertida = "";

            foreach (char letra in cadena)
                cadenaInvertida = letra + cadenaInvertida;

            return cadenaInvertida;
        }

        private bool hacerping(string direccionIp)
        {
            //return My.Computer.Network.Ping(direccionIp);
            Microsoft.VisualBasic.Devices.Network net = new Network();
            bool success = net.Ping(direccionIp);

            return success;
        }
    }
}
