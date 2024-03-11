using Sibo.WhiteList.BLL.Classes;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Sibo.WhiteList.Api.Controllers
{
    [RoutePrefix("api/Employee")]
    public class EmployeeController : ApiController
    {
        GymBLL gymBll = new GymBLL();

        /// <summary>
        /// Método que permite validar si un empleado es un usuario del sistema.
        /// </summary>
        /// <param name="userEmployee"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(eResponse))]
        public async Task<IHttpActionResult> ValidateUserEmployee(eUserEmployee userEmployee)
        {
            try
            {
                eResponse response = new eResponse();
                response = await Task.Run(() => Validateuser(userEmployee));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private eResponse Validateuser(eUserEmployee userEmployee)
        {
            gim_gimnasio gym = new gim_gimnasio();
            gim_empleados emp = new gim_empleados();
            EmployeeBLL empBll = new EmployeeBLL();
            DataTable dt = new DataTable();
            eResponse response = new eResponse();
            bool ok = false;
            string id = string.Empty;
            gym = gymBll.GetGymById(userEmployee.gymId);

            if (gym != null && gym.IdSiboPaw != null)
            {
                dt = empBll.ValidateLogonUserByCompany(userEmployee.userName, userEmployee.pwd, Convert.ToInt32(gym.IdSiboPaw));
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    ok = Convert.ToBoolean(dt.Rows[0][0]);

                    if (ok)
                    {
                        string[] array = dt.Rows[0][1].ToString().Split('/');

                        if (array.Length > 1)
                        {
                            id = array[1];
                        }

                        if (!string.IsNullOrEmpty(id))
                        {
                            emp = empBll.GetActiveEmployee(userEmployee.gymId, id);

                            if (emp != null)
                            {
                                response.state = true;
                                response.message = "Ok";
                                response.messageOne = emp.emp_identifi;
                                return response;
                            }
                            else
                            {
                                response.state = false;
                                response.message = "NoEmployee";
                                return response;
                            }
                        }
                        else
                        {
                            response.state = false;
                            response.message = "NoId";
                            return response;
                        }
                    }
                    else
                    {
                        response.state = false;
                        response.message = "NoUserOrPassword";
                        return response;
                    }
                }
                else
                {
                    response.state = false;
                    response.message = "NoUserOrPassword";
                    return response;
                }
            }
            else
            {
                response.state = false;
                response.message = "NoGym";
                return response;
            }
        }
    }
}