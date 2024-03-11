using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.Classes;

namespace Sibo.WhiteList.BLL.Classes
{
    public class AccessControlSettingsBLL
    {
        /// <summary>
        /// Método que se encarga de validar los campos para consultar la configuración del ingreso.
        /// Getulio Vargas - 2018-03-21 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public List<eConfiguration> GetAccessControlConfigurationSpecialEntity(int gymId, string branchId)
        {
            try
            {
                AccessControlSettingsDAL acs = new AccessControlSettingsDAL();
                List<gim_configuracion_ingreso> config = new List<gim_configuracion_ingreso>();
                tblConfiguracion generalConfig = new tblConfiguracion();
                eMailServerData serverData = new eMailServerData();
                MailServerDataBLL msdBLL = new MailServerDataBLL();
                ConfigurationBLL configBLL = new ConfigurationBLL();
                Validation val = new Validation();

                //if (val.ValidateGym(gymId) && val.ValidateBranch(branchId))
                if (val.ValidateGym(gymId) && val.ValidateBranch(branchId))
                {
                    config = acs.GetAccessControlConfiguration(gymId, branchId);

                    List<tblConfiguracion_FirmaContratosAcceso> lstConfigContratos = acs.getContratosConfiguracion(gymId);

                    foreach (var item in config)
                    {
                        if (lstConfigContratos != null)
                        {
                            item.bitConsentimientoInformado = lstConfigContratos.FirstOrDefault(x => x.intFkIdTipoContrato == 1).bitEstado;
                            item.bitConsentimientoDatosBiometricos = lstConfigContratos.FirstOrDefault(x => x.intFkIdTipoContrato == 3).bitEstado;
                            item.bitValideContrato = lstConfigContratos.FirstOrDefault(x => x.intFkIdTipoContrato == 5).bitEstado;
                        }
                        else
                        {
                            item.bitConsentimientoInformado = false;
                            item.bitConsentimientoDatosBiometricos = false;
                            item.bitValideContrato = false;
                        }
                    }
                }

                generalConfig = configBLL.GetConfiguration(gymId);
                serverData = msdBLL.GetMailServerData(gymId);

                return ConvertToConfiguration(config, generalConfig, serverData);
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

        /// <summary>
        /// Método que permite consultar y retornar el parámetro que determina si la lista blanca local debe ser depurada o no.
        /// Getulio Vargas - 2018-04-04 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        //public bool GetConfigToResetLocalWhiteList(int gymId, string branchId)
        //{
        //    try
        //    {
        //        AccessControlSettingsDAL acs = new AccessControlSettingsDAL();
        //        gim_configuracion_ingreso config = new gim_configuracion_ingreso();
        //        bool response = false;
        //        Validation val = new Validation();

                
        //        if (val.ValidateGym(gymId) && val.ValidateBranch(branchId))
        //        {
        //            config = acs.GetAccessControlConfiguration(gymId, branchId);

        //            List<tblConfiguracion_FirmaContratosAcceso> lstConfigContratos = acs.getContratosConfiguracion(gymId);

        //            if (lstConfigContratos != null)
        //            {
        //                config.bitConsentimientoInformado = lstConfigContratos.FirstOrDefault(x => x.intFkIdTipoContrato == 1).bitEstado;
        //                config.bitConsentimientoDatosBiometricos = lstConfigContratos.FirstOrDefault(x => x.intFkIdTipoContrato == 3).bitEstado;
        //                config.bitValideContrato = lstConfigContratos.FirstOrDefault(x => x.intFkIdTipoContrato == 5).bitEstado;
        //            }
        //            else
        //            {
        //                config.bitConsentimientoInformado = false;
        //                config.bitConsentimientoDatosBiometricos = false;
        //                config.bitValideContrato = false;
        //            }
        //        }

        //        if (config != null)
        //        {
        //            response = (config.bitResetLocalWhiteList == null) ? false : Convert.ToBoolean(config.bitResetLocalWhiteList);
        //        }
                
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex != null)
        //        {
                    
        //            throw ex;
        //        }
        //        else
        //        {
        //            throw new Exception(Exceptions.serverError);
        //        }
        //    }
        //}

        /// <summary>
        /// Método que permite actualizar el parámetro de reset de la lista blanca luego de que esta se actualizó localmente.
        /// Getulio Vargas - 2018-04-04 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        //public bool UpdateConfigToResetLocalWhiteList(int gymId, string branchId)
        //{
        //    try
        //    {
        //        AccessControlSettingsDAL acs = new AccessControlSettingsDAL();
        //        return acs.UpdateConfigToResetLocalWhiteList(gymId, branchId);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex != null)
        //        {
        //            throw ex;
        //        }
        //        else
        //        {
        //            throw new Exception(Exceptions.serverError);
        //        }
        //    }
        //}

        /// <summary>
        /// Método que se encarga de validar los campos para consultar la configuración del ingreso.
        /// Getulio Vargas - 2018-03-21 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public gim_configuracion_ingreso GetAccessControlConfiguration(int gymId, string branchId)
        {
            try
            {
                AccessControlSettingsDAL acs = new AccessControlSettingsDAL();
                gim_configuracion_ingreso config = new gim_configuracion_ingreso();
                Validation val = new Validation();

                if (val.ValidateGym(gymId) && val.ValidateBranch(branchId))
                {
                    //config = acs.GetAccessControlConfiguration(gymId, branchId);
                    List<tblConfiguracion_FirmaContratosAcceso> lstConfigContratos = acs.getContratosConfiguracion(gymId);

                    if (lstConfigContratos != null)
                    {
                        config.bitConsentimientoInformado = lstConfigContratos.FirstOrDefault(x => x.intFkIdTipoContrato == 1).bitEstado;
                        config.bitConsentimientoDatosBiometricos = lstConfigContratos.FirstOrDefault(x => x.intFkIdTipoContrato == 3).bitEstado;
                        config.bitValideContrato = lstConfigContratos.FirstOrDefault(x => x.intFkIdTipoContrato == 5).bitEstado;
                    }
                    else
                    {
                        config.bitConsentimientoInformado = false;
                        config.bitConsentimientoDatosBiometricos = false;
                        config.bitValideContrato = false;
                    }
                }

                return config;
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

        /// <summary>
        /// Método que se encarga de convertir una entidad "gim_configuracion_ingreso" a una "eConfiguration"
        /// Getulio Vargas - 2018-04-04 - OD 1307
        /// </summary>
        /// <param name="config"></param>
        /// <param name="generalConfig"></param>
        /// <returns></returns>
        private List<eConfiguration> ConvertToConfiguration(List<gim_configuracion_ingreso> config, tblConfiguracion generalConfig, eMailServerData serverData)
        {
            try
            {
                List<eConfiguration> ListConfig = new List<eConfiguration>();
                eConfiguration eConfig = new eConfiguration();
                foreach (var item in config)
                {
                                       
                    if (config != null)
                    {
                        eConfig.bitAccesoDiscapacitados = Convert.ToBoolean(item.bitAccesoDiscapacitados ?? false);
                        eConfig.bitAccesoPorReservaWeb = Convert.ToBoolean(item.bitAccesoPorReservaWeb ?? false);
                        eConfig.bitAntipassbackEntrada = Convert.ToBoolean(item.bitAntipassbackEntrada ?? false);
                        eConfig.bitBaseDatosSQLServer = Convert.ToBoolean(item.bitBaseDatosSQLServer ?? false);
                        eConfig.bitBloqueoCita = Convert.ToBoolean(item.bitBloqueoCita ?? false);
                        eConfig.bitBloqueoCitaNoCumplidaMSW = Convert.ToBoolean(item.bitBloqueoCitaNoCumplidaMSW ?? false);
                        eConfig.bitBloqueoClienteNoApto = Convert.ToBoolean(item.bitBloqueoClienteNoApto ?? false);
                        eConfig.bitBloqueoNoAutorizacionMenor = Convert.ToBoolean(item.bitBloqueoNoAutorizacionMenor ?? false);
                        eConfig.bitBloqueoNoDisentimento = Convert.ToBoolean(item.bitBloqueoNoDisentimento ?? false);
                        eConfig.bitCambiarEstadoTiqueteClase = Convert.ToBoolean(item.bitCambiarEstadoTiqueteClase ?? false);
                        eConfig.bitComplices_CortxIngs = Convert.ToBoolean(item.bitComplices_CortxIngs ?? false);
                        eConfig.bitComplices_DescuentoTiq = Convert.ToBoolean(item.bitComplices_DescuentoTiq ?? false);
                        eConfig.bitConsultaInfoCita = Convert.ToBoolean(item.bitConsultaInfoCita ?? false);
                        eConfig.bitConsentimientoDatosBiometricos = Convert.ToBoolean(item.bitConsentimientoDatosBiometricos ?? false);
                        eConfig.bitConsentimientoInformado = Convert.ToBoolean(item.bitConsentimientoInformado ?? false);
                        eConfig.bitDatosVirtualesUsuario = Convert.ToBoolean(item.bitDatosVirtualesUsuario ?? false);
                        eConfig.bitEsperarHuellaActualizar = Convert.ToBoolean(item.bitEsperarHuellaActualizar ?? false);
                        eConfig.bitFirmarContratoAlEnrolar = Convert.ToBoolean(item.bitFirmarContratoAlEnrolar ?? false);
                        eConfig.bitGenerarContratoPDFyEnviar = Convert.ToBoolean(item.bitGenerarContratoPDFyEnviar ?? false);
                        eConfig.bitImagenSIBO = Convert.ToBoolean(item.bitImagenSIBO ?? false);
                        eConfig.bitImprimirHoraReserva = Convert.ToBoolean(item.bitImprimirHoraReserva ?? false);
                        eConfig.bitIngresoAbreDesdeTouch = Convert.ToBoolean(item.bitIngresoAbreDesdeTouch ?? false);
                        eConfig.bitIngresoEmpSinPlan = Convert.ToBoolean(item.bitIngresoEmpSinPlan ?? false);
                        eConfig.bitIngresoMiniTouch = Convert.ToBoolean(item.bitIngresoMiniTouch ?? false);
                        eConfig.bitIngresoTecladoHuella = Convert.ToBoolean(item.bitIngresoTecladoHuella ?? false);
                        eConfig.bitMensajeCumpleanos = Convert.ToBoolean(item.bitMensajeCumpleanos ?? false);
                        eConfig.bitMultiplesPlanesVig = Convert.ToBoolean(item.bitMultiplesPlanesVig ?? false);
                        eConfig.bitNoValidarHuella = Convert.ToBoolean(item.bitNoValidarHuella ?? false);
                        eConfig.bitNo_Validar_Entrada_En_Salida = Convert.ToBoolean(item.bitNo_Validar_Entrada_En_Salida ?? false);
                        eConfig.bitPermitirBorrarHuella = Convert.ToBoolean(item.bitPermitirBorrarHuella ?? false);
                        eConfig.bitReplicaImgsTCAM7000 = Convert.ToBoolean(item.bitReplicaImgsTCAM7000 ?? false);
                        eConfig.bitSolo1HuellaxCliente = Convert.ToBoolean(item.bitSolo1HuellaxCliente ?? false);
                        eConfig.bitSucActiva = Convert.ToBoolean(item.bitSucActiva ?? false);
                        eConfig.bitTiqueteClaseAsistido_alImprimir = Convert.ToBoolean(item.bitTiqueteClaseAsistido_alImprimir ?? false);
                        eConfig.bitTrabajarConDBEnOtroEquipo = Convert.ToBoolean(item.bitTrabajarConDBEnOtroEquipo ?? false);
                        eConfig.bitValidarPlanYReservaWeb = Convert.ToBoolean(item.bitValidarPlanYReservaWeb ?? false);
                        eConfig.bitValideContrato = Convert.ToBoolean(item.bitValideContrato ?? false);
                        eConfig.bitValideContratoPorFactura = Convert.ToBoolean(item.bitValideContratoPorFactura ?? false);
                        eConfig.blnEntradaCumpleConPlan = Convert.ToBoolean(item.blnEntradaCumpleConPlan ?? false);
                        eConfig.blnEntradaCumpleSinPlan = Convert.ToBoolean(item.blnEntradaCumpleSinPlan ?? false);
                        eConfig.blnLimpiarDescripcionAdicionales = Convert.ToBoolean(item.blnLimpiarDescripcionAdicionales ?? false);
                        eConfig.blnPermiteIngresosAdicionales = Convert.ToBoolean(item.blnPermiteIngresosAdicionales ?? false);
                        eConfig.blnPermitirIngresoPantalla = Convert.ToBoolean(item.blnPermitirIngresoPantalla ?? false);
                        eConfig.intCalidadHuella = Convert.ToInt32(item.intCalidadHuella ?? 0);
                        eConfig.intComplices_Plan_CortxIngs = Convert.ToInt32(item.intComplices_Plan_CortxIngs ?? 0);
                        eConfig.intDiasEntreCitaNoRiesgo = Convert.ToInt32(item.intDiasEntreCitaNoRiesgo ?? 0);
                        eConfig.intDiasEntreCitaRiesgo = Convert.ToInt32(item.intDiasEntreCitaRiesgo ?? 0);
                        eConfig.intDiasGraciaClientes = Convert.ToInt32(item.intDiasGraciaClientes ?? 0);
                        eConfig.intdiassincita_bloqueoing = Convert.ToInt32(item.intdiassincita_bloqueoing ?? 0);
                        eConfig.intentradas_sincita_bloqueoing = Convert.ToInt32(item.intentradas_sincita_bloqueoing ?? 0);
                        eConfig.intfkSucursal = item.intfkSucursal;
                        eConfig.intMinutosAntesReserva = Convert.ToInt32(item.intMinutosAntesReserva ?? 0);
                        eConfig.intMinutosDescontarTiquetes = Convert.ToInt32(item.intMinutosDescontarTiquetes ?? 0);
                        eConfig.intMinutosDespuesReserva = Convert.ToInt32(item.intMinutosDespuesReserva ?? 0);
                        eConfig.intMinutosNoReingreso = Convert.ToInt32(item.intMinutosNoReingreso ?? 0);
                        eConfig.intMinutosNoReingresoDia = Convert.ToInt32(item.intMinutosNoReingresoDia ?? 0);
                        eConfig.intNivelSeguirdadLectorUSB = Convert.ToInt32(item.intNivelSeguirdadLectorUSB ?? 0);
                        eConfig.intNumeroDiasActualizacionHuella = Convert.ToInt32(item.intNumeroDiasActualizacionHuella ?? 0);
                        eConfig.intPasoFinalAutoregistroWeb = Convert.ToInt32(item.intPasoFinalAutoregistroWeb ?? 0);
                        eConfig.intRangoHoraPrint = Convert.ToInt32(item.intRangoHoraPrint ?? 0);
                        eConfig.intTiempoActualizaIngresos = Convert.ToInt32(item.intTiempoActualizaIngresos ?? 0);
                        eConfig.intTiempoDuermeHiloEnviaComandoSalida = Convert.ToInt32(item.intTiempoDuermeHiloEnviaComandoSalida ?? 0);
                        eConfig.intTiempoEspaciadoTramasReplicaImgsTCAM7000 = Convert.ToInt32(item.intTiempoEspaciadoTramasReplicaImgsTCAM7000 ?? 0);
                        eConfig.intTiempoMaximoEnvioImgsTCAM7000 = Convert.ToInt32(item.intTiempoMaximoEnvioImgsTCAM7000 ?? 0);
                        eConfig.intTiempoParaLimpiarPantalla = Convert.ToInt32(item.intTiempoParaLimpiarPantalla ?? 0);
                        eConfig.intTiempoPingContinuoTCAM7000 = Convert.ToInt32(item.intTiempoPingContinuoTCAM7000 ?? 0);
                        eConfig.intTiempoPulso = Convert.ToInt32(item.intTiempoPulso ?? 0);
                        eConfig.intTiempoReplicaHuellasTCAM7000 = Convert.ToInt32(item.intTiempoReplicaHuellasTCAM7000 ?? 0);
                        eConfig.intTiempoReplicaImgsTCAM7000 = Convert.ToInt32(item.intTiempoReplicaImgsTCAM7000 ?? 0);
                        eConfig.intTiempoValidaSiAbrePuerta = Convert.ToInt32(item.intTiempoValidaSiAbrePuerta ?? 0);
                        eConfig.intTimeOutLector = Convert.ToInt32(item.intTimeOutLector ?? 0);
                        eConfig.intTipoUbicacionTeclado = Convert.ToInt32(item.intTipoUbicacionTeclado ?? 0);
                        eConfig.intUbicacionTeclasTeclado = Convert.ToInt32(item.intUbicacionTeclasTeclado ?? 0);
                        eConfig.intVelocidadPuerto = Convert.ToInt32(item.intVelocidadPuerto ?? 0);
                        eConfig.strClave = item.strClave;
                        eConfig.strColorPpal = item.strColorPpal;
                        eConfig.strIdentificadorDos = item.strIdentificadorDos;
                        eConfig.strIdentificadorUno = item.strIdentificadorUno;
                        eConfig.strNivelSeguridad = item.strNivelSeguridad;
                        eConfig.strPuertoCom = item.strPuertoCom;
                        eConfig.strRutaArchivoGymsoftNet = item.strRutaArchivoGymsoftNet;
                        eConfig.strRutaGuiaMarcacion = item.strRutaGuiaMarcacion;
                        eConfig.strRutaNombreBanner = item.strRutaNombreBanner;
                        eConfig.strRutaNombreLogo = item.strRutaNombreLogo;
                        eConfig.strTextoMensajeCortxIngs = item.strTextoMensajeCortxIngs;
                        eConfig.strTextoMensajeCumpleanos = item.strTextoMensajeCumpleanos;
                        eConfig.strTipoIdenTributaria = item.strTipoIdenTributaria;
                        eConfig.strTipoIngreso = item.strTipoIngreso;
                        eConfig.strPuertoComSalida = item.strPuertoComSalida;
                        eConfig.bitValidarHuellaMarcacionSDKAPI = Convert.ToBoolean(item.bitValidarHuellaMarcacionSDKAPI ?? false);
                        eConfig.Validación_de_huella_de_marcación_con_SDK_del_webserver = Convert.ToBoolean(item.Validación_de_huella_de_marcación_con_SDK_del_webserver ?? false);
                        eConfig.intTimeoutValidarHuellaMarcacionSDKAPI = Convert.ToInt32(item.intTimeoutValidarHuellaMarcacionSDKAPI ?? 0);
                        eConfig.Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver = Convert.ToInt32(item.Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver ?? 0);
                        eConfig.bitValidarConfiguracionIngresoWeb = Convert.ToBoolean(item.bitValidarConfiguracionIngresoWeb ?? false);
                        eConfig.timeGetPendingActions = Convert.ToInt32(item.timeGetPendingActions ?? 0);
                        eConfig.timeRemoveFingerprints = Convert.ToInt32(item.timeRemoveFingerprints ?? 0);

                        eConfig.timeTerminalConnections = Convert.ToInt32(item.timeTerminalConnections ?? 0);
                        eConfig.intTiempoEsperaRespuestaReplicaHuellasTCAM7000 = Convert.ToInt32(item.intTiempoEsperaRespuestaReplicaHuellasTCAM7000 ?? 0);
                        eConfig.timeWaitResponseReplicateUsers = Convert.ToInt32(item.timeWaitResponseReplicateUsers ?? 0);
                        eConfig.timeWaitResponseDeleteFingerprint = Convert.ToInt32(item.timeWaitResponseDeleteFingerprint ?? 0);
                        eConfig.timeWaitResponseDeleteUser = Convert.ToInt32(item.timeWaitResponseDeleteUser ?? 0);
                        eConfig.timeResetDownloadEvents = Convert.ToInt32(item.timeResetDownloadEvents ?? 0);
                        eConfig.timeRemoveUsers = Convert.ToInt32(item.timeRemoveUsers ?? 0);
                        eConfig.timeDowndloadEvents = Convert.ToInt32(item.timeDowndloadEvents ?? 0);
                        eConfig.timeHourSync = Convert.ToInt32(item.timeHourSync ?? 0);
                        eConfig.quantityAppAccessControlSimultaneous = Convert.ToInt32(item.quantityAppAccessControlSimultaneous ?? 0);
                        eConfig.bitGenerarCodigoQRdeingresoparavalidarLocalmente = Convert.ToBoolean(item.bitGenerarCodigoQRdeingresoparavalidarLocalmente ?? false);
                        eConfig.bitLectorBiometricoSiempreEncendido = Convert.ToBoolean(item.bitLectorBiometricoSiempreEncendido);
                    }

                    if (generalConfig != null)
                    {
                        eConfig.timeGetWhiteList = Convert.ToInt32(generalConfig.intTimeGetWhiteList ?? 0);
                        eConfig.timeInsertEntries = Convert.ToInt32(generalConfig.intTimeInsertEntries ?? 0);
                        eConfig.timeGetConfiguration = Convert.ToInt32(generalConfig.intTimeGetConfiguration ?? 0);
                        eConfig.timeGetClientMessages = Convert.ToInt32(generalConfig.intTimeGetClientMessages ?? 0);
                        eConfig.timeGetTerminals = Convert.ToInt32(generalConfig.intTimeGetTerminal ?? 0);
                        eConfig.timeInsertWhiteListTCAM = Convert.ToInt32(generalConfig.intTimeInsertWhiteListOnTCAM ?? 0);
                        eConfig.allowWhiteListTCAM = Convert.ToBoolean(generalConfig.allowWhiteListOnTCAM ?? false);
                    }
                    else
                    {
                        eConfig.timeGetWhiteList = 0;
                        eConfig.timeInsertEntries = 0;
                        eConfig.timeGetConfiguration = 0;
                        eConfig.timeGetClientMessages = 0;
                        eConfig.timeGetTerminals = 0;
                        eConfig.timeInsertWhiteListTCAM = 0;
                        eConfig.allowWhiteListTCAM = false;
                    }

                    eConfig.SMTPServer = serverData.SMTPServer;
                    eConfig.user = serverData.user;
                    eConfig.password = serverData.password;
                    eConfig.port = serverData.port;

                    ListConfig.Add(eConfig);

                }
                return ListConfig;
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

        /// <summary>
        /// Método encargado de validar los campos obligatorios del cliente basados en la configuración de los datos adicionales del ingreso.
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        //public string ValidateEntryConfigurationData(int gymId, string branchId, string clientId)
        //{
        //    bool bitContrato = false, bitConsInfor = false, bitConsBio = false, bitDatosVirt = false;
        //    List<eConfiguration> config = new List<eConfiguration>();
        //    gim_detalle_contrato contract = new gim_detalle_contrato();
        //    ContractDAL contractDAL = new ContractDAL();
        //    config = GetAccessControlConfigurationSpecialEntity(gymId, branchId);

        //    if (config == null || config.intfkSucursal == 0)
        //    {
        //        return "No se encontró ninguna configuración del gimnasio registrada.";
        //    }
        //    else
        //    {
        //        bitContrato = config.bitValideContrato;
        //        bitConsInfor = config.bitConsentimientoInformado;
        //        bitConsBio = config.bitConsentimientoDatosBiometricos;
        //        bitDatosVirt = config.bitDatosVirtualesUsuario;
        //    }

        //    if (!string.IsNullOrEmpty(clientId))
        //    {
        //        if (bitContrato)
        //        {
        //            contract = contractDAL.GetClientContract(gymId, clientId, 1);

        //            if (contract == null)
        //            {
        //                return "No se encontró la firma de los contratos. No se le permitirá el ingreso hasta que no firme los contratos.";
        //            }
        //        }

        //        if (bitConsInfor)
        //        {
        //            contract = contractDAL.GetClientContract(gymId, clientId, 3);

        //            if (contract == null)
        //            {
        //                return "No se encontró la firma del Consentimiento Informado. No se le permitirá el ingreso hasta que no firme el contrato.";
        //            }
        //        }

        //        if (bitConsBio)
        //        {
        //            contract = contractDAL.GetClientContract(gymId, clientId, 5);

        //            if (contract == null)
        //            {
        //                return "No se encontró la firma del Consentimiento de datos biométricos. No se le permitirá el ingreso hasta que no firme el contrato.";
        //            }
        //        }

        //        if (bitDatosVirt)
        //        {
        //            string generalMessage = string.Empty;
        //            ClientBLL clientBLL = new ClientBLL();
        //            gim_clientes client = new gim_clientes();
        //            client = clientBLL.GetActiveClient(gymId, clientId);

        //            if (client == null || string.IsNullOrEmpty(client.cli_identifi))
        //            {
        //                return "No se encontraron los datos del cliente.";
        //            }
        //            else
        //            {
        //                //TIPO DE IDENTIFICACIÓN
        //                generalMessage += Convert.IsDBNull(client.cli_tipo_identifi) ? "Se debe configurar el tipo de identificación"
        //                                                                             : client.cli_tipo_identifi == 0 ? "Se debe configurar el tipo de identificación"
        //                                                                                                             : string.Empty;
        //                //NOMBRES
        //                generalMessage += string.IsNullOrEmpty(generalMessage) ? string.IsNullOrEmpty(client.cli_nombres) ? "Se debe configurar el nombre"
        //                                                                                                                  : string.Empty
        //                                                                       : string.IsNullOrEmpty(client.cli_nombres) ? ", el nombre"
        //                                                                                                                  : string.Empty;
        //                //PRIMER APELLIDO
        //                generalMessage += string.IsNullOrEmpty(generalMessage) ? string.IsNullOrEmpty(client.cli_primer_apellido) ? "Se debe configurar el primer apellido"
        //                                                                                                                          : string.Empty
        //                                                                       : string.IsNullOrEmpty(client.cli_primer_apellido) ? ", el primer apellido"
        //                                                                                                                          : string.Empty;
        //                //DIRECCIÓN
        //                generalMessage += string.IsNullOrEmpty(generalMessage) ? string.IsNullOrEmpty(client.cli_direccio) ? "Se debe configurar la dirección"
        //                                                                                                                   : string.Empty
        //                                                                       : string.IsNullOrEmpty(client.cli_direccio) ? ", la dirección"
        //                                                                                                                   : string.Empty;
        //                //TELÉFONO
        //                generalMessage += string.IsNullOrEmpty(generalMessage) ? string.IsNullOrEmpty(client.cli_telefono) ? "Se debe configurar el número de teléfono"
        //                                                                                                                   : string.Empty
        //                                                                       : string.IsNullOrEmpty(client.cli_telefono) ? ", el número de teléfono"
        //                                                                                                                   : string.Empty;
        //                //CELULAR
        //                generalMessage += string.IsNullOrEmpty(generalMessage) ? string.IsNullOrEmpty(client.cli_celular) ? "Se debe configurar el número de celular"
        //                                                                                                                  : string.Empty
        //                                                                       : string.IsNullOrEmpty(client.cli_celular) ? ", el número el número de celular"
        //                                                                                                                  : string.Empty;
        //                //EMAIL
        //                generalMessage += string.IsNullOrEmpty(generalMessage) ? string.IsNullOrEmpty(client.cli_mail) ? "Se debe configurar el correo electrónico"
        //                                                                                                               : string.Empty
        //                                                                       : string.IsNullOrEmpty(client.cli_mail) ? ", el número el correo electrónico"
        //                                                                                                               : string.Empty;
        //                //FECHA DE NACIMIENTO
        //                generalMessage += string.IsNullOrEmpty(generalMessage) ? Convert.IsDBNull(client.cli_fecha_nacimien) ? "Se debe configurar la fecha de nacimiento"
        //                                                                                                                     : string.Empty
        //                                                                       : Convert.IsDBNull(client.cli_fecha_nacimien) ? ", la fecha de nacimiento"
        //                                                                                                                     : string.Empty;
        //                //GÉNERO
        //                generalMessage += string.IsNullOrEmpty(generalMessage) ? Convert.IsDBNull(client.cli_sexo) ? "Se debe configurar el género"
        //                                                                                                           : client.cli_sexo == 0 ? "Se debe configurar el género"
        //                                                                                                                                  : string.Empty
        //                                                                       : Convert.IsDBNull(client.cli_sexo) ? ", el género"
        //                                                                                                           : client.cli_sexo == 0 ? ", el género"
        //                                                                                                                                  : string.Empty;
        //                //EPS
        //                generalMessage += string.IsNullOrEmpty(generalMessage) ? Convert.IsDBNull(client.cli_intepsc) ? "Se debe configurar la EPS"
        //                                                                                                              : client.cli_intepsc == 0 ? "Se debe configurar la EPS"
        //                                                                                                                                        : string.Empty
        //                                                                       : Convert.IsDBNull(client.cli_intepsc) ? ", la EPS"
        //                                                                                                              : client.cli_intepsc == 0 ? ", la EPS"
        //                                                                                                                                        : string.Empty;
        //                //CÓMO SUPO
        //                generalMessage += string.IsNullOrEmpty(generalMessage) ? Convert.IsDBNull(client.cli_cod_como_supo) ? "Se debe configurar el campo cómo supo"
        //                                                                                                                    : client.cli_cod_como_supo == 0 ? "Se debe configurar el campo cómo supo"
        //                                                                                                                                                    : string.Empty
        //                                                                       : Convert.IsDBNull(client.cli_cod_como_supo) ? ", el campo cómo supo"
        //                                                                                                                    : client.cli_cod_como_supo == 0 ? ", el campo cómo supo"
        //                                                                                                                                                    : string.Empty;
        //                //NOMBRE DE CONTACTO
        //                generalMessage += string.IsNullOrEmpty(generalMessage) ? string.IsNullOrEmpty(client.cli_nombre_Allamar) ? "Se debe configurar en caso de emergencia el nombre del contacto"
        //                                                                                                                         : string.Empty
        //                                                                       : string.IsNullOrEmpty(client.cli_nombre_Allamar) ? ", en caso de emergencia el nombre del contacto"
        //                                                                                                                         : string.Empty;
        //                //TELÉFONO DE EMERGENCIA
        //                generalMessage += string.IsNullOrEmpty(generalMessage) ? string.IsNullOrEmpty(client.cli_telefono_emergencia) ? "Se debe configurar en caso de emergencia el teléfono del contacto"
        //                                                                                                                              : string.Empty
        //                                                                       : string.IsNullOrEmpty(client.cli_telefono_emergencia) ? ", en caso de emergencia el teléfono del contacto"
        //                                                                                                                              : string.Empty;
        //                //RH
        //                generalMessage += string.IsNullOrEmpty(generalMessage) ? string.IsNullOrEmpty(client.cli_rh) ? "Se debe configurar en caso de emergencia el RH"
        //                                                                                                                  : string.Empty
        //                                                                       : string.IsNullOrEmpty(client.cli_rh) ? ", en caso de emergencia el RH"
        //                                                                                                                  : string.Empty;

        //                if (!string.IsNullOrEmpty(generalMessage))
        //                {
        //                    return generalMessage + ". Por lo tanto, no se le permitirá el ingreso hasta que no termine de configurar los datos del cliente.";
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return ("No se encontró la identificación del cliente.");
        //    }

        //    return "OK";
        //}
    }
}
