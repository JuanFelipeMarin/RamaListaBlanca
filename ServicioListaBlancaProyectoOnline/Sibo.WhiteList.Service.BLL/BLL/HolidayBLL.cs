using Newtonsoft.Json;
using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.DAL.API;
using Sibo.WhiteList.Service.DAL.Data;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.BLL.BLL
{
    public class HolidayBLL
    {
        public bool GetHolidays(int gymId)
        {
            ServiceLog log = new ServiceLog();
            leerJson lg = new leerJson();

            try
            {
                bool resp = false;
                HolidayAPI hdAPI = new HolidayAPI();
                dHoliday hdData = new dHoliday();
                List<eHoliday> hdList = new List<eHoliday>();
                log.WriteProcess("Consumo de API para descargar los días festivos.");
                hdList = hdAPI.GetHolidays(gymId);

                if (hdList != null && hdList.Count > 0)
                {
                    log.WriteProcess("Se procede a guardar o actualizar los días festivos en la BD local.");
                    //resp = InsertOrUpdateHolidays(hdList);
                    string jsonString = JsonConvert.SerializeObject(hdList);
                    resp = lg.DescargaConfiguraciones(jsonString, 4);
                }
                else
                {
                    log.WriteProcess("No se encontraron registros de días festivos en GSW para actualizar o insertar en BD local.");
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

        private bool InsertOrUpdateHolidays(List<eHoliday> hdList)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dHoliday hdData = new dHoliday();
                eHoliday hdEntity = new eHoliday();
                bool resp = false;
                log.WriteProcess("Inicia el proceso de inserción o actualización de días festivos en BD local.");

                foreach (eHoliday item in hdList)
                {
                    hdEntity = hdData.GetHolidayById(item.id);

                    if (hdEntity != null && hdEntity.id != 0)
                    {
                        if (item.date.Date != hdEntity.date.Date)
                        {
                            hdEntity.date = item.date;
                            resp = hdData.Update(hdEntity);
                        }
                        else
                        {
                            log.WriteProcess("El día festivo " + item.date.Date.ToString("yyyy-MM-dd") + 
                                             ". Ya está registrado en la base de datos, por lo tanto no es necesario insertarlo nuevamente.");
                        }
                    }
                    else
                    {
                        resp = hdData.Insert(item);
                    }
                }

                log.WriteProcess("Finaliza proceso de inserción o actualización de días festivos en BD local.");
                return resp;
            }
            catch (Exception ex)
            {
                log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                return false;
            }
        }

        public List<eHoliday> GetLocalHolidays(bool isService)
        {
            ServiceLog log = new ServiceLog();

            try
            {
                dHoliday hdData = new dHoliday();
                List<eHoliday> responseList = new List<eHoliday>();
                DataTable dt = new DataTable();
                int actualYear = DateTime.Today.Year;
                dt = hdData.GetLocalHolidays(actualYear);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        eHoliday holiday = new eHoliday();
                        holiday.id = Convert.ToInt32(row["id"].ToString());
                        holiday.date = Convert.ToDateTime(row["dateHoliday"]);
                        responseList.Add(holiday);
                    }
                }

                return responseList;
            }
            catch (Exception ex)
            {
                if (isService)
                {
                    log.WriteProcess("Ocurrió un error en el proceso, por favor verificar el log de errores.");
                    log.WriteError(ex.Message + " " + ex.StackTrace.ToString());
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
