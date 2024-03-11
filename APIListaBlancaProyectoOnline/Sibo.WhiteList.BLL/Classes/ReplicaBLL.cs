using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.BLL.Classes
{
  public  class ReplicaBLL
    {
        ReplicaDAL replicaDAL = new ReplicaDAL();
        public bool GetActualizarEstadoReplica(int gymId, string id, string ipTerminal)
        {
            try
            {
                Validation val = new Validation();
                val.ValidateGym(gymId);
                               
                return replicaDAL.GetActualizarEstadoReplica(gymId, id, ipTerminal);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    return false;
                }
                else
                {
                    throw new Exception(Exceptions.serverError);
                }
            }
        }

        public bool GetEliminarReplicaPersona(int gymId, string intIdHuella_Tarjeta, string idPersona)
        {
            try
            {
                Validation val = new Validation();
                val.ValidateGym(gymId);
                return replicaDAL.GetEliminarReplicaPersona(gymId, intIdHuella_Tarjeta, idPersona);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    return false;
                }
                else
                {
                    throw new Exception(Exceptions.serverError);
                }
            }
        }

        public bool InsertReplicaHuellas(eReplicatedFingerprint entity)
        {
            try
            {
                bool resp = false;
                if (entity != null)
                {
                    resp = replicaDAL.InsertReplicaHuellas(entity);
                    
                }

                return resp;
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    return false;
                }
                else
                {
                    throw new Exception(Exceptions.serverError);
                }
            }
        }


    }
}
