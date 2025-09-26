using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using emailEnvioSri.App_Code.DAL;

namespace emailEnvioSri.App_Code.BLL
{
   public class clsVariablesEnvio
    {
        public static clsNeg_AccesoDatos oDatos = new clsNeg_AccesoDatos("firmaEmailSri.firma.Envio.SRI");
        public static clsNeg_operaciones objEnviar = new clsNeg_operaciones();
    }
}
