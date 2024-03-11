namespace Sibo.WhiteList.Service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.SiboWhiteListServiceInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.SiboWhiteListService = new System.ServiceProcess.ServiceInstaller();
            // 
            // SiboWhiteListServiceInstaller
            // 
            this.SiboWhiteListServiceInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.SiboWhiteListServiceInstaller.Password = null;
            this.SiboWhiteListServiceInstaller.Username = null;
            // 
            // SiboWhiteListService
            // 
            this.SiboWhiteListService.Description = "Servicio encargado de realizar la comunicación con la API de GSW para lista blanc" +
    "a y de la comunicación con las terminales conectadas para el ingreso de los usua" +
    "rios al gimnasio.";
            this.SiboWhiteListService.DisplayName = "SiboWhiteListService V_1.0.0";
            this.SiboWhiteListService.ServiceName = "SiboWhiteListService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.SiboWhiteListServiceInstaller,
            this.SiboWhiteListService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller SiboWhiteListServiceInstaller;
        private System.ServiceProcess.ServiceInstaller SiboWhiteListService;
    }
}