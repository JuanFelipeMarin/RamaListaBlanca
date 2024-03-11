﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.DAL.Classes;

namespace Sibo.WhiteList.BLL.Classes
{
  public  class ZonasBLL
    {
        public List<tbl_Maestro_Zonas> GetZonas(int gymId, string branchId)
        {
            ZonasDAL termDAL = new ZonasDAL();

           return termDAL.GetZonas(gymId, branchId);
            
        }

    }
}
