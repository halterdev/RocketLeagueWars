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
            model.SetWinningPlayersList(Convert.ToInt32(Session["TeamID"]));
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

                string winningPlayers = Request.Form["winningplayer"];
                string losingPlayers = Request.Form["losingplayer"];
                string[] winners = winningPlayers.Split(',');
                string[] losers = losingPlayers.Split(',');

                for (int i = 0; i < winners.Length; i++)
                {
                    PlayerSeasonStat statRow = new PlayerSeasonStat();
                    statRow.UserID = Convert.ToInt32(winners[i]);
                    statRow.Season = LeagueLogic.GetSeason(TeamLogic.GetLeagueID(Convert.ToInt32(model.WinningTeamID)));
                    AccountLogic.UpdatePlayerSeasonStats(statRow, model);
                }

                for (int i = 0; i < losers.Length; i++)
                {
                    PlayerSeasonStat statRow = new PlayerSeasonStat();
                    statRow.UserID = Convert.ToInt32(losers[i]);
                    statRow.Season = LeagueLogic.GetSeason(TeamLogic.GetLeagueID(Convert.ToInt32(model.WinningTeamID)));
                    AccountLogic.UpdatePlayerSeasonStats(statRow, model);
                }

                SubmitGameModel newModel = new SubmitGameModel();
                newModel.SetWinningTeamDDL(Convert.ToInt32(Session["TeamID"]));
                newModel.SetWinningPlayersList(Convert.ToInt32(Session["TeamID"]));
                return View(newModel);
            }

            return View(model);
        }
    }
}
