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
    
    public partial class MedicalConsultationDetailEvaluator
    {
        public int MedicalConsultationID { get; set; }
        public Nullable<int> TypeConsultationID { get; set; }
        public Nullable<int> ConsultationPurposeID { get; set; }
        public Nullable<int> ExternalCauseID { get; set; }
        public string CurrentDisease { get; set; }
        public string EvaluationBySystemsRecommendations { get; set; }
        public Nullable<int> TrainingDaysForWeek { get; set; }
        public string DailyTimeTraining { get; set; }
        public string TrainingRange { get; set; }
        public Nullable<int> ReferredTo { get; set; }
        public Nullable<int> NextAppointmentDays { get; set; }
        public Nullable<int> PatientType { get; set; }
        public Nullable<double> ActualWeight { get; set; }
        public Nullable<int> ActualHeight { get; set; }
        public Nullable<double> GoalFatPercentage { get; set; }
        public Nullable<int> GoalWeeklyTrainingDays { get; set; }
        public Nullable<int> GoalWeeklyTrainingMinutes { get; set; }
        public Nullable<double> WeightGoal { get; set; }
        public Nullable<double> HeartRate { get; set; }
        public Nullable<double> DiastolicBloodPressure { get; set; }
        public Nullable<double> SystolicBloodPressure { get; set; }
        public Nullable<double> Temperature { get; set; }
        public Nullable<double> MeanArterialPressure { get; set; }
        public string ClassificationAHA { get; set; }
        public Nullable<double> DoubleProduct { get; set; }
        public Nullable<double> SubscapularFold { get; set; }
        public Nullable<double> TricipitalFold { get; set; }
        public Nullable<double> AxillaryFold { get; set; }
        public Nullable<double> BicipitalFold { get; set; }
        public Nullable<double> BreastFold { get; set; }
        public Nullable<double> AbdomenFold { get; set; }
        public Nullable<double> SuprailiacFold { get; set; }
        public Nullable<double> MiddleSuprailiacFold { get; set; }
        public Nullable<double> LateralSuprailiacFold { get; set; }
        public Nullable<double> SupraspinalFold { get; set; }
        public Nullable<double> IleocrestalFold { get; set; }
        public Nullable<double> ThighCrush { get; set; }
        public Nullable<double> LegFold { get; set; }
        public Nullable<double> PerimeterRelaxedArm { get; set; }
        public Nullable<double> PerimeterTenseArm { get; set; }
        public Nullable<double> ToraxPerimeter { get; set; }
        public Nullable<double> AbdomenPerimeter { get; set; }
        public Nullable<double> WaistPerimeter { get; set; }
        public Nullable<double> HipPerimeter { get; set; }
        public Nullable<double> PerimeterUpperThigh { get; set; }
        public Nullable<double> MiddleThighPerimeter { get; set; }
        public Nullable<double> LegPerimeter { get; set; }
        public Nullable<double> Faulkner { get; set; }
        public Nullable<double> AdiposityIndex { get; set; }
        public Nullable<double> FatPercentageDurninAndWomersley { get; set; }
        public Nullable<double> BodyDensityDurninAndWomersley { get; set; }
        public Nullable<double> Yuhasz { get; set; }
        public Nullable<double> FattyWeightYuhasz { get; set; }
        public Nullable<double> MagroWeightYuhasz { get; set; }
        public Nullable<double> FatPercentageYuhasz { get; set; }
        public Nullable<double> FatPercentageJacksonAndPollock { get; set; }
        public Nullable<double> BodyDensityJacksonAndPollock { get; set; }
        public Nullable<double> ProportionalWeight { get; set; }
        public Nullable<double> MuscleMassPercentage { get; set; }
        public Nullable<double> HipWaistRelationship { get; set; }
        public Nullable<double> IMC { get; set; }
        public Nullable<double> VisceralFatPercentage { get; set; }
        public Nullable<double> KBasalCalories { get; set; }
        public Nullable<int> SomatotypeID { get; set; }
        public Nullable<bool> TestAdams { get; set; }
        public string TestAdamsObservation { get; set; }
        public string AbdominalStrength { get; set; }
        public string AbdominalStrengthObservation { get; set; }
        public string CoreRating { get; set; }
        public string CoreRatingObservation { get; set; }
        public string BasicSquat { get; set; }
        public string BasicSquatObservation { get; set; }
        public string DeepSquat { get; set; }
        public string DeepSquatObservation { get; set; }
        public Nullable<double> TotalScoreBackRisk { get; set; }
        public string RecommendationBackRisk { get; set; }
        public Nullable<double> TotalScoreLowerKineticHipRisk { get; set; }
        public string RecommendationLowerKineticHipRisk { get; set; }
        public Nullable<double> TotalScoreHipRiskHigherKinetics { get; set; }
        public string RecommendationHipRiskHigherKinetics { get; set; }
        public string ExamResults { get; set; }
        public string DiagnosticObservations { get; set; }
        public string DiagnosticRecommendations { get; set; }
        public string NumberSession { get; set; }
        public string Valoration { get; set; }
        public string Evolution { get; set; }
        public Nullable<bool> PositiveInjury { get; set; }
        public Nullable<bool> Smoker { get; set; }
        public string SmokerFrequency { get; set; }
        public Nullable<bool> Drinker { get; set; }
        public string FrequencyDrinker { get; set; }
        public string NonPreferredFoods { get; set; }
        public string FavoriteFood { get; set; }
        public string WakeUp { get; set; }
        public string Sleep { get; set; }
        public string Hours { get; set; }
        public Nullable<bool> Suitable { get; set; }
        public string HourTraining { get; set; }
        public Nullable<bool> PositiveSession { get; set; }
        public string ConsultationPurposeObservation { get; set; }
        public Nullable<int> TrainingFrequencyAssistance { get; set; }
        public Nullable<int> TrainingFrequencyTime { get; set; }
        public Nullable<bool> AdditionalTrainingActivitiesCardio { get; set; }
        public Nullable<bool> AdditionalTrainingActivitiesWeight { get; set; }
        public Nullable<bool> AdditionalTrainingActivitiesGroupClasses { get; set; }
        public string AdditionalTrainingActivitiesOther { get; set; }
        public Nullable<bool> Sedentary { get; set; }
        public string BecauseSedentary { get; set; }
        public Nullable<int> BoneStructureID { get; set; }
        public string BoneStructureObservation { get; set; }
        public Nullable<bool> FeelPain { get; set; }
        public Nullable<bool> FeelPainConstant { get; set; }
        public string PainLocation { get; set; }
        public string ObservationOther { get; set; }
    
        public virtual BoneStructure BoneStructure { get; set; }
        public virtual ConsultationPurpose ConsultationPurpose { get; set; }
        public virtual ExternalCause ExternalCause { get; set; }
        public virtual MedicalConsultation MedicalConsultation { get; set; }
        public virtual ModuleMedicalModule ModuleMedicalModule { get; set; }
        public virtual tblSomatotipo tblSomatotipo { get; set; }
        public virtual TypeConsultation TypeConsultation { get; set; }
    }
}
