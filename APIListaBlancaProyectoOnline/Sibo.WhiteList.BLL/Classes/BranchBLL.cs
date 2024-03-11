﻿using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.BLL.Classes
{
    public class BranchBLL
    {
        public gim_sucursales GetBranch(int gymId, string branchId)
        {
            try
            {
                BranchDAL branchDAL = new BranchDAL();
                Validation val = new Validation();
                val.ValidateGym(gymId);
                val.ValidateBranch(branchId);
                return branchDAL.GetBranch(gymId, branchId);
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
