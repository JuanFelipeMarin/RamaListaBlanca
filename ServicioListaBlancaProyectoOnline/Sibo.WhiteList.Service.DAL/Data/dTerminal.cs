using System;
using System.Collections.Generic;
using Sibo.WhiteList.Service.Entities.Classes;
using System.Data;
using System.Data.SqlClient;
using Sibo.WhiteList.Service.Connection;

namespace Sibo.WhiteList.Service.DAL.Data
{
    public class dTerminal
    {
        #region Public Methods
        /// <summary>
        /// Método encargado de consultar una terminal por id en la BD.
        /// Getulio Vargas - 2018-04-12 - OD 1307
        /// </summary>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        public eTerminal GetTerminalById(int terminalId)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTerminal";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "GetTerminalById");
                    cmd.Parameters.AddWithValue("@terminalId", terminalId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return ConvertToEntity(dt);
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        /// <summary>
        /// Método encargado de consultar una terminal por IP.
        /// Getulio Vargas - 2018-06-25 - OD 1307
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public eTerminal GetTerminalByIp(string ipAddress)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTerminal";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "GetTerminalByIp");
                    cmd.Parameters.AddWithValue("@ipAddress", ipAddress);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return ConvertToEntity(dt);
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        /// <summary>
        /// Método encargado de actualizar los datos de una terminal en la BD.
        /// Getulio Vargas - 2018-04-12 - OD 1307
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public bool Update(eTerminal term)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTerminal";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "Update");
                    cmd.Parameters.AddWithValue("@terminalId", term.terminalId);
                    cmd.Parameters.AddWithValue("@ipAddress", term.ipAddress);
                    cmd.Parameters.AddWithValue("@bitServesToOutput", term.servesToOutput);
                    cmd.Parameters.AddWithValue("@bitServesToInputAndOutput", term.servesToInputAndOutput);
                    cmd.Parameters.AddWithValue("@timeWaitAnswerReplicate", term.timeWaitAnswerReplicate);
                    cmd.Parameters.AddWithValue("@TCAM7000", term.TCAM7000);
                    cmd.Parameters.AddWithValue("@LectorZK", term.LectorZK);
                    cmd.Parameters.AddWithValue("@ICAM7000", term.ICAM7000);
                    cmd.Parameters.AddWithValue("@bitCardMode", term.CardMode);
                    cmd.Parameters.AddWithValue("@port", term.port);
                    cmd.Parameters.AddWithValue("@speedPort", term.speedPort);
                    cmd.Parameters.AddWithValue("@withWhiteList", term.withWhiteList);
                    //OD 1689 - GETULIO VARGAS - 2020/01/27
                    cmd.Parameters.AddWithValue("@name", term.name);
                    cmd.Parameters.AddWithValue("@state", term.state);
                    cmd.Parameters.AddWithValue("@terminalTypeId", term.terminalTypeId);
                    cmd.Parameters.AddWithValue("@terminalTypeName", term.terminalTypeDescription);
                    cmd.Parameters.AddWithValue("@snTerminal", term.snTerminal);
                    cmd.Parameters.AddWithValue("@Zonas", term.Zonas);

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

        /// <summary>
        /// Método que permite consultar la lista de terminales registradas en la BD local.
        /// Getulio Vargas - 2018-04-13 - OD 1307
        /// </summary>
        /// <returns></returns>
        public List<eTerminal> GetTerminals()
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTerminal";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "GetTerminals");

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

        public List<eTerminal> GetTerminalsBioLite()
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTerminal";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "GetTerminalsBioLite");

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
        /// <summary>
        /// Método encargado de insertar una terminal en la BD.
        /// Getulio Vargas - 2018-04-12 - OD 1307
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public bool Insert(eTerminal term)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTerminal";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "Insert");
                    cmd.Parameters.AddWithValue("@terminalId", term.terminalId);
                    cmd.Parameters.AddWithValue("@ipAddress", term.ipAddress);
                    cmd.Parameters.AddWithValue("@bitServesToOutput", term.servesToOutput);
                    cmd.Parameters.AddWithValue("@bitServesToInputAndOutput", term.servesToInputAndOutput);
                    cmd.Parameters.AddWithValue("@timeWaitAnswerReplicate", term.timeWaitAnswerReplicate);
                    cmd.Parameters.AddWithValue("@TCAM7000", term.TCAM7000);
                    cmd.Parameters.AddWithValue("@LectorZK", term.LectorZK);
                    cmd.Parameters.AddWithValue("@ICAM7000", term.ICAM7000);
                    cmd.Parameters.AddWithValue("@bitCardMode", term.CardMode);
                    cmd.Parameters.AddWithValue("@port", term.port);
                    cmd.Parameters.AddWithValue("@speedPort", term.speedPort);
                    cmd.Parameters.AddWithValue("@withWhiteList", term.withWhiteList);
                    //OD 1689 - GETULIO VARGAS - 2020/01/27
                    cmd.Parameters.AddWithValue("@name", term.name);
                    cmd.Parameters.AddWithValue("@state", term.state);
                    cmd.Parameters.AddWithValue("@terminalTypeId", term.terminalTypeId);
                    cmd.Parameters.AddWithValue("@terminalTypeName", term.terminalTypeDescription);
                    cmd.Parameters.AddWithValue("@snTerminal", term.snTerminal);
                    cmd.Parameters.AddWithValue("@Zonas", term.Zonas);
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
        #endregion

        #region Private Methods
        /// <summary>
        /// Método encargado de convertir la fila de un DataTable en la entidad eTerminal.
        /// Getulio Vargas - 2018-04-12 - OD 1307
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private eTerminal ConvertToEntity(DataTable dt)
        {
            eTerminal term = new eTerminal();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                term.ICAM7000 = Convert.ToBoolean(row["ICAM7000"]);
                term.ipAddress = row["ipAddress"].ToString();
                term.LectorZK = Convert.ToBoolean(row["LectorZK"]);
                term.port = row["port"].ToString();
                term.speedPort = (row["speedPort"] == null) ? 0 : Convert.ToInt32(row["speedPort"]);
                term.TCAM7000 = Convert.ToBoolean(row["TCAM7000"]);
                term.terminalId = Convert.ToInt32(row["terminalId"]);
                term.timeWaitAnswerReplicate = Convert.ToInt32(row["timeWaitAnswerReplicate"]);
                term.CardMode = Convert.ToBoolean(row["bitCardMode"]);
                term.servesToOutput = Convert.ToBoolean(row["bitServesToOutput"]);
                term.servesToInputAndOutput = Convert.ToBoolean(row["bitServesToInputAndOutput"]);
                term.withWhiteList = Convert.ToBoolean(row["withWhiteList"] ?? false);
                term.name = row["name"].ToString();
                term.state = row["state"] == null ? false : Convert.ToBoolean(row["state"]);
                term.terminalTypeId = row["terminalTypeId"] == null ? 0 : Convert.ToInt32(row["terminalTypeId"].ToString());
                term.terminalTypeDescription = row["terminalTypeName"].ToString();
                term.snTerminal = row["snTerminal"].ToString();
                term.Zonas = row["Zonas"].ToString();
            }

            return term;
        }

        /// <summary>
        /// Método que convierte un DataTable en una lista de terminales, en la estructura de la entidad 'eTerminal'.
        /// Getulio Vargas - 2018-04-13 - OD 1307
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<eTerminal> ConvertToListEntity(DataTable dt)
        {
            List<eTerminal> responseList = new List<eTerminal>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    eTerminal term = new eTerminal()
                    {
                        CardMode = Convert.ToBoolean(row["bitCardMode"]),
                        ICAM7000 = Convert.ToBoolean(row["ICAM7000"]),
                        ipAddress = row["ipAddress"].ToString(),
                        LectorZK = Convert.ToBoolean(row["LectorZK"]),
                        port = row["port"].ToString(),
                        speedPort = Convert.ToInt32(row["speedPort"] ?? 0),
                        TCAM7000 = Convert.ToBoolean(row["TCAM7000"]),
                        terminalId = Convert.ToInt32(row["terminalId"]),
                        timeWaitAnswerReplicate = Convert.ToInt32(row["timeWaitAnswerReplicate"]),
                        servesToOutput = Convert.ToBoolean(row["bitServesToOutput"]),
                        servesToInputAndOutput = Convert.ToBoolean(row["bitServesToInputAndOutput"]),
                        withWhiteList = Convert.ToBoolean(row["withWhiteList"] ?? false),
                        name = row["name"].ToString(),
                        state = row["state"] == null ? false : Convert.ToBoolean(row["state"]),
                        terminalTypeId = row["terminalTypeId"] == null ? 0 : Convert.ToInt32(row["terminalTypeId"]),
                        terminalTypeDescription = row["terminalTypeName"].ToString(),
                        snTerminal = row["snTerminal"].ToString(),
                        Zonas = row["Zonas"].ToString()
                    };

                    responseList.Add(term);
                }
            }

            return responseList;
        }
        #endregion

        public DataTable GetTerminalsTabla()
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTerminal";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "GetTerminals");

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

        public DataTable consultarTerminalID(int idTerminal)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spTerminal";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "GetTerminalsID");
                    cmd.Parameters.AddWithValue("@terminalId", idTerminal);

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
    }
}
