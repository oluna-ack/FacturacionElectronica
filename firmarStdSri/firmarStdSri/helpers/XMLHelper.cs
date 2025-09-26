using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using firmarStdSri.modelo;
using System.IO;

namespace firmarStdSri.helpers
{
    public class XMLHelper
    {
        public static XmlDocument ConvertStringToDocument(string xml_string)
        {
            XmlDocument result = new XmlDocument();
            result.LoadXml(xml_string);

            return result;
        }

        public static XmlDocument ConvertFileToDocument(string file_path)
        {
            XmlDocument result = new XmlDocument();
            result.Load(file_path);

            return result;
        }

        public static string ConvertToBase64String(string file_path)
        {
            byte[] binarydata = File.ReadAllBytes(file_path);
            return Convert.ToBase64String(binarydata, 0, binarydata.Length);
        }

        private static string GetNodeValue(string rootNodo, string infoNodo, XmlDocument doc)
        {
            string result = null;
            string node_path = "//" + rootNodo + "//" + infoNodo;

            XmlNode node = doc.SelectSingleNode(node_path);

            if (node != null)
            {
                result = node.InnerText;
            }

            return result;
        }

        public static RespuestaSRI GetRespuestaRecepcion(XmlDocument xml_doc)
        {
            RespuestaSRI result = new RespuestaSRI();
            result.Estado = GetNodeValue("RespuestaRecepcionComprobante", "estado", xml_doc);

            if (result.Estado != "RECIBIDA")
            {
                result.ClaveAcceso = GetNodeValue("comprobante", "claveAcceso", xml_doc);
                result.ErrorIdentificador = GetNodeValue("mensaje", "identificador", xml_doc);
                result.ErrorMensaje = GetNodeValue("mensaje", "mensaje", xml_doc);
                result.ErrorInfoAdicional = GetNodeValue("mensaje", "informacionAdicional", xml_doc);
                result.ErrorTipo = GetNodeValue("mensaje", "tipo", xml_doc);
            }

            return result;
        }

        public static RespuestaSRI GetRespuestaAutorizacion(XmlDocument xml_doc)
        {
            RespuestaSRI result = new RespuestaSRI();
            string pathLevelAutorizacion = "RespuestaAutorizacionComprobante/autorizaciones/autorizacion[last()]";
            string pathLevelMensajes = "RespuestaAutorizacionComprobante/autorizaciones/autorizacion/mensajes[last()]/mensaje";

            result.Estado = GetNodeValue(pathLevelAutorizacion, "estado", xml_doc);

            if (result.Estado == "AUTORIZADO")
            {
                result.NumeroAutorizacion = GetNodeValue(pathLevelAutorizacion, "numeroAutorizacion", xml_doc);
                result.FechaAutorizacion = GetNodeValue(pathLevelAutorizacion, "fechaAutorizacion", xml_doc);
                result.Ambiente = GetNodeValue(pathLevelAutorizacion, "ambiente", xml_doc);
            }
            else if (result.Estado == "NO AUTORIZADO")
            {
                result.FechaAutorizacion = GetNodeValue(pathLevelAutorizacion, "fechaAutorizacion", xml_doc);
                result.Ambiente = GetNodeValue(pathLevelAutorizacion, "ambiente", xml_doc);
                result.ErrorIdentificador = GetNodeValue(pathLevelMensajes, "identificador", xml_doc);
                result.ErrorMensaje = GetNodeValue(pathLevelMensajes, "mensaje", xml_doc);
                result.ErrorTipo = GetNodeValue(pathLevelMensajes, "tipo", xml_doc);
            }

            return result;
        }
        public static string GetInfoTributaria(string info, XmlDocument xml_doc)
        {
            return GetNodeValue("infoTributaria", info, xml_doc);
        }

        public static string GetInfoFactura(string info, XmlDocument xml_doc)
        {
            return GetNodeValue("infoFactura", info, xml_doc);
        }
    }
}
