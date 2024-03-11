﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.BLL.Utilities
{
    public class Validation
    {
        Exception ex;

        public bool ValidateGym(int gymId)
        {
            try
            {
                if (gymId == 0 || string.IsNullOrEmpty(gymId.ToString()))
                {
                    ex = new Exception(Exceptions.nullGym);
                    throw ex;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidateBranch(string branchId)
        {
            try
            {
                // if (branchId == 0 || string.IsNullOrEmpty(branchId.ToString()))
                if (string.IsNullOrEmpty(branchId.ToString()))
                {
                    ex = new Exception(Exceptions.nullBranch);
                    throw ex;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}