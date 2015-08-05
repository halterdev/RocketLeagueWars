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

            SubmitGameModel model = new SubmitGameModel();
            model.SetWinningTeamDDL(Convert.ToInt32(Session["TeamID"]));

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(SubmitGameModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // submit game .. then go to standings page  ?
                GameLogic.Submit(model);
                TeamLogic.UpdateTeamSeasonStatRowsForGame(model);

                SubmitGameModel newModel = new SubmitGameModel();
                newModel.SetWinningTeamDDL(Convert.ToInt32(Session["TeamID"]));
                return View(newModel);
            }

            return View(model);
        }
    }
}
