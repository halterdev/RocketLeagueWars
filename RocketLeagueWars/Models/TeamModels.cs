﻿using System;
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

}
