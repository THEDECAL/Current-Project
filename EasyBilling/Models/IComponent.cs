using System;

namespace EasyBilling.Models
{
    public interface IComponent
    {
        struct ComponentActionRigths
        {
            public bool IsRead { get; set; }
            public bool IsCreate { get; set; }
            public bool IsUpdate { get; set; }
            public bool IsDelete { get; set; }
        }
        ComponentActionRigths? ActionRigths { get; }
        string Name { get; }
    }
/*    [Flags]
    public enum ComponentName
    {
        ClientManage = 1,
        ClientOperationHistory,
        ClientTariffManage,

        UserSearch,
        UserManager,
        UserList,

        DeviceManager,
        DeviceSearch,
        DeviceList,

        EventSearch,
        EventManager,
        EventList,

        TariffManager,
        TariffList,

        APIKeyManager,
        APIKeyList,

        AccessAndPermissionsManager,
        AccessAndPermissionsList,

        FinancialOperationsSearch,
        FinancialOperationsManager,
        FinancialOperationsList
    };

    [Flags]
    public enum ComponentAction
    {
        Read,
        Create,
        Update,
        Delete
    }*/
}
