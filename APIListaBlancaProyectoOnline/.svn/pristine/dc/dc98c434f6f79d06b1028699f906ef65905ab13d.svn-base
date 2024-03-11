using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class EmployeeDAL
    {
        public gim_empleados GetActiveEmployee(int gymId, string employeeId)
        {
            gim_empleados emp = new gim_empleados();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                emp = context.gim_empleados.FirstOrDefault(em => em.cdgimnasio == gymId && em.emp_identifi == employeeId && em.emp_estado == true);
                return emp;
            }
        }

        /// <summary>
        /// Método que consulta y retorna la lista de empleados activos en un gimnasio, ordenados por nombre.
        /// Getulio Vargas - 2018-08-29 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        public List<gim_empleados> GetActiveEmployees(int gymId)
        {
            List<gim_empleados> responseList = new List<gim_empleados>();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                responseList = (from emp in context.gim_empleados
                                where emp.cdgimnasio == gymId && emp.emp_estado == true
                                orderby emp.emp_nombre ascending, emp.emp_primer_apellido ascending, emp.emp_segundo_apellido
                                select emp).ToList();
            }

            return responseList;
        }

        public DataTable ValidateLogonUserByCompany(string userName, string pwd, int siboPawId)
        {
            wsSiboPawDAL.wsSiboPawSoapClient wsSiboPaw = new wsSiboPawDAL.wsSiboPawSoapClient();
            DataTable dt = new DataTable();
            dt = wsSiboPaw.ValidarLogueoUsuarioPorEmpresa(userName, pwd, siboPawId);
            return dt;
        }
    }
}
