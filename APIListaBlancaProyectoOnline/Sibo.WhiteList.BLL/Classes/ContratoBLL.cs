using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.Classes;
using Sibo.WhiteList.Service.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.DAL;

namespace Sibo.WhiteList.BLL.Classes
{
    public class ContratoBLL
    {
        private bool Insert(eContratos contrato)
        {
            ContractDAL contratoDal = new ContractDAL();
            return contratoDal.Insert(contrato);
        }

        public void Process(eContratos contrato)
        {
            WhiteListDAL wlDAL = new WhiteListDAL();
            gim_detalle_contrato detContrato = new gim_detalle_contrato();
            ContractDAL fingerDAL = new ContractDAL();
            //detContrato = fingerDAL.GetContrato(contrato.gymId, contrato.personId, contrato.id);

            //if (detContrato != null)
            //{
            //    fingerDAL.Update(detContrato, contrato);
            //}
            //else
            //{
            //    Insert(contrato);
            //}
            Insert(contrato);

            //wlDAL.UpdateFingerprint(fingerEntity.gymId, fingerEntity.personId, fingerEntity.id, fingerEntity.fingerPrint);
        }

        public bool ValidarContrato(eValidarContrato datos)
        {
            ContractDAL contratoDal = new ContractDAL();
            return contratoDal.ValidarContrato(datos.gymId, datos.branchId,datos.userId);

        }
        public bool ValidarContratoFirmadoPorPlan(eValidarContrato datos)
        {
            ContractDAL contratoDal = new ContractDAL();
            return contratoDal.ValidarContratoPorPlan(datos.gymId, datos.branchId, datos.userId,datos.PlanId);

        }
    }
}
