using Sibo.WhiteList.Service.Connection;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.DAL.Data
{
    public class dReplicatedUser
    {
        string spName = "spReplicatedUser";

        public bool Insert(eReplicatedUser replicated)
        {
            int resp = 0;
            DataAccess objConn = new DataAccess();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "Insert");
                    cmd.Parameters.AddWithValue("@fingerprintId", replicated.fingerprintId);
                    cmd.Parameters.AddWithValue("@userId", replicated.userId);
                    cmd.Parameters.AddWithValue("@ipAddress", replicated.ipAddress);
                    cmd.Parameters.AddWithValue("@replicationDate", DateTime.Now);
                    resp = cmd.ExecuteNonQuery();

                    if (resp > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        public bool DeleteUserReplicated(int fingerprintId, string userId)
        {
            int resp = 0;
            DataAccess objConn = new DataAccess();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "DeleteUserReplicated");
                    cmd.Parameters.AddWithValue("@fingerprintId", fingerprintId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    resp = cmd.ExecuteNonQuery();

                    if (resp > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

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
                    cmd.Parameters.AddWithValue("@tblReplicatedUsers", dt);
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

        public bool UpdateUsedUsers(string users)
        {
            int resp = 0;
            DataAccess objConn = new DataAccess();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "UpdateUsedUsers");
                    cmd.Parameters.AddWithValue("@ids", users);
                    resp = cmd.ExecuteNonQuery();

                    if (resp > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        public bool DeleteUsersDeleted(string ipAddress, string users)
        {
            int resp = 0;
            DataAccess objConn = new DataAccess();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "DeleteUsers");
                    cmd.Parameters.AddWithValue("@ipAddress", ipAddress);
                    cmd.Parameters.AddWithValue("@users", users);
                    resp = cmd.ExecuteNonQuery();

                    if (resp > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }
    }
}
