using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace firmaAutorizacionSri.App_Code.BLL
{
   public class clsNeg_operaciones
    {
        public DataTable listaXmlPorAutorizar(int ambiente)
        {
            return clsVariablesAutorizcion.oDatos.TraerDataTable("usp_SRI_firma_autorizacion_Listar",ambiente);
        }
        public object cambioEstadoXml(int idXmlPb, string tipoEstado, string ResultadoSri)
        {
            return clsVariablesAutorizcion.oDatos.Ejecutar("USP_SRI_BASESDATOSSERVER_ACTUALIZA_ESTADO", idXmlPb, tipoEstado, ResultadoSri);

        }
        public object GrabaErroresSri(string Empresa,string Ruc, string NumeroFactura , string Fecha, string ResultadoSri,int idXmlPb)
        {
            return clsVariablesAutorizcion.oDatos.Ejecutar("usp_Graba_Errores_autorizacion_SRI", Empresa, Ruc, NumeroFactura, Fecha, ResultadoSri, idXmlPb,"AUTORIZADOR");

        }
        public DataTable devuelveEmail(string email)
        {
            return clsVariablesAutorizcion.oDatos.TraerDataTable("usp_BASESDATOSSERVER_devuelve_Email", email);
        }
        public DataTable listaBaseDatosServer(string nombreBd)
        {
            return clsVariablesAutorizcion.oDatos.TraerDataTable("USP_SRI_BASESDATOSSERVER_BUSCAR", nombreBd);
        }

    }
}
