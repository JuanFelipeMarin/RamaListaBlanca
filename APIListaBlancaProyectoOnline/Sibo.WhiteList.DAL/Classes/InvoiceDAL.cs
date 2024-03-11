using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Classes;

namespace Sibo.WhiteList.DAL.Classes
{
    public class InvoiceDAL
    {
        public gim_planes_usuario GetVigentInvoice(int gymId, string id)
        {
            gim_planes_usuario invoice = new gim_planes_usuario();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                invoice = (from inv in context.gim_planes_usuario
                           where inv.cdgimnasio == gymId && inv.plusu_identifi_cliente == id &&
                                 inv.plusu_fecha_inicio <= DateTime.Today &&
                                 inv.plusu_fecha_vcto.Value >= DateTime.Today && inv.plusu_avisado != true &&
                                 inv.plusu_codigo_plan != 999 && inv.plusu_est_anulada != true
                           select inv).FirstOrDefault();

                return invoice;
            }
        }

        /// <summary>
        /// Método que consulta y retorna en un DataTable las facturas vigentes de un cliente específico.
        /// Getulio Vargas - 2018-08-23 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataTable GetVigentPlansByClient(int gymId, string userId)
        {
            DataTable dt = new DataTable();

            using (dbWhiteListModelEntities db = new dbWhiteListModelEntities(gymId))
            {
                string strConnection = db.Database.Connection.ConnectionString;

                using (SqlConnection connection = new SqlConnection(strConnection))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        connection.Open();
                        cmd.Connection = connection;
                        cmd.CommandText = "spInvoices";
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@action", "GetVigentPlansByClient");
                        cmd.Parameters.AddWithValue("@gymId", gymId);
                        cmd.Parameters.AddWithValue("@clientId", userId);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                        
                        return dt;
                    }
                }
            }
        }
    }
}
