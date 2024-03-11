using Sibo.WhiteList.BLL.Classes;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Sibo.WhiteList.DAL;
using System.Threading.Tasks;
using Sibo.WhiteList.Classes;
using Sibo.WhiteList.Service.Classes;
using System.Data;
using Newtonsoft.Json;

namespace Sibo.WhiteList.Api.Controllers
{
    [RoutePrefix("api/WhitelistIngresoTouchMonitor")]
    public class WhitelistIngresoTouchMonitorController : ApiController
    {
        WhiteListBLL whiteList = new WhiteListBLL();


        [HttpPost]
        [Route("ConsultarReservasCliente")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> ConsultarReservasCliente(eReservesClient reserves)
        {

            ReservesBLL reservesBLL = new ReservesBLL();

            try
            {
                if (reserves != null && reserves.gymId != 0 && reserves.userId != "")
                {

                    DataTable dt = new DataTable();
                    dt = reservesBLL.GetMisReservasTiquetes(reserves);
                    bool bitPermiteIngresoconReserva = false;
                    string intReservaFinal = "0";
                    DateTime dtmHoraPrimeraReservaWeb = new DateTime();
                    if (dt != null && dt.Rows.Count > 0)
                    {

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DateTime FechaClase = new DateTime();
                            DateTime FechaActual = new DateTime();
                            FechaActual = DateTime.Now;

                            foreach (DataRow filaDatos in dt.Rows)
                            {
                                FechaActual = DateTime.Now;
                                FechaClase = Convert.ToDateTime(filaDatos["fecha_clase"].ToString());

                                //filaDatos["fecha_clase_impresion"] = (Convert.ToDateTime(filaDatos["fecha_clase"])).ToString("dd/MM/yyyy");
                                //filaDatos["hora_clase_impresion"] = (Convert.ToDateTime(filaDatos["fecha_clase"])).ToString("hh:mm:ss");

                                if (FechaClase.ToString("dd/MM/yyyy") == FechaActual.Date.ToString("dd/MM/yyyy"))
                                {
                                    string intReserva = filaDatos["cdreserva"].ToString();

                                    if (reserves.intMinutosAntesReserva == 0 && reserves.intMinutosDespuesReserva == 0)
                                    {
                                        TimeSpan horaAcutal = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
                                        TimeSpan horaInicialRestrinccion = TimeSpan.Parse(filaDatos["horaDesde"].ToString());
                                        TimeSpan horaFinalRestrinccion = TimeSpan.Parse(filaDatos["horaHasta"].ToString());

                                        if (horaAcutal >= horaInicialRestrinccion && horaAcutal <= horaFinalRestrinccion)
                                        {
                                            dtmHoraPrimeraReservaWeb = FechaClase;
                                            bitPermiteIngresoconReserva = true;
                                            intReservaFinal = (intReserva);

                                            if (reserves.bitTiqueteClaseAsistido_alImprimir)
                                            {
                                                eReserveToUpdate eReserv = new eReserveToUpdate();
                                                eReserv.gymId = reserves.gymId;
                                                eReserv.reserveId = Convert.ToInt32(intReserva);
                                                eReserv.userId = reserves.userId;
                                                reservesBLL.UpdateReserve(eReserv);
                                            }
                                        }

                                    }


                                    if (DateTime.Now >= FechaClase.AddMinutes(-reserves.intMinutosAntesReserva) && DateTime.Now <= FechaClase.AddMinutes(reserves.intMinutosDespuesReserva))
                                    {
                                        dtmHoraPrimeraReservaWeb = FechaClase;
                                        bitPermiteIngresoconReserva = true;
                                        intReservaFinal = (intReserva);

                                        if (reserves.bitTiqueteClaseAsistido_alImprimir)
                                        {
                                            eReserveToUpdate eReserv = new eReserveToUpdate();
                                            eReserv.gymId = reserves.gymId;
                                            eReserv.reserveId = Convert.ToInt32(intReserva);
                                            eReserv.userId = reserves.userId;
                                            reservesBLL.UpdateReserve(eReserv);
                                        }
                                    }
                                }


                            }
                        }

                    }

                    if (bitPermiteIngresoconReserva)
                    {
                        return Ok(intReservaFinal);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    return Ok("Pailas");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }

        [HttpPost]
        [Route("ConsultarReservasClienteDatos")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> ConsultarReservasClienteDatos(eReservesClient reserves)
        {
            ReservesBLL reservesBLL = new ReservesBLL();

            try
            {
                if (reserves != null && reserves.gymId != 0 && reserves.userId != "")
                {
                    DataTable dt = new DataTable();
                    dt = reservesBLL.GetMisReservasTiquetes(reserves);
                    string strDatosFinal = "";
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        strDatosFinal = JsonConvert.SerializeObject(dt);

                        return Ok(strDatosFinal);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    return Ok("Error");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        [Route("ValidarContratoFirmado")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> ValidarContratoFirmado(eValidarContrato datos)
        {

            try
            { 
                ContratoBLL ContraBll = new ContratoBLL();
                if (datos != null && datos.gymId != 0 && datos.userId != "")
                {
                        return Ok(ContraBll.ValidarContrato(datos));
                }
                else
                {
                    return Ok("Falla");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }

        [HttpPost]
        [Route("ValidarContratoFirmadoPorPlan")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> ValidarContratoFirmadoPorPlan(eValidarContrato datos)
        {

            try
            {
                ContratoBLL ContraBll = new ContratoBLL();
                if (datos != null && datos.gymId != 0 && datos.userId != "")
                {
                    return Ok(ContraBll.ValidarContratoFirmadoPorPlan(datos));
                }
                else
                {
                    return Ok("Falla");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }
        [HttpPost]
        [Route("ConsultarCumpleaniosCliente")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> ConsultarCumpleaniosCliente(sEntidadCumpleanios entidad)
        {


            try
            {
                ReservesBLL reservesBLL = new ReservesBLL();
                if (entidad.id != null && entidad.gymId != 0 && entidad.id != "")
                {
                    DateTime dt = new DateTime();
                    dt = reservesBLL.GetCumpleaniosCliente(entidad.id, entidad.gymId);
                    
                    if (dt.Date==DateTime.Now.Date)
                    {

                        return Ok(false);

                    }else if (dt.Month== DateTime.Now.Month && dt.Day== DateTime.Now.Day)
                    {
                        return Ok(true);
                    }
                    else
                    {
                        return Ok(false);
                    }

                    
                }
                else
                {
                    return Ok("Pailas");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }
    }
}
