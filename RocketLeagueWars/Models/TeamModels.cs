using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.Mvc;

namespace RocketLeagueWars.Models
{
    [Table("Teams")]
    public class Team
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TeamID { get; set; }
        public int LeagueID { get; set; }
        public string TeamName { get; set; }
    }

    public class TeamDBContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
    }

    public class UserOnTeam
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public int TeamID { get; set; }
    }

    [Table("TeamSeasonStats")]
    public class TeamSeasonStats
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TeamSeasonStatID { get; set; }
        public int TeamID { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Season { get; set; }
        public string TeamName { get; set; }
    }

}
