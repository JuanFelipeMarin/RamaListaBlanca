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
    
    public partial class tblConsulta
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblConsulta()
        {
            this.tblExamenFisico = new HashSet<tblExamenFisico>();
            this.tblMovimientoAntecedentesDeportivos = new HashSet<tblMovimientoAntecedentesDeportivos>();
            this.tblMovimientoAntecedentesFamiliares = new HashSet<tblMovimientoAntecedentesFamiliares>();
            this.tblMovimientoAntecedentesOcupacionales = new HashSet<tblMovimientoAntecedentesOcupacionales>();
            this.tblMovimientoAntecedentesPersonales = new HashSet<tblMovimientoAntecedentesPersonales>();
            this.tblMovimientoDiagnosticos = new HashSet<tblMovimientoDiagnosticos>();
            this.tblMovimientoEvaluacionPorSistemas = new HashSet<tblMovimientoEvaluacionPorSistemas>();
            this.tblMovimientoPerimetros = new HashSet<tblMovimientoPerimetros>();
            this.tblMovimientoPliegues = new HashSet<tblMovimientoPliegues>();
            this.tblMovimientoRiesgoCaderaInferior = new HashSet<tblMovimientoRiesgoCaderaInferior>();
            this.tblMovimientoRiesgoCaderaSuperior = new HashSet<tblMovimientoRiesgoCaderaSuperior>();
            this.tblMovimientoRiesgoEspalda = new HashSet<tblMovimientoRiesgoEspalda>();
            this.tblMovimientoSignosVitales = new HashSet<tblMovimientoSignosVitales>();
            this.tblConsultaXArticulaciones = new HashSet<tblConsultaXArticulaciones>();
            this.tblConsultaXGrupoMuscularXRetracciones = new HashSet<tblConsultaXGrupoMuscularXRetracciones>();
            this.tblConsultaXSegmentosXVistas = new HashSet<tblConsultaXSegmentosXVistas>();
            this.tblMovimientoComplementoNutricional = new HashSet<tblMovimientoComplementoNutricional>();
            this.tblMovimientoHoraAlimentacion = new HashSet<tblMovimientoHoraAlimentacion>();
            this.tblMovimientoMedicamento = new HashSet<tblMovimientoMedicamento>();
        }
    
        public int intPkConsulta { get; set; }
        public int intNumeroConsulta { get; set; }
        public string strIdentificacionEmpleado { get; set; }
        public string strIdentificacionPaciente { get; set; }
        public System.DateTime datFechaCita { get; set; }
        public System.TimeSpan timHoraCita { get; set; }
        public string strNombreAcompanante { get; set; }
        public string strTelefonoAcompanante { get; set; }
        public string strParentescoAcompanante { get; set; }
        public int intFkTipoConsulta { get; set; }
        public string strEnfermedadActual { get; set; }
        public string strRecomendaciones { get; set; }
        public Nullable<int> intDiasSemana { get; set; }
        public string strTiempoDiario { get; set; }
        public string strRangos { get; set; }
        public int intRemitidoEspecialidad { get; set; }
        public Nullable<int> intDiasProximaCita { get; set; }
        public Nullable<int> intNumeroSesionFisio { get; set; }
        public Nullable<int> intNumeroSesionNutri { get; set; }
        public bool bitFuma { get; set; }
        public string strFrecuenciaFuma { get; set; }
        public bool bitLicor { get; set; }
        public string strFrecuenciaLicor { get; set; }
        public string strHoraLevantada { get; set; }
        public string strHoraAcuesta { get; set; }
        public string strHoraEntrena { get; set; }
        public string strTiempoEntrenamiento { get; set; }
        public string strAlimentosNoConsume { get; set; }
        public string strAlimentosConsume { get; set; }
        public string strValoracionFisio { get; set; }
        public string strEvolucionFisio { get; set; }
        public bool bitSesionPositiva { get; set; }
        public string strResultadoExamenes { get; set; }
        public string strObservacion { get; set; }
        public int intClasificacionUsuario { get; set; }
        public int intIdSucursal { get; set; }
        public int intEmpresa { get; set; }
        public bool bitActivo { get; set; }
        public string strUsuarioCreacion { get; set; }
        public System.DateTime datFechaCreacion { get; set; }
        public string strUsuarioModificacion { get; set; }
        public Nullable<System.DateTime> datFechaModificacion { get; set; }
        public Nullable<int> intIdEspecialidadEmpleado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblExamenFisico> tblExamenFisico { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoAntecedentesDeportivos> tblMovimientoAntecedentesDeportivos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoAntecedentesFamiliares> tblMovimientoAntecedentesFamiliares { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoAntecedentesOcupacionales> tblMovimientoAntecedentesOcupacionales { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoAntecedentesPersonales> tblMovimientoAntecedentesPersonales { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoDiagnosticos> tblMovimientoDiagnosticos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoEvaluacionPorSistemas> tblMovimientoEvaluacionPorSistemas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoPerimetros> tblMovimientoPerimetros { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoPliegues> tblMovimientoPliegues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoRiesgoCaderaInferior> tblMovimientoRiesgoCaderaInferior { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoRiesgoCaderaSuperior> tblMovimientoRiesgoCaderaSuperior { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoRiesgoEspalda> tblMovimientoRiesgoEspalda { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoSignosVitales> tblMovimientoSignosVitales { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblConsultaXArticulaciones> tblConsultaXArticulaciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblConsultaXGrupoMuscularXRetracciones> tblConsultaXGrupoMuscularXRetracciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblConsultaXSegmentosXVistas> tblConsultaXSegmentosXVistas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoComplementoNutricional> tblMovimientoComplementoNutricional { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoHoraAlimentacion> tblMovimientoHoraAlimentacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMovimientoMedicamento> tblMovimientoMedicamento { get; set; }
    }
}
