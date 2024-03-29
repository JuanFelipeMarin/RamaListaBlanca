﻿using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Classes;
using System.Data;

namespace Sibo.WhiteList.BLL.Classes
{
    public class FingerprintBLL
    {
        /// <summary>
        /// Método encargado de consultar la huella de un cliente en un gimnasio específico.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public gim_huellas GetFingerprint(int gymId, string clientId)
        {
            try
            {
                Exception ex;
                FingerprintDAL fingerprintDAL = new FingerprintDAL();

                if (gymId == 0 || string.IsNullOrEmpty(gymId.ToString()))
                {
                    ex = new Exception(Exceptions.nullGym);
                    throw ex;
                }

                if (string.IsNullOrEmpty(clientId.ToString()))
                {
                    ex = new Exception(Exceptions.nullId);
                    throw ex;
                }

                return fingerprintDAL.GetFingerprint(gymId, clientId);
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
        /// Método que se encarga de consultar las huellas asociadas a un cliente
        /// Getulio Vargas - 2019-04-29
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<eFingerprint> GetFingerprintsByClient(int gymId, string clientId)
        {
            try
            {
                Exception ex;
                FingerprintDAL fingerprintDAL = new FingerprintDAL();

                if (gymId == 0 || string.IsNullOrEmpty(gymId.ToString()))
                {
                    ex = new Exception(Exceptions.nullGym);
                    throw ex;
                }

                if (string.IsNullOrEmpty(clientId.ToString()))
                {
                    ex = new Exception(Exceptions.nullId);
                    throw ex;
                }

                return fingerprintDAL.GetFingerprintsByClient(gymId, clientId);
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
        /// Método encargado de consultar las huellas asociadas a los 3 últimos dígitos ingresados
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="clientIdPart"></param>
        /// <returns></returns>
        public List<eFingerprint> GetFingerPrintsByClientIdPart(int gymId, string clientIdPart)
        {
            try
            {
                Exception ex;
                FingerprintDAL fingerprintDAL = new FingerprintDAL();

                if (gymId == 0 || string.IsNullOrEmpty(gymId.ToString()))
                {
                    ex = new Exception(Exceptions.nullGym);
                    throw ex;
                }

                if (string.IsNullOrEmpty(clientIdPart.ToString()))
                {
                    ex = new Exception(Exceptions.nullId);
                    throw ex;
                }

                return fingerprintDAL.GetFingerPrintsByClientIdPart(gymId, clientIdPart);
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
        /// Método encargado de invocar la validación para el registro de huella de un usuario.
        /// Getulio Vargas - 2018-08-17 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public eResponse ValidatePersonToSaveFingerprint(int gymId, int branchId, string id)
        {
            try
            {
                return ValidatePerson(gymId, branchId, id);
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
        /// Método encargado de validar si a un usuario se le puede grabar huella en el sistema.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private eResponse ValidatePerson(int gymId, int branchId, string id)
        {
            eResponse response = new eResponse();
            gim_configuracion_ingreso config = new gim_configuracion_ingreso();
            gim_planes_usuario invoice = new gim_planes_usuario();
            gim_planes_usuario_especiales courtesy = new gim_planes_usuario_especiales();
            gim_clientes client = new gim_clientes();
            Visitors visitor = new Visitors();
            VisitorBLL visitorBll = new VisitorBLL();
            gim_clientes_especiales specialClient = new gim_clientes_especiales();
            SpecialClientBLL scBll = new SpecialClientBLL();
            gim_empleados employee = new gim_empleados();
            AccessControlSettingsBLL acs = new AccessControlSettingsBLL();
            ClientBLL clientBLL = new ClientBLL();
            EmployeeBLL employeeBLL = new EmployeeBLL();
            BlackListBLL blackListBLL = new BlackListBLL();
            InvoiceBLL invoiceBLL = new InvoiceBLL();
            CourtesyBLL courtesyBLL = new CourtesyBLL();
            DAL.WhiteList wlEntity = new DAL.WhiteList();
            WhiteListBLL wlBll = new WhiteListBLL();
            bool validateContract = false;

            if (blackListBLL.ValidateActiveRecord(gymId, id))
            {
                response.state = false;
                response.message = "BlackList";
                return response;
            }
            else
            {
                config = acs.GetAccessControlConfiguration(gymId, branchId.ToString());
                employee = employeeBLL.GetActiveEmployee(gymId, id);

                if (employee != null)
                {
                    response.state = true;
                    response.message = "Ok";
                    response.messageOne = ((employee.emp_nombre == null) ? "" : employee.emp_nombre) + " " + ((employee.emp_primer_apellido == null) ? "" : employee.emp_primer_apellido) + " " +
                                                 ((employee.emp_segundo_apellido == null) ? "" : employee.emp_segundo_apellido);
                    return GetEntityWithFingerPrintId(response, gymId, id, branchId);
                }
                else
                {
                    visitor = visitorBll.GetVisitorByVisitorId(gymId, id);
                    wlEntity = wlBll.GetUserById(gymId, id, "Visitante");


                    if (visitor != null && wlEntity != null)
                    {
                        response.state = true;
                        response.message = "Ok";
                        response.messageOne = visitor.vis_strName + " " + visitor.vis_strFirstLastName + " " + ((visitor.vis_strSecondLastName == null) ? "" : visitor.vis_strSecondLastName);
                        return GetEntityWithFingerPrintId(response, gymId, id, branchId);
                    }
                    else
                    {
                        specialClient = scBll.GetSpecialClient(gymId, id);
                        wlEntity = wlBll.GetUserById(gymId, id, "Prospecto");

                        if (specialClient != null && wlEntity != null)
                        {
                            response.state = true;
                            response.message = "Ok";
                            response.messageOne = ((specialClient.cli_nombres == null) ? "" : specialClient.cli_nombres) + " " +
                                                  ((specialClient.cli_primer_apellido == null) ? "" : specialClient.cli_primer_apellido) +
                                                  ((specialClient.cli_segundo_apellido == null) ? "" : specialClient.cli_segundo_apellido);
                            return GetEntityWithFingerPrintId(response, gymId, id, branchId);
                        }
                        else
                        {
                            client = clientBLL.GetActiveClient(gymId, id);

                            if (client != null)
                            {
                                if (config != null)
                                {
                                    //validateContract = (config.bitValideContrato == null) ? false : Convert.ToBoolean(config.bitValideContrato);
                                    validateContract = false;//(config.bitValideContrato == null) ? false : Convert.ToBoolean(config.bitValideContrato);

                                    if (validateContract)
                                    {
                                        invoice = invoiceBLL.GetVigentInvoice(gymId, id);

                                        if (invoice != null)
                                        {
                                            response.state = true;
                                            response.message = "Ok";
                                            response.messageOne = ((client.cli_nombres == null) ? "" : client.cli_nombres) + " " + ((client.cli_primer_apellido == null) ? "" : client.cli_primer_apellido) + " " +
                                                                         ((client.cli_segundo_apellido == null) ? "" : client.cli_segundo_apellido);
                                            return GetEntityWithFingerPrintId(response, gymId, id, branchId);
                                        }
                                        else
                                        {
                                            courtesy = courtesyBLL.GetVigentCourtesy(gymId, id);

                                            if (courtesy != null)
                                            {
                                                response.state = true;
                                                response.message = "Ok";
                                                response.messageOne = ((client.cli_nombres == null) ? "" : client.cli_nombres) + " " + ((client.cli_primer_apellido == null) ? "" : client.cli_primer_apellido) + " " +
                                                                             ((client.cli_segundo_apellido == null) ? "" : client.cli_segundo_apellido);
                                                return GetEntityWithFingerPrintId(response, gymId, id, branchId);
                                            }
                                            else
                                            {
                                                response.state = false;
                                                response.message = "NoVigentPlan";
                                                return response;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        response.state = true;
                                        response.message = "Ok";
                                        response.messageOne = ((client.cli_nombres == null) ? "" : client.cli_nombres) + " " + ((client.cli_primer_apellido == null) ? "" : client.cli_primer_apellido) + " " +
                                                                     ((client.cli_segundo_apellido == null) ? "" : client.cli_segundo_apellido);
                                        return GetEntityWithFingerPrintId(response, gymId, id, branchId);
                                    }
                                }
                                else
                                {
                                    response.state = false;
                                    response.message = "NoConfiguration";
                                    return response;
                                }
                            }
                            else
                            {
                                response.state = false;
                                response.message = "NoActiveClient";
                                return response;
                            }
                        }
                    }
                }
            }
        }
        
        public void Process(eFingerprint fingerEntity)
        {
            WhiteListDAL wlDAL = new WhiteListDAL();
            gim_huellas fingerEntityTable = new gim_huellas();
            FingerprintDAL fingerDAL = new FingerprintDAL();
            fingerEntityTable = fingerDAL.GetFingerprint(fingerEntity.gymId, fingerEntity.personId, fingerEntity.id);

            if (fingerEntityTable != null)
            {
                fingerDAL.Update(fingerEntityTable, fingerEntity);
            }
            else
            {
                Insert(fingerEntity);
            }

            wlDAL.UpdateFingerprint(fingerEntity.gymId, fingerEntity.personId, fingerEntity.id, fingerEntity.fingerPrint);
        }

        public List<gim_huellas> GetFingerprintsByGym(int gymId)
        {
            FingerprintDAL fingerDAL = new FingerprintDAL();
            return fingerDAL.GetFingerprintsByGym(gymId);
        }
        public DataTable GetAllFingerprintsPerson(int gymId, string id )
        {
            FingerprintDAL fingerDAL = new FingerprintDAL();
            return fingerDAL.GetAllFingerprintsPerson(gymId, id);
        }

        /// <summary>
        /// Método que se encarga de consultar el id de la huella de un cliente en un gimnasio específico.
        /// En caso de no encontrar la huella del cliente, esta será insertada sin "huella", hasta que se realice la actualización de esta.
        /// La actualización se realizaría luego de la inserción de la huella desde el ingreso.
        /// Getulio Vargas - 2018-08-17 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="id"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        private eResponse GetEntityWithFingerPrintId(eResponse response, int gymId, string id, int branchId)
        {
            string strFingerId = "0";
            int fingerId = 0;
            gim_huellas fingerprint = new gim_huellas();

            if (gymId > 0 && !string.IsNullOrEmpty(id))
            {
                fingerprint = GetFingerprint(gymId, id);

                if (fingerprint != null && fingerprint.hue_dato != null)
                {
                    strFingerId = fingerprint.hue_id.ToString();
                    response.messageTwo = strFingerId;
                    response.messageThree = "ExistFingerprint";
                }
                else
                {
                    if (fingerprint != null)
                    {
                        fingerId = fingerprint.hue_id;
                    }
                    else
                    {
                        fingerId = GetNextId(gymId);
                    }

                    if (fingerId == 0)
                    {
                        throw new Exception("No es posible grabar la huella.");
                    }

                    eFingerprint fingerEntity = new eFingerprint()
                    {
                        branchId = branchId.ToString(),
                        finger = 1,
                        gymId = gymId,
                        id = fingerId,
                        personId = id,
                        quality = 100
                    };

                    if (fingerprint == null)
                    {
                        Insert(fingerEntity);
                    }
                    
                    response.messageTwo = fingerId.ToString();
                    response.messageThree = "NoExistFingerprint";
                }
            }

            return response;
        }

        /// <summary>
        /// Método que realiza el puente para insertar una huella en GSW.
        /// Getulio Vargas - 2018-08-17 - OD 1307
        /// </summary>
        /// <param name="fingerEntity"></param>
        /// <returns></returns>
        private bool Insert(eFingerprint fingerEntity)
        {
            FingerprintDAL fingerprintDAL = new FingerprintDAL();
            return fingerprintDAL.Insert(fingerEntity);
        }

        /// <summary>
        /// Método encargado de consultar el siguiente id de la tabla huellas para un gimnasio específico.
        /// Getulio Vargas - 2018-08-17 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        private int GetNextId(int gymId)
        {
            FingerprintDAL fingerprintDAL = new FingerprintDAL();
            return fingerprintDAL.GetNextId(gymId);
        }

        public List<eFingerprint> AddUpdateFingerprint(List<eFingerprint> fingerprintList)
        {
            try
            {
                FingerprintDAL fingerprintDAL = new FingerprintDAL();
                Exception ex;

                if (fingerprintList != null && fingerprintList.Count > 0)
                {
                    return fingerprintDAL.AddUpdateFingerprint(fingerprintList);
                }
                else
                {
                    ex = new Exception(Exceptions.nullFingerprintList);
                    throw ex;
                }
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
        /// Método para validar los campos necesarios de la entidad de huella.
        /// Getulio Vargas - 2018-08-23 - OD 1307
        /// </summary>
        /// <param name="fingerEntity"></param>
        public void ValidateFields(eFingerprint fingerEntity)
        {
            if (fingerEntity == null || fingerEntity.fingerPrint == null)
            {
                throw new Exception("No fue enviada la huella para validar si existe.");
            }

            if (fingerEntity.gymId == 0)
            {
                throw new Exception("Se debe indicar el id del gimnasio.");
            }

            if (string.IsNullOrEmpty(fingerEntity.personId))
            {
                throw new Exception("Se debe indicar la identificación de la persona.");
            }
        }

        /// <summary>
        /// Método encargado de guardar la firma del contrato de una cliente en un gimnasio específico.
        /// Getulio Vargas - 2018-08-24 - OD 1307
        /// </summary>
        /// <param name="essc"></param>
        /// <returns></returns>
        public eSendClientContract SaveSignedContract(eSaveSignedContract essc)
        {
            ContractDAL contractDAL = new ContractDAL();
            InvoiceDAL invoiceDAL = new InvoiceDAL();
            DataTable dtPlanList = new DataTable();
            eSendClientContract response = new eSendClientContract();
            gim_contrato contract = new gim_contrato();
            gim_detalle_contrato detContrato = new gim_detalle_contrato();

            detContrato = contractDAL.GetClientContract2(essc.gymId, essc.userId, 1);
            contract = contractDAL.GetContract(essc.gymId);
            dtPlanList = invoiceDAL.GetVigentPlansByClient(essc.gymId, essc.userId);
            int contractId = 1;
            int resp = 0;

            if (dtPlanList != null && dtPlanList.Rows.Count > 0 && contract != null && contract.cont_codigo > 0 && detContrato == null)
            {
                int planType = dtPlanList.Rows[0]["saleType"].ToString() == "CORTESÍA" ? 1 : 2;
                int invoiceId = Convert.ToInt32(dtPlanList.Rows[0]["invoiceId"].ToString());
                int dianId = Convert.ToInt32(dtPlanList.Rows[0]["dianId"].ToString());
                resp = contractDAL.Sign(contractId, essc.userId, planType, invoiceId, dianId, essc.gymId, essc.branchId, contract.cont_texto, essc.fingerprintImage);
            }

            if (resp > 0)
            {
                response = GetSendClientContract(essc.gymId, essc.branchId, essc.userId, resp, essc.fingerprintImage);
            }

            return response;
        }

        /// <summary>
        /// Método encargado de consultar los datos del contrato firmado por el cliente al momento de enrolar y retornarlos en una entidad.
        /// Getulio Vargas - 2018-08-24 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <param name="userId"></param>
        /// <param name="contractDetailId"></param>
        /// <returns></returns>
        private eSendClientContract GetSendClientContract(int gymId, int branchId, string userId, int contractDetailId, byte[] fingerprintImage)
        {
            eSendClientContract response = new eSendClientContract();
            ContractDAL contractDAL = new ContractDAL();
            DataTable dt = new DataTable();
            dt = contractDAL.GetSignedContractById(gymId, userId, contractDetailId);

            if (dt != null && dt.Rows.Count > 0)
            {
                response.contractId = contractDetailId;
                response.contractText = dt.Rows[0]["cont_texto"].ToString();
                response.contractType = dt.Rows[0]["tipo_contrato"].ToString();
                response.documentType = dt.Rows[0]["dtcont_tipo_plan"].ToString();
                response.invoiceId = Convert.ToInt32(dt.Rows[0]["dtcont_numero_plan"].ToString());

                if (!Convert.IsDBNull(dt.Rows[0]["cont_firma_responsable"]))
                {
                    response.responsibleSignature = (byte[])dt.Rows[0]["cont_firma_responsable"];
                }

                response.SignDate = Convert.ToDateTime(dt.Rows[0]["dtcont_fecha_firma"]);
                response.userEmail = dt.Rows[0]["cli_mail"].ToString();
                response.userId = userId;
                response.userName = dt.Rows[0]["Nombre"].ToString();
                response.gymNit = dt.Rows[0]["gim_nit"].ToString();
                response.gymName = dt.Rows[0]["gim_nombre_gimnasio"].ToString();
                response.gymAddress = dt.Rows[0]["gim_direccion"].ToString();
                response.gymPhone = dt.Rows[0]["gim_telefono"].ToString();
                response.userFingerprint = fingerprintImage;
                response.branchName = dt.Rows[0]["suc_strNombre"].ToString();

                if (!Convert.IsDBNull(dt.Rows[0]["gim_logo"]))
                {
                    response.gymLogo = (byte[])dt.Rows[0]["gim_logo"];
                }
            }

            return response;
        }
    }
}
