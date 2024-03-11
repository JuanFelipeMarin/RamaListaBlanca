using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class MailServerDataDAL
    {
        public eMailServerData GetMailServerData(int gymId)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                eMailServerData response = new eMailServerData();

                response = (from msd in context.gim_servidor_correo
                            where msd.cdgimnasio == gymId
                            select new eMailServerData {
                                password = msd.sc_Password,
                                port = msd.sc_Puerto.ToString(),
                                SMTPServer = msd.sc_ServidorSMTP,
                                user = msd.sc_Usuario
                            }).FirstOrDefault();

                return response;
            }
        }
    }
}
