using Sibo.WhiteList.Service.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Data
{
    public class dWhiteList
    {
        DataAccess objConn = new DataAccess();

        public DataTable GetWhiteList()
        {
            try
            {
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = objConn.GetConnection();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spWhiteList";
                cmd.Parameters.Add("@strTipoAccion", SqlDbType.VarChar).Value = "GetWhiteList";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                objConn.ConnectionDispose();

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
