using Sibo.WhiteList.Service.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Sibo.WhiteList.Service.Entities.Classes;
using System.IO.Ports;
using static libComunicacionBioEntry.API;

namespace libComunicacionBioEntry
{
#if !SDK_AUTO_CONNECTION
    class ReconnectionTask : IDisposable
    {
        private IntPtr sdkContext;
        private bool running;
        private Thread thread;
        private readonly object locker = new object();
        private EventWaitHandle eventWaitHandle = new AutoResetEvent(false);
        private Queue<UInt32> deviceIDQueue = new Queue<UInt32>();

        public ReconnectionTask(IntPtr sdkContext)
        {
            this.sdkContext = sdkContext;
            thread = new Thread(run);
        }

        public void enqueue(UInt32 deviceID)
        {
            bool isAlreadyRequested = false;

            lock (locker)
            {
                foreach (UInt32 targetDeviceID in deviceIDQueue)
                {
                    if (targetDeviceID == deviceID)
                    {
                        isAlreadyRequested = true;
                        break;
                    }
                }

                if (!isAlreadyRequested)
                {
                    deviceIDQueue.Enqueue(deviceID);
                }
            }

            if (!isAlreadyRequested)
            {
                GenerarArchivo("enqueue", "dispositivo " + deviceID + " en cola", "transaccion", deviceID.ToString());
                eventWaitHandle.Set();
            }
        }

        public void Dispose()
        {
            stop();
        }

        public void start()
        {
            if (!running)
            {
                running = true;
                thread.Start();
            }
        }

        public void stop()
        {
            if (running)
            {
                running = false;
                lock (locker)
                {
                    deviceIDQueue.Clear();
                }
                eventWaitHandle.Set();
                thread.Join();
                eventWaitHandle.Close();
            }
        }

        public void run()
        {
            while (running)
            {
                UInt32 deviceID = 0;

                lock (locker)
                {
                    if (deviceIDQueue.Count > 0)
                    {
                        deviceID = deviceIDQueue.Dequeue();
                    }
                }

                if (deviceID != 0)
                {
                    GenerarArchivo("run", "intentando reconectar el dispositivo " + deviceID, "transaccion", deviceID.ToString());
                    int delayTerminate = 0;
                    Thread.Sleep(delayTerminate);
                    BS2ErrorCode result = (BS2ErrorCode)API.BS2_ConnectDevice(sdkContext, deviceID);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        if (result != BS2ErrorCode.BS_SDK_ERROR_CANNOT_CONNECT_SOCKET)
                        {
                            GenerarArchivo("run", "No se pudo conectar con el dispositivo " + deviceID, "errores", deviceID.ToString());
                            return;
                        }
                        else
                        {
                            enqueue(deviceID);
                        }
                    }
                }
                else
                {
                    eventWaitHandle.WaitOne();
                }
            }
        }

        private void GenerarArchivo(string nombreFuncion, string mensaje, string nombrearchivo, string ipGenera)
        {
            try
            {
                string ubicacionLogs = Application.StartupPath + "\\Logs" + "\\" + nombreFuncion + "\\" + ipGenera;
                validarRuta(ubicacionLogs);
                System.IO.StreamWriter file = new System.IO.StreamWriter(ubicacionLogs + "\\" + nombrearchivo + DateTime.Now.ToString("yyyyMMdd") + ipGenera + ".log", true);
                file.WriteLine(mensaje + " , función: " + nombreFuncion + ", Fecha generacion: " + DateTime.Now);
                file.Close();
                file.Dispose();
            }
            catch
            { }
        }

        private void validarRuta(string ruta)
        {
            if (System.IO.File.Exists(ruta))
            {
                System.IO.Directory.CreateDirectory(ruta);
            }
        }
    }
#endif

    public class clsComBioEntryPlus2
    {
        private API.OnDeviceFound cbOnDeviceFound = null;
        private API.OnDeviceAccepted cbOnDeviceAccepted = null;
        private API.OnDeviceConnected cbOnDeviceConnected = null;
        private API.OnReadyToScan cbFingerOnReadyToScan = null;
        private API.OnDeviceDisconnected cbOnDeviceDisconnected = null;
        protected IntPtr sdkContext = IntPtr.Zero;

        void ReadyToScanForFinger(UInt32 deviceID, UInt32 sequence)
        {
            Console.WriteLine("Place your finger on the device.");
        }



#if !SDK_AUTO_CONNECTION
        private ReconnectionTask reconnectionTask = null;
#endif

        private UInt32 deviceIDForServerMode = 0;
        private EventWaitHandle eventWaitHandle = new AutoResetEvent(false);


        private API.PreferMethod cbPreferMethod = null;
        private API.GetRootCaFilePath cbGetRootCaFilePath = null;
        private API.GetServerCaFilePath cbGetServerCaFilePath = null;
        private API.GetServerPrivateKeyFilePath cbGetServerPrivateKeyFilePath = null;
        private API.GetPassword cbGetPassword = null;
        private API.OnErrorOccured cbOnErrorOccured = null;

        private string ssl_server_root_crt = "resource/server/ssl_server_root.crt";
        private string ssl_server_crt = "resource/server/ssl_server.crt";
        private string ssl_server_pem = "resource/server/ssl_server.pem";
        private string ssl_server_passwd = "supremaserver";

        private IntPtr ptr_server_root_crt = IntPtr.Zero;
        private IntPtr ptr_server_crt = IntPtr.Zero;
        private IntPtr ptr_server_pem = IntPtr.Zero;
        private IntPtr ptr_server_passwd = IntPtr.Zero;

        private API.OnLogReceived cbOnLogReceived = null;

        private UInt32 ultimoRegistro = 0;
        public delegate void marcacionRec(string usuario, string codigoPuerta, DateTime fechaHora, string evento, string datosMarcacion, string tna);
        public event marcacionRec marcacionRecibida;
        public delegate void desconectoTerminalHandler(string ip);
        public event desconectoTerminalHandler desconectoTerminal;

        public UInt32 deviceID = 0;

        public Boolean bConectada = true;
        private string strIpActual = "";

        UInt32 PreferMethodHandle(UInt32 deviceID)
        {
            return (UInt32)(BS2SslMethodMaskEnum.TLS1 | BS2SslMethodMaskEnum.TLS1_1 | BS2SslMethodMaskEnum.TLS1_2);
        }

        IntPtr GetRootCaFilePathHandle(UInt32 deviceID)
        {
            //return ssl_server_root_crt;
            if (ptr_server_root_crt == IntPtr.Zero)
                ptr_server_root_crt = Marshal.StringToHGlobalAnsi(ssl_server_root_crt);
            return ptr_server_root_crt;
        }

        IntPtr GetServerCaFilePathHandle(UInt32 deviceID)
        {
            //return ssl_server_crt;
            if (ptr_server_crt == IntPtr.Zero)
                ptr_server_crt = Marshal.StringToHGlobalAnsi(ssl_server_crt);
            return ptr_server_crt;
        }

        IntPtr GetServerPrivateKeyFilePathHandle(UInt32 deviceID)
        {
            //return ssl_server_pem;
            if (ptr_server_pem == IntPtr.Zero)
                ptr_server_pem = Marshal.StringToHGlobalAnsi(ssl_server_pem);
            return ptr_server_pem;
        }

        IntPtr GetPasswordHandle(UInt32 deviceID)
        {
            //return ssl_server_passwd;
            if (ptr_server_passwd == IntPtr.Zero)
                ptr_server_passwd = Marshal.StringToHGlobalAnsi(ssl_server_passwd);
            return ptr_server_passwd;
        }

        void OnErrorOccuredHandle(UInt32 deviceID, int errCode)
        {
            GenerarArchivo("OnErrorOccuredHandle", "genero ssl error" + (BS2ErrorCode)errCode + "Device" + deviceID, "errores", deviceID.ToString());
        }

        void DeviceFound(UInt32 deviceID)
        {
            GenerarArchivo("DeviceFound", "[CB] Device[{0, 10}] a sido encontrado." + deviceID, "errores", deviceID.ToString());
        }

        void DeviceAccepted(UInt32 deviceID)
        {
            GenerarArchivo("DeviceAccepted", "Dispositivo " + deviceID.ToString() + " ha sido aceptado", "transacciones", deviceID.ToString());
            deviceIDForServerMode = deviceID;
            eventWaitHandle.Set();
        }

        void DeviceConnected(UInt32 deviceID)
        {
            GenerarArchivo("DeviceConnected", "Dispositivo " + deviceID.ToString() + " ha sido conectado", "transacciones", deviceID.ToString());
        }

        void DeviceDisconnected(UInt32 deviceID)
        {
            //GenerarArchivo("DeviceDisconnected", "Dispositivo " + deviceID.ToString() + " ha sido desconectado", "transacciones", deviceID.ToString());
#if !SDK_AUTO_CONNECTION
            if (reconnectionTask != null)
            {
                GenerarArchivo("DeviceDisconnected", "Dispositivo " + deviceID.ToString() + " en cola", "transacciones", deviceID.ToString());
                reconnectionTask.enqueue(deviceID);
            }
#endif
            desconectoTerminal?.Invoke(strIpActual);
        }

        private void NormalLogReceived(UInt32 deviceID, IntPtr log)
        {
            if (log != IntPtr.Zero)
            {
                BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(log, typeof(BS2Event));
                GenerarArchivo("NormalLogReceived", Util.GetLogMsg(eventLog), "transacciones", deviceID.ToString());
            }
        }

        public enum estadoControladora
        {
            descargandoMarcaciones,
            enrrolandoHuellas,
            conectandoParaValidadConexion,
            configurandoHorario,
            borrandoHuella,
            leyendoInformacionSistema,
            escribiendoInformacionSistema
        }

        public enum respuestaDescarga
        {
            terminalOcupada,
            errorComandoEnrrolHuellas,
            huellaEnrroladaSatisfactoriamente,
            terminalConectada,
            terminalDesconectada,
            noSepudoInicializarSdk,
            noSepudoabrirlaComunicacionUdp,
            errorAlBuscarLosDispositivos,
            noSeEncontraronDispositivos,
            dispositivosEncontrados,
            NoSePudoCrearElHorario,
            NoSePudoCrearElGrupo,
            HorarioCreadoSatisfactoriamente,
            fechaHoraActualizadaSatisfactoriamente,
            fechaHoraNoPudoSerActualizada,
            huellaBorradaSatisfactoriamente,
            errorBorrandoHuella,
            noSePudoLeerElIdDelDispositivo,
            noSePudoIniciarElIdDelDispositivo,
            noSePudeLeerLaCantidadDeLog,
            noSePudeLeerLaCantidadDeUsuariosRegistrados,
            noSePudoLeerEllogdeMarcaciones,
            noSePudeLeerLosUsuariosRegistrados,
            marcacionesDescargadasSatisfactoriamente,
            noSePudoLeerLaInformacionDelSistema,
            leyoLaInformacionDelSistemaSatisfactoriamente,
            SeleyolainformacionDelosUsuariosSatisfactoriamente,
            huellaDescargadaSatisfactoriamente,
            huellaNosepudoDescargar
        }

        public enum tipoAccesoLector
        {
            Desactivado,
            soloHuella,
            HuellaYClave,
            HuellaOClave,
            ClaveSolo,
            TarjetaSolo
        }

        public void dercargarMarcacioneslinea(string datos)
        {

        }

        public List<eTerminarBioLite> descargarMarcaciones(string direccionIp, int codigoPuerta)
        {
            try
            {
                List<eTerminarBioLite> lsReturn = new List<eTerminarBioLite>();

                if (!bConectada)
                {
                    return null;
                }
                const UInt32 defaultLogPageSize = 1024;
                Type structureType = typeof(BS2Event);
                int structSize = Marshal.SizeOf(structureType);
                bool getAllLog = false;
                UInt32 lastEventId = 0;
                UInt32 amount;
                IntPtr outEventLogObjs = IntPtr.Zero;
                UInt32 outNumEventLogs = 0;
                cbOnLogReceived = new API.OnLogReceived(NormalLogReceived);

                lastEventId = ultimoRegistro;
                amount = 0;

                if (amount == 0)
                {
                    getAllLog = true;
                    amount = defaultLogPageSize;
                }

                do
                {
                    outEventLogObjs = IntPtr.Zero;
                    int delayTerminate2 = 0;
                    Thread.Sleep(delayTerminate2);
                    BS2ErrorCode result = (BS2ErrorCode)API.BS2_GetLog(sdkContext, deviceID, lastEventId, amount, out outEventLogObjs, out outNumEventLogs);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        GenerarArchivo("descargarMarcaciones", "Genero error código:" + result, "errores", direccionIp);
                        //return respuestaDescarga.noSePudeLeerLaCantidadDeLog;
                        return null;
                    }

                    if (outNumEventLogs > 0)
                    {
                        IntPtr curEventLogObjs = outEventLogObjs;
                        for (UInt32 idx = 0; idx < outNumEventLogs; idx++)
                        {
                            BS2Event eventLog = (BS2Event)Marshal.PtrToStructure(curEventLogObjs, structureType);

                            //string strDatosValidacion = "";

                            //if ((BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS)
                            //{
                            //    strDatosValidacion = "AUTORIZADO";
                            //}
                            //else
                            //{
                            //    if ((BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_FAIL)
                            //    {
                            //        strDatosValidacion = "DENEGADO";
                            //    }


                            //    else
                            //    {
                            //        strDatosValidacion = "N/A";
                            //    }
                            //}


                            //marcacionRecibida?.Invoke(System.Text.Encoding.ASCII.GetString(eventLog.userID).TrimEnd('\0'), codigoPuerta.ToString(), Util.ConvertFromUnixTimestamp(eventLog.dateTime), ((BS2EventCodeEnum)eventLog.code).ToString(), strDatosValidacion, "N/A");
                            EventBLL eventBLL = new EventBLL();
                            if (System.Text.Encoding.ASCII.GetString(eventLog.userID).TrimEnd('\0') != "0" && System.Text.Encoding.ASCII.GetString(eventLog.userID).TrimEnd('\0') != "")
                            {
                                //WhiteListBLL wlCLS = new WhiteListBLL();
                                //wlCLS.ValidateEntryByUserId(System.Text.Encoding.ASCII.GetString(eventLog.userID).TrimEnd('\0'), false, false);
                                //eventBLL.Insert("", System.Text.Encoding.ASCII.GetString(eventLog.userID).TrimEnd('\0'), "", 0, 0, "", false, 0, null, null, false, "No es posible enrolar el usuario", "",
                                //            "El cliente ya tiene huella registrada; para actualizarla, el proceso debe ser realizado por el empleado",
                                //            false, direccionIp, "Enroll", string.Empty);
                                //GenerarArchivo("descargarMarcaciones", Util.GetLogMsg(eventLog), "transacciones", direccionIp);
                                if ((BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_ID_PIN ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_ID_FINGER ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_ID_FINGER_PIN ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_ID_FACE ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_ID_FACE_PIN ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_CARD ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_CARD_PIN ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_CARD_FINGER ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_CARD_FINGER_PIN ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_CARD_FACE ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_CARD_FACE_PIN ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_AOC ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_AOC_PIN ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_AOC_FINGER ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.VERIFY_SUCCESS_AOC_FINGER_PIN ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.IDENTIFY_SUCCESS ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.IDENTIFY_SUCCESS_FINGER ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.IDENTIFY_SUCCESS_FINGER_PIN ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.IDENTIFY_SUCCESS_FACE ||
                                        (BS2EventCodeEnum)eventLog.code == BS2EventCodeEnum.IDENTIFY_SUCCESS_FACE_PIN)
                                {
                                    lsReturn.Add(new eTerminarBioLite
                                    {
                                        ip = direccionIp,
                                        codigo = System.Text.Encoding.ASCII.GetString(eventLog.userID).TrimEnd('\0'),
                                        esAceptada = true
                                    });
                                }
                            }
                            curEventLogObjs += structSize;
                            lastEventId = eventLog.id;
                            ultimoRegistro = lastEventId;
                        }

                        API.BS2_ReleaseObject(outEventLogObjs);
                    }

                    if (outNumEventLogs < defaultLogPageSize)
                    {
                        break;
                    }
                }
                while (getAllLog);
                int delayTerminate = 0;
                Thread.Sleep(delayTerminate);
                BS2ErrorCode resultConElim = (BS2ErrorCode)API.BS2_GetLog(sdkContext, deviceID, lastEventId, amount, out outEventLogObjs, out outNumEventLogs);
                if (resultConElim == BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    if (outNumEventLogs == 0)
                    {
                        Thread.Sleep(delayTerminate);
                        BS2ErrorCode result = (BS2ErrorCode)API.BS2_ClearLog(sdkContext, deviceID);
                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            GenerarArchivo("descargarMarcaciones", "No se pudo eliminar el log", "errores", direccionIp);
                        }
                        else
                        {
                            GenerarArchivo("descargarMarcaciones", "Se eliminaron los log satisfactoriamente", "transacciones", direccionIp);
                        }
                    }
                }

                //return respuestaDescarga.marcacionesDescargadasSatisfactoriamente;
                return lsReturn;
            }
            catch (Exception ex)
            {
                if (ultimoRegistro > 0)
                {
                    ultimoRegistro -= 1;
                }
                GenerarArchivo("descargarMarcaciones", ex.Message, "errores", direccionIp);
                //return respuestaDescarga.noSePudoIniciarElIdDelDispositivo;
                return null;
            }
        }

        public respuestaDescarga EnrrolarHuellaBioEntry(DataRow drvFila, tipoAccesoLector tipoAcceso, string numeroTarjeta, byte[] datoHuella, byte[] datoHuellaDos, byte[] datoHuellaTres, byte[] datoHuellaCuatro, string indiceDedo, string[] grupoAccesoUno, string[] grupoAccesoDos, uint idDispositivo, bool autorizado, string strIdentificacion)
        {
            try
            {
                string[] strDatostarjeta = numeroTarjeta.Split('-');
                BS2UserBlob[] userBlob = Util.AllocateStructureArray<BS2UserBlob>(1);
                userBlob[0].user = Util.AllocateStructure<BS2User>();
                if (tipoAcceso == tipoAccesoLector.soloHuella)
                {
                    numeroTarjeta = "";
                    Encoding.ASCII.GetBytes(indiceDedo, 0, indiceDedo.Length, userBlob[0].user.userID, 0);
                    if (strDatostarjeta.Length == 1)
                    {
                        strDatostarjeta[0] = "0";
                    }
                    else
                    {
                        strDatostarjeta[1] = "0";
                    }
                }
                else
                {
                    Encoding.ASCII.GetBytes(strIdentificacion, 0, strIdentificacion.Length, userBlob[0].user.userID, 0);
                }

                if (tipoAcceso == tipoAccesoLector.TarjetaSolo)
                {
                    userBlob[0].user.numCards = 1;
                }
                else
                {
                    numeroTarjeta = "";
                }

                if (datoHuella != null && datoHuella.Length > 0)
                {
                    userBlob[0].user.numFingers = 1;
                }
                //if (datoHuellaDos != null && datoHuellaDos.Length > 0)
                //{
                //    userBlob[0].user.numFingers += 1;
                //}
                //if (datoHuellaTres != null && datoHuellaTres.Length > 0)
                //{
                //    userBlob[0].user.numFingers += 1;
                //}
                //if (datoHuellaCuatro != null && datoHuellaCuatro.Length > 0)
                //{
                //    userBlob[0].user.numFingers += 1;
                //}

                userBlob[0].setting.startTime = 0;
                userBlob[0].setting.endTime = 0;
                userBlob[0].setting.fingerAuthMode = 0;
                userBlob[0].setting.cardAuthMode = 2;
                userBlob[0].setting.idAuthMode = 8;
                userBlob[0].setting.securityLevel = 3;

                switch (tipoAcceso)
                {
                    case tipoAccesoLector.TarjetaSolo:
                        userBlob[0].setting.fingerAuthMode = 0;
                        userBlob[0].setting.cardAuthMode = 2;
                        break;
                    case tipoAccesoLector.soloHuella:
                        userBlob[0].setting.fingerAuthMode = 0;
                        userBlob[0].setting.cardAuthMode = 2;
                        break;
                    case tipoAccesoLector.ClaveSolo:
                        userBlob[0].setting.fingerAuthMode = 0;
                        userBlob[0].setting.cardAuthMode = 3;
                        break;
                    case tipoAccesoLector.HuellaYClave:
                        userBlob[0].setting.cardAuthMode = 3;
                        userBlob[0].setting.fingerAuthMode = 0;
                        break;
                    case tipoAccesoLector.HuellaOClave:
                        userBlob[0].setting.fingerAuthMode = 0;
                        userBlob[0].setting.cardAuthMode = 2;
                        break;
                }

                Array.Clear(userBlob[0].name, 0, BS2Environment.BS2_USER_NAME_LEN);
                userBlob[0].photo.size = 0;
                Array.Clear(userBlob[0].photo.data, 0, BS2Environment.BS2_USER_PHOTO_SIZE);

                Array.Clear(userBlob[0].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);

                if (tipoAcceso == tipoAccesoLector.TarjetaSolo)
                {
                    userBlob[0].cardObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2CSNCard)) * userBlob[0].user.numCards);

                    IntPtr curCardObjs = userBlob[0].cardObjs;
                    byte cardType = 1;
                    byte cardSize = 32;

                    byte[] cardData = new byte[32];

                    if (strDatostarjeta.Length == 1)
                    {
                        cardData = codigoTarjeta(strDatostarjeta[0]);
                    }
                    else
                    {
                        cardData = codigoTarjeta(strDatostarjeta[1]);
                    }

                    Marshal.WriteByte(curCardObjs, cardType);
                    curCardObjs += 1;
                    Marshal.WriteByte(curCardObjs, cardSize);
                    curCardObjs += 1;
                    Marshal.Copy(cardData, 0, curCardObjs, BS2Environment.BS2_CARD_DATA_SIZE);
                    curCardObjs += BS2Environment.BS2_CARD_DATA_SIZE;

                    IntPtr ptrChar = Marshal.StringToHGlobalAnsi(strIdentificacion);
                    IntPtr pinCode = Marshal.AllocHGlobal(BS2Environment.BS2_PIN_HASH_SIZE);
                    BS2ErrorCode result2 = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, ptrChar, pinCode);

                    Marshal.Copy(pinCode, userBlob[0].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                    Marshal.FreeHGlobal(ptrChar);
                    Marshal.FreeHGlobal(pinCode);
                }
                else
                {
                    userBlob[0].cardObjs = IntPtr.Zero;
                }

                if (tipoAcceso == tipoAccesoLector.soloHuella)
                {
                    if (userBlob[0].user.numFingers > 0)
                    {
                        userBlob[0].fingerObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2Fingerprint)) * userBlob[0].user.numFingers);
                        IntPtr curFingerObjs = userBlob[0].fingerObjs;

                        byte fingerIndex = 0;
                        byte fingerFlag = 0;
                        byte[] templateData;
                        if (datoHuella != null && datoHuella.Length > 0)
                        {
                            fingerIndex = 0;
                            fingerFlag = 0;
                            templateData = convertirHuella(datoHuella);

                            Marshal.WriteByte(curFingerObjs, fingerIndex);
                            curFingerObjs += 1;
                            Marshal.WriteByte(curFingerObjs, fingerFlag);
                            curFingerObjs += 3;
                            Marshal.Copy(templateData, 0, curFingerObjs, BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
                            curFingerObjs += BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE;
                        }
                        //if (datoHuellaDos != null && datoHuellaDos.Length > 0)
                        //{
                        //    fingerIndex = 1;
                        //    fingerFlag = 0;
                        //    templateData = convertirHuella(datoHuellaDos);

                        //    Marshal.WriteByte(curFingerObjs, fingerIndex);
                        //    curFingerObjs += 1;
                        //    Marshal.WriteByte(curFingerObjs, fingerFlag);
                        //    curFingerObjs += 3;
                        //    Marshal.Copy(templateData, 0, curFingerObjs, BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
                        //    curFingerObjs += BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE;
                        //}
                        //if (datoHuellaTres != null && datoHuellaTres.Length > 0)
                        //{
                        //    fingerIndex = 2;
                        //    fingerFlag = 0;
                        //    templateData = convertirHuella(datoHuellaTres);

                        //    Marshal.WriteByte(curFingerObjs, fingerIndex);
                        //    curFingerObjs += 1;
                        //    Marshal.WriteByte(curFingerObjs, fingerFlag);
                        //    curFingerObjs += 3;
                        //    Marshal.Copy(templateData, 0, curFingerObjs, BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
                        //    curFingerObjs += BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE;
                        //}
                        //if (datoHuellaCuatro != null && datoHuellaCuatro.Length > 0)
                        //{
                        //    fingerIndex = 3;
                        //    fingerFlag = 0;
                        //    templateData = convertirHuella(datoHuellaCuatro);

                        //    Marshal.WriteByte(curFingerObjs, fingerIndex);
                        //    curFingerObjs += 1;
                        //    Marshal.WriteByte(curFingerObjs, fingerFlag);
                        //    curFingerObjs += 3;
                        //    Marshal.Copy(templateData, 0, curFingerObjs, BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
                        //    curFingerObjs += BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE;
                        //}
                    }
                    else
                    {
                        userBlob[0].fingerObjs = IntPtr.Zero;
                    }
                }
                else
                {
                    userBlob[0].fingerObjs = IntPtr.Zero;
                }

                if (tipoAcceso == tipoAccesoLector.ClaveSolo)
                {
                    IntPtr ptrChar = Marshal.StringToHGlobalAnsi(strIdentificacion);
                    IntPtr pinCode = Marshal.AllocHGlobal(BS2Environment.BS2_PIN_HASH_SIZE);
                    BS2ErrorCode result2 = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, ptrChar, pinCode);

                    Marshal.Copy(pinCode, userBlob[0].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                    Marshal.FreeHGlobal(ptrChar);
                    Marshal.FreeHGlobal(pinCode);
                }

                userBlob[0].faceObjs = IntPtr.Zero;
                Array.Clear(userBlob[0].accessGroupId, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER);
                int access_group_count = 0;

                foreach (string itemGrupo in grupoAccesoUno)
                {
                    if (itemGrupo != null && itemGrupo != "" && itemGrupo != " ")
                    {
                        userBlob[0].accessGroupId[access_group_count++] = Convert.ToUInt32(itemGrupo) + 1;
                    }
                }

                int delayTerminate = 0;
                Thread.Sleep(delayTerminate);
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_EnrolUser(sdkContext, deviceID, userBlob, 1, 1);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    GenerarArchivo("EnrrolarHuellaBioEntry", "No se pudo actualizar el usuario " + (strIdentificacion.Length == 0 ? indiceDedo : strIdentificacion) + ", el error fue: " + (BS2ErrorCode)result, "errores", (numeroTarjeta.Length == 0 ? indiceDedo : numeroTarjeta));
                    return respuestaDescarga.errorComandoEnrrolHuellas;
                }
                else
                {
                    GenerarArchivo("EnrrolarHuellaBioEntry", "Se actualizo el usuario " + (strIdentificacion.Length == 0 ? indiceDedo : strIdentificacion), "transacciones", drvFila["strIpTerminal"].ToString());
                    return respuestaDescarga.huellaEnrroladaSatisfactoriamente;
                }
            }

            catch (Exception ex)
            {
                GenerarArchivo("EnrrolarHuellaBioEntry", ex.Message, "errores", drvFila["strIpTerminal"].ToString());
                return respuestaDescarga.errorComandoEnrrolHuellas;
            }
        }

        public respuestaDescarga EnrrolarHuellaBioEntryNuevo(DataRow drvFila, string numeroTarjeta, byte[] datoHuella, string indiceDedo, string[] grupoAccesoUno, string[] grupoAccesoDos, uint idDispositivo, bool autorizado, string strIdentificacion, bool ingresoSinHuella)
        {
            try
            {
                BS2UserBlob[] userBlob = Util.AllocateStructureArray<BS2UserBlob>(1);
                userBlob[0].user = Util.AllocateStructure<BS2User>();

                if (ingresoSinHuella == true)
                {
                    //Huella
                    Encoding.ASCII.GetBytes(strIdentificacion, 0, strIdentificacion.Length, userBlob[0].user.userID, 0);
                    userBlob[0].fingerObjs = IntPtr.Zero;

                    //Tarjeta
                    if (numeroTarjeta != "" && numeroTarjeta != " " && numeroTarjeta != null)
                    {
                        userBlob[0].user.numCards = 1;
                        userBlob[0].cardObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2CSNCard)) * userBlob[0].user.numCards);

                        IntPtr curCardObjs = userBlob[0].cardObjs;
                        byte cardType = 1;
                        byte cardSize = 32;

                        byte[] cardData = new byte[32];

                        cardData = codigoTarjeta(numeroTarjeta);

                        Marshal.WriteByte(curCardObjs, cardType);
                        curCardObjs += 1;
                        Marshal.WriteByte(curCardObjs, cardSize);
                        curCardObjs += 1;
                        Marshal.Copy(cardData, 0, curCardObjs, BS2Environment.BS2_CARD_DATA_SIZE);
                        curCardObjs += BS2Environment.BS2_CARD_DATA_SIZE;

                        IntPtr ptrChar = Marshal.StringToHGlobalAnsi(strIdentificacion);
                        IntPtr pinCode = Marshal.AllocHGlobal(BS2Environment.BS2_PIN_HASH_SIZE);
                        BS2ErrorCode result2 = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, ptrChar, pinCode);

                        Marshal.Copy(pinCode, userBlob[0].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                        Marshal.FreeHGlobal(ptrChar);
                        Marshal.FreeHGlobal(pinCode);
                    }
                    else
                    {
                        numeroTarjeta = "";
                        userBlob[0].cardObjs = IntPtr.Zero;
                    }

                    //Clave
                    IntPtr ptrCharPin = Marshal.StringToHGlobalAnsi(strIdentificacion);
                    IntPtr pinCodePin = Marshal.AllocHGlobal(BS2Environment.BS2_PIN_HASH_SIZE);
                    BS2ErrorCode result2Pin = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, ptrCharPin, pinCodePin);

                    Marshal.Copy(pinCodePin, userBlob[0].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                    Marshal.FreeHGlobal(ptrCharPin);
                    Marshal.FreeHGlobal(pinCodePin);
                }
                else
                {
                    //Huella
                    if (indiceDedo != "" && indiceDedo != " " && indiceDedo != null)
                    {
                        if (datoHuella != null && datoHuella.Length > 0)
                        {
                            Encoding.ASCII.GetBytes(indiceDedo, 0, indiceDedo.Length, userBlob[0].user.userID, 0);
                            userBlob[0].user.numFingers = 1;
                        }

                        if (userBlob[0].user.numFingers > 0)
                        {
                            userBlob[0].fingerObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2Fingerprint)) * userBlob[0].user.numFingers);
                            IntPtr curFingerObjs = userBlob[0].fingerObjs;

                            byte fingerIndex = 0;
                            byte fingerFlag = 0;
                            byte[] templateData;
                            if (datoHuella != null && datoHuella.Length > 0)
                            {
                                fingerIndex = 0;
                                fingerFlag = 0;
                                templateData = convertirHuella(datoHuella);

                                Marshal.WriteByte(curFingerObjs, fingerIndex);
                                curFingerObjs += 1;
                                Marshal.WriteByte(curFingerObjs, fingerFlag);
                                curFingerObjs += 3;
                                Marshal.Copy(templateData, 0, curFingerObjs, BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
                                curFingerObjs += BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE;
                            }
                        }
                    }
                    else
                    {
                        Encoding.ASCII.GetBytes(strIdentificacion, 0, strIdentificacion.Length, userBlob[0].user.userID, 0);
                        userBlob[0].fingerObjs = IntPtr.Zero;
                    }

                    //Tarjeta
                    if (numeroTarjeta != "" && numeroTarjeta != " " && numeroTarjeta != null)
                    {
                        userBlob[0].user.numCards = 1;
                        userBlob[0].cardObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2CSNCard)) * userBlob[0].user.numCards);

                        IntPtr curCardObjs = userBlob[0].cardObjs;
                        byte cardType = 1;
                        byte cardSize = 32;

                        byte[] cardData = new byte[32];

                        cardData = codigoTarjeta(numeroTarjeta);

                        Marshal.WriteByte(curCardObjs, cardType);
                        curCardObjs += 1;
                        Marshal.WriteByte(curCardObjs, cardSize);
                        curCardObjs += 1;
                        Marshal.Copy(cardData, 0, curCardObjs, BS2Environment.BS2_CARD_DATA_SIZE);
                        curCardObjs += BS2Environment.BS2_CARD_DATA_SIZE;

                        IntPtr ptrChar = Marshal.StringToHGlobalAnsi(strIdentificacion);
                        IntPtr pinCode = Marshal.AllocHGlobal(BS2Environment.BS2_PIN_HASH_SIZE);
                        BS2ErrorCode result2 = (BS2ErrorCode)API.BS2_MakePinCode(sdkContext, ptrChar, pinCode);

                        Marshal.Copy(pinCode, userBlob[0].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                        Marshal.FreeHGlobal(ptrChar);
                        Marshal.FreeHGlobal(pinCode);
                    }
                    else
                    {
                        numeroTarjeta = "";
                        userBlob[0].cardObjs = IntPtr.Zero;
                    }

                    Array.Clear(userBlob[0].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                }

                userBlob[0].setting.startTime = 0;
                userBlob[0].setting.endTime = 0;
                userBlob[0].setting.idAuthMode = 8;
                userBlob[0].setting.securityLevel = 3;

                //TIPO DE ACCESO
                userBlob[0].setting.fingerAuthMode = 0;
                userBlob[0].setting.cardAuthMode = 2; // o 2

                Array.Clear(userBlob[0].name, 0, BS2Environment.BS2_USER_NAME_LEN);
                userBlob[0].photo.size = 0;
                Array.Clear(userBlob[0].photo.data, 0, BS2Environment.BS2_USER_PHOTO_SIZE);

                //Array.Clear(userBlob[0].pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);

                userBlob[0].faceObjs = IntPtr.Zero;
                Array.Clear(userBlob[0].accessGroupId, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER);
                int access_group_count = 0;

                foreach (string itemGrupo in grupoAccesoUno)
                {
                    if (itemGrupo != null && itemGrupo != "" && itemGrupo != " ")
                    {
                        userBlob[0].accessGroupId[access_group_count++] = Convert.ToUInt32(itemGrupo) + 1;
                    }
                }

                int delayTerminate = 0;
                Thread.Sleep(delayTerminate);
                BS2ErrorCode result = (BS2ErrorCode)API.BS2_EnrolUser(sdkContext, deviceID, userBlob, 1, 1);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    GenerarArchivo("EnrrolarHuellaBioEntry", "No se pudo actualizar el usuario " + (strIdentificacion.Length == 0 ? indiceDedo : strIdentificacion) + ", el error fue: " + (BS2ErrorCode)result, "errores", (numeroTarjeta.Length == 0 ? indiceDedo : numeroTarjeta));
                    return respuestaDescarga.errorComandoEnrrolHuellas;
                }
                else
                {
                    GenerarArchivo("EnrrolarHuellaBioEntry", "Se actualizo el usuario " + (strIdentificacion.Length == 0 ? indiceDedo : strIdentificacion), "transacciones", drvFila["strIpTerminal"].ToString());
                    return respuestaDescarga.huellaEnrroladaSatisfactoriamente;
                }
            }

            catch (Exception ex)
            {
                GenerarArchivo("EnrrolarHuellaBioEntry", ex.Message, "errores", drvFila["strIpTerminal"].ToString());
                return respuestaDescarga.errorComandoEnrrolHuellas;
            }
        }

        private byte[] convertirHuella(byte[] huella)
        {
            byte[] btTEmplate = new byte[BS2Environment.BS2_FINGER_TEMPLATE_SIZE * 2];
            Array.Copy(huella, btTEmplate, BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
            Array.Copy(huella, 0, btTEmplate, BS2Environment.BS2_FINGER_TEMPLATE_SIZE, BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
            return btTEmplate;
        }

        private byte[] codigoTarjeta(string codigo)
        {
            Int64 intValue = Int64.Parse(codigo);
            string hexValue = intValue.ToString("X");
            byte[] dato = new byte[32];
            int cantidad = (hexValue.Length / 2) + (hexValue.Length % 2);
            for (int i = 1; i <= cantidad; i++)
            {
                dato[32 - i] = byte.Parse(hexValue.Substring((hexValue.Length - (i * 2)) == -1 ? 0 : (hexValue.Length - (i * 2)), (hexValue.Length - (i * 2)) == -1 ? 1 : 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return dato;
        }

        public respuestaDescarga EnrrolarMultiplesHuellasBioEntry(DataRow drvFila, tipoAccesoLector[] tipoAcceso, string[] numeroTarjeta, object[] datoHuella, object[] datoHuellaDos, object[] datoHuellaTres, object[] datoHuellaCuatro, string[] indiceDedo, string[] grupoAccesoUno, string[] grupoAccesoDos, uint idDispositivo, bool[] autorizado, string strIdentificacion)
        {
            if (!bConectada)
            {
                return respuestaDescarga.terminalOcupada;
            }
            for (int j = 0; j < numeroTarjeta.Length; j++)
            {
                if (EnrrolarHuellaBioEntry(drvFila, tipoAcceso[j], numeroTarjeta[j], (byte[])datoHuella[j], (byte[])datoHuellaDos[j], (byte[])datoHuellaTres[j], (byte[])datoHuellaCuatro[j], indiceDedo[j], grupoAccesoUno, grupoAccesoDos, idDispositivo, autorizado[j], strIdentificacion) != respuestaDescarga.huellaEnrroladaSatisfactoriamente)
                {
                    return respuestaDescarga.errorComandoEnrrolHuellas;
                }
            }

            return respuestaDescarga.huellaEnrroladaSatisfactoriamente;
        }

        public respuestaDescarga EnrrolarMultiplesHuellasBioEntryNuevo(DataRow drvFila, string numeroTarjeta, byte[] datoHuella, string indiceDedo, string[] grupoAccesoUno, string[] grupoAccesoDos, uint idDispositivo, bool autorizado, string strIdentificacion, bool ingresoSinHuella)
        {
            if (!bConectada)
            {
                return respuestaDescarga.terminalOcupada;
            }

                if (EnrrolarHuellaBioEntryNuevo(drvFila, numeroTarjeta, (byte[])datoHuella, indiceDedo, grupoAccesoUno, grupoAccesoDos, idDispositivo, autorizado, strIdentificacion, ingresoSinHuella) != respuestaDescarga.huellaEnrroladaSatisfactoriamente)
                {
                    return respuestaDescarga.errorComandoEnrrolHuellas;
                }

            return respuestaDescarga.huellaEnrroladaSatisfactoriamente;
        }

        public BS2ErrorCode BS2_GetFingerprintConfig(IntPtr sdkContext, UInt32 deviceId, out BS2FingerprintConfig config)
        {

            return (BS2ErrorCode)API.BS2_GetFingerprintConfig(sdkContext, deviceId, out config);

        }

        public BS2ErrorCode BS2_ScanFingerprintEx(IntPtr context, UInt32 deviceId, ref BS2Fingerprint finger, UInt32 templateIndex, UInt32 quality, byte templateFormat, out UInt32 outquality)
        {
            cbFingerOnReadyToScan = new API.OnReadyToScan(ReadyToScanForFinger);
            return (BS2ErrorCode)API.BS2_ScanFingerprintEx(sdkContext, deviceId, ref finger, templateIndex, quality, templateFormat, out outquality, cbFingerOnReadyToScan);
        }

        public BS2ErrorCode BS2_GetFingerTemplateQuality(IntPtr templatePtr, out Int32 score)
        {
            return (BS2ErrorCode)API.BS2_GetFingerTemplateQuality(templatePtr, (uint)BS2Environment.BS2_FINGER_TEMPLATE_SIZE, out score);
        }

        public BS2ErrorCode BS2_VerifyFingerprint(UInt32 deviceId, ref BS2Fingerprint fingerprint)
        {
            return (BS2ErrorCode)API.BS2_VerifyFingerprint(sdkContext, deviceId, ref fingerprint);
        }

        public respuestaDescarga configurarHorario(DataRow drvFila, int codigoHorario, string simboloHorario, int horaDesde, int minutosDesde, int horaHasta, int minutosHasta, uint codigoLector, int puerta)
        {
            ////BS2ErrorCode resultRemoveAllAuthGroup = (BS2ErrorCode)API.BS2_RemoveAllAuthGroup(sdkContext, deviceID);
            //BS2ErrorCode resultDeleteAccessGroup = (BS2ErrorCode)API.BS2_RemoveAllAccessGroup(sdkContext, deviceID);
            //BS2ErrorCode resultDeleteAccessLevel = (BS2ErrorCode)API.BS2_RemoveAllAccessLevel(sdkContext, deviceID);
            //BS2ErrorCode resultDeleteAccessSchedule = (BS2ErrorCode)API.BS2_RemoveAllAccessSchedule(sdkContext, deviceID);
            //BS2ErrorCode resultRemoveAllDoor = (BS2ErrorCode)API.BS2_RemoveAllDoor(sdkContext, deviceID);
            //BS2ErrorCode resultRemoveAll1 = (BS2ErrorCode)API.BS2_RemoveAllHolidayGroup(sdkContext, deviceID);
            //BS2ErrorCode resultRemoveAll2 = (BS2ErrorCode)API.BS2_RemoveAllBlackList(sdkContext, deviceID);
            //BS2ErrorCode resultRemoveAll3 = (BS2ErrorCode)API.BS2_ClearAllDeviceZoneAccessRecord(sdkContext, deviceID, ((UInt32)codigoHorario + 1));
            ////BS2ErrorCode resultRemoveAllQUITAR = (BS2ErrorCode)API.BS2_ClearDatabase(sdkContext, deviceID);
            //BS2ErrorCode resultRemoveAll4 = (BS2ErrorCode)API.BS2_RemoveAllScheduledLockUnlockZone(sdkContext, deviceID);
            //BS2ErrorCode resultRemoveAll5 = (BS2ErrorCode)API.BS2_RemoveAllFloorLevel(sdkContext, deviceID);
            //BS2ErrorCode resultRemoveAll6 = (BS2ErrorCode)API.BS2_RemoveAllDeviceZone(sdkContext, deviceID);



            if (!crearGrupodeAcceso(drvFila, codigoHorario))
            {
                return respuestaDescarga.NoSePudoCrearElHorario;
            }
            if (!crearNivelAcceso(drvFila, codigoHorario, puerta))
            {
                return respuestaDescarga.NoSePudoCrearElHorario;
            }
            if (!crearHorarioAcceso(drvFila, codigoHorario, simboloHorario, horaDesde, minutosDesde, horaHasta, minutosHasta))
            {
                return respuestaDescarga.NoSePudoCrearElHorario;
            }
            if (!crearPuerta(drvFila, puerta, codigoLector))
            {
                return respuestaDescarga.NoSePudoCrearElHorario;
            }
            if (configurarFechaHora(drvFila) == respuestaDescarga.fechaHoraNoPudoSerActualizada)
            {
                return respuestaDescarga.NoSePudoCrearElHorario;
            }

            return respuestaDescarga.HorarioCreadoSatisfactoriamente;
        }

        private Boolean crearGrupodeAcceso(DataRow drvFila, int codigoHorario)
        {
            List<BS2AccessGroup> accessGroupList = new List<BS2AccessGroup>();

            BS2AccessGroup accessGroup = Util.AllocateStructure<BS2AccessGroup>();
            accessGroup.id = (UInt32)codigoHorario + 1;

            byte[] accessGroupArray = Encoding.UTF8.GetBytes("Grupo de acceso " + codigoHorario.ToString());
            Array.Clear(accessGroup.name, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_NAME_LEN);
            Array.Copy(accessGroupArray, accessGroup.name, accessGroupArray.Length);

            accessGroup.numOflevelUnion.numAccessLevels = 1;
            accessGroup.levelUnion.accessLevels[0] = (uint)codigoHorario + 1;

            accessGroupList.Add(accessGroup);

            int structSize = Marshal.SizeOf(typeof(BS2AccessGroup));
            IntPtr accessGroupListObj = Marshal.AllocHGlobal(structSize * accessGroupList.Count);
            IntPtr curAccessGroupListObj = accessGroupListObj;

            foreach (BS2AccessGroup item in accessGroupList)
            {
                Marshal.StructureToPtr(item, curAccessGroupListObj, false);
                curAccessGroupListObj = (IntPtr)((long)curAccessGroupListObj + structSize);
            }

            GenerarArchivo("crearGrupodeAcceso", "Intentando configurar grupo de acceso", "transacciones", drvFila["strIpTerminal"].ToString());
            int delayTerminate = 0;
            Thread.Sleep(delayTerminate);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetAccessGroup(sdkContext, deviceID, accessGroupListObj, (UInt32)accessGroupList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                GenerarArchivo("crearGrupodeAcceso", "No se pudo escribir la configuración del grupo de acceso", "errores", drvFila["strIpTerminal"].ToString());
                Marshal.FreeHGlobal(accessGroupListObj);
                return false;
            }
            GenerarArchivo("crearGrupodeAcceso", "Inserto la configurar del grupo de acceso", "transacciones", drvFila["strIpTerminal"].ToString());

            Marshal.FreeHGlobal(accessGroupListObj);
            return true;
        }

        private Boolean crearNivelAcceso(DataRow drvFila, int codigoHorario, int numeroPuerta)
        {
            List<BS2AccessLevel> accessLevelList = new List<BS2AccessLevel>();

            BS2AccessLevel accessLevel = Util.AllocateStructure<BS2AccessLevel>();

            accessLevel.id = (UInt32)codigoHorario + 1;
            string accessGroupName = "Nivel de acceso" + codigoHorario.ToString();

            byte[] accessGroupArray = Encoding.UTF8.GetBytes(accessGroupName);
            Array.Clear(accessLevel.name, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_NAME_LEN);
            Array.Copy(accessGroupArray, accessLevel.name, accessGroupArray.Length);

            accessLevel.numDoorSchedules = 1;

            accessLevel.doorSchedules[0].doorID = (UInt32)numeroPuerta;
            accessLevel.doorSchedules[0].scheduleID = (UInt32)codigoHorario + 1;

            accessLevelList.Add(accessLevel);

            int structSize = Marshal.SizeOf(typeof(BS2AccessLevel));
            IntPtr accessLevelListObj = Marshal.AllocHGlobal(structSize * accessLevelList.Count);
            IntPtr curAccessLevelListObj = accessLevelListObj;
            foreach (BS2AccessLevel item in accessLevelList)
            {
                Marshal.StructureToPtr(item, curAccessLevelListObj, false);
                curAccessLevelListObj = (IntPtr)((long)curAccessLevelListObj + structSize);
            }

            GenerarArchivo("crearNivelAcceso", "Intentando configurar nivel de acceso", "transacciones", drvFila["strIpTerminal"].ToString());
            int delayTerminate = 0;
            Thread.Sleep(delayTerminate);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetAccessLevel(sdkContext, deviceID, accessLevelListObj, (UInt32)accessLevelList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                GenerarArchivo("crearNivelAcceso", "No se pudo escribir la configuración del nivel de acceso", "errores", drvFila["strIpTerminal"].ToString());
                Marshal.FreeHGlobal(accessLevelListObj);
                return false;
            }
            GenerarArchivo("crearNivelAcceso", "Inserto la configurar del nivel de acceso", "transacciones", drvFila["strIpTerminal"].ToString());

            Marshal.FreeHGlobal(accessLevelListObj);
            return true;
        }

        private Boolean crearHorarioAcceso(DataRow drvFila, int codigoHorario, string simboloHorario, int horaDesde, int minutosDesde, int horaHasta, int minutosHasta)
        {
            List<CSP_BS2Schedule> accessScheduleList = new List<CSP_BS2Schedule>();

            CSP_BS2Schedule accessSchedule = Util.AllocateStructure<CSP_BS2Schedule>();

            accessSchedule.id = (UInt32)codigoHorario + 1;
            string accessScheduleName = "Horario " + codigoHorario;

            byte[] accessScheduleArray = Encoding.UTF8.GetBytes(accessScheduleName);
            Array.Clear(accessSchedule.name, 0, BS2Environment.BS2_MAX_SCHEDULE_NAME_LEN);
            Array.Copy(accessScheduleArray, accessSchedule.name, accessScheduleArray.Length);

            //accessSchedule.numHolidaySchedules = 1;

            //accessSchedule.holidaySchedules[0].id = (UInt32)codigoHorario + 1;

            //accessSchedule.holidaySchedules[0].schedule.numPeriods = 0;

            accessSchedule.isDaily = 0;

            string strbinarioHorario = decTObin((int)System.Convert.ToChar(simboloHorario));
            if (strbinarioHorario.Length < 7)
            {
                strbinarioHorario = new String(' ', (7 - strbinarioHorario.Length)) + strbinarioHorario;
                strbinarioHorario = strbinarioHorario.Replace(" ", "0");
            }
            for (byte loop = 0; loop < BS2Environment.BS2_NUM_WEEKDAYS; ++loop)
            {
                accessSchedule.scheduleUnion.weekly.schedule[loop].numPeriods = 1;

                if (strbinarioHorario.Substring(6 - loop, 1) == "1")
                {
                    accessSchedule.scheduleUnion.weekly.schedule[loop].periods[0].startTime = (UInt16)(60 * Convert.ToUInt16(horaDesde) + Convert.ToUInt16(minutosDesde));
                    accessSchedule.scheduleUnion.weekly.schedule[loop].periods[0].endTime = (UInt16)(60 * Convert.ToUInt16(horaHasta) + Convert.ToUInt16(minutosHasta));
                }
                else
                {
                    accessSchedule.scheduleUnion.weekly.schedule[loop].periods[0].startTime = (UInt16)(60 * Convert.ToUInt16(0) + Convert.ToUInt16(0));
                    accessSchedule.scheduleUnion.weekly.schedule[loop].periods[0].endTime = (UInt16)(60 * Convert.ToUInt16(0) + Convert.ToUInt16(0));
                }
            }
            accessScheduleList.Add(accessSchedule);

            GenerarArchivo("crearHorarioAcceso", "Intentando configurar el horario de acceso", "transacciones", drvFila["strIpTerminal"].ToString());
            int delayTerminate = 0;
            Thread.Sleep(delayTerminate);
            BS2ErrorCode result = (BS2ErrorCode)API.CSP_BS2_SetAccessSchedule(sdkContext, deviceID, accessScheduleList.ToArray(), (UInt32)accessScheduleList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                GenerarArchivo("crearHorarioAcceso", "No se pudo escribir la configuración del horario de acceso", "errores", drvFila["strIpTerminal"].ToString());
                return false;
            }
            GenerarArchivo("crearHorarioAcceso", "Inserto la configurar del horario de acceso", "transacciones", drvFila["strIpTerminal"].ToString());
            return true;
        }

        private Boolean crearPuerta(DataRow drvFila, int numeroPuerta, uint codigoDispositivo)
        {
            List<BS2Door> doorList = new List<BS2Door>();

            BS2Door door = Util.AllocateStructure<BS2Door>();

            door.doorID = (UInt32)numeroPuerta;
            string doorName = "puerta " + numeroPuerta.ToString();

            byte[] doorArray = Encoding.UTF8.GetBytes(doorName);
            Array.Clear(door.name, 0, BS2Environment.BS2_MAX_DOOR_NAME_LEN);
            Array.Copy(doorArray, door.name, doorArray.Length);

            door.entryDeviceID = codigoDispositivo;
            door.exitDeviceID = codigoDispositivo;

            door.autoLockTimeout = 3;

            door.heldOpenTimeout = 3;

            door.instantLock = 0;

            door.lockFlags = (byte)BS2DoorFlagEnum.NONE;

            door.unlockFlags = (byte)BS2DoorFlagEnum.NONE;

            for (int loop = 0; loop < BS2Environment.BS2_MAX_FORCED_OPEN_ALARM_ACTION; ++loop)
            {
                door.forcedOpenAlarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
            }

            for (int loop = 0; loop < BS2Environment.BS2_MAX_HELD_OPEN_ALARM_ACTION; ++loop)
            {
                door.heldOpenAlarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
            }

            door.unconditionalLock = 0;

            door.dualAuthDevice = (byte)BS2DualAuthDeviceEnum.NO_DEVICE;
            door.dualAuthScheduleID = (UInt32)BS2ScheduleIDEnum.NEVER;
            door.dualAuthTimeout = 0;
            door.dualAuthApprovalType = (byte)BS2DualAuthApprovalEnum.NONE;
            door.numDualAuthApprovalGroups = 0;

            doorList.Add(door);

            int structSize = Marshal.SizeOf(typeof(BS2Door));
            IntPtr doorListObj = Marshal.AllocHGlobal(structSize * doorList.Count);
            IntPtr curDoorListObj = doorListObj;
            foreach (BS2Door item in doorList)
            {
                Marshal.StructureToPtr(item, curDoorListObj, false);
                curDoorListObj = (IntPtr)((long)curDoorListObj + structSize);
            }

            GenerarArchivo("crearPuerta", "Intentando configurar la puerta", "transacciones", drvFila["strIpTerminal"].ToString());
            int delayTerminate = 0;
            Thread.Sleep(delayTerminate);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_SetDoor(sdkContext, deviceID, doorListObj, (UInt32)doorList.Count);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                GenerarArchivo("crearPuerta", "No se pudo escribir la configuración de la puerta", "errores", drvFila["strIpTerminal"].ToString());
                return false;
            }
            GenerarArchivo("crearPuerta", "Inserto la configurar de la puerta", "transacciones", drvFila["strIpTerminal"].ToString());

            Marshal.FreeHGlobal(doorListObj);
            return true;
        }

        private string decTObin(long a)
        {
            string decto = "";
            if ((a / 2) == 0)
            {
                decto = decto + (a % 2);
            }
            else
            {
                decto = decTObin(a / 2) + (a % 2);
            }
            return decto;
        }

        public respuestaDescarga configurarFechaHora(DataRow drvFila)
        {
            BS2SystemConfig configs = Util.AllocateStructure<BS2SystemConfig>();
            int delayTerminate = 0;
            Thread.Sleep(delayTerminate);
            BS2ErrorCode result = 0;
            result = (BS2ErrorCode)API.BS2_GetSystemConfig(sdkContext, deviceID, out configs);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                GenerarArchivo("configurarFechaHora", "No se pudo leer la configuración del sistema", "errores", drvFila["strIpTerminal"].ToString());
                return respuestaDescarga.fechaHoraNoPudoSerActualizada;
            }

            //if (configs.timezone != -18000 || configs.syncTime != 0)
            //{
            configs.syncTime = 0;
            //configs.timezone = -18000;
            configs.timezone = 0;
            result = (BS2ErrorCode)API.BS2_SetSystemConfig(sdkContext, deviceID, ref configs);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                GenerarArchivo("configurarFechaHora", "No se pudo escribir la configuración del sistema", "errores", drvFila["strIpTerminal"].ToString());
                return respuestaDescarga.fechaHoraNoPudoSerActualizada;
            }
            GenerarArchivo("configurarFechaHora", "Se actualizo la zona horaria y la sincronización de hora actual.", "transacciones", drvFila["strIpTerminal"].ToString());
            //}

            UInt32 timestamp = 0;
            result = (BS2ErrorCode)API.BS2_GetDeviceTime(sdkContext, deviceID, out timestamp);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                GenerarArchivo("configurarFechaHora", "No se pudo leer la fecha y hora", "errores", drvFila["strIpTerminal"].ToString());
            }
            else
            {
                DateTime currentTime = Util.ConvertFromUnixTimestamp(timestamp);
                GenerarArchivo("configurarFechaHora", "La fecha actual: " + currentTime.ToString("yyyy-MM-dd HH:mm:ss"), "transacciones", drvFila["strIpTerminal"].ToString());
            }

            GenerarArchivo("configurarFechaHora", "Intentando configurar fecha", "transacciones", drvFila["strIpTerminal"].ToString());

            timestamp = Convert.ToUInt32(Util.ConvertToUnixTimestamp(DateTime.Now));
            result = (BS2ErrorCode)API.BS2_SetDeviceTime(sdkContext, deviceID, timestamp);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                GenerarArchivo("configurarFechaHora", "No se pudo actualizar la hora", "errores", drvFila["strIpTerminal"].ToString());
                return respuestaDescarga.fechaHoraNoPudoSerActualizada;
            }

            GenerarArchivo("configurarFechaHora", "Se actualizo la fecha: " + Util.ConvertFromUnixTimestamp(timestamp).ToString(), "transacciones", drvFila["strIpTerminal"].ToString());
            return respuestaDescarga.fechaHoraActualizadaSatisfactoriamente;
        }

        public respuestaDescarga EliminarHuellaUsuario(DataRow drvFila, int idPuerto, string idCodigoHuella, Boolean todos)
        {
            int delayTerminate = 0;
            Thread.Sleep(delayTerminate);
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;

            if (todos)
            {
                GenerarArchivo("EliminarHuellaUsuario", "Intentando borrar todos los usuarios", "transacciones", drvFila["strIpTerminal"].ToString());
                result = (BS2ErrorCode)API.BS2_RemoveAllUser(sdkContext, deviceID);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    GenerarArchivo("EliminarHuellaUsuario", "No se pudo eliminar todos los usuarios", "errores", drvFila["strIpTerminal"].ToString());
                    return respuestaDescarga.errorBorrandoHuella;
                }

                GenerarArchivo("EliminarHuellaUsuario", "elimino todos los usuarios", "transacciones", drvFila["strIpTerminal"].ToString());
                return respuestaDescarga.huellaBorradaSatisfactoriamente;
            }
            else
            {
                GenerarArchivo("EliminarHuellaUsuario", "Intentando borrar el usuario" + idCodigoHuella.ToString(), "transacciones", drvFila["strIpTerminal"].ToString());
                byte[] uidArray = new byte[BS2Environment.BS2_USER_ID_SIZE];
                byte[] rawUid = Encoding.UTF8.GetBytes(idCodigoHuella.ToString());
                IntPtr uids = Marshal.AllocHGlobal(BS2Environment.BS2_USER_ID_SIZE);

                Array.Clear(uidArray, 0, BS2Environment.BS2_USER_ID_SIZE);
                Array.Copy(rawUid, 0, uidArray, 0, rawUid.Length);
                Marshal.Copy(uidArray, 0, uids, BS2Environment.BS2_USER_ID_SIZE);

                result = (BS2ErrorCode)API.BS2_RemoveUser(sdkContext, deviceID, uids, 1);

                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    GenerarArchivo("EliminarHuellaUsuario", "No se pudo eliminar el usuario " + idCodigoHuella.ToString(), "errores", drvFila["strIpTerminal"].ToString());
                    return respuestaDescarga.errorBorrandoHuella;
                }

                Marshal.FreeHGlobal(uids);

                GenerarArchivo("EliminarHuellaUsuario", "elimino el usuario" + idCodigoHuella.ToString(), "transacciones", drvFila["strIpTerminal"].ToString());
                return respuestaDescarga.huellaBorradaSatisfactoriamente;
            }
        }

        public bool conectarTerminal(string direccionIp, UInt16 puertoConexion, bool mostrarLog = false)
        {
            desconectarTerminal();

            IntPtr versionPtr = API.BS2_Version();

            sdkContext = API.BS2_AllocateContext();
            if (sdkContext == IntPtr.Zero)
            {
                GenerarArchivo("conectarTerminal", "No se pudo asignar el contexto del SDK", "errores", direccionIp);
                return false;
            }
            int delayTerminate = 0;
            Thread.Sleep(delayTerminate);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_Initialize(sdkContext);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                GenerarArchivo("conectarTerminal", "No se pudo inicializar en SDK : " + result, "errores", direccionIp);
                API.BS2_ReleaseContext(sdkContext);
                sdkContext = IntPtr.Zero;
                return false;
            }

            cbOnDeviceFound = new API.OnDeviceFound(DeviceFound);
            cbOnDeviceAccepted = new API.OnDeviceAccepted(DeviceAccepted);
            cbOnDeviceConnected = new API.OnDeviceConnected(DeviceConnected);
            cbOnDeviceDisconnected = new API.OnDeviceDisconnected(DeviceDisconnected);

            result = (BS2ErrorCode)API.BS2_SetDeviceEventListener(sdkContext,
                                                                cbOnDeviceFound,
                                                                cbOnDeviceAccepted,
                                                                cbOnDeviceConnected,
                                                                cbOnDeviceDisconnected);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                GenerarArchivo("conectarTerminal", "no se pudo registra una llamada de una función del sdk" + result, "errores", direccionIp);
                API.BS2_ReleaseContext(sdkContext);
                sdkContext = IntPtr.Zero;
                return false;
            }

            //#if SDK_AUTO_CONNECTION
            //            result = (BS2ErrorCode)API.BS2_SetAutoConnection(sdkContext, 1);
            //#endif

            if (!ConnectToDevice(direccionIp, puertoConexion, mostrarLog))
            {
                deviceID = 0;
                return false;
            }
            //BS2FingerprintConfig fingerprintConfig;
            //BS2FingerprintTemplateFormatEnum templateFormat = BS2FingerprintTemplateFormatEnum.FORMAT_SUPREMA;
            //clsComBioEntryPlus2 confclsComBioEntryPlus2 = new clsComBioEntryPlus2();

            //BS2ErrorCode reuslt = BS2_GetFingerprintConfig(sdkContext, 538861281, out fingerprintConfig);

            //templateFormat = (BS2FingerprintTemplateFormatEnum)fingerprintConfig.templateFormat;

            //BS2UserBlobEx userBlob = Util.AllocateStructure<BS2UserBlobEx>();
            //userBlob.user.numFingers = 1;

            //int structSize = Marshal.SizeOf(typeof(BS2Fingerprint));
            //BS2Fingerprint fingerprint = Util.AllocateStructure<BS2Fingerprint>();
            //userBlob.fingerObjs = Marshal.AllocHGlobal(structSize * userBlob.user.numFingers);
            //IntPtr curFingerObjs = userBlob.fingerObjs;
            //confclsComBioEntryPlus2.inicializarAlParcero();
            //UInt32 outquality;

            //reuslt = (BS2ErrorCode)API.BS2_ScanFingerprintEx(sdkContext, 538861281, ref fingerprint, 0, (UInt32)BS2FingerprintQualityEnum.QUALITY_STANDARD, (byte)templateFormat, out outquality, cbFingerOnReadyToScan);

            return true;
        }

        public byte[] capturarHuellaTerminalBiolite(string ipAddress, string snTerminal)
        {
            int delayTerminate = 0;
            Thread.Sleep(delayTerminate);
            BS2ErrorCode result = BS2ErrorCode.BS_SDK_SUCCESS;
            //Console.WriteLine("Trying to get fingerprint config");


            BS2FingerprintTemplateFormatEnum templateFormat = BS2FingerprintTemplateFormatEnum.FORMAT_SUPREMA;
            BS2FingerprintConfig fingerprintConfig;

            BS2ErrorCode reuslt = BS2_GetFingerprintConfig(sdkContext, Convert.ToUInt32(snTerminal), out fingerprintConfig);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                //Console.WriteLine("Got error({0}).", result);
                GenerarArchivo("conectarTerminal", "No se pudo Conectar la terminal con el sdk" + result, "errores", ipAddress);
                return null;
            }
            else
            {
                templateFormat = (BS2FingerprintTemplateFormatEnum)fingerprintConfig.templateFormat;
            }

            BS2UserBlobEx userBlob = Util.AllocateStructure<BS2UserBlobEx>();
            userBlob.user.numFingers = 1;
            int structSize = Marshal.SizeOf(typeof(BS2Fingerprint));
            BS2Fingerprint fingerprint = Util.AllocateStructure<BS2Fingerprint>();
            userBlob.fingerObjs = Marshal.AllocHGlobal(structSize * userBlob.user.numFingers);
            IntPtr curFingerObjs = userBlob.fingerObjs;


            UInt32 outquality;
            for (int idx = 0; idx < userBlob.user.numFingers; ++idx)
            {

                for (UInt32 templateIndex = 0; templateIndex < BS2Environment.BS2_TEMPLATE_PER_FINGER;)
                {
                    //result = (BS2ErrorCode)API.BS2_ScanFingerprintEx(sdkContext, deviceID, ref fingerprint, templateIndex, (UInt32)BS2FingerprintQualityEnum.QUALITY_STANDARD, (byte)templateFormat, out outquality, cbFingerOnReadyToScan);
                    result = BS2_ScanFingerprintEx(sdkContext, Convert.ToUInt32(snTerminal), ref fingerprint, templateIndex, (UInt32)BS2FingerprintQualityEnum.QUALITY_STANDARD, (byte)templateFormat, out outquality);
                    if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                    {
                        if (result == BS2ErrorCode.BS_SDK_ERROR_EXTRACTION_LOW_QUALITY ||
                            result == BS2ErrorCode.BS_SDK_ERROR_CAPTURE_LOW_QUALITY)
                        {
                            GenerarArchivo("conectarTerminal", "No se pudo conectar al dispositivo" + result, "errores", ipAddress);
                        }
                        else
                        {

                            return null;
                        }
                    }
                    else
                    {
                        Int32 score = 0;
                        IntPtr templatePtr = Marshal.AllocHGlobal(fingerprint.data.Length);
                        Marshal.Copy(fingerprint.data, (int)(templateIndex * BS2Environment.BS2_FINGER_TEMPLATE_SIZE), templatePtr, (int)BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
                        result = BS2_GetFingerTemplateQuality(templatePtr, out score);
                        if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                        {
                            // Console.WriteLine("Got error({0})", result);
                        }
                        else
                        {
                            string decision;
                            if (80 < score)
                                decision = "Best";
                            else if (60 < score)
                                decision = "Good";
                            else if (40 < score)
                                decision = "Normal";
                            else if (20 < score)
                                decision = "Bad";
                            else // (0 ~ 20)
                                decision = "Worst";

                            //Console.WriteLine("Template {0} quality ({1}) - {2}", templateIndex, score, decision);
                        }

                        ++templateIndex;
                    }
                }

                //Console.WriteLine("Verify the fingerprints.");
                result = BS2_VerifyFingerprint(Convert.ToUInt32(snTerminal), ref fingerprint);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    if (result == BS2ErrorCode.BS_SDK_ERROR_NOT_SAME_FINGERPRINT)
                    {
                        //Console.WriteLine("The fingerprint does not match. Try again");
                        GenerarArchivo("EnrrolarHuellaBioEntry", "Error no es la misma huella registrada. " + result, "errores", ipAddress);
                        --idx;
                        continue;
                    }
                    else
                    {
                        // Console.WriteLine("Got error({0}).", result);
                        return null;
                    }
                }

                fingerprint.index = (byte)idx;

                //fingerprint.flag = Util.GetInput((byte)BS2FingerprintFlagEnum.NORMAL);

                Marshal.StructureToPtr(fingerprint, curFingerObjs, false);
                curFingerObjs += structSize;
            }

            Type type = typeof(BS2Fingerprint);
            IntPtr curObjs = userBlob.fingerObjs;

            BS2Fingerprint finger = (BS2Fingerprint)Marshal.PtrToStructure(curObjs, type);

            return finger.data;
        }

        public void desconectarTerminal()
        {
            //result = (BS2ErrorCode)API.BS2_SetServerPort(sdkContext, port);

            //#if !SDK_AUTO_CONNECTION
            if (reconnectionTask != null)
            {
                reconnectionTask.stop();
                reconnectionTask = null;
            }
            //#endif
            int delayTerminate = 0;
            Thread.Sleep(delayTerminate);
            BS2ErrorCode result;
            //GenerarArchivo("desconectarTerminal", "Intentando desconectar dispositivo", "transacciones", deviceID.ToString());
            result = (BS2ErrorCode)API.BS2_DisconnectDevice(sdkContext, deviceID);
            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                sdkContext = IntPtr.Zero;
                Thread.Sleep(delayTerminate);
                //GenerarArchivo("desconectarTerminal", "error al desconectar terminal", "errores", deviceID.ToString());
            }
            else
            {


                if (sdkContext != IntPtr.Zero)
                {
                    API.BS2_ReleaseContext(sdkContext);
                }
                sdkContext = IntPtr.Zero;
                Thread.Sleep(delayTerminate);
            }
        }

        private void GenerarArchivo(string nombreFuncion, string mensaje, string nombrearchivo, string ipGenera)
        {
            try
            {
                string ubicacionLogs = Application.StartupPath + "\\Audit" + "\\" + nombreFuncion + "\\" + ipGenera;
                validarRuta(ubicacionLogs);
                System.IO.StreamWriter file = new System.IO.StreamWriter(ubicacionLogs + "\\" + nombrearchivo + DateTime.Now.ToString("yyyyMMdd") + ipGenera + ".log", true);
                file.WriteLine(mensaje + " , función: " + nombreFuncion + ", Fecha generacion: " + DateTime.Now);
                file.Close();
                file.Dispose();
            }
            catch
            { }
        }

        private void validarRuta(string ruta)
        {
            if (!System.IO.File.Exists(ruta))
            {
                System.IO.Directory.CreateDirectory(ruta);
            }
        }

        bool ConnectToDevice(string direccionIP, UInt16 puertoConexion, bool mostrarLog = false)
        {
            strIpActual = direccionIP;
            IPAddress ipAddress;

            if (!IPAddress.TryParse(strIpActual, out ipAddress))
            {
                if (mostrarLog == true)
                {
                    GenerarArchivo("conectarTerminal", "Direccion IP erronea " + strIpActual, "errores", strIpActual);
                }
                return false;
            }

            UInt16 port = (UInt16)BS2Environment.BS2_TCP_DEVICE_PORT_DEFAULT;
            if (puertoConexion > 0)
            {
                port = puertoConexion;
            }

            if (mostrarLog == true)
            {
                GenerarArchivo("conectarTerminal", "Intentado conectar con terminal ip:" + strIpActual + " port:" + port, "transacciones", strIpActual);
            }

            IntPtr ptrIPAddr = Marshal.StringToHGlobalAnsi(strIpActual);
            int delayTerminate = 0;
            Thread.Sleep(delayTerminate);
            BS2ErrorCode result = (BS2ErrorCode)API.BS2_ConnectDeviceViaIP(sdkContext, ptrIPAddr, port, out deviceID);

            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (mostrarLog == true)
                {
                    GenerarArchivo("conectarTerminal", "No se pudo conectar con terminal " + strIpActual + ", Error: " + result.ToString(), "errores", strIpActual);
                }
                return false;
            }
            Marshal.FreeHGlobal(ptrIPAddr);

            if (mostrarLog == true)
            {
                GenerarArchivo("conectarTerminal", "conexion satisfactoria con terminal ip:" + strIpActual + " port:" + port, "transacciones", strIpActual);
            }
            return true;
        }
    }
}
