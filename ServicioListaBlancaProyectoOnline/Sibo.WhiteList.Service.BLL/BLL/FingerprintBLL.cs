using CrystalDecisions.CrystalReports.Engine;
using Sibo.WhiteList.Service.BLL.Helpers;
using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.BLL.Reports;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.Data;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Sibo.WhiteList.Service.BLL
{
    public class FingerprintBLL
    {
        /// <summary>
        /// Método para insertar huellas en la base de datos local.
        /// Getulio Vargas - 2018-08-17 - OD 1307
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ipAddress"></param>
        /// <param name="fingerprintId"></param>
        /// <param name="fingerprint"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public bool Insert(string userId, string ipAddress, int fingerprintId, byte[] fingerprint, int quality, string idhuellaActual = null)
        {
            dFingerprint fingerData = new dFingerprint();
            string msg = string.Empty;

            if (string.IsNullOrEmpty(userId))
            {
                msg = "Identificación de usuario";
            }

            if (fingerprintId <= 0)
            {
                if (string.IsNullOrEmpty(msg))
                {
                    msg = "Id de la huella";
                }
                else
                {
                    msg += " - Id de la huella";
                }
            }

            if (fingerprint == null || string.IsNullOrEmpty(fingerprint.ToString()))
            {
                if (string.IsNullOrEmpty(msg))
                {
                    msg = "Huella";
                }
                else
                {
                    msg += " - Huella";
                }
            }

            if (!string.IsNullOrEmpty(msg))
            {
                throw new Exception("No es posible grabar la huella, alguno de los datos necesarios no fue enviado correctamente. A continuación se muestran: " + msg);
            }
            else
            {
                eFingerprint fingerEntity = new eFingerprint();
                fingerEntity.finger = 1;
                fingerEntity.fingerPrint = fingerprint;
                fingerEntity.id = fingerprintId;
                fingerEntity.personId = userId;
                fingerEntity.quality = 100;
                if (idhuellaActual != null)
                {
                    fingerEntity.intIndiceHuellaActual = idhuellaActual;
                }

                return fingerData.Insert(fingerEntity);
            }
        }


        public bool InsertReplicaTarjetas (string id, string card, bool state, string ip)
        {
            dFingerprint fingerData = new dFingerprint();

            return fingerData.InsertReplicaTarjetas(id, card, state, ip);

        }

        /// <summary>
        /// Método encargado consultar los registros de huella actualizados en lista blanca y actualizarlos en GSW.
        /// Getulio Vargas - 2018-08-22 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        //public bool GetFingerprintsToUpdate(int gymId, int branchId)
        //{
        //    try
        //    {
        //        WhiteListBLL wlBll = new WhiteListBLL();
        //        List<eWhiteList> wlList = new List<eWhiteList>();
        //        List<eFingerprint> fingerList = new List<eFingerprint>();
        //        List<eFingerprint> fingerResponseList = new List<eFingerprint>();
        //        FingerPrintAPI fingerAPI = new FingerPrintAPI();
        //        log.WriteProcess("Consulta de los registros de la lista blanca local con huella actualizada.");
        //        wlList = wlBll.GetFingerprintsToUpdate();

        //        if (wlList != null && wlList.Count > 0)
        //        {
        //            foreach (eWhiteList item in wlList)
        //            {
        //                eFingerprint fingerEntity = new eFingerprint();
        //                fingerEntity.branchId = branchId;
        //                fingerEntity.finger = 1;
        //                fingerEntity.fingerPrint = item.fingerprint;
        //                fingerEntity.gymId = gymId;
        //                fingerEntity.id = Convert.ToInt32((item.fingerprintId == null) ? 0 : item.fingerprintId);
        //                fingerEntity.personId = item.id;
        //                fingerEntity.quality = 100;
        //                fingerList.Add(fingerEntity);
        //            }
        //        }

        //        if (fingerList != null && fingerList.Count > 0)
        //        {
        //            log.WriteProcess("Consumo de API para actualizar en GSW las huellas de los usuarios; se envían " + fingerList.Count.ToString() + " registros.");
        //            fingerResponseList = fingerAPI.AddFingerprint(fingerList);

        //            if (fingerResponseList != null && fingerResponseList.Count > 0)
        //            {
        //                log.WriteProcess("Se procede a actualizar en la lista blanca local los registros actualizados en GSW; se procesarán " + fingerResponseList.Count.ToString() + " registros.");

        //                foreach (eFingerprint fingerItem in fingerResponseList)
        //                {
        //                    wlBll.FingerprintUpdated(fingerItem.personId, fingerItem.id);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            log.WriteProcess("No se encontraron registros con huella actualizada en la lista blanca local.");
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
        //        log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
        //        return false;
        //    }
        //}              

        public eWhiteList DownloadFingerprint(int gymId, int branchId, string personId)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                FingerPrintAPI acAPI = new FingerPrintAPI();
                dWhiteList wlData = new dWhiteList();
                eWhiteList response = new eWhiteList();

                response = acAPI.DownloadFingerprint(gymId, branchId, personId);

                if (response != null)
                {
                    WhiteListBLL whiteListBLL = new WhiteListBLL();
                    DataTable dt = whiteListBLL.GetWhiteListByUserId(response.id);
                    bool save = false;

                    if (dt == null || dt.Rows.Count <= 0)
                    {
                        save = true;
                    }

                    if (save)
                    {
                        response.branchId = branchId;
                        wlData.Insert(response);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        public eResponse ValidatePerson(int gymId, int branchId, string personId)
        {
            ServiceLog log = new ServiceLog();
            eResponse response = new eResponse();

            try
            {
                FingerPrintAPI acAPI = new FingerPrintAPI();
                response = acAPI.ValidatePerson(gymId, branchId, personId);
                return response;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return response;
            }
        }

        /// <summary>
        /// Recibe un listado de huellas en un DataTable, para guardarlos en la BD local.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool InsertRecord(DataTable dt)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dFingerprint fingerprintData = new dFingerprint();
                int records = 0;
                log.WriteProcess("Inicia el proceso de registro de huellas a insertar o eliminar en la BD local.");
                records = fingerprintData.InsertTable(dt);

                if (records > 0)
                {
                    log.WriteProcess("Los registros de huellas se procesaron de forma correcta.");
                    return true;
                }
                else
                {
                    log.WriteProcess("Los registros de huella no se procesaron de forma correcta.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message);
                return false;
            }
        }

        public DataTable GetFingerprints(string id)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    dFingerprint fingerprintData = new dFingerprint();
                    return fingerprintData.GetFingerprintsByUser(id);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message);
                return null;
            }
        }

        public DataTable GetAllFingerprints(string id)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    dFingerprint fingerprintData = new dFingerprint();
                    return fingerprintData.GetAllFingerprints(id);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message);
                return null;
            }
        }


        public DataTable GetEliminarHuellas(string ip)
        {
            ServiceLog log = new ServiceLog();

            try
            {                
                    dFingerprint fingerprintData = new dFingerprint();
                    return fingerprintData.GetEliminarHuellas(ip);
                
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message);
                return null;
            }
        }


        public DataTable GetEliminarTarjetas(string ip)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dFingerprint fingerprintData = new dFingerprint();
                return fingerprintData.GetEliminarTarjetas(ip);

            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message);
                return null;
            }
        }


        public bool GetCabiarEstadoElimarHuellas(string id, string ip)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dFingerprint fingerprintData = new dFingerprint();
                return fingerprintData.GetCabiarEstadoElimarHuellas(id, ip);

            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message);
                return false;
            }
        }

        public bool GetCambiarEstadoElimarTarjetas(string id, string ip)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dFingerprint fingerprintData = new dFingerprint();
                return fingerprintData.GetCambiarEstadoElimarTarjetas(id, ip);

            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message);
                return false;
            }
        }
        public bool ValidateFingerprintToRecord(int gymId, int branchId, string clientId, int fingerId, byte[] fingerprintImage, int quality, bool isService, string strModoGrabacion)
        {
            try
            {
                FingerPrintAPI fingerAPI = new FingerPrintAPI();
                eResponse response = new eResponse();
                eFingerprint fingerEntity = new eFingerprint();
                ValidateData(gymId, clientId, fingerprintImage);
                fingerEntity.finger = 1;
                fingerEntity.fingerPrint = fingerprintImage;
                fingerEntity.gymId = gymId;
                fingerEntity.branchId = branchId;
                fingerEntity.id = fingerId;
                fingerEntity.personId = clientId;
                fingerEntity.quality = quality;
                fingerEntity.modoGrabacion = strModoGrabacion;
                response = fingerAPI.ValidateFingerprintToRecord(fingerEntity);

                if (response.state)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServiceLog serviceLog = new ServiceLog();
                serviceLog.WriteProcess("ERROR GUARDANDO LA HUELLA EN GSW.");
                serviceLog.WriteError(ex.Message);
                return true;
            }
        }

        public bool ValidateFingerprintToRecordPalma(int gymId, int branchId, string clientId, int fingerId, byte[] fingerprintImage, int quality, bool isService)
        {
            try
            {
                FingerPrintAPI fingerAPI = new FingerPrintAPI();
                eResponse response = new eResponse();
                eFingerprint fingerEntity = new eFingerprint();
                ValidateData(gymId, clientId, fingerprintImage);
                fingerEntity.finger = fingerId;
                fingerEntity.fingerPrint = fingerprintImage;
                fingerEntity.gymId = gymId;
                fingerEntity.branchId = branchId;
                fingerEntity.id = fingerId;
                fingerEntity.personId = clientId;
                fingerEntity.quality = quality;
                response = fingerAPI.ValidateFingerprintToRecord(fingerEntity);

                if (response.state)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServiceLog serviceLog = new ServiceLog();
                serviceLog.WriteProcess("ERROR GUARDANDO LA HUELLA EN GSW.");
                serviceLog.WriteError(ex.Message);
                return true;
            }
        }

        /// <summary>
        /// Método para validar los datos necesarios para enviar a la API y verificar si la huella existe o no en la BD.
        /// Getulio Vargas - 2018-08-23 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="clientId"></param>
        /// <param name="fingerprintImage"></param>
        private void ValidateData(int gymId, string clientId, byte[] fingerprintImage)
        {
            if (gymId <= 0)
            {
                throw new Exception("No se puede validar en la API si la huella se puede grabar. El id del gimnasio no se envió correctamente.");
            }

            if (string.IsNullOrEmpty(clientId))
            {
                throw new Exception("No se puede validar en la API si la huella se puede grabar. La identificación del cliente no se envió correctamente.");
            }

            if (fingerprintImage == null)
            {
                throw new Exception("No se puede validar en la API si la huella se puede grabar. No se envió la huella.");
            }
        }

        public void SaveSignedContract(int gymId, int branchId, string userId, byte[] fingerprintImage, bool generatePDFContractAndSend)
        {
            FingerPrintAPI fingerAPI = new FingerPrintAPI();
            eSaveSignedContract essc = new eSaveSignedContract();
            eSendClientContract response = new eSendClientContract();

            if (!string.IsNullOrEmpty(userId) && fingerprintImage != null)
            {
                essc.gymId = gymId;
                essc.branchId = branchId;
                essc.userId = userId;
                essc.fingerprintImage = fingerprintImage;
                response = fingerAPI.SaveSignedContract(essc);

                if (response != null)
                {
                    if (generatePDFContractAndSend)
                    {
                        SendSignedContract(response);
                    }
                }
            }
            else
            {
                throw new Exception("No fue posible guardar la firma del contrato debido a que alguno de los datos no se envió correctamente." +
                                    " Los datos necesarios son: Identificación de usuario e imagen de la huella.");
            }
        }

        private void SendSignedContract(eSendClientContract response)
        {
            ServiceLog log = new ServiceLog();
            string emailReceeptor = string.Empty;
            TextObject obj;
            string text = "Fecha desde: " + DateTime.Now.Date + " Fecha hasta: " + DateTime.Now.Date;
            string pathFiles = System.Windows.Forms.Application.StartupPath + @"" + "\\PDFContracts", title = "Contrato Individual";

            if (!Directory.Exists(pathFiles))
            {
                DirectoryInfo di = Directory.CreateDirectory(pathFiles);
            }
            
            using (DataSet ds = GetTablesToDataSet(response))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    RptContract objRpt = new RptContract();
                    obj = (TextObject)objRpt.Section5.ReportObjects["txtFecha"];
                    obj.Text = text;
                    objRpt.SetDataSource(ds);
                    string completePath = System.Windows.Forms.Application.StartupPath + @"\\PDFContracts\\" + response.userId + ".pdf";
                    objRpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, completePath);
                    SendMail sendMail = new SendMail();
                    bool responseSendMail = sendMail.sendContract(completePath, response.userEmail);

                    if (responseSendMail)
                    {
                        log.WriteProcess("Se envió correctamente el contrato del cliente: " + response.userId + " - " + response.userName);
                    }
                    else
                    {
                        log.WriteProcess("No fue posible enviar correctamente el contrato del cliente: " + response.userId + " - " + response.userName);
                    }
                }
            }
        }

        /// <summary>
        /// Método para crear el DataSet que se envía al reporte de contratos para generar el PDF.
        /// Getulio Vargas - 2018-08-27 - OD 1307
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private DataSet GetTablesToDataSet(eSendClientContract response)
        {
            DataSet ds = new DataSet();

            //Creamos el registro del gimnasio en el DataTable dtGym
            DataTable dtGym = new DataTable();
            dtGym.Columns.Add("gim_direccion", typeof(string));
            dtGym.Columns.Add("gim_nombre_gimnasio", typeof(string));
            dtGym.Columns.Add("gim_nit", typeof(string));
            dtGym.Columns.Add("gim_telefono", typeof(string));
            dtGym.Columns.Add("gim_logo", typeof(byte[]));
            dtGym.TableName = "gymData";
            DataRow row = dtGym.NewRow();
            row["gim_direccion"] = response.gymAddress;
            row["gim_nombre_gimnasio"] = response.gymName;
            row["gim_nit"] = response.gymNit;
            row["gim_telefono"] = response.gymPhone;
            row["gim_logo"] = response.gymLogo;
            dtGym.Rows.Add(row);

            //Agregamos el DataTable del gimnasio al DataSet
            ds.Tables.Add(dtGym);

            //Creamos el registro del contrato en el DataTable dtContract
            DataTable dtContract = new DataTable();
            dtContract.Columns.Add("Nombre", typeof(string));
            dtContract.Columns.Add("cli_mail", typeof(string));
            dtContract.Columns.Add("dtcont_numcontrato", typeof(int));
            dtContract.Columns.Add("dtcont_fecha_firma", typeof(DateTime));
            dtContract.Columns.Add("dtcont_numero_plan", typeof(int));
            dtContract.Columns.Add("dtcont_tipo_plan", typeof(string));
            dtContract.Columns.Add("dtcont_huella_cliente", typeof(byte[]));
            dtContract.Columns.Add("cont_texto", typeof(string));
            dtContract.Columns.Add("cont_firma_responsable", typeof(byte[]));
            dtContract.Columns.Add("tipo_contrato", typeof(string));
            dtContract.Columns.Add("suc_strNombre", typeof(string));
            dtContract.Columns.Add("dtcont_doc_cliente", typeof(string));
            dtContract.TableName = "spContracts";
            row = dtContract.NewRow();
            row["Nombre"] = response.userName;
            row["cli_mail"] = response.userEmail;
            row["dtcont_numcontrato"] = response.contractId;
            row["dtcont_fecha_firma"] = response.SignDate;
            row["dtcont_numero_plan"] = response.invoiceId;
            row["dtcont_tipo_plan"] = response.documentType;
            row["dtcont_huella_cliente"] = response.userFingerprint;
            row["cont_texto"] = response.contractText;
            row["cont_firma_responsable"] = response.responsibleSignature;
            row["tipo_contrato"] = response.contractType;
            row["suc_strNombre"] = response.branchName;
            row["dtcont_doc_cliente"] = response.userId;
            dtContract.Rows.Add(row);

            //Agregamos el DataTable de contratos al DataSet
            ds.Tables.Add(dtContract);

            return ds;
        }

        /// <summary>
        /// Método para consultar y retornar la lista de huellas que se deben eliminar de las terminales.
        /// Getulio Vargas - 2018-08-31 - OD 1307
        /// </summary>
        /// <returns></returns>
        public List<eFingerprint> GetFingerprintsToDelete(string ipAddress)
        {
            dFingerprint fingerData = new dFingerprint();
            return fingerData.GetFingerprintsToDelete(ipAddress);
        }

        public List<eFingerprint> GetFingerprintsToReplicate(string ipAddress)
        {
            dFingerprint fingerData = new dFingerprint();

            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new Exception("No es posible consultar las huellas por replicar a la terminal");
            }

            return fingerData.GetFingerprintsToReplicate(ipAddress);
        }

        public DataTable GetTarjetasReplica(string ipAddress)
        {
            dFingerprint fingerData = new dFingerprint();

            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new Exception("No es posible consultar las huellas por replicar a la terminal");
            }

            return fingerData.GetTarjetasReplica(ipAddress);
        }

        /// <summary>
        /// Método que permite actualizar el bit usado de una huella eliminada de las terminales.
        /// Getulio Vargas - 2018-08-31 - OD 1307
        /// </summary>
        /// <param name="fingerprintId"></param>
        /// <returns></returns>
        public void UpdateFingerprintDeleted(DataTable dtFingerprints)
        {
            dFingerprint fingerData = new dFingerprint();
            int fingerprintId = 0;

            if (dtFingerprints != null && dtFingerprints.Rows.Count > 0)
            {
                foreach (DataRow row in dtFingerprints.Rows)
                {
                    fingerprintId = Convert.ToInt32(row["fingerprintId"].ToString());

                    if (fingerprintId > 0)
                    {
                        fingerData.UpdateFingerprintDeleted(fingerprintId);
                    }
                }
            }
        }

        public string ValidateFingerprint(int gymId, string strData, byte[] imageBytes, int timeout)
        {
            ServiceLog log = new ServiceLog();

            try
            {

                eRequestFingerprint requestFingerprint = new eRequestFingerprint()
                {
                    gymId = gymId,
                    imageBytes = imageBytes,
                    strData = strData
                };
                FingerPrintAPI acAPI = new FingerPrintAPI();
                dWhiteList wlData = new dWhiteList();
                string response = string.Empty;
                response = acAPI.ValidateFingerprint(requestFingerprint, timeout);
                return response;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return string.Empty;
            }
        }

        public eFingerprint GetFingerprint(int fingerprintId)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dFingerprint fingerprintData = new dFingerprint();
                return fingerprintData.GetFingerprint(fingerprintId);
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message);
                return null;
            }
        }

        public bool UpdateUsedFingerprints(string fingerprints)
        {
            dFingerprint fingerData = new dFingerprint();
            bool response = false;

            if (!string.IsNullOrEmpty(fingerprints))
            {
                response = fingerData.UpdateUsedFingerprints(fingerprints);
            }

            return response;
        }

        public bool firmarContratoGSW(int gymId, int branchId, string clientId, int fingerId, byte[] fingerprintImage, int quality, bool isService)
        {
            try
            {
                FingerPrintAPI fingerAPI = new FingerPrintAPI();
                eResponse response = new eResponse();
                eSaveSignedContract contrato = new eSaveSignedContract();
                ValidateData(gymId, clientId, fingerprintImage);
                //contrato.finger = 1;
                contrato.fingerprintImage = fingerprintImage;
                contrato.gymId = gymId;
                contrato.branchId = branchId;
                contrato.userId = clientId;
                //contrato.id = fingerId;
                //contrato.personId = clientId;
                //contrato.quality = quality;
                response = fingerAPI.firmarContratoEnrolarGSW(contrato);

                if (response.state)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServiceLog serviceLog = new ServiceLog();
                serviceLog.WriteProcess("ERROR GUARDANDO LA HUELLA EN GSW.");
                serviceLog.WriteError(ex.Message);
                return true;
            }
        }

    }
}
