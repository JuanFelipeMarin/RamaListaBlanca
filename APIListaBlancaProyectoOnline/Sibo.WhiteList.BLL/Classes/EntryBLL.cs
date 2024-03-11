﻿using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.BLL.Classes
{
    public class EntryBLL
    {
        public bool AddEntry(List<eEntry> entryList)
        {
            try
            {
                EntryDAL entry = new EntryDAL();
                Exception ex;

                if (entryList != null && entryList.Count > 0)
                {
                    return entry.AddEntry(entryList);
                }
                else
                {
                    ex = new Exception(Exceptions.nullStringList);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    throw ex;
                }
                else
                {
                    throw new Exception(Exceptions.serverError);
                }
            }
        }


        public List<eEntry> GetListEntradasUsuarios(int gymId, string branchId, string id, bool TipoPlan)
        {
            try
            {
                EntryDAL entryDAL = new EntryDAL();
                List<eEntry> entry = new List<eEntry>();

                entry = entryDAL.GetListEntradasUsuarios(gymId, branchId, id, TipoPlan);

                if (entry != null && entry.Count > 0)
                {
                    return entry;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    throw ex;
                }
                else
                {
                    throw new Exception(Exceptions.serverError);
                }
            }
        }

    }
}