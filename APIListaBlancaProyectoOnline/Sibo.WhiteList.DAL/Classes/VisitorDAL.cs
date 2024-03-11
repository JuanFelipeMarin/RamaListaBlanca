using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Classes
{
    public class VisitorDAL
    {
        #region Visitor
        /// <summary>
        /// Método para insertar visitantes.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public bool InsertVisitor(Visitors visitor)
        {
            int resp = 0;

            using (var context = new dbWhiteListModelEntities(visitor.cdgimnasio))
            {
                context.Set<Visitors>().Add(visitor);
                resp = context.SaveChanges();
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

        /// <summary>
        /// Método para actualizar los visitantes
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public bool UpdateVisitor(Visitors visitor)
        {
            int resp = 0;

            using (var context = new dbWhiteListModelEntities(visitor.cdgimnasio))
            {
                context.Entry(visitor).State = System.Data.Entity.EntityState.Modified;
                resp = context.SaveChanges();
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

        /// <summary>
        /// Método para consultar el siguiente id que se debe guardar en la tabla Visitors.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        public int GetNextId(int gymId)
        {
            List<int> intList = new List<int>();
            int resp = 0;

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                intList = (from vis in context.Visitors
                           where vis.cdgimnasio == gymId
                           orderby vis.vis_intPK descending
                           select vis.vis_intPK).ToList();

                if (intList != null && intList.Count > 0)
                {
                    resp = intList.Max() + 1;
                }
                else
                {
                    resp = 1;
                }
            }

            return resp;
        }

        /// <summary>
        /// Método que consulta un visitante de un gimnasio específico partiendo de su número de identificación.
        /// Getulio Vargas - 2018-08-30
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public Visitors GetVisitorByVisitorId(int gymId, string visitorId)
        {
            Visitors response = new Visitors();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                response = context.Visitors.FirstOrDefault(vis => vis.cdgimnasio == gymId && vis.vis_strVisitorId == visitorId);
            }

            return response;
        }

        /// <summary>
        /// Método que consulta un visitante de un gimnasio específico, partiendo del id del visitante.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="idPK"></param>
        /// <returns></returns>
        public Visitors GetVisitorById(int gymId, int idPK)
        {
            Visitors response = new Visitors();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                response = context.Visitors.FirstOrDefault(vis => vis.cdgimnasio == gymId && vis.vis_intPK == idPK);
            }

            return response;
        } 
        #endregion

        #region Visit
        /// <summary>
        /// Método encargado de consultar el siguiente id a insertar de la tabla Visit en un gimnasio específico.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        public int GetNextIdToVisit(int gymId)
        {
            List<int> intList = new List<int>();
            int resp = 0;

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                intList = (from visit in context.Visit
                           where visit.cdgimnasio == gymId
                           orderby visit.Id descending
                           select visit.Id).ToList();

                if (intList != null && intList.Count > 0)
                {
                    resp = intList.Max() + 1;
                }
                else
                {
                    resp = 1;
                }
            }

            return resp;
        }

        /// <summary>
        /// Método encargado de insertar una visita.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="visit"></param>
        /// <returns></returns>
        public bool InsertVisit(Visit visit)
        {
            int resp = 0;

            using (var context = new dbWhiteListModelEntities(visit.cdgimnasio))
            {
                context.Set<Visit>().Add(visit);
                resp = context.SaveChanges();
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
        #endregion
    }
}
