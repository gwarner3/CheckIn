using System;
using System.Linq;
using System.Web.Mvc;
using CheckInWeb.Data.Context;
using CheckInWeb.Data.Entities;
using CheckInWeb.Data.Repositories;
using CheckInWeb.Models;

namespace CheckInWeb.Controllers
{
    public class LocationController : Controller
    {
        private readonly ILocationRepository locationRepository;

        public LocationController()
            : this(new LocationRepository())
        {
        }

        public LocationController(ILocationRepository locationRepository)
        {
            this.locationRepository = locationRepository;
        }

        //
        // GET: /Location/
        public ActionResult Index()
        {
            // Get the data
            var model = new LocationIndexViewModel();

            model.Locations = this.locationRepository
                .GetLocations()
                .OrderBy(x => x.Name)
                .ToList();

            return this.View(model);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult AddLocation(Location location)
        {

            if (!string.IsNullOrEmpty(location.Name))
            {
                var repository = new Repository(new CheckInDatabaseContext());
                var newLocation = new Location { Name = location.Name.Trim() };
                repository.Insert(newLocation);
                repository.SaveChanges();
            }

            return View();
        }
    }
}