using OnlinePoker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlinePoker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var game = new Game() { Players = { new Player(), new Player() } };
            game.CardDistribution();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}