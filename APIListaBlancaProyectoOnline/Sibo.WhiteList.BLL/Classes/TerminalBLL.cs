﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Classes;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.DAL.Classes;

namespace Sibo.WhiteList.BLL.Classes
{
    public class TerminalBLL
    {
        /// <summary>
        /// Método encargado de consultar las terminales de una sucursal y retornar la lista de estas.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public List<eTerminal> GetTerminals(int gymId, string branchId)
        {
            TerminalDAL termDAL = new TerminalDAL();
            List<Terminal> terList = new List<Terminal>();
            List<eTerminal> responseList = new List<eTerminal>();
            terList = termDAL.GetTerminals(gymId, branchId);
            responseList = ConvertToListEntity(terList, gymId);
            return responseList;
        }

        /// <summary>
        /// Método encargado de convertir una lista de la entidad de BD 'Terminal' en una lista de la entidad 'eTerminal'.
        /// Getulio Vargas - 2018-04-11 - OD 1307
        /// </summary>
        /// <param name="terList"></param>
        /// <returns></returns>
        private List<eTerminal> ConvertToListEntity(List<Terminal> terList, int gymId)
        {
            List<eTerminal> responseList = new List<eTerminal>();
            TerminalTypeDAL terminalTypeDAL = new TerminalTypeDAL();

            if (terList != null && terList.Count > 0)
            {
                foreach (Terminal item in terList)
                {
                    eTerminal ter = new eTerminal()
                    {
                        ICAM7000 = item.ter_bitICAM7000,
                        ipAddress = item.ter_strIpAddress,
                        LectorZK = item.ter_bitLectorZK,
                        port = item.ter_strPort,
                        servesToOutput = item.ter_bitServesToOutput,
                        servesToInputAndOutput = item.ter_bitServesToInputAndOutput,
                        speedPort = Convert.ToInt32(item.ter_intSpeedPort ?? 0),
                        TCAM7000 = item.ter_bitTCAM7000,
                        terminalId = item.ter_intId,
                        timeWaitAnswerReplicate = item.ter_intTimeWaitAnswerReplicate,
                        bitCardMode = item.ter_bitCardMode,
                        withWhiteList = Convert.ToBoolean(item.withWhiteList ?? false),
                        name = item.name,
                        state = item.state ?? false,
                        terminalTypeId = item.FK_TerminalTypeId ?? 0,
                        terminalTypeDescription = terminalTypeDAL.GetTerminalType((item.FK_TerminalTypeId ?? 0), gymId).description ?? string.Empty,
                        Zonas = item.Zonas,
                        snTerminal = item.snTerminal

                    };

                    responseList.Add(ter);
                }
            }

            return responseList;
        }
    }
}
