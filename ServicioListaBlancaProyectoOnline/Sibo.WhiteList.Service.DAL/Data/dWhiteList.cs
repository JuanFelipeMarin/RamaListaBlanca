using Sibo.WhiteList.Service.Connection;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Sibo.WhiteList.Service.Data
{
    public class dWhiteList
    {
        string spName = "spWhiteList";

        public DataTable GetUser(string id)
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
                    cmd.Parameters.AddWithValue("@action", "GetUser");
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt64(id).ToString());

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        public DataTable GetWhiteList()
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
                    cmd.Parameters.AddWithValue("@action", "GetWhiteList");

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        public bool Update(eWhiteList item, int intPkId)
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
                    cmd.Parameters.AddWithValue("@intPkId", intPkId);
                    cmd.Parameters.AddWithValue("@availableEntries", item.availableEntries);
                    cmd.Parameters.AddWithValue("@restrictions", item.restrictions);
                    cmd.Parameters.AddWithValue("@withoutFingerprint", item.withoutFingerprint);
                    cmd.Parameters.AddWithValue("@fingerprintId", item.fingerprintId);
                    cmd.Parameters.AddWithValue("@fingerprint", item.fingerprint);
                    cmd.Parameters.AddWithValue("@groupEntriesControl", item.groupEntriesControl);
                    cmd.Parameters.AddWithValue("@groupEntriesQuantity", item.groupEntriesQuantity);
                    cmd.Parameters.AddWithValue("@groupId", item.groupId);
                    cmd.Parameters.AddWithValue("@isRestrictionClass", item.isRestrictionClass);
                    cmd.Parameters.AddWithValue("@classSchedule", item.classSchedule);
                    cmd.Parameters.AddWithValue("@dateClass", item.dateClass);
                    cmd.Parameters.AddWithValue("@reserveId", item.reserveId);
                    cmd.Parameters.AddWithValue("@className", item.className);
                    cmd.Parameters.AddWithValue("@utilizedMegas", item.utilizedMegas);
                    cmd.Parameters.AddWithValue("@utilizedTickets", item.utilizedTickets);
                    cmd.Parameters.AddWithValue("@employeeName", item.employeeName);
                    cmd.Parameters.AddWithValue("@classIntensity", item.classIntensity);
                    cmd.Parameters.AddWithValue("@classState", item.classState);
                    cmd.Parameters.AddWithValue("@photoPath", item.photoPath);
                    cmd.Parameters.AddWithValue("@subgroupId", item.subgroupId);
                    cmd.Parameters.AddWithValue("@cardId", item.cardId);
                    cmd.Parameters.AddWithValue("@strDatoFoto", item.strDatoFoto);

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

        public bool Delete(eWhiteList whiteList)
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
                    cmd.Parameters.AddWithValue("@action", "Delete");
                    cmd.Parameters.AddWithValue("@id", whiteList.id);
                    cmd.Parameters.AddWithValue("@typePerson", whiteList.typePerson);
                    cmd.Parameters.AddWithValue("@planId", whiteList.planId);
                    cmd.Parameters.AddWithValue("@documentType", whiteList.documentType);
                    cmd.Parameters.AddWithValue("@invoiceId", whiteList.invoiceId);
                    cmd.Parameters.AddWithValue("@courtesy", whiteList.courtesy);

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
        /// Método encargado de insertar un registro en la lista blanca local.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Insert(eWhiteList item)
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
                    cmd.Parameters.AddWithValue("@id", item.id);
                    cmd.Parameters.AddWithValue("@name", item.name);
                    cmd.Parameters.AddWithValue("@planId", item.planId);
                    cmd.Parameters.AddWithValue("@planName", item.planName);
                    cmd.Parameters.AddWithValue("@expirationDate", item.expirationDate);
                    cmd.Parameters.AddWithValue("@lastEntry", item.lastEntry);
                    cmd.Parameters.AddWithValue("@planType", item.planType);
                    cmd.Parameters.AddWithValue("@typePerson", item.typePerson);
                    cmd.Parameters.AddWithValue("@availableEntries", item.availableEntries);
                    cmd.Parameters.AddWithValue("@restrictions", item.restrictions);
                    cmd.Parameters.AddWithValue("@branchId", item.branchId);
                    cmd.Parameters.AddWithValue("@branchName", item.branchName);
                    cmd.Parameters.AddWithValue("@gymId", item.gymId);
                    cmd.Parameters.AddWithValue("@personState", item.personState);
                    cmd.Parameters.AddWithValue("@withoutFingerprint", item.withoutFingerprint);
                    cmd.Parameters.AddWithValue("@fingerprintId", item.fingerprintId);
                    cmd.Parameters.AddWithValue("@fingerprint", item.fingerprint);
                    cmd.Parameters.AddWithValue("@updateFingerprint", false);
                    cmd.Parameters.AddWithValue("@know", item.know);
                    cmd.Parameters.AddWithValue("@courtesy", item.courtesy);
                    cmd.Parameters.AddWithValue("@groupEntriesControl", item.groupEntriesControl);
                    cmd.Parameters.AddWithValue("@groupEntriesQuantity", item.groupEntriesQuantity);
                    cmd.Parameters.AddWithValue("@groupId", item.groupId);
                    cmd.Parameters.AddWithValue("@isRestrictionClass", item.isRestrictionClass);
                    cmd.Parameters.AddWithValue("@classSchedule", item.classSchedule);
                    cmd.Parameters.AddWithValue("@dateClass", item.dateClass);
                    cmd.Parameters.AddWithValue("@reserveId", item.reserveId);
                    cmd.Parameters.AddWithValue("@className", item.className);
                    cmd.Parameters.AddWithValue("@utilizedMegas", item.utilizedMegas);
                    cmd.Parameters.AddWithValue("@utilizedTickets", item.utilizedTickets);
                    cmd.Parameters.AddWithValue("@employeeName", item.employeeName);
                    cmd.Parameters.AddWithValue("@classIntensity", item.classIntensity);
                    cmd.Parameters.AddWithValue("@classState", item.classState);
                    cmd.Parameters.AddWithValue("@photoPath", item.photoPath);
                    cmd.Parameters.AddWithValue("@invoiceId", item.invoiceId);
                    cmd.Parameters.AddWithValue("@dianId", item.dianId);
                    cmd.Parameters.AddWithValue("@documentType", item.documentType);
                    cmd.Parameters.AddWithValue("@subgroupId", item.subgroupId);
                    cmd.Parameters.AddWithValue("@cardId", item.cardId);
                    cmd.Parameters.AddWithValue("@strDatoFoto", item.strDatoFoto);

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
        /// Método enargado de insertar la lista blanca en formato tabla.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="dtPerson"></param>
        /// <returns></returns>
        public bool InsertTable(DataTable dtPerson)
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
                    cmd.Parameters.AddWithValue("@tblWhiteList", dtPerson);
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

        /// <summary>
        /// Método encargado de actualizar la huella de un cliente en la lista blanca local.
        /// Getulio Vargas - 2018-08-22 - OD 1307
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="fingerId"></param>
        /// <param name="fingerprintImage"></param>
        /// <returns></returns>
        public bool UpdateFingerprint(string clientId, int fingerId, byte[] fingerprintImage)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "UpdateFingerprint");
                    cmd.Parameters.AddWithValue("@id", clientId);
                    cmd.Parameters.AddWithValue("@fingerprint", fingerprintImage);
                    cmd.Parameters.AddWithValue("@fingerprintId", fingerId);
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
        /// Consulta todos los registros de una persona que pertenezcan a reservas y planes
        /// </summary>
        /// <param name="id">Identitificacion de la persona</param>
        /// <returns></returns>
        public DataTable getRegistrosListaBlancaPlanyReservas(string id)
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
                    cmd.Parameters.AddWithValue("@action", "getRegistrosListaBlancaPlanyReservas");
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt64(id).ToString());

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                }


                return dt;
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        /// <summary>
        /// Consulta todos los registros de una persona que pertenezcan a reservas
        /// </summary>
        /// <param name="id">Identitificacion de la persona</param>
        /// <returns></returns>
        public DataTable getRegistrosListaBlancaReservas(string id)
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
                    cmd.Parameters.AddWithValue("@action", "GetUserById");
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt64(id).ToString());

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        public DataTable GetInformacionZonas(string id)
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
                    cmd.Parameters.AddWithValue("@action", "getInformacionZonas");
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt64(id).ToString());

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }


        public DataTable GetUserByIdSinReservas(string id)
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
                    cmd.Parameters.AddWithValue("@action", "GetUserByIdSinReservas");
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt64(id).ToString());

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }
        
        public DataTable GetUserById(string id)
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
                    cmd.Parameters.AddWithValue("@action", "GetUserById");
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt64(id).ToString());

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        /// <summary>
        /// Consultar registros en la lista blanca por identificación, que tengan reservas hoy.
        /// Mateo Toro - 2020-05-21
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetClientsReservesFromToday(string id)
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
                    cmd.Parameters.AddWithValue("@action", "GetClientsReservesFromToday");
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt64(id).ToString());

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        public DataTable GetUsersFamilyGroup(int groupId)
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
                    cmd.Parameters.AddWithValue("@action", "GetUsersFamilyGroup");
                    cmd.Parameters.AddWithValue("@groupId", groupId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        /// <summary>
        /// Método que retorna un DataTable de lista blanca luego de consultar los registros por el código de la tarjeta asociada.
        /// Getulio Vargas - 2018-08-27 - OD 1307
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public DataTable GetClientWhiteListByCardId(string cardId)
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
                    cmd.Parameters.AddWithValue("@action", "GetClientWhiteListByCardId");
                    cmd.Parameters.AddWithValue("@cardId", cardId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        /// <summary>
        /// Método que retorna un DataTable de lista blanca luego de consultar los registros por el código de la tarjeta asociada.
        /// Getulio Vargas - 2018-08-27 - OD 1307
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public DataTable GetEmployeeWhiteListByCardId(string cardId)
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
                    cmd.Parameters.AddWithValue("@action", "GetEmployeeWhiteListByCardId");
                    cmd.Parameters.AddWithValue("@cardId", cardId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        public DataTable GetClientIdByFingerprintId(int id)
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
                    cmd.Parameters.AddWithValue("@action", "GetClientIdByFingerprintId");
                    cmd.Parameters.AddWithValue("@fingerprintId", id);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        public DataTable GetClientIdBioentry(string id)
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
                    cmd.Parameters.AddWithValue("@action", "GetClientIdBioentry");
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        /// <summary>
        /// Método que permite actualizar los tiquetes disponibles de un usuario en la lista blanca.
        /// GEtulio Vargas - 2018-09-02 - OD 1307
        /// </summary>
        /// <param name="whiteListId"></param>
        /// <returns></returns>
        public bool DiscountTicket(int idNumeroFactura, string idPersona)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "DiscountTicket");
                    cmd.Parameters.AddWithValue("@intPkId", idNumeroFactura);
                    cmd.Parameters.AddWithValue("@id", idPersona);
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


        public DataTable DiscountTicketWeb(int idNumeroFactura, string idPersona)
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
                    cmd.Parameters.AddWithValue("@action", "ActualizarTiquetesWeb");
                    cmd.Parameters.AddWithValue("@intPkId", idNumeroFactura);
                    cmd.Parameters.AddWithValue("@id", idPersona);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }
        /// <summary>
        /// Método que retorna el listado de huellas a partir de un id, esta acción trabaja con un like.
        /// Getulio Vargas - 2018-09-04 - OD 1307
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetFingerprintsById(string id)
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
                    cmd.Parameters.AddWithValue("@action", "GetFingerprintsById");
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        public bool InsertOrUpdateWhiteList(DataTable dtPerson)
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
                    cmd.Parameters.AddWithValue("@action", "InsertOrUpdateWhiteList");
                    cmd.Parameters.AddWithValue("@tblWhiteList", dtPerson);
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
                return false;
            }
        }
        public DataTable GetIngresosVisitantesTblEvents(string id)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = "spEvent";
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "IngresosVisitantes");
                    cmd.Parameters.AddWithValue("@clientId", id);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        public bool UpdateFingerprintID(byte[] fingerprint, int fingerprintId, string personId)
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
                    cmd.Parameters.AddWithValue("@action", "UpdateFingerprintID");
                    cmd.Parameters.AddWithValue("@id", personId);
                    cmd.Parameters.AddWithValue("@fingerprintId", fingerprintId);
                    cmd.Parameters.AddWithValue("@fingerprint", fingerprint);

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

        public DataTable ConsultarZonas(string ids)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = "spWhiteList";
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "ConsultarZonas");
                    cmd.Parameters.AddWithValue("@id", ids);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        public bool fingerprintReplitDelete(eWhiteList item)
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
                    cmd.Parameters.AddWithValue("@action", "fingerprintReplitDelete");
                    cmd.Parameters.AddWithValue("@id", item.id);

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

        public bool agregarHorariosRestriccion(string horaEntrada, string horaSalida)
        {
            DataAccess objConn = new DataAccess();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "agregarHorariosRestriccion");
                    cmd.Parameters.AddWithValue("@horaEntrada", horaEntrada);
                    cmd.Parameters.AddWithValue("@horaSalida", horaSalida);

                    int resp = cmd.ExecuteNonQuery();

                    if (resp > 0)
                    {
                        objConn.ConnectionDispose();
                        return true;
                    }
                }
            }
            catch { }

            objConn.ConnectionDispose();
            return false;
        }


        public DataTable ConsultarHorariosRestriccion()
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = "spWhiteList";
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "ConsultarHorariosRestriccion");

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return dt;
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        public string ConsultarHorariosRestriccionPorDiaHora(string horaEntrada, string horaSalida)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = "spWhiteList";
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "ConsultarHorariosRestriccionPorDiaHora");
                    cmd.Parameters.AddWithValue("@horaEntrada", horaEntrada);
                    cmd.Parameters.AddWithValue("@horaSalida", horaSalida);


                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();

                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0]["id"].ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        public string RemoverListaContratos(string id)
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
                    cmd.Parameters.AddWithValue("@action", "RemoverListaContratos");
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();

                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0]["clientId"].ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                return null;
            }
        }
        public bool RemoverListaPersonas(string id)
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
                    cmd.Parameters.AddWithValue("@action", "RemoverListaPersonas");
                    cmd.Parameters.AddWithValue("@id", id);

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
                return false;
            }
        }

        public DataTable obtenerIdVisitanteEliminar(string id)
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
                    cmd.Parameters.AddWithValue("@action", "obtenerIdVisitanteEliminar");
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();

                    if (dt.Rows.Count > 0)
                    {
                        return dt;
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
                return null;
            }
        }

        public DataTable dReplicaUsuariosBioLiteRestriccion()
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
                    cmd.Parameters.AddWithValue("@action", "dReplicaUsuariosBioLiteRestriccion");


                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();

                    if (dt.Rows.Count > 0)
                    {
                        return dt;
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
                return null;
            }
        }

        public DataTable obtenerPlanesPersona(int id)
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
                    cmd.Parameters.AddWithValue("@action", "getPlanesPersona");
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();

                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
                }
            }
            catch (Exception)
            {
                objConn.ConnectionDispose();
                return null;
            }

            return null;
        }
    }
}
