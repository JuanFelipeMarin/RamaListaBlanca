using Sibo.WhiteList.Service.BLL;
using System;

namespace Sibo.Servicio.ListaBlanca.Classes
{
    public class clsEntry
    {
        public bool AddEntry(int gymId, int branchId)
        {
            try
            {
                EntryBLL entryBLL = new EntryBLL();
                return entryBLL.AddEntry(gymId, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
