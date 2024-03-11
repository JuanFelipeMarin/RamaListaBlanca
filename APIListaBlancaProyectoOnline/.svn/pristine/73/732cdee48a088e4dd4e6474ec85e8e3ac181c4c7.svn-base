using Sibo.WhiteList.DAL.Helpers;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class CompanyDAL
    {
        public eCompany get(int companyID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["strConexionGSWAdmin"].ToString();
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {

                conexion.Open();
                using (SqlCommand cmdDetalle = new SqlCommand("spCompany", conexion))
                {

                    cmdDetalle.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdDetalle.CommandTimeout = 0;
                    cmdDetalle.Parameters.AddWithValue("@companyID", companyID);
                    using (SqlDataAdapter daDetalle = new SqlDataAdapter(cmdDetalle))
                    {

                        DataTable dtResultado = new DataTable();
                        daDetalle.Fill(dtResultado);
                        daDetalle.Dispose();
                        cmdDetalle.Dispose();
                        conexion.Close();
                        conexion.Dispose();
                        if (dtResultado.Rows.Count == 0)
                            throw new Exception("No se encontró gimnasio:" + companyID.ToString());
                        ConvertDataRowToEntities<eCompany> convert = new ConvertDataRowToEntities<eCompany>();
                        return convert.DataRowToEntitie(dtResultado.Rows[0]);
                    }
                }
            }
        }
    }
}
