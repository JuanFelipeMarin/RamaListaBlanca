﻿using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class BranchPlanDAL
    {
        public bool ValidatePlanInBranch(int gymId, string branchId, int planId)
        {
            bool responseBool = false;
            gim_planes_sucursal_ingreso branchPlan = new gim_planes_sucursal_ingreso();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                branchPlan = context.gim_planes_sucursal_ingreso.FirstOrDefault(bp => bp.cdgimnasio == gymId && bp.sucing_intfkSucursalEntrar == Convert.ToInt32(branchId) &&
                                                                                bp.sucing_intfkCodigoPlan == planId);
            }

            if (branchPlan != null)
            {
                responseBool = true;
            }

            return responseBool;
        }
    }
}
