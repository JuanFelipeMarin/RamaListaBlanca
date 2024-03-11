using Sibo.WhiteList.DAL.Helpers;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Classes;
using Sibo.WhiteList.Service.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Sibo.WhiteList.DAL.Classes
{
    public class ReservesDAL
    {
        public bool UpdateReserves(eReservesToUpdate reserves)
        {
            using (var context = new dbWhiteListModelEntities(reserves.gymId))
            {
                foreach (int reserveId in reserves.reserveIds)
                {
                    gim_reservas reservasDB = context.gim_reservas
                        .FirstOrDefault(r => r.IdentificacionCliente == reserves.userId
                                 && r.cdgimnasio == reserves.gymId
                                 && r.cdreserva == reserveId);

                    if (reservasDB != null)
                    {
                        reservasDB.estado = "Asistido";
                        context.SaveChanges();
                    }

                }
            }
            return true;
        }
        public bool UpdateReserve(eReserveToUpdate reserves)
        {
            using (var context = new dbWhiteListModelEntities(reserves.gymId))
            {
                gim_reservas reservasDB = context.gim_reservas
                    .FirstOrDefault(r => r.IdentificacionCliente == reserves.userId
                             && r.cdgimnasio == reserves.gymId
                             && r.cdreserva == reserves.reserveId);

                if (reservasDB != null)
                {
                    reservasDB.estado = "Asistido";
                    context.SaveChanges();
                }

            }
            return true;
        }

        public DataTable GetMisReservasTiquetes(string cdusuario, int cdgimnasio, int intsucursal,string resultadoApiReserva)
        {
            try
            {
                DataTable dt = new DataTable();
                using (var db = new dbWhiteListModelEntities(cdgimnasio))
                {
                    string strConnection = db.Database.Connection.ConnectionString.ToString();

                    using (SqlConnection conn = new SqlConnection(strConnection))
                    {
                        conn.Open();

                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = "SP_GetMisReservasTiquetes";
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@cdusuario", cdusuario);
                            cmd.Parameters.AddWithValue("@cdgimnasio", cdgimnasio.ToString());
                            cmd.Parameters.AddWithValue("@intsucursal", intsucursal.ToString());
                            cmd.Parameters.AddWithValue("@resultadoApiReserva", resultadoApiReserva.ToString());

                            
                            //cmd.ExecuteNonQuery();
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dt);
                            }
                            if (dt.Rows.Count >= 1)
                            {
                                dt.TableName = "MisReservas";

                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    dt.Rows[i]["profesor"] = consultarNombreProfesorPorClase(cdgimnasio, intsucursal, Convert.ToInt32(dt.Rows[i]["cdclase"]), Convert.ToDateTime(dt.Rows[i]["fecha_clase"]));
                                }
                            }
                            return dt;
                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string consultarNombreProfesorPorClase(int gymId, int brancId, int cdClase, DateTime fechaClase)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                string strConnection = context.Database.Connection.ConnectionString.ToString();
                using (SqlConnection conexion = new SqlConnection(strConnection))
                {
                    conexion.Open();
                    using (SqlCommand cmdDetalle = new SqlCommand("spConsultarNombreProfesorClase", conexion))
                    {
                        cmdDetalle.CommandType = System.Data.CommandType.StoredProcedure;
                        cmdDetalle.CommandTimeout = 0;
                        cmdDetalle.Parameters.AddWithValue("@gymId", gymId);
                        cmdDetalle.Parameters.AddWithValue("@brancId", brancId);
                        cmdDetalle.Parameters.AddWithValue("@cdClase", cdClase);
                        cmdDetalle.Parameters.AddWithValue("@fechaClase", fechaClase);
                        using (SqlDataAdapter daDetalle = new SqlDataAdapter(cmdDetalle))
                        {
                            DataTable dtResultado = new DataTable();
                            daDetalle.Fill(dtResultado);
                            daDetalle.Dispose();
                            cmdDetalle.Dispose();
                            conexion.Close();
                            conexion.Dispose();
                            if (dtResultado.Rows.Count == 0)
                                return "";
                         
                            return dtResultado.Rows[0]["nombreCompletoProfesor"].ToString();
                        }
                    }
                }
            }
        }

        public gim_clientes GetCumpleaniosCliente(string id, int cdgimnasio)
        {
            try
            {
                using (var db = new dbWhiteListModelEntities(cdgimnasio))
                {
                    return db.gim_clientes.Where(p => p.cdgimnasio == cdgimnasio && p.cli_identifi == id.Trim()).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetListReservaID(int gymId, int branchId, int idreserva)
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
                            cmd.CommandText = "GetReservasDisponible";
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@gymId", gymId);
                            cmd.Parameters.AddWithValue("@branchId", branchId.ToString());
                            cmd.Parameters.AddWithValue("@idreserva", idreserva.ToString());
                           
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


    }
}
