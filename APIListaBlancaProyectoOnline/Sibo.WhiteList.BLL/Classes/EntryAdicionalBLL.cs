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
    public class EntryAdicionalBLL
    {
        public bool AddEntry(List<eEntry> entryList)
        {
            try
            {
                EntryDAL entry = new EntryDAL();
                Exception ex;

                if (entryList != null && entryList.Count > 0)
                {
                    return entry.AddEntryAdicional(entryList);
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
    }
}
