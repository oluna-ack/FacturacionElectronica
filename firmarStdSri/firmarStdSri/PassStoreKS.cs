using java.security.cert;
using es.mityc.javasign.pkstore;


namespace firmarStdSri
{
   public class PassStoreKS : IPassStoreKS
    {
        private string pwd;

        public PassStoreKS(string pwd)
        {
            this.pwd = pwd;
        }

        public char[] getPassword(X509Certificate certificate, string alias)
        {
            return pwd.ToCharArray();
        }
    }

    


}
