using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sibo.WhiteList.Classes;
using Sibo.WhiteList.BLL.Classes;

namespace Sibo.WhiteList.Api.Classes
{
    /// <summary>
    /// Clase para validar si una huella existe en la base de datos.
    /// SE REALIZA EN ESTA CAPA DEBIDO A QUE LA LIBRERÍA DE SUPREMA "MATCHER" SE DEBE REFERENCIAR EN EL PROYECTO PRINCIPAL (DONDE SE ENCUENTRA EL DEBUG);
    /// ESTO PARA QUE FUNCIONE DE FORMA CORRECTA.
    /// DURANTE EL DESARROLLO SE INTENTÓ REFERENCIAR EN LA CAPA BLL PERO POR LO MENCIONADO NO FUE POSIBLE QUE FUNCIONARA DE FORMA CORRECTA.
    /// Getulio Vargas - 2018-08-23 - OD 1307
    /// </summary>
    public class SupremaValidations
    {
        byte[][] dbTemplate = null;
        int[] templateSize = null;

        public bool ExistFingerprint(List<gim_huellas> fingerprintList, byte[] fingerPrint)
        {
            try
            {
                bool resp = false;
                int match = 0;
                //Se anula momentaneamente la consulta de las huellas en la BD hasta que se encuentre la forma de validar la existencia de una huella sin que el lector esté conectado.

                //GetTemplates(fingerprintList);
                //Suprema.UFMatcher comparator = new Suprema.UFMatcher();
                //Suprema.UFM_STATUS ufm_response = new Suprema.UFM_STATUS();
                
                //if (dbTemplate != null && dbTemplate.Length > 0 && templateSize != null && templateSize.Length > 0)
                //{
                //    ufm_response = comparator.Identify(fingerPrint, 1024, dbTemplate, templateSize, dbTemplate.Length, 5000, out match);
                //}
                
                //if (ufm_response == Suprema.UFM_STATUS.OK)
                //{
                //    if (match != -1)
                //    {
                //        resp = true;
                //    }
                //    else
                //    {
                //        resp = false;
                //    }
                //}

                return resp;
            }   
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GetTemplates(List<gim_huellas> fingerprintList)
        {
            try
            {
                dbTemplate = new byte[fingerprintList.Count][];
                byte[] tmpFinger;
                int i = 0;
                templateSize = new int[fingerprintList.Count];

                if (fingerprintList != null && fingerprintList.Count > 0)
                {
                    foreach (gim_huellas item in fingerprintList)
                    {
                        tmpFinger = (byte[])item.hue_dato;
                        dbTemplate[i] = tmpFinger;
                        templateSize[i] = tmpFinger.Length;
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Método para validar si una huella que se va a grabar existe o no en la BD para un gimnasio específico.
        /// GEtulio Vargas - 2018-08-23 - OD 1307
        /// </summary>
        /// <param name="fingerEntity"></param>
        /// <returns></returns>
        public eResponse ValidateAndSaveFingerprint(eFingerprint fingerEntity)
        {
            try
            {
                FingerprintBLL fingerBll = new FingerprintBLL();
                eResponse response = new eResponse();
                SupremaValidations suprema = new SupremaValidations();
                List<gim_huellas> fingerprintList = new List<gim_huellas>();
                fingerBll.ValidateFields(fingerEntity);
                fingerprintList = fingerBll.GetFingerprintsByGym(fingerEntity.gymId);

                if (fingerprintList != null && fingerprintList.Count > 0)
                {
                    if (suprema.ExistFingerprint(fingerprintList, fingerEntity.fingerPrint))
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

                if (response.state)
                {
                    fingerBll.Process(fingerEntity);
                }

                return response;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

      
    }
}