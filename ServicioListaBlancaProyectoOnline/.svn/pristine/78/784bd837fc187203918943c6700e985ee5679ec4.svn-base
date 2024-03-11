using Sibo.WhiteList.Service.Connection;
using System;
using System.Data;
using System.Data.SqlClient;
using Sibo.WhiteList.Service.Entities.Classes;
using System.Collections.Generic;

namespace Sibo.WhiteList.Service.Data
{
    public class dFingerprint
    {
        string spName = "spFingerprint";

        /// <summary>
        /// Método para insertar huellas en la BD local.
        /// Getulio Vargas - 2018-04-05 - OD1307
        /// </summary>
        /// <param name="dt">Se envía como parámetro un DataTable y se recibe en un tipo tabla.</param>
        /// <returns></returns>
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
                    cmd.Parameters.AddWithValue("@tblFingerprint", dt);
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

        public DataTable GetFingerprintsByUser(string id)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "GetFingerprintsByUser");
                    cmd.Parameters.AddWithValue("@clientId", id);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        public DataTable GetAllFingerprints(string id)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "GetAllFingerprints");
                    cmd.Parameters.AddWithValue("@personId", id);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }


        public DataTable GetEliminarHuellas(string ip)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "ContarEliminar");
                    cmd.Parameters.AddWithValue("@ipAddress", ip);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }


        public DataTable GetEliminarTarjetas(string ip)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "ContarTarjetasEliminar");
                    cmd.Parameters.AddWithValue("@ipAddress", ip);


                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }


        public bool GetCabiarEstadoElimarHuellas(string id, string ip)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "CambiarEstadoContarEliminar");
                    cmd.Parameters.AddWithValue("@personId", id);
                    cmd.Parameters.AddWithValue("@ipAddress", ip);


                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();
                    cmd.Dispose();

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

        public bool GetCambiarEstadoElimarTarjetas(string id, string ip)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "CambiarEstadoEliminacionTarjetas");
                    cmd.Parameters.AddWithValue("@personId", id);
                    cmd.Parameters.AddWithValue("@ipAddress", ip);

                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();
                    cmd.Dispose();

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
        public DataTable GetFingerprints()
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "GetFingerprints");

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        public bool Insert(eFingerprint fingerEntity)
        {
            int resp = 0;
            DataAccess objConn = new DataAccess();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = objConn.GetConnection();
                    cmd.Parameters.AddWithValue("@action", "Insert");
                    cmd.Parameters.AddWithValue("@personId", fingerEntity.personId);
                    cmd.Parameters.AddWithValue("@fingerprintId", fingerEntity.id);
                    cmd.Parameters.AddWithValue("@fingerprint", fingerEntity.fingerPrint);
                    cmd.Parameters.AddWithValue("@bitInsert", true);
                    cmd.Parameters.AddWithValue("@bitDelete", false);
                    cmd.Parameters.AddWithValue("@registerDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@bitUsed", true);
                    if (fingerEntity.intIndiceHuellaActual != "" )
                    {
                        cmd.Parameters.AddWithValue("@intIndiceHuellaActual", fingerEntity.intIndiceHuellaActual); 
                    }
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

        public bool InsertReplicaTarjetas(string id , string card, bool state, string ip)
        {
            int resp = 0;
            DataAccess objConn = new DataAccess();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = objConn.GetConnection();
                    cmd.Parameters.AddWithValue("@action", "insertTarjetas");
                    cmd.Parameters.AddWithValue("@personId", id);
                    cmd.Parameters.AddWithValue("@card", card);
                    cmd.Parameters.AddWithValue("@bitEliminado", state);
                    cmd.Parameters.AddWithValue("@ipAddress", ip);


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

        public List<eFingerprint> GetFingerprintsToReplicate(string ipAddress)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "GetFingerprintsToReplicate");
                    cmd.Parameters.AddWithValue("@ipAddress", ipAddress);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return ConvertToEntityList(dt);
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        public DataTable GetTarjetasReplica(string ipAddress)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "GetTarjetasReplica");
                    cmd.Parameters.AddWithValue("@ipAddress", ipAddress);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        private List<eFingerprint> ConvertToEntityList(DataTable dt)
        {
            List<eFingerprint> responseList = new List<eFingerprint>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["reserveId"] == null || row["reserveId"].ToString() == "" || row["reserveId"].ToString() == " ")
                    {
                        row["reserveId"] = 0;
                    }
                    eFingerprint finger = new eFingerprint()
                    {
                        recordId = Convert.ToInt32(row["intPkId"].ToString()),
                        id = Convert.ToInt32(row["fingerprintId"].ToString()),
                        personId = row["personId"].ToString(),
                        fingerPrint = (byte[])row["fingerprint"],
                        typePerson = row["typePerson"].ToString(),
                        planId = row["planId"].ToString(),
                        gymId = Convert.ToInt32(row["gymIdWL"]),
                        branchId = Convert.ToInt32(row["branchIdWL"]),
                        restrictions = row["restrictions"].ToString(),
                        reserveId = (row["reserveId"] == System.DBNull.Value ? "0" : row["reserveId"].ToString()),
                        cardId = row["cardId"].ToString(),
                        withoutFingerprint = Convert.ToBoolean(row["withoutFingerprint"])
                    };

                    responseList.Add(finger);
                }
            }

            return responseList;
        }

        /// <summary>
        /// Método para consultar las huellas para eliminar.
        /// Getulio Vargas - 2018-08-31 - OD 1307
        /// </summary>
        /// <returns></returns>
        public List<eFingerprint> GetFingerprintsToDelete(string ipAddress)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "GetFingerprintsToDelete");
                    cmd.Parameters.AddWithValue("@ipAddress", ipAddress);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return ConvertToEntityList(dt);
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite actualizar como usada una huella eliminada de las terminales.
        /// </summary>
        /// <param name="fingerprintId"></param>
        /// <returns></returns>
        public bool UpdateFingerprintDeleted(int fingerprintId)
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
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "UpdateFingerprintDeleted");
                    cmd.Parameters.AddWithValue("@fingerprintId", fingerprintId);
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();
                    cmd.Dispose();

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

        public eFingerprint GetFingerprint(int fingerprintId)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "Get");
                    cmd.Parameters.AddWithValue("@fingerprintId", fingerprintId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return ConvertToEntity(dt);
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        private eFingerprint ConvertToEntity(DataTable dt)
        {
            eFingerprint response = null;

            if (dt != null && dt.Rows.Count > 0)
            {
                response = new eFingerprint();

                foreach (DataRow row in dt.Rows)
                {
                    response.id = Convert.ToInt32(row["fingerprintId"].ToString());
                    response.personId = row["personId"].ToString();
                    response.fingerPrint = (byte[])row["fingerprint"];
                }
            }

            return response;
        }

        public bool UpdateUsedFingerprints(string fingerprints)
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
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "UpdateUsedFingerprints");
                    cmd.Parameters.AddWithValue("@ids", fingerprints);
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();
                    cmd.Dispose();

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
