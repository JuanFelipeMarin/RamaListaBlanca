﻿using Sibo.WhiteList.Service.BLL.BLL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Service.Entities.Classes;
using System.Net.NetworkInformation;
using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.BLL.Log;
using System.Data;
using System.Timers;
using Sibo.WhiteList.Service.BLL.Helpers;
using System.Threading;
using Sibo.Servicio.ListaBlanca.Classes;
using ImagenTCAM7000;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Configuration;

namespace Sibo.Servicio.ListaBlanca.Classes
{
  public class clsZonas
    {

        public int id { get; set; }
        public string codigo_string { get; set; }
        public int cdgimnasio { get; set; }
        public string descripcion { get; set; }
        public string nombre_zona { get; set; }
        public int intSucursal { get; set; }
        public string Terminales { get; set; }
        public bool bitEstado { get; set; }
        public DateTime fechacreacion { get; set; }

        public bool GetZonas(int gymId, string branchId)
        {
            ZonasBLL termBLL = new ZonasBLL();
            return termBLL.GetZonas(gymId, branchId);
        }
    }

    
}
