using Sibo.WhiteList.Service.DAL.Data;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.BLL.BLL
{
    public class AdditionalEntryBLL
    {
        public bool Insert(eAdditionalEntry aEntry)
        {
            dAdditionalEntry aEntryData = new dAdditionalEntry();

            if (aEntry != null)
            {
                if (!string.IsNullOrEmpty(aEntry.entryType) && !string.IsNullOrEmpty(aEntry.reason))
                {
                    return aEntryData.Insert(aEntry);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
