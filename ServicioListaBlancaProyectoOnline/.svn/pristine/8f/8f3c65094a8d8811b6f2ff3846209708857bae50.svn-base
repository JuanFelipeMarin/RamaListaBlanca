using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.DAL.Data;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.BLL.BLL
{
    public class ReplicatedUserBLL
    {
        public bool Insert(eReplicatedUser replicated)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dReplicatedUser replicatedData = new dReplicatedUser();
                string msg = string.Empty;

                if (string.IsNullOrEmpty(replicated.ipAddress))
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = "Dirección ip de la terminal";
                    }
                    else
                    {
                        msg = " - Dirección ip de la terminal";
                    }
                }

                if (string.IsNullOrEmpty(replicated.userId))
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = "Identificación del usuario";
                    }
                    else
                    {
                        msg = " - Identificación del usuario";
                    }
                }

                if (!string.IsNullOrEmpty(msg))
                {
                    throw new Exception("No es posible insertar el registro del usuario replicada, alguno de los parámetros no se envió de forma correcta; Los datos erroneos son: " + msg);
                }

                return replicatedData.Insert(replicated);
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message);
                return false;
            }
        }

        public bool DeleteUserReplicated(int fingerprintId, string userId)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dReplicatedUser replicatedData = new dReplicatedUser();
                string msg = string.Empty;

                if (string.IsNullOrEmpty(userId))
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = "Identificación del cliente";
                    }
                    else
                    {
                        msg = " -Identificación del cliente";
                    }
                }

                if (!string.IsNullOrEmpty(msg))
                {
                    throw new Exception("No es posible eliminar los registros de réplicas anteriores, alguno de los parámetros no se envió de forma correcta; Los datos incorrectos son: " + msg);
                }

                return replicatedData.DeleteUserReplicated(fingerprintId, userId);
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message);
                return false;
            }
        }

        public bool InsertTable(DataTable dt)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dReplicatedUser userData = new dReplicatedUser();
                int records = 0;
                log.WriteProcess("Inicia el proceso de registro de usuarios a insertar en la BD local.");
                records = userData.InsertTable(dt);

                if (records > 0)
                {
                    log.WriteProcess("Los registros de usuarios se procesaron de forma correcta.");
                    return true;
                }
                else
                {
                    log.WriteProcess("Los registros de usuarios no se procesaron de forma correcta.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message);
                return false;
            }
        }

        public bool UpdateUsedUsers(string users)
        {
            dReplicatedUser replicatedData = new dReplicatedUser();
            bool response = false;

            if (!string.IsNullOrEmpty(users))
            {
                replicatedData.UpdateUsedUsers(users);
            }

            return response;
        }

        public bool DeleteUsersDeleted(string ipAddress, string users)
        {
            dReplicatedUser replicatedUser = new dReplicatedUser();
            bool response = false;

            if (!string.IsNullOrEmpty(ipAddress) && !string.IsNullOrEmpty(users))
            {
                response = replicatedUser.DeleteUsersDeleted(ipAddress, users);
            }

            return response;
        }
    }
}
