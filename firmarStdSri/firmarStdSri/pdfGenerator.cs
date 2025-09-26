using System;
using System.Text;
using System.Data;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Telerik.ReportViewer.WebForms;
using System.IO;
using System.Xml;
using System.Net.Mail;
using Ionic.Zip;
using System.IO;

namespace firmarStdSri
{


    public class pdfGenerator
    {  // tipoDocumento 1 Factura a cliente / 3 Nota de crédito a cliente / 9 Retenciones a proveedor / 11 guia remisión
        public string generarPdf(string pathLogo, int largoLogo, int altoLogo, string pathArchivo, string pathArchivoPdf, string tipoDocumento)
        {

            //**************************************************************************************
            string nombreArchRef = string.Empty;
            string nombreArchPdf = string.Empty;

            string numeroAutorizacion = string.Empty, fechaAutorizacion = string.Empty, ambiente = string.Empty;
            string numeroFactura = string.Empty, claveAcceso = string.Empty, tipoEmision = string.Empty, fechaEmision = string.Empty;
            string identificacionComprador = string.Empty, razonSocialComprador = string.Empty, campoAdicional = string.Empty;
            string subTotalIva0 = string.Empty, subTotalIva12 = string.Empty, valorIVA12 = string.Empty, totalSinImpuestos = string.Empty;
            string totalDescuento = string.Empty, importeTotal = string.Empty;

            string ruc = string.Empty, razon = string.Empty, obligadoContabilidad = string.Empty, numcontribuyente = string.Empty, actividadEmpresa = string.Empty;
            string direc = string.Empty, DireccionLocal = string.Empty, ciudadLocal = string.Empty;
            string detalleXmlB = string.Empty, ubicacionArchivo = string.Empty, periodoFiscal = string.Empty, valorPorcentajeImpuesto = string.Empty;

            string fechaEmisionDocSustento = string.Empty, motivo = string.Empty, numDocModificado = string.Empty, XmlInterno = string.Empty;
            string propina = string.Empty, informacionAdicional = string.Empty, formaPago = string.Empty;

            string fechaIniTransporte = string.Empty, fechaFinTransporte = string.Empty, placa = string.Empty;
            string dirPartida = string.Empty, razonSocialTransportista = string.Empty, tipoIdentificacionTransportista = string.Empty;
            string rucTransportista = string.Empty, identificacionDestinatario = string.Empty, razonSocialDestinatario = string.Empty;
            string dirDestinatario = string.Empty, motivoTraslado = string.Empty, codEstabDestino = string.Empty;
            string datosTransporte = string.Empty, datosDestino = string.Empty;
            string codDocSustento = string.Empty, numDocSustento = string.Empty, numAutDocSustento = string.Empty;
            string tipoDocModificado = string.Empty, guiaRemisioFac = string.Empty;
            string nac = string.Empty, micro = string.Empty, direccionComprador=string.Empty;
            string rimpe = string.Empty;
            bool cobraServicio = false;
            string valorServicio = string.Empty;

            DataTable dt = new DataTable();
            //DataTable dtB = new DataTable();
            //DataTable dtLogo = new DataTable();
            //DataTable dty = new DataTable();
            //dtLogo = clsVariablesAut.objDs.buscarLogoEmpresa(idArchivo);

            //Session["nombreEmpresaPdf"] = null  -> razon;
            try
            {
                //dtB = clsVariablesAut.objDs.rideRegistrosEmpresaB(idArchivo);
                //nombreArchRef = dtB.Rows[0][16].ToString();
                //nombreArchPdf = dtB.Rows[0][11].ToString();


                XmlDocument xDoc = new XmlDocument();
                //xDoc.Load(Session["pathCargas"] + dtB.Rows[0][2].ToString() + "/" + nombreArchRef);
                xDoc.Load(pathArchivo); 
                //xDoc.Load(@"C:\edocs\2712201903179140530700110010010000000041234567811.xml");

                //if (xDoc.SelectSingleNode("RespuestaAutorizacionComprobante/autorizaciones/autorizacion/comprobante").ChildNodes[0].NodeType.ToString()=="CDATA")
                if (xDoc.GetElementsByTagName("comprobante")[0].ChildNodes[0].NodeType.ToString() == "CDATA")
                {
                    //XmlCDataSection cDataNode = (XmlCDataSection)(xDoc.SelectSingleNode("RespuestaAutorizacionComprobante/autorizaciones/autorizacion/comprobante").ChildNodes[0]);
                    XmlCDataSection cDataNode = (XmlCDataSection)(xDoc.GetElementsByTagName("comprobante")[0].ChildNodes[0]);
                    XmlInterno = cDataNode.Data;
                }


                XmlDocument xDocInt = new XmlDocument();
                if (string.IsNullOrEmpty(XmlInterno) || string.IsNullOrWhiteSpace(XmlInterno))
                {

                    //XmlNodeList xmlOriginal = xDoc.GetElementsByTagName("comprobante");
                    xDocInt.LoadXml(xDoc.GetElementsByTagName("comprobante")[0].InnerXml.Replace("&lt;", "<").Replace("&gt;", ">"));

                }
                else
                {
                    xDocInt.LoadXml(XmlInterno);
                }

                numeroAutorizacion = xDoc.GetElementsByTagName("numeroAutorizacion")[0].InnerXml;
                fechaAutorizacion = xDoc.GetElementsByTagName("fechaAutorizacion")[0].InnerXml;

                //dty = clsVariablesAut.objDs.rideRegistrosEmpresa(idArchivo, xDocInt.GetElementsByTagName("estab")[0].InnerXml);
                ruc = xDocInt.GetElementsByTagName("ruc")[0].InnerXml;
                razon = xDocInt.GetElementsByTagName("razonSocial")[0].InnerXml;


                if (xDocInt.GetElementsByTagName("guiaRemision").Count > 0)
                    guiaRemisioFac = xDocInt.GetElementsByTagName("guiaRemision")[0].InnerXml;
                else
                    guiaRemisioFac = "";

                if (xDocInt.GetElementsByTagName("obligadoContabilidad").Count > 0)
                    obligadoContabilidad = xDocInt.GetElementsByTagName("obligadoContabilidad")[0].InnerXml;
                else
                    obligadoContabilidad = "";

                if (xDocInt.GetElementsByTagName("contribuyenteEspecial").Count > 0)
                    numcontribuyente = xDocInt.GetElementsByTagName("contribuyenteEspecial")[0].InnerXml;
                else
                    numcontribuyente = "N/D";

                //actividadEmpresa = dty.Rows[0][4].ToString();  dirMatriz 

                if (xDocInt.GetElementsByTagName("dirMatriz").Count > 0)
                    direc = xDocInt.GetElementsByTagName("dirMatriz")[0].InnerXml;
                else
                    direc = "N/D";


                if (xDocInt.GetElementsByTagName("direccionComprador").Count > 0)
                    direccionComprador = xDocInt.GetElementsByTagName("direccionComprador")[0].InnerXml;
                else
                    direccionComprador = "N/D";


                if (xDocInt.GetElementsByTagName("dirEstablecimiento").Count > 0)
                    DireccionLocal = xDocInt.GetElementsByTagName("dirEstablecimiento")[0].InnerXml;
                else
                    DireccionLocal = direc;


                //ciudadLocal = dty.Rows[0][7].ToString();

                if (xDocInt.GetElementsByTagName("propina").Count > 0)
                    propina = xDocInt.GetElementsByTagName("propina")[0].InnerXml;
                else
                    propina = "0.00";


                numeroFactura = xDocInt.GetElementsByTagName("estab")[0].InnerXml + "-" + xDocInt.GetElementsByTagName("ptoEmi")[0].InnerXml + "-" + xDocInt.GetElementsByTagName("secuencial")[0].InnerXml;

                ambiente = xDocInt.GetElementsByTagName("ambiente")[0].InnerXml;
                if (ambiente == "1") { ambiente = "pruebas"; }
                if (ambiente == "2") { ambiente = "produccion"; }

                tipoEmision = xDocInt.GetElementsByTagName("tipoEmision")[0].InnerXml;
                if (tipoEmision == "1") { tipoEmision = "normal"; }
                if (tipoEmision == "2") { tipoEmision = "con indisponibilidad del sistema"; }

                claveAcceso = xDocInt.GetElementsByTagName("claveAcceso")[0].InnerXml;

                if (tipoDocumento != "11")
                    fechaEmision = xDocInt.GetElementsByTagName("fechaEmision")[0].InnerXml;
                else
                {
                    fechaIniTransporte = xDocInt.GetElementsByTagName("fechaIniTransporte")[0].InnerXml;
                    fechaFinTransporte = xDocInt.GetElementsByTagName("fechaFinTransporte")[0].InnerXml;
                    placa = xDocInt.GetElementsByTagName("placa")[0].InnerXml;
                }

                if (tipoDocumento == "9" || tipoDocumento == "10")
                {
                    razonSocialComprador = xDocInt.GetElementsByTagName("razonSocialSujetoRetenido")[0].InnerXml;
                    identificacionComprador = xDocInt.GetElementsByTagName("identificacionSujetoRetenido")[0].InnerXml;
                    periodoFiscal = xDocInt.GetElementsByTagName("periodoFiscal")[0].InnerXml;

                }
                else
                {
                    if (tipoDocumento != "11")
                    {
                        if (tipoDocumento != "13")
                        {
                            razonSocialComprador = xDocInt.GetElementsByTagName("razonSocialComprador")[0].InnerXml;
                            identificacionComprador = xDocInt.GetElementsByTagName("identificacionComprador")[0].InnerXml;
                        }
                        totalSinImpuestos = xDocInt.GetElementsByTagName("totalSinImpuestos")[0].InnerXml;
                    }

                }

                if (tipoDocumento == "13")
                {
                    razonSocialComprador = xDocInt.GetElementsByTagName("razonSocialProveedor")[0].InnerXml;
                    identificacionComprador = xDocInt.GetElementsByTagName("identificacionProveedor")[0].InnerXml;

                }


                if (tipoDocumento == "11")
                {
                    dirPartida = xDocInt.GetElementsByTagName("dirPartida")[0].InnerXml;
                    razonSocialTransportista = xDocInt.GetElementsByTagName("razonSocialTransportista")[0].InnerXml;
                    tipoIdentificacionTransportista = xDocInt.GetElementsByTagName("tipoIdentificacionTransportista")[0].InnerXml;
                    rucTransportista = xDocInt.GetElementsByTagName("rucTransportista")[0].InnerXml;
                    identificacionDestinatario = xDocInt.GetElementsByTagName("identificacionDestinatario")[0].InnerXml;
                    razonSocialDestinatario = xDocInt.GetElementsByTagName("razonSocialDestinatario")[0].InnerXml;
                    dirDestinatario = xDocInt.GetElementsByTagName("dirDestinatario")[0].InnerXml;
                    motivoTraslado = xDocInt.GetElementsByTagName("motivoTraslado")[0].InnerXml;
                    if(xDocInt.GetElementsByTagName("codEstabDestino").Count>0)
                    codEstabDestino = xDocInt.GetElementsByTagName("codEstabDestino")[0].InnerXml;
                    datosTransporte = @"<table>";
                    datosTransporte += @"<tr><td><br /><b>Identificación Transportista:</b> </td><td colspan=""3"">" + xDocInt.GetElementsByTagName("rucTransportista")[0].InnerXml + @"</td></tr>";
                    datosTransporte += @"<tr><td><br /><b>Razón Social / Nomrbes:</b> </td><td colspan=""3"">" + xDocInt.GetElementsByTagName("razonSocialTransportista")[0].InnerXml + @"</td></tr>";
                    datosTransporte += @"<tr><td><br /><b>Placa:</b> </td><td colspan=""3"">" + xDocInt.GetElementsByTagName("placa")[0].InnerXml + @"</td></tr>";
                    datosTransporte += @"<tr><td><br /><b>Punto de partida:</b> </td><td colspan=""3"">" + xDocInt.GetElementsByTagName("dirPartida")[0].InnerXml + @"</td></tr>";
                    datosTransporte += @"<tr><td><br /><b>Fecha inicio transporte:</b> </td><td>" + xDocInt.GetElementsByTagName("fechaIniTransporte")[0].InnerXml + @"</td>";
                    datosTransporte += @"<td><b> &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; fechaFinTransporte:</b> </td><td>" + xDocInt.GetElementsByTagName("fechaFinTransporte")[0].InnerXml + @"</td></tr>";
                    datosTransporte += @"</table>";


                    if (xDocInt.GetElementsByTagName("codDocSustento").Count > 0)
                    {
                        if (xDocInt.GetElementsByTagName("codDocSustento").Count > 0)
                            codDocSustento = xDocInt.GetElementsByTagName("codDocSustento")[0].InnerXml;

                        if (xDocInt.GetElementsByTagName("numDocSustento").Count > 0)
                            numDocSustento = xDocInt.GetElementsByTagName("numDocSustento")[0].InnerXml;

                        if (xDocInt.GetElementsByTagName("numAutDocSustento").Count > 0)
                            numAutDocSustento = xDocInt.GetElementsByTagName("numAutDocSustento")[0].InnerXml;

                        if (xDocInt.GetElementsByTagName("fechaEmisionDocSustento").Count > 0)
                            fechaEmisionDocSustento = xDocInt.GetElementsByTagName("fechaEmisionDocSustento")[0].InnerXml;

                        if (codDocSustento == "01") codDocSustento = "FACTURA";
                        if (codDocSustento == "03") codDocSustento = "Liquidación de compra";
                        if (codDocSustento == "04") codDocSustento = "NOTA DE CREDITO";
                        if (codDocSustento == "05") codDocSustento = "NOTA DE DEBITO";
                        if (codDocSustento == "06") codDocSustento = "GUIA DE REMISION";
                        if (codDocSustento == "07") codDocSustento = "COMPROBANTE DE RETENCION";

                    }

                    datosDestino = @"<table>";
                    if (xDocInt.GetElementsByTagName("codDocSustento").Count > 0)
                        datosDestino += @"<tr><td><br /><b>COMPROBANTE DE VENTA:</b> " + codDocSustento + "&nbsp;&nbsp;&nbsp;" + numDocSustento + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FECHA EMISION: " + fechaEmisionDocSustento + @"</td></tr>";
                    datosDestino += @"<tr><td><br /><b>Número de autorización:</b> " + numAutDocSustento + @"</td></tr>";
                    datosDestino += @"<tr><td><br /><b>Motivo traslado:</b> " + motivoTraslado + @"</td></tr>";
                    datosDestino += @"<tr><td><br /><b>Destino (Punto de llegada):</b> " + dirDestinatario + @"</td></tr>";
                    datosDestino += @"<tr><td><br /><b>Identificación (destinatario):</b> " + identificacionDestinatario + @"</td></tr>";
                    datosDestino += @"<tr><td><br /><b>Razón Social (destinatario):</b> " + razonSocialDestinatario + @"</td></tr>";
                    if (xDocInt.GetElementsByTagName("docAduaneroUnico").Count > 0)
                        datosDestino += @"<tr><td><br /><b>Documento Aduanero:</b> " + xDocInt.GetElementsByTagName("docAduaneroUnico")[0].InnerXml + @"</td></tr>";
                    if (xDocInt.GetElementsByTagName("codEstabDestino").Count > 0)
                        datosDestino += @"<tr><td><br /><b>Código Establecimiento Destino:</b> " + xDocInt.GetElementsByTagName("codEstabDestino")[0].InnerXml + @"</td></tr>";
                    if (xDocInt.GetElementsByTagName("ruta").Count > 0)
                        datosDestino += @"<tr><td><br /><b>Ruta:</b> " + xDocInt.GetElementsByTagName("ruta")[0].InnerXml + @"</td></tr>";
                    datosDestino += @"</table>";
                }

                // llenar forma de pago
                if (xDocInt.GetElementsByTagName("pagos").Count > 0)
                {
                    formaPago = @"<table>";
                    XmlNodeList ListaPagos = xDocInt.GetElementsByTagName("pagos");
                    XmlNodeList lista = ((XmlElement)ListaPagos[0]).GetElementsByTagName("pago");
                    formasPago fp = new formasPago();

                    foreach (XmlElement nodo in lista)
                    {
                        int i = 0;

                        // obtener codigo del tipo de pago
                        if (nodo.GetElementsByTagName("formaPago").Count > 0)
                        {
                            XmlNodeList formaPagoFac = nodo.GetElementsByTagName("formaPago");
                            formaPago += @"<tr><td><br /><b>Forma de Pago:</b> </td><td>" + fp.textoFp(formaPagoFac[i].InnerXml) + @"</td></tr>";
                        }

                        if (nodo.GetElementsByTagName("total").Count > 0)
                        {
                            XmlNodeList totalFac = nodo.GetElementsByTagName("total");
                            formaPago += @"<tr><td> / <b>Total:</b> </td><td>" + totalFac[i].InnerXml + @"</td></tr>";
                        }

                        if (nodo.GetElementsByTagName("plazo").Count > 0)
                        {
                            XmlNodeList plazo = nodo.GetElementsByTagName("plazo");
                            formaPago += @"<tr><td> / <b>Plazo:</b> </td><td>" + plazo[i].InnerXml + @"</td></tr>";
                        }

                        if (nodo.GetElementsByTagName("unidadTiempo").Count > 0)
                        {
                            XmlNodeList unidadTiempo = nodo.GetElementsByTagName("unidadTiempo");
                            formaPago += @"<tr><td> / <b>Unidad de tiempo:</b> </td><td>" + unidadTiempo[i].InnerXml + @"<br /></td></tr>";
                        }

                        i++;
                    }
                    //fin foreach
                    formaPago += @"</table>";
                }



                // version anterior campoAdicional
                //informacionAdicional = @"<table>";
                //if (xDocInt.GetElementsByTagName("campoAdicional")[0] != null)
                //{
                //    campoAdicional = xDocInt.GetElementsByTagName("campoAdicional")[0].InnerXml;
                //    informacionAdicional += @"<tr><td><br /><b>Email:</b> </td><td>" + campoAdicional + @"</td></tr>";
                //}

                //version nueva campoAdicional
                if (xDocInt.GetElementsByTagName("campoAdicional").Count > 0)
                {
                    informacionAdicional = @"<table>";
                    XmlNodeList lista = xDocInt.GetElementsByTagName("campoAdicional");
                    foreach (XmlElement nodo in lista) 
                    {

                        informacionAdicional += @"<tr><td><br /><b>" + nodo.Attributes["nombre"].Value + ":</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Nac"))
                            //nac = "Agente de Retención Resolución NAC-DNCRASC20-00000001"; cmina 2025-06-04
                            nac = nodo.InnerXml.Substring(30); //"NRO. NAC-DGERCGC25-00000010";
                        if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Micro"))
                            micro = "Contribuyente Régimen Microempresas";
                        if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Rimpe"))
                            rimpe = "Contribuyente Régimen Rimpe";

                        if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "valorServicioAdicional"))
                        {
                            valorServicio = nodo.InnerXml;
                            cobraServicio = true;
                        }


                        //if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "EmailCliente"))
                        //	informacionAdicional += @"<tr><td><br /><b>Email:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Emailcliente"))
                        //	informacionAdicional += @"<tr><td><br /><b>Email:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "CorreoCliente"))
                        //	informacionAdicional += @"<tr><td><br /><b>Email:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Email"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Email:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "email"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Email:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "EMAIL"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Email:</b> </td><td>" + nodo.InnerXml.Replace(";", "<br>") + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "E-MAIL"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Email:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Direccion"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Dirección:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Telefono"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Teléfono:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Medico"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Médico:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Paciente"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Paciente:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "HistoriaClinica"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Historia Clínica:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Orden Compra"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Orden de compra:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Codigo Almacen"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Código Almacén:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Observacion"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Observación:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "OBSERVACION"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Observación:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Concepto"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Concepto:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Fecha Cirugia"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Fecha cirugía:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Lugar"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Lugar:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "OrdenCompra"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Orden de Compra:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Nota"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Nota:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Propina"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Propina:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "ReferenciaPedido"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Referencia Nro Pedido:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Do"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Do:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Ref"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Ref:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "De"))
                        //                      informacionAdicional += @"<tr><td><br /><b>De:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Para"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Para:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Embarque"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Embarque:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Trayecto"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Trayecto:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "ID DEVOLUCION"))
                        //                      informacionAdicional += @"<tr><td><br /><b>ID DEVOLUCION:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "INCONTERM"))
                        //                      informacionAdicional += @"<tr><td><br /><b>INCONTERM:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Partida Sugerida"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Partida Sugerida:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Peso Neto"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Peso Neto:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Peso Bruto"))
                        //                      informacionAdicional += @"<tr><td><br /><b>Peso Bruto:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Ciudad"))
                        //	informacionAdicional += @"<tr><td><br /><b>Ciudad:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Contacto de la empresa"))
                        //	informacionAdicional += @"<tr><td><br /><b>Contacto de la empresa:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "CAJERO"))
                        //	informacionAdicional += @"<tr><td><br /><b>CAJERO:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "TRANSACCION"))
                        //	informacionAdicional += @"<tr><td><br /><b>TRANSACCION:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "FORMA DE PAGO"))
                        //	informacionAdicional += @"<tr><td><br /><b>FORMA DE PAGO:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "GRATIFICACION"))
                        //	informacionAdicional += @"<tr><td><br /><b>GRATIFICACION:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "FECHA CIERRE"))
                        //	informacionAdicional += @"<tr><td><br /><b>FECHA CIERRE:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "TRANSPORTADO POR"))
                        //	informacionAdicional += @"<tr><td><br /><b>TRANSPORTADO POR:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "URL"))
                        //                      informacionAdicional += @"<tr><td><br /><b>URL:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "DIRECCION"))
                        //                      informacionAdicional += @"<tr><td><br /><b>DIRECCION:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "TELEFONO"))
                        //                      informacionAdicional += @"<tr><td><br /><b>TELEFONO:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "ENTRADA"))
                        //                      informacionAdicional += @"<tr><td><br /><b>ENTRADA:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "USUARIO"))
                        //                      informacionAdicional += @"<tr><td><br /><b>USUARIO:</b> </td><td>" + nodo.InnerXml + @"</td></tr>";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Nac"))
                        //                      nac = nodo.InnerXml.Substring(30); // cmina 2025-06-10
                        //                  //"Agente de Retención Resolución NAC-DNCRASC20-00000001";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Micro"))
                        //                      micro = "Contribuyente Régimen Microempresas";
                        //                  if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Rimpe"))
                        //                      rimpe = "Contribuyente Régimen Rimpe";


                    }
                    informacionAdicional += @"</table>";
                }

                //código para facturación  
                if (tipoDocumento == "2" || tipoDocumento == "1" || tipoDocumento == "12" || tipoDocumento == "13")
                {
                    if (tipoDocumento == "12") {
                        importeTotal = xDocInt.GetElementsByTagName("valorTotal")[0].InnerXml;
                    }
                    else { 
                    totalDescuento = xDocInt.GetElementsByTagName("totalDescuento")[0].InnerXml;
                    importeTotal = xDocInt.GetElementsByTagName("importeTotal")[0].InnerXml;
                    }
                }

                //código para notas de crédito
                if (tipoDocumento == "4" || tipoDocumento == "3" || tipoDocumento == "12")
                {
                    if(tipoDocumento != "12") { 
                    importeTotal = xDocInt.GetElementsByTagName("valorModificacion")[0].InnerXml;
                    motivo = xDocInt.GetElementsByTagName("motivo")[0].InnerXml;
                    }

                    if (xDocInt.GetElementsByTagName("fechaEmisionDocSustento").Count > 0)
                        fechaEmisionDocSustento = xDocInt.GetElementsByTagName("fechaEmisionDocSustento")[0].InnerXml;

                    if (xDocInt.GetElementsByTagName("numDocModificado").Count > 0)
                        numDocModificado = xDocInt.GetElementsByTagName("numDocModificado")[0].InnerXml;

                }

                if (tipoDocumento != "9" && tipoDocumento != "10" && tipoDocumento != "11")
                {
                    XmlNodeList lista;

                    if (tipoDocumento == "12"||tipoDocumento == "3")
                    {
                        if (xDocInt.GetElementsByTagName("codDocModificado").Count > 0) {

                            if (xDocInt.GetElementsByTagName("codDocModificado")[0].InnerXml == "01")
                                tipoDocModificado = "FACTURA";
                            if (xDocInt.GetElementsByTagName("codDocModificado")[0].InnerXml == "04")
                                tipoDocModificado = "NOTA DE CREDITO";
                            if (xDocInt.GetElementsByTagName("codDocModificado")[0].InnerXml == "05")
                                tipoDocModificado = "NOTA DE DEBITO";
                        }

                        if (tipoDocumento != "3")
                        {
                            XmlNodeList totalConImpuestos = xDocInt.GetElementsByTagName("impuestos");
                            lista = ((XmlElement)totalConImpuestos[0]).GetElementsByTagName("impuesto");
                        }
                        else
                        {
                            XmlNodeList totalConImpuestos = xDocInt.GetElementsByTagName("totalConImpuestos");
                            lista = ((XmlElement)totalConImpuestos[0]).GetElementsByTagName("totalImpuesto");
                        }

                    }
                    else
                    {
                        XmlNodeList totalConImpuestos = xDocInt.GetElementsByTagName("totalConImpuestos");
                        lista = ((XmlElement)totalConImpuestos[0]).GetElementsByTagName("totalImpuesto");
                    }
                    //inicio foreach

                    foreach (XmlElement nodo in lista)
                    {

                        int i = 0;

                        XmlNodeList codigo = nodo.GetElementsByTagName("codigo");

                        XmlNodeList codigoPorcentaje = nodo.GetElementsByTagName("codigoPorcentaje");

                        XmlNodeList baseImponible = nodo.GetElementsByTagName("baseImponible");

                        XmlNodeList valor = nodo.GetElementsByTagName("valor");

                        XmlNodeList tarifa = nodo.GetElementsByTagName("tarifa");


                        if (codigo[i].InnerXml == "2" && codigoPorcentaje[i].InnerXml == "0")
                        {
                            subTotalIva0 = baseImponible[i].InnerXml;
                        }
                        if (codigo[i].InnerXml == "2" && codigoPorcentaje[i].InnerXml == "2")
                        {
                            subTotalIva12 = baseImponible[i].InnerXml;
                            valorIVA12 = valor[i].InnerXml;
                            valorPorcentajeImpuesto = "12";
                        }
                        if (codigo[i].InnerXml == "2" && codigoPorcentaje[i].InnerXml == "3")
                        {
                            subTotalIva12 = baseImponible[i].InnerXml;
                            valorIVA12 = valor[i].InnerXml;
                            valorPorcentajeImpuesto = "14";
                        }
                        if (codigo[i].InnerXml == "2" && codigoPorcentaje[i].InnerXml == "8")
                        {
                            subTotalIva12 = baseImponible[i].InnerXml;
                            valorIVA12 = valor[i].InnerXml;
                            valorPorcentajeImpuesto = "8";
                        }
                        if (codigo[i].InnerXml == "2" && codigoPorcentaje[i].InnerXml == "4")
                        {
                            subTotalIva12 = baseImponible[i].InnerXml;
                            valorIVA12 = valor[i].InnerXml;
                            valorPorcentajeImpuesto = "15";
                        }
                        //valorPorcentajeImpuesto = tarifa[i].InnerXml;
                        i++;
                    }
                    //fin foreach
                }

                if (string.IsNullOrEmpty(subTotalIva0))
                {
                    subTotalIva0 = "0.00";
                }
                if (string.IsNullOrEmpty(subTotalIva12))
                {
                    subTotalIva12 = "0.00";
                }
                if (string.IsNullOrEmpty(valorIVA12))
                {
                    valorIVA12 = "0.00";
                }

                //detalle inicio
                string xml = string.Empty;
                if (tipoDocumento == "9" || tipoDocumento == "10")
                {
                    XmlNodeList impuestos = xDocInt.GetElementsByTagName("impuestos");
                    XmlNodeList listaDetalle = ((XmlElement)impuestos[0]).GetElementsByTagName("impuesto");
                    xml = @"<?xml version=""1.0"" encoding=""utf-8""?><impuestos>" + impuestos[0].InnerXml + "</impuestos>";
                }
                else if (tipoDocumento != "12")
                {
                    XmlNodeList detalles = xDocInt.GetElementsByTagName("detalles");
                    XmlNodeList listaDetalle = ((XmlElement)detalles[0]).GetElementsByTagName("detalle");
                    xml = @"<?xml version=""1.0"" encoding=""utf-8""?><detalles>" + detalles[0].InnerXml + "</detalles>";
                }
                if (tipoDocumento == "12" && xDocInt.GetElementsByTagName("motivos").Count > 0)
                {
                    XmlNodeList ListaMotDeb = xDocInt.GetElementsByTagName("motivos");
                    XmlNodeList listaMot = ((XmlElement)ListaMotDeb[0]).GetElementsByTagName("motivo");
                    xml = @"<?xml version=""1.0"" encoding=""utf-8""?><motivos>" + ListaMotDeb[0].InnerXml + "</motivos>";
                }

                // dsDetalle es el detalle del reporte, el datasource del reporte
                DataSet dsDetalle = new DataSet();
                dsDetalle.ReadXml(new XmlTextReader(new StringReader(xml)));

                //detalle fin
                Telerik.Reporting.InstanceReportSource instanceReportSource1 = new Telerik.Reporting.InstanceReportSource();

                if (tipoDocumento == "1")
                {
                    if (ruc == "1792750555001" || ruc == "1792583438001" || ruc == "1792222206001")
                    {
                        if (dsDetalle.Tables.Count > 0)
                        {
                            dsDetalle.Tables[0].Columns.Add("detAdicional1", typeof(String));
                            dsDetalle.Tables[0].Columns.Add("detAdicional2", typeof(String));
                            dsDetalle.Tables[0].Columns.Add("detAdicional3", typeof(String));

                            foreach (DataRow dr in dsDetalle.Tables[0].Rows)
                            {
                                int contador = 7;
                                foreach (DataRow dr2 in dsDetalle.Tables[2].Rows)
                                {
                                    if (contador < 10 && dr2[2].ToString() == dr[6].ToString())
                                    {
                                        dr[contador] = dr2[0].ToString() + " " + dr2[1].ToString();
                                        contador++;
                                    }
                                }

                            }
                        }

                        if (!string.IsNullOrEmpty(pathLogo))
                        {

                            rideFacV2 rideRpt = new rideFacV2(pathLogo, largoLogo, altoLogo);
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                        else
                        {
                            rideFacV2 rideRpt = new rideFacV2();
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                    }
                    else if (ruc == "1791949862001")//bandainicio
                    {
                        if (dsDetalle.Tables.Count > 0)
                        {
                            dsDetalle.Tables[0].Columns.Add("detAdicional3", typeof(String));

                            foreach (DataRow dr in dsDetalle.Tables[0].Rows)
                            {

                                int contador = 1;
                                foreach (DataRow dr2 in dsDetalle.Tables[2].Rows)
                                {
                                    if (contador <= 3 && dr2[2].ToString() == dr[6].ToString())
                                    {
                                        if (contador <= 2)
                                            dr[1] += " / " + dr2[1].ToString();

                                        contador++;

                                        if (contador == 4)
                                        {
                                            dr[7] = dr2[1].ToString();
                                        }
                                    }

                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(pathLogo))
                        {

                            rideFacV3 rideRpt = new rideFacV3(pathLogo, largoLogo, altoLogo);
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                        else
                        {
                            rideFacV3 rideRpt = new rideFacV3();
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                    }
                    //bandafin
                    // brico inicio 1791845870001
                    else if (ruc == "1791845870001")
                    {

                        if (!string.IsNullOrEmpty(pathLogo))
                        {
                            
                            if (!dsDetalle.Tables[0].Columns.Contains("codigoAuxiliar"))
                            {
                                dsDetalle.Tables[0].Columns.Add("codigoAuxiliar", typeof(String));
                            }
                            
                            rideFacV4 rideRpt = new rideFacV4(pathLogo, largoLogo, altoLogo);
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                        else
                        {
                            rideFacV4 rideRpt = new rideFacV4();
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                    }

                    // brico fin 1791845870001
                    // adexus inicio
                    else if (ruc == "0991339957001")
                    {
                        if (!string.IsNullOrEmpty(pathLogo))
                        {
                            if (!dsDetalle.Tables[0].Columns.Contains("codigoAuxiliar"))
                            {
                                dsDetalle.Tables[0].Columns.Add("codigoAuxiliar", typeof(String));
                            }
                            if (!dsDetalle.Tables[0].Columns.Contains("detAdicional"))
                            {
                                dsDetalle.Tables[0].Columns.Add("detAdicional", typeof(String));
                            }

                            foreach (DataRow dr in dsDetalle.Tables[0].Rows)
                            {
                                string detAdicional = string.Empty;
                                
                                //int contador = 1;
                                foreach (DataRow dr2 in dsDetalle.Tables[2].Rows)
                                {
                                    if (dr2[2].ToString() == dr[6].ToString())
                                    {
                                        dr2[1] = dr2[1].ToString().Replace("||", "√");
                                        detAdicional +=dr2[1].ToString().Replace('√', (char)10) + (char)13 + (char)10 + (char)13 + (char)10;
                                    }

                                }
                                dr[8]=detAdicional;

                            }
                            rideFacV5 rideRpt = new rideFacV5(pathLogo, largoLogo, altoLogo);
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                        else
                        {
                            rideFacV5 rideRpt = new rideFacV5();
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }

                    }
                    // adexus fin
                    else
                    {
                        if(!cobraServicio)
                        { 
                            if (!string.IsNullOrEmpty(pathLogo))
                            {

                                rideFact rideRpt = new rideFact(pathLogo, largoLogo, altoLogo);
                                rideRpt.DataSource = dsDetalle.Tables[0];
                                instanceReportSource1.ReportDocument = rideRpt;
                            }
                            else
                            {
                                rideFact rideRpt = new rideFact();
                                rideRpt.DataSource = dsDetalle.Tables[0];
                                instanceReportSource1.ReportDocument = rideRpt;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(pathLogo))
                            {

                                rideFactServ rideRpt = new rideFactServ(pathLogo, largoLogo, altoLogo);
                                rideRpt.DataSource = dsDetalle.Tables[0];
                                instanceReportSource1.ReportDocument = rideRpt;
                            }
                            else
                            {
                                rideFactServ rideRpt = new rideFactServ();
                                rideRpt.DataSource = dsDetalle.Tables[0];
                                instanceReportSource1.ReportDocument = rideRpt;
                            }
                        }
                    }

                }

                if (tipoDocumento == "13")
                {
                    if (!string.IsNullOrEmpty(pathLogo))
                    {

                        rideFactLiquidacion rideRptLiquidacion = new rideFactLiquidacion(pathLogo, largoLogo, altoLogo);
                        rideRptLiquidacion.DataSource = dsDetalle.Tables[0];
                        instanceReportSource1.ReportDocument = rideRptLiquidacion;
                    }
                    else
                    {
                        rideFactLiquidacion rideRptLiquidacion = new rideFactLiquidacion();
                        rideRptLiquidacion.DataSource = dsDetalle.Tables[0];
                        instanceReportSource1.ReportDocument = rideRptLiquidacion;
                    }
                }


                if (tipoDocumento == "3")
                {
                    if (ruc == "1791949862001") //bandainicio
                    {
                        if (dsDetalle.Tables.Count > 0)
                        {
                            dsDetalle.Tables[0].Columns.Add("detAdicional3", typeof(String));

                            foreach (DataRow dr in dsDetalle.Tables[0].Rows)
                            {

                                int contador = 1;
                                foreach (DataRow dr2 in dsDetalle.Tables[2].Rows)
                                {
                                    if (contador <= 3 && dr2[2].ToString() == dr[6].ToString())
                                    {
                                        if (contador <= 2)
                                            dr[1] += " / " + dr2[1].ToString();

                                        contador++;

                                        if (contador == 4)
                                        {
                                            dr[7] = dr2[1].ToString();
                                        }
                                    }

                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(pathLogo))
                        {

                            rideNcV2 rideRpt = new rideNcV2(pathLogo, largoLogo, altoLogo);
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                        else
                        {
                            rideNcV2 rideRpt = new rideNcV2();
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                    }
                    //bandafin
                    // brico inicio 1791845870001
                    else if (ruc == "1791845870001")
                    {

                        if (!string.IsNullOrEmpty(pathLogo))
                        {
                            if (!dsDetalle.Tables[0].Columns.Contains("codigoAdicional"))
                            dsDetalle.Tables[0].Columns.Add("codigoAdicional", typeof(String)); 
                            rideNcV3 rideRpt = new rideNcV3(pathLogo, largoLogo, altoLogo);
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                        else
                        {
                            rideNcV3 rideRpt = new rideNcV3();
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                    }

                    // brico fin 1791845870001
                    //inicio adexus
                    else if (ruc == "0991339957001")
                    {
                        if (!string.IsNullOrEmpty(pathLogo))
                        {
                            if (!dsDetalle.Tables[0].Columns.Contains("detAdicional"))
                            {
                                dsDetalle.Tables[0].Columns.Add("detAdicional", typeof(String));
                            }

                            foreach (DataRow dr in dsDetalle.Tables[0].Rows)
                            {
                                string detAdicional = string.Empty;

                                //int contador = 1;
                                foreach (DataRow dr2 in dsDetalle.Tables[2].Rows)
                                {
                                    if (dr2[2].ToString() == dr[6].ToString())
                                    {
                                        dr2[1] = dr2[1].ToString().Replace("||", "√");
                                        detAdicional += dr2[1].ToString().Replace('√', (char)10) + (char)13 + (char)10 + (char)13 + (char)10;
                                    }

                                }
                                dr[7] = detAdicional;

                            }
                            rideNcV4 rideRpt = new rideNcV4(pathLogo, largoLogo, altoLogo);
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                        else
                        {
                            rideNcV4 rideRpt = new rideNcV4();
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }

                    }

                    //fin adexus
                    else
                    {
                        if (!string.IsNullOrEmpty(pathLogo))
                        {

                            rideNc rideRpt = new rideNc(pathLogo, largoLogo, altoLogo);
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                        else
                        {
                            rideNc rideRpt = new rideNc();
                            rideRpt.DataSource = dsDetalle.Tables[0];
                            instanceReportSource1.ReportDocument = rideRpt;
                        }
                    }

                }

                //if (dtB.Rows[0][1].ToString() == "4")
                //{
                //    verNc rideRpt = new verNc();
                //    rideRpt.DataSource = dsDetalle.Tables[0];
                //    instanceReportSource1.ReportDocument = rideRpt;
                //}
                if (tipoDocumento == "9")
                {

                    if (!string.IsNullOrEmpty(pathLogo))
                    {

                        rideRet rideRpt = new rideRet(pathLogo, largoLogo, altoLogo);
                        rideRpt.DataSource = dsDetalle.Tables[0];
                        instanceReportSource1.ReportDocument = rideRpt;
                    }
                    else
                    {
                        rideRet rideRpt = new rideRet();
                        rideRpt.DataSource = dsDetalle.Tables[0];
                        instanceReportSource1.ReportDocument = rideRpt;
                    }
                }

                if (tipoDocumento == "11")
                {

                    if (!string.IsNullOrEmpty(pathLogo))
                    {

                        rideRem rideRpt = new rideRem(pathLogo, largoLogo, altoLogo);
						rideRpt.DataSource = dsDetalle.Tables[0];
						if (!dsDetalle.Tables[0].Columns.Contains("codigoAdicional"))
							dsDetalle.Tables[0].Columns.Add("codigoAdicional", typeof(String));
                        instanceReportSource1.ReportDocument = rideRpt;
                    }
                    else
                    {
                        rideRem rideRpt = new rideRem();
                        rideRpt.DataSource = dsDetalle.Tables[0];
						if (!dsDetalle.Tables[0].Columns.Contains("codigoAdicional"))
							dsDetalle.Tables[0].Columns.Add("codigoAdicional", typeof(String));
                        instanceReportSource1.ReportDocument = rideRpt;
                    }
                }

                if (tipoDocumento == "12")
                {
                    if (!string.IsNullOrEmpty(pathLogo))
                    {
                        rideDeb rideDebito = new rideDeb(pathLogo, largoLogo, altoLogo);
                        rideDebito.DataSource = dsDetalle.Tables[0];
                        instanceReportSource1.ReportDocument = rideDebito;
                    }
                    else
                    {
                        rideDeb rideDebito = new rideDeb();
                        rideDebito.DataSource = dsDetalle.Tables[0];
                        instanceReportSource1.ReportDocument = rideDebito;
                    }
                }

                //if (dtB.Rows[0][1].ToString() == "10")
                //{
                //    verRet verRpt = new verRet();
                //    verRpt.DataSource = dsDetalle.Tables[0];
                //    instanceReportSource1.ReportDocument = verRpt;
                //}
                razon = razon.Replace("&amp;", "&");
                razonSocialComprador = razonSocialComprador.Replace("&amp;", "&");
                instanceReportSource1.Parameters.Clear();
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("ruc", ruc));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("razon", razon));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("obligadoContabilidad", obligadoContabilidad));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("numcontribuyente", numcontribuyente));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("actividadEmpresa", ""));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("direc", direc));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("DireccionLocal", DireccionLocal));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("ciudadLocal", ciudadLocal));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("numeroFactura", numeroFactura));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("ambiente", ambiente));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("tipoEmision", tipoEmision));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("claveAcceso", claveAcceso));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("fechaEmision", fechaEmision));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("razonSocialComprador", razonSocialComprador));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("identificacionComprador", identificacionComprador));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("totalSinImpuestos", totalSinImpuestos));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("totalDescuento", totalDescuento));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("importeTotal", importeTotal));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("subTotalIva0", subTotalIva0));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("subTotalIva12", subTotalIva12));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("valorIVA12", valorIVA12));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("numeroAutorizacion", numeroAutorizacion));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("fechaAutorizacion", fechaAutorizacion));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("porcentajeValor", valorPorcentajeImpuesto + "%"));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("InformacionAdicional", informacionAdicional));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("nac", nac));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("micro", micro));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("rimpe", rimpe));
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("direccionComprador", direccionComprador));
                
                if(cobraServicio)
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("servicio", valorServicio));


                if (ruc == "1791949862001" || tipoDocumento == "1") 
                instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("guiaRemisioFac", guiaRemisioFac));

                if (tipoDocumento == "12"|| tipoDocumento == "3")
                {
                   instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("tipoDocModificado", tipoDocModificado));
                }

                if (tipoDocumento == "11")
                { 
                    instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("infoTransporte", datosTransporte));
                    instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("infoTraslado", datosDestino));
                }


                if (tipoDocumento == "1" || tipoDocumento == "3" || tipoDocumento == "12"|| tipoDocumento == "13")
                    {
                    if (tipoDocumento != "12" && tipoDocumento != "13")
                    {
                        if(cobraServicio)
                        propina= Decimal.Round(Convert.ToDecimal(propina)-Convert.ToDecimal(valorServicio), 2).ToString();
                        instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("propina", propina));
                    }
                        if (tipoDocumento != "3")
                            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("formaPago", formaPago));
                    }
                

                if (tipoDocumento == "4" || tipoDocumento == "3" || tipoDocumento == "12")
                {
                    instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("fechaEmisionDocSustento", fechaEmisionDocSustento));

                    if (tipoDocumento != "12")
                    instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("motivo", motivo));

                    instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("numDocModificado", numDocModificado));
                }
                if (tipoDocumento == "9")
                {
                    instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("periodoFiscal", periodoFiscal));
                }


                ReportProcessor reportProcessor = new ReportProcessor();
                //Telerik.Reporting.InstanceReportSource instanceReportSource = new Telerik.Reporting.InstanceReportSource();
                //instanceReportSource.ReportDocument = instanceReportSource1.ReportDocument;
                RenderingResult result = reportProcessor.RenderReport("PDF", instanceReportSource1, null);
                //string nombreArchivo = @"\\192.168.1.30\documentos\pdf\" + nombreArchRef + ".pdf";

                //string MyString = nombreArchRef;
                //char[] MyChar = { 'x', 'm', 'l'};
                //nombreArchRef = MyString.TrimEnd(MyChar);


                //ubicacionArchivo = Session["pathCargas"] + "reenvio/" + nombreArchRef + "pdf"; 
                ubicacionArchivo = pathArchivoPdf;
                using (FileStream fs = new FileStream(ubicacionArchivo, FileMode.Create))
                {
                    fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
                }

				if (File.Exists(ubicacionArchivo))
					return "1";
				else
					return "0";  
            }
            catch (Exception ex)
            {
                return ex.Message;
                //StringBuilder strScriptB = new StringBuilder();
                //strScriptB.Append("<script language='JavaScript'>");
                //strScriptB.Append("alert('Error no se pudo reenviar el correo:" + ex.Message + "');");
                //strScriptB.Append("</script>");
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "ClientScriptA", strScriptB.ToString(), false);
                //strScriptB.Clear();
            }

        }
      }
    }

