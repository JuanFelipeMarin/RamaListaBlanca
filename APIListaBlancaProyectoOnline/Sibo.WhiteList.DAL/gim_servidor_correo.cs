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
    
    public partial class gim_servidor_correo
    {
        public int cdgimnasio { get; set; }
        public string sc_ServidorSMTP { get; set; }
        public int sc_Puerto { get; set; }
        public string sc_Usuario { get; set; }
        public string sc_Password { get; set; }
        public bool sc_bool_modificado { get; set; }
        public string sc_ModCom_confenv_SMS_login { get; set; }
        public string sc_ModCom_confenv_SMS_pwd { get; set; }
        public Nullable<int> sc_ModCom_confenv_SMS_codigopais { get; set; }
        public string sc_ModCom_confenv_MAIL_login { get; set; }
        public string sc_ModCom_confenv_MAIL_pwd { get; set; }
        public Nullable<int> sc_ModCom_confenv_MAIL_mailboxFromId { get; set; }
        public Nullable<int> sc_ModCom_confenv_MAIL_mailboxReplyId { get; set; }
        public Nullable<int> sc_ModCom_confenv_MAIL_mailboxReportId { get; set; }
        public Nullable<int> sc_ModCom_confenv_MAIL_packageId { get; set; }
        public string sc_ModCom_confenv_MAIL_apiKey { get; set; }
        public string sc_ModCom_confenv_MAIL_URL { get; set; }
        public string sc_txtRemitente { get; set; }
        public string bits_replica { get; set; }
        public Nullable<bool> sc_bitInactivo { get; set; }
        public Nullable<int> sc_intPriority { get; set; }
        public Nullable<int> sc_intContEnvios { get; set; }
        public Nullable<int> sc_intLimiteEnvioMasivo { get; set; }
        public Nullable<int> int_codigoPFM { get; set; }
    }
}
