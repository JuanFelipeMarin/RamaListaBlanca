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
    public class dUser
    {
        string spName = "spUser";

        public bool InsertTable(DataTable dtUsers)
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
                    cmd.Parameters.AddWithValue("@action", "InsertTable");
                    cmd.Parameters.AddWithValue("@tblUsers", dtUsers);
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();

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
            catch
            {
                objConn.ConnectionDispose();
                return false;
            }
        }

        public bool InsertOrUpdateUsers(DataTable dtUsers)
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
                    cmd.Parameters.AddWithValue("@action", "InsertOrUpdateUsers");
                    cmd.Parameters.AddWithValue("@tblUsers", dtUsers);
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();

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
            catch
            {
                objConn.ConnectionDispose();
                return false;
            }
        }

        public bool Update(string userId, bool withoutFingerprint, int fingerprintId, bool insert, bool delete)
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
                    cmd.Parameters.AddWithValue("@action", "Update");
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@bitInsert", insert);
                    cmd.Parameters.AddWithValue("@withoutFingerprint", withoutFingerprint);
                    cmd.Parameters.AddWithValue("@bitDelete", delete);
                    cmd.Parameters.AddWithValue("@fingerprintId", fingerprintId);
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();

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
            catch
            {
                objConn.ConnectionDispose();
                return false;
            }
        }

        public bool InsertUser(eUser user)
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
                    cmd.Parameters.AddWithValue("@action", "InsertUser");
                    cmd.Parameters.AddWithValue("@userId", user.userId);
                    cmd.Parameters.AddWithValue("@userName", user.userName);
                    cmd.Parameters.AddWithValue("@withoutFingerprint", user.withoutFingerprint);
                    cmd.Parameters.AddWithValue("@fingerprintId", user.fingerprintId);
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();

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
            catch
            {
                objConn.ConnectionDispose();
                return false;
            }
        }

        public bool UpdateUsedUsers(string userList)
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
                    cmd.Parameters.AddWithValue("@action", "UpdateUsedUsers");
                    cmd.Parameters.AddWithValue("@ids", userList);
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();

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
            catch
            {
                objConn.ConnectionDispose();
                return false;
            }
        }

        public List<eUser> GetUsersToReplicate(string ipAddress)
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
                    cmd.Parameters.AddWithValue("@action", "GetUsersToReplicate");
                    cmd.Parameters.AddWithValue("@ipAddress", ipAddress);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                        return ConvertToListEntity(dt);
                    }
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        public List<eUser> GetUsersToDelete(string ipAddress)
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
                    cmd.Parameters.AddWithValue("@action", "GetUsersToDelete");
                    cmd.Parameters.AddWithValue("@ipAddress", ipAddress);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                        return ConvertToListEntity(dt);
                    }
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        public bool UpdateFingerprintId(string userId, int fingerprintId)
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
                    cmd.Parameters.AddWithValue("@action", "UpdateFingerprintId");
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@fingerprintId", fingerprintId);
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();

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
            catch
            {
                objConn.ConnectionDispose();
                return false;
            }
        }

        public eUser GetUser(string userId)
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
                    cmd.Parameters.AddWithValue("@action", "GetUser");
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                        return ConvertToEntity(dt);
                    }
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        private List<eUser> ConvertToListEntity(DataTable dt)
        {
            List<eUser> responseList = new List<eUser>();

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        eUser user = new eUser()
                        {
                            delete = Convert.ToBoolean(row["bitDelete"]??false),
                            fingerprintId = Convert.ToInt32(row["fingerprintId"]??0),
                            id = Convert.ToInt32(row["PK_userId"]??0),
                            insert = Convert.ToBoolean(row["bitInsert"]??false),
                            used = Convert.ToBoolean(row["bitUsed"]??false),
                            userId = row["userId"].ToString(),
                            userName = row["userName"].ToString(),
                            withoutFingerprint = Convert.ToBoolean(row["withoutFingerprint"] ?? false)
                        };

                        responseList.Add(user);
                    }
                }
            }

            return responseList;
        }

        private eUser ConvertToEntity(DataTable dt)
        {
            eUser user = new eUser();

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        user.delete = Convert.ToBoolean(row["bitDelete"] ?? false);
                        user.fingerprintId = Convert.ToInt32(row["fingerprintId"] ?? 0);
                        user.id = Convert.ToInt32(row["PK_userId"] ?? 0);
                        user.insert = Convert.ToBoolean(row["bitInsert"] ?? false);
                        user.used = Convert.ToBoolean(row["bitUsed"] ?? false);
                        user.userId = row["userId"].ToString();
                        user.userName = row["userName"].ToString();
                        user.withoutFingerprint = Convert.ToBoolean(row["withoutFingerprint"] ?? false);
                        break;
                    }
                }
            }

            return user;
        }

        public bool Insert(string userId, string userName, int fingerprintId, bool withoutFingerprint, bool bitInsert, bool bitDelete)
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
                    cmd.Parameters.AddWithValue("@action", "Insert");
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@fingerprintId", fingerprintId);
                    cmd.Parameters.AddWithValue("@withoutFingerprint", withoutFingerprint);
                    cmd.Parameters.AddWithValue("@bitInsert", bitInsert);
                    cmd.Parameters.AddWithValue("@bitDelete", bitDelete);
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();

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
            catch
            {
                objConn.ConnectionDispose();
                return false;
            }
        }
    }
}
