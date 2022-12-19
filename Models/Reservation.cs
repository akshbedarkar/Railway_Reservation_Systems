using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace RailwayReservationMVC.Models
{
    public class Reservation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Res_Id { get; set; }
        public string Res_Name { get; set; }

        public string Res_Gender { get; set; }
        //public string Res_Address { get; set; }

        [ForeignKey("User")]

        public int User_Id { get; set; }
        public User User { get; set; }
        public string QuotaType { get; set; }
       
        public string Res_Date { get; set; }

        //[ForeignKey("TrainDetails")]
        //public int Train_Id { get; set; }
        public TrainDetails TrainDetails { get; set; }
        public string PNR_NO { get; set; }
        public int Seat_No { get; set; }
        public string Transaction_Id { get; set; }
    }
}