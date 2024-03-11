using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;

namespace Sibo.WhiteList.Service
{
    partial class Service : ServiceBase
    {
        Timer timerConfiguration, timerWhiteList, timerEntries, timerMessages, timerTerminal;
        int timeConfiguration = 0, timeWhiteList = 0, timeEntries = 0, timeMessages = 0, timeTerminal = 0, oneMinute = 60000;
        bool getWL = false, getConf = false, updateEntries = false, getMsg = false, fingerprint = false, getTerminal = false;
        ServiceLog log = new ServiceLog();
        clsTerminal ter = new clsTerminal();

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                DebugMode();
                log.WriteProcess("INICIA EJECUCIÓN DEL SERVICIO.");

                //CONSULTAMOS LAS CONFIGURACIONES INICIALES PARA EL SERVICIO
                InitialExecutions();

                if (timeTerminal == 0)
                {
                    //Timer para consultar las terminales configuradas
                    timerTerminal = new Timer(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["timeTerminal"].ToString()));
                    timerTerminal.Elapsed += new ElapsedEventHandler(timerTerminal_Elapsed);
                    timerTerminal.AutoReset = true;
                    timerTerminal.Start();
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        private void InitialExecutions()
        {
            try
            {
                log.WriteProcess("Inicia el proceso de consulta y actualización o inserción de configuración.");
                Principal("Configuration");
                UpdateTimers();
                log.WriteProcess("Finaliza proceso de consulta y actualización o inserción de configuración.");

                log.WriteProcess("Inicia consulta de terminales.");
                Principal("Terminal");
                ter.OpenSockets();
                log.WriteProcess("Finaliza la consulta de terminales.");
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        [Conditional("DEBUG_SERVICE")]
        private static void DebugMode()
        {
            Debugger.Break();
        }

        protected override void OnStop()
        {
            try
            {
                FinalizeTimerConfiguration();
                FinalizeTimerEntries();
                FinalizeTimerMessages();
                FinalizeTimerWhiteList();
                FinalizeTimerTerminal();
                log.WriteProcess("FINALIZA LA EJECUCIÓN DEL SERVICIO.");
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        public void timerConfiguration_Elapsed(object sender, EventArgs e)
        {
            try
            {
                bool respConfiguration = false;

                if (!getConf)
                {
                    log.WriteProcess("Inicia el proceso de consulta y actualización o inserción de configuración.");
                    getConf = true;
                    //Detiene el Timer
                    timerConfiguration.Enabled = false;
                    respConfiguration = Principal("Configuration");
                    UpdateTimers();
                    //habilita el Timer nuevamente.
                    timerConfiguration.Enabled = true;
                    getConf = false;
                    log.WriteProcess("Finaliza proceso de consulta y actualización o inserción de configuración.");
                }
            }
            catch (Exception ex)
            {
                log.WriteError("Error al consultar y/o procesar la configuración - " + ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        public void timerWhiteList_Elapsed(object sender, EventArgs e)
        {
            try
            {
                if (!getWL)
                {
                    log.WriteProcess("Inicia el proceso de consulta y actualización o inserción de lista blanca.");
                    getWL = true;
                    //Detiene el Timer
                    timerWhiteList.Enabled = false;
                    Principal("WhiteList");
                    //habilita el Timer nuevamente.
                    timerWhiteList.Enabled = true;
                    getWL = false;
                    log.WriteProcess("Finaliza proceso de consulta y actualización o inserción de lista blanca.");
                }
            }
            catch (Exception ex)
            {
                log.WriteError("Error al consultar y/o procesar la lista blanca - " + ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        public void timerEntries_Elapsed(object sender, EventArgs e)
        {
            try
            {
                if (!updateEntries)
                {
                    log.WriteProcess("Inicia el proceso de actualización de las entradas.");
                    updateEntries = true;
                    //Detiene el Timer
                    timerEntries.Stop();
                    Principal("Entry");
                    //habilita el Timer nuevamente.
                    timerEntries.Start();
                    updateEntries = false;
                    log.WriteProcess("Inicia el proceso de actualización de las entradas.");
                }
            }
            catch (Exception ex)
            {
                log.WriteError("Error en el proceso para actualizar las entradas - " + ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        public void timerMessages_Elapsed(object sender, EventArgs e)
        {
            try
            {
                if (!getMsg)
                {
                    log.WriteProcess("Inicia el proceso de consulta y actualización o inserción de los mensajes al cliente.");
                    getMsg = true;
                    //Detiene el Timer
                    timerMessages.Enabled = false;
                    Principal("ClientMessages");
                    //habilita el Timer nuevamente.
                    timerMessages.Enabled = true;
                    getMsg = false;
                    log.WriteProcess("Finaliza proceso de consulta y actualización o inserción de los mensajes al cliente.");
                }
            }
            catch (Exception ex)
            {
                log.WriteError("Error al consultar y/o procesar los mensajes al cliente - " + ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        //public void timerInsertUpdateFingerprints_Elapsed(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (!fingerprint)
        //        {
        //            log.WriteProcess("Inicia procesamiento de huellas.");
        //            fingerprint = true;
        //            //Detiene el Timer
        //            timerInsertUpdateFingerprints.Stop();
        //            timerInsertUpdateFingerprints.Enabled = false;
        //            Principal("FingerPrint");
        //            //habilita el Timer nuevamente.
        //            timerInsertUpdateFingerprints.Enabled = true;
        //            timerInsertUpdateFingerprints.Start();
        //            fingerprint = false;
        //            log.WriteProcess("Finaliza procesamiento de huellas.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.WriteError("Error en el procesamiento de huellas - " + ex.Message + " " + ex.StackTrace.ToString());
        //    }
        //}

        public void timerTerminal_Elapsed(object sender, EventArgs e)
        {
            try
            {
                if (!getTerminal)
                {
                    log.WriteProcess("Inicia consulta de terminales.");
                    getTerminal = true;
                    //Detiene el Timer
                    timerTerminal.Stop();
                    timerTerminal.Enabled = false;
                    Principal("Terminal");
                    ter.OpenSockets();
                    //habilita el Timer nuevamente.
                    timerTerminal.Enabled = true;
                    timerTerminal.Start();
                    getTerminal = false;
                    log.WriteProcess("Finaliza la consulta de terminales.");
                }
            }
            catch (Exception ex)
            {
                log.WriteError("Error en el procesamiento de huellas - " + ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Método encargado de controlar qué proceso se ejecuta en el servicio.
        /// Getulio Vargas - 2018-04-10 - OD 1307
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool Principal(string action)
        {
            try
            {
                string strGymId = string.Empty, strBranchId = string.Empty;
                int gymId = 0, branchId = 0;
                bool resp = false;
                strGymId = System.Configuration.ConfigurationManager.AppSettings["gymId"].ToString();
                strBranchId = System.Configuration.ConfigurationManager.AppSettings["branchId"].ToString();

                if (!string.IsNullOrEmpty(strGymId) && !string.IsNullOrEmpty(strBranchId))
                {
                    gymId = Convert.ToInt32(strGymId);
                    branchId = Convert.ToInt32(strBranchId);
                }
                else
                {
                    throw new Exception("No se ha configurado de forma correcta el código del gimnasio y/o la sucursal.");
                }

                switch (action)
                {
                    case "ClientMessages":
                        ClientMessagesBLL msgBll = new ClientMessagesBLL();
                        resp = msgBll.GetClientsMessages(gymId, branchId);
                        break;
                    case "WhiteList":
                        clsWhiteList whiteList = new clsWhiteList();
                        resp = whiteList.GetWhiteList(gymId, branchId);
                        break;
                    case "Configuration":
                        AccessControlSettingsBLL acsBLL = new AccessControlSettingsBLL();
                        HolidayBLL hdBll = new HolidayBLL();
                        resp = acsBLL.GetAccessControlSettings(gymId, branchId);
                        hdBll.GetHolidays(gymId);
                        break;
                    case "Entry":
                        EventBLL entryBLL = new EventBLL();
                        resp = entryBLL.AddEntry(gymId, branchId);
                        break;
                    case "FingerPrint":
                        //clsFingerprint fingerprintCLS = new clsFingerprint();
                        //resp = fingerprintCLS.GetFingerprintsToUpdate(gymId, branchId);
                        break;
                    case "Terminal":
                        clsTerminal terminal = new clsTerminal();
                        resp = terminal.GetTerminal(gymId, branchId);
                        break;
                    default:
                        break;
                }

                return resp;

                //ValidatePerson(gymId, branchId, "15017050");
                //DownloadFingerprint(gymId, 3, "1128437660");
                //AddFingerprint(gymId, branchId, "1128437660");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Finaliza Timer de configuración.
        /// Getulio Vargas - 2018-04-10 - OD 1307
        /// </summary>
        private void FinalizeTimerConfiguration()
        {
            try
            {
                if (timerConfiguration != null)
                {
                    timerConfiguration.Stop();
                    timerConfiguration.Enabled = false;
                    timerConfiguration.Dispose();
                    timerConfiguration = null;
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Finaliza Timer de lista blanca.
        /// Getulio Vargas - 2018-04-10 - OD 1307
        /// </summary>
        private void FinalizeTimerWhiteList()
        {
            try
            {
                if (timerWhiteList != null)
                {
                    timerWhiteList.Stop();
                    timerWhiteList.Enabled = false;
                    timerWhiteList.Dispose();
                    timerWhiteList = null;
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Finaliza Timer de entradas.
        /// Getulio Vargas - 2018-04-10 - OD 1307
        /// </summary>
        private void FinalizeTimerEntries()
        {
            try
            {
                if (timerEntries != null)
                {
                    timerEntries.Stop();
                    timerEntries.Enabled = false;
                    timerEntries.Dispose();
                    timerEntries = null;
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Finaliza Timer de mensajes.
        /// Getulio Vargas - 2018-04-10 - OD 1307
        /// </summary>
        private void FinalizeTimerMessages()
        {
            try
            {
                if (timerMessages != null)
                {
                    timerMessages.Stop();
                    timerMessages.Enabled = false;
                    timerMessages.Dispose();
                    timerMessages = null;
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Finaliza timer de terminales.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        private void FinalizeTimerTerminal()
        {
            try
            {
                if (timerTerminal != null)
                {
                    timerTerminal.Stop();
                    timerTerminal.Enabled = false;
                    timerTerminal.Dispose();
                    timerTerminal = null;
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Método encargado de actualizar los tiempos de cada timer.
        /// Getulio Vargas - 2018-04-10 - OD 1307
        /// </summary>
        private void UpdateTimers()
        {
            try
            {
                AccessControlSettingsBLL acsBLL = new AccessControlSettingsBLL();
                List<int> timerList = new List<int>();
                int aux = 0;
                log.WriteProcess("Inicia el proceso para actualizar timers.");
                timerList = acsBLL.GetTimers();

                if (timerList != null && timerList.Count > 0)
                {
                    if (timerList[0] > 0 && (timerList[0] * oneMinute) != timeConfiguration)
                    {
                        aux = timeConfiguration;
                        //timeConfiguration = 72000000;
                        timeConfiguration = timerList[0] * oneMinute;
                        FinalizeTimerConfiguration();
                        InitializeTimerConfiguration(timeConfiguration);
                        log.WriteProcess("Se actualiza timer de configuración, pasa de " + aux.ToString() + " ms a " + timeConfiguration.ToString() + " ms.");
                    }
                    
                    if (timerList[1] > 0 && (timerList[1] * oneMinute) != timeWhiteList)
                    {
                        aux = timeWhiteList;
                        timeWhiteList = timerList[1] * oneMinute;
                        //timeWhiteList = 72000000;
                        FinalizeTimerWhiteList();
                        InitializeTimerWhiteList(timeWhiteList);
                        log.WriteProcess("Se actualiza timer de lista blanca, pasa de " + aux.ToString() + " ms a " + timeWhiteList.ToString() + " ms.");
                    }

                    if (timerList[2] > 0 && (timerList[2] * oneMinute) != timeEntries)
                    {
                        aux = timeEntries;
                        //timeEntries = 1000;
                        timeEntries = timerList[2] * oneMinute;
                        FinalizeTimerEntries();
                        InitializeTimerEntries(timeEntries);
                        log.WriteProcess("Se actualiza timer de ingresos, pasa de " + aux.ToString() + " ms a " + timeEntries.ToString() + " ms.");
                    }

                    if (timerList[3] > 0 && (timerList[3] * oneMinute) != timeMessages)
                    {
                        aux = timeMessages;
                        //timeMessages = 72000000;
                        timeMessages = timerList[3] * oneMinute;
                        FinalizeTimerMessages();
                        InitializeTimerMessages(timeMessages);
                        log.WriteProcess("Se actualiza timer de mensajes al cliente, pasa de " + aux.ToString() + " ms a " + timeMessages.ToString() + " ms.");
                    }

                    if (timerList[4] > 0 && (timerList[4] * oneMinute) != timeTerminal)
                    {
                        aux = timeTerminal;
                        //timeTerminal = 72000000;
                        timeTerminal = timerList[4] * oneMinute;
                        FinalizeTimerTerminal();
                        InitializeTimerTerminal(timeTerminal);
                        log.WriteProcess("Se actualiza timer de terminales, pasa de " + aux.ToString() + " ms a " + timeTerminal.ToString() + " ms.");
                    }
                }
                //System.Threading.Thread.Sleep(72000000);
                log.WriteProcess("Finaliza proceso para actualizar timers.");
            }
            catch (Exception ex)
            {
                log.WriteError("Error al actualizar los timers. - " + ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Inicializa timer de configuración.
        /// Getulio Vargas - 2018-04-10 - OD 1307
        /// </summary>
        /// <param name="timeConfiguration"></param>
        private void InitializeTimerConfiguration(int timeConfiguration)
        {
            try
            {
                if (timeConfiguration > 0)
                {
                    timerConfiguration = new Timer(timeConfiguration);
                }
                else
                {
                    timerConfiguration = new Timer(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["time"].ToString()));
                }

                timerConfiguration.Elapsed += new ElapsedEventHandler(timerConfiguration_Elapsed);
                timerConfiguration.AutoReset = true;
                timerConfiguration.Start();
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Inicializa timer de lista blanca.
        /// Getulio Vargas - 2018-04-10 - OD 1307
        /// </summary>
        /// <param name="timeWhiteList"></param>
        private void InitializeTimerWhiteList(int timeWhiteList)
        {
            try
            {
                if (timeWhiteList > 0)
                {
                    timerWhiteList = new Timer(timeWhiteList);
                    timerWhiteList.Elapsed += new ElapsedEventHandler(timerWhiteList_Elapsed);
                    timerWhiteList.AutoReset = true;
                    timerWhiteList.Start();
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Inicializa timer de entradas.
        /// Getulio Vargas - 2018-04-10 - OD 1307
        /// </summary>
        /// <param name="timeEntries"></param>
        private void InitializeTimerEntries(int timeEntries)
        {
            try
            {
                if (timeEntries > 0)
                {
                    timerEntries = new Timer(timeEntries);
                    timerEntries.Elapsed += new ElapsedEventHandler(timerEntries_Elapsed);
                    timerEntries.AutoReset = true;
                    timerEntries.Start();
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Inicializa timer de mensajes.
        /// Getulio Vargas - 2018-04-10 - OD 1307
        /// </summary>
        /// <param name="timeMessages"></param>
        private void InitializeTimerMessages(int timeMessages)
        {
            try
            {
                if (timeMessages > 0)
                {
                    timerMessages = new Timer(timeMessages);
                    timerMessages.Elapsed += new ElapsedEventHandler(timerMessages_Elapsed);
                    timerMessages.AutoReset = true;
                    timerMessages.Start();
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Inicializa timer de terminales.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <param name="timeTerminal"></param>
        private void InitializeTimerTerminal(int timeTerminal)
        {
            try
            {
                if (timeTerminal > 0)
                {
                    timerTerminal = new Timer(timeTerminal);
                    timerTerminal.Elapsed += new ElapsedEventHandler(timerTerminal_Elapsed);
                    timerTerminal.AutoReset = true;
                    timerTerminal.Start();
                }
            }
            catch (Exception ex)
            {
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
            }
        }
    }
}
