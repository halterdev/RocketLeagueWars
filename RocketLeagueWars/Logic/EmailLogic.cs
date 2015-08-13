using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using RocketLeagueWars.Models;

namespace RocketLeagueWars.Logic
{
    public class EmailLogic
    {
        public const int NewRegistrationEmail = 1;

        public static void SendNewRegistrationEmail(RegisterModel model)
        {
            string subj = "Welcome to Rocket League Wars";
            string body = String.Empty;

            StringBuilder sbBody = new StringBuilder();
            sbBody.Append("Hello, ");
            sbBody.Append(model.UserName + "!");
            sbBody.Append("<br> <br>");
            sbBody.Append("Thanks for joining. The Admin Team will now assign you to a team. You will be emailed when this happens.");
            sbBody.Append("<br> <br>");
            sbBody.Append("Thanks,");
            sbBody.Append("<br>");
            sbBody.Append("RocketLeagueWars.com");
            body = sbBody.ToString();

            MailMessage email = new MailMessage();
            email.IsBodyHtml = true;
            email.Subject = subj;
            email.Body = body;
            email.From = GetSupportEmail();
            email.To.Add(model.Email.Trim());

            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.Credentials = new NetworkCredential(GetSupportEmailUsername(), GetSupportEmailPassword());
            smtp.Host = GetSupportEmailHost();
            smtp.Send(email);

            InsertEmailTrackingRow(smtp, AccountLogic.GetUserID(model.UserName));
        }

        private static void InsertEmailTrackingRow(SmtpClient smtp, int userID)
        {
            string sql = @"insert into EmailTracking (EmailTypeID, SentDate, RecipientUserID)
                            values (@EmailTypeID, @SentDate, @RecipientUserID)";

            using (SqlConnection conn = new SqlConnection(Main.GetDSN()))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@EmailTypeID", NewRegistrationEmail);
                command.Parameters.AddWithValue("@SentDate", DateTime.Now);
                command.Parameters.AddWithValue("@RecipientUserID", userID);
                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
            }
        }
        private static MailAddress GetSupportEmail()
        {
            return new MailAddress(WebConfigurationManager.AppSettings["supportEmail"].ToString());
        }
        private static string GetSupportEmailUsername()
        {
            return WebConfigurationManager.AppSettings["supportEmailUsername"].ToString();
        }
        private static string GetSupportEmailPassword()
        {
            return WebConfigurationManager.AppSettings["supportEmailPassword"].ToString();
        }
        private static string GetSupportEmailHost()
        {
            return WebConfigurationManager.AppSettings["supportEmailHost"].ToString();
        }
    }
}
