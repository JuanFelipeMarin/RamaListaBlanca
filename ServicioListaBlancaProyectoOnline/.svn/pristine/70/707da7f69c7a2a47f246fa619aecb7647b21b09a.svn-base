using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sibo.WhiteList.Service.BLL.Helpers
{
    public class clsShowMessage
    {
        public static DialogResult Show(string message, clsEnum.MessageType messageType)
        {
            switch (messageType)
            {
                case clsEnum.MessageType.Atencion:
                    return MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                case clsEnum.MessageType.Error:
                    return MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                case clsEnum.MessageType.Informa:
                    return MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                case clsEnum.MessageType.Interroga:
                    return MessageBox.Show(message, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            return DialogResult.OK;
        }
    }
}
