using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Connection
{
    public class DataAccess
    {
        #region Objects
        SqlConnection sqlCon = new SqlConnection(); 
        #endregion

        #region Public Methods
        public SqlConnection GetConnection()
        {
            try
            {
                string strConnection = ConfigurationManager.ConnectionStrings["strConnection"].ConnectionString;
                sqlCon = new SqlConnection(strConnection);
                sqlCon.Open();
                return sqlCon;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite descargar la conexión.
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
