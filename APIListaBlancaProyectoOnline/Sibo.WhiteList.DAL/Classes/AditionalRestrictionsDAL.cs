using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    /// <summary>
    /// Clase que conecta a diferentes tablas de GSW para hacer validaciones extras a la lista blanca
    /// </summary>
    public class AditionalRestrictionsDAL
    {
        /// <summary>
        /// Pregunta si un cliente tiene un contrato (gim_detalle_contrato)
        /// MToro
        /// </summary>
        /// <param name="id">id del cliente</param>
        /// <param name="gymId"></param>
        /// <returns></returns>
        /// //ORIGINAL
        //public bool hasContract(string id, int gymId)
        //{
        //    using (var context = new dbWhiteListModelEntities(gymId))
        //    {
        //        double idNumber = Convert.ToDouble(id);
        //        gim_detalle_contrato detalleContrato =
        //            context.gim_detalle_contrato.FirstOrDefault(d => d.dtcont_doc_cliente == idNumber
        //                                        && d.cdgimnasio == gymId);

        //        if (detalleContrato == null)
        //            return false;
        //        else
        //            return true;
        //    }
        //}

        public bool hasContract(string id, int gymId)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                double idNumber = Convert.ToDouble(id);

                //Misma validación que tiene el SP de spGenerateWhitelist y solo se valida el contrato general ya que el sp solo valida ese tipo de contrato
                var detalleContrato = (from dtc in context.gim_detalle_contrato
                                       join contrato in context.gim_contrato on dtc.dtcont_FKcontrato equals contrato.cont_codigo
                                       where contrato.cdgimnasio == gymId
                                       && dtc.cdgimnasio == gymId
                                       && contrato.int_fkTipoContrato == 1
                                       && dtc.cdgimnasio == contrato.cdgimnasio
                                       && dtc.dtcont_doc_cliente == idNumber
                                       select dtc).FirstOrDefault();

                if (detalleContrato == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Busca el cliente en GSW y comprueba sí es de alto riesgo (cli_altoRiesgo),
        /// en cuyo caso comprueba si ha asistido a alguna cita de la tabla de tblCitas
        /// </summary>
        /// <param name="id">id del cliente</param>
        /// <param name="gymId">id del gimnasio</param>
        /// <returns></returns>
        public bool isHealthy(string id, int gymId)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                var cliente = context.gim_clientes.FirstOrDefault(c => c.cli_identifi == id && c.cdgimnasio == gymId);

                if (cliente != null && (cliente.cli_altoRiesgo ?? false))
                {
                    var cita = context.tblCitas.FirstOrDefault(c => c.strIdentificacionPaciente == id && c.intEmpresa == gymId && c.bitAtendida);

                    return cita != null && cita.bitAtendida;
                }

                return cliente != null;
            }
        }

        //QUITAR FUNCIONALIDADES
        public bool hasContractPorFactura(string id, int gymId, int sucursal, int dian, int numero_factura)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                double idNumber = Convert.ToDouble(id);

                var query = (from dtc in context.gim_detalle_contrato 
                            join pu in context.gim_planes_usuario on dtc.dtcont_numero_plan equals pu.plusu_numero_fact 
                            where pu.cdgimnasio == gymId 
                            && dtc.cdgimnasio == gymId
                            && dtc.dtcont_doc_cliente == idNumber
                            && pu.plusu_identifi_cliente == id
                            && dtc.dtcont_sucursal_plan == sucursal
                            && pu.plusu_sucursal == sucursal
                            && dtc.dtcont_fkdia_codigo == dian
                            && pu.plusu_fkdia_codigo == dian
                            && pu.plusu_numero_fact == numero_factura
                            && dtc.dtcont_numero_plan == numero_factura
                            select dtc).FirstOrDefault();

                if (query == null)
                    return false;
                else
                    return true;
            }
        }

        public bool hasReserva(string id, int gymId, int sucursal, int factura)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                //double idNumber = Convert.ToDouble(id);

                //REVISAR BIEN MAÑANA Y DEJELO QUIETO!!!!
                var query = (from reserva in context.gim_reservas
                             
                             where reserva.cdgimnasio == gymId
                             && reserva.cdsucursal == sucursal
                             && reserva.IdentificacionCliente == id
                             
                             //clase día despues
                             && reserva.fecha_clase > DateTime.Now
                             
                             select reserva).FirstOrDefault();

                if (query == null)
                    return true;
                else
                    return false;
            }
        }

    }
}
