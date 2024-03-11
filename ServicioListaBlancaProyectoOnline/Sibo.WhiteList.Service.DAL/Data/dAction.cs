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
    public class dAction
    {
        public int Insert(eAction action)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "spAction";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "Insert");
                    cmd.Parameters.AddWithValue("@ipAddress", action.ipAddress);
                    cmd.Parameters.AddWithValue("@strAction", action.strAction);
                    cmd.Parameters.AddWithValue("@actionDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@used", false);
                    cmd.Parameters.AddWithValue("@actionState", false);
                    cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        resp = Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString());
                    }

                    return resp;
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }


        public void InsertHuellaBioLite(string strip, string snTerminar, string personId, bool bitInsertFingert)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();
            
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "spAction";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "InsertAccionHuellaBioLite");
                    cmd.Parameters.AddWithValue("@ipAddress", strip);
                    cmd.Parameters.AddWithValue("@snTerminar", snTerminar);
                    cmd.Parameters.AddWithValue("@personId", personId);
                    cmd.Parameters.AddWithValue("@bitInsertFingert", bitInsertFingert);


                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                                      
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        public bool Update(eAction action)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "spAction";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "Update");
                    cmd.Parameters.AddWithValue("@id", action.id);
                    cmd.Parameters.AddWithValue("@actionState", action.stateAction);

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
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        public List<eAction> GetPendingActionsByTerminal(string ipAddress)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spAction";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "GetPendingActionsByTerminal");
                    cmd.Parameters.AddWithValue("@ipAddress", ipAddress);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return ConvertToListEntity(dt);
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        private List<eAction> ConvertToListEntity(DataTable dt)
        {
            List<eAction> actionList = new List<eAction>();
            string actualIp = string.Empty;

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    eAction action = new eAction()
                    {
                        dateAction = row["actionDate"].ToString(),
                        id = Convert.ToInt32(row["id"].ToString()),
                        ipAddress = row["ipAddress"].ToString(),
                        strAction = row["strAction"].ToString(),
                        used = Convert.ToBoolean(row["used"].ToString())
                    };

                    actionList.Add(action);
                }
            }

            return actionList;
        }

        private eAction ConvertToActionEntity(DataTable dt)
        {
            eAction action = new eAction();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                action.dateAction = row["actionDate"].ToString();
                action.id = Convert.ToInt32(row["id"].ToString());
                action.ipAddress = row["ipAddress"].ToString();
                action.strAction = row["strAction"].ToString();
                action.used = Convert.ToBoolean(row["used"].ToString());
            }

            return action;
        }

        public DataTable ConsultarAccionPendiente( string ip)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "spAction";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "ConsultarAccionPendiente");
                    cmd.Parameters.AddWithValue("@ipAddress", ip);


                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;

                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        public void ActualizarHuellasPersonas(byte[] byteHuela, int intId)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "spAction";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "ActualizarrAccionPendiente");
                    cmd.Parameters.AddWithValue("@fingerprint", byteHuela);
                    cmd.Parameters.AddWithValue("@id", intId);


                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                   

                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }

        }

        public void EliminarDetalleAccion(int id)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "spAction";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "EliminarAccionPendiente");
                    cmd.Parameters.AddWithValue("@id", id);


                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
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
