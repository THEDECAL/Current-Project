using EasyBilling.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyBilling.Controllers
{
    [Authorize]
    [CheckAccessRights]
    [NoShowToMenu]
    public class ClientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
