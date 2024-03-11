using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.DAL.Data;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.BLL.BLL
{
    public class ReplicatedFingerprintBLL
    {
        public bool Insert(eReplicatedFingerprint replicated)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dReplicatedFingerprint replicatedData = new dReplicatedFingerprint();
                string msg = string.Empty;

                if (replicated.fingerprintId <= 0)
                {
                    msg = "Id de la huella";
                }

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
                    throw new Exception("No es posible insertar el registro de la huella replicada, alguno de los parámetros no se envió de forma correcta; Los datos erroneos son: " + msg);
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

        public bool DeleteFingerprintReplicated(int fingerprintId, string userId)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dReplicatedFingerprint replicatedData = new dReplicatedFingerprint();
                string msg = string.Empty;

                if (fingerprintId <= 0)
                {
                    msg = "Id de la huella";
                }

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

                return replicatedData.DeleteFingerprintReplicated(fingerprintId, userId);
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message);
                return false;
            }
        }

        public bool DeleteFingerprintsDeleted(string ipAddress, string fingerprints)
        {
            dReplicatedFingerprint replicatedFingerprint = new dReplicatedFingerprint();
            bool response = false;

            if (!string.IsNullOrEmpty(ipAddress) && !string.IsNullOrEmpty(fingerprints))
            {
                response = replicatedFingerprint.DeleteFingerprintsDeleted(ipAddress, fingerprints);
            }

            return response;
        }
    }
}
