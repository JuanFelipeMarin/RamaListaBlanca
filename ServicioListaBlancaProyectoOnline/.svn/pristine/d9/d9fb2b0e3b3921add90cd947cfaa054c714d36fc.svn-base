using System;
using System.Collections.Generic;
using System.Text;

namespace libComunicacionBioEntry
{
    class clsComandoBioEntry
    {
        public int inicializarSDK()
        {
            int result = 0;
            result = BSSDK.BS_InitSDK();
            return result;
        }

        public int abrirInternoUDP(ref int handler)
        {   
            int result = 0;
            result = BSSDK.BS_OpenInternalUDP(ref handler);
            return result;
        }

        public int BuscarenRed(int m_Handle, ref int m_NumOfDevice, ref uint[] m_DeviceID, ref int[] m_DeviceType, ref uint[] m_DeviceAddr)
        {
            int result = 0;
            result = BSSDK.BS_SearchDeviceInLAN(m_Handle, ref m_NumOfDevice, m_DeviceID, m_DeviceType, m_DeviceAddr);
            return result;
        }
    }
}
