using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using firmarStdSri.helpers;
using firmarStdSri.modelo;
using wsH = firmarStdSri.helpers.WSHelper;
using xmlH = firmarStdSri.helpers.XMLHelper;

namespace firmarStdSri
{


    public class operacionesComprobantes
    {
        public wsGrabarDocV3.wsGrabarDoc grabarArchivo = new wsGrabarDocV3.wsGrabarDoc();
        public wsGrabarDocV3Test.wsGrabarDoc grabarArchivoTest = new wsGrabarDocV3Test.wsGrabarDoc();
        public subirDocumentosElectronicos.wsarchivoselectronicosoffline subirArchivoElectronico = new subirDocumentosElectronicos.wsarchivoselectronicosoffline();

		private string wsEnvio = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl";
        private string wsAutorizacion = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl";
        private string wsEnvio_pruebas = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl";
        private string wsAutorizacion_pruebas = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl";

        private XmlDocument xmlDoc;

        private static byte[] ConvertToBytes(XmlDocument doc)
        {
            Encoding encoding = Encoding.UTF8;
            byte[] docAsBytes = encoding.GetBytes(doc.OuterXml);
            return docAsBytes;
        }

        public string EnviarComprobante(string RutaXml, string ambiente)// ambiente 1 pruebas 2 producción 
        {
            bool result = false;
            string respuestaStr = string.Empty;
            xmlDoc = xmlH.ConvertFileToDocument(RutaXml);

            if (!string.IsNullOrWhiteSpace(ambiente) && !string.IsNullOrWhiteSpace(RutaXml))
            {
                if (ambiente == "1")
                    wsH.URL_Envio = wsEnvio_pruebas;
                if (ambiente == "2")
                    wsH.URL_Envio = wsEnvio;

                wsH.RutaXML = RutaXml;
                try
                {
                    RespuestaSRI respuesta = wsH.EnvioComprobante();

                    if (respuesta.Estado == "RECIBIDA")
                    {
                        respuestaStr = "ESTADO: " + respuesta.Estado;
                        result = true;
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("ESTADO: " + respuesta.Estado);
                        sb.AppendLine(";IDENTIFICADOR: " + respuesta.ErrorIdentificador);
                        sb.AppendLine(";MENSAJE: " + respuesta.ErrorMensaje);
                        sb.AppendLine(";INFO ADICIONAL: " + respuesta.ErrorInfoAdicional);
                        sb.AppendLine(";TIPO: " + respuesta.ErrorTipo);
                        sb.AppendLine(";result: " + result.ToString());

                        respuestaStr = sb.ToString();

                    }

                }
                catch (Exception ex)
                {
                    respuestaStr = ";Error al usar los web services del SRI (envío): " + ex.Message;
                }
            }

            return respuestaStr;//ESTADO: RECIBIDO Ó DEVUELTO
        }
        public string EnviarComprobanteB(string RutaXml, string ambiente, string clave, string pathXmlAut, string nombreNoAutorizado)// ambiente 1 pruebas 2 producción 
        {

            bool result = false;
            string respuestaStr = string.Empty;
            xmlDoc = xmlH.ConvertFileToDocument(RutaXml);

            if (!string.IsNullOrWhiteSpace(ambiente) && !string.IsNullOrWhiteSpace(RutaXml))
            {
                if (ambiente == "1")
                    wsH.URL_Envio = wsEnvio_pruebas;
                if (ambiente == "2")
                    wsH.URL_Envio = wsEnvio;

                wsH.RutaXML = RutaXml;
                try
                {
                    RespuestaSRI respuesta = wsH.EnvioComprobante();
                    respuesta.Ambiente = ambiente;

                    respuesta.Ambiente = (ambiente == "1") ? "PRUEBAS" : ((ambiente == "2") ? "PRODUCCION" :"");

                    respuesta.ClaveAcceso = clave;
                    respuesta.NumeroAutorizacion = clave;
                    respuesta.FechaAutorizacion = DateTime.Now.ToString("yyyy-MM-ddTH:mm:ssK");

                    if (respuesta.Estado == "RECIBIDA")
                    {
                        respuestaStr = "ESTADO: " + respuesta.Estado;
                        result = true;
                        
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("ESTADO: " + respuesta.Estado);
                        sb.AppendLine(";IDENTIFICADOR: " + respuesta.ErrorIdentificador);
                        sb.AppendLine(";MENSAJE: " + respuesta.ErrorMensaje);
                        sb.AppendLine(";INFO ADICIONAL: " + respuesta.ErrorInfoAdicional);
                        sb.AppendLine(";TIPO: " + respuesta.ErrorTipo);
                        sb.AppendLine(";result: " + result.ToString());

                        respuestaStr = sb.ToString();
                    }
                    crearXmlCarga(RutaXml, respuesta, pathXmlAut, true, nombreNoAutorizado);

                }
                catch (Exception ex)
                {
                    respuestaStr = ";Error al usar los web services del SRI (envío): " + ex.Message;
                    crearXmlCarga(RutaXml, pathXmlAut, clave, (ambiente == "1") ? "PRUEBAS" : ((ambiente == "2") ? "PRODUCCION" : ""));
                }
            }

            return respuestaStr;//ESTADO: RECIBIDO Ó DEVUELTO
        }

        public string EnviarComprobanteC(string RutaXml, int ambiente, string email, string nombreBase)// ambiente 1 pruebas 2 producción 
        { 

            bool result = false;
            string respuestaStr = string.Empty;
            xmlDoc = xmlH.ConvertFileToDocument(RutaXml);

            if (!string.IsNullOrWhiteSpace(RutaXml))
            {
                if (ambiente == 1)
                    wsH.URL_Envio = wsEnvio_pruebas;
                if (ambiente == 2)
                    wsH.URL_Envio = wsEnvio;

                wsH.RutaXML = RutaXml;
                try
                {
                    RespuestaSRI respuesta = wsH.EnvioComprobante();

                    if (respuesta.Estado == "RECIBIDA")
                    {
                        respuestaStr = "ESTADO: " + respuesta.Estado;
                        result = true;

                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("ESTADO: " + respuesta.Estado);
                        sb.AppendLine(";IDENTIFICADOR: " + respuesta.ErrorIdentificador);
                        sb.AppendLine(";MENSAJE: " + respuesta.ErrorMensaje);
                        sb.AppendLine(";INFO ADICIONAL: " + respuesta.ErrorInfoAdicional);
                        sb.AppendLine(";TIPO: " + respuesta.ErrorTipo);
                        sb.AppendLine(";result: " + result.ToString());

                        respuestaStr = sb.ToString();
                    }
                    /// codigo ws
                    try
                    {
                        subirArchivoElectronico.wsGrabarDocumentoElectronico(ambiente, nombreBase, xmlDoc.InnerXml, respuesta.Estado, email, respuesta.Estado == "RECIBIDA" ? true : false);
                    }
                    catch (Exception ex)
                    {
                        respuestaStr = ";Error al usar los web services de ACKLINS (envío): " + ex.Message;
                    }
                }
                catch (Exception ex)
                {
                    respuestaStr = ";Error al usar los web services del SRI (envío): " + ex.Message;
                }
            }

            return respuestaStr;//ESTADO: RECIBIDO Ó DEVUELTO
        }

        public string AutorizarComprobante(string clave, string RutaXml, string pathXmlAut, string nombreNoAutorizado, string ambiente)// ambiente 1 pruebas 2 producción 
        {
            bool result = false;
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
                docResp.Load(RutaXml);

                try
                {
                    RespuestaSRI respuesta = wsH.AutorizacionComprobante(out xmlAut);

                    if (respuesta.Estado == "AUTORIZADO")
                    {
                        StringBuilder sb = new StringBuilder();
                        result = true;
                        sb.AppendLine("ESTADO: " + respuesta.Estado);
                        sb.AppendLine("; Nro AUTO.: " + respuesta.NumeroAutorizacion);
                        sb.AppendLine("; FECHA AUTO.: " + respuesta.FechaAutorizacion);
                        sb.AppendLine("; AMBIENTE: " + respuesta.Ambiente);
                        sb.AppendLine("; RAZON SOCIAL: " + xmlH.GetInfoTributaria("razonSocial", docResp));
                        sb.AppendLine("; CLIENTE: " + xmlH.GetInfoFactura("razonSocialComprador", docResp));
                        sb.AppendLine("; result: " + result.ToString());
                        sb.AppendLine("; XmlAutorizadoGrabado: CORRECTO");

                        respuestaStr = sb.ToString();

                    }
                    else if (respuesta.Estado == "NO AUTORIZADO" || string.IsNullOrEmpty(respuesta.Estado))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("ESTADO: " + respuesta.Estado);
                        sb.AppendLine("; FECHA AUTO.: " + respuesta.FechaAutorizacion);
                        sb.AppendLine("; AMBIENTE: " + respuesta.Ambiente);
                        sb.AppendLine("; IDENTIFICADOR: " + respuesta.ErrorIdentificador);
                        sb.AppendLine("; MENSAJE: " + respuesta.ErrorMensaje);
                        sb.AppendLine("; TIPO: " + respuesta.ErrorTipo);
                        sb.AppendLine("; result: " + result.ToString());
                         sb.AppendLine("; XmlAutorizadoGrabado: ERROR");

                        respuestaStr = sb.ToString();
                    }
                    else
                    {
                        respuestaStr = respuesta.Estado;
                    }

                }
                catch (Exception ex)
                {
                    respuestaStr = ";Error al usar los web services del SRI (autorización): " + ex.Message;
                }
            }

            return respuestaStr;
        }

        private bool crearXmlCarga(string RutaXml, string pathXmlAut, string clave, string ambiente)
        {
            string comprobanteAutorizado = string.Empty;
            XmlDocument xdoc = new XmlDocument();
            XmlDocument xdocAut = new XmlDocument();
            bool retorno = false;
            xdoc.Load(RutaXml);

                comprobanteAutorizado = @"<?xml version=""1.0"" encoding=""UTF-8""?><autorizacion><estado>EN PROCESO</estado><numeroAutorizacion>" + clave + "</numeroAutorizacion><fechaAutorizacion>" + DateTime.Now.ToString() + "</fechaAutorizacion><ambiente>" + ambiente + "</ambiente><comprobante>" + xdoc.InnerXml.ToString().Replace("<", "&lt;").Replace(">", "&gt;") + "</comprobante><mensajes/></autorizacion>";

            xdocAut.LoadXml(comprobanteAutorizado);

                xdocAut.Save(pathXmlAut + clave + ".xml");
                if (File.Exists(pathXmlAut + clave + ".xml"))
                    retorno = true;

            return retorno;
        }


        private bool crearXmlCarga(string RutaXml, RespuestaSRI respuesta, string pathXmlAut, bool autorizado, string nombreNoAutorizado)
        {
            string comprobanteAutorizado = string.Empty;
            XmlDocument xdoc = new XmlDocument();
            XmlDocument xdocAut = new XmlDocument();
            bool retorno = false;
            xdoc.Load(RutaXml);


            if (autorizado)
                comprobanteAutorizado = @"<?xml version=""1.0"" encoding=""UTF-8""?><autorizacion><estado>" + respuesta.Estado + "</estado><numeroAutorizacion>" + respuesta.NumeroAutorizacion + "</numeroAutorizacion><fechaAutorizacion>" + respuesta.FechaAutorizacion + "</fechaAutorizacion><ambiente>" + respuesta.Ambiente + "</ambiente><comprobante>" + xdoc.InnerXml.ToString().Replace("<", "&lt;").Replace(">", "&gt;") + "</comprobante><mensajes/></autorizacion>";

            if (!autorizado)
                comprobanteAutorizado = @"<?xml version=""1.0"" encoding=""UTF-8""?><autorizacion><estado>" + respuesta.Estado + "</estado><numeroAutorizacion>NO AUTORIZADO</numeroAutorizacion><fechaAutorizacion>NO AUTORIZADO</fechaAutorizacion><ambiente>" + respuesta.Ambiente + "</ambiente><comprobante>" + xdoc.InnerXml.ToString().Replace("<", "&lt;").Replace(">", "&gt;") + "</comprobante><mensajes/></autorizacion>";

            xdocAut.LoadXml(comprobanteAutorizado);

            if (autorizado)
            {
                xdocAut.Save(pathXmlAut + respuesta.NumeroAutorizacion + ".xml");
                if (File.Exists(pathXmlAut + respuesta.NumeroAutorizacion + ".xml"))
                    retorno = true;
            }
            if (!autorizado)
            {
                xdocAut.Save(pathXmlAut + nombreNoAutorizado + ".xml");
                if (File.Exists(pathXmlAut + nombreNoAutorizado + ".xml"))
                    retorno = true;
            }

            return retorno;
        }


        public string subirSitioDeCargasyDescargas(int idTipoDocumento, string nombreBd, string numeroDocumento, string numeroIdentificacion, string correo, string usuario,
                                                  string titulo, string descripcion, string numeroAutorizacion, string tamanoArchivo,
                                                   string periodo, int idGrupo, string Nombre, string Direccion, string pathXmlAut, bool autorizacion, string carpetaCliente, string ambiente, string empresa, string repositorio)// ambiente 1 pruebas 2 producción // empresa d=dm  o=os//
        {
            string error="Error: Ambiente no indicado o incorrecto";
            if (!string.IsNullOrEmpty(ambiente)&& !string.IsNullOrEmpty(repositorio))
            {
                string xmlArchivo = string.Empty;
                XmlDocument xmlAutdoc = new XmlDocument();
                xmlAutdoc.Load(@pathXmlAut);
                xmlArchivo = xmlAutdoc.InnerXml;

                if (ambiente == "1")
                {
                    if (empresa == "A")
                    {
                        return grabarArchivoTest.GrabarDatosDocumentoXml(idTipoDocumento, nombreBd, numeroDocumento, numeroIdentificacion, correo, correo,
                                                     titulo, descripcion, "1", numeroAutorizacion + ".zip", 1, tamanoArchivo, periodo,
                                                        idGrupo, numeroAutorizacion + ".xml", Nombre, Direccion, xmlArchivo, pathXmlAut, repositorio + carpetaCliente + "/", numeroAutorizacion, autorizacion);
                    }
                    return error;

                }
                else if (ambiente == "2")
                {
                    if (empresa == "A")
                    {
                        return grabarArchivo.GrabarDatosDocumentoXml(idTipoDocumento, nombreBd, numeroDocumento, numeroIdentificacion, correo, correo,
                                                                  titulo, descripcion, "1", numeroAutorizacion + ".zip", 1, tamanoArchivo, periodo,
                                                                  idGrupo, numeroAutorizacion + ".xml", Nombre, Direccion, xmlArchivo, pathXmlAut, repositorio + carpetaCliente + "/", numeroAutorizacion, autorizacion,ambiente);
                    }
                    return error;
                }
                else
                    return error;
            }
            else
                return error;

        }

        public string subirSitioDeCargasyDescargasLab(int idTipoDocumento, string nombreBd, string numeroDocumento, string numeroIdentificacion, string correo, string usuario,
                                                  string titulo, string descripcion, string numeroAutorizacion, string tamanoArchivo,
                                                   string periodo, int idGrupo, string Nombre, string Direccion, string pathXmlAut, bool autorizacion, string carpetaCliente, string ambiente, string empresa, string repositorio)// ambiente 1 pruebas 2 producción // empresa d=dm  o=os//
        {
            string error = "Error: Ambiente no indicado o incorrecto";
            if (!string.IsNullOrEmpty(ambiente) && !string.IsNullOrEmpty(repositorio))
            {
                string xmlArchivo = string.Empty;
                XmlDocument xmlAutdoc = new XmlDocument();
                xmlAutdoc.Load(@pathXmlAut);
                xmlArchivo = xmlAutdoc.InnerXml;

                if (ambiente == "1")
                {
                    if (empresa == "A")
                    {
                        return grabarArchivoTest.GrabarDatosDocumentoXmlLab(idTipoDocumento, nombreBd, numeroDocumento, numeroIdentificacion, correo, correo,
                                                     titulo, descripcion, "1", numeroAutorizacion + ".zip", 1, tamanoArchivo, periodo,
                                                        idGrupo, numeroAutorizacion + ".xml", Nombre, Direccion, xmlArchivo, pathXmlAut, repositorio + carpetaCliente + "/", numeroAutorizacion, autorizacion);
                    }
                    return error;

                }
                else if (ambiente == "2")
                {
                    if (empresa == "A")
                    {
                        return grabarArchivo.GrabarDatosDocumentoXmlLab(idTipoDocumento, nombreBd, numeroDocumento, numeroIdentificacion, correo, correo,
                                                                  titulo, descripcion, "1", numeroAutorizacion + ".zip", 1, tamanoArchivo, periodo,
                                                                  idGrupo, numeroAutorizacion + ".xml", Nombre, Direccion, xmlArchivo, pathXmlAut, repositorio + carpetaCliente + "/", numeroAutorizacion, autorizacion, ambiente);
                    }
                    return error;
                }
                else
                    return error;

            }
            else
                return error;

        }


        public static string[] dtDataWs(string pathArchivoXml)// método de uso opcional, si no se obtienen los datos del sistema, los extraemos del xml
        {
            string DataWs;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pathArchivoXml);
            string numDocumento = xmlDoc.GetElementsByTagName("estab")[0].InnerXml + "-" + xmlDoc.GetElementsByTagName("ptoEmi")[0].InnerXml + "-" + xmlDoc.GetElementsByTagName("secuencial")[0].InnerXml;
            string ciRucComprador = string.Empty;
            string claveAcceso = xmlDoc.GetElementsByTagName("claveAcceso")[0].InnerXml;
            string nombreComprador = string.Empty;
            string direccionComprador = string.Empty;
            string tituloDoc = string.Empty;
            string idTipoDoc = string.Empty;
            string periodoFiscal = string.Empty;


            if (xmlDoc.GetElementsByTagName("periodoFiscal").Count > 0)
            {
                periodoFiscal = xmlDoc.GetElementsByTagName("periodoFiscal")[0].InnerXml;
            }
            else
            {
                periodoFiscal = DateTime.Now.Year.ToString() + '-' + DateTime.Now.Month.ToString();
            }

            if (xmlDoc.GetElementsByTagName("identificacionComprador").Count > 0)
            {
                ciRucComprador = xmlDoc.GetElementsByTagName("identificacionComprador")[0].InnerXml;
            }

            if (xmlDoc.GetElementsByTagName("identificacionSujetoRetenido").Count > 0)
            {
                ciRucComprador = xmlDoc.GetElementsByTagName("identificacionSujetoRetenido")[0].InnerXml;
            }

            if (xmlDoc.GetElementsByTagName("razonSocialComprador").Count > 0)
            {
                nombreComprador = xmlDoc.GetElementsByTagName("razonSocialComprador")[0].InnerXml;
            }

            if (xmlDoc.GetElementsByTagName("razonSocialSujetoRetenido").Count > 0)
            {
                nombreComprador = xmlDoc.GetElementsByTagName("razonSocialSujetoRetenido")[0].InnerXml;
            }

            if (xmlDoc.GetElementsByTagName("direccionComprador").Count > 0)
            {
                direccionComprador = xmlDoc.GetElementsByTagName("direccionComprador")[0].InnerXml;
            }
            else
            {
                direccionComprador = "N/D";
            }

            if (xmlDoc.GetElementsByTagName("infoFactura").Count > 0)
            {
                tituloDoc = "Factura";
                idTipoDoc = "1";
            }
            if (xmlDoc.GetElementsByTagName("infoNotaCredito").Count > 0)
            {
                tituloDoc = "Nota de crédito";
                idTipoDoc = "3";
            }
            if (xmlDoc.GetElementsByTagName("infoCompRetencion").Count > 0)
            {
                tituloDoc = "Comprobante Retencion";
                idTipoDoc = "9";
            }
            if (xmlDoc.GetElementsByTagName("infoGuiaRemision").Count > 0)
            {
                tituloDoc = "Guia Remisión";
                idTipoDoc = "11";
            }

            FileInfo tamanoFile = new System.IO.FileInfo(pathArchivoXml);

            DataWs = idTipoDoc + ";" + numDocumento + ";" + ciRucComprador + ";" + tituloDoc + ";" + claveAcceso + ";" + tamanoFile.Length.ToString() + ";" + nombreComprador + ";" + direccionComprador + ";" + periodoFiscal;
            return DataWs.Split(';');


        }
    }
}
