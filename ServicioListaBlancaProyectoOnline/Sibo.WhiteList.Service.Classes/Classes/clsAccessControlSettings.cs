using Sibo.WhiteList.Service.BLL;
using System;
using System.Collections.Generic;

namespace Sibo.WhiteList.Service.Classes
{
    public class clsAccessControlSettings
    {
        public bool GetAccessControlSettings(int gymId, int branchId)
        {
            try
            {
                AccessControlSettingsBLL acsBLL = new AccessControlSettingsBLL();
                return acsBLL.GetAccessControlSettings(gymId, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<int> GetTimers()
        {
            try
            {
                AccessControlSettingsBLL acsBLL = new AccessControlSettingsBLL();
                return acsBLL.GetTimers();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
