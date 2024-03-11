﻿using Sibo.WhiteList.Service.BLL.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.Classes
{
    public class clsHoliday
    {
        public bool GetHolidays(int gymId)
        {
            try
            {
                HolidayBLL hdBll = new HolidayBLL();
                return hdBll.GetHolidays(gymId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}