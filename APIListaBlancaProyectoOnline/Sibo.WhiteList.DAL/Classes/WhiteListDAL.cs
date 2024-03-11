using Sibo.WhiteList.Classes;
using Sibo.WhiteList.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class WhiteListDAL
    {

        /// <summary>
        /// Método que se encarga de consultar los registros pendientes por descargar de la lista blanca.
        /// Getulio Vargas - 2018-03-21 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public List<spWhiteList_Result> GetWhiteList(int gymId, string branchId)
        {
            try
            {
                using (var context = new dbWhiteListModelEntities(gymId))
                {
                    context.Database.CommandTimeout = 0;
                    var query = context.spWhiteList(gymId, Convert.ToInt32(branchId)).ToList();
                    return query;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<gim_planes_sucursal> GetListPlanesSucursalPorPlan(int gymId, string branchId, int planId)
        {
            List<gim_planes_sucursal> responseList = new List<gim_planes_sucursal>();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                responseList = context.gim_planes_sucursal.Where(wl => wl.cdgimnasio == gymId && wl.plansuc_intpkSucursal == Convert.ToInt32(branchId) && wl.plansuc_intpkPlan == planId).ToList();
            }

            return responseList;
        }

        public string getZonasAcceso(int idEmpresa, string idSucursal, int idPlan, int idReserva)
        {
            string zonasPlanesSucurusal;
            string zonasReservas = "";
            string zonasPermitidas = "";

            using (var context = new dbWhiteListModelEntities(idEmpresa))
            {
                zonasPlanesSucurusal = context.gim_planes_sucursal.FirstOrDefault(wl => wl.cdgimnasio == idEmpresa && wl.plansuc_intpkSucursal == Convert.ToInt32(idSucursal) && wl.plansuc_intpkPlan == idPlan).Zonas;

                zonasPermitidas = zonasPlanesSucurusal == null ? "" : zonasPlanesSucurusal;

                if (idReserva != 0)
                {
                    zonasReservas = context.gim_reservas.Join(context.gim_clases, a => a.cdclase, b => b.cdclase, (a, b) => new { a, b }).FirstOrDefault(x => x.a.cdreserva == idReserva).b.intZona.ToString();
                    zonasPermitidas += zonasReservas == null || zonasReservas == "" ? "" : "," + zonasReservas;
                }
            }

            return zonasPermitidas;
        }

        public bool GetActualizarCantidadTiquetesAsync(int id, int invoiceId, int dianId, string documentType, int availableEntries, int gymId, string branchId, int planId)
        {
            try
            {
                DataTable dt = new DataTable();
                using (var context = new dbWhiteListModelEntities(gymId))
                {
                    string strConnection = context.Database.Connection.ConnectionString.ToString();

                    using (SqlConnection conn = new SqlConnection(strConnection))
                    {
                       
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = "spActualizarCantidadTiquetesWeb";
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
                            cmd.Parameters.AddWithValue("@dianId", dianId);
                            cmd.Parameters.AddWithValue("@documentType", RemoveAccent(documentType));
                            cmd.Parameters.AddWithValue("@availableEntries", availableEntries);
                            cmd.Parameters.AddWithValue("@gymId", gymId);
                            cmd.Parameters.AddWithValue("@branchId", branchId);
                            cmd.Parameters.AddWithValue("@planId", planId);

                            
                            //cmd.ExecuteNonQuery();
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dt);
                            }
                            if (dt.Rows.Count >= 1)
                            {
                                return Convert.ToBoolean(dt.Rows[0]["respuesta"]);

                            }
                            else
                            {
                                return false;
                            }

                        }

                    }
                }
              
            }
            catch (Exception)
            {
                return false;
            }
        }


        public string GetListSucursalPorClase(int gymId, int branchId, int cdreserva)
        {
            string valorReturn;
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                var responseList = (from reserva in context.gim_reservas
                                    join clase in context.gim_clases on reserva.cdclase equals clase.cdclase
                                    where clase.cdgimnasio == gymId
                                    && reserva.cdgimnasio == gymId
                                    && clase.cdsucursal == branchId
                                    && reserva.cdsucursal == branchId
                                    && reserva.cdreserva == cdreserva
                                    select clase.intZona).FirstOrDefault().ToString();

                valorReturn = responseList;
            }

            return valorReturn;
        }

        public bool UpdateFingerprint(int gymId, string personId, int fingerprintId, byte[] fingerPrint)
        {
            List<WhiteList> wlEntityList = new List<WhiteList>();
            wlEntityList = GetUsers(gymId, personId);

            if (wlEntityList != null && wlEntityList.Count > 0)
            {
                foreach (WhiteList item in wlEntityList)
                {
                    item.fingerprint = fingerPrint;
                    item.fingerprintId = fingerprintId;
                    item.updateFingerprint = false;
                    Update(item);
                }
            }

            return true;
        }

        /// <summary>
        /// Método encargado de consultar los registros de un usuario en un gimnasio específico.
        /// Getulio Vargas - 2018-08-22 - OD 13107
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        private List<WhiteList> GetUsers(int gymId, string personId)
        {
            List<WhiteList> responseList = new List<WhiteList>();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                responseList = context.WhiteList.Where(wl => wl.gymId == gymId && wl.id == personId).ToList();
            }

            return responseList;
        }

        /// <summary>
        /// Método que se encarga de recorrer la lista que llega de la lista blanca para enviar a actualizar el registro en la BD.
        /// Getulio Vargas - 2018-03-21 - OD 1307
        /// </summary>
        /// <param name="entityWhiteList"></param>
        /// <returns></returns>
        public bool UpdateWhiteListRecords(eMarcarListaBlanca entity)
        {
            try
            {
                int gymId = entity.idEmpresa;
                string users = string.Join(", ", entity.entities.Select(item => item));

                if (!string.IsNullOrEmpty(users))
                {
                    UpdateByIdList(users, gymId);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void UpdateByIdList(string users, int gymId)
        {
            try
            {

                using (var context = new dbWhiteListModelEntities(gymId))
                {
                    string strConnection = context.Database.Connection.ConnectionString.ToString();

                    using (SqlConnection conn = new SqlConnection(strConnection))
                    {
                        conn.Open();

                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = "spUpdateWhiteList";
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@users", users);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

            }
            catch (Exception ex)
            {


            }
        }

        /// <summary>
        /// Método que se encarga de actualizar un registro de la lista blanca en la BD.
        /// Getulio Vargas - 2018-03-21 - OD 1307
        /// </summary>
        /// <param name="entity"></param>
        public bool Update(WhiteList entity)
        {
            int resp = 0;

            using (var context = new dbWhiteListModelEntities(entity.gymId))
            {
                context.Entry(entity).State = EntityState.Modified;
                entity.personState = "Pendiente";
                resp = context.SaveChanges();
            }

            if (resp > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InsertRecordNewBranch(int gymId, int actualBranch, int branchId, string branchName, int planId, string clientId)
        {
            spWhiteListCliente_Result whiteListFn = new spWhiteListCliente_Result();
            WhiteList whiteList = new WhiteList();
            WhiteList auxWL = new WhiteList();

            GetWhiteListNotExistingRecord(gymId, actualBranch, clientId);

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                whiteList = GetWhiteListExistingRecord(gymId, actualBranch, planId, clientId);
                auxWL = GetWhiteListExistingRecord(gymId, branchId, planId, clientId);
            }

            if (auxWL == null)
            {
                if (whiteList != null)
                {
                    whiteList.branchId = branchId;
                    whiteList.branchName = branchName;
                    whiteList.personState = "Pendiente";
                    Insert(whiteList);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public WhiteList InsertRecordNewBranchAndReturn(int gymId, int actualBranch, int branchId, string branchName, int planId, string clientId)
        {
            spWhiteListCliente_Result whiteListFn = new spWhiteListCliente_Result();
            WhiteList whiteList = new WhiteList();
            WhiteList auxWL = new WhiteList();

            GetWhiteListNotExistingRecord(gymId, actualBranch, clientId);

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                whiteList = GetWhiteListExistingRecord(gymId, actualBranch, planId, clientId);
                auxWL = GetWhiteListExistingRecord(gymId, branchId, planId, clientId);
            }

            if (auxWL == null)
            {
                if (whiteList != null)
                {
                    whiteList.branchId = branchId;
                    whiteList.branchName = branchName;
                    whiteList.personState = "Pendiente";
                    Insert(whiteList);
                    return whiteList;
                }
                else
                {
                    return whiteList;
                }
            }
            else
            {
                return whiteList;
            }
        }

        /// <summary>
        /// Método que consulta un registro específico de la lista blanca; partiendo del gimnasio, identificación del usuario y tipo de persona.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="userId"></param>
        /// <param name="personType"></param>
        /// <returns></returns>
        public WhiteList GetUserById(int gymId, string userId, string personType)
        {
            WhiteList response = new WhiteList();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                response = context.WhiteList.FirstOrDefault(wl => wl.gymId == gymId && wl.id == userId && wl.typePerson == personType);
            }

            return response;
        }

        private WhiteList ConvertToWhiteList(spWhiteListCliente_Result whiteListFn, int branchId, string branchName)
        {
            WhiteList whiteList = new WhiteList();

            if (whiteListFn != null)
            {
                whiteList.availableEntries = (whiteListFn.availableEntries == null) ? 0 : Convert.ToInt32(whiteListFn.availableEntries);
                whiteList.branchId = branchId;
                whiteList.branchName = branchName;
                whiteList.cardId = whiteListFn.cardId;
                whiteList.classIntensity = whiteListFn.classIntensity;
                whiteList.className = whiteListFn.className;
                whiteList.classSchedule = whiteListFn.classSchedule;
                whiteList.classState = whiteListFn.classState;
                whiteList.courtesy = (whiteListFn.courtesy == null) ? false : Convert.ToBoolean(whiteListFn.courtesy);
                whiteList.dateClass = whiteListFn.dateClass;
                whiteList.dianId = whiteListFn.dianId;
                whiteList.documentType = whiteListFn.documentType;
                whiteList.employeeName = whiteListFn.employeeName;
                whiteList.fingerprint = whiteListFn.fingerprint;
                whiteList.groupEntriesControl = (whiteListFn.groupEntriesControl == null) ? false : Convert.ToBoolean(whiteListFn.groupEntriesControl);
                whiteList.groupEntriesQuantity = (whiteListFn.groupEntriesQuantity == null) ? 0 : Convert.ToInt32(whiteListFn.groupEntriesQuantity);
                whiteList.groupId = whiteListFn.groupId;
                whiteList.gymId = whiteListFn.gymId;
                whiteList.id = whiteListFn.id;
                whiteList.invoiceId = whiteListFn.invoiceId;
                whiteList.isRestrictionClass = (whiteListFn.isRestrictionClass == null) ? false : Convert.ToBoolean(whiteListFn.isRestrictionClass);
                whiteList.know = (whiteListFn.know == null) ? false : Convert.ToBoolean(whiteListFn.know);
                whiteList.lastEntry = null;
                whiteList.name = whiteListFn.name;
                whiteList.personState = whiteListFn.personState;
                whiteList.photoPath = whiteListFn.photoPath;
                whiteList.planId = whiteListFn.planId;
                whiteList.planName = whiteListFn.planName;
                whiteList.planType = whiteListFn.planType;
                whiteList.reserveId = whiteListFn.reserveId;
                whiteList.restrictions = whiteListFn.restrictions;
                whiteList.subgroupId = whiteListFn.subgroupId;
                whiteList.typePerson = whiteListFn.typePerson;
                whiteList.utilizedMegas = whiteListFn.utilizedMegas;
                whiteList.utilizedTickets = whiteListFn.utilizedTickets;
                whiteList.withoutFingerprint = (whiteListFn.withoutFingerprint == null) ? false : Convert.ToBoolean(whiteListFn.withoutFingerprint);
            }

            return whiteList;
        }

        private void Insert(WhiteList whiteList)
        {
            using (var context = new dbWhiteListModelEntities(whiteList.gymId))
            {
                context.Set<WhiteList>().Add(whiteList);
                context.SaveChanges();
            }
        }

        private void GetWhiteListNotExistingRecord(int gymId, int actualBranch, string clientId)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                context.spWhiteListCliente(gymId, actualBranch, clientId);
            }
        }

        private WhiteList GetWhiteListExistingRecord(int gymId, int actualBranch, int planId, string clientId)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                context.Database.CommandTimeout = 0;
                WhiteList objReturn = context.WhiteList.FirstOrDefault(wl => wl.gymId == gymId && wl.branchId == actualBranch && wl.planId == planId && wl.id == clientId);

                if (objReturn != null)
                {
                    objReturn.tblPalmasId = new List<eClientesPalmas>();
                    objReturn.tblPalmasId = (from h in context.gim_huellas
                                              where h.cdgimnasio == gymId && h.hue_identifi == clientId
                                              select new eClientesPalmas
                                              {
                                                  hue_identifi = h.hue_identifi,
                                                  hue_dedo = h.hue_dedo,
                                                  hue_dato = h.hue_dato,
                                                  hue_id = h.hue_id
                                              }).ToList();
                    objReturn.cantidadhuellas = objReturn.tblPalmasId.Count;

                    return objReturn;
                }

                return null;
            }

        }

        
        /// <summary>
        /// Método que se encarga de consultar la información de un registro de la lista blanca en la BD.
        /// Getulio Vargas - 2018-03-21 - OD 1307
        /// </summary>
        /// <param name="id"></param>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <param name="typePerson"></param>
        /// <returns></returns>
        private WhiteList Get(string id, int gymId, int branchId, string typePerson)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                return context.WhiteList.FirstOrDefault(wl => wl.id == id && wl.gymId == gymId && wl.branchId == branchId && wl.typePerson == typePerson && wl.personState != "Enviado");
            }
        }

        public string RemoveAccent(string text)
        {
            string normalizedText = text.Normalize(NormalizationForm.FormD);
            string pattern = @"[^a-zA-Z0-9\s]";

            string filteredText = Regex.Replace(normalizedText, pattern, "");

            return filteredText;
        }

        public DataTable GetListPersonaConsultar(int gymId, string branchId, string id, bool option)
        {
            try
            {
                DataTable dt = new DataTable();
                using (var db = new dbWhiteListModelEntities(gymId))
                {
                    string strConnection = db.Database.Connection.ConnectionString.ToString();

                    using (SqlConnection conn = new SqlConnection(strConnection))
                    {
                        conn.Open();

                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = "spWhiteListTipoAcceso";
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@action", "GetListarCliente");
                            cmd.Parameters.AddWithValue("@gymId", gymId);
                            cmd.Parameters.AddWithValue("@branchId", branchId.ToString());
                            cmd.Parameters.AddWithValue("@id", id.ToString());
                            cmd.Parameters.AddWithValue("@option", option);

                            //cmd.ExecuteNonQuery();
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dt);
                            }
                            if (dt.Rows.Count >= 1)
                            {
                                return dt;

                            }
                            else
                            {
                                return null;
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public DataTable GetListPersonaConsultar(int gymId, string branchId, string idGrupo)
        {
            try
            {
                DataTable dt = new DataTable();
                using (var db = new dbWhiteListModelEntities(gymId))
                {
                    string strConnection = db.Database.Connection.ConnectionString.ToString();

                    using (SqlConnection conn = new SqlConnection(strConnection))
                    {
                        conn.Open();

                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = "spWhiteListTipoAcceso";
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@action", "GetUsersFamilyGroup");
                            cmd.Parameters.AddWithValue("@gymId", gymId);
                            cmd.Parameters.AddWithValue("@branchId", branchId.ToString());
                            cmd.Parameters.AddWithValue("@groupId", idGrupo);
                            cmd.Parameters.AddWithValue("@option", false);
                            

                            //cmd.ExecuteNonQuery();
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dt);
                            }
                            if (dt.Rows.Count >= 1)
                            {
                                return dt;

                            }
                            else
                            {
                                return null;
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public DataTable GetConsultarEliminacionContratos(int gymId, string branchId, string id)
        {
            try
            {
                DataTable dt = new DataTable();
                using (var db = new dbWhiteListModelEntities(gymId))
                {
                    string strConnection = db.Database.Connection.ConnectionString.ToString();

                    using (SqlConnection conn = new SqlConnection(strConnection))
                    {
                        conn.Open();

                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = "spConsultarEliminacionContratos";
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@intIdGimnasio", gymId);
                            cmd.Parameters.AddWithValue("@id", id);

                            //cmd.ExecuteNonQuery();
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dt);
                            }
                            if (dt.Rows.Count >= 1)
                            {
                                return dt;

                            }
                            else
                            {
                                return dt;
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public DataTable GetRespuestaIngresosVisitantes(int gymId, string id)
        {
            try
            {
                DataTable dt = new DataTable();
          
                using (var db = new dbWhiteListModelEntities(gymId))
                {
                    string strConnection = db.Database.Connection.ConnectionString.ToString();

                    using (SqlConnection conn = new SqlConnection(strConnection))
                    {
                        conn.Open();

                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = "spRespuestaIngresosVisitantes";
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@intIdGimnasio", gymId);
                            cmd.Parameters.AddWithValue("@id", id);

                            //cmd.ExecuteNonQuery();
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dt);
                            }
                            if (dt.Rows.Count >= 1)
                            {
                                return dt;

                            }
                            else
                            {
                                return dt;
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

    }
}
