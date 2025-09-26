using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace emailEnvioSri.App_Code.BLL
{
   public class clsNeg_operaciones
    {
        public DataTable listaXmlPorEnviar(int ambiente)
        {
            return clsVariablesEnvio.oDatos.TraerDataTable("usp_SRI_email_envio_Listar", ambiente);
        }
        public DataTable listaBaseDatosServer(string nombreBd)
        {
            return clsVariablesEnvio.oDatos.TraerDataTable("USP_SRI_BASESDATOSSERVER_BUSCAR", nombreBd);
        }
        public int estadoEmailActualizar(int idArchivo, bool estado, string resultadoEnvio, int relay)
        {
            return clsVariablesEnvio.oDatos.Ejecutar("USP_SRI_BASESDATOSSERVER_ACTUALIZA_ESTADO_CORREO", idArchivo, estado ,resultadoEnvio, relay);
        }
        public int estadoEmailActualizarSitioWeb(string clave, string resultadoActualizacion)
        {
            return clsVariablesEnvio.oDatos.Ejecutar("USP_SRI_DATOSARCHIVOS_ACTUALIZA_ESTADO_CORREO", clave, resultadoActualizacion);
        }

    }
}
