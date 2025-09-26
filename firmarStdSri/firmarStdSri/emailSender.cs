using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;



namespace firmarStdSri
{

    public class emailSender
    {
        public static bool Send(string subject, string message, string destination, List<string> ccAccount, string emailHost, int emailPort, bool emailSsl, string emailUserName, string emailPassword, List<string> attachments = null, string logoEmail = null)
        {

            var smtp = new SmtpClient();
            smtp.Host = emailHost;
            smtp.Port = emailPort;
            smtp.EnableSsl = emailSsl;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(emailUserName, emailPassword);


            using (var msg = new MailMessage(emailUserName, destination))
            {
                if (ccAccount != null && ccAccount.Count > 0)
                {
                    foreach (string ccAcc in ccAccount)
                    {
                        msg.Bcc.Add(ccAcc);
                    }
                }
                //msg.CC.Add(ccAccount);
                msg.Subject = subject;
                msg.Body = message;
                msg.IsBodyHtml = true;

                if (logoEmail != null)
                {
                    AlternateView av1 = AlternateView.CreateAlternateViewFromString(msg.Body, null, System.Net.Mime.MediaTypeNames.Text.Html);
                    string strImageUrl = System.Web.HttpContext.Current.Server.MapPath(logoEmail);
                    LinkedResource logo = new LinkedResource(strImageUrl, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                    logo.ContentId = "companylogo";
                    //To refer to this image in the html body, use <img src="cid:companylogo"/> 
                    av1.LinkedResources.Add(logo);
                    msg.AlternateViews.Add(av1);
                }


                if (attachments != null)
                {
                    foreach (string attachment in attachments)
                    {
                        //comprobamos si existe el archivo y lo agregamos a los adjuntos
                        if (System.IO.File.Exists(@attachment))
                            msg.Attachments.Add(new Attachment(@attachment));

                    }
                }

                try
                {
                    smtp.Send(msg);
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
