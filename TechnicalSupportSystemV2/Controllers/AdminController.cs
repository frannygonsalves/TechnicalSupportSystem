using Postal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TechnicalSupportSystem.Models;
using TechnicalSupportSystem.DAL;
using TechnicalSupportSystem.Models;
using WebMatrix.WebData;

namespace TechnicalSupportSystem.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        
        /* Initalise db */
        public SystemDBContext db = new SystemDBContext();

        /* Menu method*/
        public ActionResult Index()
        {
            return View();
        }

        /* Method to get accounts which need to be approved*/
        public ActionResult ApproveAccounts()
        {
            // get users where useraccount is not verified, checked and has a username filled out.
            var query = from u in db.UserProfiles
                        where u.IsVerifyed==false && u.IsChecked== false && u.UserName!=null
                        select u;
            
            return View(query);

        }

        /*Method to reactivate user accounts with added search functionaility */ 
        public ActionResult ReactivateAccount(string searchString)
        {
            // get users that are not verified and accounts is not checked
            var query = from u in db.UserProfiles
                        where u.IsVerifyed == false && u.IsChecked == true
                        select u;

            // check the search box 
            if (!String.IsNullOrEmpty(searchString))
            {
                try // do a search for the username
                {
                    query = query.Where(u => u.UserName == searchString);

                }
                catch (Exception)
                {

                }
            }

            return View(query);
        }

        /* Method to enable activate accounts */
        [HttpPost]
        public ActionResult EnableAccount(int id)
        {
            UserProfile user = db.UserProfiles.Find(id); // retreives user
            user.IsChecked = true; 
            user.IsVerifyed = true;

            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified; //saves changes of new state to db
                db.SaveChanges();
            }

            return RedirectToAction("ReactivateAccount"); // refreshes the reactivate view

        }

        /* Method to see projects that had not been registered by supervisors despite students entering their supervisor email */
        [HttpGet]
        public ActionResult ProjectsNotRegisteredBySupervisors()
        {
            var projects = from p in db.Projects.Include("Supervisor.UserProfile")
                           where p.Supervisor.UserProfile.UserName ==null
                           select p;
            
            return View(projects);
        }

        /* get list of project registered so that the supervisor could be changed if need*/
        public ActionResult ProjectsRegistered()
        {
            var projects = from p in db.Projects.Include("Supervisor.UserProfile")
                           where p.Supervisor.UserProfile.UserName != null
                           select p;

            return View(projects);
        }

       
        /* Method to display text box to change supervisor email */
        [HttpGet]
        public ActionResult ChangeProjectSupervisor(int id)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            return View(userprofile);

        }

        /* Method to change supervisor */
        [HttpPost]
        public ActionResult ChangeProjectSupervisor(UserProfile userprofile)
        {
            UserProfile userprofileold = db.UserProfiles.Find(userprofile.UserId); // find project supervisor
            userprofileold.Email = userprofile.Email; // change project supervisor email to new supervisor

            db.Entry<UserProfile>(userprofileold).State = EntityState.Modified; // save state changes in db
            db.SaveChanges();

            return RedirectToAction("ProjectsRegistered"); // refresh view
        }

        /* Method to display text box to correct supervisor email */
        [HttpGet]
        public ActionResult CorrectSupervisorEmail(int id)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            return View(userprofile);
        }


        /* Method to correct supervisor email */

        [HttpPost]
        public ActionResult CorrectSupervisorEmail(UserProfile userprofile)
        {
            UserProfile userprofileold = db.UserProfiles.Find(userprofile.UserId); // find old record
            userprofileold.Email = userprofile.Email; // update old with new

            db.Entry<UserProfile>(userprofileold).State = EntityState.Modified;
            db.SaveChanges(); // save changes

            dynamic email = new Email("ProjectRegisteredSupervisorEmailV2"); // notify new supervisor
            email.To = userprofile.Email;

            return RedirectToAction("ProjectsNotRegisteredBySupervisors");
        }

     
        /* Method to show active user in the system */
        public ActionResult DeactivateAccount(string searchString)
        {
            // get list of accounts that are active and display in view
            var query = from u in db.UserProfiles
                        where u.IsVerifyed == true && u.IsChecked == true && u.Email !=null
                        select u;

            // find faster by search box
            if (!String.IsNullOrEmpty(searchString))
            {
                try
                {
                    query = query.Where(u => u.UserName == searchString);

                }
                catch (Exception)
                {

                }
            }
            return View(query);
        }

        
        /* Method to block entry to system */
        [HttpPost]
        public ActionResult DisableAccount(int id)
        {
            UserProfile user = db.UserProfiles.Find(id);
            user.IsChecked = true;
            user.IsVerifyed = false; // set state to false

            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified; // save changes
                db.SaveChanges();
            }

            return RedirectToAction("DeactivateAccount");

        }

        /* Method to activate user account when first registered */
        [HttpPost]
        public ActionResult ActivateAccount(int id)
        {
            UserProfile user = db.UserProfiles.Find(id);
            user.IsChecked = true;
            user.IsVerifyed = true;

            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("ApproveAccounts");
        }

        /* Method to reject user account */
        [HttpPost]
        public ActionResult RejectAccount(int id)
        {
            UserProfile user = db.UserProfiles.Find(id);
            user.IsChecked = true;
            user.IsVerifyed = false;

            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("ApproveAccounts");

        }
        /* Method to show what accounts are rejected */
        [HttpGet]
        public ActionResult DeleteAccounts()
        {
            var query = from u in db.UserProfiles
                        where u.IsVerifyed == false && u.IsChecked == true
                        select u;
            return View(query);
        }


        /*Method to delete rejected accounts*/
        [HttpPost]
        public ActionResult DeleteAccount(int id)
        {
            Student student = db.Students.Include("UserProfile").SingleOrDefault(u => u.UserProfile.UserId == id); // search through relevant tables for related user profile entry

            Technician technician = db.Technicians.Include("UserProfile").SingleOrDefault(u => u.UserProfile.UserId == id);

            Supervisor supervisor = db.Supervisors.Include("UserProfile").SingleOrDefault(u => u.UserProfile.UserId == id);
            
            // if can find related  account then delete account

            if (student != null)
            {
                db.Entry(student).State = EntityState.Deleted;
                db.SaveChanges();
            }
            else if (technician != null)
            {
                db.Entry(technician).State = EntityState.Deleted;
                db.SaveChanges();
            }
            else if (supervisor != null)
            {
                db.Entry(supervisor).State = EntityState.Deleted;
                db.SaveChanges();
            }

            UserProfile user = db.UserProfiles.Find(id);

            ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(user.UserName);


            ((SimpleMembershipProvider)Membership.Provider).DeleteUser(user.UserName, true); // delete membership entry too.

            return RedirectToAction("ApproveAccounts");
        }

    }
}