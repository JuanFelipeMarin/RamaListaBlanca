using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Classes;
using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.DAL;

namespace Sibo.WhiteList.BLL.Classes
{
    public class MasterDataBLL
    {
        /// <summary>
        /// Método encargado de consultar los tipos de identificación de un gimnasio específico y los retorna en una lista genérica.
        /// Getulio Vargas - 2018-08-29 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        public List<eGeneric> GetIdType(int gymId)
        {
            List<eGeneric> responseList = new List<eGeneric>();
            MasterDataDAL masterDAL = new MasterDataDAL();
            List<gim_tipo_identificacion> idTypeList = new List<gim_tipo_identificacion>();
            idTypeList = masterDAL.GetIdTypeByGym(gymId);

            if (idTypeList != null && idTypeList.Count > 0)
            {
                eGeneric gen = new eGeneric()
                {
                    key = "0",
                    value = "--Seleccione--"
                };

                responseList.Add(gen);

                foreach (gim_tipo_identificacion item in idTypeList)
                {
                    gen = new eGeneric()
                    {
                        key = item.tipoiden_codigo.ToString(),
                        value = item.tipoiden_descripc
                    };

                    responseList.Add(gen);
                }
            }

            return responseList;
        }

        /// <summary>
        /// Método encargado de consultar las EPS de un gimnasio específico y los retorna en una lista genérica.
        /// Getulio Vargas - 2018-08-29 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        public List<eGeneric> GetEPS(int gymId)
        {
            List<eGeneric> responseList = new List<eGeneric>();
            MasterDataDAL masterDAL = new MasterDataDAL();
            List<gim_eps> idTypeList = new List<gim_eps>();
            idTypeList = masterDAL.GetEPS(gymId);

            if (idTypeList != null && idTypeList.Count > 0)
            {
                eGeneric gen = new eGeneric();
                gen.key = "0";
                gen.value = "--Seleccione--";
                responseList.Add(gen);

                foreach (gim_eps item in idTypeList)
                {
                    gen = new eGeneric();
                    gen.key = item.eps_codigo.ToString();
                    gen.value = item.eps_nombre;
                    responseList.Add(gen);
                }
            }

            return responseList;
        }
    }
}
