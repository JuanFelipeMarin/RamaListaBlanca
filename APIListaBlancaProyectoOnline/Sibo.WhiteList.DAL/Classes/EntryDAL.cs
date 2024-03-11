using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Sibo.WhiteList.DAL.Classes
{
    public class EntryDAL
    {
        public bool AddEntry(List<eEntry> entriesList)
        {
            InvoiceDAL invDAL = new InvoiceDAL();
            CourtesyDAL courDAL = new CourtesyDAL();

            foreach (eEntry item in entriesList)
            {
                if(item.thirdMessage == "HightTemp")
                {
                    InsertHightTempEvent(item);
                    continue;
                }

                decimal? decimalTemp = null;
                if (!string.IsNullOrEmpty(item.temperature))
                {
                    decimalTemp = Convert.ToDecimal(item.temperature);
                }

                gim_entradas_usuarios entry = new gim_entradas_usuarios()
                {
                    bits_replica = "111111111111111111111111111111",
                    bool_modificado_replica = true,
                    cdgimnasio = item.gymId,
                    enusu_blnrestotiquete = item.discountTicket,
                    enusu_identifi = item.clientId,
                    enusu_intnumero = item.invoiceId,
                    enusu_modificado = 1,
                    enusu_numero = GetNextId(item.gymId, item.branchId),
                    enusu_plan = item.planId,
                    enusu_referenciacodigo = item.invoiceId.ToString(),
                    enusu_referenciadesc = item.documentType,
                    enusu_sede = item.branchId,
                    enusu_strconcepto = item.documentType,
                    enusu_strtipoventa = item.documentType,
                    enusu_sucursal = Convert.ToInt16(item.branchId),
                    enusu_VisitId = item.visitId,
                    entryCode = item.qrCode,
                    entryTemperature = decimalTemp,
                };

                if (!Convert.IsDBNull(item.entryDate) && item.entryDate != new DateTime(1900, 1, 1))
                {
                    entry.enusu_fecha_entrada = item.entryDate;
                }

                if (!Convert.IsDBNull(item.outDate) && item.outDate != new DateTime(1900, 1, 1))
                {
                    entry.enusu_fecha_salida = item.outDate;
                }

                if (!Convert.IsDBNull(item.entryHour) && item.entryHour != new DateTime(1900, 1, 1))
                {
                    entry.enusu_hora_entrada = item.entryHour;
                }

                if (!Convert.IsDBNull(item.entryHour) && item.outHour != new DateTime(1900, 1, 1))
                {
                    entry.enusu_hora_salida = item.outHour;
                }
                
                Insert(entry);
            }

            return true;
        }

        public bool AddEntryAdicional(List<eEntry> entriesList)
        {
            InvoiceDAL invDAL = new InvoiceDAL();
            CourtesyDAL courDAL = new CourtesyDAL();

            foreach (eEntry item in entriesList)
            {
                gim_entradas_adicionales dataEntradaAdicional = new gim_entradas_adicionales();
                int nuevoId = 1;
                using (var context = new dbWhiteListModelEntities(item.gymId))
                {

                    dataEntradaAdicional = context.gim_entradas_adicionales.Where(p => p.cdgimnasio == item.gymId).OrderByDescending(b => b.adi_numero).FirstOrDefault();
                    if (dataEntradaAdicional == null)
                    {
                        nuevoId = 1;
                    }
                    else
                    {
                        nuevoId = dataEntradaAdicional.adi_numero + 1;
                    }
                }
                gim_entradas_adicionales entry = new gim_entradas_adicionales();
                entry.adi_numero = nuevoId;
                entry.cdgimnasio = item.gymId;
                entry.adi_descripcion = item.secondMessage;
                entry.adi_fecha = item.entryDate;
                entry.adi_hora = item.entryDate;
                entry.adi_modificado = 0;
                entry.adi_sucursal = Convert.ToInt16(item.branchId);
                entry.bool_modificado_replica = true;
                entry.bits_replica = "true";
                entry.adi_IDPersona = item.clientId;
                entry.adi_NombrePersona = item.clientName;
                entry.adi_EmpresaPersona = item.strEmpresaIngresoAdicional;
                entry.adi_emp_usuario = item.usuario;


                InsertIngresoAdicional(entry);

            }
            return true;
        }
        private void InsertHightTempEvent(eEntry item)
        {
            using (var context = new dbWhiteListModelEntities(item.gymId))
            {
                try
                {
                    string temp = item.temperature.Replace(".", "");
                    int tempInt = Convert.ToInt32(temp);
                    gim_canceledForTemperature obj = new gim_canceledForTemperature()
                    {
                        identifi = item.clientId,
                        cdgimnasio = item.gymId,
                        branchId = item.branchId,
                        entryHour = item.entryHour,
                        qr = item.qrCode,
                        temperature = tempInt,
                        reason = "Temperatura corporal anormal",
                    };

                    context.gim_canceledForTemperature.Add(obj);
                    context.SaveChanges();
                    return;
                }
                catch (Exception)
                {
                    return;
                }

            }
        }

        private int GetNextId(int gymId, int branchId)
        {
            gim_entradas_usuarios userEntry = new gim_entradas_usuarios();
            int num = 0;

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                num = (from ue in context.gim_entradas_usuarios
                       where ue.cdgimnasio == gymId && ue.enusu_sucursal == branchId
                       orderby ue.enusu_numero descending
                       select ue.enusu_numero).Take(1).FirstOrDefault();

                if (num == null)
                {
                    return 1;
                }
                else
                {
                    return (num + 1);
                }
            }
        }

        private void Insert(gim_entradas_usuarios entry)
        {
            using (var context = new dbWhiteListModelEntities(entry.cdgimnasio))
            {
                context.Set<gim_entradas_usuarios>().Add(entry);
                context.SaveChanges();
            }
        }

        private void InsertIngresoAdicional(gim_entradas_adicionales entry)
        {
            //using (dbXpacios_NewEntities context = new dbXpacios_NewEntities(entry.cdgimnasio))
            //{
            //    context.gim_entradas_adicionales.Add(entry);
            //    context.SaveChanges();
            //}

            using (var context = new dbWhiteListModelEntities(entry.cdgimnasio))
            {
                string strConnection = context.Database.Connection.ConnectionString.ToString();

                using (SqlConnection conn = new SqlConnection(strConnection))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "spInsert_gim_entradas_adicionales";
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Connection = conn;
                        cmd.Parameters.AddWithValue("@action", "Insert");
                        cmd.Parameters.AddWithValue("@adi_numero", entry.adi_numero);
                        cmd.Parameters.AddWithValue("@cdgimnasio", entry.cdgimnasio);
                        cmd.Parameters.AddWithValue("@adi_descripcion", entry.adi_descripcion);
                        cmd.Parameters.AddWithValue("@adi_fecha", entry.adi_fecha);
                        cmd.Parameters.AddWithValue("@adi_hora", entry.adi_hora);
                        cmd.Parameters.AddWithValue("@adi_modificado", entry.adi_modificado);
                        cmd.Parameters.AddWithValue("@adi_sucursal", entry.adi_sucursal);
                        cmd.Parameters.AddWithValue("@bool_modificado_replica", entry.bool_modificado_replica);
                        cmd.Parameters.AddWithValue("@bits_replica", entry.bits_replica);
                        cmd.Parameters.AddWithValue("@adi_IDPersona", entry.adi_IDPersona);
                        cmd.Parameters.AddWithValue("@adi_NombrePersona", entry.adi_NombrePersona);
                        cmd.Parameters.AddWithValue("@adi_EmpresaPersona", entry.adi_EmpresaPersona);
                        cmd.Parameters.AddWithValue("@adi_emp_usuario", entry.adi_emp_usuario);
                        cmd.Parameters.AddWithValue("@adi_tipoBarrera", entry.adi_tipoBarrera);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }

        public List<eEntry> GetListEntradasUsuarios(int gymId, string branchId, string id, bool TipoPlan)
        {
            List<eEntry> list = new List<eEntry>();
            gim_entradas_usuarios listt;
            List<gim_entradas_usuarios> listtt;
            eEntry elist = new eEntry();
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                if (TipoPlan == true)
                {
                    listtt = (from evento in context.gim_entradas_usuarios
                            where evento.cdgimnasio == gymId && evento.enusu_identifi == id && evento.enusu_plan == 0
                            orderby evento.enusu_numero descending
                            select evento).ToList();

                    elist = new eEntry()
                    {
                        intPkId = listtt[0].enusu_numero,
                        clientId = listtt[0].enusu_identifi,
                        planId = listtt[0].enusu_plan == null ? 0 : Convert.ToInt32(listtt[0].enusu_plan),
                        entryDate = Convert.ToDateTime(listtt[0].enusu_fecha_entrada),
                        entryHour = Convert.ToDateTime(listtt[0].enusu_hora_entrada),
                        outDate = Convert.ToDateTime(listtt[0].enusu_fecha_salida),
                        outHour = Convert.ToDateTime(listtt[0].enusu_hora_salida),
                        thirdMessage = listtt[0].entryCode.ToString(),
                        cantidadREgistrosEntrada = listtt.Count()
                    };

                }
                else
                {
                    listt = (from evento in context.gim_entradas_usuarios
                             where evento.cdgimnasio == gymId && evento.enusu_identifi == id
                             orderby evento.enusu_numero descending
                             select evento).FirstOrDefault();

                     elist = new eEntry()
                    {
                        intPkId = listt.enusu_numero,
                        clientId = listt.enusu_identifi,
                        planId = listt.enusu_plan == null ? 0 : Convert.ToInt32(listt.enusu_plan),
                        entryDate = Convert.ToDateTime(listt.enusu_fecha_entrada),
                        entryHour = Convert.ToDateTime(listt.enusu_hora_entrada),
                        outDate = Convert.ToDateTime(listt.enusu_fecha_salida),
                        outHour = Convert.ToDateTime(listt.enusu_hora_salida),
                        thirdMessage = listt.entryCode.ToString()
                    };

                }

           
                

            list.Add(elist);

             }

            return list;
        }
    }
}
