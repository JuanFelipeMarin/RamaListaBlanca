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
    
    public partial class sp_ReportClassessAttendance_Result
    {
        public int idBranch { get; set; }
        public string branch { get; set; }
        public string idEmployee { get; set; }
        public string employee { get; set; }
        public Nullable<int> cdclase { get; set; }
        public string className { get; set; }
        public string dayClass { get; set; }
        public System.DateTime dateClass { get; set; }
        public int idCA { get; set; }
        public Nullable<int> cdgimnasio { get; set; }
        public Nullable<int> cdreserva { get; set; }
        public Nullable<int> cdhorario_clase { get; set; }
        public Nullable<System.DateTime> inicioAtencion { get; set; }
        public Nullable<System.DateTime> finAtencion { get; set; }
        public Nullable<int> timeDelay { get; set; }
        public string state { get; set; }
        public string identification { get; set; }
        public string nameClient { get; set; }
    }
}