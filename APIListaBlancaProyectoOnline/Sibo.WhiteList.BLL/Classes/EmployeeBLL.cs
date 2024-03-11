using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Classes;
using System.Data;

namespace Sibo.WhiteList.BLL.Classes
{
    public class EmployeeBLL
    {
        public gim_empleados GetActiveEmployee(int gymId, string employeeId)
        {
            try
            {
                Exception ex;
                EmployeeDAL employeeDAL = new EmployeeDAL();
                gim_empleados employee = new gim_empleados();

                if (gymId == 0 || string.IsNullOrEmpty(gymId.ToString()))
                {
                    ex = new Exception(Exceptions.nullGym);
                    throw ex;
                }

                if (string.IsNullOrEmpty(employeeId.ToString()))
                {
                    ex = new Exception(Exceptions.nullId);
                    throw ex;
                }

                employee = employeeDAL.GetActiveEmployee(gymId, employeeId);
                return employee;
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

        public DataTable ValidateLogonUserByCompany(string userName, string pwd, int siboPawId)
        {
            EmployeeDAL employeeDAL = new EmployeeDAL();
            return employeeDAL.ValidateLogonUserByCompany(userName, pwd, siboPawId);
        }

        /// <summary>
        /// Método encargado de consultar los empleados activos de un gimnasio específico y retornarlos en una lista genérica (Por ejemplo para llenar un combo).
        /// GEtulio Vargas - 2018-08-29 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        public List<eGeneric> GetActiveEmployees(int gymId)
        {
            Exception ex;
            EmployeeDAL employeeDAL = new EmployeeDAL();
            List<gim_empleados> empList = new List<gim_empleados>();
            List<eGeneric> responseList = new List<eGeneric>();

            if (gymId == 0 || string.IsNullOrEmpty(gymId.ToString()))
            {
                ex = new Exception(Exceptions.nullGym);
                throw ex;
            }

            empList = employeeDAL.GetActiveEmployees(gymId);

            if (empList != null && empList.Count > 0)
            {
                eGeneric gen = new eGeneric();
                gen.key = "0";
                gen.value = "--Seleccione--";
                responseList.Add(gen);

                foreach (gim_empleados item in empList)
                {
                    gen = new eGeneric();
                    gen.key = item.emp_identifi;
                    gen.value = ((item.emp_nombre == null) ? "" : item.emp_nombre) + " " + ((item.emp_primer_apellido == null) ? "" : item.emp_primer_apellido) +
                                " " + ((item.emp_segundo_apellido == null) ? "" : item.emp_segundo_apellido);
                    responseList.Add(gen);
                }
            }

            return responseList;
        }
    }
}
