using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.Servicio.ListaBlanca.Classes
{
    public class clsReplicatedFingerprint
    {
        public bool Insert(eReplicatedFingerprint replicated)
        {
            try
            {
                ReplicatedFingerprintBLL replicatedBll = new ReplicatedFingerprintBLL();
                return replicatedBll.Insert(replicated);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteFingerprintReplicated(int fingerprintId, string userId)
        {
            try
            {
                ReplicatedFingerprintBLL replicatedBll = new ReplicatedFingerprintBLL();
                return replicatedBll.DeleteFingerprintReplicated(fingerprintId, userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
