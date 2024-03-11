using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.DAL.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.BLL.Classes
{
    public class BranchPlanBLL
    {
        public bool ValidatePlanInBranch(int gymId, string branchId, int planId)
        {
            try
            {
                BranchPlanDAL branchPlanDAL = new BranchPlanDAL();
                Exception ex;

                if (gymId == 0 || string.IsNullOrEmpty(gymId.ToString()))
                {
                    ex = new Exception(Exceptions.nullGym);
                    throw ex;
                }

                if (branchId == "0" || string.IsNullOrEmpty(branchId.ToString()))
                {
                    ex = new Exception(Exceptions.nullBranch);
                    throw ex;
                }

                if (planId == 0 || string.IsNullOrEmpty(planId.ToString()))
                {
                    ex = new Exception(Exceptions.nullPlan);
                    throw ex;
                }

                return branchPlanDAL.ValidatePlanInBranch(gymId, branchId, planId);
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
