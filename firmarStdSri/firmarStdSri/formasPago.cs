using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace firmarStdSri
{
   public class formasPago
    {
        private Hashtable fp = new Hashtable();
       
        public formasPago ()
        {
            fp.Add("01", "SIN UTILIZACION DEL SISTEMA FINANCIERO");
            fp.Add("02", "CHEQUE PROPIO");
            fp.Add("03", "CHEQUE CERTIFICADO");
            fp.Add("04", "CHEQUE DE GERENCIA");
            fp.Add("05", "CHEQUE DEL EXTERIOR");
            fp.Add("06", "DEBITO DE CUENTA");
            fp.Add("07", "TRANSFERENCIA PROPIO BANCO");
            fp.Add("08", "TRANSFERENCIA OTRO BANCO NACIONAL");
            fp.Add("09", "TRANSFERENCIA BANCO EXTERIOR");
            fp.Add("10", "TARJETA DE CREDITO NACIONAL");
            fp.Add("11", "TARJETA DE CREDITO INTERNACIONAL");
            fp.Add("12", "GIRO");
            fp.Add("13", "DEPOSITO EN CUENTA CORRIENTE-AHORROS");
            fp.Add("14", "ENDOSO DE INVERSION");
            fp.Add("15", "COMPENSACION DE DEUDAS");
            fp.Add("16", "TARJETA DE DEBITO");
            fp.Add("17", "DINERO ELECTRONICO");
            fp.Add("18", "TARJETA PREPAGO");
            fp.Add("19", "TARJETA DE CREDITO");
            fp.Add("20", "OTROS CON UTILIZACION DEL SISTEMA FINANCIERO");
            fp.Add("21", "ENDOSO DE TITULOS");

        }
        public string textoFp(string codigo)
        {
            string textFp = string.Empty;

            foreach (DictionaryEntry de in fp)
            {
                if (de.Key.ToString() == codigo)
                 textFp = de.Value.ToString();
            }

            return textFp;
        }
       
    }
}
