using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RocketLeagueWars.Logic;
using RocketLeagueWars.Models;

namespace RocketLeagueWars.Controllers
{
    public class GameController : Controller
    {
        //
        // GET: /Game/

        [AllowAnonymous]
        public ActionResult Submit(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new SubmitGameModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(SubmitGameModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                GameLogic.Submit(model);
                return View(new SubmitGameModel());
            }

            return View(model);
        }
    }
}
