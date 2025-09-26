using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using revisionSriEnvio.App_Code.DAL;

namespace revisionSriEnvio.App_Code.BLL
{
    public class clsVariablesFirma
    {
        public static clsNeg_AccesoDatos oDatos = new clsNeg_AccesoDatos("revision.Envio.SRI");
        public static clsNeg_revisiones objRevision = new clsNeg_revisiones();
    }
}
