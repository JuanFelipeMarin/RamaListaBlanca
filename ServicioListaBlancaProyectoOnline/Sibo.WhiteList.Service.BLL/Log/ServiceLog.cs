using Newtonsoft.Json;
using Sibo.WhiteList.Service.BLL.BLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sibo.WhiteList.Service.BLL.Log
{
    public class ServiceLog
    {
        private const string NAME_FOLDER_ERRORS = "Errors";
        private const string NAME_FOLDER_PROCESS = "Process";
        private const string NAME_FOLDER_TERMINALS = "Terminals";

        public void WriteProcess(string msg)
        {
            try
            {
                string path = string.Empty, date = string.Empty;

                path = Application.StartupPath + "\\Audit\\" + NAME_FOLDER_PROCESS;

                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path + "\\" + NAME_FOLDER_PROCESS + DateTime.Now.ToString(" yyyy-MM-dd") + ".txt", true))
                {
                    file.WriteLine("- " + msg + "  (Fecha: " + System.DateTime.Now + ")");
                }

                ServiceLogBLL logBLL = new ServiceLogBLL();
                logBLL.InsertProcess(msg);
            }
            catch (Exception ex)
            {
                WriteError("Error al escribir un proceso. - Acción: " + msg + " - Error: " + ex.Message);
            }
        }

        public void WriteError(string msg)
        {
            try
            {
                string path = string.Empty, date = string.Empty;

                path = Application.StartupPath + "\\Audit\\" + NAME_FOLDER_ERRORS;

                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path + "\\" + NAME_FOLDER_ERRORS + DateTime.Now.ToString(" yyyy-MM-dd") + ".txt", true))
                {
                    file.WriteLine("- " + msg + "  (Fecha: " + System.DateTime.Now + ")");
                }

                ServiceLogBLL logBLL = new ServiceLogBLL();
                logBLL.InsertError(msg);
            }
            catch (Exception ex)
            {
                WriteProcess(msg);
            }
        }

        public void WriteTerminals(string msg, string ipAddress)
        {
            try
            {
                string path = string.Empty, date = string.Empty;

                path = Application.StartupPath + "\\Audit\\" + NAME_FOLDER_TERMINALS;

                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path + "\\" + ipAddress + " - " + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true))
                {
                    file.WriteLine("- " + msg + "  (Fecha: " + System.DateTime.Now + ")");
                }

                ServiceLogBLL logBLL = new ServiceLogBLL();
                logBLL.InsertProcess(msg);
            }
            catch (Exception ex)
            {
                WriteError("Error al escribir un proceso. - Acción: " + msg + " - Error: " + ex.Message);
            }
        }

        public void WriteErrorsByTerminals(string msg, string ipAddress)
        {
            try
            {
                string path = string.Empty, date = string.Empty;

                path = Application.StartupPath + "\\Audit\\" + NAME_FOLDER_ERRORS;

                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path + "\\" + ipAddress + " - " + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true))
                {
                    file.WriteLine("- " + msg + "  (Fecha: " + System.DateTime.Now + ")");
                }

                ServiceLogBLL logBLL = new ServiceLogBLL();
                logBLL.InsertProcess(msg);
            }
            catch (Exception ex)
            {
                WriteError("Error al escribir un proceso. - Acción: " + msg + " - Error: " + ex.Message);
            }
        }

        public void WriteProcessByTerminal(string msg, string ipAddress)
        {
            try
            {
                string path = string.Empty, date = string.Empty;

                path = Application.StartupPath + "\\Audit\\" + NAME_FOLDER_PROCESS;

                if (System.IO.Directory.Exists(path) == false)
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path + "\\" + ipAddress + " - " + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true))
                {
                    file.WriteLine("- " + msg + "  (Fecha: " + System.DateTime.Now + ")");
                }

                ServiceLogBLL logBLL = new ServiceLogBLL();
                logBLL.InsertProcess(msg);
            }
            catch (Exception ex)
            {
                WriteError("Error al escribir un proceso. - Acción: " + msg + " - Error: " + ex.Message);
            }
        }


        public void GenerarArchivolOG(string nombreFuncion, string mensaje, string nombrearchivo, string ipGenera)
        {
            try
            {
                string ubicacionLogs = Application.StartupPath + "\\Audit" + "\\" + nombreFuncion + "\\" + ipGenera;
                validarRutalOG(ubicacionLogs);
                System.IO.StreamWriter file = new System.IO.StreamWriter(ubicacionLogs + "\\" + nombrearchivo + DateTime.Now.ToString("yyyyMMdd") + ipGenera + ".log", true);
                file.WriteLine(mensaje + " , función: " + nombreFuncion + ", Fecha generacion: " + DateTime.Now);
                file.Close();
                file.Dispose();
            }
            catch
            { }
        }

        private void validarRutalOG(string ruta)
        {
            if (!System.IO.File.Exists(ruta))
            {
                System.IO.Directory.CreateDirectory(ruta);
            }
        }

        public void GenerarArchivolOG(string v1, string v2, string v3)
        {
            throw new NotImplementedException();
        }
        //public void WriteErrorsTerminals(string msg, string ipAddress)
        //{
        //    try
        //    {
        //        string path = string.Empty, date = string.Empty;

        //        path = Application.StartupPath + "\\Audit\\Terminals\\Errors\\";

        //        if (System.IO.Directory.Exists(path) == false)
        //        {
        //            System.IO.Directory.CreateDirectory(path);
        //        }
        //        System.IO.StreamWriter file;
        //        file = new System.IO.StreamWriter(path + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true);

        //        file.WriteLine("- " + ipAddress + " - " + msg + "  (Fecha: " + System.DateTime.Now + ")");
        //        file.Close();

        //        ServiceLogBLL logBLL = new ServiceLogBLL();
        //        logBLL.InsertTerminalError(msg, ipAddress);
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteError(ex.Message);
        //    }
        //}

        //public void WriteTerminalConnections(string msg, string ipAddress)
        //{
        //    try
        //    {
        //        string path = string.Empty, date = string.Empty;

        //        path = Application.StartupPath + "\\Audit\\Terminals\\Connections\\";

        //        if (System.IO.Directory.Exists(path) == false)
        //        {
        //            System.IO.Directory.CreateDirectory(path);
        //        }
        //        System.IO.StreamWriter file;
        //        file = new System.IO.StreamWriter(path + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true);

        //        file.WriteLine("- " + ipAddress + " - " + msg + "  (Fecha: " + System.DateTime.Now + ")");
        //        file.Close();

        //        ServiceLogBLL logBLL = new ServiceLogBLL();
        //        logBLL.InsertTerminalConnections(msg, ipAddress);
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteError(ex.Message);
        //    }
        //}


    }

    public class leerJson
    {
        ServiceLog log = new ServiceLog();
        public bool DescargaConfiguraciones(string json, int tipo)
        {
            bool respuesta = false;
            try
            {

                string configuracion = "";

                if (tipo == 1)
                {
                    configuracion = "listaConfiguracion";
                }

                if (tipo == 2)
                {
                    configuracion = "listaTerminales";
                }
                if (tipo == 3)
                {
                    configuracion = "listaZonas";
                }
                if (tipo == 4)
                {
                    configuracion = "listaFestivos";
                }
                if (tipo == 5)
                {
                    configuracion = "listaMensajeClientes";
                }

                string pathPrincipal = string.Empty, date = string.Empty;

                pathPrincipal = Application.StartupPath + "\\descargaConfiguraciones\\";

                if (System.IO.Directory.Exists(pathPrincipal) == false)
                {
                    System.IO.Directory.CreateDirectory(pathPrincipal);
                }

                    pathPrincipal = pathPrincipal + "\\" + configuracion;

                    if (System.IO.Directory.Exists(pathPrincipal) == true)
                    {
                        /// elimina el archivo 

                        foreach (string archivo in Directory.GetFiles(pathPrincipal))
                        {
                            File.Delete(archivo);
                        }

                        Directory.Delete(pathPrincipal);
                    }
                                        
                    if (System.IO.Directory.Exists(pathPrincipal) == false)
                    {
                        System.IO.Directory.CreateDirectory(pathPrincipal);
                    }

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathPrincipal + "\\" + configuracion + ".json", true))
                    {
                        file.WriteLine(json);
                        ServiceLogBLL logBLL = new ServiceLogBLL();
                        logBLL.InsertProcess(pathPrincipal);
                        respuesta = true;
                    }

                
                return respuesta;
            }
            catch (Exception ex)
            {

                log.WriteError("Error al escribir un proceso. - Acción: " + json + " - Error: " + ex.Message);
                return respuesta;
            }
        }


        public bool guardadoDeEntradasTXT(string json, int tipo)
        {
            bool respuesta = false;
            try
            {

                string configuracion = "";

                if (tipo == 1)
                {
                    configuracion = "ListarEventoEntrada";
                }

                
                string pathPrincipal = string.Empty, date = string.Empty;

                pathPrincipal = Application.StartupPath + "\\descargaConfiguraciones\\";

                if (System.IO.Directory.Exists(pathPrincipal) == false)
                {
                    System.IO.Directory.CreateDirectory(pathPrincipal);
                }

                //pathPrincipal = pathPrincipal + "\\" + configuracion;

                //if (System.IO.Directory.Exists(pathPrincipal) == true)
                //{
                //    /// elimina el archivo 

                //    foreach (string archivo in Directory.GetFiles(pathPrincipal))
                //    {
                //        File.Delete(archivo);
                //    }

                //    Directory.Delete(pathPrincipal);
                //}

                //if (System.IO.Directory.Exists(pathPrincipal) == false)
                //{
                //    System.IO.Directory.CreateDirectory(pathPrincipal);
                //}

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathPrincipal + "\\" + configuracion + DateTime.Now.ToString("yyyyMMdd") + ".txt", true))
                {
                    file.WriteLine(json);
                    file.Close();
                    respuesta = true;
                }


                return respuesta;
            }
            catch (Exception ex)
            {

                log.WriteError("Error al escribir un proceso. - Acción: " + json + " - Error: " + ex.Message);
                return respuesta;
            }
        }
        public string cargarArchivos(int tipo)
        {
            
            var json = "";
            string pathPrincipal = string.Empty, date = string.Empty , archivo = string.Empty;

            if (tipo == 1)
            {
                archivo = "listaConfiguracion";
            }

            if (tipo == 2)
            {
                archivo = "listaTerminales";
            }

            if (tipo == 3)
            {
                archivo = "listaZonas";
            }
            if (tipo == 4)
            {
                archivo = "listaFestivos";
            }
            if (tipo == 5)
            {
                archivo = "listaMensajeClientes";
            }

            pathPrincipal = Application.StartupPath + "\\descargaConfiguraciones\\"+ archivo ;

            if (System.IO.Directory.Exists(pathPrincipal) == true)
            {
                pathPrincipal = Application.StartupPath + "\\descargaConfiguraciones\\" + archivo + "\\" + archivo + ".json";
                using (StreamReader jsonStream = File.OpenText(pathPrincipal))
                {
                    json = jsonStream.ReadToEnd();
                }
            }
            else
            {
                json = "";
            }

            

            return json;
        }
    }


}
