using Sibo.WhiteList.Service.DAL.Data;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Sibo.WhiteList.Service.BLL.BLL
{
    public class ActionBLL
    {
        public int Insert(eAction action)
        {
            dAction actionData = new dAction();

            if (action != null)
            {
                if (!string.IsNullOrEmpty(action.ipAddress) && !string.IsNullOrEmpty(action.strAction))
                {
                    return actionData.Insert(action);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public void InsertHuellaBioLite(string strip, string snTerminar, string personId , bool bitInsertFingert)
        {
            dAction actionData = new dAction();
                        
            actionData.InsertHuellaBioLite(strip, snTerminar, personId, bitInsertFingert);
            
        }


        public DataTable ConsultarAccionPendiente(string ip)
        {
            dAction actionData = new dAction();

          return  actionData.ConsultarAccionPendiente(ip);

        }
        public void EliminarDetalleAccion(int id)
        {
            dAction actionData = new dAction();
            actionData.EliminarDetalleAccion(id);
        }
        public void ActualizarHuellasPersonas(byte[] byteHuela, int intId)
        {
            dAction actionData = new dAction();

            actionData.ActualizarHuellasPersonas(byteHuela, intId);
        }

        public bool Update(eAction action)
        {
            dAction actionData = new dAction();

            if (action != null)
            {
                return actionData.Update(action);
            }
            else
            {
                return false;
            }
        }

        public List<eAction> GetPendingActionsByTerminal(string ipAddress)
        {
            dAction actionData = new dAction();
            return actionData.GetPendingActionsByTerminal(ipAddress);
        }
    }
}
