using Newtonsoft.Json;
using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.DAL.Data;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Sibo.WhiteList.Service.BLL.BLL
{
    public class TerminalBLL
    {
        #region Public Methods
        /// <summary>
        /// Método encargado de consultar las terminales de la sucursal en GSW y enviarlas a guardar a la BD local.
        /// Getulio Vargas - 2018-04-12 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public bool GetTerminals(int gymId, string branchId)
        {
            ServiceLog log = new ServiceLog();
            leerJson lg = new leerJson();

            try
            {
                TerminalAPI termAPI = new TerminalAPI();
                List<eTerminal> termList = new List<eTerminal>();
                bool resp = false;
                log.WriteProcess("Consumo de API para descargar la lista de terminales.");
                termList = termAPI.GetTerminals(gymId,branchId);

                if (termList != null && termList.Count > 0)
                {
                    log.WriteProcess("Se procede a guardar o actualizar las terminales en la BD local.");

                    string jsonString = JsonConvert.SerializeObject(termList);
                    resp = lg.DescargaConfiguraciones(jsonString, 2);

                    //resp = InsertOrUpdateTerminals(termList);
                }
                else
                {
                    log.WriteProcess("No se encontraron registros de terminales en GSW para actualizar o insertar en BD local.");
                }

                return resp;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return false;
            }
        }

        /// <summary>
        /// Método que permite consultar la lista de terminales registradas en la BD local.
        /// Getulio Vargas - 2018-04-13 - OD 1307
        /// </summary>
        /// <returns></returns>
        public List<eTerminal> GetTerminals()
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dTerminal termData = new dTerminal();
                List<eTerminal> termList = new List<eTerminal>();
                termList = termData.GetTerminals();
                return termList;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        public List<eTerminal> GetTerminalsBioLite()
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dTerminal termData = new dTerminal();
                List<eTerminal> termList = new List<eTerminal>();
                termList = termData.GetTerminalsBioLite();
                return termList;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        /// <summary>
        /// Método para consultar una terminal por dirección IP.
        /// Getulio Vargas - 2018-06-25 - OD 1307
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public eTerminal GetTerminalByIp(string ipAddress)
        {
            dTerminal termData = new dTerminal();
            eTerminal terminal = new eTerminal();
            terminal = termData.GetTerminalByIp(ipAddress);
            return terminal;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Método encargado de determinar si una terminal se debe insertar o actualizar en la BD local.
        /// Getulio Vargas - 2018-04-12 - OD 1307
        /// </summary>
        /// <param name="termList"></param>
        /// <returns></returns>
        private bool InsertOrUpdateTerminals(List<eTerminal> termList)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dTerminal termData = new dTerminal();
                eTerminal term = new eTerminal();
                bool resp = false;
                log.WriteProcess("Inicia el proceso de inserción o actualización de terminales en BD local.");

                foreach (eTerminal item in termList)
                {
                    term = termData.GetTerminalById(item.terminalId);

                    if (term != null && term.terminalId != 0)
                    {
                        term.CardMode = item.CardMode;
                        term.ICAM7000 = item.ICAM7000;
                        term.ipAddress = item.ipAddress;
                        term.LectorZK = item.LectorZK;
                        term.port = item.port;
                        term.servesToInputAndOutput = item.servesToInputAndOutput;
                        term.servesToOutput = item.servesToOutput;
                        term.speedPort = item.speedPort;
                        term.TCAM7000 = item.TCAM7000;
                        term.timeWaitAnswerReplicate = item.timeWaitAnswerReplicate;
                        term.withWhiteList = item.withWhiteList;
                        //OD 1689 - GETULIO VARGAS - 2020/01/27
                        term.name = item.name;
                        term.terminalTypeId = item.terminalTypeId;
                        term.terminalTypeDescription = item.terminalTypeDescription;
                        term.state = item.state;
                        term.snTerminal = item.snTerminal;
                        term.Zonas = item.Zonas;

                        resp = termData.Update(term);
                    }
                    else
                    {
                        resp = termData.Insert(item);
                    }
                }

                log.WriteProcess("Finaliza proceso de inserción o actualización de terminales en BD local.");
                return resp;
            }
            catch (Exception ex)
            {
                log.WriteError("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteProcess(ex.Message + " " + ex.StackTrace.ToString());
                return false;
            }
        }
        #endregion

        public DataTable consultarTerminal()
        {
            DataTable dtt = new DataTable();
            dTerminal termData = new dTerminal();

            dtt = termData.GetTerminalsTabla();
            return dtt;

        }

        public DataTable consultarTerminalID(int idTerminal)
        {
            DataTable dtt = new DataTable();
            dTerminal termData = new dTerminal();

            dtt = termData.consultarTerminalID(idTerminal);
            return dtt;

        }
    }
}
