using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Helpers
{
    public static class CustomFieldsHelper
    {
        const string CSTM_FIELDS_CFG_FILE = "Settings\\customFields.json";
        static public async Task<string> GetCustomFieldNameAsync([NotNull] string defaultFieldName)
        {
            string customFieldName = "";

            await Task.Run(() =>
            {
                try
                {
                    var config = new ConfigurationBuilder().AddJsonFile(CSTM_FIELDS_CFG_FILE).Build();
                    customFieldName = config[defaultFieldName];
                }
                catch (Exception ex)
                { Console.WriteLine(ex.StackTrace); }
            });

            return customFieldName;
        }
    }
}
