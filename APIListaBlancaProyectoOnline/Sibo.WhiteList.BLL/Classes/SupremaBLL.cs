using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Classes;
using Suprema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.BLL.Classes
{
    public class SupremaBLL
    {
        Byte[] HuellaBytes;
        int intTamanoImagen = 1024;
        bool resp = false;
        int calidadHuella = 0;
        int EnrollQuality = 0;
        int Intentos = 1;
        string temp_inBase64 = "";
        string mjs = "";
        byte[][] bdTemplate;
        int[] dbTemplateSize = { 0 };
        int index = -1;

        /// <summary>
        /// Método para validar si una huella que se va a grabar existe o no en la BD para un gimnasio específico.
        /// Se validan todas las huellas de la BD.
        /// GEtulio Vargas - 2018-08-23 - OD 1307
        /// </summary>
        /// <param name="fingerEntity"></param>
        /// <returns></returns>
        public eResponse ValidateAndSaveFingerprint(eFingerprint fingerEntity)
        {
            FingerprintBLL fingerBll = new FingerprintBLL();
            AccessControlSettingsBLL confBLL = new AccessControlSettingsBLL();
            eResponse response = new eResponse();
            List<gim_huellas> fingerprintList = new List<gim_huellas>();
            List<eConfiguration> conf = new List<eConfiguration>();
            fingerBll.ValidateFields(fingerEntity);

            ////consultar configuracion 
            conf = confBLL.GetAccessControlConfigurationSpecialEntity(fingerEntity.gymId, fingerEntity.branchId);

            ////VAlidar el tipo de fabricante para hacer la comparacion de huellas ya que esta libreria solo es para suprema pero los lector Sibo avance utilizan el mismo componente 
            if (fingerEntity.modoGrabacion == "suprema")
            {
                if (conf[0].Validación_de_huella_de_marcación_con_SDK_del_webserver == true)
                {
                    fingerprintList = fingerBll.GetFingerprintsByGym(fingerEntity.gymId);

                    if (fingerprintList != null && fingerprintList.Count > 0)
                    {
                        if (ExistFingerprint(fingerprintList, fingerEntity.fingerPrint))
                        {
                            response.state = false;
                            response.message = "ExistFingerprint";
                        }
                        else
                        {
                            response.state = true;
                            response.message = "Ok";
                        }
                    }
                    else
                    {
                        response.state = true;
                        response.message = "Ok";
                    }
                }
                else
                {
                    response.state = true;
                    response.message = "Ok";
                }
                
            }
            else
            {
                response.state = true;
                response.message = "Ok";

            }

            if (response.state)
            {
                fingerBll.Process(fingerEntity);
            }

            return response;
        }

        public UFM_STATUS validarHuella(byte[] a)
        {
            UFM_STATUS vartest;

            UFMatcher test = new UFMatcher();
            bool Verifysuccess = true;
            vartest = test.Verify(a, a.Length, a, a.Length, out Verifysuccess);
            return vartest;
        }

        public bool validarHuellaCliente(byte[] huella, byte[] HuellaBase)
        {
            bool Verifysuccess = true;

            UFMatcher test = new UFMatcher();
            UFM_STATUS vartest;
            vartest = test.Verify(huella, huella.Length, HuellaBase, HuellaBase.Length, out Verifysuccess);
            return Verifysuccess;
        }

        public string IniciarLector()
        {
            string mensaje = string.Empty;
            UFScanner usLectorUSB = new UFScanner();
            UFScannerManager usLectorManagenUSB = new UFScannerManager(null);

            if (!usLectorUSB.IsSensorOn)
            {
                usLectorManagenUSB.Init();
            }

            if (usLectorManagenUSB.Scanners.Count > 0)
            {
                usLectorUSB.SetScanner(0);
                bool resul = false;
                resul = LeerHuella();

                if (resul)
                {
                    mensaje = "Todo salió bn";
                }
                else
                {
                    mensaje = "Salió mal, no prendió";
                }
            }
            else
            {
                mensaje = "No se encontró el lector";
            }

            return mensaje;
        }

        public bool LeerHuella()
        {
            UFScanner usLectorUSB = new UFScanner();
            Suprema.UFS_STATUS Estado = new Suprema.UFS_STATUS();
            usLectorUSB.Timeout = 10000;
            usLectorUSB.Sensitivity = 7;
            Estado = usLectorUSB.ClearCaptureImageBuffer();
            Estado = usLectorUSB.CaptureSingleImage();

            if (Estado == Suprema.UFS_STATUS.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GetTemplates(List<gim_huellas> fingerprintList)
        {
            bdTemplate = new byte[fingerprintList.Count][];
            byte[] tmpFinger;
            int i = 0;
            dbTemplateSize = new int[fingerprintList.Count];

            if (fingerprintList != null && fingerprintList.Count > 0)
            {
                foreach (gim_huellas item in fingerprintList)
                {
                    tmpFinger = (byte[])item.hue_dato;
                    bdTemplate[i] = tmpFinger;
                    dbTemplateSize[i] = tmpFinger.Length;
                    i++;
                }
            }
        }

        /// <summary>
        /// Método que permite validar si una huella específica existe en una lista de huellas.
        /// Getulio Vargas OD 1307
        /// </summary>
        /// <param name="fingerprintList"></param>
        /// <param name="fingerPrint"></param>
        /// <returns></returns>
        public bool ExistFingerprint(List<gim_huellas> fingerprintList, byte[] fingerPrint)
        {
            try
            {
                bool resp = false;
                int match = 0;
                GetTemplates(fingerprintList);
                Suprema.UFMatcher comparator = new Suprema.UFMatcher();
                Suprema.UFM_STATUS ufm_response = new Suprema.UFM_STATUS();

                if (bdTemplate != null && bdTemplate.Length > 0 && dbTemplateSize != null && dbTemplateSize.Length > 0)
                {
                    ufm_response = comparator.Identify(fingerPrint, 1024, bdTemplate, dbTemplateSize, bdTemplate.Length, 5000, out match);
                }

                if (ufm_response == Suprema.UFM_STATUS.OK)
                {
                    if (match != -1)
                    {
                        resp = true;
                    }
                    else
                    {
                        resp = false;
                    }
                }

                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método que permite validar si una huella existe o no en base de datos
        /// Getulio Vargas - 2019-04-29
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="strClientIdPart"></param>
        /// <param name="imageBytes"></param>
        /// <returns></returns>
        public string ValidateFingerprint(int gymId, string strClientIdPart, byte[] imageBytes)
        {
            string response = string.Empty;
            List<eFingerprint> fingerprintList = new List<eFingerprint>();
            fingerprintList = ObtenerHuellasClienteRegistro(gymId, strClientIdPart);

            if (fingerprintList == null || fingerprintList.Count <= 0)
            {
                response = string.Empty;
            }

            foreach (var item in fingerprintList)
            {
                if (validarHuellaCliente(imageBytes, item.fingerPrint))
                {
                    response = item.personId;
                }
            }

            return response;
        }

        private void ObtenerTemplate(DataTable dtHuellas)
        {
            bdTemplate = new byte[0][];
            byte[][] aux;
            dbTemplateSize = new int[0];

            if (dtHuellas != null && dtHuellas.Rows.Count > 0)
            {
                int i = 0;
                foreach (DataRow row in dtHuellas.Rows)
                {
                    if (bdTemplate == null)
                    {
                        bdTemplate = new byte[1][];
                        dbTemplateSize = new int[1];
                    }
                    else
                    {
                        aux = new byte[bdTemplate.Length][];
                        Array.Copy(bdTemplate, aux, bdTemplate.Length);
                        bdTemplate = new byte[bdTemplate.Length + 1][];
                        Array.Copy(aux, bdTemplate, aux.Length);
                        dbTemplateSize = new int[dbTemplateSize.Length + 1];
                    }

                    byte[] tmpHuella = (byte[])row["byteHuella"];
                    bdTemplate[i] = tmpHuella;
                    dbTemplateSize[i] = tmpHuella.Length;
                    i++;
                }
            }
        }

        public bool IdentificarSiHuellaExiste(byte[] huella, DataTable dtHuellas)
        {
            try
            {
                ObtenerTemplate(dtHuellas);
                UFMatcher test = new UFMatcher();
                UFM_STATUS vartest;
                byte[,] Template2Array;
                int[] Template2SizeArray = new int[intTamanoImagen + 1];

                int nMatchIndex = 0;
                Template2Array = new byte[intTamanoImagen + 1, 1];

                if (bdTemplate == null)
                    return false;
                else
                    vartest = test.Identify(huella, intTamanoImagen, bdTemplate, dbTemplateSize, bdTemplate.Length, 5000, out nMatchIndex);

                if (((vartest == Suprema.UFM_STATUS.OK)))
                {
                    if ((nMatchIndex != -1))
                        return true;
                    else
                        return false;
                }
                else
                {
                    throw new Exception(test.InitResult + " " + vartest.ToString());
                }
            }
            catch (Exception ex)
            {
                mjs = "ERROR: EN EXTRAER HUELLAS PARA COMPARAR SU EXITENCIA " + ex.Message;
                throw new Exception(mjs); //ex;
            }
        }

        public List<eFingerprint> ObtenerHuellasCliente(int gymId, string clientId)
        {
            List<byte[]> listaHuellas = new List<byte[]>();
            FingerprintBLL fingerBLL = new FingerprintBLL();
            List<eFingerprint> fingerList = new List<eFingerprint>();
            fingerList = fingerBLL.GetFingerprintsByClient(gymId, clientId);

            return fingerList;
        }
        
        public string IdentificarSiHuellaExisteIdentifi(byte[] huella, DataTable dtHuellas)
        {
            try
            {
                ObtenerTemplate(dtHuellas);

                Suprema.UFMatcher test = new Suprema.UFMatcher();
                Suprema.UFM_STATUS vartest;
                byte[,] Template2Array;
                int[] Template2SizeArray = new int[intTamanoImagen + 1];

                int nMatchIndex = 0;
                Template2Array = new byte[intTamanoImagen + 1, 1];
                if (bdTemplate == null)
                    return "";
                else
                    vartest = test.Identify(huella, intTamanoImagen, bdTemplate, dbTemplateSize, bdTemplate.Length, 5000, out nMatchIndex);

                if (((vartest == Suprema.UFM_STATUS.OK)))
                {
                    if ((nMatchIndex != -1))
                    {
                        string cc = dtHuellas.Rows[nMatchIndex][2].ToString();
                        return cc;
                    }
                    else
                    {
                        return "No se encontró huella registrada";
                    }
                }
                else
                {
                    throw new Exception(test.InitResult + " " + vartest.ToString());
                }
            }
            catch (Exception ex)
            {
                mjs = "ERROR: EN EXTRAER HUELLAS PARA COMPARAR SU EXITENCIA " + ex.Message;
                throw new Exception(mjs);
            }
        }

        public List<eFingerprint> ObtenerHuellasClienteRegistro(int gymId, string clientIdPart)
        {
            List<byte[]> listaHuellas = new List<byte[]>();
            FingerprintBLL objBll = new FingerprintBLL();
            List<eFingerprint> fingerList = new List<eFingerprint>();

            if (clientIdPart.Length <= 3)
            {
                fingerList = objBll.GetFingerPrintsByClientIdPart(gymId, clientIdPart);
            }
            else
            {
                fingerList = objBll.GetFingerprintsByClient(gymId, clientIdPart);
            }            

            return fingerList;
        }

        public eResponse firmarContratoGSW(eContratos contrato)
        {
            ContratoBLL fingerBll = new ContratoBLL();
            eResponse response = new eResponse();
            List<gim_huellas> fingerprintList = new List<gim_huellas>();
            //fingerBll.ValidateFieldsContrato(contrato);

            response.state = true;
            response.message = "Ok";

            if (response.state)
            {
                fingerBll.Process(contrato);
            }

            return response;
        }

        public List<eFingerprintWL> GetAllFingerprintsPerson(int gymId, string id)
        {
            try
            {
                FingerprintBLL fingerBll = new FingerprintBLL();
                DataTable dt = new DataTable();
                List<eFingerprintWL> fingerprintList = new List<eFingerprintWL>();
                dt = fingerBll.GetAllFingerprintsPerson(gymId, id);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        eFingerprintWL fingerprint = new eFingerprintWL()
                        {
                            fingerPrint = (byte[])(item["hue_dato"]),
                            id = Convert.ToInt32(item["hue_id"]),
                            gymId = Convert.ToInt32(item["cdgimnasio"].ToString()),
                            finger = Convert.ToInt32(item["hue_dedo"].ToString()),
                            quality = Convert.ToInt32(item["hue_calidad"].ToString()),
                            branchId = Convert.ToInt32(item["intfkSucursal"].ToString()),
                            personId = item["hue_identifi"].ToString(),
                        };

                        fingerprintList.Add(fingerprint);
                    }

                    return fingerprintList;
                }
                else
                {
                    return fingerprintList;
                }

            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }


    }
}
