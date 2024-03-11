using Sibo.WhiteList.Service.Connection;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.DAL.Data
{
    public class dActionParameters
    {
        string spName = "spActionParameters";

        public bool Insert(eActionParameters aParameters)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = spName;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "Insert");
                    cmd.Parameters.AddWithValue("@actionId", aParameters.actionId);
                    cmd.Parameters.AddWithValue("@parameterName", aParameters.parameterName);
                    cmd.Parameters.AddWithValue("@parameterValue", aParameters.parameterValue);
                    cmd.Parameters.AddWithValue("@dateParameter", DateTime.Now);
                    resp = cmd.ExecuteNonQuery();
                    objConn.ConnectionDispose();

                    if (resp > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        public List<eActionParameters> GetParameters(int actionId)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "GetParameters");
                    cmd.Parameters.AddWithValue("@actionId", actionId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return ConvertToActionParametersEntity(dt);
                }
            }
            catch (Exception ex)
            {
                objConn.ConnectionDispose();
                throw ex;
            }
        }

        private List<eActionParameters> ConvertToActionParametersEntity(DataTable dt)
        {
            List<eActionParameters> responseList = new List<eActionParameters>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    eActionParameters aParameters = new eActionParameters()
                    {
                        actionId = Convert.ToInt32(row["actionId"].ToString()),
                        dateParameter = row["dateParameter"].ToString(),
                        id = Convert.ToInt32(row["id"].ToString()),
                        parameterName = row["parameterName"].ToString(),
                        parameterValue = row["parameterValue"].ToString()
                    };

                    responseList.Add(aParameters);
                }
            }

            return responseList;
        }
    }
}
