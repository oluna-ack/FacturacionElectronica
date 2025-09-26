using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace revisionSriEnvio.App_Code.BLL
{
    public class clsNeg_revisiones
    {
        public DataTable listaXmlPorRevisar(int ambiente)
        {
            return clsVariablesFirma.oDatos.TraerDataTable("usp_SRI_firma_revision_Listar", ambiente);
        }
        public int repetirEnvio(int idArchivo)
        {
            return clsVariablesFirma.oDatos.Ejecutar("usp_SRI_firma_revision_repetir", idArchivo);
        }
        public int actualizarEnvio(int idArchivo)
        {
            return clsVariablesFirma.oDatos.Ejecutar("usp_SRI_firma_revision_actualizarEnvio", idArchivo);
        }
        //public int repetirDevuelto(int idArchivo)
        //{
        //    return clsVariablesFirma.oDatos.Ejecutar("usp_SRI_firma_revision_repetir_devuelto", idArchivo);
        //}
        public int cambioEstadoXml(int idXmlPb, string tipoEstado, string ResultadoSri)
        {
            return clsVariablesFirma.oDatos.Ejecutar("USP_SRI_BASESDATOSSERVER_ACTUALIZA_ESTADO", idXmlPb, tipoEstado, ResultadoSri);

        }
        public object cambioEstadoContingenteXml(int idXmlPb, bool operacion)
        {
            return clsVariablesFirma.oDatos.Ejecutar("usp_SRI_firma_revision_contingente", idXmlPb, operacion);

        }
		public object corregirEstadoEmail()
		{
			return clsVariablesFirma.oDatos.Ejecutar("USP_RESET_ESTADO_EMAIL");
		}
    }
}
