using Sibo.WhiteList.Service.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.Servicio.ListaBlanca.Classes
{
    public class clsFingerprint
    {
        public bool Insert(string userId, string ipAddress, int fingerprintId, byte[] tmpFingerprint, int quality)
        {
            try
            {
                FingerprintBLL fpBll = new FingerprintBLL();
                return fpBll.Insert(userId, ipAddress, fingerprintId, tmpFingerprint, quality);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
