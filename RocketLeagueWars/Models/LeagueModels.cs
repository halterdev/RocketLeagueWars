using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace RocketLeagueWars.Models
{
    [Table("Leagues")]
    public class League
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LeagueID { get; set; }
        public string LeagueName { get; set; }
    }

    public class LeagueDBContext : DbContext
    {
        public DbSet<League> Leagues { get; set; }
    }
}