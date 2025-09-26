using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using firmaEnvioSri.App_Code.DAL;

namespace firmaEnvioSri.App_Code.BLL
{
   public class clsVariablesFirma
    {
        public static clsNeg_AccesoDatos oDatos = new clsNeg_AccesoDatos("firmaEnvioSri.firma.Envio.SRI");
        public static clsNeg_operaciones objfirma = new clsNeg_operaciones();
        
    }
}
