//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sibo.WhiteList.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Terminal
    {
        public int ter_intId { get; set; }
        public int cdgimnasio { get; set; }
        public int ter_intBranchId { get; set; }
        public string ter_strIpAddress { get; set; }
        public bool ter_bitServesToOutput { get; set; }
        public bool ter_bitServesToInputAndOutput { get; set; }
        public int ter_intTimeWaitAnswerReplicate { get; set; }
        public bool ter_bitTCAM7000 { get; set; }
        public bool ter_bitLectorZK { get; set; }
        public bool ter_bitICAM7000 { get; set; }
        public bool ter_bitCardMode { get; set; }
        public string ter_strPort { get; set; }
        public Nullable<int> ter_intSpeedPort { get; set; }
        public Nullable<bool> withWhiteList { get; set; }
        public string bits_replica { get; set; }
        public Nullable<bool> ter_modificado { get; set; }
        public string name { get; set; }
        public Nullable<bool> state { get; set; }
        public Nullable<int> FK_TerminalTypeId { get; set; }
        public string Zonas { get; set; }
        public string snTerminal { get; set; }
        public string ModeloTerminal { get; set; }
        public string IdModeloTerminal { get; set; }
    
        public virtual TerminalTypes TerminalTypes { get; set; }
    }
}