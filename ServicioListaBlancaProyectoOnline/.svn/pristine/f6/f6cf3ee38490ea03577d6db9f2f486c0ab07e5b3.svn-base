using Sibo.WhiteList.Service.Connection;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.DAL.Data
{
    public class dReplicatedFingerprint
    {
        string spName = "spReplicatedFingerprint";

        public bool Insert(eReplicatedFingerprint replicated)
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

        public bool DeleteFingerprintReplicated(int fingerprintId, string userId)
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
                    cmd.Parameters.AddWithValue("@action", "DeleteFingerprintReplicated");
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

        public bool DeleteFingerprintsDeleted(string ipAddress, string fingerprints)
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
                    cmd.Parameters.AddWithValue("@action", "DeleteFingerprints");
                    cmd.Parameters.AddWithValue("@ipAddress", ipAddress);
                    cmd.Parameters.AddWithValue("@fingerprints", fingerprints);
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
