using Sibo.WhiteList.Service.Connection;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.DAL.Data
{
    public class dAdditionalEntry
    {
        public bool Insert(eAdditionalEntry aEntry)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandText = "spAdditionalEntry";
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "Insert");
                    cmd.Parameters.AddWithValue("@reason", aEntry.reason);
                    cmd.Parameters.AddWithValue("@entryDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@updated", false);
                    cmd.Parameters.AddWithValue("@entryType", aEntry.entryType);

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
    }
}
