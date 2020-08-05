using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Helpers
{
    public class PageHelper
    {

    }
    [Flags]
    public enum PageName
    {
        AccessRights = 1,
        APIKey,
        Client,
        Device,
        Event,
        FinancialOperations,
        Home,
        Tariff,
        Users,
        Cassa
    }
}
