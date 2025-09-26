using System.Data;
using revisionSriEnvio.App_Code.BLL;
using firmarStdSri;
using System;
using System.IO;

namespace revisionSriEnvio
{
   public class Program
    {
        static void Main(string[] args)
        {
            string pathFile = string.Empty;
            string pathFileB = string.Empty;
            string plataforma = string.Empty;
			int ambienteAck = 1;

			clsVariablesFirma.objRevision.corregirEstadoEmail();

			plataforma = args[0];

			//plataforma = "P";

			if (plataforma == "P")
				ambienteAck = 2;

			if (plataforma == "P")
            {
                pathFile = @"\\10.0.0.4\documentos\xmlSinFirma\autorizado\";
                pathFileB = @"\\10.0.0.4\documentos\";
            }
            if (plataforma == "T")
            {
                pathFile = @"\\10.0.0.4\documentosTest\xmlSinFirma\autorizado\";
                pathFileB = @"\\10.0.0.4\documentosTest\";
            }

                DataTable dt = new DataTable();
                dt = clsVariablesFirma.objRevision.listaXmlPorRevisar(ambienteAck);
                operacionesComprobantes op = new operacionesComprobantes();
                string respuestaAutorizacion = string.Empty;
                foreach (DataRow dtRow in dt.Rows)
                {
                        try
                        {
                            respuestaAutorizacion = op.AutorizarComprobante(dtRow[7].ToString(), dtRow[11].ToString(), pathFile, dtRow[7].ToString() + "error", dtRow[3].ToString());


							if (respuestaAutorizacion.Contains("ESTADO: AUTORIZADO") || respuestaAutorizacion.Contains("ESTADO: DEVUELTO") || respuestaAutorizacion.Contains("EN PROCESO") || respuestaAutorizacion.Contains("en procesamiento"))
                            {
                                clsVariablesFirma.objRevision.actualizarEnvio(Convert.ToInt32(dtRow[0].ToString()));
                            }
                            else
                            {
                                clsVariablesFirma.objRevision.repetirEnvio(Convert.ToInt32(dtRow[0].ToString()));
                            }

                            if (!respuestaAutorizacion.Contains(";Error al usar los web services del SRI (envío): "))
                            clsVariablesFirma.objRevision.cambioEstadoContingenteXml(Convert.ToInt32(dtRow[0].ToString()), false);
					        
                        }
                        catch (Exception ex)
                        {
							clsVariablesFirma.objRevision.cambioEstadoXml(Convert.ToInt32(dtRow[0].ToString()), "a", "error de aplicación o procedimiento almacenado");
                        }

               }
        }
    }
}
