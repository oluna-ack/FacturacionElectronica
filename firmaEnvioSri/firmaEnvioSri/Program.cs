using firmarStdSri;
using System.Data;
using firmaEnvioSri.App_Code.BLL;
using System;
using System.IO;
using System.Xml;
using System.Net.Mail;
using System.Collections.Generic;
using System.Configuration;

namespace firmaEnvioSri
{
    public class Program
    {
        static void Main(string[] args)// P = produccion T=pruebas
        {
            ///////////////
            string pathFile = string.Empty;
            string pathFileB = string.Empty;
            string plataforma = string.Empty; //// P = produccion T=pruebas
			int ambienteAck = 1;
            string empresa = string.Empty, clave = string.Empty, numComp = string.Empty, tipoDoc = string.Empty, cliente = string.Empty, idCliente = string.Empty;
            string email1 = string.Empty;
            string email2 = string.Empty;
            string emailEmpre = string.Empty;
            string Ruc = string.Empty;
            string FechaRegistro = string.Empty;
            int id;
            

            Console.Title = "Proceso de Firma  de comprobantes - Acklins";
            Console.WriteLine("Inicio  proceso de FirmaEnvioSri...");
           
            try
            {

                // Leer correos desde configuración
                string correos = ConfigurationManager.AppSettings["EmailAcklins"];
                string[] listaCorreos = correos?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

                if (listaCorreos.Length > 0)
                    email1 = listaCorreos[0].Trim();

                if (listaCorreos.Length > 1)
                    email2 = listaCorreos[1].Trim();
                if (args.Length == 0)
                        throw new ArgumentException("No se ha especificado el parámetro de plataforma (P o T).");

                    plataforma = args[0];
                    //plataforma = "P";

                    if (plataforma == "P")
                        ambienteAck = 2;

                    if (plataforma == "P")
                    {
                        pathFile = @"\\10.0.0.4\documentos\xmlSinFirma\autorizado\";
                        pathFileB = @"\\10.0.0.4\documentos\";
                    }
                    else if (plataforma == "T")
                    {
                        pathFile = @"\\10.0.0.4\documentosTest\xmlSinFirma\autorizado\";
                        pathFileB = @"\\10.0.0.4\documentosTest\"; // <-- corregida IP 110.0.0.4 por 10.0.0.4
                    }
                    else
                    {
                        throw new ArgumentException("Parámetro de plataforma no reconocido. Use 'P' para Producción o 'T' para Test.");
                    }

            
                DataTable dt = clsVariablesFirma.objfirma.listaXmlPorFirmar(ambienteAck);
          
                var tiposDocumento = new Dictionary<string, TipoDocumento>
                {
                    { "1", new TipoDocumento { TipoDoc = "FACTURA", Titulo = "Factura" } },
                    { "3", new TipoDocumento { TipoDoc = "NOTA DE CREDITO", Titulo = "Nota de crédito" } },
                    { "12", new TipoDocumento { TipoDoc = "NOTA DE DEBITO", Titulo = "Nota de débito" } },
                    { "11", new TipoDocumento { TipoDoc = "GUIA DE REMISION", Titulo = "Guía de Remisión" } },
                    { "9", new TipoDocumento { TipoDoc = "COMPROBANTE DE RETENCION", Titulo = "Retenciones" } },
                    { "13", new TipoDocumento { TipoDoc = "LIQUIDACION DE COMPRAS", Titulo = "Liquidación de compra" } }
                };


                foreach (DataRow dtRow in dt.Rows)
                {
                    id = Convert.ToInt32(dtRow[0]);
                    empresa = dtRow[2].ToString();
                    clave = dtRow[7].ToString();
                    numComp = dtRow[30].ToString();
                    cliente = dtRow[28].ToString();
                    idCliente = dtRow[10].ToString();
                    FechaRegistro = dtRow[12].ToString();

                    string codigo = dtRow[4].ToString();
                    TipoDocumento doc;
                    if (tiposDocumento.TryGetValue(codigo, out doc))
                    {
                        tipoDoc = doc.TipoDoc;
                    }


                    //emailEmpre = clsVariablesFirma.objfirma.devuelveEmail(empresa).Rows[0][0].ToString();

                    DataTable dtEmail = clsVariablesFirma.objfirma.devuelveEmail(empresa);
                    if (dtEmail.Rows.Count > 0 && dtEmail.Rows[0][0] != DBNull.Value)
                    {
                        emailEmpre = dtEmail.Rows[0][0].ToString();
                    }
                    else
                    {
                        emailEmpre = string.Empty; // o un valor predeterminado
                        EnviarCorreo("feletronica@acklins.online", email1, email2, "feletronica@acklins.online", "No se encontró direccion de correo", empresa, clave, numComp, tipoDoc, cliente, idCliente, "");

                    }

                    bool firmado;
                    string respuestaEnvio;

                    DataTable dtBasesServer = new DataTable();
                    try
                    {
                        dtBasesServer = clsVariablesFirma.objfirma.listaBaseDatosServer(dtRow[2].ToString());
                        
                        Ruc = dtBasesServer.Rows[0][11].ToString();


                        Console.WriteLine("Consulta Informacion Firma");
                        //Console.ReadKey();

                        firmaSri firmar = new firmaSri();
                        operacionesComprobantes operaciones = new operacionesComprobantes();

                        if (File.Exists(dtRow[11].ToString()))
                        {

                            Console.WriteLine("Empieza proceso de Firma");
                            //Console.ReadKey();


                            XmlDocument xDoc = new XmlDocument();
                            xDoc.Load(dtRow[11].ToString());

                            int contadorNac = 0;
                            int contadorMicro = 0;
                            int contadorRimpe = 0;

                            // valida y asegurar que los nodos de leyenda de la base de datos sean los mismos en el xml y no estén duplicados
                            if (xDoc.GetElementsByTagName("campoAdicional").Count > 0)
                            {
                                XmlNodeList lista = xDoc.GetElementsByTagName("campoAdicional");
                                foreach (XmlElement nodo in lista)  // validamos que el xml contenga las leyendas indicas en la base de datos y que el nodo esté 1 sola vez
                                {
                                    if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Nac"))
                                        contadorNac++;
                                    if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Micro"))
                                        contadorMicro++;
                                    if (nodo.Attributes["nombre"] != null && (nodo.Attributes["nombre"].Value == "Rimpe"))
                                        contadorRimpe++;
                                }

                                if (contadorRimpe > 1)
                                {
                                    for (int i = lista.Count - 1; i >= 0; i--)
                                    {
                                        if (lista[i].Attributes["nombre"] != null && (lista[i].Attributes["nombre"].Value == "Rimpe"))
                                            lista[i].ParentNode.RemoveChild(lista[i]);
                                    }
                                    contadorRimpe = 0;
                                }

                                if (contadorNac > 1)
                                {
                                    for (int i = lista.Count - 1; i >= 0; i--)
                                    {
                                        if (lista[i].Attributes["nombre"] != null && (lista[i].Attributes["nombre"].Value == "Nac"))
                                            lista[i].ParentNode.RemoveChild(lista[i]);
                                    }
                                    contadorNac = 0;
                                }
                                if (contadorMicro > 1)
                                {
                                    for (int i = lista.Count - 1; i >= 0; i--)
                                    {
                                        if (lista[i].Attributes["nombre"] != null && (lista[i].Attributes["nombre"].Value == "Micro"))
                                            lista[i].ParentNode.RemoveChild(lista[i]);
                                    }
                                    contadorMicro = 0;
                                }
                            }
                            ///


          

                            if (dtBasesServer.Rows[0][23].ToString() == "True" && contadorRimpe == 0)
                            {
                                XmlNode campoAdicional = xDoc.CreateNode(XmlNodeType.Element, "campoAdicional", null);
                                XmlAttribute attr = xDoc.CreateAttribute("nombre");
                                attr.Value = "Rimpe";
                                campoAdicional.Attributes.Append(attr);
                                campoAdicional.InnerText = "CONTRIBUYENTE REGIMEN RIMPE";
                                xDoc.GetElementsByTagName("infoAdicional")[0].AppendChild(campoAdicional);

                                //if (xDoc.GetElementsByTagName("regimenMicroempresas").Count == 0)
                                //{
                                //    XmlNode regimenMicroempresas = xDoc.CreateNode(XmlNodeType.Element, "regimenMicroempresas", null);
                                //    xDoc.GetElementsByTagName("infoTributaria")[0].AppendChild(regimenMicroempresas);
                                //    xDoc.GetElementsByTagName("regimenMicroempresas")[0].InnerText = "CONTRIBUYENTE RÉGIMEN MICROEMPRESAS";
                                //}
                            }

                            if (dtBasesServer.Rows[0][22].ToString() == "True" && contadorMicro == 0)
                            {
                                XmlNode campoAdicional = xDoc.CreateNode(XmlNodeType.Element, "campoAdicional", null);
                                XmlAttribute attr = xDoc.CreateAttribute("nombre");
                                attr.Value = "Micro";
                                campoAdicional.Attributes.Append(attr);
                                campoAdicional.InnerText = "CONTRIBUYENTE_REGIMEN_MICOREMPRESAS";
                                xDoc.GetElementsByTagName("infoAdicional")[0].AppendChild(campoAdicional);

                                if (xDoc.GetElementsByTagName("regimenMicroempresas").Count == 0)
                                {
                                    XmlNode regimenMicroempresas = xDoc.CreateNode(XmlNodeType.Element, "regimenMicroempresas", null);
                                    xDoc.GetElementsByTagName("infoTributaria")[0].AppendChild(regimenMicroempresas);
                                    xDoc.GetElementsByTagName("regimenMicroempresas")[0].InnerText = "CONTRIBUYENTE RÉGIMEN MICROEMPRESAS";
                                }
                            }


                            if (dtBasesServer.Rows[0][21].ToString() == "True" && contadorNac == 0)
                            {
                                XmlNode campoAdicional = xDoc.CreateNode(XmlNodeType.Element, "campoAdicional", null);
                                XmlAttribute attr = xDoc.CreateAttribute("nombre");
                                attr.Value = "Nac";
                                campoAdicional.Attributes.Append(attr);
                                //campoAdicional.InnerText = "AGENTE DE RETENCION RESOLUCION NAC-DNCRASC20-00000001"; 2025-06-06 cmina
                                //campoAdicional.InnerText = "AGENTE DE RETENCION RESOLUCION NRO.NAC - DGERCGC25 - 00000010";
                                campoAdicional.InnerText = dtBasesServer.Rows[0][26].ToString();

                                xDoc.GetElementsByTagName("infoAdicional")[0].AppendChild(campoAdicional);

                                //if (xDoc.GetElementsByTagName("agenteRetencion").Count == 0)
                                //{
                                //    XmlNode agenteRetencion = xDoc.CreateNode(XmlNodeType.Element, "agenteRetencion", null);
                                //    xDoc.GetElementsByTagName("infoTributaria")[0].AppendChild(agenteRetencion);
                                //    xDoc.GetElementsByTagName("agenteRetencion")[0].InnerText = "";

                                //}
                            }


                            xDoc.Save(dtRow[11].ToString());
                            contadorNac = 0;
                            contadorMicro = 0;

                            // i = individual  l = lote -> firmar.firmar("i",....
                            firmado = firmar.firmar("i", dtRow[11].ToString(), dtBasesServer.Rows[0][8].ToString(), dtBasesServer.Rows[0][9].ToString());

                            if (firmado)
                            {
                                Console.WriteLine("Proceso EnviarComprobanteB...");
                                respuestaEnvio = operaciones.EnviarComprobanteB(dtRow[11].ToString(), dtRow[3].ToString(), dtRow[7].ToString(), pathFile, dtRow[7].ToString() + "_error_" + DateTime.Now.Millisecond); // envio al sri y recibo su respuesta.
                                Console.WriteLine("Respuesta EnviarComprobanteB..." + respuestaEnvio);
                                Console.WriteLine("Tipo de Documento: " + tipoDoc + " id Archivo: " + dtRow[0].ToString() + " Numero de Transacción: " + dtRow[27].ToString());

                          
                                //Console.ReadKey();


                                if (respuestaEnvio.Contains(";Error al usar los web services del SRI (envío): "))
                                {
                                    Console.WriteLine("Error al usar los web services del SRI (envío):");
                                    clsVariablesFirma.objfirma.cambioEstadoContingenteXml(Convert.ToInt32(dtRow[0].ToString()), true);

                                    if (dtRow[27].ToString() == "0")
                                    {
                                        EnviarCorreo("feletronica@acklins.online", email1, email2, emailEmpre, "ERROR: SRI WS PARA ENVIO COMPROBANTE FUERA DE LINEA", empresa, clave, numComp, tipoDoc, cliente, idCliente, "No se obtuvo respuesta el servicio web del SRI (RECEPCION DE ARCHIVO)");
                                    }

                                   
                                }
                                

                                clsVariablesFirma.objfirma.cambioEstadoXml(Convert.ToInt32(dtRow[0].ToString()), "1");
                                clsVariablesFirma.objfirma.cambioEstadoXml(Convert.ToInt32(dtRow[0].ToString()), "e", respuestaEnvio);

                                    //Console.ReadKey();

                                if (respuestaEnvio.Contains("ERROR"))
                                {
                                         Console.WriteLine("respuestaEnvio.Contains = ERROR");
                                        if (dtRow[27].ToString() == "0")
                                        {
                                            EnviarCorreo("feletronica@acklins.online", email1, email2, emailEmpre, "ERROR: COMPROBANTE RECHAZADO", empresa, clave, numComp, tipoDoc, cliente, idCliente, respuestaEnvio);
                                        }

                                       Console.WriteLine("Grabo Error Sri: " + tipoDoc + " Numero de Transacción: " + dtRow[27].ToString() + " Comprobante: " + dtRow[30].ToString());
                                       clsVariablesFirma.objfirma.GrabaErroresSri(empresa, Ruc, numComp, FechaRegistro, respuestaEnvio, id);
                                }

                            }
                            else
                            {   

                                        clsVariablesFirma.objfirma.cambioEstadoXml(Convert.ToInt32(dtRow[0].ToString()), "e", "archivo no firmado");
                                        clsVariablesFirma.objfirma.cambioEstadoXml(Convert.ToInt32(dtRow[0].ToString()), "0");


                                if (dtRow[27].ToString() == "0")
                                {
                                    EnviarCorreo("feletronica@acklins.online", email1, email2, emailEmpre, "Error: No se pudo encontrar el archivo", empresa, clave, numComp, tipoDoc, cliente, idCliente, "No se pudo firmar el archivo");

                                }

                               
                            }
                        }
                        else
                        {


                            Console.WriteLine("Error archivo de firma no existe");
                            clsVariablesFirma.objfirma.cambioEstadoXml(Convert.ToInt32(dtRow[0].ToString()), "e", "archivo no existe");
                            clsVariablesFirma.objfirma.cambioEstadoXml(Convert.ToInt32(dtRow[0].ToString()), "0");

                            if (dtRow[27].ToString() == "0")
                            {
                                EnviarCorreo("feletronica@acklins.online", email1, email2, emailEmpre, "Error: No se pudo encontrar el archivo", empresa, clave, numComp, tipoDoc, cliente, idCliente, "Posible Error de red");

                            }

                            
                        }
                    }
                    catch (Exception ex)
                    {


                        Console.WriteLine("se presento un error en el proceseso de firma:" + ex.Message);
                        clsVariablesFirma.objfirma.cambioEstadoXml(Convert.ToInt32(dtRow[0].ToString()), "e", "se presento un error en el proceseso de firma: " + ex.Message);
                        clsVariablesFirma.objfirma.cambioEstadoXml(Convert.ToInt32(dtRow[0].ToString()), "0");

                        if (dtRow[27].ToString() == "0")
                        {
                            EnviarCorreo("feletronica@acklins.online", email1, email2, emailEmpre, "Error: al leer el archivo sin firmar no se encontraron datos", empresa, clave, numComp, tipoDoc, cliente, idCliente, ex.Message); 
                        }
                       
                    }
                }

            }

            catch (TypeInitializationException ex)
            {
                Console.WriteLine("Error al inicializar clsVariablesFirma");
                Console.WriteLine("Mensaje: " + ex.Message);
                Console.WriteLine("Causa interna: " + ex.InnerException?.Message);
                Console.WriteLine("Stack interno: " + ex.InnerException?.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrió un error: " + ex.Message);
              
            }

       
        }
        public static void EnviarCorreo(string fromEmail, string toEmail, string toCc, string emailEmpre, string asunto, string empresa, string clave, string numComp, string tipoDoc, string cliente, string idCliente, string errorSistema)
        {


            MailMessage objMail;
            string nombreEmpresa = string.Empty; 

            objMail = new MailMessage();
            objMail.From = new MailAddress(fromEmail); //Remitente
            objMail.To.Add(emailEmpre); //Email a enviar *** 
            objMail.CC.Add(toEmail);
            objMail.CC.Add(toCc);
            objMail.CC.Add("christian.mina@acklins.net");
            objMail.IsBodyHtml = true;
            objMail.Subject = asunto;
            //objMail.Body = recursos.recursos.strPlantillaHtml.Replace("#PROVEEDOR#",nombreEmpresa); 
            objMail.Body = recursos.recursos.strPlantillaHtml.Replace("#empre", empresa).Replace("#clave", clave).Replace("#numComp", numComp).Replace("#tc", tipoDoc).Replace("#cli", cliente).Replace("#id", idCliente).Replace("#error", errorSistema);

            //objMail.Body = Session["plantillaMail"].ToString();
            //objMail.BodyEncoding = Encoding.UTF8;
            //objMail.IsBodyHtml = false;

            SmtpClient SmtpMail = new SmtpClient("mail.smtp2go.com");
            SmtpMail.Port = 587;
            //SmtpMail.Port = 465;
            //SmtpMail.EnableSsl = true;
            SmtpMail.UseDefaultCredentials = false;
            System.Net.NetworkCredential cred = new System.Net.NetworkCredential("feletronica@acklins.online", "olb3DWBkKaNnqVpd");
            SmtpMail.Credentials = cred;
            //SmtpMail.TargetName = "STARTTLS/smtp.office365.com";
            SmtpMail.Send(objMail);
            objMail.Dispose();
            SmtpMail.Dispose();

        }
    }
    class TipoDocumento
    {
        public string TipoDoc { get; set; }
        public string Titulo { get; set; }
    }
}
