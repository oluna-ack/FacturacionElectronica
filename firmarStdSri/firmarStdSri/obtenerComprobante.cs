using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using firmarStdSri.modelo;
using System.Runtime.InteropServices;
using wsH = firmarStdSri.helpers.WSHelper;
using xmlH = firmarStdSri.helpers.XMLHelper;




namespace firmarStdSri
{


    public class obtenerComprobante
    {
        private string wsEnvio = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl";
        private string wsAutorizacion = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl";
        private string wsEnvio_pruebas = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl";
        private string wsAutorizacion_pruebas = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl";


        private static byte[] ConvertToBytes(XmlDocument doc)
        {
            Encoding encoding = Encoding.UTF8;
            byte[] docAsBytes = encoding.GetBytes(doc.OuterXml);
            return docAsBytes;
        }

        public string ObtenerStringComprobante(string clave, string ambiente)
        {

            string respuestaStr = string.Empty;

            if (!string.IsNullOrWhiteSpace(clave))
            {
                if (ambiente == "1")
                    wsH.URL_Autorizacion = wsAutorizacion_pruebas;

                if (ambiente == "2")
                    wsH.URL_Autorizacion = wsAutorizacion;


                wsH.ClaveAcceso = clave;
                XmlDocument docResp = new XmlDocument();
                XmlDocument xmlAut;

                try
                {
                    RespuestaSRI respuesta = wsH.AutorizacionComprobante(out xmlAut);

                    respuestaStr = respuesta.Comprobante;

                }
                catch (Exception ex)
                {
                    respuestaStr = ";Error al usar los web services del SRI (autorización): " + ex.Message;
                }
            }

            return respuestaStr;
        }

    }
}
