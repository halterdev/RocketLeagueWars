using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.Mvc;
using RocketLeagueWars.Logic;


namespace RocketLeagueWars.Models
{
    [Table("Games")]
    public class Game
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int GameID { get; set; }
        [ForeignKey("Team")]
        public int WinningTeamID { get; set; }
        [ForeignKey("Team")]
        public int LosingTeamID { get; set; }
        public int WinningScore { get; set; }
        public int LosingScore { get; set; }
    }

    public class SubmitGameModel
    {
        [Required]
        [Display(Name = "Winning Team")]
        public string WinningTeamID { get; set; }

        [Required]
        [Display(Name = "Losing Team")]
        public string LosingTeamID { get; set; }

        [Required]
        [Display(Name = "Game Type")]
        public string GameTypeID { get; set; }

        [Required]
        [Display(Name = "Winning Score")]
        public string WinningScore { get; set; }

        [Required]
        [Display(Name = "Losing Score")]
        public string LosingScore { get; set; }

        public List<SelectListItem> GameTypesDDL = new List<SelectListItem>()
        {
            new SelectListItem() { Value = "0", Text = "-- Select Game Type --" }, 
            new SelectListItem() { Value = "1", Text = "1 vs. 1" }, 
            new SelectListItem() { Value = "2", Text = "2 vs. 2" },
            new SelectListItem() { Value = "3", Text = "3 vs. 3" }, 
            new SelectListItem() { Value = "4", Text = "4 vs. 4" }, 
        };
        public List<SelectListItem> WinningTeamDDL = TeamLogic.GetTeamsDDL(true);
        public List<SelectListItem> LosingTeamDDL = TeamLogic.GetTeamsDDL(false);
    }
}
