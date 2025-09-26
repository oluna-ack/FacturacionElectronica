using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using firmarStdSri;
using emailEnvioSri.App_Code.BLL;
using System.Text.RegularExpressions;

namespace emailEnvioSri
{
    public class Program
    {
        static void Main(string[] args)// P = produccion T=pruebas
        {
            string pathFile = string.Empty;
            string pathFileB = string.Empty;
            string plataforma = string.Empty; //// P = produccion T=pruebas
            string destination = string.Empty;//// correo de destino
			string paRutaLogo = string.Empty;
			int ambienteAck=1;
            plataforma = args[0];
            // plataforma = "P";

            Console.Title = "Proceso de Envio Email SRI -  Acklins";
            Console.WriteLine("Inicio del proceso  de Envio Email SRI...");

            if (plataforma == "P")
				ambienteAck = 2;
			
			DataTable dt = new DataTable();
            dt = clsVariablesEnvio.objEnviar.listaXmlPorEnviar(ambienteAck);

            string[] arrAccounts = null;
        

        
            if (plataforma == "P") {
                pathFile = @"\\10.0.0.4\documentos\xmlSinFirma\autorizado\";
                pathFileB = @"\\10.0.0.4\documentos\xmlSinFirma\";

                //pathFile = @"C:\documentos\xmlSinFirma\autorizado\";
                //pathFileB = @"C:\documentos\xmlSinFirma\";

            }
			if (plataforma == "T")
            {
                pathFile = @"\\10.0.0.4\documentosTest\xmlSinFirma\autorizado\";
                pathFileB = @"\\10.0.0.4\documentosTest\xmlSinFirma\";
            }
			//pathFile = @"\\192.168.0.200\documentosTest\xmlSinFirma\autorizado\";



			int i = 0;
            int e = 0;
            foreach (DataRow dtRow in dt.Rows)
                {

    
                Console.WriteLine("destination..." + dtRow[9].ToString());

                //if (dtRow[10].ToString() != "9999999999999") 
                //{ 
                try
                    {
                        string formatoClienteEmail = string.Empty;
                        List<string> ccAccounts = new List<string>();
                        List<string> strAttachments = new List<string>();
                        //string[] arrAccounts = null;
                        bool enviado=false;
                        string tipoDocumento = string.Empty;
                        string nombreLogo = string.Empty;

                        DataTable dtL = new DataTable();
                        dtL = clsVariablesEnvio.objEnviar.listaBaseDatosServer(dtRow[2].ToString()); //dtL sub 1
                        pdfGenerator generarPdf = new pdfGenerator();
                        
                        if (dt.Rows[i][4].ToString() == "1") tipoDocumento = "FACTURA";
                        if (dt.Rows[i][4].ToString() == "3") tipoDocumento = "NOTA DE CREDITO";
                        if (dt.Rows[i][4].ToString() == "9") tipoDocumento = "RETENCION";
                        if (dt.Rows[i][4].ToString() == "11") tipoDocumento = "GUIA DE REMISIÓN";
                        if (dt.Rows[i][4].ToString() == "12") tipoDocumento = "NOTA DE DEBITO";
                        if (dt.Rows[i][4].ToString() == "13") tipoDocumento = "LIQUIDACION DE COMPRA";

                        string empresaEmail = dtL.Rows[0][1].ToString();

                        nombreLogo = dtL.Rows[0][5].ToString();
						string generecionCorrecta = string.Empty;

                        paRutaLogo = @"\\10.0.0.4\images\logos\";

                        //paRutaLogo =@"C:\images\logos\";

                        generecionCorrecta = generarPdf.generarPdf(paRutaLogo + nombreLogo, Convert.ToInt32(dtL.Rows[0][6].ToString()), Convert.ToInt32(dtL.Rows[0][7].ToString()), pathFile + dtRow[7].ToString() + ".xml", pathFile + dtRow[7].ToString() + ".pdf", dtRow[4].ToString());
						 Console.WriteLine("generecionCorrecta..." + generecionCorrecta);



           
                    if (generecionCorrecta == "1")
						{

                        //Console.WriteLine("enviado 1...");

                        if (dt.Rows[i][8].ToString().Split(';').Count() > 0)
							{


                  
                            arrAccounts = dt.Rows[i][8].ToString().Split(';');


								foreach (string cuenta in arrAccounts)
								{
									//if (Regex.IsMatch(cuenta,@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1250)))
									if (Regex.IsMatch(cuenta, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1250)))
										ccAccounts.Add(cuenta);

                            }
							}

                        //Console.WriteLine("enviado 2...");
                        string[] datosXml = operacionesComprobantes.dtDataWs(pathFileB + dtRow[7].ToString() + ".xml");
							formatoClienteEmail = recursos.recursos.textoEmail;
                        //Console.WriteLine("enviado 3...");

                        string textoRetencion = "";
							//if (tipoDocumento == "FACTURA") 
							//    textoRetencion = "<br /><font color = \"red\" style = \"text-align:justify\" ><b> Retenciones:</ b> Artículo 50 de la Ley del régimen Tributario Interno “Obligaciones de los agentes de retención.-La retención en la fuente deberá realizarse al momento del pago o crédito en cuenta, lo que suceda primero.Los agentes de retención están obligados a entregar el respectivo comprobante de retención, dentro del término no mayor de cinco días de recibido el comprobante de venta “</font ><br />";

							formatoClienteEmail = formatoClienteEmail.Replace("#TipoDocumento", tipoDocumento).Replace("#Serie", dt.Rows[i][5].ToString()).Replace("#Numero", dt.Rows[i][6].ToString()).Replace("#ClaveAcceso", dt.Rows[i][7].ToString()).Replace("#ClienteAcklins", empresaEmail).Replace("#nombreComprador", datosXml[6]).Replace("#archivoXmlAdjunto", dtRow[7].ToString() + ".xml").Replace("#archivoPdfAdjunto", dtRow[7].ToString() + ".pdf").Replace("#textoRetencion", textoRetencion);
                            strAttachments.Add(pathFile + dtRow[7].ToString() + ".xml");
                            strAttachments.Add(pathFile + dtRow[7].ToString() + ".pdf");
                            destination = dt.Rows[i][9].ToString();


							//if (Regex.IsMatch(destination, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1250)))
							if (Regex.IsMatch(destination, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1250)))
							{

                                //Console.WriteLine("enviado 4...");
                                //Console.WriteLine("enviado ..." + e);
                                //Console.WriteLine("Documentos electrónicos: " + dtL.Rows[0][1].ToString());
                                //Console.WriteLine("formatoClienteEmail ..." + formatoClienteEmail);
                                //Console.WriteLine("destination ..." + destination);
                                //Console.WriteLine("ccAccounts ..." + string.Join(", ", ccAccounts));
                            try
                            {
                                    if (e == 0)
                   
                                     enviado = emailSender.Send("Documentos electrónicos: " + dtL.Rows[0][1].ToString(), formatoClienteEmail, destination, ccAccounts, "mail.smtp2go.com", Convert.ToInt32(587), false, "noreply@acklins.online", "Laredo199612$", strAttachments);
                                else
                                        enviado = emailSender.Send("Documentos electrónicos: " + dtL.Rows[0][1].ToString(), formatoClienteEmail, destination, ccAccounts, "mail.smtp2go.com", Convert.ToInt32(587), false, "noreply@acklins.online", "Laredo199612$", strAttachments);


                            }
                            catch (Exception ex)
                            {
                                // Aquí puedes loguear el error o mostrar un mensaje
                                Console.WriteLine("Error al enviar correo: " + ex.Message);
                                enviado = false;  // o lo que uses para marcar fallo
                            }

                            //Console.WriteLine("enviado 5...");
                            //Console.WriteLine("enviado..." + enviado);
                            //Console.ReadKey();



                        }
                        if (enviado)
							{
								if (strAttachments.Count == 2)
									clsVariablesEnvio.objEnviar.estadoEmailActualizar(Convert.ToInt32(dtRow[0].ToString()), enviado, null, e);
								else
									clsVariablesEnvio.objEnviar.estadoEmailActualizar(Convert.ToInt32(dtRow[0].ToString()), enviado, "Solo se adjunto 1 archivo", e);
								e++;
							}
							else
								clsVariablesEnvio.objEnviar.estadoEmailActualizar(Convert.ToInt32(dtRow[0].ToString()), enviado, "Correo no enviado, POSIBLE FORMATO INCORRECTO", e);

							if (e > 1) // contador para usar dos cuentas de relay
								e = 0;

							try
							{
								clsVariablesEnvio.objEnviar.estadoEmailActualizarSitioWeb(dtRow[7].ToString(), "S");
							}
							catch (Exception ex)
							{
								clsVariablesEnvio.objEnviar.estadoEmailActualizarSitioWeb(dtRow[7].ToString(), "E");
							}
							i++;
						}
						else {
							if(generecionCorrecta=="0")
							clsVariablesEnvio.objEnviar.estadoEmailActualizar(Convert.ToInt32(dtRow[0].ToString()), false, "no se generó el pdf", 3);
							else
						    clsVariablesEnvio.objEnviar.estadoEmailActualizar(Convert.ToInt32(dtRow[0].ToString()), false, generecionCorrecta, 3);
							i++;
						}
					
                }
                catch (Exception ex)
                    {
                        clsVariablesEnvio.objEnviar.estadoEmailActualizar(Convert.ToInt32(dtRow[0].ToString()), false, ex.Message,3);
                    i++;
                    }
				//}
			}
         }
      }
   }

