using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CheckInWeb.Data.Context;
using CheckInWeb.Data.Entities;
using CheckInWeb.Data.Repositories;
using CheckInWeb.Models;

namespace CheckInWeb.Controllers
{
    [Authorize]
    public class CheckInController : Controller
    {
        private readonly IRepository repository;

        public CheckInController()
            : this(new Repository(new CheckInDatabaseContext()))
        {
        }

        public CheckInController(IRepository repository)
        {
            this.repository = repository;
        }

        public ActionResult Index()
        {
            // Get the data
            var model = new MyCheckInViewModel();
            var username = HttpContext.User.Identity.Name;

            model.CheckIns = repository
                .Query<CheckIn>()
                .Include(x => x.Location)
                .Where(x => x.User.UserName == username)
                .Select(x => new CheckInViewModel
                {
                    Id = x.Id,
                    Time = x.Time,
                    Location = x.Location.Name
                })
                .OrderByDescending(x => x.Time)
                .ToList();

            return this.View(model);
        }

        public ActionResult Here(int locationId)
        {
            // Get the data
            var location = repository.GetById<Location>(locationId);
            if (location == null)
            {
                return new HttpNotFoundResult();
            }

            var username = HttpContext.User.Identity.Name;

            var user = repository.Query<ApplicationUser>().SingleOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return new HttpNotFoundResult();
            }

            // make a new check in
            var checkIn = new CheckIn();
            checkIn.User = user;
            checkIn.Location = location;
            checkIn.Time = DateTime.Now;
            repository.Insert(checkIn);
            repository.SaveChanges();

            //Move to separate class
            // check to see if this user meets any achievements
            var allCheckins = repository.Query<CheckIn>().Where(c => c.User.Id == user.Id);
            var allAchievements = repository.Query<Achievement>();
            var allLocationIds = repository.Query<Location>().Select(l => l.Id);
            var allAtThisLocationToday = repository.Query<CheckIn>()
                            .Where(c => DbFunctions.TruncateTime(c.Time) == DateTime.Today
                            && c.Location.Id == checkIn.Location.Id);

            foreach (var otherCheckIn in allAtThisLocationToday)
            {
                var achievementsToday = repository.Query<Achievement>().Where(i =>
                    i.User.Id == otherCheckIn.User.Id && i.TimeAwarded == DateTime.Today && i.Type == AchievementType.InTogether);

                var inTogetherAwardedToday = allAchievements.Any(u => u.User.Id == otherCheckIn.User.Id
                                                                      && DbFunctions.TruncateTime((DateTime?) u.TimeAwarded) == DateTime.Today
                                                                      && u.Type == AchievementType.InTogether);

                if (allAtThisLocationToday.Count() > 1 && !inTogetherAwardedToday && (otherCheckIn.Time >= DateTime.Now.AddHours(-1) || otherCheckIn.Time >= DateTime.Now.AddHours(-1)))
                {
                    var inTogether = new Achievement { Type = AchievementType.InTogether, User = otherCheckIn.User, TimeAwarded = DateTime.Now };
                    repository.Insert(inTogether);
                }
            }

            //two in one day?
            var uniqueCheckInsToday = allCheckins.Where(c => DbFunctions.TruncateTime(c.Time) == DateTime.Today)
                .GroupBy(i => i.Location.Id);

            if (!allAchievements.Any(a => a.Type == AchievementType.TwoInOneDay) && uniqueCheckInsToday.Count() == 2)
            {
                var twoInOneDay = new Achievement { Type = AchievementType.TwoInOneDay, User = user, TimeAwarded = DateTime.Now };
                repository.Insert(twoInOneDay);
            }

            // all locations?
            var hasAll = false;

            foreach (var testLocationId in allLocationIds)
            {
                hasAll = false || allCheckins.Any(c => c.Location.Id == testLocationId);

                if (!hasAll)
                {
                    break;
                }
            }

            if (!allAchievements.Any(a => a.Type == AchievementType.AllLocations) && hasAll)
            {
                var allLocations = new Achievement { Type = AchievementType.AllLocations, User = user, TimeAwarded = DateTime.Now };
                repository.Insert(allLocations);
            }

            // some day we'll have hundreds of achievements!

            repository.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}