using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Service.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Entities.Classes;

namespace Sibo.WhiteList.BLL.Classes
{
    public class ReservesBLL
    {

        public bool UpdateReserves(eReservesToUpdate reserves)
        {
            try
            {
                ReservesDAL reservesDAL = new ReservesDAL();

                if (reserves != null && reserves.reserveIds.Count > 0)
                {
                    return reservesDAL.UpdateReserves(reserves);
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    throw ex;
                }
            }
            return true;
        }
        public bool UpdateReserve(eReserveToUpdate reserves)
        {
            try
            {
                ReservesDAL reservesDAL = new ReservesDAL();

                if (reserves != null && reserves.reserveId != 0)
                {
                    return reservesDAL.UpdateReserve(reserves);
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    throw ex;
                }
            }
            return true;
        }

        public DataTable GetMisReservasTiquetes(eReservesClient reserves)
        {
            try
            {
                DataTable dt = new DataTable();
                ReservesDAL reservesDAL = new ReservesDAL();

                if (reserves != null && reserves.userId != "")
                {
                    dt = reservesDAL.GetMisReservasTiquetes(reserves.userId, reserves.gymId, reserves.branchId, reserves.resultadoApiReserva);
                }
                return dt;
            }
            catch (Exception ex)
            {

                throw ex;

            }

        }

        public DateTime GetCumpleaniosCliente(string id, int gym)
        {
            try
            {
                gim_clientes cliente = new gim_clientes();
                ReservesDAL reservesDAL = new ReservesDAL();

                if (id != null && id != "")
                {
                    cliente = reservesDAL.GetCumpleaniosCliente(id, gym);
                }
                if (cliente != null)
                {
                    if (cliente.cli_fecha_nacimien != null) {
                        return Convert.ToDateTime(cliente.cli_fecha_nacimien);
                    }
                    else
                    {
                        return DateTime.Now;
                    }
                }
                else
                {
                    return DateTime.Now;
                }

               
            }
            catch (Exception ex)
            {

                throw ex;

            }

        }

        public List<eDatosReserva> GetListReservaID(int gymId, int branchId, int idreserva)
        {
            try
            {
                List<eDatosReserva> listDato = new List<eDatosReserva>();
                ReservesDAL whiteListReservaDAL = new ReservesDAL();
                DataTable dt = new DataTable();

                dt = whiteListReservaDAL.GetListReservaID(gymId, branchId, idreserva);
                listDato = ConvertToEntityReserva(dt);

                return listDato;
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    throw ex;
                }
                else
                {
                    throw new Exception(Exceptions.serverError);
                }
            }
        }

        private List<eDatosReserva> ConvertToEntityReserva(DataTable list)
        {
            try
            {
                List<eDatosReserva> response = new List<eDatosReserva>();
               
                if (list != null && list.Rows.Count > 0)
                {
                    foreach (DataRow item in list.Rows)
                    {
                        eDatosReserva whiteList = new eDatosReserva()
                        {
                              intReserva = item["intReserva"].ToString(),
                            dbvalorGenerado = item["dbvalorGenerado"].ToString(),
                            strClienteReserva = item["strClienteReserva"].ToString(),
                            strNombreClaReserva = item["strNombreClaReserva"].ToString(),
                            intCantMegas = item["intCantMegas"].ToString(),
                            strFechaReserva = Convert.ToDateTime(item["strFechaReserva"]),
                            strHoraReserva = Convert.ToDateTime(item["strHoraReserva"]),
                            strProfesor = item["strProfesor"].ToString(),
                            strUbicacion = item["strUbicacion"].ToString()
                        };

                       
                        response.Add(whiteList);
                    }
                }
                return response;
            }
            catch (Exception ex)
            {


                return null;
            }
        }

    }
}
