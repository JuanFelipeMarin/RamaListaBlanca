﻿using libzkfpcsharp;
using Sample;
using Sibo.WhiteList.IngresoMonitor;
using Sibo.WhiteList.IngresoTouch.Classes;
using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.BLL.Helpers;
using Sibo.WhiteList.Service.Entities.Classes;
using Sibo.WhiteList.Service.BLL.Log;
using Suprema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Timers;
using Sibo.WhiteList.Service.DAL.API;

namespace Sibo.WhiteList.IngresoTouch
{
    public partial class FrmIngreso : Form
    {
        ServiceLog log = new ServiceLog();

        string userName = string.Empty, planName = string.Empty, expirationDate = string.Empty, strIdCliente = string.Empty,
               date = string.Empty, firstMessage = string.Empty, secondMessage = string.Empty, strImageFingerprint = string.Empty, userId = string.Empty,
               strPuerto = "COM1", strPuertoOut = "COM1", strColorPpal = "#527BE7", nombreLogo = string.Empty;
        bool blnPermitirIngresoAdicional = true, blnAccesoDiscapacitados = false, blnSirveParaSalida = false, blnSirveParaEntradaySalida = false,
             blnModoTarjeta = false, blnIngresoAbreDesdeTouch = true, blnAntipassbackEntrada = false;
        int intVelocidad = 9600, intTiempoPulso = 9, intTamanoImagen = 1024, index, intTiempoParaLimpiarPantalla = 5;
        System.Timers.Timer timerConfiguration;
        int timeConfiguration = 0;
        bool getConf = false;
       bool getTerminal = false;
        byte[][] bdTemplate;
        int[] dbTemplateSize;
        DataTable dtHuellas = new DataTable();
        UFScanner usLectorUSB = new UFScanner();
        UFScannerManager usLectorManagenUSB;
        Bitmap ImagenCapturada;
        byte[] HuellaBytes = new byte[1024];
        UFMatcher usComparadorUSB;
        System.Timers.Timer timerClearScreen;
        bool MostrarImagen = false;
        public string estadoLoginIngresoAdicional = "";
       public List<eTerminal> Listaterminal = new List<eTerminal>();
        string json = "";

        Boolean lectorCama = false;
        Boolean lectorHuellaZk = false;
        Boolean ZkPalmPv10M = false;
        Boolean bitLectorSiempreEncendido = false;
        Thread m_hThreadCapture = null;
        bool m_bStop = false;
        leerJson lg = new leerJson();
        FingerPrintAPI fpAPI = new FingerPrintAPI();
        ActionAPI action = new ActionAPI();
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

        private void itemRecordFingerprintUSB_Click(object sender, EventArgs e)
        {
            try
            {
                m_bStop = true;
                if (bitLectorSiempreEncendido)
                {
                    if (!ZkPalmPv10M && !lectorHuellaZk && !lectorCama)
                    {
                        usLectorManagenUSB.Scanners[0].AbortCapturing();
                    }
                }
                if (lectorHuellaZk)
                {
                    bIsTimeToDie = true;
                    zkfp2.CloseDevice(mDevHandle);
                    zkfp2.Terminate();
                }

                strImageFingerprint = string.Empty;
                frmFingerprint = new FrmRecordFingerprint(ref usLectorUSB, ref usLectorManagenUSB, ref usComparadorUSB);
                frmFingerprint.blnTCAM7000 = false;
                frmFingerprint.gymId = gymId;
                frmFingerprint.branchId = branchId;
                frmFingerprint.ShowDialog();

                if (frmFingerprint.DialogResult == DialogResult.OK)
                {
                    if (bitLectorSiempreEncendido)
                    {
                        if (ZkPalmPv10M)
                        {
                            m_bStop = false;
                            ActivarLectorPalmaZk("0");
                        }
                        else
                        {
                            if (!lectorHuellaZk && !lectorCama)
                            {
                                IniciarLector2();
                            }
                            else
                            {
                                if (lectorHuellaZk)
                                {
                                    ObtenerTemplateZk("0");
                                    if (dtHuellas != null && dtHuellas.Rows.Count > 0)
                                    {
                                        bIsTimeToDie = false;
                                        Thread captureThread = new Thread(new ThreadStart(DoCapture));
                                        captureThread.IsBackground = true;
                                        captureThread.Start();
                                    }
                                }
                            }
                        }
                    }
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

        IntPtr mDevHandle = IntPtr.Zero;
        IntPtr mDBHandle = IntPtr.Zero;
        IntPtr FormHandle = IntPtr.Zero;
        int RegisterCount = 0;
        int cbRegTmp = 0;
        int iFid = 1;
        byte[][] RegTmps = new byte[3][];
        private int mfpWidth = 0;
        private int mfpHeight = 0;
        byte[] FPBuffer;
        bool bIsTimeToDie = false;
        int cbCapTmp = 2000;
        byte[] CapTmp = new byte[2000];
        byte[] RegTmp = new byte[2000];

        const int MESSAGE_CAPTURED_OK = 0x0400 + 6;

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private bool IniciarLectorZk()
        {
            try
            {
                FormHandle = this.Handle;
                int ret = zkfperrdef.ZKFP_ERR_OK;
                if ((ret = zkfp2.Init()) == zkfperrdef.ZKFP_ERR_OK)
                {
                    int nCount = zkfp2.GetDeviceCount();
                    if (nCount == 0)
                    {
                        zkfp2.Terminate();
                        clsShowMessage.Show("No hay lector conectado", clsEnum.MessageType.Informa);
                        return false;
                    }
                    ret = zkfp.ZKFP_ERR_OK;
                    if (IntPtr.Zero == (mDevHandle = zkfp2.OpenDevice(0)))
                    {
                        clsShowMessage.Show("No se puede conectar el dispositivo", clsEnum.MessageType.Informa);
                        return false;
                    }
                    if (IntPtr.Zero == (mDBHandle = zkfp2.DBInit()))
                    {
                        clsShowMessage.Show("No se pudo iniciar la base de datos", clsEnum.MessageType.Informa);
                        zkfp2.CloseDevice(mDevHandle);
                        mDevHandle = IntPtr.Zero;
                        return false;
                    }
                    RegisterCount = 0;
                    cbRegTmp = 0;
                    iFid = 1;
                    for (int i = 0; i < 3; i++)
                    {
                        RegTmps[i] = new byte[2000];
                    }
                    byte[] paramValue = new byte[4];
                    int size = 4;
                    zkfp2.GetParameters(mDevHandle, 1, paramValue, ref size);
                    zkfp2.ByteArray2Int(paramValue, ref mfpWidth);

                    size = 4;
                    zkfp2.GetParameters(mDevHandle, 2, paramValue, ref size);
                    zkfp2.ByteArray2Int(paramValue, ref mfpHeight);

                    FPBuffer = new byte[mfpWidth * mfpHeight];

                    if (bitLectorSiempreEncendido)
                    {
                        ObtenerTemplateZk("0");
                        if (dtHuellas != null && dtHuellas.Rows.Count > 0)
                        {
                            bIsTimeToDie = false;
                            Thread captureThread = new Thread(new ThreadStart(DoCapture));
                            captureThread.IsBackground = true;
                            captureThread.Start();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se pudo inicializar el lector zk, ret=" + ret + " !");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DoCapture()
        {
            while (!bIsTimeToDie)
            {
                cbCapTmp = 2000;
                if (lblMensajeRestricion.Text.IndexOf("Coloque") < 0 && lblMensajeRestricion.Text.Length == 0)
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            lblMensajeAcceso.Text = string.Empty;
                            lblMensajeRestricion.Text = "Coloque el dedo en el lector de huella";
                            lblMensajeRestricion.ForeColor = Color.Blue;
                            this.Refresh();
                        }));
                    }
                }

                int ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
                if (ret == zkfp.ZKFP_ERR_OK)
                {
                    SendMessage(FormHandle, MESSAGE_CAPTURED_OK, IntPtr.Zero, IntPtr.Zero);
                }
                Thread.Sleep(200);
                if (lblMensajeRestricion.Text == "Huella erronea!")
                {
                    Thread.Sleep(1000);
                    Invoke(new Action(() =>
                    {
                        lblMensajeRestricion.Text = "";
                        this.Refresh();
                    }));

                }
            }
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MESSAGE_CAPTURED_OK:
                    {
                        MemoryStream ms = new MemoryStream();
                        BitmapFormat.GetBitmap(FPBuffer, mfpWidth, mfpHeight, ref ms);
                        Bitmap bmp = new Bitmap(ms);

                        int ret = zkfp.ZKFP_ERR_OK;
                        int fid = 0, score = 0;
                        ret = zkfp2.DBIdentify(mDBHandle, CapTmp, ref fid, ref score);
                        if (zkfp.ZKFP_ERR_OK == ret)
                        {
                            strIdCliente = dtHuellas.Rows[fid - 1]["id"].ToString();
                            ValidateEntryByUserId(strIdCliente);
                            strIdCliente = "";
                            return;
                        }
                        else
                        {
                            lblMensajeRestricion.Text = "Huella erronea!";
                            lblMensajeRestricion.ForeColor = Color.Red;
                            return;
                        }
                    }
                    break;

                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void ctrlTecladoIngreso_KeyPress(object sender, KeyPressEventArgs e)
        {
            ctrlTecladoIngreso.swKeyboard = true;
        }


        private void itemExit_Click(object sender, EventArgs e)
        {
            if (bitLectorSiempreEncendido)
            {
                if (lectorHuellaZk)
                {
                    bIsTimeToDie = true;
                    zkfp2.CloseDevice(mDevHandle);
                    zkfp2.Terminate();

                }
            }
            m_bStop = true;
            Thread.Sleep(200);
            this.Close();
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

        private void picLogoEmpresa_Click(object sender, EventArgs e)
        {

        }

        //Incidencia 0005260 Mtoro
        private void FrmIngreso_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Regex regex = new Regex(@"[0-9]");
                string key = e.KeyChar.ToString();

                if (regex.IsMatch(key))
                {
                    ctrlTecladoIngreso.Quantity += key;

                }
                else if (key == "\r")//enter
                {
                    if (ctrlTecladoIngreso.Quantity.Length > 0)
                    {
                        if (!blnModoTarjeta)
                        {
                            ValidateEntryByUserId(ctrlTecladoIngreso.Quantity);
                        }
                        else
                        {
                            ValidateEntryByUserId("T-" + ctrlTecladoIngreso.Quantity);
                        }
                    }

                    ctrlTecladoIngreso.swKeyboard = false;
                }
                else if (key == "\b" && ctrlTecladoIngreso.Quantity.Length > 0)//borrar dígito
                {
                    ctrlTecladoIngreso.Quantity =
                        ctrlTecladoIngreso.Quantity.Remove(ctrlTecladoIngreso.Quantity.Length - 1);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void itemAbrirPuerta_Click(object sender, EventArgs e)
        {
            eEvent eventos = new eEvent();
           
            EventBLL eventosBLL = new EventBLL();
            DataTable dttTerminal = new DataTable();
            TerminalBLL terminalBll = new TerminalBLL();
            DateTime fechaActual = new DateTime();
            clsAction act = new clsAction();
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
                abrirPuerta.Text = "Ingreso adicional";
                string motivo = string.Empty;
                string terminal = string.Empty;

                if (abrirPuerta.ShowDialog() == DialogResult.OK)
                {
                    motivo = abrirPuerta.motivos;
                    terminal = abrirPuerta.terminal;
                    fechaActual = DateTime.Now;

                    string[] splitTerminal = terminal.Split('.');
                    dttTerminal = terminalBll.consultarTerminalID(Convert.ToInt16(splitTerminal[0]));
                    if (!string.IsNullOrEmpty(motivo))
                    {
                        
                        ////eventosBLL.Insert("", "0", "Ingreso adicional", 0, 0, "", false, 0, fechaActual, fechaActual, false, "Ingreso adicional", "", "Ingreso adicional : " + motivo, true, dttTerminal.Rows[0]["ipAddress"].ToString(), "Entry", "", null, null);
                        ////aqui va un insert a GWS con la marcacion del ingreso adicional 
                   
                        lblNombre.Text = "Nombre: " + motivo;
                        lblUltimoIngreso.Text = "Fecha ingreso: " + fechaActual.ToString("yyyy-MM-dd");
                        lblUltimaHora.Text = "Hora: " + fechaActual.ToString("hh:mm");

                        // creacion de evento accion en tabla lista blanca
                        // resp = act.Insert(dttTerminal.Rows[0]["ipAddress"].ToString(), clsEnum.ServiceActions.AutorizeEntryClientById);

                        //// se realiza cambio para creacion de accion el cual sera dispara por el servicio, se crea api para insertar nuevo comando
                        string enrolar = clsEnum.ServiceActions.AutorizeEntryClientById.ToString();
                        resp = action.InsertAction(gymId, branchId.ToString(), Listaterminal[0].ipAddress, enrolar);

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
                            aParamEntity.gymId = gymId;
                            aParametersList.Add(aParamEntity);
                            //aParameters.Insert(aParametersList);
                            action.AddActionParameters(aParametersList);
                        }
                    }
                }
            }
            else if (bitLoginIngresoAdicional == true)
            {
                FrmAbriPuertaLoginIngresoAdicional abriPuertaLoginIngresoAdicional = new FrmAbriPuertaLoginIngresoAdicional();

                abriPuertaLoginIngresoAdicional.Text = "Ingreso adicional";

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
                    terminal = abriPuertaLoginIngresoAdicional.terminal;
                    identificacion = abriPuertaLoginIngresoAdicional.identificacion;
                    nombre = abriPuertaLoginIngresoAdicional.nombre;
                    empresa = abriPuertaLoginIngresoAdicional.empresa;
                    usuario = abriPuertaLoginIngresoAdicional.usuario;
                    contrasena = abriPuertaLoginIngresoAdicional.contrasena;
                    fechaActual = DateTime.Now.Date;

                    string[] splitTerminal = terminal.Split('.');
                    // dttTerminal = terminalBll.consultarTerminalID(Convert.ToInt16(splitTerminal[0]));
                    Listaterminal.Where(p => p.ipAddress == splitTerminal[0]).ToList();

                    if (!string.IsNullOrEmpty(motivos))
                    {
                        ////eventosBLL.Insert("", identificacion, nombre, 0, 0, "", false, 0, fechaActual, fechaActual, false, "Ingreso adicional", "", "Ingreso adicional usuario: " + nombre + " Identificacion: " + identificacion + " " + motivos, true, dttTerminal.Rows[0]["ipAddress"].ToString(), "Entry", "", null, null, usuario, empresa);
                        ////aqui va un insert a GWS con la marcacion del ingreso adicional 
                        // presentacion en pantalla

                        lblNombre.Text = "Nombre: " + nombre;
                        lblUltimoIngreso.Text = "Fecha ingreso: " + fechaActual.ToString("yyyy-MM-dd");
                        lblUltimaHora.Text = "Hora: " + fechaActual.ToString("hh:mm");

                        //  creacion de accion para ser disparada desde el servicio
                        //resp = act.Insert(dttTerminal.Rows[0]["ipAddress"].ToString(), clsEnum.ServiceActions.AutorizeEntryClientById);

                        //// se realiza cambio para creacion de accion el cual sera dispara por el servicio, se crea api para insertar nuevo comando
                        string enrolar = clsEnum.ServiceActions.AutorizeEntryClientById.ToString();
                        resp = action.InsertAction(gymId,branchId.ToString(), Listaterminal[0].ipAddress, enrolar);

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
                            aParamEntity.gymId = gymId;
                            aParametersList.Add(aParamEntity);
                           //  aParameters.Insert(aParametersList);
                            action.AddActionParameters(aParametersList);
                        }

                    }

                }

            }
            // timer para limpiar campos
            timerClearScreen.Enabled = true;
            timerClearScreen.Start();
        }

        //FIN Incidencia 0005260 Mtoro

        int timeUpdateEntries = 1000, gymId = 0, branchId = 0;

        private void FrmIngreso_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_bStop = true;
        }

        FrmRecordFingerprint frmFingerprint;

        public string strIdClienteGrabaHuella;

        public FrmIngreso()
        {
            InitializeComponent();
        }

        private void FrmIngreso_Load(object sender, EventArgs e)
        {
            try
            {
                Principal("Configuration");
                UpdateTimers();
                string parCama = ConfigurationManager.AppSettings["LectorHuellaCama"].ToString();
                if (parCama == "0") parCama = "false";
                if (parCama == "1") parCama = "true";
                string parHuellaZK = ConfigurationManager.AppSettings["LectorHuellaZk"].ToString();
                if (parHuellaZK == "0") parHuellaZK = "false";
                if (parHuellaZK == "1") parHuellaZK = "true";
                string parZkPalmPv10M = ConfigurationManager.AppSettings["ZkPalmPv10M"].ToString();
                if (parZkPalmPv10M == "0") parZkPalmPv10M = "false";
                if (parZkPalmPv10M == "1") parZkPalmPv10M = "true";
                lectorCama = Boolean.Parse(parCama);
                lectorHuellaZk = Boolean.Parse(parHuellaZK);
                ZkPalmPv10M = Boolean.Parse(parZkPalmPv10M);
                int cantLecAct = 0;
                if (lectorCama) cantLecAct += 1;
                if (lectorHuellaZk) cantLecAct += 1;
                if (ZkPalmPv10M) cantLecAct += 1;
                //CheckForIllegalCrossThreadCalls = false;//mtoro (desactivar el control de cruce de hilos)
                if (!(lectorCama || lectorHuellaZk || ZkPalmPv10M))
                {
                    usLectorManagenUSB = new UFScannerManager(this);
                    usComparadorUSB = new UFMatcher();
                }
                if (cantLecAct > 1)
                {
                    MessageBox.Show("No se puenden tener mas de un lector configurado ya sea cama, zk o de palma.");
                    Application.Exit();
                    return;
                }
                if (ZkPalmPv10M)
                {
                    itemRecordFingerprintUSB.Text = "Grabar palma en USB";
                }
                validarArchivos();
                List<eConfiguration> config = new List<eConfiguration>();
                eTerminal terminal = new eTerminal();
                TerminalBLL terminalBll = new TerminalBLL();
                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                this.Text = "Ingreso Gymsoft - Versión " + Application.ProductVersion;
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
                    Listaterminal = JsonConvert.DeserializeObject<List<eTerminal>>(json);
                }
                //config = acsBll.GetLocalAccessControlSettings();

                if (config != null)
                {
                    if (config[0].intTiempoActualizaIngresos > 0)
                    {
                        timeUpdateEntries = config[0].intTiempoActualizaIngresos;
                    }

                    blnPermitirIngresoAdicional = config[0].blnPermiteIngresosAdicionales;

                    string strServesToOutput = System.Configuration.ConfigurationManager.AppSettings["servesToOutput"].ToString();
                    string strServesToInputAndOutput = System.Configuration.ConfigurationManager.AppSettings["servesToInputAndOutput"].ToString();
                    string strCardMode = System.Configuration.ConfigurationManager.AppSettings["cardMode"].ToString();
                    if (!string.IsNullOrEmpty(strServesToOutput)) { blnSirveParaSalida = Convert.ToBoolean(strServesToOutput); }
                    if (!string.IsNullOrEmpty(strServesToInputAndOutput)) { blnSirveParaEntradaySalida = Convert.ToBoolean(strServesToInputAndOutput); }
                    if (!string.IsNullOrEmpty(strCardMode)) { blnModoTarjeta = Convert.ToBoolean(strCardMode); }

                    blnAccesoDiscapacitados = config[0].bitAccesoDiscapacitados;
                    blnIngresoAbreDesdeTouch = config[0].bitIngresoAbreDesdeTouch;

                    if (!string.IsNullOrEmpty(config[0].strRutaNombreLogo))
                    {
                        nombreLogo = config[0].strRutaNombreLogo;
                    }

                    if (!string.IsNullOrEmpty(config[0].strPuertoCom) && config[0].strPuertoCom != "Seleccione")
                    {
                        strPuerto = config[0].strPuertoCom;
                    }

                    if (config[0].intVelocidadPuerto > 0)
                    {
                        intVelocidad = config[0].intVelocidadPuerto;
                    }

                    if (config[0].intTiempoPulso > 0)
                    {
                        intTiempoPulso = config[0].intTiempoPulso;
                    }

                    if (!string.IsNullOrEmpty(config[0].strPuertoComSalida) && config[0].strPuertoComSalida != "Seleccione")
                    {
                        strPuertoOut = config[0].strPuertoComSalida;
                    }

                    blnAntipassbackEntrada = config[0].bitAntipassbackEntrada;

                    if (!string.IsNullOrEmpty(config[0].strColorPpal))
                    {
                        strColorPpal = config[0].strColorPpal.Contains("#") ? config[0].strColorPpal : "#" + config[0].strColorPpal;
                    }

                    if (config[0].intTiempoParaLimpiarPantalla > 0)
                    {
                        intTiempoParaLimpiarPantalla = config[0].intTiempoParaLimpiarPantalla;
                    }

                    bitLectorSiempreEncendido = config[0].bitLectorBiometricoSiempreEncendido;

                    MostrarImagen = config[0].bitImagenSIBO;
                    if (MostrarImagen)
                    {
                        //picLogo.Visible = true;
                        panelPiePagina.Visible = true;
                    }
                    else
                    {
                        //picLogo.Visible = false;
                        panelPiePagina.Visible = false;
                    }
                }


                timerClearScreen = new System.Timers.Timer(intTiempoParaLimpiarPantalla * 1000);
                timerClearScreen.Elapsed += TimerClearScreen_Elapsed;

                picBanner.BackColor = ColorTranslator.FromHtml(strColorPpal);
                lblNombreApp.BackColor = ColorTranslator.FromHtml(strColorPpal);
                lblNombre.ForeColor = ColorTranslator.FromHtml(strColorPpal);
                lblPlan.ForeColor = ColorTranslator.FromHtml(strColorPpal);
                lblFechaVencimiento.ForeColor = ColorTranslator.FromHtml(strColorPpal);
                lblUltimoIngreso.ForeColor = ColorTranslator.FromHtml(strColorPpal);
                lblUltimaHora.ForeColor = ColorTranslator.FromHtml(strColorPpal);
                btnIngreso.BackColor = ColorTranslator.FromHtml(strColorPpal);
                btnSalida.BackColor = ColorTranslator.FromHtml(strColorPpal);
                cmdPlanesVig.BackColor = ColorTranslator.FromHtml(strColorPpal);
                lblVersion.Text = "Versión " + Application.ProductVersion.ToString();

                //Resolución incidente 0005472 (Mostrar logo gym) MToro
                if (!string.IsNullOrEmpty(nombreLogo))
                {
                    // Obtengo el directorio del servicio(i.e. \bin\Debug)
                    string workingDirectory = Environment.CurrentDirectory;
                    // obtengo el directorio del proyecto actual
                    string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
                    //picLogoEmpresa.ImageLocation = projectDirectory + "\\Resources\\" + nombreLogo;
                    //picLogoEmpresa.Visible = true;

                    string RutaLogoEmpresa = nombreLogo;
                    FileInfo ArchivoExist = new FileInfo(RutaLogoEmpresa);

                    FileInfo archivoLogo = new FileInfo(RutaLogoEmpresa);
                    if (archivoLogo.Exists)
                    {
                        picLogoEmpresa.ImageLocation = (archivoLogo.FullName);
                        picLogoEmpresa.Visible = true;
                    }
                    else
                    {
                        picLogoEmpresa.Image = null;
                    }
                }
                //FIN Resolución incidente 0005472


                //Invocamos el método que se encarga de ubicar el teclado en el ingreso
                UbicaTecladoNumerico();

                if (bitLectorSiempreEncendido)
                {
                    if (ZkPalmPv10M)
                    {
                        ActivarLectorPalmaZk("0");
                    }
                    else
                    {
                        if (!lectorCama && !lectorHuellaZk)
                        {
                            IniciarLector2();
                        }
                    }
                }
                if (lectorHuellaZk)
                {
                    IniciarLectorZk();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void validarArchivos()
        {
            if (lectorHuellaZk || ZkPalmPv10M)
            {
                FileInfo dllEx = new FileInfo(Application.StartupPath + @"\libzksensorcore.dll");
                if (dllEx.Exists)
                {
                    if (lectorHuellaZk) dllEx.Delete();
                }
                else
                {
                    if (ZkPalmPv10M)
                    {
                        dllEx = new FileInfo(Application.StartupPath + @"\DllzkPalma\libzksensorcore.dll");
                        if (dllEx.Exists)
                        {
                            dllEx.CopyTo(Application.StartupPath + @"\libzksensorcore.dll");
                        }
                    }
                }
                FileInfo dllExCore = new FileInfo(Application.StartupPath + @"\ZKIRPalmCore.dll");
                if (dllExCore.Exists)
                {
                    if (lectorHuellaZk) dllExCore.Delete();
                }
                else
                {
                    if (ZkPalmPv10M)
                    {
                        dllExCore = new FileInfo(Application.StartupPath + @"\DllzkPalma\ZKIRPalmCore.dll");
                        if (dllExCore.Exists)
                        {
                            dllExCore.CopyTo(Application.StartupPath + @"\ZKIRPalmCore.dll");
                        }
                    }
                }
                FileInfo dllExService = new FileInfo(Application.StartupPath + @"\ZKIRPalmService.dll");
                if (dllExService.Exists)
                {
                    if (lectorHuellaZk) dllExService.Delete();
                }
                else
                {
                    if (ZkPalmPv10M)
                    {
                        dllExService = new FileInfo(Application.StartupPath + @"\DllzkPalma\ZKIRPalmService.dll");
                        if (dllExService.Exists)
                        {
                            dllExService.CopyTo(Application.StartupPath + @"\ZKIRPalmService.dll");
                        }
                    }
                }
                FileInfo dllExAPI = new FileInfo(Application.StartupPath + @"\ZKPalmAPI.dll");
                if (dllExAPI.Exists)
                {
                    if (lectorHuellaZk) dllExAPI.Delete();
                }
                else
                {
                    if (ZkPalmPv10M)
                    {
                        dllExAPI = new FileInfo(Application.StartupPath + @"\DllzkPalma\ZKPalmAPI.dll");
                        if (dllExAPI.Exists)
                        {
                            dllExAPI.CopyTo(Application.StartupPath + @"\ZKPalmAPI.dll");
                        }
                    }
                }
                FileInfo dllExCap = new FileInfo(Application.StartupPath + @"\ZKPalmCap.dll");
                if (dllExCap.Exists)
                {
                    if (lectorHuellaZk) dllExCap.Delete();
                }
                else
                {
                    if (ZkPalmPv10M)
                    {
                        dllExCap = new FileInfo(Application.StartupPath + @"\DllzkPalma\ZKPalmCap.dll");
                        if (dllExCap.Exists)
                        {
                            dllExCap.CopyTo(Application.StartupPath + @"\ZKPalmCap.dll");
                        }
                    }
                }
            }
        }

        private void ActivarLectorPalmaZk(string strCodigo)
        {
            llenarPalmas(strCodigo);
            if (dtHuellas != null && dtHuellas.Rows.Count > 0)
            {
                m_hThreadCapture = new Thread(new ThreadStart(validarPalmas));
                m_hThreadCapture.Start();
            }
            else
            {
                clsShowMessage.Show("No se encontraron palmas relacionadas al id ingresado.", clsEnum.MessageType.Informa);
            }
        }

        private void validarPalmas()
        {
            m_bStop = false;
            clsLectorPalma objLectorP = new clsLectorPalma();
            objLectorP.m_bRegister = false;
            if (objLectorP.conectarDispositivo())
            {
                objLectorP.actualizarBaseDatos(dtHuellas);
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        lblMensajeAcceso.Text = string.Empty;
                        lblMensajeRestricion.Text = "Coloque la palma sobre el lector";
                        lblMensajeRestricion.ForeColor = Color.Blue;
                        this.Refresh();
                    }));
                }

                int intCantidadFallos = 0;
                while (!m_bStop)
                {
                    string strMensaje = objLectorP.DoVerify(ref picImgGuia);
                    if (strMensaje.IndexOf("Fallo") < 0)
                    {
                        strIdCliente = strMensaje;
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                ValidateEntryByUserId(strIdCliente);
                            }));
                        }
                        if (!bitLectorSiempreEncendido)
                        {
                            objLectorP.desconectarDispositivo();
                            return;
                        }
                    }
                    else
                    {
                        if (strMensaje.IndexOf("compara") > 0)
                        {
                            intCantidadFallos += 1;
                            if (!bitLectorSiempreEncendido && intCantidadFallos >= 5)
                            {
                                m_bStop = true;
                                Invoke(new Action(() =>
                                {
                                    lblMensajeAcceso.Text = string.Empty;
                                    lblMensajeRestricion.Text = "Error al validar la palma";
                                    lblMensajeRestricion.ForeColor = Color.Red;
                                    this.Refresh();
                                    objLectorP.desconectarDispositivo();
                                    Thread.Sleep(2000);
                                    lblMensajeRestricion.Text = "";
                                    lblMensajeRestricion.ForeColor = Color.Blue;
                                    this.Refresh();
                                    return;
                                }));
                            }
                        }
                    }
                    if (!m_bStop)
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                if (lblMensajeRestricion.Text.Length == 0)
                                {
                                    lblMensajeAcceso.Text = string.Empty;
                                    lblMensajeRestricion.Text = "Coloque la palma sobre el lector";
                                    lblMensajeRestricion.ForeColor = Color.Blue;
                                    this.Refresh();
                                }
                            }));
                        }
                    }
                    Thread.Sleep(10);
                }
                objLectorP.desconectarDispositivo();
            }
            else
            {
                MessageBox.Show(objLectorP.errorConexion);
            }
        }

        private void llenarPalmas(string strCodigo)
        {
           // FingerprintBLL fingerBll = new FingerprintBLL();
            //dtHuellas = fingerBll.GetAllFingerprints(strCodigo);
            List<eFingerprint> listFP = new List<eFingerprint>();
            listFP= fpAPI.GetAllFingerprintsPerson(gymId, strCodigo);
            dtHuellas = ConvertirListaEnTabla(listFP);


            DataTable dtHuellasAgrupadas = new DataTable();
            dtHuellasAgrupadas.Columns.Add("personId", typeof(System.String));
            dtHuellasAgrupadas.Columns.Add("fingerprint", typeof(System.Byte[]));

            dtHuellas.DefaultView.Sort = "personId, fingerprintId";
           
            string idAct = "0";
            byte[] datoPalma = new byte[8844];
            foreach (DataRowView fila in dtHuellas.DefaultView)
            {
                if (idAct != fila["personId"].ToString())
                {
                    idAct = fila["personId"].ToString();
                    datoPalma = new byte[8844];
                }
                if (fila["fingerprintId"].ToString() != "5")
                {
                    byte[] datobd = (byte[])fila["fingerprint"];
                    Array.Copy(datobd, 0, datoPalma, (int.Parse(fila["fingerprintId"].ToString()) - 1) * 2000, 2000);
                }
                else
                {
                    byte[] datobd = (byte[])fila["fingerprint"];
                    Array.Copy(datobd, 0, datoPalma, (int.Parse(fila["fingerprintId"].ToString()) - 1) * 2000, 844);
                    dtHuellasAgrupadas.Rows.Add(idAct, datoPalma);
                }
            }
            dtHuellas = dtHuellasAgrupadas;
        }

        private void TimerClearScreen_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerClearScreen.Enabled = false;
            timerClearScreen.Stop();

            Clear();
        }

        /// <summary>
        /// Método que se encarga de ubicar el teclado numérico.
        /// Getulio Vargas - 2018-09-04 - OD 1307
        /// </summary>
        private void UbicaTecladoNumerico()
        {
            try
            {
                int X = 5, Y = 3;
                Random rnd = new Random();
                int tipoPantalla = rnd.Next(1, 6);

                switch (tipoPantalla)
                {
                    case 1:
                    case 2:
                        Y = 3;
                        break;
                    case 3:
                    case 4:
                        Y = 100;
                        break;
                    case 5:
                    case 6:
                        Y = 155;
                        break;
                    default:
                        Y = 3;
                        break;
                }

                if (tipoPantalla % 2 == 0)
                {
                    this.splitPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.splitPrincipal.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
                    this.splitPrincipal.Panel1.Controls.Add(this.gpbDatosIngreso);
                    this.splitPrincipal.Panel2.Controls.Add(this.ctrlTecladoIngreso);
                    this.splitPrincipal.Panel2.Controls.Add(this.panelOcultar);
                    this.ctrlTecladoIngreso.Location = new Point(X, Y);
                    this.splitPrincipal.SplitterDistance = Screen.PrimaryScreen.Bounds.Width - 280;
                }
                else
                {
                    this.splitPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.splitPrincipal.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
                    this.splitPrincipal.Panel2.Controls.Add(this.gpbDatosIngreso);
                    this.splitPrincipal.Panel1.Controls.Add(this.ctrlTecladoIngreso);
                    this.splitPrincipal.Panel1.Controls.Add(this.panelOcultar);
                    this.ctrlTecladoIngreso.Location = new Point(X, Y);
                    this.splitPrincipal.SplitterDistance = 280;
                }
            }
            catch (Exception)
            {
                this.splitPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
                this.splitPrincipal.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
                this.splitPrincipal.Panel1.Controls.Add(this.gpbDatosIngreso);
                this.splitPrincipal.Panel2.Controls.Add(this.ctrlTecladoIngreso);
                this.ctrlTecladoIngreso.Location = new Point(17, 3);
                this.splitPrincipal.SplitterDistance = Screen.PrimaryScreen.Bounds.Width - 280;
            }
        }

        private void CtrlTecladoIngreso_ctrlOk(string strValue)
        {
            try
            {
                if (strValue.Length > 0)
                {
                    if (!blnModoTarjeta)
                    {
                        ValidateEntryByUserId(strValue);
                    }
                    else
                    {
                        ValidateEntryByUserId("T-" + strValue);
                    }
                }

                ctrlTecladoIngreso.swKeyboard = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ValidateEntryByUserId(string id)
        {
            try
            {
                eResponseWhiteList resp = new eResponseWhiteList();
                eResponseWhiteList respuesta = new eResponseWhiteList();
                string originalId = string.Empty;
                clsWhiteList wlCLS = new clsWhiteList();
                List<eClientMessages> cmList = new List<eClientMessages>();
                ClientMessagesBLL cmBll = new ClientMessagesBLL();

                if (id.StartsWith("T-"))
                {
                    originalId = id;
                    id = originalId.Substring(2);
                    id = GetClientIdByCardId(id);

                    if (string.IsNullOrEmpty(id))
                    {
                        id = originalId.Substring(2);
                        id = GetEmployeeIdByCardId(id);
                    }
                }

                if (id.Length == 13)
                {
                    //swWithoutFingerprint = true;
                    id = id.Substring(1);
                }
                else if (id.Length == 12)
                {
                    //swWithoutFingerprint = false;
                    id = GetClientIdByFingerprintId(Convert.ToInt32(id));
                }

                if (id.Trim().Length == 0)
                {
                    return;
                }

                cmList = cmBll.GetLocalCLientMessages();
                string messageToClient = string.Empty, messageClientId = string.Empty, messageType = "0", messageClientOrder = string.Empty;
                int timeMessageClient = 0;

                if (cmList != null && cmList.Count > 0)
                {
                    foreach (eClientMessages item in cmList)
                    {
                        if (!string.IsNullOrEmpty(item.messageText))
                        {
                            messageToClient = messageToClient + " " + item.messageText;
                        }

                        timeMessageClient = item.messageDurationTime;
                        messageClientId = item.messageId.ToString();
                        messageType = item.messageType;
                        messageClientOrder = item.messageImgOrder;
                    }
                }

                eConfiguration config = new eConfiguration();
                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                config = acsBll.GetLocalAccessControlSettings();

                if (config.bitMensajeCumpleanos)
                {
                    WhiteListBLL wlBll = new WhiteListBLL();
                    if (wlBll.CumpleAniosCliente(id))
                    {
                        messageToClient += " ¡Feliz Cumpleaños!";
                    }
                }

                if (id.Trim().Length == 3)
                {
                    if (!(lectorCama || ZkPalmPv10M))
                    {
                        if (lectorHuellaZk)
                        {
                            if (!bitLectorSiempreEncendido)
                            {
                                ObtenerTemplateZk(id);
                                if (dtHuellas != null && dtHuellas.Rows.Count > 0)
                                {
                                    bIsTimeToDie = false;
                                    Thread captureThread = new Thread(new ThreadStart(DoCapture));
                                    captureThread.IsBackground = true;
                                    captureThread.Start();
                                }
                            }
                        }
                        else
                        {
                            ObtenerTemplate(id);

                            if (dtHuellas != null && dtHuellas.Rows.Count > 0)
                            {
                                if (LectorStart())
                                {
                                    id = strIdCliente;
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                clsShowMessage.Show("No se encontraron huellas relacionadas al id ingresado.", clsEnum.MessageType.Informa);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (!bitLectorSiempreEncendido)
                        {
                            if (ZkPalmPv10M)
                            {
                                ActivarLectorPalmaZk(id);
                                return;
                            }
                        }
                    }
                }
                else if (id.Trim().Length < 3)
                {
                    clsShowMessage.Show("Debe ingresar al menos 3 dígitos.", clsEnum.MessageType.Informa);
                    return;
                }

                if (blnSirveParaEntradaySalida)
                {
                    try
                    {
                        FrmEntryOrExit frmEntryOrExit = new FrmEntryOrExit();
                        frmEntryOrExit.ShowDialog();
                        string entry = frmEntryOrExit.entry;

                        if (string.IsNullOrEmpty(entry))
                        {
                            clsShowMessage.Show("Respuesta no válida", clsEnum.MessageType.Informa);
                            return;
                        }

                        if (entry == "Entry")
                        {
                            if (blnAntipassbackEntrada)
                            {
                                //Validamos si la persona tiene una entrada anterior SIN SALIDA, en caso de ser así se bloquea el ingreso de esta.
                                //Funcionalidad basada en el parámetro "blnAntipassbackEntrada"
                                if (!wlCLS.ValidateEntryWithoutExit(id))
                                {
                                    return;
                                }
                            }

                            if (!blnAntipassbackEntrada)
                            {
                                respuesta = wlCLS.ValidateEntryByUserId(id, true);
                            }
                        }
                        else if (entry == "Exit")
                        {
                            //Validamos que el usuario pueda salir sin problema.
                            respuesta = wlCLS.ValidateExitById(id, true);
                        }
                        else
                        {
                            clsShowMessage.Show("Respuesta no válida", clsEnum.MessageType.Informa);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
                //FIN MToro
                else if (!blnSirveParaSalida)
                {
                    if (blnAntipassbackEntrada)
                    {
                        //Validamos si la persona tiene una entrada anterior SIN SALIDA, en caso de ser así se bloquea el ingreso de esta.
                        //Funcionalidad basada en el parámetro "blnAntipassbackEntrada"
                        if (!wlCLS.ValidateEntryWithoutExit(id))
                        {
                            return;
                        }
                    }

                    if (!blnAntipassbackEntrada)
                    {
                        respuesta = wlCLS.ValidateEntryByUserId(id, true);
                    }
                }
                else
                {
                    //Validamos que el usuario pueda salir sin problema.
                    respuesta = wlCLS.ValidateExitById(id, true);
                }

                //Seteamos los datos del cliente en los labels correspondientes
                if (respuesta.whiteList != null)
                {
                    if (respuesta.response.messageThree != "Vuelva pronto")
                    {
                        lblUltimoIngreso.Text = "Fecha hora ingreso: " + (respuesta.whiteList.lastEntry == null ? "" : Convert.ToDateTime(respuesta.whiteList.lastEntry).ToString("yyyy-MM-dd"));
                        lblUltimaHora.Text = "Hora: " + (respuesta.whiteList.lastEntry == null ? "" : Convert.ToDateTime(respuesta.whiteList.lastEntry).ToString("hh:mm"));
                        lblNombre.Text = "Cliente: " + respuesta.whiteList.name;
                        lblPlan.Text = "Plan: " + respuesta.whiteList.planName;
                        lblFechaVencimiento.Text = "Fecha de vencimiento: " + (respuesta.whiteList.expirationDate == null ? "" : Convert.ToDateTime(respuesta.whiteList.expirationDate).ToString("yyyy-MM-dd"));

                    }
                }

                //messageToClient
                if (respuesta.response.state)
                {
                    lblMensajeRestricion.Text = respuesta.response.messageThree + "\n" + messageToClient;//SA
                    lblMensajeAcceso.Text = respuesta.response.messageTwo;
                    lblMensajeRestricion.ForeColor = Color.Blue;
                    lblMensajeAcceso.ForeColor = Color.Green;

                    //Mostramos la imagen verde como ingreso o salida satisfactorio
                    picBolaVerde.Visible = true;
                    picBolaRoja.Visible = false;

                    //Generamos el comando de apertura, sea para salida o para entrada
                    if (blnSirveParaSalida)
                    {
                        //ComandoMemoriaEnviar(strPuertoOut);
                    }
                    else
                    {
                        //ComandoMemoriaEnviar(strPuerto);
                    }
                }
                else
                {
                    //Mostramos la imagen roja como ingreso o salida no satisfactorio
                    picBolaVerde.Visible = false;
                    picBolaRoja.Visible = true;

                    lblMensajeRestricion.Text = respuesta.response.messageThree;
                    lblMensajeAcceso.Text = respuesta.response.messageTwo;
                    lblMensajeRestricion.ForeColor = Color.Blue;
                    lblMensajeAcceso.ForeColor = Color.Red;
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
        /// Método que 
        /// </summary>
        /// <returns></returns>
        private bool LectorStart()
        {
            try
            {
                if (!usLectorUSB.IsSensorOn)
                {
                    usLectorManagenUSB.Init();
                }

                if (usLectorManagenUSB.Scanners.Count > 0)
                {
                    usLectorUSB = usLectorManagenUSB.Scanners[0];
                    return verifyFingerprint();
                }
                else
                {
                    clsShowMessage.Show("No hay lectores de huella conectados", clsEnum.MessageType.Informa);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IniciarLector2()
        {
            try
            {
                if (!usLectorUSB.IsSensorOn)
                {
                    usLectorManagenUSB.Init();
                }
                if (usLectorManagenUSB.Scanners.Count > 0)
                {
                    usLectorUSB = usLectorManagenUSB.Scanners[0];
                    ObtenerTemplate("0");
                    Thread hiloLectura = new Thread(new ThreadStart(LeerHuella2));
                    hiloLectura.Start();
                    return true;
                }
                else
                {
                    clsShowMessage.Show("No hay lectores de huella conectados", clsEnum.MessageType.Informa);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LeerHuella2()
        {
            try
            {
                UFS_STATUS Estado = new UFS_STATUS();
                usLectorUSB.Timeout = 0;
                usLectorUSB.Sensitivity = 7;
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        lblMensajeAcceso.Text = string.Empty;
                        lblMensajeRestricion.Text = "Coloque el dedo en el lector de huella";
                        lblMensajeRestricion.ForeColor = Color.Blue;
                        this.Refresh();
                    }));
                }

                Estado = usLectorUSB.ClearCaptureImageBuffer();
                Estado = usLectorUSB.CaptureSingleImage();

                if (Estado == UFS_STATUS.OK)
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            this.ImagenCapturada = CapturaImagen();
                            this.HuellaBytes = ExtraeHuella();

                            verifyFingerprint();
                            ValidateEntryByUserId(strIdCliente);
                            strIdCliente = "";
                        }));
                    }
                    Thread.Sleep(intTiempoParaLimpiarPantalla * 1000);
                    IniciarLector2();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que sirve para leer la huella del usuario y validar si esta coincide con alguna de las registradas en la BD.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <returns></returns>
        private bool verifyFingerprint()
        {
            try
            {
                bool blncomparacion = false;

                if (!ReadFingerprint())
                {
                    Clear();
                    lblMensajeRestricion.Text = "Debe colocar el dedo en lector de huella";
                    lblMensajeRestricion.ForeColor = Color.Red;
                }

                blncomparacion = IdentificarHuella();

                if (!blncomparacion)
                {
                    ReadFingerprint();
                    blncomparacion = IdentificarHuella();
                }

                if (blncomparacion)
                {
                    strIdCliente = dtHuellas.Rows[index]["id"].ToString();
                }
                else
                {
                    lblMensajeAcceso.ForeColor = Color.Red;
                    lblMensajeRestricion.Text = "Huella no coincide";
                    //Mostramos la imagen roja como ingreso o salida no satisfactorio
                    picBolaVerde.Visible = false;
                    picBolaRoja.Visible = true;

                    if (blnSirveParaSalida)
                    {
                        lblMensajeAcceso.Text = "No puede salir";
                    }
                    else
                    {
                        lblMensajeAcceso.Text = "No puede ingresar";
                    }
                }

                return blncomparacion;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que se encarga de comparar si una huella existe en una lista de huellas previamente llenada
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <returns></returns>
        private bool IdentificarHuella()
        {
            try
            {
                UFM_STATUS ufm_res = new UFM_STATUS();

                if (bdTemplate != null)
                {
                    ufm_res = usComparadorUSB.Identify(HuellaBytes, intTamanoImagen, bdTemplate, dbTemplateSize, bdTemplate.Length, 5000, out index);

                    if (ufm_res == UFM_STATUS.OK)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Clear()
        {

            try
            {
                //Invocación de delegado requerida para el acceso
                //a los controles del formulario desde diferentes hilos -- MToro
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {

                        lblNombre.Text = "Nombre Cliente:";
                        lblUltimoIngreso.Text = "Último ingreso:";
                        lblUltimaHora.Text = "Hora:";
                        lblPlan.Text = "Plan:";
                        lblFechaVencimiento.Text = "Fecha Vencimiento:";
                        lblMensajeAcceso.Text = String.Empty;
                        lblMensajeRestricion.Text = String.Empty;
                        picBolaRoja.Visible = true;
                        picBolaVerde.Visible = false;
                        picFoto.Image = null;

                    }));//fin invoke
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ReadFingerprint()
        {
            try
            {
                UFS_STATUS Estado = new UFS_STATUS();
                usLectorUSB.Timeout = 10000;
                usLectorUSB.Sensitivity = 7;
                lblMensajeAcceso.Text = string.Empty;
                lblMensajeRestricion.Text = "Coloque el dedo en el lector de huella";
                lblMensajeRestricion.ForeColor = Color.Blue;
                this.Refresh();
                Estado = usLectorUSB.ClearCaptureImageBuffer();
                Estado = usLectorUSB.CaptureSingleImage();
                lblMensajeRestricion.Text = string.Empty;
                this.Refresh();

                if (Estado == UFS_STATUS.OK)
                {
                    this.ImagenCapturada = CapturaImagen();
                    this.HuellaBytes = ExtraeHuella();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private byte[] ExtraeHuella()
        {
            try
            {
                int tamanoTMP, calidadHuella;
                byte[] bytesTMP = new byte[intTamanoImagen];
                usLectorUSB.Extract(bytesTMP, out tamanoTMP, out calidadHuella);
                return bytesTMP;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Bitmap CapturaImagen()
        {
            try
            {
                Bitmap Imagenhuella;
                int Resolucion = 0;
                usLectorUSB.GetCaptureImageBuffer(out Imagenhuella, out Resolucion);
                return Imagenhuella;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ObtenerTemplate(string id)
        {
            try
            {
                WhiteListBLL wlBll = new WhiteListBLL();
                dtHuellas = wlBll.GetFingerprintsById(id);
                bdTemplate = new byte[0][];
                byte[][] aux;
                dbTemplateSize = new int[0];

                if (dtHuellas != null && dtHuellas.Rows.Count > 0)
                {
                    int i = 0;
                    foreach (DataRow row in dtHuellas.Rows)
                    {
                        if (bdTemplate == null)
                        {
                            bdTemplate = new byte[1][];
                            dbTemplateSize = new int[1];
                        }
                        else
                        {
                            aux = new byte[bdTemplate.Length][];
                            Array.Copy(bdTemplate, aux, bdTemplate.Length);
                            bdTemplate = new byte[bdTemplate.Length + 1][];
                            Array.Copy(aux, bdTemplate, aux.Length);
                            dbTemplateSize = new int[dbTemplateSize.Length + 1];
                        }

                        byte[] tmpHuella = (byte[])row["fingerprint"];
                        bdTemplate[i] = tmpHuella;
                        dbTemplateSize[i] = tmpHuella.Length;
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ObtenerTemplateZk(string id)
        {
            try
            {
                WhiteListBLL wlBll = new WhiteListBLL();
                //dtHuellas = wlBll.GetFingerprintsById(id);

                 List<eFingerprint> listFP = new List<eFingerprint>();
                 listFP= fpAPI.GetAllFingerprintsPerson(gymId, id);
                    dtHuellas = ConvertirListaEnTabla(listFP);

                zkfp2.DBClear(mDBHandle);
                if (dtHuellas != null && dtHuellas.Rows.Count > 0)
                {
                    int i = 1;
                    foreach (DataRow row in dtHuellas.Rows)
                    {
                        RegTmp = (byte[])row["fingerprint"];
                        zkfp2.DBAdd(mDBHandle, i, RegTmp);
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ComandoMemoriaEnviar(string COMPort)
        {
            try
            {
                if (blnIngresoAbreDesdeTouch)
                {
                    OpenDoorPulse(COMPort);
                }
                else
                {
                    //Hacer que lo habra el monitor
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void OpenDoorPulse(string COMPort)
        {
            try
            {
                SerialPort_Entrada.PortName = COMPort;
                SerialPort_Entrada.BaudRate = intVelocidad;
                SerialPort_Entrada.Open();
                SerialPort_Entrada.Write("X" + intTiempoPulso.ToString());
                SerialPort_Entrada.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método encargado de consultar la identificación del cliente de acuerdo con el número de tarjeta de este.
        /// Getulio Vargas - 2018-08-28 - OD 1307
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        private string GetClientIdByCardId(string cardId)
        {
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
                throw ex;
            }
        }

        /// <summary>
        /// Método para retornar la identificación de un cliente partiendo del código de la tarjeta asociado.
        /// Getulio Vargas - 2018-08-27 - OD 1307
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public string GetEmployeeIdByCardId(string cardId)
        {
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
                throw ex;
            }
        }

        public string GetClientIdByFingerprintId(int id)
        {
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
                throw ex;
            }
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

        public static DataTable ConvertirListaEnTabla(List<eFingerprint> lista)
        {
            DataTable tabla = new DataTable();

            tabla.Columns.Add("recordId", typeof(int));
            tabla.Columns.Add("id", typeof(int));
            tabla.Columns.Add("fingerPrint", typeof(byte[]));
            tabla.Columns.Add("gymId", typeof(int));
            tabla.Columns.Add("finger", typeof(int));
            tabla.Columns.Add("quality", typeof(int));
            tabla.Columns.Add("branchId", typeof(int));
            tabla.Columns.Add("personId", typeof(string));
            tabla.Columns.Add("typePerson", typeof(string));
            tabla.Columns.Add("planId", typeof(string));
            tabla.Columns.Add("restrictions", typeof(string));
            tabla.Columns.Add("reserveId", typeof(string));
            tabla.Columns.Add("cardId", typeof(string));
            tabla.Columns.Add("withoutFingerprint", typeof(bool));
            tabla.Columns.Add("intIndiceHuellaActual", typeof(string));


            foreach (eFingerprint persona in lista)
            {
                tabla.Rows.Add(persona.recordId, persona.id, persona.fingerPrint, persona.gymId, persona.finger, persona.quality, persona.branchId, persona.personId, persona.typePerson, persona.planId, persona.restrictions, persona.reserveId, persona.cardId, persona.withoutFingerprint, persona.intIndiceHuellaActual);
            }
            return tabla;

        }
    }
}
