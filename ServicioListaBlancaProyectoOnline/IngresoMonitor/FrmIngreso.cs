using libComunicacionBioEntry;
using Sibo.WhiteList.IngresoMonitor.Classes;
using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.BLL.Helpers;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using libComunicacionBioEntry;
using Sibo.WhiteList.Service.BLL.Log;
using System.Configuration;
using System.Timers;
using Newtonsoft.Json;
using System.Linq;
using Sibo.WhiteList.Service.DAL.API;

namespace Sibo.WhiteList.IngresoMonitor
{
    public partial class FrmIngresos : Form
    {
        leerJson lg = new leerJson();
        ServiceLog log = new ServiceLog();
        ActionAPI action = new ActionAPI();
        string ipAddress = string.Empty, userName = string.Empty, planName = string.Empty, expirationDate = string.Empty, entryType = string.Empty,
               date = string.Empty, firstMessage = string.Empty, secondMessage = string.Empty, strImageFingerprint = string.Empty, userId = string.Empty , snTerminal = string.Empty;
        bool blnPermitirIngresoAdicional = true, firstTime = true, blnTCAM7000 = false, isMonitor = false, blnSirveParaSalida = false,
             blnSirveParaEntradaySalida = false, blnAccesoDiscapacitados = false, blnModoTarjeta = false;
        int timeUpdateEntries = 6000, gymId = 0, branchId = 0, quantityAppAccessControlSimultaneous = 0, terminalTypeId = 0, intTiempoParaLimpiarPantalla = 5, intTipoTerminal = 0;
        System.Timers.Timer timerUpdateEntries, timerClearScreen;
        public string estadoLoginIngresoAdicional = "";
        System.Timers.Timer timerConfiguration;
        int timeConfiguration = 0;
        bool getConf = false;
        public List<eTerminal> ListaterminalFiltro = new List<eTerminal>();
        public List<eTerminal> Listaterminal= new List<eTerminal>();
        string json = "";
        private void itemAbrirPuerta_Click(object sender, EventArgs e)
        {
            eEvent eventos = new eEvent();
            EventBLL eventosBLL = new EventBLL();
            TerminalBLL terminalBll = new TerminalBLL();
            DataTable dttTerminal = new DataTable();
            clsAction act = new clsAction();
            DateTime fechaActual = new DateTime();
            int resp = 0;

            bool bitLoginIngresoAdicional = false;
            if (estadoLoginIngresoAdicional == "1")
            {
                bitLoginIngresoAdicional = true;
            }
            else
            {
                bitLoginIngresoAdicional = false;
            }

            if (bitLoginIngresoAdicional == false)
            {
                FrmAbriPuerta abrirPuerta = new FrmAbriPuerta();
                abrirPuerta.Text = "Ingreso Adicional";
                string motivo = string.Empty;
                string terminal = string.Empty;

                if (abrirPuerta.ShowDialog() == DialogResult.OK)
                {
                    motivo = abrirPuerta.motivos;
                    terminal = abrirPuerta.terminal;
                    fechaActual = DateTime.Now.Date;

                    string[] splitTerminal = terminal.Split('.');
                    dttTerminal = terminalBll.consultarTerminalID(Convert.ToInt16(splitTerminal[0]));

                    if (!string.IsNullOrEmpty(motivo))
                    {
                        eventosBLL.Insert("", "0", "Ingreso adicional", 0, 0, "", false, 0, fechaActual, fechaActual, false, "Ingreso adicional", "", "Ingreso adicional : " + motivo, true, dttTerminal.Rows[0]["ipAddress"].ToString(), "Entry", "", null, null);

                        //creacion de comando de accion para mandar pulso
                        resp = act.Insert(dttTerminal.Rows[0]["ipAddress"].ToString(), clsEnum.ServiceActions.AutorizeEntryClientById);

                        if (resp <= 0)
                        {
                            clsShowMessage.Show("El usuario " + motivo + " puede ingresar pero no fue posible enviar el comando a la terminal.\n Por favor vuelva a intentarlo.", clsEnum.MessageType.Informa);
                        }
                        else
                        {
                            clsActionParameters aParameters = new clsActionParameters();
                            List<eActionParameters> aParametersList = new List<eActionParameters>();
                            eActionParameters aParamEntity = new eActionParameters();
                            aParamEntity.actionId = resp;
                            aParamEntity.parameterName = clsEnum.ActionParameters.UserId.ToString();
                            aParamEntity.parameterValue = motivo;
                            aParametersList.Add(aParamEntity);
                            aParameters.Insert(aParametersList);
                        }

                        ShowData();
                        return;

                    }

                }

            }
            else if (bitLoginIngresoAdicional == true)

            {

                FrmAbriPuertaLoginIngresoAdicional abriPuertaLoginIngresoAdicional = new FrmAbriPuertaLoginIngresoAdicional();

                abriPuertaLoginIngresoAdicional.Text = "Ingreso Adicional";

                string motivos = string.Empty;
                string terminal = string.Empty;
                string identificacion = string.Empty;
                string nombre = string.Empty;
                string empresa = string.Empty;
                string usuario = string.Empty;
                string contrasena = string.Empty;


                if (abriPuertaLoginIngresoAdicional.ShowDialog() == DialogResult.OK)
                {
                    motivos = abriPuertaLoginIngresoAdicional.motivos;
                    identificacion = abriPuertaLoginIngresoAdicional.identificacion;
                    terminal = abriPuertaLoginIngresoAdicional.terminal;
                    nombre = abriPuertaLoginIngresoAdicional.nombre;
                    empresa = abriPuertaLoginIngresoAdicional.empresa;
                    usuario = abriPuertaLoginIngresoAdicional.usuario;
                    contrasena = abriPuertaLoginIngresoAdicional.contrasena;
                    fechaActual = DateTime.Now.Date;

                    string[] splitTerminal = terminal.Split('.');
                    dttTerminal = terminalBll.consultarTerminalID(Convert.ToInt16(splitTerminal[0]));

                    if (!string.IsNullOrEmpty(motivos))
                    {
                        eventosBLL.Insert("", identificacion, nombre, 0, 0, "", false, 0, fechaActual, fechaActual, false, "Ingreso adicional", "", "Ingreso adicional usuario: " + nombre + " Identificacion: " + identificacion + " " + motivos, true, dttTerminal.Rows[0]["ipAddress"].ToString(), "Entry", "", null, null, usuario, empresa);

                        resp = act.Insert(dttTerminal.Rows[0]["ipAddress"].ToString(), clsEnum.ServiceActions.AutorizeEntryClientById);

                        if (resp <= 0)
                        {
                            clsShowMessage.Show("El usuario " + identificacion + " puede ingresar pero no fue posible enviar el comando a la terminal.\n Por favor vuelva a intentarlo.", clsEnum.MessageType.Informa);
                        }
                        else
                        {
                            clsActionParameters aParameters = new clsActionParameters();
                            List<eActionParameters> aParametersList = new List<eActionParameters>();
                            eActionParameters aParamEntity = new eActionParameters();
                            aParamEntity.actionId = resp;
                            aParamEntity.parameterName = clsEnum.ActionParameters.UserId.ToString();
                            aParamEntity.parameterValue = identificacion;
                            aParametersList.Add(aParamEntity);
                            aParameters.Insert(aParametersList);
                        }

                        ShowData();
                        return;

                    }

                }

            }

        }

        FrmRecordFingerprint frmFingerprint;
        bool MostrarImagen = false;

        public string strIdClienteGrabaHuella;

        private void ExecuteEntry(string entryType)
        {
            try
            {
                string title = string.Empty;

                if (entryType == clsEnum.EntryType.Entry.ToString())
                {
                    title = "Ingreso de discapacitados";
                }
                else if (entryType == clsEnum.EntryType.Exit.ToString())
                {
                    title = "Salida de discapacitados";
                }

                if (blnPermitirIngresoAdicional)
                {
                    FrmOpenDoor frmOpenDoor = new FrmOpenDoor(title);
                    frmOpenDoor.ipAddress = ipAddress;
                    frmOpenDoor.entryType = entryType;

                    if (frmOpenDoor.ShowDialog() == DialogResult.OK)
                    {
                        clsShowMessage.Show("En un momento se abrirá el ingreso.", clsEnum.MessageType.Informa);
                        return;
                    }
                }
                else
                {
                    clsShowMessage.Show("La configuración actual no permite ingresos adicionales.", clsEnum.MessageType.Informa);
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FrmIngresos()
        {
            InitializeComponent();
        }

        private void FrmIngresos_Load(object sender, EventArgs e)
        {
            try
            {
                Principal("Configuration");
                UpdateTimers();
                List<eConfiguration> config = new List<eConfiguration>();
                //eTerminal terminal = new eTerminal();
                TerminalBLL terminalBll = new TerminalBLL();
                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                this.Text = "Ingreso Gymsoft - Versión " + Application.ProductVersion;
                ipAddress = System.Configuration.ConfigurationManager.AppSettings["ipAddress"].ToString();
                string strGymId = System.Configuration.ConfigurationManager.AppSettings["gymId"].ToString();
                string strBranchId = System.Configuration.ConfigurationManager.AppSettings["branchId"].ToString();
                estadoLoginIngresoAdicional = System.Configuration.ConfigurationManager.AppSettings["LoginIngresoAdicional"].ToString();

                if (string.IsNullOrEmpty(strGymId) || string.IsNullOrEmpty(strBranchId))
                {
                    throw new Exception("No se ha configurado el id del gimnasio y/o el id de la sucursal.");
                }
                else
                {
                    gymId = Convert.ToInt32(strGymId);
                    branchId = Convert.ToInt32(strBranchId);
                }

                //config = acsBll.GetLocalAccessControlSettings();

                // lerr archivo Json de consulta 
                json = lg.cargarArchivos(1);
                if (json != "")
                {
                    config = JsonConvert.DeserializeObject<List<eConfiguration>>(json);
                }
                ///Leer archivo de terminales 
                json = lg.cargarArchivos(2);
                if (json != "")
                {
                    ListaterminalFiltro = JsonConvert.DeserializeObject<List<eTerminal>>(json);
                    Listaterminal = JsonConvert.DeserializeObject<List<eTerminal>>(json);
                }
                

                if (config != null)
                {
                    if (config[0].intTiempoActualizaIngresos > 0)
                    {
                        timeUpdateEntries = config[0].intTiempoActualizaIngresos;
                    }

                    blnPermitirIngresoAdicional = config[0].blnPermiteIngresosAdicionales;
                    entryType = config[0].strTipoIngreso;
                    blnAccesoDiscapacitados = config[0].bitAccesoDiscapacitados;
                    quantityAppAccessControlSimultaneous = config[0].quantityAppAccessControlSimultaneous;

                    if (config[0].intTiempoParaLimpiarPantalla > 0)
                    {
                        intTiempoParaLimpiarPantalla = config[0].intTiempoParaLimpiarPantalla;
                    }
                    MostrarImagen = config[0].bitImagenSIBO;
                    if (MostrarImagen)
                    {
                        //picLogo.Visible = true;
                        piclogo1.Visible = true;
                        pictureBox1.Visible = true;
                    }
                    else
                    {
                        //picLogo.Visible = false;
                        piclogo1.Visible = false;
                        pictureBox1.Visible = false;
                    }
                }

                ControlAppAccessControl();

                if (string.IsNullOrEmpty(ipAddress))
                {
                    isMonitor = true;
                }

                timerClearScreen = new System.Timers.Timer(intTiempoParaLimpiarPantalla * 1000);
                timerClearScreen.Elapsed += TimerClearScreen_Elapsed;

                if (!isMonitor)
                {
                    // terminal = terminalBll.GetTerminalByIp(ipAddress);

                    ListaterminalFiltro.Where(p => p.ipAddress == ipAddress).ToList();

                    if (ListaterminalFiltro != null)
                    {
                        blnTCAM7000 = ListaterminalFiltro[0].TCAM7000;
                        intTipoTerminal = ListaterminalFiltro[0].terminalTypeId;
                        snTerminal = ListaterminalFiltro[0].snTerminal;
                        blnSirveParaSalida = ListaterminalFiltro[0].servesToOutput;

                        if (blnSirveParaSalida)
                        {
                            blnSirveParaEntradaySalida = false;
                        }
                        else
                        {
                            blnSirveParaEntradaySalida = ListaterminalFiltro[0].servesToInputAndOutput;
                        }

                        blnModoTarjeta = ListaterminalFiltro[0].CardMode;
                        terminalTypeId = ListaterminalFiltro[0].terminalTypeId;
                    }
                }
                else
                {
                    string strServesToOutput = System.Configuration.ConfigurationManager.AppSettings["servesToOutput"].ToString();
                    string strServesToInputAndOutput = System.Configuration.ConfigurationManager.AppSettings["servesToInputAndOutput"].ToString();
                    string strCardMode = System.Configuration.ConfigurationSettings.AppSettings["cardMode"].ToString();
                    if (!string.IsNullOrEmpty(strServesToOutput)) { blnSirveParaSalida = Convert.ToBoolean(strServesToOutput); }

                    if (blnSirveParaSalida)
                    {
                        blnSirveParaEntradaySalida = false;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(strServesToInputAndOutput)) { blnSirveParaEntradaySalida = Convert.ToBoolean(strServesToInputAndOutput); }
                    }

                    if (!string.IsNullOrEmpty(strCardMode)) { blnModoTarjeta = Convert.ToBoolean(strCardMode); }
                }
                //////aca se debe de cambiar el metodod e consulta de tabla a archivo JSON segun condiciones de consulta 
                ShowData();
                StartTimerUpdateEntries();

                //Validamos que opciones se deben mostrar del menú del ingreso monitor cuando sea ingreso touch o terminal
                if (isMonitor)
                {
                    itemRecordFingerprint.Visible = false;
                    itemDisabledEntry.Visible = false;
                    itemEntryByID.Visible = false;
                    itemRestartTerminal.Visible = false;
                    itemDisabledExit.Visible = false;
                    itemExitByID.Visible = false;

                    //TerminalBLL terminalBLL = new TerminalBLL();
                    //List<eTerminal> list = new List<eTerminal>();
                    //list = terminalBLL.GetTerminals();
                    int IL = 0;

                    if (Listaterminal != null && Listaterminal.Count > 0)
                    {
                        foreach (eTerminal item in Listaterminal)
                        {
                            if (item.terminalTypeId == 4 && item.state)
                            {
                                IL++;
                            }
                        }
                    }

                    if (IL > 0)
                    {
                        descargarHuellaDeOtraSedeToolStripMenuItem.Visible = false;
                        itemRecordFingerprintUSB.Visible = false;
                    }
                    else
                    {
                        descargarHuellaDeOtraSedeToolStripMenuItem.Visible = true;
                        itemRecordFingerprintUSB.Visible = true;
                    }
                }
                else
                {
                    if (terminalTypeId == 4)
                    {
                        descargarHuellaDeOtraSedeToolStripMenuItem.Visible = false;
                        itemRecordFingerprintUSB.Visible = false;
                        itemRecordFingerprint.Visible = false;
                        itemRestartTerminal.Visible = false;
                    }
                    else
                    {
                        descargarHuellaDeOtraSedeToolStripMenuItem.Visible = true;
                        itemRecordFingerprintUSB.Visible = true;
                        itemRecordFingerprint.Visible = true;
                        itemRestartTerminal.Visible = true;
                    }

                    //Validamos si el ingreso sirve para salida y/o entrada y salida
                    if (blnSirveParaSalida && !blnSirveParaEntradaySalida)
                    {
                        this.Text = "[SALIDA]";
                        visitantesToolStripMenuItem.Visible = false;

                        if (blnAccesoDiscapacitados)
                        {
                            itemDisabledExit.Visible = true;
                            itemDisabledEntry.Visible = false;
                        }
                        else
                        {
                            itemDisabledExit.Visible = false;
                            itemDisabledEntry.Visible = false;
                        }

                        itemExitByID.Visible = true;
                        itemEntryByID.Visible = false;
                    }
                    else if (!blnSirveParaSalida && !blnSirveParaEntradaySalida)
                    {
                        this.Text = "[ENTRADA]";
                        visitantesToolStripMenuItem.Visible = true;

                        if (blnAccesoDiscapacitados)
                        {
                            itemDisabledExit.Visible = false;
                            itemDisabledEntry.Visible = true;
                        }
                        else
                        {
                            itemDisabledExit.Visible = false;
                            itemDisabledEntry.Visible = false;
                        }

                        itemExitByID.Visible = false;
                        itemEntryByID.Visible = true;
                    }

                    if (blnSirveParaEntradaySalida)
                    {
                        this.Text = "[ENTRADA Y SALIDA]";
                        visitantesToolStripMenuItem.Visible = true;

                        if (blnAccesoDiscapacitados)
                        {
                            itemDisabledExit.Visible = true;
                            itemDisabledEntry.Visible = true;
                        }
                        else
                        {
                            itemDisabledExit.Visible = false;
                            itemDisabledEntry.Visible = false;
                        }

                        itemExitByID.Visible = true;
                        itemEntryByID.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void TimerClearScreen_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerClearScreen.Enabled = false;
            timerClearScreen.Stop();
            lblName.Text = "Nombre: ";
            lblExpirationDate.Text = "Fecha de Vencimiento: ";
            lblEntryHour.Text = "Hora de Ingreso: ";
            lblPlan.Text = "Plan: ";
            lblAccessMessage.Text = string.Empty;
            lblrestrictionMessage.Text = string.Empty;
        }

        private void ControlAppAccessControl()
        {
            object[] vec = System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName);

            if (quantityAppAccessControlSimultaneous != 0 && vec.Length > quantityAppAccessControlSimultaneous)
            {
                clsShowMessage.Show("Se han abierto más ingresos de los permitidos!!", clsEnum.MessageType.Informa);
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Método para iniciar el timer de actualización de ingresos.
        /// Getulio Vargas - 2018-06-27 - OD 1307
        /// </summary>
        private void StartTimerUpdateEntries()
        {
            try
            {
                timerUpdateEntries = new System.Timers.Timer(timeUpdateEntries);
                timerUpdateEntries.Elapsed += TimerUpdateEntries_Elapsed;
                timerUpdateEntries.AutoReset = true;
                timerUpdateEntries.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void descargarHuellaDeOtraSedeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmDownloadFingerprint frmDown = new FrmDownloadFingerprint();
                frmDown.gymId = gymId;
                frmDown.branchId = branchId;
                frmDown.ShowDialog();
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void visitantesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmEmployeeLogin frmEmpLogin = new FrmEmployeeLogin();

                //Abrimos el formulario para el logueo del empleado.
                if (frmEmpLogin.ShowDialog() == DialogResult.OK)
                {
                    FrmVisitor frmVisitor = new FrmVisitor();
                    frmVisitor.employeeId = frmEmpLogin.employeeId;
                    frmVisitor.gymId = gymId;
                    frmVisitor.branchId = branchId;
                    frmVisitor.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Evento que se encarga de consultar y mostrar las entradas realizadas en el día actual para que se puedan mostrar en el grid.
        /// Este evento se ejecuta cada x tiempo.
        /// Getulio Vargas - 2018-06-27 - OD 1307
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerUpdateEntries_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                timerUpdateEntries.Stop();
                ShowData();
                System.Threading.Thread.Sleep(1000);
                timerUpdateEntries.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite mostrar los datos de las entradas en el grid del ingreso.
        /// Getulio Vargas - 2018-06-27 - OD 1307
        /// </summary>
        private void ShowData()
        {
            try
            {
                EventBLL entryBll = new EventBLL();
                DataTable dt = new DataTable();

                //Validamos si es ingreso monitor para ingreso touch o si es para mostrar en terminal.
                if (isMonitor)
                {
                    dt = entryBll.GetEntriesToShowMonitor(blnSirveParaSalida, blnSirveParaEntradaySalida);
                }
                else
                {
                    dt = entryBll.GetEntriesToShow(ipAddress, blnSirveParaSalida, blnSirveParaEntradaySalida);
                }

                //if (dt != null)
                if(dt.Rows.Count > 0)
                {
                    CheckForIllegalCrossThreadCalls = false;

                    if ((dt.Rows.Count > dgvEntries.Rows.Count) || firstTime)
                    {
                        //dgvEntries.DataSource = null;
                        //dgvEntries.DataSource = dt;

                        //dgvEntries.Invoke(delegate (DataTable table)
                        //{
                        //    dgvEntries.DataSource = table;
                        //}, dt);


                        //mtcanbebyhere
                        dgvEntries.Invoke((MethodInvoker)delegate { dgvEntries.DataSource = dt; });

                        dgvEntries.Columns[0].Width = 80;
                        dgvEntries.Columns[1].Width = 170;
                        dgvEntries.Columns[2].Width = 200;
                        dgvEntries.Columns[3].Width = 200;
                        dgvEntries.Columns[4].Width = 136;
                        dgvEntries.Columns[5].Width = 300;
                        dgvEntries.Columns[6].Width = 300;

                        if (dt.Rows.Count > 0)
                        {
                            //Mensaje a Donchi
                            string mensajeCliente = string.Empty;
                            List<eClientMessages> cmList = new List<eClientMessages>();
                            ClientMessagesBLL cmBll = new ClientMessagesBLL();
                            cmList = cmBll.GetLocalCLientMessages();

                            //eConfiguration config = new eConfiguration();
                            //AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                            //config = acsBll.GetLocalAccessControlSettings();

                            if (cmList != null && cmList.Count > 0)
                            {
                                foreach (eClientMessages item in cmList)
                                {
                                    mensajeCliente = mensajeCliente + " " + item.messageText;
                                }
                            }

                            string id = "";
                            id = dt.Rows[0]["Identificación"].ToString();

                            eConfiguration config = new eConfiguration();
                            AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                            config = acsBll.GetLocalAccessControlSettings();

                            if (config.bitMensajeCumpleanos)
                            {
                                WhiteListBLL wlBll = new WhiteListBLL();
                                if (wlBll.CumpleAniosCliente(id))
                                {
                                    mensajeCliente += " ¡Feliz Cumpleaños!";
                                }
                            }

                            userName = dt.Rows[0]["Nombre"].ToString();
                            planName = dt.Rows[0]["Plan"].ToString();
                            expirationDate = dt.Rows[0]["Fecha de vencimiento"].ToString();
                            date = dt.Rows[0]["Fecha"].ToString();
                            firstMessage = dt.Rows[0]["Mensaje de ingreso"].ToString();
                            secondMessage = dt.Rows[0]["Mensaje de restricción"].ToString();

                            lblName.Text = "Nombre: " + userName;
                            lblExpirationDate.Text = "Fecha de Vencimiento: " + expirationDate;
                            lblEntryHour.Text = "Hora de Ingreso: " + date;
                            lblPlan.Text = "Plan: " + planName;
                            lblAccessMessage.Text = firstMessage;
                            lblrestrictionMessage.Text = secondMessage + "\n" + mensajeCliente;
                            lblrestrictionMessage.ForeColor = Color.Blue;

                            //PRUEBAS
                            //if (dt.Rows[0]["Entrada exitosa"].ToString() == "SI" && dt.Rows[0]["Salida"].ToString() == "NO")
                            //{
                            //    lblAccessMessage.Text = firstMessage;
                            //    lblrestrictionMessage.Text = secondMessage + "\n" + mensajeCliente;
                            //    lblrestrictionMessage.ForeColor = Color.Blue;
                            //    lblAccessMessage.ForeColor = Color.Green;
                            //}
                            //else
                            //{
                            //    lblAccessMessage.Text = "Puede Salir";
                            //    lblrestrictionMessage.Text = "Vuelva Pronto\n" + mensajeCliente;
                            //    lblrestrictionMessage.ForeColor = Color.Blue;
                            //    lblAccessMessage.ForeColor = Color.Green;
                            //    //lblAccessMessage.ForeColor = Color.Red;
                            //}

                            if (dt.Rows[0]["Entrada exitosa"].ToString() == "SI")
                            {
                                lblAccessMessage.ForeColor = Color.Green;
                            }
                            else
                            {
                                lblAccessMessage.ForeColor = Color.Red;
                            }

                            timerClearScreen.Enabled = true;
                            timerClearScreen.Start();
                        }

                        firstTime = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método encargado de mostrar los datos de la entrada seleccionada en el grid, en los controles superiores.
        /// Getulio Vargas - 2018-06-27 - OD 1307
        /// </summary>
        /// <param name="rowIndex"></param>
        private void ShowSpecificData(int rowIndex)
        {
            try
            {
                ////Mensaje a Donchi
                //string mensajeCliente = string.Empty;
                //List<eClientMessages> cmList = new List<eClientMessages>();
                //ClientMessagesBLL cmBll = new ClientMessagesBLL();
                //cmList = cmBll.GetLocalCLientMessages();

                ////eConfiguration config = new eConfiguration();
                ////AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                ////config = acsBll.GetLocalAccessControlSettings();

                //if (cmList != null && cmList.Count > 0)
                //{
                //    foreach (eClientMessages item in cmList)
                //    {
                //        mensajeCliente = mensajeCliente + " " + item.messageText;
                //    }
                //}
                    

                userName = dgvEntries.Rows[rowIndex].Cells["nombre"].Value.ToString();
                planName = dgvEntries.Rows[rowIndex].Cells["Plan"].Value.ToString();
                expirationDate = dgvEntries.Rows[rowIndex].Cells["Fecha de vencimiento"].Value.ToString();
                date = dgvEntries.Rows[rowIndex].Cells["Fecha"].Value.ToString();
                firstMessage = dgvEntries.Rows[rowIndex].Cells["Mensaje de ingreso"].Value.ToString();
                secondMessage = dgvEntries.Rows[rowIndex].Cells["Mensaje de restricción"].Value.ToString();

                lblName.Text = "Nombre: " + userName;
                lblExpirationDate.Text = "Fecha de Vencimiento: " + expirationDate;
                lblEntryHour.Text = "Hora de Ingreso: " + date;
                lblPlan.Text = "Plan: " + planName;
                lblAccessMessage.Text = firstMessage;
                lblrestrictionMessage.Text = secondMessage; //SA
                lblrestrictionMessage.ForeColor = Color.Blue;

                if (dgvEntries.Rows[rowIndex].Cells["Entrada exitosa"].Value.ToString() == "SI")
                {
                    lblAccessMessage.ForeColor = Color.Green;
                }
                else
                {
                    lblAccessMessage.ForeColor = Color.Red;
                }

                timerClearScreen.Enabled = true;
                timerClearScreen.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Evento para actualizar los datos de la entrada en los controles superiores.
        /// Getulio Vargas - 2018-06-27 - OD 1307
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvEntries_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvEntries.CurrentCell != null)
                {
                    ShowSpecificData(dgvEntries.CurrentCell.RowIndex);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Evento para grabar la huella por medio de la terminal.
        /// Getulio Vargas - 2018-07-05 - OD 1307
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemRecordFingerprint_Click(object sender, EventArgs e)
        {
            try
            {
                int resp = 0;
                int fingerId = 0;

                if (blnTCAM7000 && intTipoTerminal != 7 && intTipoTerminal != 8)
                {
                    strImageFingerprint = string.Empty;
                    frmFingerprint = new FrmRecordFingerprint();
                    frmFingerprint.blnTCAM7000 = blnTCAM7000;
                    frmFingerprint.gymId = gymId;
                    frmFingerprint.branchId = branchId;
                    frmFingerprint.ShowDialog();

                    if (frmFingerprint.DialogResult == DialogResult.OK)
                    {
                        fingerId = frmFingerprint.fingerId;
                        userId = frmFingerprint.clientId;
                        //clsAction act = new clsAction();
                        //resp = act.Insert(ipAddress, clsEnum.ServiceActions.Enroll);
                        string enrolar = clsEnum.ServiceActions.Enroll.ToString();
                        resp = action.InsertAction(gymId, branchId.ToString(), ListaterminalFiltro[0].ipAddress, enrolar);

                        if (resp > 0)
                        {
                            clsActionParameters aParameters = new clsActionParameters();
                            List<eActionParameters> aParametersList = new List<eActionParameters>();
                            eActionParameters aParamEntity = new eActionParameters();
                            aParamEntity.actionId = resp;
                            //Agregamos la identificación del usuario a los parámetros de la acción.
                            aParamEntity.parameterName = clsEnum.ActionParameters.UserId.ToString();
                            aParamEntity.parameterValue = userId;
                            aParamEntity.gymId = gymId;
                            aParametersList.Add(aParamEntity);
                            //Agregamos el id de la huella a grabar a los parámetros de la acción.
                            eActionParameters aParamEntityFinger = new eActionParameters();
                            aParamEntityFinger.actionId = resp;
                            aParamEntityFinger.parameterName = clsEnum.ActionParameters.FingerPrintId.ToString();
                            aParamEntityFinger.parameterValue = fingerId.ToString();
                            aParamEntityFinger.gymId = gymId;
                            aParametersList.Add(aParamEntityFinger);
                            //aParameters.Insert(aParametersList);
                            action.AddActionParameters(aParametersList);
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                else if (intTipoTerminal == 7 || intTipoTerminal == 8)
                {

                    strImageFingerprint = string.Empty;
                    frmFingerprint = new FrmRecordFingerprint();
                    frmFingerprint.blnTCAM7000 = true;
                    frmFingerprint.gymId = gymId;
                    frmFingerprint.branchId = branchId;
                    frmFingerprint.ShowDialog();

                    if (frmFingerprint.DialogResult == DialogResult.OK)
                    {

                        //ActionBLL acionHuellaInsert = new ActionBLL();
                        //acionHuellaInsert.InsertHuellaBioLite(ipAddress, snTerminal.ToString(), frmFingerprint.txtDocument.Text, false);
                        //// se crea comando de action para enrrolar la huella en las biolite se inserta la cedula en el campo strAction para ser reconocido en el timer de acciones pendiente 
                        string enrolar = frmFingerprint.txtDocument.Text;
                        resp = action.InsertAction(gymId, branchId.ToString(), ListaterminalFiltro[0].ipAddress, enrolar);

                    }
                }
                else
                {
                    //clsShowMessage.Show("No es posible mostrar la ventana de grabación de huellas por alguna(s) de las siguientes razones: " +
                    //                    "\n- No se encontró la configuración del ingreso en la Base de Datos. " +
                    //                    "\n- El parámetro para identificar la terminal no está habilitado.", clsEnum.MessageType.Informa);
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Evento para grabar la huella por medio de lector USB.
        /// Getulio Vargas - 2018-07-05 - OD 1307
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemRecordFingerprintUSB_Click(object sender, EventArgs e)
        {
            try
            {
                strImageFingerprint = string.Empty;
                frmFingerprint = new FrmRecordFingerprint();
                frmFingerprint.blnTCAM7000 = false;
                frmFingerprint.gymId = gymId;
                frmFingerprint.branchId = branchId;
                frmFingerprint.ShowDialog();

                if (frmFingerprint.DialogResult == DialogResult.OK)
                {

                }
                else
                {
                    return;
                }

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Evento para el ingreso de discapacitados.
        /// Getulio Vargas - 2018-07-05 - OD 1307
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemDisabledEntry_Click(object sender, EventArgs e)
        {
            try
            {
                ExecuteEntry(clsEnum.EntryType.Entry.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Evento para la salida de discapacitados.
        /// Getulio Vargas - 2018-07-05 - OD 1307
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemDisabledExit_Click(object sender, EventArgs e)
        {
            try
            {
                ExecuteEntry(clsEnum.EntryType.Exit.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Evento para reiniciar la terminal.
        /// Getulio Vargas - 2018-07-05 - OD 1307
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemRestartTerminal_Click(object sender, EventArgs e)
        {
            try
            {
                FrmEmployeeLogin frmELogin = new FrmEmployeeLogin();
                clsAction act = new clsAction();
                int resp = 0;

                if (frmELogin.ShowDialog() == DialogResult.OK)
                {
                    resp = act.Insert(ipAddress, clsEnum.ServiceActions.Restart);

                    if (resp > 0)
                    {
                        clsShowMessage.Show("La terminal será reiniciada en un momento.", clsEnum.MessageType.Informa);
                        return;
                    }
                    else
                    {
                        clsShowMessage.Show("No fue posible reiniciar la terminal.", clsEnum.MessageType.Informa);
                        return;
                    }
                }
                else
                {
                    clsShowMessage.Show("No fue posible reiniciar la terminal.", clsEnum.MessageType.Informa);
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void itemEntryByID_Click(object sender, EventArgs e)
        {
            try
            {
                FrmEntryOrExitById frmEntry = new FrmEntryOrExitById();
                clsWhiteList wl = new clsWhiteList();
                clsAction act = new clsAction();
                frmEntry.Text = "Ingreso por ID";
                string id = string.Empty;
                int resp = 0;

                if (frmEntry.ShowDialog() == DialogResult.OK)
                {
                    id = frmEntry.id;

                    if (!string.IsNullOrEmpty(id))
                    {
                        if (wl.ValidateEntryByUserId(id, ipAddress, true))
                        {
                            resp = act.Insert(ipAddress, clsEnum.ServiceActions.AutorizeEntryClientById);

                            if (resp <= 0)
                            {
                                clsShowMessage.Show("El usuario " + id + " puede ingresar pero no fue posible enviar el comando a la terminal.\n Por favor vuelva a intentarlo.", clsEnum.MessageType.Informa);
                            }
                            else
                            {
                                clsActionParameters aParameters = new clsActionParameters();
                                List<eActionParameters> aParametersList = new List<eActionParameters>();
                                eActionParameters aParamEntity = new eActionParameters();
                                aParamEntity.actionId = resp;
                                aParamEntity.parameterName = clsEnum.ActionParameters.UserId.ToString();
                                aParamEntity.parameterValue = id;
                                aParametersList.Add(aParamEntity);
                                aParameters.Insert(aParametersList);
                            }

                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        clsShowMessage.Show("No se ingresó la identificación del usuario.", clsEnum.MessageType.Informa);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void itemExitByID_Click(object sender, EventArgs e)
        {
            try
            {
                FrmEntryOrExitById frmExit = new FrmEntryOrExitById();
                clsWhiteList wl = new clsWhiteList();
                clsAction act = new clsAction();
                frmExit.Text = "Salida por ID";
                string id = string.Empty;
                int resp = 0;

                if (frmExit.ShowDialog() == DialogResult.OK)
                {
                    id = frmExit.id;

                    if (!string.IsNullOrEmpty(id))
                    {
                        if (wl.ValidateExitById(id, ipAddress, true))
                        {
                            resp = act.Insert(ipAddress, clsEnum.ServiceActions.AutorizeExitClientById);

                            if (resp <= 0)
                            {
                                clsShowMessage.Show("El usuario " + id + " puede salir pero no fue posible enviar el comando a la terminal.\n Por favor vuelva a intentarlo.", clsEnum.MessageType.Informa);
                            }
                            else
                            {
                                clsActionParameters aParameters = new clsActionParameters();
                                List<eActionParameters> aParametersList = new List<eActionParameters>();
                                eActionParameters aParamEntity = new eActionParameters();
                                aParamEntity.actionId = resp;
                                aParamEntity.parameterName = clsEnum.ActionParameters.UserId.ToString();
                                aParamEntity.parameterValue = id;
                                aParametersList.Add(aParamEntity);
                                aParameters.Insert(aParametersList);
                            }

                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        clsShowMessage.Show("No se ingresó la identificación del usuario.", clsEnum.MessageType.Informa);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void itemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool Principal(string action)
        {
            try
            {
                string strGymId = ConfigurationManager.AppSettings["gymId"];
                string strBranchId = ConfigurationManager.AppSettings["branchId"];
                ZonasBLL z = new ZonasBLL();
                TerminalBLL ter = new TerminalBLL();

                if (string.IsNullOrEmpty(strGymId) || string.IsNullOrEmpty(strBranchId))
                {
                    throw new Exception("No se ha configurado de forma correcta el código del gimnasio y/o la sucursal.");
                }

                int gymId = Convert.ToInt32(strGymId);
                // int branchId = Convert.ToInt32(strBranchId);
                string branchId = strBranchId;
                bool resp = false;

                switch (action)
                {
                    case "Configuration":
                        resp = new AccessControlSettingsBLL().GetAccessControlSettings(gymId, branchId);
                        new HolidayBLL().GetHolidays(gymId);
                        z.GetZonas(gymId, branchId);
                        ter.GetTerminals(gymId, branchId);
                        break;
                    default:
                        break;
                }

                return resp;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} {ex.StackTrace}");
            }
        }

        private void UpdateTimers()
        {
            try
            {
                int oneMinute = 60000;
                AccessControlSettingsBLL acsBLL = new AccessControlSettingsBLL();
                List<int> timerList = new List<int>();
                int aux = 0;
                log.WriteProcess("Inicia el proceso para actualizar timers.");


                string pathPrincipal = string.Empty, date = string.Empty;

                timerList = acsBLL.GetTimers();

                if (timerList != null && timerList.Count > 0)
                {
                    if (timerList[0] > 0 && (timerList[0] * oneMinute) != timeConfiguration)
                    {
                        aux = timeConfiguration;
                        timeConfiguration = timerList[0] * oneMinute;
                        FinalizeTimerConfiguration();
                        InitializeTimerConfiguration(timeConfiguration);
                        log.WriteProcess("Se actualiza timer de configuración, pasa de " + aux.ToString() + " ms a " + timeConfiguration.ToString() + " ms.");
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

        private void InitializeTimerConfiguration(int timeConfiguration)
        {
            try
            {
                if (timeConfiguration > 0)
                {
                    timerConfiguration = new System.Timers.Timer(timeConfiguration);
                }
                else
                {
                    timerConfiguration = new System.Timers.Timer(Convert.ToInt32(ConfigurationManager.AppSettings["time"].ToString()));
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
    }
}
