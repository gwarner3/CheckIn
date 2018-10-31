using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CheckInWeb.Data.Context;
using CheckInWeb.Data.Entities;
using CheckInWeb.Data.Repositories;

namespace CheckInWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository repository;

        public HomeController()
            : this(new Repository(new CheckInDatabaseContext()))
        {
        }

        public HomeController(IRepository repository)
        {
            this.repository = repository;
        }

        public ActionResult Index()
        {
            // Request #6
            // You can use the snippet below to run SQL in Entity Framework, but feel free to implement the solution using your prefered framework.
            //var result = this.repository.SqlQuery<Top10LocationsModel>("SQL QUERY HERE");
            //return View(result);

            //var result = from c in repository.Query<CheckIn>()
            //    where DbFunctions.TruncateTime(c.Time) == DateTime.Now.AddDays(-30)
            //    group c by c.Location;



            return View();
        }
    }
}