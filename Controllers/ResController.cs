using RailwayReservationMVC.Models.DAL;
using RailwayReservationMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace RailwayReservationMVC.Controllers
{
    public class ResController : Controller
    {

        private IRailwayRepository<TrainDetails> TrainObj;
        private IRailwayRepository<Reservation> ReservationObj;
        public ResController()
        {
            this.TrainObj = new RailwayRepository<TrainDetails>();
            this.ReservationObj = new RailwayRepository<Reservation>();

        }
        //reservation page
        public ActionResult Index()
        {

            List<TrainDetails> trainDetails = TrainObj.GetModel().ToList();

            ViewData["TrainDetailS"] = new SelectList(trainDetails, dataValueField: "SourceStation", dataTextField: "SourceStation");
            ViewData["TrainDetailDesti"] = new SelectList(trainDetails, dataValueField: "DestinationStation", dataTextField: "DestinationStation");

            return View();

        }
        [HttpPost]
        public ActionResult Index(Reservation res)

        {
            List<TrainDetails> trainDetails = TrainObj.GetModel().ToList();

            ViewData["TrainDetailS"] = new SelectList(trainDetails, dataValueField: "SourceStation", dataTextField: "SourceStation");
            ViewData["TrainDetailDesti"] = new SelectList(trainDetails, dataValueField: "DestinationStation", dataTextField: "DestinationStation");

            if(res != null)
            {
                Session["ResName"] = res.Res_Name;
                Session["ResGender"] = res.Res_Gender;
                Session["SourceStation"] = res.TrainDetails.SourceStation;
                Session["DestinationStation"] = res.TrainDetails.DestinationStation;
                Session["Quota"] = res.QuotaType;
                //res.User_Id = (int)Session["user_id"];
                //var train_Id = from id in trainDetails where id.SourceStation == res.TrainDetails.SourceStation select id;

                //ReservationObj.InsertModel(res);
                //ReservationObj.Save();

                return RedirectToAction("Index", "Payment");
            }
            
            return View();

        }

    }
}
