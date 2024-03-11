using Sibo.WhiteList.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
   public class ReplicaDAL
    {
        public bool GetActualizarEstadoReplica(int gymId, string id, string ipTerminal)
        {
            try
            {
                using (var context = new dbWhiteListModelEntities(gymId))
                {
                    try
                    {
                        tblReplicaHuellas consulta = new tblReplicaHuellas();

                        consulta = context.tblReplicaHuellas.Where(a => a.idPersona == id && a.ipAddress == ipTerminal).FirstOrDefault();
                        if (consulta != null)
                        {
                            consulta.bitDelete = true;

                            context.tblReplicaHuellas.Add(consulta);
                            context.SaveChanges();
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                    catch (Exception)
                    {
                        return false;
                    }

                }

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool GetEliminarReplicaPersona(int gymId, string intIdHuella_Tarjeta,string idPersona)
        {
            try
            {
                DataTable dt = new DataTable();
                bool resp = false;
                using (var db = new dbWhiteListModelEntities(gymId))
                {
                    string strConnection = db.Database.Connection.ConnectionString.ToString();

                    using (SqlConnection conn = new SqlConnection(strConnection))
                    {
                        conn.Open();

                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = "spReplicaHuellas";
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@action", "EliminarReplicaPersona");
                            cmd.Parameters.AddWithValue("@intIdHuella_Tarjeta", intIdHuella_Tarjeta);
                            cmd.Parameters.AddWithValue("@idPersona", idPersona);

                            //cmd.ExecuteNonQuery();
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dt);
                                resp = true;
                            }

                        }
                    }

                }

                return resp;
            }
            catch (Exception ex)
            {
                return false;
            }


        }


        public bool InsertReplicaHuellas(eReplicatedFingerprint entity)
        {
            try
            {
                bool resp = false; 
                tblReplicaHuellas rh = new tblReplicaHuellas()
                {
                    intIdHuella_Tarjeta = entity.fingerprintId,
                    idPersona = entity.userId,
                    ipAddress = entity.ipAddress,
                    idEmpresa = entity.cdgimnacio,
                    dtmfechaReplica = entity.replicationDate,
                    bitDelete = entity.bitDelete,
                    bitHuella = entity.bitHuella,
                    bitTarje_Pin = entity.bitTarje_Pin
                };

        
              using (var context = new dbWhiteListModelEntities(entity.cdgimnacio))
               {
                 context.tblReplicaHuellas.Add(rh);
                 context.SaveChanges();
                  resp = true;
               }

                return resp;
                    
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
