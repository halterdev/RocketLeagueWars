using System;
using System.Configuration;
using RocketLeagueWars.Models;
using System.Data.SqlClient;

namespace RocketLeagueWars.Logic
{
    public class AccountLogic
    {
        public static int Register(RegisterModel user)
        {
            int result = 0;
            string sql = @"insert into Users (Username, Password) values (@Username, @Password)
                            select scope_identity()";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DSN"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@Username", user.UserName);
                command.Parameters.AddWithValue("@Password", user.Password);
                conn.Open();
                object userID = command.ExecuteScalar();
                if (userID != null)
                {
                    result = Convert.ToInt32(userID);
                }
                conn.Close();
            }

            return result;
        }
        public static void UpdatePlayerSeasonStats(PlayerSeasonStat stats, SubmitGameModel game)
        {
            int teamID = GetTeamID(stats.UserID);
            int points = GameLogic.GetPointsForGameType(Convert.ToInt32(game.GameTypeID));
            string sql = @"declare @Count int
                            set @Count = (select count(pss.PlayerSeasonStatID)
                                            from PlayerSeasonStats pss
                                            where UserID = @UserID
                                                and Season = @Season)
                           if @Count = 0
                           begin 
                               insert into PlayerSeasonStats (UserID, Wins, Losses, Season, Points)
                                values (@UserID, 
                                        case @Win when 1 then 1 else 0 end, 
                                        case @Win when 1 then 0 else 1 end,
                                        @Season, @Points)
                           end 
                           else 
                               if @Win = 1
                               begin
                                    update PlayerSeasonStats 
                                    set Wins = Wins + 1, Points = Points + @Points
                                    where UserID = @UserID and Season = @Season
                               end
                               else 
                                    update PlayerSeasonStats
                                    set Losses = Losses + 1
                                    where UserID = @UserID and Season = @Season";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@UserID", stats.UserID);
                command.Parameters.AddWithValue("@Season", stats.Season);
                command.Parameters.AddWithValue("@Points", points);
                command.Parameters.AddWithValue("@Win", teamID == Convert.ToInt32(game.WinningTeamID));
                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static int Login(LoginModel user)
        {
            int result = 0;
            string sql = @"select UserID
                            from Users 
                            where Username = @Username";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@Username", user.UserName);
                conn.Open();
                object userID = command.ExecuteScalar();
                if (userID != null)
                {
                    result = Convert.ToInt32(userID);
                }
                conn.Close();
            }

            return result;
        }
        public static int GetUserID(string username)
        {
            int result = 0;
            string sql = @"select UserID
                            from Users 
                            where Username = @Username";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@Username", username);
                conn.Open();
                result = Convert.ToInt32(command.ExecuteScalar());
                conn.Close();
            }

            return result;
        }
        public static int GetTeamID(int userID)
        {
            int result = 0;
            string sql = @"select isnull(TeamID, 0)
                            from Users
                            where UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@UserID", userID);
                conn.Open();
                result = Convert.ToInt32(command.ExecuteScalar());
                conn.Close();
            }

            return result;
        }
    }
}