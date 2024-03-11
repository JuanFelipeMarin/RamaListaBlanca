using Sibo.WhiteList.Service.Connection;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Sibo.WhiteList.Service.Data
{
    public class dEvent
    {
        string spName = "spEvent";

        public List<eEvent> GetEntries()
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
                    cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "GetEntries";

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return ConvertToListEntryEntity(dt);
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        public DataTable GetEntriesToShow(string ipAddress, bool blnSirveParaSalida, bool blnSirveParaEntradaySalida)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();
            string action = string.Empty;

            if (blnSirveParaSalida)
            {
                action = "GetExitToShow";
            }
            else if (!blnSirveParaSalida && !blnSirveParaEntradaySalida)
            {
                action = "GetEntriesToShow";
            }
            else if (!blnSirveParaSalida && blnSirveParaEntradaySalida)
            {
                action = "GetBothEvents";
            }

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = action;
                    cmd.Parameters.Add("@ipAddress", SqlDbType.VarChar, 100).Value = ipAddress;

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

        /// <summary>
        /// Método que permite consultar los eventos registrados en la sucursal para ingreso touch.
        /// Getulio Vargas - 2018-09-04 - OD 1307
        /// </summary>
        /// <returns></returns>
        public DataTable GetEntriesToShowMonitor(bool blnSirveParaSalida, bool blnSirveParaEntradaySalida)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();
            string action = string.Empty;

            if (blnSirveParaSalida)
            {
                action = "GetExitToShowMonitor";
            }
            else if (!blnSirveParaSalida && !blnSirveParaEntradaySalida)
            {
                action = "GetEntriesToShowMonitor";
            }
            else if (!blnSirveParaSalida && blnSirveParaEntradaySalida)
            {
                action = "GetBothEvents";
            }

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = action;

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

        public bool UpdateState(eEvent item)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Parameters.AddWithValue("@action", "UpdateState");
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@intPkId", item.intPkId);
                    cmd.Parameters.AddWithValue("@modifiedDate", item.modifiedDate);
                    cmd.Parameters.AddWithValue("@updated", true);

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

        public bool Update(string clientId, int planId, int invoiceId, DateTime outDate, DateTime outHour, string thirdMessage)
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
                    cmd.Parameters.AddWithValue("@outDate", outDate);
                    cmd.Parameters.AddWithValue("@outHour", outHour);
                    cmd.Parameters.AddWithValue("@clientId", Convert.ToInt64(clientId));
                    cmd.Parameters.AddWithValue("@planId", planId);
                    cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
                    cmd.Parameters.AddWithValue("@thirdMessage", thirdMessage);

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

        public bool Insert(eEvent entry)
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

                    if (entry.entryDate != DateTime.MinValue)
                    {
                        cmd.Parameters.AddWithValue("@entryDate", entry.entryDate);
                    }

                    if (entry.entryHour != DateTime.MinValue)
                    {
                        cmd.Parameters.AddWithValue("@entryHour", entry.entryHour);
                    }

                    if (entry.outDate != DateTime.MinValue)
                    {
                        cmd.Parameters.AddWithValue("@outDate", entry.outDate);
                    }

                    if (entry.outHour != DateTime.MinValue)
                    {
                        cmd.Parameters.AddWithValue("@outHour", entry.outHour);
                    }

                    entry.clientId = string.IsNullOrEmpty(entry.clientId) ? "0" : entry.clientId;

                    cmd.Parameters.AddWithValue("@clientId", Convert.ToInt64(entry.clientId));
                    cmd.Parameters.AddWithValue("@clientName", entry.clientName);
                    cmd.Parameters.AddWithValue("@planId", entry.planId);
                    cmd.Parameters.AddWithValue("@planName", entry.planName);
                    cmd.Parameters.AddWithValue("@invoiceId", entry.invoiceId);
                    cmd.Parameters.AddWithValue("@documentType", entry.documentType);
                    cmd.Parameters.AddWithValue("@discountTicket", entry.discountTicket);
                    cmd.Parameters.AddWithValue("@visitId", entry.visitId);
                    cmd.Parameters.AddWithValue("@updated", false);

                    if (!string.IsNullOrEmpty(entry.dateLastEntry))
                    {
                        cmd.Parameters.AddWithValue("@dateLastEntry", Convert.ToDateTime(entry.dateLastEntry));
                    }

                    cmd.Parameters.AddWithValue("@successEntry", entry.successEntry);
                    cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ipAddress", entry.ipAddress);
                    cmd.Parameters.AddWithValue("@strFirstMessage", entry.firstMessage);
                    cmd.Parameters.AddWithValue("@strSecondMessage", entry.secondMessage);
                    cmd.Parameters.AddWithValue("@thirdMessage", entry.thirdMessage);
                    cmd.Parameters.AddWithValue("@qrCode", entry.qrCode);
                    cmd.Parameters.AddWithValue("@temperature", entry.temperature);
                    cmd.Parameters.AddWithValue("@usuarioLoginIngresoAdicional", entry.usuario);
                    cmd.Parameters.AddWithValue("@strEmpresaIngresoAdicional", entry.strEmpresaIngresoAdicional);

                    if (!string.IsNullOrEmpty(entry.expirationDate))
                    {
                        cmd.Parameters.AddWithValue("@expirationDate", Convert.ToDateTime(entry.expirationDate));
                    }

                    cmd.Parameters.AddWithValue("@eventType", entry.eventType);

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

        private List<eEvent> ConvertToListEntryEntity(DataTable dt)
        {
            List<eEvent> responseList = new List<eEvent>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    //eEvent entry = new eEvent()
                    //{
                    //    clientId = row["clientId"].ToString(),
                    //    clientName = row["clientName"].ToString(),
                    //    documentType = row["documentType"].ToString(),
                    //    entryDate = Convert.IsDBNull(row["entryDate"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["entryDate"]),
                    //    entryHour = Convert.IsDBNull(row["entryHour"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["entryHour"]),
                    //    intPkId = row["intPkId"] == null ? 0 : Convert.ToInt32(row["intPkId"]),
                    //    invoiceId = row["invoiceId"] == null ? 0 : Convert.ToInt32(row["invoiceId"]),
                    //    discountTicket = row["discountTicket"] == null ? false : Convert.ToBoolean(row["discountTicket"]),
                    //    outDate = Convert.IsDBNull(row["outDate"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["outDate"]),
                    //    outHour = Convert.IsDBNull(row["outHour"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["outHour"]),
                    //    planId = row["planId"] == null ? 0 : Convert.ToInt32(row["planId"]),
                    //    planName = row["planName"].ToString(),
                    //    visitId = row["visitId"] == null ? 0 : Convert.ToInt32(row["visitId"]),
                    //    firstMessage = row["strFirstMessage"].ToString(),
                    //    secondMessage = row["strSecondMessage"].ToString(),
                    //    thirdMessage = row["thirdMessage"].ToString(),
                    //    expirationDate = Convert.IsDBNull(row["expirationDate"]) ? new DateTime(1900, 1, 1).ToString("yyyy/MM/dd hh:MM") : Convert.ToDateTime(row["expirationDate"]).ToString("yyyy/MM/dd hh:MM"),
                    //    dateLastEntry = Convert.IsDBNull(row["dateLastEntry"]) ? new DateTime(1900, 1, 1).ToString("yyyy/MM/dd hh:MM") : Convert.ToDateTime(row["dateLastEntry"]).ToString("yyyy/MM/dd hh:MM"),
                    //};

                    eEvent entry = new eEvent();
                    entry.clientId = row["clientId"].ToString();
                    entry.clientName = row["clientName"].ToString();
                    entry.documentType = row["documentType"].ToString();
                    entry.entryDate = Convert.IsDBNull(row["entryDate"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["entryDate"]);
                    entry.entryHour = Convert.IsDBNull(row["entryHour"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["entryHour"]);
                    entry.intPkId = row["intPkId"] == null ? 0 : Convert.ToInt32(row["intPkId"]);
                    entry.invoiceId = row["invoiceId"] == null ? 0 : Convert.ToInt32(row["invoiceId"]);
                    entry.discountTicket = row["discountTicket"] == null ? false : Convert.ToBoolean(row["discountTicket"]);
                    entry.outDate = Convert.IsDBNull(row["outDate"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["outDate"]);
                    entry.modifiedDate = Convert.IsDBNull(row["modifiedDate"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["modifiedDate"]);
                    entry.outHour = Convert.IsDBNull(row["outHour"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["outHour"]);
                    entry.planId = row["planId"] == null ? 0 : Convert.ToInt32(row["planId"]);
                    entry.planName = row["planName"].ToString();
                    entry.visitId = row["visitId"] == null ? 0 : Convert.ToInt32(row["visitId"]);
                    entry.firstMessage = row["strFirstMessage"].ToString();
                    entry.secondMessage = row["strSecondMessage"].ToString();
                    entry.thirdMessage = row["thirdMessage"].ToString();
                    entry.expirationDate = Convert.IsDBNull(row["expirationDate"]) ? new DateTime(1900, 1, 1).ToString("yyyy/MM/dd hh:MM") : Convert.ToDateTime(row["expirationDate"]).ToString("yyyy/MM/dd hh:MM");
                    entry.dateLastEntry = Convert.IsDBNull(row["dateLastEntry"]) ? new DateTime(1900, 1, 1).ToString("yyyy/MM/dd hh:MM") : Convert.ToDateTime(row["dateLastEntry"]).ToString("yyyy/MM/dd hh:MM");
                    entry.qrCode = row["qrCode"].ToString();
                    entry.temperature = row["temperature"].ToString();
                    entry.strEmpresaIngresoAdicional = row["strEmpresaIngresoAdicional"].ToString();
                    entry.usuario = row["strUsuarioLogin"].ToString();

                    responseList.Add(entry);
                }

                return responseList;
            }
            else
            {
                return null;
            }
        }

        public DataTable GetEntryByUserWithoutOutput(string userId, int planId, int invoiceId)
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
                    cmd.Parameters.AddWithValue("@action", "GetEntryByUserWithoutOutput");
                    cmd.Parameters.AddWithValue("@clientId", Convert.ToInt64(userId));
                    cmd.Parameters.AddWithValue("@planId", planId);
                    cmd.Parameters.AddWithValue("@invoiceId", invoiceId);

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

        public DataTable GetEntriesByIdFamilyGroup(string idList)
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
                    cmd.Parameters.AddWithValue("@action", "GetEntriesByIdFamilyGroup");
                    cmd.Parameters.AddWithValue("@idList", idList);

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

        /// <summary>
        /// Método que permite consultar la última entrada de un usuario específico.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public eEvent GetLastEntryByUserId(string userId)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();
            eEvent response = null;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "GetLastEntryByUserId");
                    cmd.Parameters.AddWithValue("@clientId", Convert.ToInt64(userId).ToString());

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();

                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            response = ConvertToEntity(dt);
                        }
                    }

                    return response;
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite consultar la última entrada de un usuario, conociendo el id del usuario, el número de la factura y el id del plan.
        /// Getulio Vargas - 2018-09-02 - OD 1307
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public eEvent GetLastEntryByUserIdAndInvoiceIdAndPlanId(string userId, int invoiceId, int planId)
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
                    cmd.Parameters.AddWithValue("@action", "GetLastEntryByUserIdAndInvoiceIdAndPlanId");
                    cmd.Parameters.AddWithValue("@clientId", userId);
                    cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
                    cmd.Parameters.AddWithValue("@planId", planId);

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

        private eEvent ConvertToEntity(DataTable dt)
        {
            eEvent response = new eEvent();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                response.clientId = row["clientId"].ToString();
                response.clientName = row["clientName"].ToString();
                response.dateLastEntry = row["dateLastEntry"].ToString();

                if (!Convert.IsDBNull(row["entryDate"]))
                {
                    response.entryDate = Convert.ToDateTime(row["entryDate"]);
                }

                if (!Convert.IsDBNull(row["entryHour"]))
                {
                    response.entryHour = Convert.ToDateTime(row["entryHour"]);
                }

                response.eventType = row["eventType"].ToString();

                if (!Convert.IsDBNull(row["expirationDate"]))
                {
                    response.expirationDate = Convert.ToDateTime(row["expirationDate"]).ToString("yyyy/MM/dd");
                }

                response.firstMessage = row["strFirstMessage"].ToString();
                response.planName = row["planName"].ToString();
                response.secondMessage = row["strSecondMessage"].ToString();
                response.thirdMessage = row["thirdMessage"].ToString();
            }

            return response;
        }

        public bool InsertTable(DataTable dt)
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
                    cmd.Parameters.AddWithValue("@tblEvent", dt);

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

        public bool InsertTerminalEvents(DataTable dt)
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
                    cmd.Parameters.AddWithValue("@action", "InsertTerminalEvents");
                    cmd.Parameters.AddWithValue("@tblTerminalEvents", dt);

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

        public List<eEvent> GetEntriesAdicional()
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
                    cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "GetEntriesAdicional";

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return ConvertToListEntryEntity(dt);
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
