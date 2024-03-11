using Sibo.WhiteList.Service.Connection;
using System;
using System.Data.SqlClient;

namespace Sibo.WhiteList.Service.DAL.Data
{
    public class dServiceLog
    {
        /// <summary>
        /// Método que se comunica con la BD local y ejecutar las acciones del spLog.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="action"></param>
        public void Insert(string msg, string action)
        {
            int resp = 0;
            DataAccess objConn = new DataAccess();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "spLog";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@dateLog", DateTime.Now);
                    cmd.Parameters.AddWithValue("@msg", msg);
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();
                }
            }
            catch
            {
                objConn.ConnectionDispose();
            }
        }
    }
}
