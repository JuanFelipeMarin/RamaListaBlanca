using Sibo.WhiteList.Service.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.DAL.Data
{
    public class dClientCard
    {
        string spName = "spClientCard";

        public int InsertTable(DataTable dt)
        {
            int resp = 0;
            DataAccess objConn = new DataAccess();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "InsertTable");
                    cmd.Parameters.AddWithValue("@tblClientCards", dt);
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();

                    return resp;
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        public int GetCard(string cardId, DataTable dt)
        {
            //int int_cardId = int.Parse(cardId);
            int resp = 0;
            DataAccess objConn = new DataAccess();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "GetCardById");
                    cmd.Parameters.AddWithValue("@tblClientCards", dt);
                    cmd.Parameters.AddWithValue("@cardId", cardId);
                    resp = Convert.ToInt32(cmd.ExecuteScalar());
                    //resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();

                    return resp;
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        public bool DeleteAll()
        {
            int resp = 0;
            bool response = false;
            DataAccess objConn = new DataAccess();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "DeleteAll");
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();
                    response = true;
                }

                return response;
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        public bool DeleteByClients(string clients)
        {
            int resp = 0;
            bool response = false;
            DataAccess objConn = new DataAccess();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "DeleteByClients");
                    cmd.Parameters.AddWithValue("@clients", clients);
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();
                    response = true;
                }

                return response;
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }
    }
}
