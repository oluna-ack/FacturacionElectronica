using org.w3c.dom;
using javax.xml.parsers;
using java.io;
using java.security.cert;
using java.security;
using es.mityc.javasign.pkstore;
using es.mityc.javasign.pkstore.keystore;
using System.IO;
using es.mityc.firmaJava.libreria.xades;
using es.mityc.firmaJava.libreria.utilidades;
using es.mityc.javasign.xml.refs;
using System.Xml;
using System.Runtime.InteropServices;


namespace firmarStdSri
{


    public class firmaSri
    {
        private Document cargaXml(string path)
        {
            Document document = null;

            try
            {
                DocumentBuilderFactory documentBuilderFactory = DocumentBuilderFactory.newInstance();
                documentBuilderFactory.setNamespaceAware(true);
                document = documentBuilderFactory.newDocumentBuilder().parse(new BufferedInputStream(new FileInputStream(path)));
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("{0} LoadXML(): ", ex.Message);
            }
            return document;
        }

        private X509Certificate cargarCert(string path, string password, out PrivateKey privateKey, out Provider provider)
        {
            X509Certificate cert = null;
            provider = null;
            privateKey = null;
            try
            {
                KeyStore instance = KeyStore.getInstance("PKCS12");
                instance.load(new BufferedInputStream(new FileInputStream(path)), password.ToCharArray());
                IPKStoreManager pkStoreManager = new KSStore(instance, new PassStoreKS(password));
                if (pkStoreManager.getSignCertificates().size() == 1)
                    cert = (X509Certificate)pkStoreManager.getSignCertificates().get(0);
                if (pkStoreManager.getSignCertificates().size() == 2)
                    cert = (X509Certificate)pkStoreManager.getSignCertificates().get(1);
                provider = pkStoreManager.getProvider(cert);
                privateKey = pkStoreManager.getPrivateKey(cert);

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("{0} Carga Certificado ", ex.Message);
            }
            return cert;
        }
        public bool firmar(string tipo, string pathXml, string pathP12, string pass) //tipo i=individual l=lote
        {

            FileOutputStream fileOutputStream = null;
            Provider provider;
            PrivateKey privateKey;
            X509Certificate certificado = cargarCert(Path.GetFullPath(pathP12), pass, out privateKey, out provider);
            bool result = false;
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(pathXml);

            if (xDoc.GetElementsByTagName("ds:Signature").Count > 0)
            {
                XmlNodeList nodo = xDoc.GetElementsByTagName("ds:Signature");
                xDoc.DocumentElement.RemoveChild(nodo[0]);
                xDoc.Save(pathXml);
            }
            xDoc.Load(pathXml);

            if (certificado != null && xDoc.GetElementsByTagName("ds:Signature").Count == 0)
            {
                try
                {
                    DataToSign dataToSign = new DataToSign();
                    dataToSign.setXMLEncoding("UTF-8");
                    dataToSign.setEnveloped(true);
                    dataToSign.setXadesFormat(EnumFormatoFirma.XAdES_BES);
                    dataToSign.setEsquema(XAdESSchemas.XAdES_132);

                    dataToSign.setDocument(cargaXml(pathXml));
                    if (tipo == "i")
                        dataToSign.addObject(new ObjectToSign(new InternObjectToSign("comprobante"), "comprobante", null, "text/xml", null));
                    if (tipo == "l")
                        dataToSign.addObject(new ObjectToSign(new InternObjectToSign("lote"), "lote", null, "text/xml", null));

                    object[] objArray = new FirmaXML().signFile(certificado, dataToSign, privateKey, provider);
                    fileOutputStream = new FileOutputStream(pathXml);
                    UtilidadTratarNodo.saveDocumentToOutputStream((Document)objArray[0], fileOutputStream, true);

                    result = true;
                }
                catch (System.Exception ex)
                {
                    if (tipo == "i")
                        System.Console.WriteLine("{0} Firma por archivo ", ex.Message);
                    if (tipo == "l")
                        System.Console.WriteLine("{0} Firma lote ", ex.Message);

                    return false;
                }
                finally
                {
                    if (fileOutputStream != null) fileOutputStream.close();
                }
            }
            return result;
        }


    }
}
