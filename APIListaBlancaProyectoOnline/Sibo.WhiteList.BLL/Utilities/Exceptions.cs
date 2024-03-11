using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.BLL.Utilities
{
    public class Exceptions
    {
        public const string nullGym = "El código del gimnasio no fue enviado de forma correcta.";
        public const string nullBranch = "El código de la sucursal no fue enviado de forma correcta.";
        public const string nullPlan = "El código del plan no fue enviado de forma correcta.";
        public const string nullStringList = "No se enviaron registros para actualizar la lista blanca, la lista está vacía o es nula.";
        public const string nullEntryList = "No se enviaron entradas para insertar, la lista está vacía o es nula.";
        public const string nullFingerprintList = "No se enviaron huellas para insertar o actualizar, la lista está vacía o es nula.";
        public const string nullId = "La identificación no fue enviada de forma correcta.";
        public const string serverError = "Ha ocurrido un error en el sistema, por favor intente de nuevo o contacte al administrador";
        public const string NotClient = "La persona que desea descargar no es un cliente activo del gimnasio.";
        public const string NotVigentPlan = "La persona que desea descargar no tiene plan vigente en el gimnasio.";
        public const string NotPlan = "La persona que desea descargar tiene plan vigente pero no se puede determinar cual es, por favor verifique la factura.";
        public const string NotEntry = "El plan que usted tiene vigente no permite acceso a esta sucursal.";
        public const string existFingerprint = "La huella ya se encuentra descargada en la sucursal deseada.";
        public const string nullVisitorData = "Los datos del visitante no se enviaron de forma correcta.";
    }
}
