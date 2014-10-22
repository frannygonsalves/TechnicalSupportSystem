using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechnicalSupportSystem.Models;
using TechnicalSupportSystemV2.DAL;
using TechnicalSupportSystemV2.Models;


namespace TechnicalSupportSystemV2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private SystemDBContext db = new SystemDBContext();

        public ActionResult Index()
        {
            string username = User.Identity.Name; // get current logged in user

            Student student = db.Students.SingleOrDefault(s => s.UserProfile.UserName == username);
            Supervisor supervisor = db.Supervisors.Include("UserProfile").FirstOrDefault(s => s.UserProfile.UserName == username);// check logged in username vs name on table
            Technician technician = db.Technicians.Include("UserProfile").SingleOrDefault(s => s.UserProfile.UserName == username);// check logged in username vs name on table
           
            if (student != null)
            {
                return RedirectToRoute("Student");
            }

            else if (technician != null)
            {
                return RedirectToRoute("Technician");
            }

            else if(supervisor!=null){

                // get update project details with the right supervisor details
                foreach (Project project in db.Projects.Include("Supervisor").Include("Supervisor.UserProfile"))
                {
                    // modify fields
                    if (project.Supervisor.UserProfile.Email == supervisor.UserProfile.Email && project.Supervisor.FirstName==null)
                    {
                        Supervisor oldSupervisor = project.Supervisor;
                        UserProfile oldSupervisorUserProfile= project.Supervisor.UserProfile;
                        project.Supervisor = supervisor; // update the project supervisor details
                        db.Entry(oldSupervisor).State = EntityState.Deleted; // delete old supervisor details
                        db.Entry(oldSupervisorUserProfile).State = EntityState.Deleted; // delete old supervisor user profile// 
                        db.Entry(project).State = EntityState.Modified; // update project details
                    }
                }
                db.SaveChanges();
                return RedirectToRoute("Supervisor");
            }

            else if(User.IsInRole("Admin"))
            {
                return RedirectToRoute("Admin");
            }

            return View();
        }


        public ActionResult Admin()
        {
            return View();
        }

        [Authorize(Roles="Admin")]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
