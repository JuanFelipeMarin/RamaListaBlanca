using Sibo.WhiteList.DAL.Classes;
using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;

namespace Sibo.WhiteList.DAL
{
    public class clsConexion
    {
        public static DbConnection GetConnection(int cdgimnasio)
        {
            ConnectionDAL connectionDAL = new ConnectionDAL();
            string defaultConnection = ConfigurationManager.ConnectionStrings["strConexionGSW"]?.ToString();
            bool ambienteDesarrollo = ConfigurationManager.AppSettings["bitAmbienteDLLO"]?.ToUpper() == "TRUE";
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            string cadenaConexion;
                
            try
            {
                if (ambienteDesarrollo && !string.IsNullOrEmpty(defaultConnection))
                {
                    defaultConnection = defaultConnection.Replace("metadata=res://*/dbWhiteListModel.csdl|res://*/dbWhiteListModel.ssdl|res://*/dbWhiteListModel.msl;provider=System.Data.SqlClient;provider connection string=", "").Replace("MultipleActiveResultSets=True;App=EntityFramework", "");
                    cadenaConexion = defaultConnection.Replace("\"", "");
                }
                else
                {
                    cadenaConexion = connectionDAL.decrypt(connectionDAL.GetConnection(cdgimnasio));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la cadena de conexión", ex);
            }


            entityBuilder.Provider = "System.Data.SqlClient";
            entityBuilder.ProviderConnectionString = cadenaConexion;
            entityBuilder.Metadata = "res://*/";

            return new EntityConnection(entityBuilder.ToString());
        }

        private static string GetConnectionWithoutMetadata(string connectionString)
        {
            string metadataPrefix = "metadata=";
            string metadataSuffix = "MultipleActiveResultSets=True;App=EntityFramework";
            int metadataStartIndex = connectionString.IndexOf(metadataPrefix);
            int metadataEndIndex = connectionString.IndexOf(metadataSuffix) + metadataSuffix.Length;
            return connectionString.Remove(metadataStartIndex, metadataEndIndex - metadataStartIndex);
        }
    }
}
