using System;
using Sibo.WhiteList.Service.Entities.Classes;
using System.Data;
using System.Data.SqlClient;
using Sibo.WhiteList.Service.Connection;

namespace Sibo.WhiteList.Service.Data
{
    public class dAccessControlSettings
    {
        /// <summary>
        /// Método que permite consultar la configuración en la BD local.
        /// Getulio Vargas - 2018-04-06 - OD 1307
        /// </summary>
        /// <returns></returns>
        public eConfiguration GetConfiguration()
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spConfiguration";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "GetConfiguration";

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        return ConvertToConfigurationEntity(dt);
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
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite actualizar la configuración en la BD local.
        /// Getulio Vargas - 2018-04-06 - Od 1307
        /// </summary>
        /// <param name="configEntity"></param>
        /// <returns></returns>
        public bool Update(eConfiguration configEntity)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spConfiguration";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "Update");
                    cmd.Parameters.AddWithValue("@intPkId", configEntity.intPkIdentificador);
                    cmd.Parameters.AddWithValue("@strTipoIngreso", configEntity.strTipoIngreso);
                    cmd.Parameters.AddWithValue("@intMinutosDescontarTiquetes", configEntity.intMinutosDescontarTiquetes);
                    cmd.Parameters.AddWithValue("@bitIngresoEmpSinPlan", configEntity.bitIngresoEmpSinPlan);
                    cmd.Parameters.AddWithValue("@blnPermitirIngresoPantalla", configEntity.blnPermitirIngresoPantalla);
                    cmd.Parameters.AddWithValue("@blnPermiteIngresosAdicionales", configEntity.blnPermiteIngresosAdicionales);
                    cmd.Parameters.AddWithValue("@intMinutosNoReingreso", configEntity.intMinutosNoReingreso);
                    cmd.Parameters.AddWithValue("@intMinutosNoReingresoDia", configEntity.intMinutosNoReingresoDia);
                    cmd.Parameters.AddWithValue("@blnLimpiarDescripcionAdicionales", configEntity.blnLimpiarDescripcionAdicionales);
                    cmd.Parameters.AddWithValue("@blnEntradaCumpleConPlan", configEntity.blnEntradaCumpleConPlan);
                    cmd.Parameters.AddWithValue("@blnEntradaCumpleSinPlan", configEntity.blnEntradaCumpleSinPlan);
                    cmd.Parameters.AddWithValue("@intDiasGraciaClientes", configEntity.intDiasGraciaClientes);
                    cmd.Parameters.AddWithValue("@bitBloqueoCitaNoCumplidaMSW", configEntity.bitBloqueoCitaNoCumplidaMSW);
                    cmd.Parameters.AddWithValue("@bitBloqueoClienteNoApto", configEntity.bitBloqueoClienteNoApto);
                    cmd.Parameters.AddWithValue("@bitBloqueoNoDisentimento", configEntity.bitBloqueoNoDisentimento);
                    cmd.Parameters.AddWithValue("@bitBloqueoNoAutorizacionMenor", configEntity.bitBloqueoNoAutorizacionMenor);
                    cmd.Parameters.AddWithValue("@bitConsultaInfoCita", configEntity.bitConsultaInfoCita);
                    cmd.Parameters.AddWithValue("@bitImprimirHoraReserva", configEntity.bitImprimirHoraReserva);
                    cmd.Parameters.AddWithValue("@bitTiqueteClaseAsistido_alImprimir", configEntity.bitTiqueteClaseAsistido_alImprimir);
                    cmd.Parameters.AddWithValue("@bitAccesoPorReservaWeb", configEntity.bitAccesoPorReservaWeb);
                    cmd.Parameters.AddWithValue("@bitValidarPlanYReservaWeb", configEntity.bitValidarPlanYReservaWeb);
                    cmd.Parameters.AddWithValue("@bitValideContrato", configEntity.bitValideContrato);
                    cmd.Parameters.AddWithValue("@bitValideContratoPorFactura", configEntity.bitValideContratoPorFactura);
                    cmd.Parameters.AddWithValue("@intMinutosAntesReserva", configEntity.intMinutosAntesReserva);
                    cmd.Parameters.AddWithValue("@intMinutosDespuesReserva", configEntity.intMinutosDespuesReserva);
                    cmd.Parameters.AddWithValue("@intdiassincita_bloqueoing", configEntity.intdiassincita_bloqueoing);
                    cmd.Parameters.AddWithValue("@intentradas_sincita_bloqueoing", configEntity.intentradas_sincita_bloqueoing);
                    cmd.Parameters.AddWithValue("@bitIngresoTecladoHuella", configEntity.bitIngresoTecladoHuella);
                    cmd.Parameters.AddWithValue("@strClave", configEntity.strClave);
                    cmd.Parameters.AddWithValue("@strNivelSeguridad", configEntity.strNivelSeguridad);
                    cmd.Parameters.AddWithValue("@bitPermitirBorrarHuella", configEntity.bitPermitirBorrarHuella);
                    cmd.Parameters.AddWithValue("@bitIngresoMiniTouch", configEntity.bitIngresoMiniTouch);
                    cmd.Parameters.AddWithValue("@bitTrabajarConDBEnOtroEquipo", configEntity.bitTrabajarConDBEnOtroEquipo);
                    cmd.Parameters.AddWithValue("@strRutaNombreLogo", configEntity.strRutaNombreLogo);
                    cmd.Parameters.AddWithValue("@intUbicacionTeclasTeclado", configEntity.intUbicacionTeclasTeclado);
                    cmd.Parameters.AddWithValue("@bitBaseDatosSQLServer", configEntity.bitBaseDatosSQLServer);
                    cmd.Parameters.AddWithValue("@intTiempoParaLimpiarPantalla", configEntity.intTiempoParaLimpiarPantalla);
                    cmd.Parameters.AddWithValue("@intTipoUbicacionTeclado", configEntity.intTipoUbicacionTeclado);
                    cmd.Parameters.AddWithValue("@strRutaNombreBanner", configEntity.strRutaNombreBanner);
                    cmd.Parameters.AddWithValue("@bitIngresoAbreDesdeTouch", configEntity.bitIngresoAbreDesdeTouch);
                    cmd.Parameters.AddWithValue("@intNivelSeguirdadLectorUSB", configEntity.intNivelSeguirdadLectorUSB);
                    cmd.Parameters.AddWithValue("@intTimeOutLector", configEntity.intTimeOutLector);
                    cmd.Parameters.AddWithValue("@intTiempoDuermeHiloEnviaComandoSalida", configEntity.intTiempoDuermeHiloEnviaComandoSalida);
                    cmd.Parameters.AddWithValue("@bitMensajeCumpleanos", configEntity.bitMensajeCumpleanos);
                    cmd.Parameters.AddWithValue("@intTiempoActualizaIngresos", configEntity.intTiempoActualizaIngresos);
                    cmd.Parameters.AddWithValue("@intTiempoValidaSiAbrePuerta", configEntity.intTiempoValidaSiAbrePuerta);
                    cmd.Parameters.AddWithValue("@strRutaArchivoGymsoftNet", configEntity.strRutaArchivoGymsoftNet);
                    cmd.Parameters.AddWithValue("@bitComplices_CortxIngs", configEntity.bitComplices_CortxIngs);
                    cmd.Parameters.AddWithValue("@bitComplices_DescuentoTiq", configEntity.bitComplices_DescuentoTiq);
                    cmd.Parameters.AddWithValue("@strTextoMensajeCumpleanos", configEntity.strTextoMensajeCumpleanos);
                    cmd.Parameters.AddWithValue("@bitSolo1HuellaxCliente", configEntity.bitSolo1HuellaxCliente);
                    cmd.Parameters.AddWithValue("@bitMultiplesPlanesVig", configEntity.bitMultiplesPlanesVig);
                    cmd.Parameters.AddWithValue("@intComplices_Plan_CortxIngs", configEntity.intComplices_Plan_CortxIngs);
                    cmd.Parameters.AddWithValue("@strTextoMensajeCortxIngs", configEntity.strTextoMensajeCortxIngs);
                    cmd.Parameters.AddWithValue("@bitImagenSIBO", configEntity.bitImagenSIBO);
                    cmd.Parameters.AddWithValue("@strColorPpal", configEntity.strColorPpal);
                    cmd.Parameters.AddWithValue("@strRutaGuiaMarcacion", configEntity.strRutaGuiaMarcacion);
                    cmd.Parameters.AddWithValue("@intPasoFinalAutoregistroWeb", configEntity.intPasoFinalAutoregistroWeb);
                    cmd.Parameters.AddWithValue("@bitAntipassbackEntrada", configEntity.bitAntipassbackEntrada);
                    cmd.Parameters.AddWithValue("@bitReplicaImgsTCAM7000", configEntity.bitReplicaImgsTCAM7000);
                    cmd.Parameters.AddWithValue("@intTiempoReplicaImgsTCAM7000", configEntity.intTiempoReplicaImgsTCAM7000);
                    cmd.Parameters.AddWithValue("@bitAccesoDiscapacitados", configEntity.bitAccesoDiscapacitados);
                    cmd.Parameters.AddWithValue("@intTiempoEspaciadoTramasReplicaImgsTCAM7000", configEntity.intTiempoEspaciadoTramasReplicaImgsTCAM7000);
                    cmd.Parameters.AddWithValue("@intTiempoMaximoEnvioImgsTCAM7000", configEntity.intTiempoMaximoEnvioImgsTCAM7000);
                    cmd.Parameters.AddWithValue("@intCalidadHuella", configEntity.intCalidadHuella);
                    cmd.Parameters.AddWithValue("@bitEsperarHuellaActualizar", configEntity.bitEsperarHuellaActualizar);
                    cmd.Parameters.AddWithValue("@bitGenerarContratoPDFyEnviar", configEntity.bitGenerarContratoPDFyEnviar);
                    cmd.Parameters.AddWithValue("@intNumeroDiasActualizacionHuella", configEntity.intNumeroDiasActualizacionHuella);
                    cmd.Parameters.AddWithValue("@bitNoValidarHuella", configEntity.bitNoValidarHuella);
                    cmd.Parameters.AddWithValue("@intTiempoReplicaHuellasTCAM7000", configEntity.intTiempoReplicaHuellasTCAM7000);
                    cmd.Parameters.AddWithValue("@intTiempoPingContinuoTCAM7000", configEntity.intTiempoPingContinuoTCAM7000);
                    cmd.Parameters.AddWithValue("@strTipoIdenTributaria", configEntity.strTipoIdenTributaria);
                    cmd.Parameters.AddWithValue("@bitNo_Validar_Entrada_En_Salida", configEntity.bitNo_Validar_Entrada_En_Salida);
                    cmd.Parameters.AddWithValue("@strIdentificadorUno", configEntity.strIdentificadorUno);
                    cmd.Parameters.AddWithValue("@strIdentificadorDos", configEntity.strIdentificadorDos);
                    cmd.Parameters.AddWithValue("@strPuertoCom", configEntity.strPuertoCom);
                    cmd.Parameters.AddWithValue("@intVelocidadPuerto", configEntity.intVelocidadPuerto);
                    cmd.Parameters.AddWithValue("@intTiempoPulso", configEntity.intTiempoPulso);
                    cmd.Parameters.AddWithValue("@bitBloqueoCita", configEntity.bitBloqueoCita);
                    cmd.Parameters.AddWithValue("@intDiasEntreCitaRiesgo", configEntity.intDiasEntreCitaRiesgo);
                    cmd.Parameters.AddWithValue("@intDiasEntreCitaNoRiesgo", configEntity.intDiasEntreCitaNoRiesgo);
                    cmd.Parameters.AddWithValue("@bitCambiarEstadoTiqueteClase", configEntity.bitCambiarEstadoTiqueteClase);
                    cmd.Parameters.AddWithValue("@timeGetWhiteList", configEntity.timeGetWhiteList);
                    cmd.Parameters.AddWithValue("@timeInsertEntries", configEntity.timeInsertEntries);
                    cmd.Parameters.AddWithValue("@timeGetConfiguration", configEntity.timeGetConfiguration);
                    cmd.Parameters.AddWithValue("@timeGetClientMessages", configEntity.timeGetClientMessages);
                    cmd.Parameters.AddWithValue("@timeGetTerminals", configEntity.timeGetTerminals);
                    cmd.Parameters.AddWithValue("@bitFirmarContratoAlEnrolar", configEntity.bitFirmarContratoAlEnrolar);
                    cmd.Parameters.AddWithValue("@strPuertoComSalida", configEntity.strPuertoComSalida);
                    cmd.Parameters.AddWithValue("@bitConsentimientoDatosBiometricos", configEntity.bitConsentimientoDatosBiometricos);
                    cmd.Parameters.AddWithValue("@bitConsentimientoInformado", configEntity.bitConsentimientoInformado);
                    cmd.Parameters.AddWithValue("@bitDatosVirtualesUsuario", configEntity.bitDatosVirtualesUsuario);
                    cmd.Parameters.AddWithValue("@bitValidarHuellaMarcacionSDKAPI", configEntity.bitValidarHuellaMarcacionSDKAPI);
                    cmd.Parameters.AddWithValue("@intTimeoutValidarHuellaMarcacionSDKAPI", configEntity.intTimeoutValidarHuellaMarcacionSDKAPI);
                    cmd.Parameters.AddWithValue("@Validación_de_huella_de_marcación_con_SDK_del_webserver", configEntity.Validación_de_huella_de_marcación_con_SDK_del_webserver);
                    cmd.Parameters.AddWithValue("@Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver", configEntity.Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver);
                    cmd.Parameters.AddWithValue("@bitValidarConfiguracionIngresoWeb", configEntity.bitValidarConfiguracionIngresoWeb);
                    cmd.Parameters.AddWithValue("@SMTPServer", configEntity.SMTPServer);
                    cmd.Parameters.AddWithValue("@user", configEntity.user);
                    cmd.Parameters.AddWithValue("@password", configEntity.password);
                    cmd.Parameters.AddWithValue("@port", configEntity.port);
                    cmd.Parameters.AddWithValue("@timeInsertWhiteListOnTCAM", configEntity.timeInsertWhiteListTCAM);
                    cmd.Parameters.AddWithValue("@allowWhiteListOnTCAM", configEntity.allowWhiteListTCAM);
                    cmd.Parameters.AddWithValue("@timeTerminalConnections", configEntity.timeTerminalConnections);
                    cmd.Parameters.AddWithValue("@intTiempoEsperaRespuestaReplicaHuellasTCAM7000", configEntity.intTiempoEsperaRespuestaReplicaHuellasTCAM7000);
                    cmd.Parameters.AddWithValue("@timeWaitResponseReplicateUsers", configEntity.timeWaitResponseReplicateUsers);
                    cmd.Parameters.AddWithValue("@timeWaitResponseDeleteFingerprint", configEntity.timeWaitResponseDeleteFingerprint);
                    cmd.Parameters.AddWithValue("@timeWaitResponseDeleteUser", configEntity.timeWaitResponseDeleteUser);
                    cmd.Parameters.AddWithValue("@timeResetDownloadEvents", configEntity.timeResetDownloadEvents);
                    cmd.Parameters.AddWithValue("@timeRemoveUsers", configEntity.timeRemoveUsers);
                    cmd.Parameters.AddWithValue("@timeDowndloadEvents", configEntity.timeDowndloadEvents);
                    cmd.Parameters.AddWithValue("@timeHourSync", configEntity.timeHourSync);
                    cmd.Parameters.AddWithValue("@intRangoHoraPrint", configEntity.intRangoHoraPrint);
                    cmd.Parameters.AddWithValue("@timeGetPendingActions", configEntity.timeGetPendingActions);
                    cmd.Parameters.AddWithValue("@timeRemoveFingerprints", configEntity.timeRemoveFingerprints);
                    cmd.Parameters.AddWithValue("@quantityAppAccessControlSimultaneous", configEntity.quantityAppAccessControlSimultaneous);
                    cmd.Parameters.AddWithValue("@bitGenerarCodigoQRdeingresoparavalidarLocalmente", configEntity.bitGenerarCodigoQRdeingresoparavalidarLocalmente);
                    cmd.Parameters.AddWithValue("@bitLectorBiometricoSiempreEncendido", configEntity.bitLectorBiometricoSiempreEncendido);

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
        /// Método que permite insertar la configuración en la BD local.
        /// Getulio Vargas - 2018-0
        /// </summary>
        /// <param name="configEntity"></param>
        /// <returns></returns>
        public bool Insert(eConfiguration configEntity)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spConfiguration";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "Insert");
                    cmd.Parameters.AddWithValue("@bitSucActiva", configEntity.bitSucActiva);
                    cmd.Parameters.AddWithValue("@intfkSucursal", configEntity.intfkSucursal);
                    cmd.Parameters.AddWithValue("@strTipoIngreso", configEntity.strTipoIngreso);
                    cmd.Parameters.AddWithValue("@intMinutosDescontarTiquetes", configEntity.intMinutosDescontarTiquetes);
                    cmd.Parameters.AddWithValue("@bitIngresoEmpSinPlan", configEntity.bitIngresoEmpSinPlan);
                    cmd.Parameters.AddWithValue("@blnPermitirIngresoPantalla", configEntity.blnPermitirIngresoPantalla);
                    cmd.Parameters.AddWithValue("@blnPermiteIngresosAdicionales", configEntity.blnPermiteIngresosAdicionales);
                    cmd.Parameters.AddWithValue("@intMinutosNoReingreso", configEntity.intMinutosNoReingreso);
                    cmd.Parameters.AddWithValue("@intMinutosNoReingresoDia", configEntity.intMinutosNoReingresoDia);
                    cmd.Parameters.AddWithValue("@blnLimpiarDescripcionAdicionales", configEntity.blnLimpiarDescripcionAdicionales);
                    cmd.Parameters.AddWithValue("@blnEntradaCumpleConPlan", configEntity.blnEntradaCumpleConPlan);
                    cmd.Parameters.AddWithValue("@blnEntradaCumpleSinPlan", configEntity.blnEntradaCumpleSinPlan);
                    cmd.Parameters.AddWithValue("@intDiasGraciaClientes", configEntity.intDiasGraciaClientes);
                    cmd.Parameters.AddWithValue("@bitBloqueoCitaNoCumplidaMSW", configEntity.bitBloqueoCitaNoCumplidaMSW);
                    cmd.Parameters.AddWithValue("@bitBloqueoClienteNoApto", configEntity.bitBloqueoClienteNoApto);
                    cmd.Parameters.AddWithValue("@bitBloqueoNoDisentimento", configEntity.bitBloqueoNoDisentimento);
                    cmd.Parameters.AddWithValue("@bitBloqueoNoAutorizacionMenor", configEntity.bitBloqueoNoAutorizacionMenor);
                    cmd.Parameters.AddWithValue("@bitConsultaInfoCita", configEntity.bitConsultaInfoCita);
                    cmd.Parameters.AddWithValue("@bitImprimirHoraReserva", configEntity.bitImprimirHoraReserva);
                    cmd.Parameters.AddWithValue("@bitTiqueteClaseAsistido_alImprimir", configEntity.bitTiqueteClaseAsistido_alImprimir);
                    cmd.Parameters.AddWithValue("@bitAccesoPorReservaWeb", configEntity.bitAccesoPorReservaWeb);
                    cmd.Parameters.AddWithValue("@bitValidarPlanYReservaWeb", configEntity.bitValidarPlanYReservaWeb);
                    cmd.Parameters.AddWithValue("@bitValideContrato", configEntity.bitValideContrato);
                    cmd.Parameters.AddWithValue("@bitValideContratoPorFactura", configEntity.bitValideContratoPorFactura);
                    cmd.Parameters.AddWithValue("@intMinutosAntesReserva", configEntity.intMinutosAntesReserva);
                    cmd.Parameters.AddWithValue("@intMinutosDespuesReserva", configEntity.intMinutosDespuesReserva);
                    cmd.Parameters.AddWithValue("@intdiassincita_bloqueoing", configEntity.intdiassincita_bloqueoing);
                    cmd.Parameters.AddWithValue("@intentradas_sincita_bloqueoing", configEntity.intentradas_sincita_bloqueoing);
                    cmd.Parameters.AddWithValue("@bitIngresoTecladoHuella", configEntity.bitIngresoTecladoHuella);
                    cmd.Parameters.AddWithValue("@strClave", configEntity.strClave);
                    cmd.Parameters.AddWithValue("@strNivelSeguridad", configEntity.strNivelSeguridad);
                    cmd.Parameters.AddWithValue("@bitPermitirBorrarHuella", configEntity.bitPermitirBorrarHuella);
                    cmd.Parameters.AddWithValue("@bitIngresoMiniTouch", configEntity.bitIngresoMiniTouch);
                    cmd.Parameters.AddWithValue("@bitTrabajarConDBEnOtroEquipo", configEntity.bitTrabajarConDBEnOtroEquipo);
                    cmd.Parameters.AddWithValue("@strRutaNombreLogo", configEntity.strRutaNombreLogo);
                    cmd.Parameters.AddWithValue("@intUbicacionTeclasTeclado", configEntity.intUbicacionTeclasTeclado);
                    cmd.Parameters.AddWithValue("@bitBaseDatosSQLServer", configEntity.bitBaseDatosSQLServer);
                    cmd.Parameters.AddWithValue("@intTiempoParaLimpiarPantalla", configEntity.intTiempoParaLimpiarPantalla);
                    cmd.Parameters.AddWithValue("@intTipoUbicacionTeclado", configEntity.intTipoUbicacionTeclado);
                    cmd.Parameters.AddWithValue("@strRutaNombreBanner", configEntity.strRutaNombreBanner);
                    cmd.Parameters.AddWithValue("@bitIngresoAbreDesdeTouch", configEntity.bitIngresoAbreDesdeTouch);
                    cmd.Parameters.AddWithValue("@intNivelSeguirdadLectorUSB", configEntity.intNivelSeguirdadLectorUSB);
                    cmd.Parameters.AddWithValue("@intTimeOutLector", configEntity.intTimeOutLector);
                    cmd.Parameters.AddWithValue("@intTiempoDuermeHiloEnviaComandoSalida", configEntity.intTiempoDuermeHiloEnviaComandoSalida);
                    cmd.Parameters.AddWithValue("@bitMensajeCumpleanos", configEntity.bitMensajeCumpleanos);
                    cmd.Parameters.AddWithValue("@intTiempoActualizaIngresos", configEntity.intTiempoActualizaIngresos);
                    cmd.Parameters.AddWithValue("@intTiempoValidaSiAbrePuerta", configEntity.intTiempoValidaSiAbrePuerta);
                    cmd.Parameters.AddWithValue("@strRutaArchivoGymsoftNet", configEntity.strRutaArchivoGymsoftNet);
                    cmd.Parameters.AddWithValue("@bitComplices_CortxIngs", configEntity.bitComplices_CortxIngs);
                    cmd.Parameters.AddWithValue("@bitComplices_DescuentoTiq", configEntity.bitComplices_DescuentoTiq);
                    cmd.Parameters.AddWithValue("@strTextoMensajeCumpleanos", configEntity.strTextoMensajeCumpleanos);
                    cmd.Parameters.AddWithValue("@bitSolo1HuellaxCliente", configEntity.bitSolo1HuellaxCliente);
                    cmd.Parameters.AddWithValue("@bitMultiplesPlanesVig", configEntity.bitMultiplesPlanesVig);
                    cmd.Parameters.AddWithValue("@intComplices_Plan_CortxIngs", configEntity.intComplices_Plan_CortxIngs);
                    cmd.Parameters.AddWithValue("@strTextoMensajeCortxIngs", configEntity.strTextoMensajeCortxIngs);
                    cmd.Parameters.AddWithValue("@bitImagenSIBO", configEntity.bitImagenSIBO);
                    cmd.Parameters.AddWithValue("@strColorPpal", configEntity.strColorPpal);
                    cmd.Parameters.AddWithValue("@strRutaGuiaMarcacion", configEntity.strRutaGuiaMarcacion);
                    cmd.Parameters.AddWithValue("@intPasoFinalAutoregistroWeb", configEntity.intPasoFinalAutoregistroWeb);
                    cmd.Parameters.AddWithValue("@bitAntipassbackEntrada", configEntity.bitAntipassbackEntrada);
                    cmd.Parameters.AddWithValue("@bitReplicaImgsTCAM7000", configEntity.bitReplicaImgsTCAM7000);
                    cmd.Parameters.AddWithValue("@intTiempoReplicaImgsTCAM7000", configEntity.intTiempoReplicaImgsTCAM7000);
                    cmd.Parameters.AddWithValue("@bitAccesoDiscapacitados", configEntity.bitAccesoDiscapacitados);
                    cmd.Parameters.AddWithValue("@intTiempoEspaciadoTramasReplicaImgsTCAM7000", configEntity.intTiempoEspaciadoTramasReplicaImgsTCAM7000);
                    cmd.Parameters.AddWithValue("@intTiempoMaximoEnvioImgsTCAM7000", configEntity.intTiempoMaximoEnvioImgsTCAM7000);
                    cmd.Parameters.AddWithValue("@intCalidadHuella", configEntity.intCalidadHuella);
                    cmd.Parameters.AddWithValue("@bitEsperarHuellaActualizar", configEntity.bitEsperarHuellaActualizar);
                    cmd.Parameters.AddWithValue("@bitGenerarContratoPDFyEnviar", configEntity.bitGenerarContratoPDFyEnviar);
                    cmd.Parameters.AddWithValue("@intNumeroDiasActualizacionHuella", configEntity.intNumeroDiasActualizacionHuella);
                    cmd.Parameters.AddWithValue("@bitNoValidarHuella", configEntity.bitNoValidarHuella);
                    cmd.Parameters.AddWithValue("@intTiempoReplicaHuellasTCAM7000", configEntity.intTiempoReplicaHuellasTCAM7000);
                    cmd.Parameters.AddWithValue("@intTiempoPingContinuoTCAM7000", configEntity.intTiempoPingContinuoTCAM7000);
                    cmd.Parameters.AddWithValue("@strTipoIdenTributaria", configEntity.strTipoIdenTributaria);
                    cmd.Parameters.AddWithValue("@bitNo_Validar_Entrada_En_Salida", configEntity.bitNo_Validar_Entrada_En_Salida);
                    cmd.Parameters.AddWithValue("@strIdentificadorUno", configEntity.strIdentificadorUno);
                    cmd.Parameters.AddWithValue("@strIdentificadorDos", configEntity.strIdentificadorDos);
                    cmd.Parameters.AddWithValue("@strPuertoCom", configEntity.strPuertoCom);
                    cmd.Parameters.AddWithValue("@intVelocidadPuerto", configEntity.intVelocidadPuerto);
                    cmd.Parameters.AddWithValue("@intTiempoPulso", configEntity.intTiempoPulso);
                    cmd.Parameters.AddWithValue("@bitBloqueoCita", configEntity.bitBloqueoCita);
                    cmd.Parameters.AddWithValue("@intDiasEntreCitaRiesgo", configEntity.intDiasEntreCitaRiesgo);
                    cmd.Parameters.AddWithValue("@intDiasEntreCitaNoRiesgo", configEntity.intDiasEntreCitaNoRiesgo);
                    cmd.Parameters.AddWithValue("@bitCambiarEstadoTiqueteClase", configEntity.bitCambiarEstadoTiqueteClase);
                    cmd.Parameters.AddWithValue("@timeGetWhiteList", configEntity.timeGetWhiteList);
                    cmd.Parameters.AddWithValue("@timeInsertEntries", configEntity.timeInsertEntries);
                    cmd.Parameters.AddWithValue("@bitFirmarContratoAlEnrolar", configEntity.bitFirmarContratoAlEnrolar);
                    cmd.Parameters.AddWithValue("@intRangoHoraPrint", configEntity.intRangoHoraPrint);
                    cmd.Parameters.AddWithValue("@timeGetConfiguration", configEntity.timeGetConfiguration);
                    cmd.Parameters.AddWithValue("@timeGetClientMessages", configEntity.timeGetClientMessages);
                    cmd.Parameters.AddWithValue("@timeGetTerminals", configEntity.timeGetTerminals);
                    cmd.Parameters.AddWithValue("@strPuertoComSalida", configEntity.strPuertoComSalida);
                    cmd.Parameters.AddWithValue("@bitConsentimientoDatosBiometricos", configEntity.bitConsentimientoDatosBiometricos);
                    cmd.Parameters.AddWithValue("@bitConsentimientoInformado", configEntity.bitConsentimientoInformado);
                    cmd.Parameters.AddWithValue("@bitDatosVirtualesUsuario", configEntity.bitDatosVirtualesUsuario);
                    cmd.Parameters.AddWithValue("@bitValidarHuellaMarcacionSDKAPI", configEntity.bitValidarHuellaMarcacionSDKAPI);
                    cmd.Parameters.AddWithValue("@Validación_de_huella_de_marcación_con_SDK_del_webserver", configEntity.Validación_de_huella_de_marcación_con_SDK_del_webserver);
                    cmd.Parameters.AddWithValue("@intTimeoutValidarHuellaMarcacionSDKAPI", configEntity.intTimeoutValidarHuellaMarcacionSDKAPI);
                    cmd.Parameters.AddWithValue("@Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver", configEntity.Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver);
                    cmd.Parameters.AddWithValue("@bitValidarConfiguracionIngresoWeb", configEntity.bitValidarConfiguracionIngresoWeb);
                    cmd.Parameters.AddWithValue("@SMTPServer", configEntity.SMTPServer);
                    cmd.Parameters.AddWithValue("@user", configEntity.user);
                    cmd.Parameters.AddWithValue("@password", configEntity.password);
                    cmd.Parameters.AddWithValue("@port", configEntity.port);
                    cmd.Parameters.AddWithValue("@timeInsertWhiteListOnTCAM", configEntity.timeInsertWhiteListTCAM);
                    cmd.Parameters.AddWithValue("@allowWhiteListOnTCAM", configEntity.allowWhiteListTCAM);
                    cmd.Parameters.AddWithValue("@timeTerminalConnections", configEntity.timeTerminalConnections);
                    cmd.Parameters.AddWithValue("@intTiempoEsperaRespuestaReplicaHuellasTCAM7000", configEntity.intTiempoEsperaRespuestaReplicaHuellasTCAM7000);
                    cmd.Parameters.AddWithValue("@timeWaitResponseReplicateUsers", configEntity.timeWaitResponseReplicateUsers);
                    cmd.Parameters.AddWithValue("@timeWaitResponseDeleteFingerprint", configEntity.timeWaitResponseDeleteFingerprint);
                    cmd.Parameters.AddWithValue("@timeWaitResponseDeleteUser", configEntity.timeWaitResponseDeleteUser);
                    cmd.Parameters.AddWithValue("@timeResetDownloadEvents", configEntity.timeResetDownloadEvents);
                    cmd.Parameters.AddWithValue("@timeRemoveUsers", configEntity.timeRemoveUsers);
                    cmd.Parameters.AddWithValue("@timeDowndloadEvents", configEntity.timeDowndloadEvents);
                    cmd.Parameters.AddWithValue("@timeHourSync", configEntity.timeHourSync);
                    cmd.Parameters.AddWithValue("@timeGetPendingActions", configEntity.timeGetPendingActions);
                    cmd.Parameters.AddWithValue("@timeRemoveFingerprints", configEntity.timeRemoveFingerprints);
                    cmd.Parameters.AddWithValue("@quantityAppAccessControlSimultaneous", configEntity.quantityAppAccessControlSimultaneous);
                    cmd.Parameters.AddWithValue("@bitGenerarCodigoQRdeingresoparavalidarLocalmente", configEntity.bitGenerarCodigoQRdeingresoparavalidarLocalmente);
                    cmd.Parameters.AddWithValue("@bitLectorBiometricoSiempreEncendido", configEntity.bitLectorBiometricoSiempreEncendido);

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
        /// Método que permite convertir el DataRow de tblConfiguration en la entidad eConfiguration
        /// Getulio Vargas - 2018-04-06 - OD 1307
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private eConfiguration ConvertToConfigurationEntity(DataTable dt)
        {
            eConfiguration response = new eConfiguration();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                response.intPkIdentificador = Convert.IsDBNull(row["intPkId"]) ? 0 : Convert.ToInt32(row["intPkId"]);
                response.bitAccesoDiscapacitados = Convert.IsDBNull(row["bitAccesoDiscapacitados"]) ? false : Convert.ToBoolean(row["bitAccesoDiscapacitados"]);
                response.bitAccesoPorReservaWeb = Convert.IsDBNull(row["bitAccesoPorReservaWeb"]) ? false : Convert.ToBoolean(row["bitAccesoPorReservaWeb"]);
                response.bitAntipassbackEntrada = Convert.IsDBNull(row["bitAntipassbackEntrada"]) ? false : Convert.ToBoolean(row["bitAntipassbackEntrada"]);
                response.bitBaseDatosSQLServer = Convert.IsDBNull(row["bitBaseDatosSQLServer"]) ? false : Convert.ToBoolean(row["bitBaseDatosSQLServer"]);
                response.bitBloqueoCita = Convert.IsDBNull(row["bitBloqueoCita"]) ? false : Convert.ToBoolean(row["bitBloqueoCita"]);
                response.bitBloqueoCitaNoCumplidaMSW = Convert.IsDBNull(row["bitBloqueoCitaNoCumplidaMSW"]) ? false : Convert.ToBoolean(row["bitBloqueoCitaNoCumplidaMSW"]);
                response.bitBloqueoClienteNoApto = Convert.IsDBNull(row["bitBloqueoClienteNoApto"]) ? false : Convert.ToBoolean(row["bitBloqueoClienteNoApto"]);
                response.bitBloqueoNoAutorizacionMenor = Convert.IsDBNull(row["bitBloqueoNoAutorizacionMenor"]) ? false : Convert.ToBoolean(row["bitBloqueoNoAutorizacionMenor"]);
                response.bitBloqueoNoDisentimento = Convert.IsDBNull(row["bitBloqueoNoDisentimento"]) ? false : Convert.ToBoolean(row["bitBloqueoNoDisentimento"]);
                response.bitCambiarEstadoTiqueteClase = Convert.IsDBNull(row["bitCambiarEstadoTiqueteClase"]) ? false : Convert.ToBoolean(row["bitCambiarEstadoTiqueteClase"]);
                response.bitComplices_CortxIngs = Convert.IsDBNull(row["bitComplices_CortxIngs"]) ? false : Convert.ToBoolean(row["bitComplices_CortxIngs"]);
                response.bitComplices_DescuentoTiq = Convert.IsDBNull(row["bitComplices_DescuentoTiq"]) ? false : Convert.ToBoolean(row["bitComplices_DescuentoTiq"]);
                response.bitConsultaInfoCita = Convert.IsDBNull(row["bitConsultaInfoCita"]) ? false : Convert.ToBoolean(row["bitConsultaInfoCita"]);
                response.bitEsperarHuellaActualizar = Convert.IsDBNull(row["bitEsperarHuellaActualizar"]) ? false : Convert.ToBoolean(row["bitEsperarHuellaActualizar"]);
                response.bitFirmarContratoAlEnrolar = Convert.IsDBNull(row["bitFirmarContratoAlEnrolar"]) ? false : Convert.ToBoolean(row["bitFirmarContratoAlEnrolar"]);
                response.bitGenerarContratoPDFyEnviar = Convert.IsDBNull(row["bitGenerarContratoPDFyEnviar"]) ? false : Convert.ToBoolean(row["bitGenerarContratoPDFyEnviar"]);
                response.bitImagenSIBO = Convert.IsDBNull(row["bitImagenSIBO"]) ? false : Convert.ToBoolean(row["bitImagenSIBO"]);
                response.bitImprimirHoraReserva = Convert.IsDBNull(row["bitImprimirHoraReserva"]) ? false : Convert.ToBoolean(row["bitImprimirHoraReserva"]);
                response.bitIngresoAbreDesdeTouch = Convert.IsDBNull(row["bitIngresoAbreDesdeTouch"]) ? false : Convert.ToBoolean(row["bitIngresoAbreDesdeTouch"]);
                response.bitIngresoEmpSinPlan = Convert.IsDBNull(row["bitIngresoEmpSinPlan"]) ? false : Convert.ToBoolean(row["bitIngresoEmpSinPlan"]);
                response.bitIngresoMiniTouch = Convert.IsDBNull(row["bitIngresoMiniTouch"]) ? false : Convert.ToBoolean(row["bitIngresoMiniTouch"]);
                response.bitIngresoTecladoHuella = Convert.IsDBNull(row["bitIngresoTecladoHuella"]) ? false : Convert.ToBoolean(row["bitIngresoTecladoHuella"]);
                response.bitMensajeCumpleanos = Convert.IsDBNull(row["bitMensajeCumpleanos"]) ? false : Convert.ToBoolean(row["bitMensajeCumpleanos"]);
                response.bitMultiplesPlanesVig = Convert.IsDBNull(row["bitMultiplesPlanesVig"]) ? false : Convert.ToBoolean(row["bitMultiplesPlanesVig"]);
                response.bitNoValidarHuella = Convert.IsDBNull(row["bitNoValidarHuella"]) ? false : Convert.ToBoolean(row["bitNoValidarHuella"]);
                response.bitNo_Validar_Entrada_En_Salida = Convert.IsDBNull(row["bitNo_Validar_Entrada_En_Salida"]) ? false : Convert.ToBoolean(row["bitNo_Validar_Entrada_En_Salida"]);
                response.bitConsentimientoDatosBiometricos = Convert.IsDBNull(row["bitConsentimientoDatosBiometricos"]) ? false : Convert.ToBoolean(row["bitConsentimientoDatosBiometricos"]);
                response.bitConsentimientoInformado = Convert.IsDBNull(row["bitConsentimientoInformado"]) ? false : Convert.ToBoolean(row["bitConsentimientoInformado"]);
                response.bitDatosVirtualesUsuario = Convert.IsDBNull(row["bitDatosVirtualesUsuario"]) ? false : Convert.ToBoolean(row["bitDatosVirtualesUsuario"]);
                response.bitPermitirBorrarHuella = Convert.IsDBNull(row["bitPermitirBorrarHuella"]) ? false : Convert.ToBoolean(row["bitPermitirBorrarHuella"]);
                response.bitReplicaImgsTCAM7000 = Convert.IsDBNull(row["bitReplicaImgsTCAM7000"]) ? false : Convert.ToBoolean(row["bitReplicaImgsTCAM7000"]);
                response.bitSolo1HuellaxCliente = Convert.IsDBNull(row["bitSolo1HuellaxCliente"]) ? false : Convert.ToBoolean(row["bitSolo1HuellaxCliente"]);
                response.bitSucActiva = Convert.IsDBNull(row["bitSucActiva"]) ? false : Convert.ToBoolean(row["bitSucActiva"]);
                response.bitTiqueteClaseAsistido_alImprimir = Convert.IsDBNull(row["bitTiqueteClaseAsistido_alImprimir"]) ? false : Convert.ToBoolean(row["bitTiqueteClaseAsistido_alImprimir"]);
                response.bitTrabajarConDBEnOtroEquipo = Convert.IsDBNull(row["bitTrabajarConDBEnOtroEquipo"]) ? false : Convert.ToBoolean(row["bitTrabajarConDBEnOtroEquipo"]);
                response.bitValidarPlanYReservaWeb = Convert.IsDBNull(row["bitValidarPlanYReservaWeb"]) ? false : Convert.ToBoolean(row["bitValidarPlanYReservaWeb"]);
                response.bitValideContrato = Convert.IsDBNull(row["bitValideContrato"]) ? false : Convert.ToBoolean(row["bitValideContrato"]);
                response.bitValideContratoPorFactura = Convert.IsDBNull(row["bitValideContratoPorFactura"]) ? false : Convert.ToBoolean(row["bitValideContratoPorFactura"]);
                response.blnEntradaCumpleConPlan = Convert.IsDBNull(row["blnEntradaCumpleConPlan"]) ? false : Convert.ToBoolean(row["blnEntradaCumpleConPlan"]);
                response.blnEntradaCumpleSinPlan = Convert.IsDBNull(row["blnEntradaCumpleSinPlan"]) ? false : Convert.ToBoolean(row["blnEntradaCumpleSinPlan"]);
                response.blnLimpiarDescripcionAdicionales = Convert.IsDBNull(row["blnLimpiarDescripcionAdicionales"]) ? false : Convert.ToBoolean(row["blnLimpiarDescripcionAdicionales"]);
                response.blnPermiteIngresosAdicionales = Convert.IsDBNull(row["blnPermiteIngresosAdicionales"]) ? false : Convert.ToBoolean(row["blnPermiteIngresosAdicionales"]);
                response.blnPermitirIngresoPantalla = Convert.IsDBNull(row["blnPermitirIngresoPantalla"]) ? false : Convert.ToBoolean(row["blnPermitirIngresoPantalla"]);
                response.intCalidadHuella = Convert.IsDBNull(row["intCalidadHuella"]) ? 0 : Convert.ToInt32(row["intCalidadHuella"]);
                response.intComplices_Plan_CortxIngs = Convert.IsDBNull(row["intComplices_Plan_CortxIngs"]) ? 0 : Convert.ToInt32(row["intComplices_Plan_CortxIngs"]);
                response.intDiasEntreCitaNoRiesgo = Convert.IsDBNull(row["intDiasEntreCitaNoRiesgo"]) ? 0 : Convert.ToInt32(row["intDiasEntreCitaNoRiesgo"]);
                response.intDiasEntreCitaRiesgo = Convert.IsDBNull(row["intDiasEntreCitaRiesgo"]) ? 0 : Convert.ToInt32(row["intDiasEntreCitaRiesgo"]);
                response.intDiasGraciaClientes = Convert.IsDBNull(row["intDiasGraciaClientes"]) ? 0 : Convert.ToInt32(row["intDiasGraciaClientes"]);
                response.intdiassincita_bloqueoing = Convert.IsDBNull(row["intdiassincita_bloqueoing"]) ? 0 : Convert.ToInt32(row["intdiassincita_bloqueoing"]);
                response.intentradas_sincita_bloqueoing = Convert.IsDBNull(row["intentradas_sincita_bloqueoing"]) ? 0 : Convert.ToInt32(row["intentradas_sincita_bloqueoing"]);
                response.intfkSucursal = Convert.IsDBNull(row["intfkSucursal"]) ? 0 : Convert.ToInt32(row["intfkSucursal"]);
                response.intMinutosAntesReserva = Convert.ToInt32(row["intMinutosAntesReserva"] ?? 0);
                response.intMinutosDescontarTiquetes = Convert.IsDBNull(row["intMinutosDescontarTiquetes"]) ? 0 : Convert.ToInt32(row["intMinutosDescontarTiquetes"]);
                response.intMinutosDespuesReserva = Convert.IsDBNull(row["intMinutosDespuesReserva"]) ? 0 : Convert.ToInt32(row["intMinutosDespuesReserva"]);
                response.intMinutosNoReingreso = Convert.IsDBNull(row["intMinutosNoReingreso"]) ? 0 : Convert.ToInt32(row["intMinutosNoReingreso"]);
                response.intNivelSeguirdadLectorUSB = Convert.IsDBNull(row["intNivelSeguirdadLectorUSB"]) ? 0 : Convert.ToInt32(row["intNivelSeguirdadLectorUSB"]);
                response.intNumeroDiasActualizacionHuella = Convert.IsDBNull(row["intNumeroDiasActualizacionHuella"]) ? 0 : Convert.ToInt32(row["intNumeroDiasActualizacionHuella"]);
                response.intPasoFinalAutoregistroWeb = Convert.IsDBNull(row["intPasoFinalAutoregistroWeb"]) ? 0 : Convert.ToInt32(row["intPasoFinalAutoregistroWeb"]);
                response.intRangoHoraPrint = Convert.IsDBNull(row["intRangoHoraPrint"]) ? 0 : Convert.ToInt32(row["intRangoHoraPrint"]);
                response.intTiempoActualizaIngresos = Convert.IsDBNull(row["intTiempoActualizaIngresos"]) ? 0 : Convert.ToInt32(row["intTiempoActualizaIngresos"]);
                response.intTiempoDuermeHiloEnviaComandoSalida = Convert.IsDBNull(row["intTiempoDuermeHiloEnviaComandoSalida"]) ? 0 : Convert.ToInt32(row["intTiempoDuermeHiloEnviaComandoSalida"]);
                response.intTiempoEspaciadoTramasReplicaImgsTCAM7000 = Convert.IsDBNull(row["intTiempoEspaciadoTramasReplicaImgsTCAM7000"]) ? 0 : Convert.ToInt32(row["intTiempoEspaciadoTramasReplicaImgsTCAM7000"]);
                response.intTiempoMaximoEnvioImgsTCAM7000 = Convert.IsDBNull(row["intTiempoMaximoEnvioImgsTCAM7000"]) ? 0 : Convert.ToInt32(row["intTiempoMaximoEnvioImgsTCAM7000"]);
                response.intTiempoParaLimpiarPantalla = Convert.IsDBNull(row["intTiempoParaLimpiarPantalla"]) ? 0 : Convert.ToInt32(row["intTiempoParaLimpiarPantalla"]);
                response.intTiempoPingContinuoTCAM7000 = Convert.IsDBNull(row["intTiempoPingContinuoTCAM7000"]) ? 0 : Convert.ToInt32(row["intTiempoPingContinuoTCAM7000"]);
                response.intTiempoPulso = Convert.IsDBNull(row["intTiempoPulso"]) ? 0 : Convert.ToInt32(row["intTiempoPulso"]);
                response.intTiempoReplicaHuellasTCAM7000 = Convert.IsDBNull(row["intTiempoReplicaHuellasTCAM7000"]) ? 0 : Convert.ToInt32(row["intTiempoReplicaHuellasTCAM7000"]);
                response.intTiempoReplicaImgsTCAM7000 = Convert.IsDBNull(row["intTiempoReplicaImgsTCAM7000"]) ? 0 : Convert.ToInt32(row["intTiempoReplicaImgsTCAM7000"]);
                response.intTiempoValidaSiAbrePuerta = Convert.IsDBNull(row["intTiempoValidaSiAbrePuerta"]) ? 0 : Convert.ToInt32(row["intTiempoValidaSiAbrePuerta"]);
                response.intTimeOutLector = Convert.IsDBNull(row["intTimeOutLector"]) ? 0 : Convert.ToInt32(row["intTimeOutLector"]);
                response.intTipoUbicacionTeclado = Convert.IsDBNull(row["intTipoUbicacionTeclado"]) ? 0 : Convert.ToInt32(row["intTipoUbicacionTeclado"]);
                response.intUbicacionTeclasTeclado = Convert.IsDBNull(row["intUbicacionTeclasTeclado"]) ? 0 : Convert.ToInt32(row["intUbicacionTeclasTeclado"]);
                response.intVelocidadPuerto = Convert.IsDBNull(row["intVelocidadPuerto"]) ? 0 : Convert.ToInt32(row["intVelocidadPuerto"]);
                response.strClave = row["strClave"].ToString();
                response.strColorPpal = row["strColorPpal"].ToString();
                response.strIdentificadorDos = row["strIdentificadorDos"].ToString();
                response.strIdentificadorUno = row["strIdentificadorUno"].ToString();
                response.strNivelSeguridad = row["strNivelSeguridad"].ToString();
                response.strPuertoCom = row["strPuertoCom"].ToString();
                response.strRutaArchivoGymsoftNet = row["strRutaArchivoGymsoftNet"].ToString();
                response.strRutaGuiaMarcacion = row["strRutaGuiaMarcacion"].ToString();
                response.strRutaNombreBanner = row["strRutaNombreBanner"].ToString();
                response.strRutaNombreLogo = row["strRutaNombreLogo"].ToString();
                response.strTextoMensajeCortxIngs = row["strTextoMensajeCortxIngs"].ToString();
                response.strTextoMensajeCumpleanos = row["strTextoMensajeCumpleanos"].ToString();
                response.strTipoIdenTributaria = row["strTipoIdenTributaria"].ToString();
                response.strTipoIngreso = row["strTipoIngreso"].ToString();
                response.timeGetWhiteList = Convert.IsDBNull(row["timeGetWhiteList"]) ? 0 : Convert.ToInt32(row["timeGetWhiteList"]);
                response.timeInsertEntries = Convert.IsDBNull(row["timeInsertEntries"]) ? 0 : Convert.ToInt32(row["timeInsertEntries"]);
                response.timeGetConfiguration = Convert.IsDBNull(row["timeGetConfiguration"]) ? 0 : Convert.ToInt32(row["timeGetConfiguration"]);
                response.timeGetClientMessages = Convert.IsDBNull(row["timeGetClientMessages"]) ? 0 : Convert.ToInt32(row["timeGetClientMessages"]);
                response.timeGetTerminals = Convert.IsDBNull(row["timeGetTerminals"]) ? 0 : Convert.ToInt32(row["timeGetTerminals"]);
                response.strPuertoComSalida = row["strPuertoComSalida"].ToString();
                response.bitValidarHuellaMarcacionSDKAPI = Convert.IsDBNull(row["bitValidarHuellaMarcacionSDKAPI"]) ? false : Convert.ToBoolean(row["bitValidarHuellaMarcacionSDKAPI"]);
                response.Validación_de_huella_de_marcación_con_SDK_del_webserver = Convert.IsDBNull(row["Validación_de_huella_de_marcación_con_SDK_del_webserver"]) ? false : Convert.ToBoolean(row["Validación_de_huella_de_marcación_con_SDK_del_webserver"]);
                response.intTimeoutValidarHuellaMarcacionSDKAPI = Convert.IsDBNull(row["intTimeoutValidarHuellaMarcacionSDKAPI"]) ? 0 : Convert.ToInt32(row["intTimeoutValidarHuellaMarcacionSDKAPI"]);
                response.Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver = Convert.IsDBNull(row["Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver"]) ? 0 : Convert.ToInt32(row["Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver"]);
                response.bitValidarConfiguracionIngresoWeb = Convert.IsDBNull(row["bitValidarConfiguracionIngresoWeb"]) ? false : Convert.ToBoolean(row["bitValidarConfiguracionIngresoWeb"]);
                response.SMTPServer = row["SMTPServer"].ToString();
                response.SMTPServer = row["strUser"].ToString();
                response.SMTPServer = row["strPassword"].ToString();
                response.SMTPServer = row["strPort"].ToString();
                response.allowWhiteListTCAM = Convert.IsDBNull(row["allowWhiteListOnTCAM"]) ? false : Convert.ToBoolean(row["allowWhiteListOnTCAM"]);
                response.timeInsertWhiteListTCAM = Convert.IsDBNull(row["timeInsertWhiteListOnTCAM"]) ? 0 : Convert.ToInt32(row["timeInsertWhiteListOnTCAM"]);
                response.timeTerminalConnections = Convert.IsDBNull(row["timeTerminalConnections"]) ? 0 : Convert.ToInt32(row["timeTerminalConnections"]);
                response.intTiempoEsperaRespuestaReplicaHuellasTCAM7000 = Convert.IsDBNull(row["intTiempoEsperaRespuestaReplicaHuellasTCAM7000"]) ? 0 : Convert.ToInt32(row["intTiempoEsperaRespuestaReplicaHuellasTCAM7000"]);
                response.timeWaitResponseReplicateUsers = Convert.IsDBNull(row["timeWaitResponseReplicateUsers"]) ? 0 : Convert.ToInt32(row["timeWaitResponseReplicateUsers"]);
                response.timeWaitResponseDeleteFingerprint = Convert.IsDBNull(row["timeWaitResponseDeleteFingerprint"]) ? 0 : Convert.ToInt32(row["timeWaitResponseDeleteFingerprint"]);
                response.timeWaitResponseDeleteUser = Convert.IsDBNull(row["timeWaitResponseDeleteUser"]) ? 0 : Convert.ToInt32(row["timeWaitResponseDeleteUser"]);
                response.timeResetDownloadEvents = Convert.IsDBNull(row["timeResetDownloadEvents"]) ? 0 : Convert.ToInt32(row["timeResetDownloadEvents"]);
                response.timeRemoveUsers = Convert.IsDBNull(row["timeRemoveUsers"]) ? 0 : Convert.ToInt32(row["timeRemoveUsers"]);
                response.timeDowndloadEvents = Convert.IsDBNull(row["timeDowndloadEvents"]) ? 0 : Convert.ToInt32(row["timeDowndloadEvents"]);
                response.timeHourSync = Convert.IsDBNull(row["timeHourSync"]) ? 0 : Convert.ToInt32(row["timeHourSync"]);
                response.timeGetPendingActions = Convert.IsDBNull(row["timeGetPendingActions"]) ? 0 : Convert.ToInt32(row["timeGetPendingActions"]);
                response.timeRemoveFingerprints = Convert.IsDBNull(row["timeRemoveFingerprints"]) ? 0 : Convert.ToInt32(row["timeRemoveFingerprints"]);
                response.quantityAppAccessControlSimultaneous = Convert.IsDBNull(row["quantityAppAccessControlSimultaneous"]) ? 0 : Convert.ToInt32(row["quantityAppAccessControlSimultaneous"]);
                response.bitLectorBiometricoSiempreEncendido = Convert.IsDBNull(row["bitLectorBiometricoSiempreEncendido"]) ? false : Convert.ToBoolean(row["bitLectorBiometricoSiempreEncendido"]);
                response.intMinutosNoReingresoDia = Convert.IsDBNull(row["intMinutosNoReingresoDia"]) ? 0 : Convert.ToInt32(row["intMinutosNoReingresoDia"]);
            }

            return response;
        }
    }
}
