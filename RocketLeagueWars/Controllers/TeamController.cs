using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RocketLeagueWars.Logic;
using RocketLeagueWars.Models;

namespace RocketLeagueWars.Controllers
{
    public class TeamController : Controller
    {
        //
        // GET: /Team/

        public ActionResult Index(int id)
        {
            //if (Session["Username"] != null)
            //{
            //    Response.Redirect("~/Home/Index");
            //}

            Team team = TeamLogic.GetTeam(id);
            return View(team);
        }

        public ActionResult MyTeam(int id)
        {
            if (Session["Username"] != null)
            {
                int userID = AccountLogic.GetUserID(Session["Username"].ToString());
                if (!TeamLogic.IsUserMemberOfTeam(userID, id))
                {
                    Response.Redirect("~/Account/Login");
                }
            }
            else
            {
                Response.Redirect("~/Account/Login");
            }

            return View();
        }

        public JsonResult GetPlayers(int id)
        {
            List<UserOnTeam> players = TeamLogic.GetPlayersOnTeam(id);
            return Json(players, JsonRequestBehavior.AllowGet);
        }
    }
}
