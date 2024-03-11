using Sibo.WhiteList.Service.DAL.Data;
using System;

namespace Sibo.WhiteList.Service.BLL.BLL
{
    public class ServiceLogBLL
    {
        #region Public Methods
        /// <summary>
        /// Método que permite insertar un registro en la tabla de log de procesos.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <param name="msg"></param>
        public void InsertProcess(string msg)
        {
            dServiceLog logDAL = new dServiceLog();

            if (!string.IsNullOrEmpty(msg))
            {
                logDAL.Insert(msg, "InsertProcess");
            }

            logDAL = null;
        }

        /// <summary>
        /// Método que permite insertar un registro en la tabla de log de errores.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <param name="msg"></param>
        public void InsertError(string msg)
        {
            dServiceLog logDAL = new dServiceLog();

            if (!string.IsNullOrEmpty(msg))
            {
                logDAL.Insert(msg, "InsertError");
            }

            logDAL = null;
        }

        //public void InsertTerminalError(string msg, string ipAddress)
        //{
        //    try
        //    {
        //        dServiceLog logDAL = new dServiceLog();

        //        if (!string.IsNullOrEmpty(msg))
        //        {
        //            logDAL.Insert(msg, "InsertTerminalError", ipAddress);
        //        }

        //        logDAL = null;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //public void InsertTerminalConnections(string msg, string ipAddress)
        //{
        //    try
        //    {
        //        dServiceLog logDAL = new dServiceLog();

        //        if (!string.IsNullOrEmpty(msg))
        //        {
        //            logDAL.Insert(msg, "InsertTerminalConnection", ipAddress);
        //        }

        //        logDAL = null;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        #endregion
    }
}
