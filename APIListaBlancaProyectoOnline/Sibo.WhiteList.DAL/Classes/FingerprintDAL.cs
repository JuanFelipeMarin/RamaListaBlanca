﻿using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Classes;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;

namespace Sibo.WhiteList.DAL.Classes
{
    public class FingerprintDAL
    {
        /// <summary>
        /// Método que permite consultar la huella de un usuario específico, incluyendo también como parámetro el id de la huella.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="clientId"></param>
        /// <param name="fingerprintId"></param>
        /// <returns></returns>
        public gim_huellas GetFingerprint(int gymId, string clientId, int fingerprintId)
        {
            gim_huellas fp = new gim_huellas();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                fp = context.gim_huellas.FirstOrDefault(f => f.cdgimnasio == gymId && f.hue_identifi == clientId && f.hue_id == fingerprintId);
                return fp;
            }
        }

        /// <summary>
        /// Método que permite consultar la huella de un usuario específico.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public gim_huellas GetFingerprint(int gymId, string clientId)
        {
            gim_huellas fp = new gim_huellas();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                fp = context.gim_huellas.FirstOrDefault(f => f.cdgimnasio == gymId && f.hue_identifi == clientId);
                return fp;
            }
        }

        /// <summary>
        /// Método que permite consultar las huellas asociadas a un cliente específico
        /// Getulio Vargas - 2019-04-29
        /// </summary>
        /// <param name="gymId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<eFingerprint> GetFingerprintsByClient(int gymId, string clientId)
        {
            List<eFingerprint> responseList = new List<eFingerprint>();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                responseList = (from h in context.gim_huellas
                                where h.cdgimnasio == gymId && h.hue_identifi == clientId
                                select new eFingerprint
                                {
                                    branchId = h.intfkSucursal.ToString(),
                                    finger = Convert.ToInt32(h.hue_dedo ?? 0),
                                    fingerPrint = h.hue_dato,
                                    gymId = h.cdgimnasio,
                                    id = h.hue_id,
                                    personId = h.hue_identifi,
                                    quality = Convert.ToInt32(h.hue_calidad ?? 0)
                                }).ToList();
            }

            return responseList;
        }

        public List<eFingerprint> GetFingerPrintsByClientIdPart(int gymId, string clientIdPart)
        {
            List<eFingerprint> responseList = new List<eFingerprint>();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                responseList = (from h in context.gim_huellas
                                where h.cdgimnasio == gymId && h.hue_identifi.Substring(h.hue_identifi.Length - 3, 3) == clientIdPart.PadLeft(3, '0')
                                select new eFingerprint
                                {
                                    branchId = h.intfkSucursal.ToString(),
                                    finger = Convert.ToInt32(h.hue_dedo ?? 0),
                                    fingerPrint = h.hue_dato,
                                    gymId = h.cdgimnasio,
                                    id = h.hue_id,
                                    personId = h.hue_identifi,
                                    quality = Convert.ToInt32(h.hue_calidad ?? 0)
                                }).ToList();
            }

            return responseList;
        }

        /// <summary>
        /// Método encargado de insertar o actualizar la huella de un cliente en la BD de GSW. Por medio de una lista de huellas.
        /// Getulio Vargas - 2018-08-22 - OD 1307
        /// </summary>
        /// <param name="fingerprintList">Como parámetro recibe una lista de huellas</param>
        /// <returns>Retorna el listado de huellas actualizadas/returns>
        public List<eFingerprint> AddUpdateFingerprint(List<eFingerprint> fingerprintList)
        {
            WhiteListDAL wlDAL = new WhiteListDAL();
            List<eFingerprint> responseList = new List<eFingerprint>();
            bool resp = false;

            foreach (eFingerprint item in fingerprintList)
            {
                gim_huellas fingerprint = new gim_huellas();

                wlDAL.UpdateFingerprint(item.gymId, item.personId, item.id, item.fingerPrint);
                fingerprint = GetFingerprint(item.gymId, item.personId, item.id);

                if (fingerprint != null)
                {
                    resp = Update(fingerprint, item);
                }
                else
                {
                    resp = Insert(item);
                }

                if (resp)
                {
                    responseList.Add(item);
                }
            }

            return responseList;
        }

        /// <summary>
        /// Método encargado de insertar las huellas en la tabla gim_huellas.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Insert(eFingerprint item)
        {
            int resp = 0;
            gim_huellas fingerprint = new gim_huellas()
            {
                bits_replica = "111111111111111111111111111111",
                bool_modificado_replica = true,
                cdgimnasio = item.gymId,
                hue_calidad = item.quality,
                hue_dato = item.fingerPrint,
                hue_dedo = item.finger,
                hue_id = GetNextId(item.gymId),
                hue_identifi = item.personId,
                hue_modificado = 1,
                intfkSucursal = Convert.ToInt32(item.branchId)
            };

            using (var context = new dbWhiteListModelEntities(item.gymId))
            {
                context.Set<gim_huellas>().Add(fingerprint);
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
        /// Método encargado de actualizar las huellas en la tabla gim_huellas.
        /// Getuluio Vargas - OD 1307
        /// </summary>
        /// <param name="fingerprint"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Update(gim_huellas fingerprint, eFingerprint item)
        {
            int resp = 0;
            fingerprint.hue_calidad = item.quality;
            fingerprint.hue_dato = item.fingerPrint;
            //fingerprint.hue_id = item.id;
            fingerprint.hue_identifi = item.personId;
            fingerprint.intfkSucursal = Convert.ToInt32(item.branchId);
            fingerprint.bits_replica = "111111111111111111111111111111";

            using (var context = new dbWhiteListModelEntities(item.gymId))
            {
                context.Entry(fingerprint).State = EntityState.Modified;
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
        /// Método encargado de consultar el último id de huella guardado en un gimnasio específico.
        /// Getulio Vargas - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        public int GetNextId(int gymId)
        {
            List<int> intList = new List<int>();
            gim_huellas hue = new gim_huellas();
            int resp = 0;

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                intList = (from finger in context.gim_huellas
                           where finger.cdgimnasio == gymId
                           orderby finger.hue_id descending
                           select finger.hue_id).ToList();

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
        /// Método encargado de consultar las huellas de un gimnasio específico.
        /// Getulio Vargas - 2018-08-23 - OD 1307
        /// </summary>
        /// <param name="gymId"></param>
        /// <returns></returns>
        public List<gim_huellas> GetFingerprintsByGym(int gymId)
        {
            DataTable dt = new DataTable();

            using (dbWhiteListModelEntities db = new dbWhiteListModelEntities(gymId))
            {
                string strConnection = db.Database.Connection.ConnectionString;

                using (SqlConnection connection = new SqlConnection(strConnection))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        connection.Open();
                        cmd.Connection = connection;
                        cmd.CommandText = "spFingerprint";
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@action", "GetFingerprintsByGym");
                        cmd.Parameters.AddWithValue("@cdgimnasio", gymId);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        connection.Dispose();
                        return ConvertToListEntity(dt);
                    }
                }
            }
        }

        /// <summary>
        /// Método encargado de convertir el DataTable de huellas a una lista de la entidad gim_huellas
        /// Getulio Vargas - 2018-08-23 - OD 1307
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<gim_huellas> ConvertToListEntity(DataTable dt)
        {
            List<gim_huellas> fingerList = new List<gim_huellas>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    gim_huellas finger = new gim_huellas();

                    if (!Convert.IsDBNull(row["hue_dato"]))
                    {
                        finger.hue_dato = (byte[])row["hue_dato"];
                        finger.hue_id = Convert.ToInt32(row["hue_id"].ToString());

                        fingerList.Add(finger);
                    }
                }
            }

            return fingerList;
        }

        public DataTable GetAllFingerprintsPerson(int gymId, string id)
        {
            DataTable dt = new DataTable();

            using (dbWhiteListModelEntities db = new dbWhiteListModelEntities(gymId))
            {
                string strConnection = db.Database.Connection.ConnectionString;

                using (SqlConnection connection = new SqlConnection(strConnection))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        connection.Open();
                        cmd.Connection = connection;
                        cmd.CommandText = "spFingerprint";
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@action", "GetAllFingerprints");
                        cmd.Parameters.AddWithValue("@gymId", gymId);
                        cmd.Parameters.AddWithValue("@personId", id);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        connection.Dispose();
                        return dt;
                    }
                }
            }
        }
    }
}