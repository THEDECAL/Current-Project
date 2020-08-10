using System;
using System.ComponentModel;
using System.Linq;
using EasyBilling.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyBilling.Controllers
{
    [Authorize]
    [CheckAccessRights]
    [DisplayName("Пользователи")]
    public class UsersController : CustomController
    {
    }
}
