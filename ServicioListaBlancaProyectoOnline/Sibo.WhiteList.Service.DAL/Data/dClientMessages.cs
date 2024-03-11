using Sibo.WhiteList.Service.Connection;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Sibo.WhiteList.Service.Data
{
    public class dClientMessages
    {
        /// <summary>
        /// Método que permite consultar la lista de mensajes "activos" guardados en la BD local.
        /// Getulio Vargas - 2018-04-06 - OD 1307
        /// </summary>
        /// <returns></returns>
        public List<eClientMessages> GetClientMessages()
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spClientMessages";
                    cmd.CommandTimeout = 0;
                    //cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "GetClientMessages";
                    cmd.Parameters.AddWithValue("@action", "GetClientMessages");
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    objConn.ConnectionDispose();
                    return ConvertToListEntityClientMessages(dt);
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite consultar un mensaje específico.
        /// Getulio Vargas - 2018-04-09 - OD 1307
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public eClientMessages GetClientMessage(int messageId)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spClientMessages";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "GetClientMessage");
                    cmd.Parameters.AddWithValue("@messageId", messageId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        return ConvertToEntityClientMessages(dt);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite convertir la fila de un DataTable a la entidad eClientMessages, trabaja solo con la fila 1, es dicr un solo registro.
        /// Getulio Vargas - 2018-04-09 - OD 1307
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private eClientMessages ConvertToEntityClientMessages(DataTable dt)
        {
            eClientMessages cm = new eClientMessages();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                cm.messageDurationTime = Convert.IsDBNull(row["messageDurationTime"]) ? 0 : Convert.ToInt32(row["messageDurationTime"]);
                cm.messageFinalDate = Convert.IsDBNull(row["messageFinalDate"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["messageFinalDate"]);
                cm.messageFinalHour = Convert.IsDBNull(row["messageFinalHour"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["messageFinalHour"]);
                cm.messageId = Convert.ToInt32(row["messageId"]);
                cm.messageImgOrder = Convert.IsDBNull(row["messageImgOrder"]) ? string.Empty : row["messageImgOrder"].ToString();
                cm.messageInitialDate = Convert.IsDBNull(row["messageInitialDate"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["messageInitialDate"]);
                cm.messageInitialHour = Convert.IsDBNull(row["messageInitialHour"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["messageInitialHour"]);
                cm.messageState = Convert.ToBoolean(row["messageState"]);
                cm.messageText = row["messageState"].ToString();
                cm.messageType = row["messageType"].ToString();
            }

            return cm;
        }

        public DataTable GetMessagesWithoutReplicate(string ipAddress)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = "spReplicatedMessages";
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "GetMessagesWithoutReplicate");
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

        public bool InsertReplicatedImage(int iDImg, string ipAddress)
        {
            int resp = 0;
            DataAccess objConn = new DataAccess();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = "spReplicatedMessages";
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "Insert");
                    cmd.Parameters.AddWithValue("@messageId", iDImg);
                    cmd.Parameters.AddWithValue("@ipAddress", ipAddress);
                    cmd.Parameters.AddWithValue("@replicationDate", DateTime.Now);
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

        /// <summary>
        /// Método que permite convertir un DataTable en una lista de la entidad eClientMessages.
        /// Getulio Vargas - 2018-04-06 - OD 1307
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<eClientMessages> ConvertToListEntityClientMessages(DataTable dt)
        {
            List<eClientMessages> responseList = new List<eClientMessages>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    eClientMessages cm = new eClientMessages();
                    cm.messageDurationTime = Convert.IsDBNull(row["messageDurationTime"]) ? 0 : Convert.ToInt32(row["messageDurationTime"]);
                    cm.messageFinalDate = Convert.IsDBNull(row["messageFinalDate"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["messageFinalDate"]);
                    cm.messageFinalHour = Convert.IsDBNull(row["messageFinalHour"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["messageFinalHour"]);
                    cm.messageId = Convert.ToInt32(row["messageId"]);

                    if (row["messageImage"] != null && row["messageImage"].ToString() != "")
                    {
                        cm.messageImage = (byte[])row["messageImage"];
                    }

                    cm.messageImgOrder = Convert.IsDBNull(row["messageImgOrder"]) ? string.Empty : row["messageImgOrder"].ToString();
                    cm.messageInitialDate = Convert.IsDBNull(row["messageInitialDate"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["messageInitialDate"]);
                    cm.messageInitialHour = Convert.IsDBNull(row["messageInitialHour"]) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(row["messageInitialHour"]);
                    cm.messageState = Convert.ToBoolean(row["messageState"]);
                    cm.messageText = row["messageText"].ToString();
                    cm.messageType = row["messageType"].ToString();

                    responseList.Add(cm);
                }
            }

            return responseList;
        }

        /// <summary>
        /// Método para insertar o actualizar los mensajes al cliente.
        /// Getulio Vargas - 2018-04-06 - OD 1307
        /// </summary>
        /// <param name="item">Entidad eClientMessages.</param>
        /// <param name="action">Acción a realizar: Insert o Update</param>
        /// <returns></returns>
        public bool InsertOrUpdate(eClientMessages item, string action)
        {
            int resp = 0;
            DataAccess objConn = new DataAccess();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spClientMessages";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@messageId", item.messageId);
                    cmd.Parameters.AddWithValue("@messageType", item.messageType);
                    cmd.Parameters.AddWithValue("@messageText", item.messageText);
                    cmd.Parameters.AddWithValue("@messageDurationTime", item.messageDurationTime);
                    cmd.Parameters.AddWithValue("@messageInitialDate", item.messageInitialDate);
                    cmd.Parameters.AddWithValue("@messageInitialHour", item.messageInitialHour);
                    cmd.Parameters.AddWithValue("@messageFinalDate", item.messageFinalDate);
                    cmd.Parameters.AddWithValue("@messageFinalHour", item.messageFinalHour);
                    cmd.Parameters.AddWithValue("@messageState", item.messageState);
                    cmd.Parameters.AddWithValue("@messageImgOrder", item.messageImgOrder);
                    cmd.Parameters.AddWithValue("@messageImage", item.messageImage);

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
    }
}
