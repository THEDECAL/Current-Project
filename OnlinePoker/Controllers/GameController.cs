using OnlinePoker.Models;
using OnlinePoker.Hubs;
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
            //Разкомментировать по оканчании разработки
            //try
            //{
                var game = new Game(playerCount);
                GameHub.Games.Add(game);

                return View(game.Id as object);
            //}
            //catch (Exception)
            //{
            //    return View("Error");
            //}
        }
        public ActionResult CurrentGames()
        {            
            //Разкомментировать по оканчании разработки
            //try
            //{
                return View(GameHub.Games);
            //}
            //catch (Exception)
            //{
            //    return View("Error");
            //}
        }
    }
}