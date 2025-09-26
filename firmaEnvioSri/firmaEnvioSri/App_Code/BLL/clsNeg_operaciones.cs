using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace firmaEnvioSri.App_Code.BLL
{
    public class clsNeg_operaciones
    {
        public DataTable listaXmlPorFirmar(int ambiente)
        {

           
            return clsVariablesFirma.oDatos.TraerDataTable("usp_SRI_firma_envio_Listar", ambiente);
        }

        public object GrabaErroresSri(string Empresa, string Ruc, string NumeroFactura, string Fecha, string ResultadoSri, int idXmlPb)
        {
            return clsVariablesFirma.oDatos.Ejecutar("usp_Graba_Errores_autorizacion_SRI", Empresa, Ruc, NumeroFactura, Fecha, ResultadoSri, idXmlPb, "FIRMADOR");

        }

        public DataTable listaBaseDatosServer(string nombreBd)
        {
            return clsVariablesFirma.oDatos.TraerDataTable("USP_SRI_BASESDATOSSERVER_BUSCAR", nombreBd);
        }
        public object cambioEstadoXml(int idXmlPb, string tipoEstado, string ResultadoSri)
        {
            return clsVariablesFirma.oDatos.Ejecutar("USP_SRI_BASESDATOSSERVER_ACTUALIZA_ESTADO", idXmlPb, tipoEstado, ResultadoSri);

        }
        public object cambioEstadoContingenteXml(int idXmlPb, bool operacion)
        {
            return clsVariablesFirma.oDatos.Ejecutar("usp_SRI_firma_revision_contingente", idXmlPb, operacion);

        }
        public object cambioEstadoXml(int idXmlPb, string valido)
        {
            return clsVariablesFirma.oDatos.Ejecutar("USP_SRI_BASESDATOSSERVER_ACTUALIZA_ESTADO_ARCHIVO", idXmlPb, valido);

        }

        public DataTable devuelveEmail(string email)
        {
            return clsVariablesFirma.oDatos.TraerDataTable("usp_BASESDATOSSERVER_devuelve_Email", email);
        }

        public DataTable ConsultaLeyendas(string txtCodigo)
        {
            return clsVariablesFirma.oDatos.TraerDataTable("USP_SRI_BASESDATOSSERVER_BUSCAR", txtCodigo);
        }
    }
}
