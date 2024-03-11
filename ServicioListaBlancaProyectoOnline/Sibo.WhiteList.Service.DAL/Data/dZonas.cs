using System;
using System.Collections.Generic;
using Sibo.WhiteList.Service.Entities.Classes;
using System.Data;
using System.Data.SqlClient;
using Sibo.WhiteList.Service.Connection;


namespace Sibo.WhiteList.Service.DAL.Data
{
 public   class dZonas
    {

        public eZonas GetZonasById(int id)
        {
            DataAccess objConn = new DataAccess();
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spZonas";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "GetZonasById");
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    objConn.ConnectionDispose();
                    return ConvertToEntity(dt);
                }
            }
            catch
            {
                objConn.ConnectionDispose();
                return null;
            }
        }

        private eZonas ConvertToEntity(DataTable dt)
        {
            eZonas term = new eZonas();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                term.id = Convert.ToInt32(row["id"]);
                term.codigo_string = row["codigo_string"].ToString();
                term.cdgimnasio = Convert.ToInt32(row["cdgimnasio"]);
                term.descripcion = row["descripcion"].ToString();
                term.nombre_zona = row["nombre_zona"].ToString();
                term.intSucursal = Convert.ToInt32(row["intSucursal"]);
                term.Terminales = row["Terminales"].ToString();
                term.bitEstado = Convert.ToBoolean(row["bitEstado"]);
                term.fechacreacion = Convert.ToDateTime(row["fechacreacion"]);

                
            }

            return term;
        }

        public bool Update(eZonas term)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spZonas";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "Update");
                    cmd.Parameters.AddWithValue("@id", term.id);
                    cmd.Parameters.AddWithValue("@codigo_string", term.codigo_string);
                    cmd.Parameters.AddWithValue("@cdgimnasio", term.cdgimnasio);
                    cmd.Parameters.AddWithValue("@descripcion", term.descripcion);
                    cmd.Parameters.AddWithValue("@nombre_zona", term.nombre_zona);
                    cmd.Parameters.AddWithValue("@intSucursal", term.intSucursal);
                    cmd.Parameters.AddWithValue("@Terminales", term.Terminales);
                    cmd.Parameters.AddWithValue("@bitEstado", term.bitEstado);
                    cmd.Parameters.AddWithValue("@fechacreacion", term.fechacreacion);

                   
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
            catch
            {
                objConn.ConnectionDispose();
                return false;
            }
        }

        public bool Insert(eZonas term)
        {
            DataAccess objConn = new DataAccess();
            int resp = 0;

            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = objConn.GetConnection();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spZonas";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@action", "Insert");
                    cmd.Parameters.AddWithValue("@id", term.id);
                    cmd.Parameters.AddWithValue("@codigo_string", term.codigo_string);
                    cmd.Parameters.AddWithValue("@cdgimnasio", term.cdgimnasio);
                    cmd.Parameters.AddWithValue("@descripcion", term.descripcion);
                    cmd.Parameters.AddWithValue("@nombre_zona", term.nombre_zona);
                    cmd.Parameters.AddWithValue("@intSucursal", term.intSucursal);
                    cmd.Parameters.AddWithValue("@Terminales", term.Terminales);
                    cmd.Parameters.AddWithValue("@bitEstado", term.bitEstado);
                    cmd.Parameters.AddWithValue("@fechacreacion", term.fechacreacion);
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
            catch
            {
                objConn.ConnectionDispose();
                return false;
            }
        }
    }
}
