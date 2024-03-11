using Sibo.WhiteList.Service.Connection;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Sibo.WhiteList.Service.Data
{
    public class dEntry
    {
        DataAccess objConn = new DataAccess();
        string spName = "spEntry";

        public List<eEntries> GetEntries()
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = objConn.GetConnection();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = spName;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "GetEntries";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            objConn.ConnectionDispose();

            return ConvertToListEntryEntity(dt);
        }

        public DataTable GetEntriesToShow(string ipAddress)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = objConn.GetConnection();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "GetEntriesToShow";
                cmd.Parameters.Add("@ipAddress", SqlDbType.VarChar, 100).Value = ipAddress;
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

        public bool UpdateState(eEntries item)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                int resp = 0;

                cmd.Parameters.AddWithValue("@action", "UpdateState");
                cmd.Connection = objConn.GetConnection();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;
                cmd.CommandTimeout = 0;
                cmd.Parameters.AddWithValue("@intPkId", item.intPkId);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Update(string clientId, int planId, int invoiceId, DateTime outDate, DateTime outHour)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                int resp = 0;
                cmd.Connection = objConn.GetConnection();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;
                cmd.CommandTimeout = 0;
                cmd.Parameters.AddWithValue("@action", "Update");
                cmd.Parameters.AddWithValue("@outDate", outDate);
                cmd.Parameters.AddWithValue("@outHour", outHour);
                cmd.Parameters.AddWithValue("@clientId", clientId);
                cmd.Parameters.AddWithValue("@planId", planId);
                cmd.Parameters.AddWithValue("@invoiceId", invoiceId);

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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Insert(eEntries entry)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                int resp = 0;
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
                
                cmd.Parameters.AddWithValue("@clientId", entry.clientId);
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

                if (!string.IsNullOrEmpty(entry.expirationDate))
                {
                    cmd.Parameters.AddWithValue("@expirationDate", Convert.ToDateTime(entry.expirationDate));
                }
                
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private List<eEntries> ConvertToListEntryEntity(DataTable dt)
        {
            try
            {
                List<eEntries> responseList = new List<eEntries>();
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        eEntries entry = new eEntries();
                        entry.clientId = row["clientId"].ToString();
                        entry.clientName = row["clientName"].ToString();
                        entry.documentType = row["documentType"].ToString();
                        entry.entryDate = Convert.IsDBNull(row["entryDate"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["entryDate"]);
                        entry.entryHour = Convert.IsDBNull(row["entryHour"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["entryHour"]);
                        entry.intPkId = Convert.ToInt32(row["intPkId"]);
                        entry.invoiceId = Convert.ToInt32(row["invoiceId"]);
                        entry.discountTicket = Convert.IsDBNull(row["discountTicket"]) ? false : Convert.ToBoolean(row["discountTicket"]);
                        entry.outDate = Convert.IsDBNull(row["outDate"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["outDate"]);
                        entry.outHour = Convert.IsDBNull(row["outHour"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["outHour"]);
                        entry.planId = Convert.ToInt32(row["planId"]);
                        entry.planName = row["planName"].ToString();
                        entry.visitId = Convert.IsDBNull(row["visitId"]) ? 0 : Convert.ToInt32(row["visitId"]);
                        entry.firstMessage = row["strFirstMessage"].ToString();
                        entry.secondMessage = row["strSecondMessage"].ToString();
                        entry.expirationDate = Convert.IsDBNull(row["expirationDate"]) ? new DateTime(1900, 1, 1).ToString("yyyy/MM/dd hh:MM") : Convert.ToDateTime(row["expirationDate"]).ToString("yyyy/MM/dd hh:MM");
                        entry.dateLastEntry = Convert.IsDBNull(row["dateLastEntry"]) ? new DateTime(1900, 1, 1).ToString("yyyy/MM/dd hh:MM") : Convert.ToDateTime(row["dateLastEntry"]).ToString("yyyy/MM/dd hh:MM");

                        responseList.Add(entry);
                    }

                    return responseList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetEntryByUserWithoutOutput(string userId, int planId, int invoiceId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                cmd.Connection = objConn.GetConnection();
                cmd.CommandText = spName;
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetEntryByUserWithoutOutput");
                cmd.Parameters.AddWithValue("@clientId", userId);
                cmd.Parameters.AddWithValue("@planId", planId);
                cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
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

        public DataTable GetEntriesByIdFamilyGroup(string idList)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                cmd.Connection = objConn.GetConnection();
                cmd.CommandText = spName;
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetEntriesByIdFamilyGroup");
                cmd.Parameters.AddWithValue("@idList", idList);
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
