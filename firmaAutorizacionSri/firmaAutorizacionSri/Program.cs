using firmarStdSri;
using System.Data;
using firmaAutorizacionSri.App_Code.BLL;
using System;
using System.IO;
using System.Net.Mail;
using System.Configuration;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace firmaAutorizacionSri
{
   public class Program
    {


        static void Main(string[] args)// P = produccion T=pruebas
        {
            string pathFile = string.Empty;
            string pathFileB = string.Empty;
            string plataforma = string.Empty;
            string empresa = string.Empty;

            string nomEmpresa = string.Empty, clave = string.Empty, numComp = string.Empty, tipoDoc = string.Empty,  titulo = string.Empty, cliente = string.Empty, idCliente = string.Empty ;
            string email1 = string.Empty; //"dario.portilla@acklins.net";
            string email2 = string.Empty; // "factelectronica@acklins.net";
            string emailEmpre = string.Empty;
            int ambienteAck = 1;
            
            int id;
            string Ruc =  string.Empty;
            string FechaRegistro = string.Empty;
            string FechaDevuelta = string.Empty; ;

            Console.Title = "Proceso de autorización de comprobantes - Acklins";
            Console.WriteLine("Inicio del proceso de autorización...");
            //cmina 2025-06-17  
            try
            {
                // Leer correos desde configuración
                string correos = ConfigurationManager.AppSettings["EmailAcklins"];
                string[] listaCorreos = correos?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

                if (listaCorreos.Length > 0)
                    email1 = listaCorreos[0].Trim();

                if (listaCorreos.Length > 1)
                    email2 = listaCorreos[1].Trim();

                // Validar argumentos
                if (args.Length < 2)
                    throw new ArgumentException("Faltan argumentos: se esperaban [plataforma] y [empresa].");

                plataforma = args[0];
                empresa = args[1];

                // Configurar ambiente y rutas
                if (plataforma == "P")
                {
                    ambienteAck = 2;
                    pathFile = @"\\10.0.0.4\documentos\xmlSinFirma\autorizado\";
                    pathFileB = @"\\10.0.0.4\documentos\";
                }
                else if (plataforma == "T")
                {
                    ambienteAck = 1;
                    pathFile = @"\\10.0.0.4\documentosTest\xmlSinFirma\autorizado\";
                    pathFileB = @"\\10.0.0.4\documentosTest\";
                }
                else
                {
                    throw new ArgumentException("Parámetro 'plataforma' no válido. Debe ser 'P' o 'T'.");
                }

                // Consultar XMLs por autorizar
                DataTable dt = clsVariablesAutorizcion.objAutorizar.listaXmlPorAutorizar(ambienteAck);

                // Aquí puedes continuar el procesamiento con el DataTable `dt`

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

            try
            {

                DataTable dtBasesServer = new DataTable();
                dtBasesServer = clsVariablesAutorizcion.objAutorizar.listaBaseDatosServer(dtRow[2].ToString());

                cliente = dtRow[28].ToString();
                idCliente = dtRow[10].ToString();
                nomEmpresa = dtRow[2].ToString();
                clave = dtRow[7].ToString();
                numComp = dtRow[30].ToString();


               id = Convert.ToInt32(dtRow[0]);
               Ruc = dtBasesServer.Rows[0][11].ToString();          
               FechaRegistro = dtRow[12].ToString();
               FechaDevuelta = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
               

               string codigo = dtRow[4].ToString();
                 TipoDocumento doc;
                    if (tiposDocumento.TryGetValue(codigo, out doc))
                    {
                        tipoDoc = doc.TipoDoc;
                        titulo = doc.Titulo;
                    }

                     //emailEmpre = clsVariablesAutorizcion.objAutorizar.devuelveEmail(nomEmpresa).Rows[0][0].ToString();

                        DataTable dtEmail = clsVariablesAutorizcion.objAutorizar.devuelveEmail(nomEmpresa);
                        if (dtEmail.Rows.Count > 0 && dtEmail.Rows[0][0] != DBNull.Value)
                        {
                            emailEmpre = dtEmail.Rows[0][0].ToString();
                        }
                        else
                        {
                            emailEmpre = string.Empty; // o un valor predeterminado
                            EnviarCorreo("feletronica@acklins.online", email1, email2, "factelectronica@acklins.net", "Tareas Autoriza comprobantes", empresa, clave, numComp, tipoDoc, cliente, idCliente, "No se encontró direccion de correo");

                        }


                    string respuestaAutorizacion = string.Empty;
                    string cargaWs = string.Empty;
                    FileInfo Fi = new FileInfo(dtRow[11].ToString());
            

                    operacionesComprobantes operaciones = new operacionesComprobantes();
                    Console.WriteLine("Proceso AutorizarComprobante...");
                    respuestaAutorizacion = operaciones.AutorizarComprobante(dtRow[7].ToString(), dtRow[11].ToString(), pathFile, dtRow[7].ToString()+"error", dtRow[3].ToString());
                    Console.WriteLine("Respuesta AutorizarComprobante..." + respuestaAutorizacion);
                    Console.WriteLine("Tipo de Documento: " + tipoDoc + " Numero de Transacción: " + dtRow[27].ToString() + " Comprobante: " + dtRow[30].ToString());

                   clsVariablesAutorizcion.objAutorizar.cambioEstadoXml(Convert.ToInt32(dtRow[0].ToString()), "a", respuestaAutorizacion);

                    if (respuestaAutorizacion.Contains("ESTADO: AUTORIZADO"))
                    {
                        if(dtRow[25].ToString()=="True")
                        cargaWs = operaciones.subirSitioDeCargasyDescargasLab(Convert.ToInt32(dtRow[4].ToString()), dtRow[2].ToString(), dtRow[6].ToString(), dtRow[10].ToString(), dtRow[9].ToString(), dtRow[9].ToString(), titulo, titulo + ": "+ dtRow[28].ToString(), dtRow[7].ToString(),
                                                                                Fi.Length.ToString(), DateTime.Now.Year + "-" + DateTime.Now.Month, 3, "", "", pathFile + dtRow[7].ToString() + ".xml", true, dtRow[2].ToString(), dtRow[3].ToString(), empresa, pathFileB);   //"A" Acklins
                        else
                        cargaWs = operaciones.subirSitioDeCargasyDescargas(Convert.ToInt32(dtRow[4].ToString()), dtRow[2].ToString(), dtRow[6].ToString(), dtRow[10].ToString(), dtRow[9].ToString(), dtRow[9].ToString(), titulo, titulo + " CS", dtRow[7].ToString(),
                                                                            Fi.Length.ToString(), DateTime.Now.Year + "-" + DateTime.Now.Month, 3, "", "", pathFile + dtRow[7].ToString() + ".xml", true, dtRow[2].ToString(), dtRow[3].ToString(), empresa, pathFileB);   //"A" Acklins
					}
                    else
                    {
                        if (dtRow[25].ToString() == "True")
                        cargaWs = operaciones.subirSitioDeCargasyDescargasLab(Convert.ToInt32(dtRow[4].ToString()), dtRow[2].ToString(), dtRow[6].ToString(),dtRow[10].ToString(), dtRow[9].ToString(), dtRow[9].ToString(), titulo, titulo + ": " + dtRow[28].ToString(), dtRow[7].ToString(),
                                                                                                    Fi.Length.ToString(), DateTime.Now.Year + "-" + DateTime.Now.Month, 3, "", "", pathFile + dtRow[7].ToString() + ".xml", false, dtRow[2].ToString(), dtRow[3].ToString(), empresa, pathFileB);
                        else
                        cargaWs = operaciones.subirSitioDeCargasyDescargas(Convert.ToInt32(dtRow[4].ToString()), dtRow[2].ToString(), dtRow[6].ToString(), dtRow[10].ToString(), dtRow[9].ToString(), dtRow[9].ToString(), titulo, titulo + " CS", dtRow[7].ToString(),
                                                                                Fi.Length.ToString(), DateTime.Now.Year + "-" + DateTime.Now.Month, 3, "", "", pathFile + dtRow[7].ToString() + ".xml", false, dtRow[2].ToString(), dtRow[3].ToString(), empresa, pathFileB);
                        if (dtRow[27].ToString() == "0")
                            EnviarCorreo("noreply@acklins.net", email1, email2, emailEmpre, "ERROR: COMPROBANTE NO AUTORIZADO", nomEmpresa, clave, numComp, tipoDoc, cliente, idCliente, respuestaAutorizacion);


                        Console.WriteLine("Grabo Error Sri: " + tipoDoc + " Numero de Transacción: " + dtRow[27].ToString() + " Comprobante: " + dtRow[30].ToString());
                        clsVariablesAutorizcion.objAutorizar.GrabaErroresSri(nomEmpresa, Ruc, numComp, FechaRegistro, respuestaAutorizacion,id);


                        }
                }
                catch (Exception ex)
                {
                    
                    clsVariablesAutorizcion.objAutorizar.cambioEstadoXml(Convert.ToInt32(dtRow[0].ToString()), "a", "Error Autorización SP: " + ex.Message);
                    if (dtRow[27].ToString() == "0")
                        EnviarCorreo("feletronica@acklins.online", email1, email2, emailEmpre, "ERROR: FALLO EN LA AUTORIZACION", nomEmpresa, clave, numComp, tipoDoc, cliente, idCliente, ex.Message);
                }
            }



            }
            catch (Exception ex)
            {
                Console.WriteLine("Se produjo un error: " + ex.Message);
                // Opcional: log en archivo, base de datos o enviar email con detalles del error.
            }



        }
        public static void EnviarCorreo(string fromEmail, string toEmail, string toCc, string emailEmpre, string asunto, string empresa, string clave, string numComp, string tipoDoc, string cliente, string idCliente, string errorSistema)
        {


            MailMessage objMail;
            string nombreEmpresa = string.Empty; ;

            objMail = new MailMessage();
            objMail.From = new MailAddress(fromEmail); //Remitente
            objMail.To.Add(emailEmpre); //Email a enviar *** mail de prueba Augusto Merchan
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

