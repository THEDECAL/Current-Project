using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Providers
{
    public class SettingsProvider : ConfigurationProvider
    {
        const string DIR_NAME = "Settings";
        public SettingsProvider()
        {

        }
    }
}
