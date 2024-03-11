using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Sibo.WhiteList.Service.Connection
{
    public class DataAccess
    {
        #region Objects
        SqlConnection sqlCon = new SqlConnection(); 
        #endregion

        #region Public Methods
        /// <summary>
        /// Método que entrega la conexión a la BD local.
        /// Getulio Vargas - 2018-04-03 - OD 1307
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetConnection()
        {
            try
            {
                string strConnection = ConfigurationManager.ConnectionStrings["strConnection"].ConnectionString;
                sqlCon = new SqlConnection(strConnection);
                
                if (sqlCon.State == System.Data.ConnectionState.Closed)
                {
                    sqlCon.Open();
                }
                
                return sqlCon;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite descargar la conexión.
        /// Getulio Vargas - 2018-04-03 - OD 1307
        /// </summary>
        /// <returns></returns>
        public bool ConnectionDispose()
        {
            try
            {
                sqlCon.Close();
                sqlCon.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion
    }
}
