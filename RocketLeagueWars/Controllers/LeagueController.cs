using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RocketLeagueWars.Logic;
using RocketLeagueWars.Models;

namespace RocketLeagueWars.Controllers
{
    public class LeagueController : Controller
    {
        //
        // GET: /League/

        public ActionResult Index()
        {
            IEnumerable<League> leagues = LeagueLogic.GetLeagues();
            return View(leagues);
        }

        public ActionResult League(int id)
        {
            League league = LeagueLogic.GetLeague(id);
            return View(league);
        }

        public ActionResult Teams(int id)
        {
            IEnumerable<Team> teams = TeamLogic.GetTeams(id);
            return View(teams);
        }
    }
}
