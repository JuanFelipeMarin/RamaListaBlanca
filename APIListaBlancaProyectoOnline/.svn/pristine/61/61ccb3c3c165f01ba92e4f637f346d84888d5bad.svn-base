using Sibo.WhiteList.BLL.Utilities;
using Sibo.WhiteList.DAL.Classes;
using Sibo.WhiteList.DAL;
using Sibo.WhiteList.Classes;
using System;
using System.Collections.Generic;

namespace Sibo.WhiteList.BLL.Classes
{
    public class VisitorBLL
    {
        /// <summary>
        /// Método para insertar los visitantes.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public bool InsertVisitor(eVisitor visitor)
        {
            VisitorDAL visitorDAL = new VisitorDAL();

            if (visitor != null && !string.IsNullOrEmpty(visitor.visitorId) && visitor.gymId > 0)
            {
                return visitorDAL.InsertVisitor(GetTableEntity(visitor));
            }
            else
            {
                throw new Exception(Exceptions.nullVisitorData);
            }
        }

        private bool UpdateVisitor(Visitors visitor)
        {
            VisitorDAL visDAL = new VisitorDAL();

            if (visitor != null && !string.IsNullOrEmpty(visitor.vis_strVisitorId) && visitor.cdgimnasio > 0)
            {
                visitor.bits_replica = "111111111111111111111111111111";
                visitor.vis_bitModified = true;
                return visDAL.UpdateVisitor(visitor);
            }
            else
            {
                throw new Exception(Exceptions.nullVisitorData);
            }
        }

        /// <summary>
        /// Método que permite consultar todos los datos de necesarios para crear un nuevo visitante y/o visita.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public eVisitorData GetDataToVisitor(int gymId, string visitorId)
        {
            EmployeeBLL empBll = new EmployeeBLL();
            MasterDataBLL masterBll = new MasterDataBLL();
            eVisitorData visitorEntity = new eVisitorData();
            List<eGeneric> generic = new List<eGeneric>();

            if (gymId == 0 || string.IsNullOrEmpty(gymId.ToString()))
            {
                throw new Exception(Exceptions.nullGym);
            }

            if (string.IsNullOrEmpty(visitorId))
            {
                throw new Exception(Exceptions.nullId);
            }

            //Si el visitante ya está registrado para el gimnasio, se llena la entidad de este para mostrar los datos en la pantalla de visitantes del ingreso.
            visitorEntity.visitor = ValidateExistVisitor(gymId, visitorId);

            //Consultamos la lista de empleados del gimnasio y los agregamos a la entidad de respuesta.
            generic = empBll.GetActiveEmployees(gymId);
            visitorEntity.whoAuthorized = generic;
            visitorEntity.visitedPerson = generic;
            //Consultamos la lista de tipos de identificación del gimnasio y los agregamos a la entidad de respuesta.
            generic = masterBll.GetIdType(gymId);
            visitorEntity.idType = generic;
            //Consultamos la lista de eps del gimnasio y los agregamos a la entidad de respuesta.
            generic = masterBll.GetEPS(gymId);
            visitorEntity.eps = generic;

            return visitorEntity;
        }

        public DAL.WhiteList InsertUpdateVisitor(eVisitor visitor)
        {
            VisitorDAL visDAL = new VisitorDAL();
            Visitors visEntity = new Visitors();
            WhiteListBLL wlBll = new WhiteListBLL();
            DAL.WhiteList response = new DAL.WhiteList();
            bool resp = false;

            if (visitor != null && !string.IsNullOrEmpty(visitor.visitorId))
            {
                if (visitor.idPK == 0)
                {
                    if (InsertVisitor(visitor))
                    {
                        if (InsertVisit(visitor))
                        {
                            resp = true;
                        }
                    }
                }
                else
                {
                    visEntity = visDAL.GetVisitorById(visitor.gymId, visitor.idPK);

                    if (visEntity != null && !string.IsNullOrEmpty(visEntity.vis_strVisitorId))
                    {
                        visEntity.vis_dtmDateBorn = visitor.bornDate;
                        visEntity.vis_dtmRegisterDate = DateTime.Now;
                        visEntity.vis_EntryFingerprint = visitor.entryWithFingerprint;
                        visEntity.vis_intEPS = visitor.eps;
                        visEntity.vis_intGenre = visitor.genre;
                        visEntity.vis_intTypeId = visitor.idType;
                        visEntity.vis_ModifiedDate = DateTime.Now;
                        visEntity.vis_ModifiedUser = visitor.userId;
                        visEntity.vis_strAddress = visitor.address;
                        visEntity.vis_strEmail = visitor.email;
                        visEntity.vis_strFirstLastName = visitor.firstLastName;
                        visEntity.vis_strName = visitor.name;
                        visEntity.vis_strPhone = visitor.phone;
                        visEntity.vis_strSecondLastName = visitor.secondLastName;
                        visEntity.vis_strVisitorId = visitor.visitorId;

                        if (UpdateVisitor(visEntity))
                        {
                            if (InsertVisit(visitor))
                            {
                                resp = true;
                            }
                        }
                    }
                    else
                    {
                        if (InsertVisitor(visitor))
                        {
                            if (InsertVisit(visitor))
                            {
                                resp = true;
                            }
                        }
                    }
                }

                if (resp)
                {
                    response = wlBll.GetUserById(visitor.gymId, visitor.visitorId, "Visitante");

                    if (response != null && response.intPkId > 0)
                    {
                        response.personState = "Enviado";
                        wlBll.Update(response);
                    }
                }

                return response;
            }
            else
            {
                return null;
            }
        }

        public Visitors GetVisitorByVisitorId(int gymId, string id)
        {
            VisitorDAL visDAL = new VisitorDAL();
            return visDAL.GetVisitorByVisitorId(gymId, id);
        }

        /// <summary>
        /// Método por medio del cual será insertada la visita.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        private bool InsertVisit(eVisitor visitor)
        {
            VisitorDAL visDAL = new VisitorDAL();

            Visit visit = new Visit()
            {
                bitModified = true,
                bits_replica = "111111111111111111111111111111",
                Branch = visitor.branchId,
                cdgimnasio = visitor.gymId,
                CreatedDate = DateTime.Now,
                CreatedUser = visitor.userId,
                DateVisit = DateTime.Now,
                ElementsIn = visitor.visit.elements,
                Id = visDAL.GetNextIdToVisit(visitor.gymId),
                VisitedPerson = visitor.visit.visitedPerson,
                VisitorId = visitor.visitorId,
                VisitReason = visitor.visit.reason,
                VisitTime = visitor.visit.time,
                WhoAuthorized = visitor.visit.whoAuthorized
            };

            return visDAL.InsertVisit(visit);
        }

        /// <summary>
        /// Método que valida si un visitante existe en la base de datos.
        /// Getulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        private eVisitor ValidateExistVisitor(int gymId, string visitorId)
        {
            Visitors visEntity = new Visitors();
            VisitorDAL visDAL = new VisitorDAL();

            //Consultamos si el visitante a crear ya existe en la BD.
            visEntity = visDAL.GetVisitorByVisitorId(gymId, visitorId);

            if (visEntity != null && !string.IsNullOrEmpty(visEntity.vis_strVisitorId))
            {
                eVisitor response = new eVisitor()
                {
                    address = visEntity.vis_strAddress,
                    bornDate = visEntity.vis_dtmDateBorn,
                    email = visEntity.vis_strEmail,
                    entryWithFingerprint = Convert.ToBoolean(visEntity.vis_EntryFingerprint ?? false),
                    eps = Convert.ToInt32(visEntity.vis_intEPS ?? 0),
                    firstLastName = visEntity.vis_strFirstLastName,
                    genre = Convert.ToInt32(visEntity.vis_intGenre ?? 0),
                    idPK = visEntity.vis_intPK,
                    visitorId = visEntity.vis_strVisitorId,
                    idType = visEntity.vis_intTypeId,
                    name = visEntity.vis_strName,
                    phone = visEntity.vis_strPhone,
                    secondLastName = visEntity.vis_strSecondLastName
                };

                return response;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Método para convertir la entidad eVisitor a la entidad de la tabla Visitors.
        /// GEetulio Vargas - 2018-08-30 - OD 1307
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        private Visitors GetTableEntity(eVisitor visitor)
        {
            VisitorDAL visitorDAL = new VisitorDAL();
            Visitors visitorSave = new Visitors()
            {
                bits_replica = "111111111111111111111111111111",
                cdgimnasio = visitor.gymId,
                vis_bitModified = true,
                vis_CreatedUser = visitor.userId,
                vis_dtmDateBorn = visitor.bornDate,
                vis_dtmRegisterDate = DateTime.Now,
                vis_EntryFingerprint = visitor.entryWithFingerprint,
                vis_intBranch = visitor.branchId,
                vis_intEPS = visitor.eps,
                vis_intGenre = visitor.genre,
                vis_intPK = visitorDAL.GetNextId(visitor.gymId),
                vis_intTypeId = visitor.idType,
                vis_strAddress = visitor.address,
                vis_strEmail = visitor.email,
                vis_strFirstLastName = visitor.firstLastName,
                vis_strName = visitor.name,
                vis_strPhone = visitor.phone,
                vis_strSecondLastName = visitor.secondLastName,
                vis_strVisitorId = visitor.visitorId
            };

            return visitorSave;
        }
    }
}
