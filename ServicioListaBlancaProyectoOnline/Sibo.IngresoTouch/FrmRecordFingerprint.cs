﻿using Sibo.WhiteList.IngresoTouch.Classes;
using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.BLL.Helpers;
using Sibo.WhiteList.Service.Entities.Classes;
using Suprema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using Sample;
using libzkfpcsharp;

namespace Sibo.WhiteList.IngresoTouch
{
    public partial class FrmRecordFingerprint : Form
    {
        public bool blnTCAM7000 = false;
        public string clientId = string.Empty;
        public int fingerId = 0, gymId = 0, branchId = 0;
        bool blnFirmarContratoAlEnrolar = false, blnNoValidarHuella = false, recordFingerPrint = false, sw = false;
        int qualityFingerprintConfig = 0, timeOutLector = 10000, quality = 0;
        UFScanner usbLector;
        UFScannerManager usbLectorManager;
        UFMatcher matcher;
        IntPtr mDevHandle = IntPtr.Zero;
        IntPtr mDBHandle = IntPtr.Zero;
        IntPtr FormHandle = IntPtr.Zero;
        int RegisterCount = 0;
        int cbRegTmp = 0;
        int iFid = 1;
        byte[][] RegTmps = new byte[3][];
        byte[] RegTmp = new byte[2000];

        private int mfpWidth = 0;
        private int mfpHeight = 0;

        byte[] FPBuffer;
        bool bIsTimeToDie = false;
        int cbCapTmp = 2000;
        byte[] CapTmp = new byte[2000];

        const int MESSAGE_CAPTURED_OK = 0x0400 + 6;

        bool IsRegister = false;
        bool bIdentify = false;
        const int REGISTER_FINGER_COUNT = 3;
        Boolean lectorHuellaZk = false;
        Boolean lectorCama = false;
        Boolean ZkPalmPv10M = false;

        private void FrmRecordFingerprint_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                FrmIngreso frmEntry = new FrmIngreso();
                frmEntry.strIdClienteGrabaHuella = txtDocument.Text;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void txtDocument_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (!string.IsNullOrEmpty(txtDocument.Text))
                {
                    if (ZkPalmPv10M || ValidateRecordFingerprint(txtDocument.Text))
                    {
                        clientId = txtDocument.Text;

                        if (!lectorHuellaZk)
                        {
                            if (blnTCAM7000)
                            {
                                DialogResult = DialogResult.OK;
                            }
                            else
                            {
                                if (lectorCama)
                                {
                                    eConfiguration config = new eConfiguration();
                                    AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                                    FingerprintBLL fingerBll = new FingerprintBLL();
                                    config = acsBll.GetLocalAccessControlSettings();

                                    var bitFirmaContratoEnrolar = false;
                                    bitFirmaContratoEnrolar = config.bitFirmarContratoAlEnrolar;

                                    string cadenaConexion = ConfigurationManager.ConnectionStrings["strConnection"].ConnectionString;
                                    Process objProceso = new Process();
                                    objProceso.StartInfo.Arguments = txtDocument.Text + " | " + cadenaConexion + " | 0 | " + gymId + " | " + branchId + " | " + Convert.ToString(bitFirmaContratoEnrolar);
                                    objProceso.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["RutaLectorHuellaCama"].ToString();
                                    objProceso.Start();
                                    while (!objProceso.HasExited)
                                    {
                                        System.Threading.Thread.Sleep(200);
                                    }

                                    btnClose.PerformClick();
                                    clsShowMessage.Show("Proceso realizo con éxito.", clsEnum.MessageType.Informa);
                                }
                                else
                                {
                                    if (ZkPalmPv10M)
                                    {
                                        clsLectorPalma objLectorP = new clsLectorPalma();
                                        objLectorP.m_bRegister = true;
                                        if (objLectorP.conectarDispositivo())
                                        {
                                            bool bProceso = true;
                                            while (bProceso)
                                            {
                                                string strMensaje = objLectorP.DoRegister(ref picPhoto);
                                                lblText.Text = strMensaje;
                                                if (strMensaje.IndexOf("satisfactoriamente") >= 0 || strMensaje.IndexOf("erronea") >= 0)
                                                {
                                                    bProceso = false;
                                                }
                                                Application.DoEvents();
                                                Thread.Sleep(10);
                                            }
                                            objLectorP.desconectarDispositivo();
                                            if (objLectorP.regExitoso != null)
                                            {
                                                quality = 90;
                                                EventBLL entryBll = new EventBLL();
                                                clsWhiteList wlCLS = new clsWhiteList();
                                                FingerprintBLL fingerBll = new FingerprintBLL();
                                                clsFingerprint fingerCLS = new clsFingerprint();
                                                //Insertamos la huella del cliente para tenerla en cuenta y actualizarla en la(s) terminal(es)
                                                for (int i = 0; i < 5; i++)
                                                {
                                                    fingerId = i + 1;
                                                    int tamano = 2000;
                                                    if (((i * 2000) + tamano) > objLectorP.regExitoso.Length)
                                                    {
                                                        tamano = objLectorP.regExitoso.Length - (i * 2000);
                                                    }
                                                    Array.Copy(objLectorP.regExitoso, i * 2000, fingerprintImage, 0, tamano);
                                                    if (fingerBll.ValidateFingerprintToRecordPalma(gymId, branchId, clientId, fingerId, fingerprintImage, quality, false))
                                                    {
                                                        fingerCLS.Insert(clientId, "", fingerId, fingerprintImage, quality);
                                                    }
                                                }

                                                //Insertamos el registro de enrolamiento en los eventos
                                                entryBll.Insert("", clientId, "", 0, 0, "", false, 0, null, null, false, "Huella grabada", "", "La huella del usuario fue grabada de forma correcta", true,
                                                                "", "Enroll", string.Empty);
                                                DialogResult = DialogResult.OK;
                                                this.Close();
                                            }
                                        }
                                        else
                                        {
                                            lblText.Text = objLectorP.errorConexion;
                                        }
                                    }
                                    else
                                    {
                                        //LectorStart();
                                        backWork = new BackgroundWorker();
                                        backWork.DoWork += BackWork_DoWork;
                                        backWork.RunWorkerCompleted += BackWork_RunWorkerCompleted;
                                        backWork.WorkerSupportsCancellation = true;
                                        backWork.RunWorkerAsync();
                                    }
                                }
                            }
                        }
                        else
                        {
                            InicialLectorZk();
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    clsShowMessage.Show("Debe ingresar el número de identificación del usuario al que le desea grabar la huella.", clsEnum.MessageType.Informa);
                    return;
                }
            }
        }

        private void BackWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                EventBLL entryBll = new EventBLL();
                bool closeWindow = true;
                clsWhiteList wlCLS = new clsWhiteList();
                clsFingerprint fingerCLS = new clsFingerprint();
                FingerprintBLL fingerBll = new FingerprintBLL();

                if (recordFingerPrint && sw)
                {
                    //Validamos por medio de la API si esta huella se puede grabar; es decir, si no está registrada para otro usuario en la BD.
                    if (fingerBll.ValidateFingerprintToRecord(gymId, branchId, clientId, fingerId, fingerprintImage, quality, false,"suprema"))
                    {
                        //Insertamos la huella del cliente para tenerla en cuenta y actualizarla en la(s) terminal(es)
                        if (fingerCLS.Insert(clientId, "", fingerId, fingerprintImage, quality))
                        {
                            //Validamos si se debe firmar contrato al enrolar
                            eConfiguration config = new eConfiguration();
                            AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                            config = acsBll.GetLocalAccessControlSettings();

                            var bitFirmaContratoEnrolar = false;
                            bitFirmaContratoEnrolar = config.bitFirmarContratoAlEnrolar;
                            if (bitFirmaContratoEnrolar)
                            {
                                fingerBll.firmarContratoGSW(gymId, branchId, clientId, fingerId, fingerprintImage, quality, false);

                            }

                            //Actualizamos la lista blanca local
                            wlCLS.UpdateFingerprint(gymId, branchId, clientId, fingerId, fingerprintImage);
                            //Insertamos el registro de enrolamiento en los eventos
                            entryBll.Insert("", clientId, "", 0, 0, "", false, 0, null, null, false, "Huella grabada", "", "La huella del usuario fue grabada de forma correcta", true,
                                            "", "Enroll", string.Empty);
                            lblText.Text = "Registro de huella exitoso";
                        }
                    }
                    else
                    {
                        //Insertamos el registro de enrolamiento en los eventos
                        entryBll.Insert("", clientId, "", 0, 0, "", false, 0, null, null, false, "No es posible enrolar el usuario", "", "La huella ya existe", false, "", "Enroll",
                                        string.Empty);
                        lblText.Text = "La huella ya existe, registre otro dedo";
                    }
                }
                else
                {
                    try
                    {
                        if (usbLectorManager.Scanners.Count > 0)
                        {
                            if (quality < qualityFingerprintConfig)
                            {
                                lblText.Text = "Calidad de huella deficiente. Por favor reintente.";
                                sw = false;
                                recordFingerPrint = false;
                                closeWindow = false;
                            }
                            else
                            {
                                lblText.Text = "(Tiempo expiró) Registre de nuevo la huella";
                            }

                            lblClientName.Text = string.Empty;
                            txtDocument.Text = string.Empty;
                        }
                        else
                        {
                            lblText.Text = "No hay lector conectado";
                        }
                    }
                    catch (Exception) { }
                }

                Refresh();
                recordFingerPrint = false;
                System.Threading.Thread.Sleep(3000);
                backWork.CancelAsync();
                LectorClose();

                if (closeWindow)
                {
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    System.Threading.Thread.Sleep(1000);
                    lblText.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                LectorClose();
                //LectorStart();
                lblText.Text = "Error al Guardar, Reintente";
                clsShowMessage.Show(ex.Message, clsEnum.MessageType.Error);
            }
        }

        /// <summary>
        /// Método que se encarga de capturar la imagen de la huella y guardarla en la ruta raíz del ingreso.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <returns></returns>
        private Bitmap CaptureImage()
        {
            try
            {
                Bitmap image;
                int resolution = 0;
                usbLector.GetCaptureImageBuffer(out image, out resolution);
                usbLector.SaveCaptureImageBufferToBMP(Application.StartupPath + "\\Fingerprint.bmp");
                return image;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BackWork_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (!recordFingerPrint)
                {
                    if (!sw)
                    {
                        ReadFingerprint();

                        if (state == UFS_STATUS.OK)
                        {
                            lblQualityFingerprintValue.Text = quality.ToString();
                            capturedImage = CaptureImage();

                            if (quality < qualityFingerprintConfig)
                            {
                                sw = false;
                                recordFingerPrint = false;
                            }
                            else
                            {
                                sw = true;
                                recordFingerPrint = true;
                            }

                            return;
                        }
                        else
                        {
                            fingerprintImage = null;
                        }

                        sw = true;
                        recordFingerPrint = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        UFS_STATUS state;
        Bitmap capturedImage;
        private const int maxSizeTemplate = 2000, maxNumberTemplate = 10;

        private void FrmRecordFingerprint_Load(object sender, EventArgs e)
        {
            try
            {
                //if (!lectorHuellaZk)
                //{
                //    usbLectorManager = new UFScannerManager(this);
                //}
                //else
                //{
                if (lectorHuellaZk)
                {
                    FormHandle = this.Handle;
                    int ret = zkfperrdef.ZKFP_ERR_OK;
                    if ((ret = zkfp2.Init()) == zkfperrdef.ZKFP_ERR_OK)
                    {
                        int nCount = zkfp2.GetDeviceCount();
                        if (nCount == 0)
                        {
                            zkfp2.Terminate();
                            lblText.Text = "No hay lector conectado";
                        }
                        ret = zkfp.ZKFP_ERR_OK;
                        if (IntPtr.Zero == (mDevHandle = zkfp2.OpenDevice(0)))
                        {
                            lblText.Text = "No se puede conectar el dispositivo";
                            return;
                        }
                        if (IntPtr.Zero == (mDBHandle = zkfp2.DBInit()))
                        {
                            lblText.Text = "No se pudo iniciar la base de datos";
                            zkfp2.CloseDevice(mDevHandle);
                            mDevHandle = IntPtr.Zero;
                            return;
                        }
                        RegisterCount = 0;
                        cbRegTmp = 0;
                        iFid = fingerId;
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

                        Thread captureThread = new Thread(new ThreadStart(DoCapture));
                        captureThread.IsBackground = true;
                        captureThread.Start();
                        bIsTimeToDie = false;
                    }
                    else
                    {
                        MessageBox.Show("No se pudo inicializar el lector zk, ret=" + ret + " !");
                    }
                }
                //}
                lblClientName.Text = string.Empty;
                lblQualityFingerprintValue.Text = "0";
                eConfiguration config = new eConfiguration();
                eTerminal terminal = new eTerminal();
                TerminalBLL terminalBll = new TerminalBLL();
                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                this.Text = "Ingreso Gymsoft - Versión " + Application.ProductVersion;
                config = acsBll.GetLocalAccessControlSettings();

                if (config != null)
                {
                    qualityFingerprintConfig = config.intCalidadHuella;
                    blnFirmarContratoAlEnrolar = config.bitFirmarContratoAlEnrolar;
                    blnNoValidarHuella = config.bitNoValidarHuella;
                    timeOutLector = config.intTimeOutLector * 1000;
                }

                lblAcceptableFingerprintQualityValue.Text = qualityFingerprintConfig.ToString();
                Control.CheckForIllegalCrossThreadCalls = false;
                if (ZkPalmPv10M)
                {
                    lblAcceptableFingerprintQuality.Visible = false;
                    lblAcceptableFingerprintQualityValue.Visible = false;
                    lblQualityFingerprint.Visible = false;
                    lblQualityFingerprintValue.Visible = false;
                    lblData.Visible = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FrmRecordFingerprint_Activated(object sender, EventArgs e)
        {
            txtDocument.SelectAll();
            txtDocument.Focus();
        }

        private BackgroundWorker backWork;
        byte[] fingerprintImage = new byte[maxSizeTemplate];

        public FrmRecordFingerprint(ref UFScanner usLectorUSBIng, ref UFScannerManager usLectorManagenUSBIng, ref UFMatcher usComparadorUSBIng)
        {
            InitializeComponent();

            usbLector = usLectorUSBIng;
            usbLectorManager = usLectorManagenUSBIng;
            matcher = usComparadorUSBIng;

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
            lectorHuellaZk = Boolean.Parse(parHuellaZK);

            if (!lectorCama && !lectorHuellaZk && !ZkPalmPv10M)
            {
                if (usLectorUSBIng.ID == null)
                {
                    LectorStart();
                }
            }
        }

        private void ctrlTecladoHuella_ctrlOk(string strValue)
        {
            try
            {
                if (strValue.Length > 0)
                {
                    txtDocument.Text = strValue;

                    if (ZkPalmPv10M || ValidateRecordFingerprint(txtDocument.Text))
                    {
                        clientId = txtDocument.Text;

                        if (!lectorHuellaZk)
                        {
                            if (blnTCAM7000)
                            {
                                DialogResult = DialogResult.OK;
                            }
                            else
                            {
                                if (lectorCama)
                                {
                                    eConfiguration config = new eConfiguration();
                                    AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                                    FingerprintBLL fingerBll = new FingerprintBLL();
                                    config = acsBll.GetLocalAccessControlSettings();

                                    var bitFirmaContratoEnrolar = false;
                                    bitFirmaContratoEnrolar = config.bitFirmarContratoAlEnrolar;

                                    string cadenaConexion = ConfigurationManager.ConnectionStrings["strConnection"].ConnectionString;
                                    Process objProceso = new Process();
                                    objProceso.StartInfo.Arguments = txtDocument.Text + " | " + cadenaConexion + " | 0 | " + gymId + " | " + branchId + " | " + Convert.ToString(bitFirmaContratoEnrolar);
                                    objProceso.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["RutaLectorHuellaCama"].ToString();
                                    objProceso.Start();
                                    while (!objProceso.HasExited)
                                    {
                                        System.Threading.Thread.Sleep(200);
                                    }

                                    btnClose.PerformClick();
                                    clsShowMessage.Show("Proceso realizo con éxito.", clsEnum.MessageType.Informa);
                                }
                                else
                                {
                                    if (ZkPalmPv10M)
                                    {
                                        clsLectorPalma objLectorP = new clsLectorPalma();
                                        objLectorP.m_bRegister = true;
                                        if (objLectorP.conectarDispositivo())
                                        {
                                            bool bProceso = true;
                                            while (bProceso)
                                            {
                                                string strMensaje = objLectorP.DoRegister(ref picPhoto);
                                                lblText.Text = strMensaje;
                                                if (strMensaje.IndexOf("satisfactoriamente") >= 0 || strMensaje.IndexOf("erronea") >= 0)
                                                {
                                                    bProceso = false;
                                                }
                                                Application.DoEvents();
                                                Thread.Sleep(10);
                                            }
                                            objLectorP.desconectarDispositivo();
                                            if (objLectorP.regExitoso != null)
                                            {
                                                quality = 90;
                                                EventBLL entryBll = new EventBLL();
                                                clsWhiteList wlCLS = new clsWhiteList();
                                                FingerprintBLL fingerBll = new FingerprintBLL();
                                                clsFingerprint fingerCLS = new clsFingerprint();
                                                //Insertamos la huella del cliente para tenerla en cuenta y actualizarla en la(s) terminal(es)
                                                for (int i = 0; i < 5; i++)
                                                {
                                                    fingerId = i + 1;
                                                    int tamano = 2000;
                                                    if (((i * 2000) + tamano) > objLectorP.regExitoso.Length)
                                                    {
                                                        tamano = objLectorP.regExitoso.Length - (i * 2000);
                                                    }
                                                    Array.Copy(objLectorP.regExitoso, i * 2000, fingerprintImage, 0, tamano);
                                                    if (fingerBll.ValidateFingerprintToRecordPalma(gymId, branchId, clientId, fingerId, fingerprintImage, quality, false))
                                                    {
                                                        fingerCLS.Insert(clientId, "", fingerId, fingerprintImage, quality);
                                                    }
                                                }

                                                //Insertamos el registro de enrolamiento en los eventos
                                                entryBll.Insert("", clientId, "", 0, 0, "", false, 0, null, null, false, "Huella grabada", "", "La huella del usuario fue grabada de forma correcta", true,
                                                                "", "Enroll", string.Empty);
                                                DialogResult = DialogResult.OK;
                                                this.Close();
                                            }
                                        }
                                        else
                                        {
                                            lblText.Text = objLectorP.errorConexion;
                                        }
                                    }
                                    else
                                    {
                                        //LectorStart();
                                        backWork = new BackgroundWorker();
                                        backWork.DoWork += BackWork_DoWork;
                                        backWork.RunWorkerCompleted += BackWork_RunWorkerCompleted;
                                        backWork.WorkerSupportsCancellation = true;
                                        backWork.RunWorkerAsync();
                                    }
                                }
                            }
                        }
                        else
                        {
                            InicialLectorZk();
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            if (blnTCAM7000)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// Método para liberar el lector de huella.
        /// Getulio Vargas - OD 1307
        /// </summary>
        private void LectorClose()
        {
            try
            {
                if (usbLectorManager.Scanners.Count > 0)
                {
                    usbLectorManager.Scanners[0].AbortCapturing();
                    usbLectorManager.Uninit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método encargado de leer la huella del cliente al momento de enrolar desde el lector USB.
        /// Getulio Vargas - OD 1307
        /// </summary>
        private void ReadFingerprint()
        {
            try
            {
                usbLector.Timeout = timeOutLector;
                byte[] template = new byte[maxSizeTemplate], additionalTemplate = new byte[maxSizeTemplate], additionalFingerprintWithCompleteSize, fingerprintWithCompleteSize;
                int templateSize = maxSizeTemplate, enrollQuality = 0, retry = 0;
                bool sameFingerprint = false;
                string strPlaceFinger = "Coloque el Dedo en el Lector de Huella", strRetireFinger = "Levante el Dedo del Lector de Huella", strDifferentFingerprint = "Las huellas no coinciden";
                state = usbLector.ClearCaptureImageBuffer();
                lblText.Text = strPlaceFinger;

                while (true)
                {
                    state = usbLector.CaptureSingleImage();
                    state = usbLector.Extract(template, out templateSize, out enrollQuality);

                    if (state == UFS_STATUS.OK)
                    {
                        lblText.Text = strRetireFinger;
                    }

                    this.Cursor = Cursors.WaitCursor;
                    fingerprintWithCompleteSize = new byte[maxSizeTemplate];

                    if (template != null)
                    {
                        Array.Copy(template, fingerprintWithCompleteSize, maxSizeTemplate);
                    }

                    template = fingerprintWithCompleteSize;

                    while (true)
                    {
                        System.Threading.Thread.Sleep(2000);
                        lblText.Text = strPlaceFinger;
                        state = usbLector.CaptureSingleImage();

                        if (state != UFS_STATUS.OK)
                        {
                            return;
                        }
                        else
                        {
                            lblText.Text = strRetireFinger;
                            System.Threading.Thread.Sleep(600);
                        }

                        state = usbLector.Extract(additionalTemplate, out templateSize, out enrollQuality);

                        if (state == UFS_STATUS.OK)
                        {
                            this.Cursor = Cursors.WaitCursor;
                            additionalFingerprintWithCompleteSize = new byte[maxSizeTemplate];

                            if (additionalTemplate != null)
                            {
                                Array.Copy(additionalTemplate, additionalFingerprintWithCompleteSize, maxSizeTemplate);
                            }

                            additionalTemplate = additionalFingerprintWithCompleteSize;

                            matcher.Verify(fingerprintWithCompleteSize, maxSizeTemplate, additionalFingerprintWithCompleteSize, maxSizeTemplate, out sameFingerprint);

                            if (sameFingerprint)
                            {
                                fingerprintImage = additionalFingerprintWithCompleteSize;
                                quality = enrollQuality;
                                break;
                            }
                            else
                            {
                                System.Threading.Thread.Sleep(600);
                                lblText.Text = strDifferentFingerprint;
                                retry++;

                                if (retry == 3)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (retry == 3 && !sameFingerprint)
                    {
                        state = UFS_STATUS.ERROR;
                        break;
                    }
                    else if (retry == 3 && sameFingerprint)
                    {
                        break;
                    }
                    else if (retry < 3 && sameFingerprint)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Método encargado de iniciar el objeto del lector USB.
        /// Getulio Vargas - OD 1307
        /// </summary>
        private void LectorStart()
        {
            try
            {
                if (!usbLector.IsSensorOn)
                {
                    usbLectorManager.Init();
                }

                if (usbLectorManager.Scanners.Count > 0)
                {
                    usbLector = usbLectorManager.Scanners[0];
                }

                matcher = new UFMatcher();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que sirve para validar si es posible grabar la huella de un usuario.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool ValidateRecordFingerprint(string id)
        {
            try
            {
                EventBLL entryBll = new EventBLL();
                string strGymId = string.Empty, strBranchId = string.Empty, message = string.Empty, msg = string.Empty; ;
                int gymId = 0, branchId = 0;
                bool state = false;
                FingerprintBLL fingerprintBll = new FingerprintBLL();
                eResponse response = new eResponse();
                strGymId = System.Configuration.ConfigurationManager.AppSettings["gymId"].ToString();
                strBranchId = System.Configuration.ConfigurationManager.AppSettings["branchId"].ToString();

                if (!string.IsNullOrEmpty(strGymId) && !string.IsNullOrEmpty(strBranchId))
                {
                    gymId = Convert.ToInt32(strGymId);
                    branchId = Convert.ToInt32(strBranchId);
                }

                if (gymId > 0 && branchId > 0)
                {
                    response = fingerprintBll.ValidatePerson(gymId, branchId, id);
                }

                if (response != null)
                {
                    state = response.state;
                    message = response.message;
                    fingerId = Convert.ToInt32(response.messageTwo);

                    if (state)
                    {
                        lblClientName.Text = response.messageOne;
                        return state;
                    }
                    else
                    {
                        switch (message)
                        {
                            case "BlackList":
                                msg = "No es posible grabar la huella del usuario " + id + " porque se encuentra registrado en la lista blanca.";
                                break;
                            case "NoActiveClient":
                                msg = "No es posible grabar la huella del usuario " + id + " porque no se encuentra como empleado(a) activo(a) o cliente activo(a).";
                                break;
                            case "NoConfiguration":
                                msg = "No es posible grabar la huella del usuario " + id + " porque no se encontró la configuración del ingreso para esta sucursal.";
                                break;
                            case "NoVigentPlan":
                                msg = "No es posible grabar la huella del usuario " + id + " porque no tiene plan vigente en el gimnasio.";
                                break;
                            default:
                                msg = "No es posible grabar la huella del usuario " + id +
                                                    ".\nEs posible que no haya conexión con el servidor para validar si se puede grabar la huella de este usuario.";
                                break;
                        }

                        entryBll.Insert("", id, "", 0, 0, "", false, 0, null, null, false, "No es posible enrolar el usuario", "", msg, false, "", "Enroll", string.Empty);
                        clsShowMessage.Show(msg, clsEnum.MessageType.Informa);

                        return false;
                    }
                }
                else
                {
                    entryBll.Insert("", id, "", 0, 0, "", false, 0, null, null, false, "No es posible enrolar el usuario", "", msg, false, "", "Enroll", string.Empty);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InicialLectorZk()
        {
            iFid = fingerId;
            IsRegister = false;
            if (!IsRegister)
            {
                IsRegister = true;
                RegisterCount = 0;
                cbRegTmp = 0;
                lblText.Text = "Por favor coloque su huella 3 veces!";
            }
        }

        private void DoCapture()
        {
            while (!bIsTimeToDie)
            {
                cbCapTmp = 2000;
                int ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
                if (ret == zkfp.ZKFP_ERR_OK)
                {
                    SendMessage(FormHandle, MESSAGE_CAPTURED_OK, IntPtr.Zero, IntPtr.Zero);
                }
                Thread.Sleep(200);
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
                        this.picPhoto.Image = bmp;
                        if (IsRegister)
                        {
                            int ret = zkfp.ZKFP_ERR_OK;
                            int fid = 0, score = 0;
                            ret = zkfp2.DBIdentify(mDBHandle, CapTmp, ref fid, ref score);
                            if (zkfp.ZKFP_ERR_OK == ret)
                            {
                                lblText.Text = "Esta huella ya fue registrada " + fid + "!";
                                return;
                            }
                            if (RegisterCount > 0 && zkfp2.DBMatch(mDBHandle, CapTmp, RegTmps[RegisterCount - 1]) <= 0)
                            {
                                lblText.Text = "Por favor coloque al misma huella 3 veces para el enrolamiento";
                                return;
                            }
                            Array.Copy(CapTmp, RegTmps[RegisterCount], cbCapTmp);
                            String strBase64 = zkfp2.BlobToBase64(CapTmp, cbCapTmp);
                            byte[] blob = zkfp2.Base64ToBlob(strBase64);
                            RegisterCount++;
                            if (RegisterCount >= REGISTER_FINGER_COUNT)
                            {
                                RegisterCount = 0;
                                if (zkfp.ZKFP_ERR_OK == (ret = zkfp2.DBMerge(mDBHandle, RegTmps[0], RegTmps[1], RegTmps[2], RegTmp, ref cbRegTmp)) &&
                                       zkfp.ZKFP_ERR_OK == (ret = zkfp2.DBAdd(mDBHandle, iFid, RegTmp)))
                                {
                                    EventBLL entryBll = new EventBLL();
                                    clsWhiteList wlCLS = new clsWhiteList();
                                    FingerprintBLL fingerBll = new FingerprintBLL();
                                    clsFingerprint fingerCLS = new clsFingerprint();
                                    fingerprintImage = RegTmp;
                                    quality = 90;
                                    if (fingerBll.ValidateFingerprintToRecord(gymId, branchId, clientId, fingerId, fingerprintImage, quality, false,"zk"))
                                    {
                                        //Insertamos la huella del cliente para tenerla en cuenta y actualizarla en la(s) terminal(es)
                                        if (fingerCLS.Insert(clientId, "", fingerId, fingerprintImage, quality))
                                        {
                                            //Validamos si se debe firmar contrato al enrolar
                                            eConfiguration config = new eConfiguration();
                                            AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                                            config = acsBll.GetLocalAccessControlSettings();

                                            var bitFirmaContratoEnrolar = false;
                                            bitFirmaContratoEnrolar = config.bitFirmarContratoAlEnrolar;
                                            if (bitFirmaContratoEnrolar)
                                            {
                                                fingerBll.firmarContratoGSW(gymId, branchId, clientId, fingerId, fingerprintImage, quality, false);

                                            }

                                            //Actualizamos la lista blanca local
                                            wlCLS.UpdateFingerprint(gymId, branchId, clientId, fingerId, fingerprintImage);
                                            //Insertamos el registro de enrolamiento en los eventos
                                            entryBll.Insert("", clientId, "", 0, 0, "", false, 0, null, null, false, "Huella grabada", "", "La huella del usuario fue grabada de forma correcta", true,
                                                            "", "Enroll", string.Empty);
                                            lblText.Text = "Registro de huella exitoso";
                                            DialogResult = DialogResult.OK;
                                            this.Close();
                                        }
                                    }
                                    else
                                    {
                                        //Insertamos el registro de enrolamiento en los eventos
                                        entryBll.Insert("", clientId, "", 0, 0, "", false, 0, null, null, false, "No es posible enrolar el usuario", "", "La huella ya existe", false, "", "Enroll",
                                                        string.Empty);
                                        lblText.Text = "La huella ya existe, registre otro dedo";
                                    }
                                }
                                else
                                {
                                    lblText.Text = "Error al enrolar, error codigo=" + ret;
                                }
                                IsRegister = false;
                                return;
                            }
                            else
                            {
                                lblText.Text = "Usted necesita colocar la huella " + (REGISTER_FINGER_COUNT - RegisterCount) + " veces";
                            }
                        }
                        else
                        {
                            if (cbRegTmp <= 0)
                            {
                                lblText.Text = "Por favor registre su huella primero!";
                                return;
                            }
                            if (bIdentify)
                            {
                                int ret = zkfp.ZKFP_ERR_OK;
                                int fid = 0, score = 0;
                                ret = zkfp2.DBIdentify(mDBHandle, CapTmp, ref fid, ref score);
                                if (zkfp.ZKFP_ERR_OK == ret)
                                {
                                    lblText.Text = "Identify succ, fid= " + fid + ",score=" + score + "!";
                                    return;
                                }
                                else
                                {
                                    lblText.Text = "Identify fail, ret= " + ret;
                                    return;
                                }
                            }
                            else
                            {
                                int ret = zkfp2.DBMatch(mDBHandle, CapTmp, RegTmp);
                                if (0 < ret)
                                {
                                    lblText.Text = "Match finger succ, score=" + ret + "!";
                                    return;
                                }
                                else
                                {
                                    lblText.Text = "Match finger fail, ret= " + ret;
                                    return;
                                }
                            }
                        }
                    }
                    break;

                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

    }


}