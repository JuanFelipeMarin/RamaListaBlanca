using Sibo.WhiteList.Service.BLL.Log;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Classes
{
    public class clsWinSockCliente
    {
        #region Attributes
        private Stream stm;
        private Thread tcpThd;
        private Thread tcpCnx;
        private TcpClient tcpClient;
        public bool connected = false, replicating = false, withWithelist = false;
        public int terminalTypeId = 0;
        private bool verifyingConnection = true;
        private string ipAddress;
        private int port;
        byte[] readingBuffer;
        byte[] aux;
        #endregion

        #region Events
        public delegate void ReceivedDataHandler(string ipAddress, byte[] data);
        public event ReceivedDataHandler ReceivedData;
        public delegate void ReceivedDataWithObjectHandler(byte[] data, clsWinSockCliente socket);
        public event ReceivedDataWithObjectHandler ReceivedDataWithObject;
        public delegate void FinalizeConnectionHandler(clsWinSockCliente winSocketClient);
        public event FinalizeConnectionHandler FinalizeConnection;
        #endregion

        #region Properties
        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        public int Port
        {
            get { return port; }
            set { port = value; }
        }
        #endregion

        ServiceLog testLog = new ServiceLog();
        
        #region Constructors
        public clsWinSockCliente(string serverIpAddress, int serverPort, bool withWhiteList, bool replicating, int terminalTypeId)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                this.ipAddress = serverIpAddress;
                this.port = serverPort;
                this.withWithelist = withWhiteList;
                this.replicating = replicating;
                this.terminalTypeId = terminalTypeId;
                Connect(false);
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores de las terminales.");
                log.WriteError(ex.Message + " Terminal " + serverIpAddress + " " + ex.StackTrace.ToString());
            }
        }
        #endregion

        #region Public Methods
        public void SendData(string data)
        {
            try
            {
                byte[] writingBuffer;
                writingBuffer = Encoding.ASCII.GetBytes(data);

                if (stm != null)
                {
                    //Envío los datos al Servidor
                    Task.Run(()=> stm.Write(writingBuffer, 0, writingBuffer.Length));
                }
            }
            catch (Exception ex)
            {
                testLog.WriteError(ex.Message);
                throw ex;
            }
        }

        public void SendData(byte[] data)
        {
            try
            {
                string sendData = string.Empty;
                byte[] writingBuffer;

                foreach (byte d in data)
                {
                    sendData += d.ToString("X") + "-";
                }

                writingBuffer = data;

                if (stm != null)
                {
                    //Envío los datos al Servidor
                    stm.Write(writingBuffer, 0, writingBuffer.Length);
                }
            }
            catch (Exception ex)
            {
                testLog.WriteError(ex.Message);
                throw ex;
            }
        }

        public void CloseConnection()
        {
            try
            {
                verifyingConnection = false;

                if (stm != null)
                {
                    stm.Close();
                }

                stm = null;
            }
            catch (Exception ex)
            {
                testLog.WriteError(ex.Message);
                throw ex;
            }
        }
        #endregion

        #region Private Methods
        private void Connect(bool reconnection)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                //Se encarga de escuchar mensajes enviados por el Servidor
                tcpClient = new TcpClient();
                //Me conecto al objeto de la clase Servidor, determinado por las propiedades IPDelHost y PuertoDelHost
                tcpClient.Connect(ipAddress, port);
                connected = true;
                log.WriteProcessByTerminal("TERMINAL CONECTADA PUERTO: " + port, ipAddress);
                stm = tcpClient.GetStream();
                //Creo e inicio un thread para que escuche los mensajes enviados por el Servidor
                tcpThd = new Thread(new ThreadStart(readSocket));
                tcpThd.Start();

                if (!reconnection)
                {
                    tcpCnx = new Thread(new ThreadStart(verifyConnection));
                    tcpCnx.Start();
                }
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores de las terminales.");
                log.WriteErrorsByTerminals(ex.Message + " " + ex.StackTrace.ToString(), ipAddress);
            }
        }

        private void verifyConnection()
        {
            ServiceLog log = new ServiceLog();

            try
            {
                while (verifyingConnection)
                {
                    try
                    {
                        Thread.Sleep(1000);
                        if (tcpClient != null && tcpClient.Client != null)
                        {
                            if ((!tcpClient.Client.Connected | tcpClient.Client.Poll(1500, SelectMode.SelectRead)) || !connected)
                            {
                                connected = false;

                                if (stm != null)
                                {
                                    stm.Close();
                                    stm = null;
                                }

                                if (tcpClient != null)
                                {
                                    tcpClient.Close();
                                    tcpClient = null;
                                }

                                Connect(true);
                            }
                        }
                        else
                        {
                            Connect(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores de las terminales.");
                        log.WriteError(ex.Message + " Terminal " + ipAddress + " " + ex.StackTrace.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores de las terminales.");
                log.WriteError(ex.Message + " Terminal " + ipAddress + " " + ex.StackTrace.ToString());
            }
        }

        private async void readSocket()
        {
            ServiceLog log = new ServiceLog();

            try
            {
                while (true)
                {
                    try
                    {
                        int ret = 0;
                        readingBuffer = new byte[25600];
                        //Me quedo esperando a que llegue algún mensaje
                        ret = stm.Read(readingBuffer, 0, readingBuffer.Length);
                        //Genero el evento 'ReceivedData', ya que se han recibido datos desde el Servidor
                        if (ret > 0)
                        {
                            //readingBuffer = Encoding.ASCII.GetBytes(Encoding.ASCII.GetString(readingBuffer).Replace(13.ToString(), "").Replace(10.ToString(), "").Replace(0.ToString(), "").Replace(" ", ""));
                            //Cambiamos el tamaño del array
                            aux = new byte[ret];

                            if (readingBuffer != null)
                            {
                                Array.Copy(readingBuffer, aux, Math.Min(readingBuffer.Length, aux.Length));
                            }

                            readingBuffer = aux;

                            await Task.Run(() => ReceivedData(ipAddress, readingBuffer));
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        testLog.WriteError(ex.Message);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores de las terminales.");
                log.WriteError(ex.Message + " Terminal " + ipAddress + " " + ex.StackTrace.ToString());
            }
        }
        #endregion
    }
}
