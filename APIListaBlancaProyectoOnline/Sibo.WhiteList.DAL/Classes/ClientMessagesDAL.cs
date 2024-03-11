using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class ClientMessagesDAL
    {
        public List<gim_mensajes_cliente> GetClientMessages(int gymId, string branchId)
        {
            List<gim_mensajes_cliente> responseList = new List<gim_mensajes_cliente>();
            string[] ListabranchId = branchId.Split(',');

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                int sucursal = 0;
                for (int i = 0; i < ListabranchId.Length; i++)
                {
                    sucursal = Convert.ToInt32(ListabranchId[i]);

                    var listterminal = (from messages in context.gim_mensajes_cliente
                                    where messages.cdgimnasio == gymId && messages.men_sucursal == sucursal && (messages.men_tipo == "1" || messages.men_tipo == "2")
                                        select messages).ToList();

                    foreach (var item in listterminal)
                    {
                        responseList.Add(item);
                    }

                }
                return responseList;
            }
        }

        public List<gim_mensajes_cliente> GetClientMessagesDownload(int gymId, string branchId, string ipTerminal)
        {
            List<gim_mensajes_cliente> responseList = new List<gim_mensajes_cliente>();
            string[] ListabranchId = branchId.Split(',');

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                int sucursal = 0;
                for (int i = 0; i < ListabranchId.Length; i++)
                {
                    sucursal = Convert.ToInt32(ListabranchId[i]);

                    var listReplica = (from messages in context.tblReplicatedMessages
                                       where messages.ipAddress == ipTerminal
                                       select messages.messageId).ToList();

                    var listReplica2 = (from messages in context.gim_mensajes_cliente
                                       where messages.cdgimnasio == gymId && messages.men_sucursal == sucursal && (messages.men_tipo == "1" || messages.men_tipo == "2")
                                       && !listReplica.Contains(messages.men_codigo)
                                        select messages).ToList();

                    foreach (var item in listReplica2)
                    {
                        responseList.Add(item);
                    }

                }
                return responseList;
            }
        }

        public bool GetClientMessagesInserReplicate(int gymId, string branchId, int IDImg, string ipTerminal)
        {
            int resp = 0;
            try
            {
                using (var context = new dbWhiteListModelEntities(gymId))
                {
                    tblReplicatedMessages consulta = new tblReplicatedMessages();

                    consulta.messageId = IDImg;
                    consulta.ipAddress = ipTerminal;
                    consulta.replicationDate = DateTime.Now;

                    context.Set<tblReplicatedMessages>().Add(consulta);
                    resp = context.SaveChanges();



                    if (resp > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
