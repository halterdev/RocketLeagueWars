using System;
using System.Configuration;
using RocketLeagueWars.Models;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Data;

namespace RocketLeagueWars.Logic
{
    public class AccountLogic
    {
        public static int Register(RegisterModel user)
        {
            int result = 0;
            string sql = @"insert into Users (Username, Email, Password) values (@Username, @Email, @Password)
                            select scope_identity()";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DSN"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@Username", user.UserName);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", PasswordHash.CreateHash(user.Password));
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
            DataTable data = new DataTable();
            int result = 0;
            string sql = @"select UserID, Password
                            from Users 
                            where Username = @Username";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@Username", user.UserName);
                new SqlDataAdapter(command).Fill(data);

                if (data.Rows.Count > 0)
                {
                    DataRow userRow = data.Rows[0];
                    string hash = userRow["Password"].ToString();
                    if (PasswordHash.ValidatePassword(user.Password, hash))
                    {
                        result = Convert.ToInt32(userRow["UserID"]);
                    }
                }
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
        public static bool DoesUsernameExist(string username)
        {
            bool result = false;
            string sql = @"select count(UserID)
                            from Users
                            where Username = @Username";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@Username", username);
                conn.Open();
                result = Convert.ToInt32(command.ExecuteScalar()) > 0;
                conn.Close();
            }

            return result;
        }
        public static bool DoesEmailExist(string email)
        {
            bool result = false;
            string sql = @"select count(UserID)
                            from Users
                            where Email = @Email";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@Email", email);
                conn.Open();
                result = Convert.ToInt32(command.ExecuteScalar()) > 0;
                conn.Close();
            }

            return result;
        }
    }

    /// <summary>
    /// Salted password hashing with PBKDF2-SHA1.
    /// Author: havoc AT defuse.ca
    /// www: http://crackstation.net/hashing-security.htm
    /// Compatibility: .NET 3.0 and later.
    /// </summary>
    public class PasswordHash
    {
        // The following constants may be changed without breaking existing hashes.
        public const int SALT_BYTE_SIZE = 24;
        public const int HASH_BYTE_SIZE = 24;
        public const int PBKDF2_ITERATIONS = 1000;

        public const int ITERATION_INDEX = 0;
        public const int SALT_INDEX = 1;
        public const int PBKDF2_INDEX = 2;

        /// <summary>
        /// Creates a salted PBKDF2 hash of the password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hash of the password.</returns>
        public static string CreateHash(string password)
        {
            // Generate a random salt
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SALT_BYTE_SIZE];
            csprng.GetBytes(salt);

            // Hash the password and encode the parameters
            byte[] hash = PBKDF2(password, salt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE);
            return PBKDF2_ITERATIONS + ":" +
                Convert.ToBase64String(salt) + ":" +
                Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Validates a password given a hash of the correct one.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <param name="correctHash">A hash of the correct password.</param>
        /// <returns>True if the password is correct. False otherwise.</returns>
        public static bool ValidatePassword(string password, string correctHash)
        {
            // Extract the parameters from the hash
            char[] delimiter = { ':' };
            string[] split = correctHash.Split(delimiter);
            int iterations = Int32.Parse(split[ITERATION_INDEX]);
            byte[] salt = Convert.FromBase64String(split[SALT_INDEX]);
            byte[] hash = Convert.FromBase64String(split[PBKDF2_INDEX]);

            byte[] testHash = PBKDF2(password, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        /// <summary>
        /// Compares two byte arrays in length-constant time. This comparison
        /// method is used so that password hashes cannot be extracted from
        /// on-line systems using a timing attack and then attacked off-line.
        /// </summary>
        /// <param name="a">The first byte array.</param>
        /// <param name="b">The second byte array.</param>
        /// <returns>True if both byte arrays are equal. False otherwise.</returns>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }

        /// <summary>
        /// Computes the PBKDF2-SHA1 hash of a password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The PBKDF2 iteration count.</param>
        /// <param name="outputBytes">The length of the hash to generate, in bytes.</param>
        /// <returns>A hash of the password.</returns>
        private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            pbkdf2.IterationCount = iterations;
            return pbkdf2.GetBytes(outputBytes);
        }
    }
}