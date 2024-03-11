using Sibo.WhiteList.BLL.Utilities;
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
    public class ClientMessagesBLL
    {
        public List<eClientMessages> GetClientMessages(int gymId, string branchId)
        {
            try
            {
                Validation val = new Validation();
                ClientMessagesDAL cm = new ClientMessagesDAL();
                List<gim_mensajes_cliente> messageList = new List<gim_mensajes_cliente>();

                val.ValidateGym(gymId);
                val.ValidateBranch(branchId);
                messageList = cm.GetClientMessages(gymId, branchId);

                return ConvertToEntityMessage(messageList);
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

        private List<eClientMessages> ConvertToEntityMessage(List<gim_mensajes_cliente> messageList)
        {
            List<eClientMessages> responseList = new List<eClientMessages>();

            if (messageList != null && messageList.Count > 0)
            {
                foreach (gim_mensajes_cliente item in messageList)
                {
                    eClientMessages mc = new eClientMessages()
                    {
                        messageDurationTime = Convert.ToInt32(item.men_duracion_pantalla??0),
                        messageFinalDate = Convert.ToDateTime(item.men_fecha_fin??DateTime.MinValue),
                        messageFinalHour = Convert.ToDateTime(item.men_hora_fin??DateTime.MinValue),
                        messageId = item.men_codigo,
                        messageImage = item.men_ImgMsg,
                        messageImgOrder = item.men_OrdenImgMsg,
                        messageInitialDate = Convert.ToDateTime(item.men_fecha_inicio??DateTime.MinValue),
                        messageInitialHour = Convert.ToDateTime(item.men_hora_inicio??DateTime.MinValue),
                        messageState = item.men_estado,
                        messageText = item.men_textomensaje,
                        messageType = item.men_tipo
                    };

                    responseList.Add(mc);
                }
            }

            return responseList;
        }

        public List<eClientMessages> GetClientMessagesDownload(int gymId, string branchId, string ipTerminal)
        {
            try
            {
                Validation val = new Validation();
                ClientMessagesDAL cm = new ClientMessagesDAL();
                List<gim_mensajes_cliente> messageList = new List<gim_mensajes_cliente>();

                val.ValidateGym(gymId);
                val.ValidateBranch(branchId);
                messageList = cm.GetClientMessagesDownload(gymId, branchId, ipTerminal.Replace(',','.'));

                return ConvertToEntityMessage(messageList);
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

        public bool GetClientMessagesInserReplicate(int gymId, string branchId, int IDImg, string ipTerminal)
        {
            try
            {
                Validation val = new Validation();
                ClientMessagesDAL cm = new ClientMessagesDAL();
                bool messageList = false;

                val.ValidateGym(gymId);
                val.ValidateBranch(branchId);
                messageList = cm.GetClientMessagesInserReplicate(gymId, branchId, IDImg, ipTerminal);

                return messageList;
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
