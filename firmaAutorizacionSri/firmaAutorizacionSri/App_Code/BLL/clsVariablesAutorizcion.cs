using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using firmaAutorizacionSri.App_Code.DAL;

namespace firmaAutorizacionSri.App_Code.BLL
{
    public class clsVariablesAutorizcion
    {
        public static clsNeg_AccesoDatos oDatos = new clsNeg_AccesoDatos("firmaAutorizacionSri.firma.Envio.SRI");
        public static clsNeg_operaciones objAutorizar = new clsNeg_operaciones();

    }
}
