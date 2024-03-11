﻿using Newtonsoft.Json;
using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.DAL.Data;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.BLL.BLL
{
 public   class ZonasBLL
    {
        public bool GetZonas(int gymId, string branchId)
        {
            ServiceLog log = new ServiceLog();
            leerJson lg = new leerJson();

            try
            {
                ZonaAPI termAPI = new ZonaAPI();
                List<eZonas> termList = new List<eZonas>();
                bool resp = false;
                log.WriteProcess("Consumo de API para descargar la lista de Zonas.");
                termList = termAPI.GetZonasList(gymId, branchId);

                if (termList != null && termList.Count > 0)
                {
                    log.WriteProcess("Se procede a guardar o actualizar las zonas en la BD local.");
                    string jsonString = JsonConvert.SerializeObject(termList);
                    resp = lg.DescargaConfiguraciones(jsonString, 3);
                    //resp = InsertOrUpdateZonas(termList);
                }
                else
                {
                    log.WriteProcess("No se encontraron registros de zonas en GSW para actualizar o insertar en BD local.");
                }

                return resp;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return false;
            }
        }


        private bool InsertOrUpdateZonas(List<eZonas> termList)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dZonas termData = new dZonas();
                eZonas term = new eZonas();
                bool resp = false;
                log.WriteProcess("Inicia el proceso de inserción o actualización de terminales en BD local.");

                foreach (eZonas item in termList)
                {
                    term = termData.GetZonasById(item.id);

                    if (term != null && term.id != 0)
                    {

                        term.id = item.id;
                        term.codigo_string = item.codigo_string;
                        term.cdgimnasio = item.cdgimnasio;
                        term.descripcion = item.descripcion;
                        term.nombre_zona = item.nombre_zona;
                        term.intSucursal = item.intSucursal;
                        term.Terminales = item.Terminales;
                        term.bitEstado = item.bitEstado;
                        term.fechacreacion = item.fechacreacion;

                       
                        resp = termData.Update(term);
                    }
                    else
                    {
                        resp = termData.Insert(item);
                    }
                }

                log.WriteProcess("Finaliza proceso de inserción o actualización de terminales en BD local.");
                return resp;
            }
            catch (Exception ex)
            {
                log.WriteError("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteProcess(ex.Message + " " + ex.StackTrace.ToString());
                return false;
            }
        }

    }
}
