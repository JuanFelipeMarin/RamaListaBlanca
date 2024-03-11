using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.BLL.BLL
{
    public class EmployeeBLL
    {
        public eResponse ValidateUserEmployee(int gymId, string userName, string pwd)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                eUserResponse userResponse = new eUserResponse();
                EmployeeAPI employeeAPI = new EmployeeAPI();
                eResponse resp = new eResponse();

                if (!string.IsNullOrEmpty(gymId.ToString()) && gymId != 0 && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(pwd))
                {
                    userResponse.gymId = gymId;
                    userResponse.pwd = pwd;
                    userResponse.userName = userName;
                    log.WriteProcess("Consumo de API para validar el usuario " + userName);
                    resp = employeeAPI.ValidateUserEmployee(userResponse);
                }
                else
                {
                    resp.state = false;
                    resp.message = "NoData";
                }

                return resp;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " - " + ex.StackTrace.ToString());
                return null;
            }
        }
    }
}
