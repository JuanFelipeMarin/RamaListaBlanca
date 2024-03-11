﻿using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using Sibo.WhiteList.DAL.Classes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.DAL;

namespace Sibo.WhiteList.BLL.Classes
{
    public class AditionalRestrictionsBLL
    {
        /// <summary>
        /// Toma la lista blanca antes de descargarla al servicio y le hace validaciones extras
        /// MToro
        /// </summary>
        /// <param name="responseList">Lista blanca</param>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public List<eWhiteList> ApplyRestrictionsAndFilter(List<eWhiteList> responseList, int gymId, string branchId)
        {
            List<eWhiteList> newResponseList = new List<eWhiteList>();
            AccessControlSettingsBLL accessControlSettingsBLL = new AccessControlSettingsBLL();
            AditionalRestrictionsDAL AditionalRestrictionsDAL = new AditionalRestrictionsDAL();
            gim_configuracion_ingreso configuration = accessControlSettingsBLL.GetAccessControlConfiguration(gymId, branchId);
            bool bitBloqueoCitaNoCumplidaMSW = configuration.bitBloqueoCitaNoCumplidaMSW ?? false;

            foreach (eWhiteList item in responseList)
            {
                bool canDownload = true;

                if (bitBloqueoCitaNoCumplidaMSW && canDownload)
                    canDownload = AditionalRestrictionsDAL.isHealthy(item.id, gymId);

                if (canDownload)
                    newResponseList.Add(item);
            }

            return newResponseList;
        }
    }
}