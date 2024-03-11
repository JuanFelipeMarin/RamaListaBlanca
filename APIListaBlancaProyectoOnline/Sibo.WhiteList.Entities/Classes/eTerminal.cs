using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Classes
{
    public class eTerminal
    {
        public int terminalId { get; set; }
        public string ipAddress { get; set; }
        public bool servesToOutput { get; set; }
        public bool servesToInputAndOutput { get; set; }
        public int timeWaitAnswerReplicate { get; set; }
        public bool TCAM7000 { get; set; }
        public bool LectorZK { get; set; }
        public bool ICAM7000 { get; set; }
        public string port { get; set; }
        public int speedPort { get; set; }
        public bool bitCardMode { get; set; }
        public bool withWhiteList { get; set; }
        public string name { get; set; }
        public int terminalTypeId { get; set; }
        public string terminalTypeDescription { get; set; }
        public bool state { get; set; }

        public string snTerminal { get; set; }
        public string Zonas { get; set; }
    }
}
