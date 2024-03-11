using Sibo.WhiteList.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
   public class ActionDAL
    {

        public List<tblAction> GetAction(int gymId, string branchId, string IpAddres)
        {

            List<tblAction> list = new List<tblAction>();
            string[] ListabranchId = branchId.Split(',');

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                int sucursal = 0;
                for (int i = 0; i < ListabranchId.Length; i++)
                {
                    sucursal = Convert.ToInt32(ListabranchId[i]);

                    var listt= (from ter in context.tblAction
                                        where ter.cdgimnasio == gymId && ter.intIdSucursales == sucursal && ter.ipAddress == IpAddres && ter.actionState == true
                                        select ter).ToList();

                    foreach (var item in listt)
                    {
                        list.Add(item);
                    }

                }

            }

            return list;

        }

        public bool GetActionUpdate(int id, string ipAddress, string intIdSucursales, int cdgimndasio)
        {
            try
            {               
                string[] ListabranchId = intIdSucursales.Split(',');

                using (var context = new dbWhiteListModelEntities(cdgimndasio))
                {
                    try
                    {
                        tblAction consulta = new tblAction();

                        consulta = context.tblAction.Where(a => a.id == id && a.cdgimnasio == cdgimndasio).FirstOrDefault();
                        if (consulta != null)
                        {
                            consulta.actionState = true;

                            context.tblAction.Add(consulta);
                            context.SaveChanges();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                        
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    
                }

            }
            catch (Exception)
            {
                return false;
            }

          

        }

        public int InsertAction(int gymId, string branchId, string IpAddres, string enrolar)
        {
            int idIdentity = 0;
            try
            {

                tblAction action = new tblAction()
                {
                    ipAddress = IpAddres,
                    strAction = enrolar,
                    actionDate = DateTime.Now,
                    used = false,
                    actionState  = false,
                    modifiedDate = DateTime.Now,
                    cdgimnasio = gymId,
                    intIdSucursales = Convert.ToInt32(branchId)

                };

                using (var context = new dbWhiteListModelEntities(gymId))
                {
                    context.tblAction.Add(action);
                    context.SaveChanges();
                    idIdentity = context.tblAction.Local[0].id;
                }

                return idIdentity;
            }
            catch (Exception)
            {

                return 0;
            }

        }

        public bool Insert(eActionParameters aParameters)
        {
           
            int resp = 0;

            try
            {
                tblActionParameters actionParameters = new tblActionParameters()
                {
                    actionId = aParameters.actionId,
                    parameterName = aParameters.parameterName,
                    parameterValue = aParameters.parameterValue,
                    dateParameter = DateTime.Now
                };

                using (var context = new dbWhiteListModelEntities(aParameters.gymId))
                {
                    context.tblActionParameters.Add(actionParameters);
                    context.SaveChanges();
                    resp = context.tblActionParameters.Local[0].id;
                }

                if (resp > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
   
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
