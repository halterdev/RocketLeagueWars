using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using RocketLeagueWars.Models;

namespace RocketLeagueWars.Logic
{
    class TeamLogic
    {
        public static IEnumerable<Team> GetTeams(int leagueID)
        {
            DataTable teamData = new DataTable();
            string sql = @"select TeamID, LeagueID, TeamName
                            from Teams
                            where LeagueID = @LeagueID";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@LeagueID", leagueID);
                new SqlDataAdapter(command).Fill(teamData);
            }

            List<Team> teams = new List<Team>();
            foreach (DataRow teamRow in teamData.Rows)
            {
                Team team = new Team();
                team.TeamID = Convert.ToInt32(teamRow["TeamID"]);
                team.LeagueID = Convert.ToInt32(teamRow["LeagueID"]);
                team.TeamName = teamRow["TeamName"].ToString();
                teams.Add(team);
            }

            return teams;
        }
        public static Team GetTeam(int teamID)
        {
            DataTable teamData = new DataTable();
            string sql = @"select TeamID, LeagueID, TeamName
                            from Teams 
                            where TeamID = @TeamID";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@TeamID", teamID);
                new SqlDataAdapter(command).Fill(teamData);
            }

            Team team = new Team();
            team.TeamID = Convert.ToInt32(teamData.Rows[0]["TeamID"]);
            team.LeagueID = Convert.ToInt32(teamData.Rows[0]["LeagueID"]);
            team.TeamName = teamData.Rows[0]["TeamName"].ToString();

            return team;
        }
        public static bool IsUserMemberOfTeam(int userID, int teamID)
        {
            bool result = false;
            string sql = @"select isnull(TeamID, 0)
                            from Users 
                            where UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@UserID", userID);
                conn.Open();
                result = Convert.ToInt32(command.ExecuteScalar()) == teamID;
                conn.Close();
            }

            return result;
        }
        public static List<SelectListItem> GetTeamsDDL(bool winning)
        {
            DataTable results = new DataTable();
            string sql = @"select TeamID, TeamName
                            from Teams
                            order by TeamName";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                new SqlDataAdapter(command).Fill(results);
            }

            List<SelectListItem> teams = new List<SelectListItem>();
            foreach(DataRow teamRow in results.Rows)
            {
                teams.Add(new SelectListItem() { Value = teamRow["TeamID"].ToString(), Text = teamRow["TeamName"].ToString() });
            }

            if(winning)
            {
                teams.Insert(0, new SelectListItem() { Value = "0", Text = "-- Select Winning Team --" });
            }
            else
            {
                teams.Insert(0, new SelectListItem() { Value = "0", Text = "-- Select Losing Team --" });
            }

            return teams;
        }
    }
}
