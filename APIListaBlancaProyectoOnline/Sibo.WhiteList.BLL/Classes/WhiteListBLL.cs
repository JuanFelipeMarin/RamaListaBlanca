﻿using Newtonsoft.Json;
using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.Classes;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;



namespace Sibo.WhiteList.BLL.Classes
{
    public class WhiteListBLL
    {
        //se comenta codigo ya que no se utilizara lsita balnca local 
        //public List<eWhiteList> GetWhiteList(int gymId, string branchId)
        //{
        //    try
        //    {
        //        List<eWhiteList> responseList = new List<eWhiteList>();
        //        AditionalRestrictionsBLL aditionalRestrictionsBLL = new AditionalRestrictionsBLL();
        //        WhiteListDAL whiteListDAL = new WhiteListDAL();
        //        Validation val = new Validation();

        //        val.ValidateGym(gymId);
        //        val.ValidateBranch(branchId);

        //       // responseList = ConvertToEntity(whiteListDAL.GetWhiteList(gymId, branchId));
        //        responseList = aditionalRestrictionsBLL.ApplyRestrictionsAndFilter(responseList, gymId, branchId);

        //        return responseList;
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

        public string GetListPlanesSucursalPorPlan(int gymId, string branchId, int planId)
        {
            try
            {

                WhiteListDAL whiteListDAL = new WhiteListDAL();
                Validation val = new Validation();
                List<gim_planes_sucursal> responseList = new List<gim_planes_sucursal>();

                val.ValidateGym(gymId);
                val.ValidateBranch(branchId);
                responseList = whiteListDAL.GetListPlanesSucursalPorPlan(gymId, branchId, planId);

                if (responseList != null)
                {
                    return responseList[0].Zonas;
                }
                else
                {
                    return "";
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

        public string getZonasAcceso(int idEmpresa, string idSucursal, int idPlan, int idReserva)
        {
            try
            {
                Validation val = new Validation();
                val.ValidateGym(idEmpresa);
                val.ValidateBranch(idSucursal);

                WhiteListDAL whiteListDAL = new WhiteListDAL();
                return whiteListDAL.getZonasAcceso(idEmpresa, idSucursal, idPlan, idReserva);
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

        public bool GetActualizarCantidadTiquetes(int id, int invoiceId, int dianId, string documentType, int availableEntries, int gymId, string branchId , int planId)
        {
            try
            {
                Validation val = new Validation();
                val.ValidateGym(gymId);
                val.ValidateBranch(branchId);
                WhiteListDAL whiteListDAL = new WhiteListDAL();
                return  whiteListDAL.GetActualizarCantidadTiquetesAsync(id, invoiceId, dianId, documentType, availableEntries, gymId, branchId , planId);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    return false;
                }
                else
                {
                    throw new Exception(Exceptions.serverError);
                }
            }
        }
//se comenta codigo ya que no se utilizara lsita balnca local 
        //private List<eWhiteList> ConvertToEntity(List<spWhiteList_Result> list)
        //{
        //    try
        //    {
        //        List<eWhiteList> response = new List<eWhiteList>();
        //        ClientCardBLL clientCardBLL = new ClientCardBLL();

        //        if (list != null && list.Count > 0)
        //        {
        //            foreach (spWhiteList_Result item in list)
        //            {
        //                eWhiteList whiteList = new eWhiteList()
        //                {
        //                    availableEntries = item.availableEntries,
        //                    branchId = item.branchId,
        //                    branchName = item.branchName,
        //                    cardId = item.cardId,
        //                    classIntensity = item.classIntensity,
        //                    className = item.className,
        //                    classSchedule = item.classSchedule,
        //                    classState = item.classState,
        //                    courtesy = item.courtesy,
        //                    dateClass = item.dateClass,
        //                    dianId = item.dianId,
        //                    documentType = item.documentType,
        //                    employeeName = item.employeeName,
        //                    expirationDate = item.expirationDate,
        //                    fingerprint = item.fingerprint,
        //                    fingerprintId = item.fingerprintId,
        //                    groupEntriesControl = item.groupEntriesControl,
        //                    groupEntriesQuantity = item.groupEntriesQuantity,
        //                    groupId = item.groupId,
        //                    gymId = item.gymId,
        //                    id = item.id,
        //                    intPkId = item.intPkId,
        //                    invoiceId = item.invoiceId,
        //                    isRestrictionClass = item.isRestrictionClass,
        //                    know = item.know,
        //                    lastEntry = item.lastEntry,
        //                    name = item.name,
        //                    personState = item.personState,
        //                    photoPath = item.photoPath,
        //                    planId = item.planId,
        //                    planName = item.planName,
        //                    planType = item.planType,
        //                    reserveId = item.reserveId,
        //                    restrictions = item.restrictions,
        //                    subgroupId = item.subgroupId,
        //                    typePerson = item.typePerson,
        //                    updateFingerprint = item.updateFingerprint,
        //                    utilizedMegas = item.utilizedMegas,
        //                    utilizedTickets = item.utilizedTickets,
        //                    withoutFingerprint = item.withoutFingerprint,
        //                    cantidadhuellas = Convert.ToInt32(item.cantidadhuellas ?? 0) 
        //                };

        //                if (item.tblCardId == "" || item.tblCardId == null)
        //                {
        //                    whiteList.tblCardId = null;
        //                }
        //                else
        //                {
        //                    List<eClientCard> tablaCard = JsonConvert.DeserializeObject<List<eClientCard>>(item.tblCardId);
        //                    whiteList.tblCardId = tablaCard;
        //                }
        //                if (item.cantidadhuellas >= 2 && item.withoutFingerprint == false)
        //                {
        //                    List<eClientesPalmas> tablaPalma = JsonConvert.DeserializeObject<List<eClientesPalmas>>(item.tblHuellaPalmas);
        //                    whiteList.tblPalmasId = tablaPalma;
        //                    whiteList.fingerprint = null;
        //                    whiteList.fingerprintId = 0;
        //                }
        //                else
        //                {
        //                    whiteList.tblPalmasId = null;
        //                }




        //                response.Add(whiteList);
        //            }
        //        }
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {


        //        return null;
        //    }
        //}


        public bool DownloadClient(int gymId, string branchId, string clientId)
        {
            try
            {
                WhiteListDAL whiteListDAL = new WhiteListDAL();
                gim_clientes client = new gim_clientes();
                gim_planes_usuario invoice = new gim_planes_usuario();
                gim_planes_usuario_especiales courtesy = new gim_planes_usuario_especiales();
                gim_sucursales branch = new gim_sucursales();
                ClientBLL clientBLL = new ClientBLL();
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                CourtesyBLL courtesyBLL = new CourtesyBLL();
                BranchPlanBLL branchPlanBLL = new BranchPlanBLL();
                BranchBLL branchBLL = new BranchBLL();
                int planId = 0, actualBranch = 0;

                client = clientBLL.GetActiveClient(gymId, clientId);

                if (client == null)
                {
                    throw new Exception(Exceptions.NotClient);
                }

                invoice = invoiceBLL.GetVigentInvoice(gymId, clientId);

                if (invoice == null)
                {
                    courtesy = courtesyBLL.GetVigentCourtesy(gymId, clientId);

                    if (courtesy == null)
                    {
                        throw new Exception(Exceptions.NotVigentPlan);
                    }
                }

                if (invoice != null)
                {
                    planId = (invoice.plusu_codigo_plan == null) ? 0 : Convert.ToInt32(invoice.plusu_codigo_plan);
                    actualBranch = invoice.plusu_sucursal;
                }
                else if (courtesy != null)
                {
                    planId = (courtesy.plusu_codigo_plan == null) ? 0 : Convert.ToInt32(courtesy.plusu_codigo_plan);
                    actualBranch = courtesy.plusu_sucursal;
                }

                if (planId == 0)
                {
                    throw new Exception(Exceptions.NotPlan);
                }

                if (!branchPlanBLL.ValidatePlanInBranch(gymId, branchId, planId))
                {
                    throw new Exception(Exceptions.NotEntry);
                }

                if (Convert.ToInt32(branchId) != Convert.ToInt32(actualBranch))
                {
                    branch = branchBLL.GetBranch(gymId, branchId);
                    return whiteListDAL.InsertRecordNewBranch(gymId, actualBranch, Convert.ToInt32(branchId), branch.suc_strNombre, planId, clientId);
                }
                else
                {
                    throw new Exception(Exceptions.existFingerprint);
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

        public DAL.WhiteList DownloadFingerprintToLocalWhiteList(int gymId, int branchId, string clientId)
        {
            try
            {
                WhiteListDAL whiteListDAL = new WhiteListDAL();
                gim_clientes client = new gim_clientes();
                gim_planes_usuario invoice = new gim_planes_usuario();
                gim_planes_usuario_especiales courtesy = new gim_planes_usuario_especiales();
                gim_sucursales branch = new gim_sucursales();
                ClientBLL clientBLL = new ClientBLL();
                InvoiceBLL invoiceBLL = new InvoiceBLL();
                CourtesyBLL courtesyBLL = new CourtesyBLL();
                BranchPlanBLL branchPlanBLL = new BranchPlanBLL();
                BranchBLL branchBLL = new BranchBLL();
                int planId = 0, actualBranch = 0;

                client = clientBLL.GetActiveClient(gymId, clientId);

                if (client == null)
                {
                    return null;
                    //throw new Exception(Exceptions.NotClient);
                }

                invoice = invoiceBLL.GetVigentInvoice(gymId, clientId);

                if (invoice == null)
                {
                    courtesy = courtesyBLL.GetVigentCourtesy(gymId, clientId);

                    if (courtesy == null)
                    {
                        return null;
                        //throw new Exception(Exceptions.NotVigentPlan);
                    }
                }

                if (invoice != null)
                {
                    planId = (invoice.plusu_codigo_plan == null) ? 0 : Convert.ToInt32(invoice.plusu_codigo_plan);
                    actualBranch = invoice.plusu_sucursal;
                }
                else if (courtesy != null)
                {
                    planId = (courtesy.plusu_codigo_plan == null) ? 0 : Convert.ToInt32(courtesy.plusu_codigo_plan);
                    actualBranch = courtesy.plusu_sucursal;
                }

                if (planId == 0)
                {
                    return null;
                    //throw new Exception(Exceptions.NotPlan);
                }

                if (!branchPlanBLL.ValidatePlanInBranch(gymId, branchId.ToString(), planId))
                {
                    return null;
                    //throw new Exception(Exceptions.NotEntry);
                }

                if (branchId != actualBranch)
                {
                    branch = branchBLL.GetBranch(gymId, branchId.ToString());
                    return whiteListDAL.InsertRecordNewBranchAndReturn(gymId, actualBranch, branchId, branch.suc_strNombre, planId, clientId);
                }
                else
                {
                    return null;
                    //throw new Exception(Exceptions.existFingerprint);
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
                    return null;
                    //throw new Exception(Exceptions.serverError);
                }
            }
        }

        public bool Update(DAL.WhiteList response)
        {
            WhiteListDAL wlDAL = new WhiteListDAL();
            return wlDAL.Update(response);
        }

        /// <summary>
        /// Método que permite consultar un registro en la lista blanca, de un gimnasio específico; partiendo de la identificación y el tipo de persona.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="userId"></param>
        /// <param name="personType"></param>
        /// <returns></returns>
        public DAL.WhiteList GetUserById(int gymId, string userId, string personType)
        {
            WhiteListDAL wlDAL = new WhiteListDAL();

            if (gymId <= 0)
            {
                throw new Exception(Exceptions.nullGym);
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception(Exceptions.nullId);
            }

            if (string.IsNullOrEmpty(personType))
            {
                throw new Exception("No es posible consultar el registro de la lista blanca, no se envió el tipo de persona.");
            }

            return wlDAL.GetUserById(gymId, userId, personType);
        }

        public bool UpdateWhiteListRecords(eMarcarListaBlanca entity)
        {
            try
            {
                WhiteListDAL whiteListDAL = new WhiteListDAL();

                if (entity.entities != null && entity.entities.Count > 0)
                {
                    return whiteListDAL.UpdateWhiteListRecords(entity);
                }
                else
                {
                    Exception ex;
                    ex = new Exception(Exceptions.nullStringList);

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

        public string GetListSucursalPorClase(int gymId, int branchId, int cdreserva)
        {
            try
            {
                WhiteListDAL whiteListDAL = new WhiteListDAL();

                return whiteListDAL.GetListSucursalPorClase(gymId, branchId, cdreserva);
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

        public List<eWhiteList> GetListPersonaConsultar(int gymId, string branchId, string id, bool option)
        {
            try
            {
                DataTable dt = new DataTable();
                WhiteListDAL whiteListDAL = new WhiteListDAL();
                List<eWhiteList> ewl = new List<eWhiteList>();
                dt = whiteListDAL.GetListPersonaConsultar(gymId, branchId, id, option);

                ewl = ConvertToEntityLista(dt);
                
                return ewl;
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

        private List<eWhiteList> ConvertToEntityLista(DataTable list)
        {
            try
            {
                byte[] imgNull = new byte[0];
                List<eWhiteList> response = new List<eWhiteList>();
                ClientCardBLL clientCardBLL = new ClientCardBLL();

                if (list != null && list.Rows.Count > 0)
                {
                    foreach (DataRow item in list.Rows)
                    {
                        eWhiteList whiteList = new eWhiteList()
                        {
                            availableEntries = Convert.ToInt32(item["availableEntries"].ToString()),
                            branchId = Convert.ToInt32(item["branchId"].ToString()) ,
                            branchName = item["branchName"].ToString(),
                            cardId = item["availableEntries"].ToString(),
                            classIntensity = item["classIntensity"].ToString(),
                            className = item["className"].ToString(),
                            classSchedule = item["classSchedule"].ToString(),
                            classState = item["classState"].ToString(),
                            courtesy = Convert.ToBoolean(item["courtesy"].ToString()),
                            dateClass = item["dateClass"].ToString() == "" ? new DateTime()  : Convert.ToDateTime(item["dateClass"].ToString()),
                            dianId = Convert.ToInt32(item["dianId"].ToString()),
                            documentType = item["documentType"].ToString(),
                            employeeName = item["employeeName"].ToString(),
                            expirationDate = item["expirationDate"].ToString() == "" ? new DateTime() : Convert.ToDateTime(item["expirationDate"].ToString()),
                            fingerprint = item["fingerprint"].ToString() == "" ? imgNull : (byte[])(item["fingerprint"]),
                            fingerprintId = Convert.ToInt32(item["fingerprintId"].ToString()),
                            groupEntriesControl = Convert.ToBoolean(item["groupEntriesControl"].ToString()),
                            groupEntriesQuantity = Convert.ToInt32(item["groupEntriesQuantity"].ToString()),
                            groupId = Convert.ToInt32(item["groupId"].ToString()),
                            gymId = Convert.ToInt32(item["gymId"].ToString()),
                            id = item["id"].ToString(),
                            intPkId = Convert.ToInt32(item["intPkId"].ToString()),
                            invoiceId = Convert.ToInt32(item["invoiceId"].ToString()),
                            isRestrictionClass = Convert.ToBoolean(item["isRestrictionClass"].ToString()),
                            know = Convert.ToBoolean(item["know"].ToString()),
                            lastEntry = item["lastEntry"].ToString() == "" ? new DateTime() : Convert.ToDateTime(item["lastEntry"].ToString()),
                            name = item["name"].ToString(),
                            personState = item["personState"].ToString(),
                            photoPath = item["photoPath"].ToString(),
                            planId = Convert.ToInt32(item["planId"].ToString()),
                            planName = item["planName"].ToString(),
                            planType = item["planType"].ToString(),
                            reserveId = Convert.ToInt32(item["reserveId"].ToString()),
                            restrictions = item["restrictions"].ToString(),
                            subgroupId = Convert.ToInt32(item["subgroupId"].ToString()),
                            typePerson = item["typePerson"].ToString(),
                            updateFingerprint = Convert.ToBoolean(item["updateFingerprint"].ToString()),
                            utilizedMegas = Convert.ToInt32(item["utilizedMegas"].ToString()),
                            utilizedTickets = Convert.ToInt32(item["utilizedTickets"].ToString()),
                            withoutFingerprint = Convert.ToBoolean(item["withoutFingerprint"].ToString()),
                            tipo = item["tipo"].ToString(),
                            //cantidadhuellas = Convert.ToInt32(item["cantidadhuellas"].ToString())

                        };

                        //if (item.tblCardId == "" || item.tblCardId == null)
                        //{
                        //    whiteList.tblCardId = null;
                        //}
                        //else
                        //{
                        //    List<eClientCard> tablaCard = JsonConvert.DeserializeObject<List<eClientCard>>(item.tblCardId);
                        //    whiteList.tblCardId = tablaCard;
                        //}
                        //if (item.cantidadhuellas >= 2 && item.withoutFingerprint == false)
                        //{
                        //    List<eClientesPalmas> tablaPalma = JsonConvert.DeserializeObject<List<eClientesPalmas>>(item.tblHuellaPalmas);
                        //    whiteList.tblPalmasId = tablaPalma;
                        //    whiteList.fingerprint = null;
                        //    whiteList.fingerprintId = 0;
                        //}
                        //else
                        //{
                        //    whiteList.tblPalmasId = null;
                        //}


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

        public string GetCantidadPersonasGrupoFamiliar(int gymId, string branchId, string idGrupo)
        {
            try
            {
                DataTable dt = new DataTable();
                WhiteListDAL whiteListDAL = new WhiteListDAL();
                string ewl = "";
                dt = whiteListDAL.GetListPersonaConsultar(gymId, branchId, idGrupo);
                ewl = dt.Rows[0]["idCantidadREgistrosGrupoFamiliar"].ToString();
                return ewl;
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

        public string GetConsultarEliminacionContratos(int gymId, string branchId, string id)
        {
            try
            {
                DataTable dt = new DataTable();
                WhiteListDAL whiteListDAL = new WhiteListDAL();
                string ewl = "";
                dt = whiteListDAL.GetConsultarEliminacionContratos(gymId, branchId, id);

                if (dt.Rows != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        ewl = dt.Rows[0]["id"].ToString();
                    }
                    else
                    {
                        ewl = "";
                    }
                }
               
                return ewl;
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

        public bool GetRespuestaIngresosVisitantes(int gymId, string id)
        {
            try
            {
                DataTable dt = new DataTable();
                WhiteListDAL whiteListDAL = new WhiteListDAL();
                bool ewl = false;
                dt = whiteListDAL.GetRespuestaIngresosVisitantes(gymId, id);

                if (dt.Rows != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        ewl = true;
                    }
                    else
                    {
                        ewl = false;
                    }
                }
                else
                {
                     ewl = false;
                } 

                return ewl;
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
    }
}