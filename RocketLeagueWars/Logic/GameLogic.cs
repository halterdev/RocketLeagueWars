using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RocketLeagueWars.Models;

namespace RocketLeagueWars.Logic
{
    public class GameLogic
    {
        public static int OneVsOne = 1;
        public static int TwoVsTwo = 2;
        public static int ThreeVsThree = 3; 
        public static int FourVsFour = 4;

        public static void Submit(SubmitGameModel game)
        {
            string sql = @"insert into Games (WinningTeamID, LosingTeamID, GameTypeID, WinningScore, LosingScore)
                            values (@WinningTeamID, @LosingTeamID, @GameTypeID, @WinningScore, @LosingScore)";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@WinningTeamID", Convert.ToInt32(game.WinningTeamID));
                command.Parameters.AddWithValue("@LosingTeamID", Convert.ToInt32(game.LosingTeamID));
                command.Parameters.AddWithValue("@GameTypeID", Convert.ToInt32(game.GameTypeID));
                command.Parameters.AddWithValue("@WinningScore", Convert.ToInt32(game.WinningScore));
                command.Parameters.AddWithValue("@LosingScore", Convert.ToInt32(game.LosingScore));
                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
