using RailwayReservationMVC.Models;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;
using System.Text;
using RailwayReservationMVC.Models.DAL;

namespace RailwayReservationMVC.Controllers
{
    public class HomeController : Controller
    {
        #region Repository Object
        private IRailwayRepository<Reservation> ReservationObject;

        private IRailwayRepository<TrainDetails> TrainDetailsObject;
        private IRailwayRepository<User> UserObject;
        #endregion


        #region Repository Constructor
        public HomeController()
        {
            this.ReservationObject = new RailwayRepository<Reservation>();
            this.TrainDetailsObject = new RailwayRepository<TrainDetails>();
            this.UserObject = new RailwayRepository<User>();

        }
        #endregion


        #region Homepage
        public ActionResult Homepage()
        {
            var data = TrainDetailsObject.GetModel().ToList();
            return View(data);
        }

        #endregion



        #region Search Train
        [HttpPost]
        public ActionResult Homepage(string searching)
        {
            return View(TrainDetailsObject.GetModel().Where(x => x.DestinationStation.Contains(searching) || searching == null).ToList());
        }



        #endregion



        #region SignUp and Email Notification
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(User u)
        {
            //email notification
            MailMessage mm = new MailMessage("railwayreservationsystemmail@gmail.com", u.Email);

            mm.Subject = "Welcome to Railway Reservation System";
            mm.Body = "This is your password :" + u.Password.ToString() + "\n" + "Have a safe journey with us !";
            mm.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential("railwayreservationsystemmail@gmail.com", "chfxpbtcfjfobhlv");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mm);
            TempData["Message"] ="Thank you for Connecting with us!Your password has been sent to your regsitered mail id  ";

            //password encryption 
            u.Password = Convert.ToBase64String(
            System.Security.Cryptography.SHA256.Create()
            .ComputeHash(Encoding.UTF8.GetBytes(u.Password)));

            //update database with new user
            UserObject.InsertModel(u);
            UserObject.Save();
            return RedirectToAction("afterlogin");
        }

        #endregion



        #region Login
        public ActionResult Login()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
               
                var data = UserObject.GetModel().Where(model => model.Email.Equals(email) && model.Password.Equals(password)).ToList();
                if (data.Count() > 0)
                { 
                    return RedirectToAction("afterlogin");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }

            return View();
        }

        public ActionResult AdminLogin()
        {
            return View("");
        }
        [HttpPost]
        public ActionResult AdminLogin(string email, string password, string role)
        {
            if (ModelState.IsValid)
            {

                var data = UserObject.GetModel().Where(model => model.Email.Equals(email) && model.Password.Equals(password) && model.Role.Equals(role)).ToList();
                if (data.Count() > 0)
                {
                    
                    return RedirectToAction("Updatetrain");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("AdminLogin");
                }
            }

            return View();
        }

        public ActionResult afterlogin()
        {
            return View();
        }

        #endregion



        #region logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        #endregion


        #region Reservation

        public ActionResult ReservationPage()
        {
            var data = ReservationObject.GetModel();
            return View(data);
        }
        public ActionResult CreateReservation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateReservation(Reservation res)

        {

            var data = ReservationObject.GetModel();
            if (data.Count() >= 7)
            {
                TempData["msg"] = "<script>alert('YOU CANNOT ADD MORE THAN 6 PASSENGER');</script>";
            }
            else
            {

                ReservationObject.InsertModel(res);
                ReservationObject.Save();

                return RedirectToAction("ReservationPage");
            }
            return View(res);

        }

        public ActionResult DeleteReservation(int id)
        {
            Reservation rt = ReservationObject.GetModelByID(id);
            return View(rt);
        }
        [HttpPost]
        public ActionResult DeleteReservation(int id, Reservation reservation)
        {

            ReservationObject.DeleteModel(id);
            ReservationObject.Save();
            return RedirectToAction("ReservationPage");
        }

        #endregion



    }
}
  