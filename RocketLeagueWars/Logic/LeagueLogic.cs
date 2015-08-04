using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RocketLeagueWars.Models;

namespace RocketLeagueWars.Logic
{
    class LeagueLogic
    {
        public static IEnumerable<League> GetLeagues()
        {
            DataTable leagueData = new DataTable();
            string sql = @"select LeagueID, LeagueName
                            from Leagues";
            
            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                new SqlDataAdapter(command).Fill(leagueData);
            }

            List<League> leagues = new List<League>();
            foreach (DataRow leagueRow in leagueData.Rows)
            {
                League league = new League();
                league.LeagueID = Convert.ToInt32(leagueRow["LeagueID"]);
                league.LeagueName = Convert.ToString(leagueRow["LeagueName"]);
                leagues.Add(league);
            }

            return leagues;
        }
        public static League GetLeague(int leagueID)
        {
            DataTable leagueData = new DataTable();
            string sql = @"select LeagueID, LeagueName 
                            from Leagues 
                            where LeagueID = @LeagueID";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@LeagueID", leagueID);
                new SqlDataAdapter(command).Fill(leagueData);
            }

            League league = new League();
            league.LeagueID = leagueID;
            league.LeagueName = leagueData.Rows[0]["LeagueName"].ToString();

            return league;
        }
    }
}
