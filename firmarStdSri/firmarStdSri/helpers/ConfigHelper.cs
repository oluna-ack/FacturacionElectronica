using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


namespace firmarStdSri.helpers
{
    public class ConfigHelper
    {
        public static string GetAppSetting(string key)
        {
            return ConfigurationSettings.AppSettings.Get(@key);
        }
    }
}
