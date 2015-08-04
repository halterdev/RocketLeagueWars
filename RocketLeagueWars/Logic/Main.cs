using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketLeagueWars.Logic
{
    class Main
    {
        public static string GetDSN()
        {
            return ConfigurationManager.ConnectionStrings["DSN"].ConnectionString;
        }
    }
}
