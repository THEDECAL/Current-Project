using OnlinePoker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlinePoker.Controllers
{
    public class GameController : Controller
    {
        [Authorize]
        public ActionResult NewGame(int playerCount = 2)
        {
            var game = new Game(playerCount);

            return View(game);
        }
    }
}