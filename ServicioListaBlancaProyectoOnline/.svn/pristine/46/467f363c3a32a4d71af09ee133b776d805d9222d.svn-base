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
    public class UserBLL
    {
        public bool InsertTable(DataTable dtUsers)
        {
            dUser user = new dUser();
            
            if (dtUsers.Rows.Count > 0)
            {
                return user.InsertTable(dtUsers);
            }
            else
            {
                return false;
            }
        }

        public bool Update(string userId, bool withoutFingerprint, int fingerprintId, bool insert, bool delete)
        {
            dUser user = new dUser();

            if (!string.IsNullOrEmpty(userId))
            {
                return user.Update(userId, withoutFingerprint, fingerprintId, insert, delete);
            }
            else
            {
                return false;
            }
        }

        public bool UpdateUsedUsers(string userList)
        {
            dUser user = new dUser();

            if (!string.IsNullOrEmpty(userList))
            {
                return user.UpdateUsedUsers(userList);
            }
            else
            {
                return false;
            }
        }

        public List<eUser> GetUsersToReplicate(string ipAddress)
        {
            dUser user = new dUser();
            return user.GetUsersToReplicate(ipAddress);
        }

        public List<eUser> GetUsersToDelete(string ipAddress)
        {
            dUser user = new dUser();
            return user.GetUsersToDelete(ipAddress);
        }

        public bool UpdateFingerprintId(string userId, int fingerprintId)
        {
            dUser user = new dUser();
            return user.UpdateFingerprintId(userId, fingerprintId);
        }

        public eUser GetUser(string userId)
        {
            dUser user = new dUser();
            return user.GetUser(userId);
        }

        public bool Insert(string userId, string userName, int fingerprintId, bool withoutFingerprint, bool bitInsert, bool bitDelete)
        {
            dUser user = new dUser();

            if (!string.IsNullOrEmpty(userId))
            {
                return user.Insert(userId, userName, fingerprintId, withoutFingerprint, bitInsert, bitDelete);
            }
            else
            {
                return false;
            }
        }

        public bool InsertUser(eUser user)
        {
            dUser userData = new dUser();
            bool response = false;

            if (user != null && !string.IsNullOrEmpty(user.userId))
            {
                return userData.InsertUser(user);
            }

            return response;
        }
    }
}
