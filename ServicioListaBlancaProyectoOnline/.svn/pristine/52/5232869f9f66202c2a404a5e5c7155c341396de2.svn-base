using Sibo.WhiteList.Service.BLL.BLL;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.IngresoMonitor.Classes
{
    public class clsAdditionalEntry
    {
        public bool Insert(string reason, string entryType)
        {
            try
            {
                AdditionalEntryBLL aEntryBll = new AdditionalEntryBLL();
                eAdditionalEntry aEntryEntity = new eAdditionalEntry();

                aEntryEntity.entryType = entryType;
                aEntryEntity.reason = reason;
                return aEntryBll.Insert(aEntryEntity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
