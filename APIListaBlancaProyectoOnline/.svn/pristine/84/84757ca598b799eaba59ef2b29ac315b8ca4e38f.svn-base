using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Sibo.WhiteList.Classes;
using System.Data.Entity;

namespace Sibo.WhiteList.DAL.Classes
{
    public class ContractDAL
    {
        /// <summary>
        /// Método que consulta el tipo de contrato 1.
        /// Getulio Vargas - 2018-08-24 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        public gim_contrato GetContract(int gymId)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                return context.gim_contrato.FirstOrDefault(c => c.cdgimnasio == gymId && c.cont_codigo == 1);
            }
        }

        /// <summary>
        /// Método que permite insertar el detalle del contrato firmado por el cliente al enrolar.
        /// Getulio Vargas - 2018-08-24 - OD 1307
        /// </summary>
        /// <param name="contractId"></param>
        /// <param name="userId"></param>
        /// <param name="planType"></param>
        /// <param name="invoiceId"></param>
        /// <param name="dianId"></param>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <param name="cont_texto"></param>
        /// <param name="fingerprintImage"></param>
        /// <returns></returns>
        public int Sign(int contractId, string userId, int planType, int invoiceId, int dianId, int gymId, int branchId, string cont_texto, byte[] fingerprintImage)
        {
            int resp = 0, respId = 0;
            respId = GetNextId(gymId);
            gim_detalle_contrato contractDetail = new gim_detalle_contrato()
            {
                bits_replica = "111111111111111111111111111111",
                bool_modificado_replica = true,
                cdgimnasio = gymId,
                dtcont_doc_cliente = Convert.ToDouble(userId),
                dtcont_fecha_firma = DateTime.Now,
                dtcont_firmado_acudiente = false,
                dtcont_FKcontrato = contractId,
                dtcont_fkdia_codigo = dianId,
                dtcont_huella_cliente = fingerprintImage,
                dtcont_modificado = 1,
                dtcont_numcontrato = respId,
                dtcont_numero_plan = invoiceId,
                dtcont_sucursal_plan = branchId,
                dtcont_tipo_plan = planType,
                dtcont_txtContrato = cont_texto
            };

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                context.Set<gim_detalle_contrato>().Add(contractDetail);
                resp = context.SaveChanges();
            }

            if (resp > 0)
            {
                return respId;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Método para consultar el siguiente id de la tabla gim_detalle_contrato en un gimnasio específico.
        /// Getulio Vargas - 2018-08-24 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        private int GetNextId(int gymId)
        {
            List<int> intList = new List<int>();
            int resp = 0;

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                intList = (from cd in context.gim_detalle_contrato
                           where cd.cdgimnasio == gymId
                           orderby cd.dtcont_numcontrato descending
                           select cd.dtcont_numcontrato).ToList();

                if (intList != null && intList.Count > 0)
                {
                    resp = intList.Max() + 1;
                }
                else
                {
                    resp = 1;
                }
            }

            return resp;
        }

        /// <summary>
        /// Método encargado de consultar un contrato firmado por un usuario en un gimnasio específico.
        /// Getulio Vargas - 2018-08-24 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <param name="userId"></param>
        /// <param name="contractDetailId"></param>
        /// <returns></returns>
        public DataTable GetSignedContractById(int gymId, string userId, int contractDetailId)
        {
            DataTable dt = new DataTable();

            using (dbWhiteListModelEntities db = new dbWhiteListModelEntities(gymId))
            {
                string strConnection = db.Database.Connection.ConnectionString;

                using (SqlConnection connection = new SqlConnection(strConnection))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "spContracts";
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@action", "GetSignedContractById");
                        cmd.Parameters.AddWithValue("@gymId", gymId);
                        cmd.Parameters.AddWithValue("@clientId", Convert.ToDouble(userId));
                        cmd.Parameters.AddWithValue("@contractDetailId", contractDetailId);
                        
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        return dt;
                    }
                }
            }
        }

        public gim_detalle_contrato GetClientContract(int gymId, string clientId, int contractId)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                return context.gim_detalle_contrato.FirstOrDefault(p => p.cdgimnasio == gymId && p.dtcont_doc_cliente == Convert.ToDouble(clientId.Trim()) && p.dtcont_FKcontrato == contractId);
            }
        }

        public gim_detalle_contrato GetClientContract2(int gymId, string clientId, int contractId)
        {
            double idNumber = Convert.ToDouble(clientId.Trim());
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                return context.gim_detalle_contrato.FirstOrDefault(p => p.cdgimnasio == gymId && p.dtcont_doc_cliente == idNumber && p.dtcont_FKcontrato == contractId);
            }
        }

        public bool ValidarContrato(int gymId,int branchId ,string clientId)
        {
            gim_detalle_contrato gim = new gim_detalle_contrato();
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                double idNumber = Convert.ToDouble(clientId.Trim());
                gim = context.gim_detalle_contrato.FirstOrDefault(p => p.cdgimnasio == gymId && p.dtcont_doc_cliente == idNumber && p.dtcont_FKcontrato==1);
            }
            if (gim != null)
            {
                return true;

            }
            else
            {
                return false;

            }
        }
        public bool ValidarContratoPorPlan(int gymId, int branchId, string clientId,int? PlanId)
        {
            gim_detalle_contrato gim = new gim_detalle_contrato();
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                double idNumber = Convert.ToDouble(clientId.Trim());
                gim = context.gim_detalle_contrato.FirstOrDefault(p => p.cdgimnasio == gymId && p.dtcont_doc_cliente == idNumber && p.dtcont_numero_plan==PlanId);
            }
            if (gim != null)
            {
                return true;

            }
            else
            {
                return false;

            }
        }

        public bool Insert(eContratos item)
        {
            try
            {
                int resp = 0;


                gim_detalle_contrato newContrato = new gim_detalle_contrato();
                var idNumber = Convert.ToDouble(item.personId);

                newContrato.dtcont_FKcontrato = 1;
                newContrato.dtcont_doc_cliente = idNumber;
                newContrato.dtcont_fecha_firma = DateTime.Now;
                byte[] a = item.fingerPrint.ToArray();
                string image64 = Convert.ToBase64String(a);
                byte[] imageBytes = Convert.FromBase64String(image64);
                newContrato.dtcont_huella_cliente = imageBytes;
                newContrato.cdgimnasio = item.gymId;
                newContrato.dtcont_sucursal_plan = item.branchId;
                newContrato.dtcont_firma_cliente = imageBytes;
                newContrato.bits_replica = "111111111111111111111111111111";
                newContrato.bool_modificado_replica = true;
                newContrato.dtcont_modificado = 1;
               

                DataTable dt = new DataTable();
                
                using (dbWhiteListModelEntities context = new dbWhiteListModelEntities(item.gymId))
                {
                    int cod = 1;
                    gim_detalle_contrato list = context.gim_detalle_contrato.OrderByDescending(e => e.dtcont_numcontrato).Where(p => p.cdgimnasio == item.gymId).FirstOrDefault();
                    if (list != null)
                    {
                        cod = (list.dtcont_numcontrato == null ? 0 : list.dtcont_numcontrato) + 1;
                    }
                    string strConnection = context.Database.Connection.ConnectionString.ToString();

                    using (SqlConnection onea = new SqlConnection(strConnection))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {

                            cmd.Connection = onea;
                            cmd.CommandText = "spContratoDetalle";
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@action", "Inserta");
                            cmd.Parameters.AddWithValue("@dtcont_doc_cliente", idNumber);
                            cmd.Parameters.AddWithValue("@dtcont_codigo_contrato", newContrato.dtcont_FKcontrato);
                            cmd.Parameters.Add("@dtcont_huella_cliente", SqlDbType.Image);
                            cmd.Parameters["@dtcont_huella_cliente"].Value = imageBytes;
                            cmd.Parameters.AddWithValue("@dtcont_numero_plan", newContrato.dtcont_numero_plan);
                            cmd.Parameters.AddWithValue("@dtcont_tipo_plan", newContrato.dtcont_tipo_plan);
                            cmd.Parameters.AddWithValue("@@dtcont_sucursal_plan", newContrato.dtcont_sucursal_plan);
                            cmd.Parameters.AddWithValue("@dtcont_numcontrato", cod);
                            cmd.Parameters.AddWithValue("@dtcont_fkdia_codigo", 0);
                            cmd.Parameters.AddWithValue("@dtcont_txtcontrato", "");
                            //cmd.Parameters.AddWithValue("@dtcont_firma_cliente", "");
                            //cmd.Parameters.AddWithValue("@dtcont_firmado_acudiente", "");
                            cmd.Parameters.AddWithValue("@cdgimnasio", newContrato.cdgimnasio);

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dt);
                            }

                            //objConn.ConnectionDispose();
                            //return dt;
                            if (dt.Rows.Count > 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
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

        //private void UpdateByIdList(string users, int gymId)
        //{
        //    using (var context = new dbWhiteListModelEntities(gymId))
        //    {
        //        string strConnection = context.Database.Connection.ConnectionString.ToString();

        //        using (SqlConnection conn = new SqlConnection(strConnection))
        //        {
        //            conn.Open();

        //            using (SqlCommand cmd = new SqlCommand())
        //            {
        //                cmd.CommandText = "spUpdateWhiteList";
        //                cmd.CommandTimeout = 0;
        //                cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //                cmd.Connection = conn;
        //                cmd.Parameters.AddWithValue("@users", users);
        //                cmd.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //}

        //public bool Update(gim_detalle_contrato contrato, eContratos item)
        //{
        //    int resp = 0;
        //    //fingerprint.hue_calidad = item.quality;
        //    //fingerprint.hue_dato = item.fingerPrint;
        //    ////fingerprint.hue_id = item.id;
        //    //fingerprint.hue_identifi = item.personId;
        //    //fingerprint.intfkSucursal = item.branchId;
        //    //fingerprint.bits_replica = "111111111111111111111111111111";

        //    using (var context = new dbWhiteListModelEntities(item.gymId))
        //    {
        //        context.Entry(contrato).State = EntityState.Modified;
        //        resp = context.SaveChanges();
        //    }

        //    if (resp > 0)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public gim_detalle_contrato GetContrato(int gymId, string clientId, int fingerprintId)
        //{
        //    gim_detalle_contrato fp = new gim_detalle_contrato();
        //    var idNumber = Convert.ToDouble(clientId);

        //    using (var context = new dbWhiteListModelEntities(gymId))
        //    {
        //        var query = (from dtc in context.gim_detalle_contrato
        //                     join c in context.gim_contrato on dtc.dtcont_FKcontrato equals c.cont_codigo
        //                     where c.cdgimnasio == gymId
        //                     && dtc.cdgimnasio == gymId
        //                     && dtc.dtcont_doc_cliente == idNumber
        //                     select new eContratoConDetalle
        //                     {
        //                         tipoContrato = c.int_fkTipoContrato
        //                     }).ToList();
        //        fp = context.gim_detalle_contrato.FirstOrDefault(f => f.cdgimnasio == gymId && f.dtcont_doc_cliente == idNumber);
        //        return fp;
        //    }
        //}
    }
}
