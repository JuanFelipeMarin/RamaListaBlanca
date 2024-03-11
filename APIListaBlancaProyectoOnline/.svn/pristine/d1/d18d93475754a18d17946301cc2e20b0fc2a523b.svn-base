using Sibo.WhiteList.Classes;
using Sibo.WhiteList.DAL.SiboPawHelpers;
using Sibo.WhiteList.DAL.wsSiboPawDAL;
using System;

namespace Sibo.WhiteList.DAL.Classes
{
    public class ConnectionDAL
    {
        public string GetConnection(int cdgimnasio)
        {
            wsSiboPawSoapClient ws = new wsSiboPawSoapClient();
            CompanyDAL companyBll = new CompanyDAL();
            eCompany company = companyBll.get(cdgimnasio);

            if (company == null)
            {
                throw new Exception("No se encontró información de la compañía");
            }

            eConnection con;

            try
            {
                con = ws.getConnectionByCompanyApplication(company.companyIDSiboPaw, company.applicationID);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la cadena de conexión", ex);
            }

            if (con == null)
            {
                throw new Exception("No se encontró cadena de conexión");
            }

            string server = decrypt(con.server);
            string database = decrypt(con.dataBase);
            string userDatabase = decrypt(con.userDataBase);
            string passwordDatabase = decrypt(con.passwordDataBase);
            string connectionString = "Data Source=" + server + ";Initial Catalog=" + database + ";User ID=" + userDatabase + ";Password=" + passwordDatabase + ";";
            return encrypt(connectionString);
        }

        public string decrypt(string connectionString)
        {
            dEncriptaDesdencripta decrypt = new dEncriptaDesdencripta();
            return decrypt.Desencripta(connectionString);
        }

        public string encrypt(string connectionString)
        {
            dEncriptaDesdencripta encrypt = new dEncriptaDesdencripta();
            return encrypt.Encripta(connectionString);
        }
    }
}
