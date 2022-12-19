using RailwayReservationMVC.Models.DAL;
using RailwayReservationMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Numerics;
using System.Web.Util;

namespace RailwayReservationMVC.Controllers
{
    
    public class PaymentController : Controller
    {
        public string transactionId;
        // GET: Payment
        private IRailwayRepository<Reservation> ResObj;
        private IRailwayRepository<TrainDetails> TrainObj;
        public PaymentController()
        {
            this.ResObj = new RailwayRepository<Reservation>();
            this.TrainObj = new RailwayRepository<TrainDetails>();

        }
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CreateOrder(Models.OrderModel _requestData)
        {

            Random randomObj = new Random();
              transactionId = randomObj.Next(10000000, 100000000).ToString();
            Session["transactionId"] = transactionId;

            Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient("rzp_test_aJVAJtaSskH7US", "5ug92ZJIU89n0R3SKU7O0RP8");
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", 500 * 100);
            options.Add("receipt", transactionId);
            options.Add("currency", "INR");
            options.Add("payment_capture", "0");
            Razorpay.Api.Order orderResponse = client.Order.Create(options);
            string orderId = orderResponse["id"].ToString();

            Models.OrderModel orderModel = new Models.OrderModel
            {
                orderId = orderResponse.Attributes["id"],
                razorpayKey = "rzp_test_aJVAJtaSskH7US",
                amount = 500 * 100,
                currency = "INR",
                name = _requestData.name,
                email = _requestData.email,
                contactNumber = _requestData.contactNumber,
                address = _requestData.address,

            };

            return View("PaymentPage", orderModel);
        }




        [HttpPost]
        public ActionResult Complete(Reservation reserve)
        {
           
            string paymentId = Request.Params["rzp_paymentid"];

            string orderId = Request.Params["rzp_orderid"];

            Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient("rzp_test_aJVAJtaSskH7US", "5ug92ZJIU89n0R3SKU7O0RP8");

            Razorpay.Api.Payment payment = client.Payment.Fetch(paymentId);

            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", payment.Attributes["amount"]);
            Razorpay.Api.Payment paymentCaptured = payment.Capture(options);
            string amt = paymentCaptured.Attributes["amount"];



            if (paymentCaptured.Attributes["status"] == "captured")
            {
                //sessions
               


                Guid guid = Guid.NewGuid();

                BigInteger big = new BigInteger(guid.ToByteArray());
                var pnr = big.ToString().Substring(0, 10);

                var str = pnr.Replace("-", string.Empty);
                List<TrainDetails> trainDetails = TrainObj.GetModel().ToList();
                



                reserve.PNR_NO = str;
                Session["PNR"] = str;
                reserve.Res_Name = (string)Session["ResName"];
                reserve.Res_Gender = (string)Session["ResGender"];
                //reserve.TrainDetails.SourceStation = Session["SourceStation"];
                //reserve.TrainDetails.DestinationStation = (string)Session["DestinationStation"];
                reserve.QuotaType= (string)Session["Quota"];

                reserve.User_Id = 1; /*(int)Session["user_id"];*/
                
                var train_Id = from id in  trainDetails where id.SourceStation == reserve.TrainDetails.SourceStation select id;
                //var transactionid = Session["transactionId"];
                reserve.Transaction_Id = transactionId;
                ResObj.InsertModel(reserve);
                ResObj.Save();
                return RedirectToAction("Success");
            }
            else
            {
                return RedirectToAction("Failed");
            }
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Failed()
        {
            return View();
        }
    }
}