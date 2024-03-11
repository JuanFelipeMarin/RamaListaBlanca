using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class ClientCardDAL
    {
        public List<eClientCard> GetClientCards(int gymId, string clientId)
        {
            using (var context = new dbWhiteListModelEntities(gymId))
            {
                var query = (from cc in context.ClientCards
                        where cc.cdgimnasio == gymId && cc.cli_identifi == clientId
                        select new eClientCard
                        {
                            cardCode = cc.cardCode,
                            clientCardId = cc.PK_ClientCardId,
                            clientId = cc.cli_identifi,
                            state = cc.state
                        }).ToList<eClientCard>();
                return query;
            }
        }
    }
}
