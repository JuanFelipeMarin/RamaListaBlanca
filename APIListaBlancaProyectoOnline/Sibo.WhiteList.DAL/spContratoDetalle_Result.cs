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
    
    public partial class spContratoDetalle_Result
    {
        public int dtcont_FKcontrato { get; set; }
        public double dtcont_doc_cliente { get; set; }
        public System.DateTime dtcont_fecha_firma { get; set; }
        public byte[] dtcont_huella_cliente { get; set; }
        public int dtcont_numero_plan { get; set; }
        public int dtcont_tipo_plan { get; set; }
        public int dtcont_sucursal_plan { get; set; }
        public int cdgimnasio { get; set; }
        public int dtcont_fkdia_codigo { get; set; }
        public Nullable<bool> bool_modificado_replica { get; set; }
        public string bits_replica { get; set; }
        public string dtcont_txtContrato { get; set; }
        public byte[] dtcont_firma_cliente { get; set; }
        public Nullable<bool> dtcont_firmado_acudiente { get; set; }
        public Nullable<int> dtcont_modificado { get; set; }
        public int dtcont_numcontrato { get; set; }
        public byte[] dtcont_huella_acudiente { get; set; }
        public byte[] dtcont_firma_acudiente { get; set; }
        public string dtcont_Id_Acudiente { get; set; }
        public string dtcont_Nombre_Acudiente { get; set; }
        public Nullable<bool> bitContratoPendientePorEnvia { get; set; }
    }
}
