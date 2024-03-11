using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class TerminalTypeDAL
    {
        public TerminalTypes GetTerminalType(int terminalTypeId, int gymId)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                return context.TerminalTypes.FirstOrDefault(tt => tt.PK_TerminalTypeId == terminalTypeId);
            }
        }
    }
}
