using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.DAL.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.BLL.Classes
{
    public class BlackListBLL
    {
        public bool ValidateActiveRecord(int gymId, string id)
        {
            try
            {

                BlackListDAL blackListDAL = new BlackListDAL();
                Exception ex;

                if (gymId == 0 || string.IsNullOrEmpty(gymId.ToString()))
                {
                    ex = new Exception(Exceptions.nullGym);
                    throw ex;
                }

                if (string.IsNullOrEmpty(id.ToString()))
                {
                    ex = new Exception(Exceptions.nullId);
                    throw ex;
                }

                return blackListDAL.ValidateActiveRecord(gymId, Convert.ToDouble(id));
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
