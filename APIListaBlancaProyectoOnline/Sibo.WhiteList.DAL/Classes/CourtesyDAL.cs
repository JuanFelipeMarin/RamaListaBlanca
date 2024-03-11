using Sibo.WhiteList.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sibo.WhiteList.Classes;
using System.Data.SqlClient;
using System.Data;

namespace Sibo.WhiteList.DAL.Classes
{
    public class CourtesyDAL
    {
        public gim_planes_usuario_especiales GetVigentCourtesy(int gymId, string id)
        {
            gim_planes_usuario_especiales courtesy = new gim_planes_usuario_especiales();

            using (var context = new dbWhiteListModelEntities(gymId))
            {
                courtesy = (from cou in context.gim_planes_usuario_especiales
                            where cou.cdgimnasio == gymId && cou.plusu_identifi_cliente == id &&
                                  cou.plusu_fecha_inicio <= DateTime.Today &&
                                  cou.plusu_fecha_vcto >= DateTime.Today && cou.plusu_avisado != true &&
                                  cou.plusu_codigo_plan != 999 && cou.plusu_est_anulada != true
                            select cou).FirstOrDefault();

                return courtesy;
            }
        }
    }
}
