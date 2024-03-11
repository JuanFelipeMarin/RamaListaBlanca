using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.DAL;

namespace Sibo.WhiteList.DAL.Classes
{
    public class AccessControlSettingsDAL
    {
        /// <summary>
        /// Método que se encarga de consultar la configuración del ingreso de una sucursal específica.
        /// Getulio Vargas - 2018-03-21 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public List<gim_configuracion_ingreso> GetAccessControlConfiguration(int gymId, string branchId)
        {
            List<gim_configuracion_ingreso> list = new List<gim_configuracion_ingreso>();

            string[] ListabranchId = branchId.Split(',');

             using (var context = new dbWhiteListModelEntities(gymId))
                {
                int sucursal = 0;
                    for (int i = 0; i < ListabranchId.Length; i++)
                    {
                    sucursal = Convert.ToInt32(ListabranchId[i]);

                      list.Add(context.gim_configuracion_ingreso.FirstOrDefault(x => x.cdgimnasio == gymId && x.intfkSucursal == sucursal));
                    }
                }

            return list;
        }

        public List<tblConfiguracion_FirmaContratosAcceso> getContratosConfiguracion(int gymId)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                return context.tblConfiguracion_FirmaContratosAcceso.ToList();
            }
        }


        /// <summary>
        /// Método que se encarga de validar si existe o no la configuración del ingreso en una sucursal específica, para saber si se puede actualizar el parámetro reset o no.
        /// Getulio Vargas - 2018-04-04 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        //public bool UpdateConfigToResetLocalWhiteList(int gymId, string branchId)
        //{
        //    gim_configuracion_ingreso config = new gim_configuracion_ingreso();
        //    config = GetAccessControlConfiguration(gymId, branchId);

        //    if (config != null)
        //    {
        //        List<tblConfiguracion_FirmaContratosAcceso> lstConfigContratos = getContratosConfiguracion(gymId);

        //        if (lstConfigContratos != null)
        //        {
        //            config.bitConsentimientoInformado = lstConfigContratos.FirstOrDefault(x => x.intFkIdTipoContrato == 1).bitEstado;
        //            config.bitConsentimientoDatosBiometricos = lstConfigContratos.FirstOrDefault(x => x.intFkIdTipoContrato == 3).bitEstado;
        //            config.bitValideContrato = lstConfigContratos.FirstOrDefault(x => x.intFkIdTipoContrato == 5).bitEstado;
        //        }
        //        else
        //        {
        //            config.bitConsentimientoInformado = false;
        //            config.bitConsentimientoDatosBiometricos = false;
        //            config.bitValideContrato = false;
        //        }

        //        return Update(config);
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// Método que permite actualizar el parámetro para resetear la lista blanca local cuando esta se ha actualizado localmente.
        /// Getulio Vargas - 2018-04-04 - OD 1307
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private bool Update(gim_configuracion_ingreso config)
        {
            config.bitResetLocalWhiteList = false;

            using (var context = new dbWhiteListModelEntities(config.cdgimnasio))
            {
                context.Entry(config).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            return true;
        }
    }
}
