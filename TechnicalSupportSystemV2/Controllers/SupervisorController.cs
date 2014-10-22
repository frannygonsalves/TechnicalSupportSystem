using Postal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechnicalSupportSystem.Models;
using TechnicalSupportSystem.DAL;
using TechnicalSupportSystem.ViewModels;

namespace TechnicalSupportSystem.Controllers
{
    [Authorize]
    public class SupervisorController : Controller
    {
        // Initalises the db
        private SystemDBContext db = new SystemDBContext();

        //
        // GET: /Supervisor/

        // Method to display menu */
        public ActionResult Index()
        {
            string username = User.Identity.Name;
            Supervisor supervisor = db.Supervisors.SingleOrDefault(i => i.UserProfile.UserName == username);

            return View(supervisor);
        }

        /* Method which allows supervisors to view projects and change project budgets */
        [HttpGet]
        public ActionResult ProjectsAllocated()
        {
            string username = User.Identity.Name; // gets currrent logged in user
            Supervisor supervisor = db.Supervisors.SingleOrDefault(i => i.UserProfile.UserName == username);


            // retrieves projects where supervisor ip matches with current logged in supervisor id
            var query = from p in db.Projects.Include(b => b.Student).Include(c => c.Supervisor)
                        where p.Supervisor.SupervisorID == supervisor.SupervisorID
                        select p;

            return View(query);
        }

        [HttpGet]
        public ActionResult ApproveOrders()
        {
            //OrdersPendingIndexView viewModel = new OrdersPendingIndexView();

            string username = User.Identity.Name; // get current logged in user

            Supervisor supervisor = db.Supervisors.SingleOrDefault(s => s.UserProfile.UserName == username); // gets supervisor from username

            // query retrieves orders which have not been checked by project supervisor 
            var query = from o in db.Orders.Include("Student").Include("Student.Project").Include("Components")
                        where (o.Student.Project.Supervisor.SupervisorID == supervisor.SupervisorID && o.IsChecked == false) // display orders for students for supervisor
                        select o;

            // checks each order if they are above budget and if they are then marks the boolean as true and saves to db
            foreach (Order o in query)
            {
                if (o.OrderTotal > o.Student.Project.Budget)
                {
                    o.IsOverBudget = true;
                    db.Entry(o).State = EntityState.Modified;
                }
            }

            db.SaveChanges();

            // view model.Ienumberalbe = query

            return View(query);
        }

        /* Retrieves order details from db and sends to view */
        public ActionResult ViewComponentDetails(int id)
        {
            Order order = db.Orders.Find(id);
            return View(order);
        }

        /* Updates the order status of order to approved when approve is clicked in view */
        [HttpPost]
        public ActionResult UpdateOrderAsApproved(int id)
        {
            Order order = db.Orders.Find(id); // finds order
            order.IsApproved = true;
            order.IsChecked = true;

            // checks if it's over budget and if it is then budget becomes 0.
            if (order.IsOverBudget)
            {
                order.Student.Project.Budget = 0;
            }
            else // deducts cost from budget
            {
                BudgetDeduction(order.OrderID);
            }
            
            // sends order approved email

                SendOrderApprovedEmail(order.OrderID);

            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ApproveOrders");
        }

        /* Method which updates order as rejected when reject button pressed */
        [HttpPost]
        public ActionResult UpdateOrderAsRejected(int id)
        {
            Order order = db.Orders.Find(id); // finds order
            order.IsChecked = true; //sets status to true and approved status is set as default to false
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ApproveOrders");
        }
        /* Method to retrieve what orders are approved */
        [HttpGet]
        public ActionResult ApprovedOrders()
        {
            string username = User.Identity.Name; // gets current logged in username
            Supervisor supervisor = db.Supervisors.SingleOrDefault(i => i.UserProfile.UserName == username); // retrieves supervisor from db

            // finds orders which had been approved
            var query = from o in db.Orders.Include("Student").Include("Student.Project").Include("Components")
                        where (o.Student.Project.Supervisor.SupervisorID == supervisor.SupervisorID && o.IsApproved == true && o.IsChecked == true)
                        select o;

            return View(query);
        }

        // Method to retrieve what orders are rejected
        [HttpGet]
        public ActionResult RejectedOrders()
        {
            string username = User.Identity.Name; // gets current logged in username
            Supervisor supervisor = db.Supervisors.SingleOrDefault(i => i.UserProfile.UserName == username); // gets supervisor from db

            // finds orders which had been rejected
            var query = from o in db.Orders.Include("Student").Include("Student.Project").Include("Components")
                        where (o.Student.Project.Supervisor.SupervisorID == supervisor.SupervisorID && o.IsApproved == false && o.IsChecked == true)
                        select o;

            return View(query);
        }

        /* Method to deduct budget once order is approved */
        private void BudgetDeduction(int id)
        {
            Order order = db.Orders.Include("Components").Include("Student.Project").SingleOrDefault(i => i.OrderID == id); // gets order from db
            decimal cost = order.OrderTotal; 
            Student student = order.Student;
            decimal budget = student.Project.Budget;
            budget = budget - cost; // does the subtraction 

            student.Project.Budget = budget; // sets the new budget

            if (ModelState.IsValid) // add order to table
            {
                db.Entry(student).State = EntityState.Modified;

                db.SaveChanges();
            }

        }

        /* A Project budget editing view*/
        [HttpGet]
        public ActionResult AdjustProjectBudget(int id)
        {
            Project project = db.Projects.Find(id);
            return View(project);
            
        }

        /* A method to set the new project budget */
        [HttpPost]
        public ActionResult AdjustProjectBudget(Project projectEdit)
        {
            Project project = db.Projects.Find(projectEdit.ProjectID); // get project information
            project.Budget = projectEdit.Budget; // set the project budget only nothing else

                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
            
            return RedirectToAction("ProjectsAllocated");
        }

   

        /* Method to send Order approval email using Postal API*/
        private void SendOrderApprovedEmail(int id)
        {
            Order order = db.Orders.Include("Student").Include("Student.UserProfile").FirstOrDefault(i => i.OrderID == id);
            Student student = order.Student;
            dynamic email = new Email("OrderApprovedEmail");
            email.To = student.UserProfile.Email;
            email.FirstName = student.FirstName;
            email.LastName = student.LastName;
            email.VendorOrderNumber = order.VendorOrderNumber;
            email.Send();
        }

        /* Method to send order rejection email using Postal API*/
        private void SendOrderRejectedEmail(int id)
        {
            Order order = db.Orders.Include("Student").Include("Student.UserProfile").FirstOrDefault(i => i.OrderID == id);
            Student student = order.Student;
            dynamic email = new Email("OrderApprovedEmail");
            email.To = student.UserProfile.Email;
            email.FirstName = student.FirstName;
            email.LastName = student.LastName;
            email.VendorOrderNumber = order.VendorOrderNumber;
            email.Send();
        }

        /*Clears memory allocated for db*/
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}