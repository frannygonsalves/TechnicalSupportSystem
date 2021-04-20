using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using Postal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechnicalSupportSystem.Models;
using TechnicalSupportSystem.DAL;
using TechnicalSupportSystem.ViewModels;


namespace TechnicalSupportSystem.Controllers
{
    public class TechnicianController : Controller
    {
        //Initalises db
        SystemDBContext db = new SystemDBContext();
        //
        // GET: /Technician/

        /*Displays menu and graph*/
        public ActionResult Index()
        {
            string username = User.Identity.Name;
            Technician technician = db.Technicians.SingleOrDefault(s => s.UserProfile.UserName == username);
            TechnicianOrderOverview viewModel = new TechnicianOrderOverview(); // view model created to show extra information
            viewModel.Technician = technician; // pass technician object to viewmodel object

            // find out how many people did not collect orders despite being notified*/
            var ordersNotCollected = from o in db.Orders
                         where o.StudentIsNotified == true && o.IsCollected == false   // find the notified orders 
                         select o;
            viewModel.OrdersNotCollected = ordersNotCollected.Count();

            // find out how many orders the supervisor needs to be order on Onecall 
            var ordersWhichNeedOrdering = from o in db.Orders
                         where o.IsOrdered == false && o.IsApproved == true
                         select o;
            viewModel.OrdersWaitingToBeOrdered = ordersWhichNeedOrdering.Count();

            // Find how many students need to be notified that their order had been delivered 
            var ordersNotNotfied = from o in db.Orders
                                   where o.StudentIsNotified == false && o.IsOrdered == true   // find the notified orders 
                                   select o;
            viewModel.StudentsWhoNeedToBeNotified = ordersNotNotfied.Count();

            viewModel = PieChartForOrdersOverview(viewModel); // use method and use viewmodel object. 


            return View(viewModel); // send to view
        }

        /* Method to send email using Postal API*/
        private void SendOrderOrderedEmail(int id)
        {
            Order order = db.Orders.Include("Student").Include("Student.UserProfile").FirstOrDefault(i => i.OrderID == id);
            Student student = order.Student;
            dynamic email = new Email("OrderOrdered");
            email.To = student.UserProfile.Email;
            email.FirstName = student.FirstName;
            email.LastName = student.LastName;
            email.VendorOrderNumber = order.VendorOrderNumber;
            email.Send();
        }

        /* Method to send email using Postal API*/
        public void SendOrderReadyToCollectEmail(int id)
        {
            Order order = db.Orders.Include("Student").Include("Student.UserProfile").FirstOrDefault(i => i.OrderID == id);
            Student student = order.Student;
            dynamic email = new Email("OrderReadyToCollect");
            email.To = student.UserProfile.Email;
            email.FirstName = student.FirstName;
            email.LastName = student.LastName;
            email.VendorOrderNumber = order.VendorOrderNumber;
            email.Send();
        }

        /* Method to create piechart for dashboard*/
        private TechnicianOrderOverview PieChartForOrdersOverview(TechnicianOrderOverview viewModel)
        {
            Highcharts pieChartOrders = new Highcharts("pieChartOrders")
                .InitChart(new Chart { PlotShadow = false, Width = 500, Height = 500 }) // set size parameters
                .SetTitle(new Title { Text = "Orders Overview" }) // set piechart title
                .SetPlotOptions(new PlotOptions // set the plotting options
                {
                    Pie = new PlotOptionsPie
                    {
                        AllowPointSelect = true,
                        Cursor = Cursors.Pointer,
                        DataLabels = new PlotOptionsPieDataLabels
                        {
                            Color = ColorTranslator.FromHtml("#000000"),
                            ConnectorColor = ColorTranslator.FromHtml("#000000"),
                            
                        }
                    }
                })
                .SetSeries(new Series // set series options
                {
                    Type = ChartTypes.Pie,
                    Name = "Overview",
                    Data = new Data(new object[]
                            {
                                  new DotNet.Highcharts.Options.Point
                                    {
                                        Name = "Orders Waiting To Be Ordered", // set title slice
                                        Y = (Double)viewModel.OrdersWaitingToBeOrdered,
                                        Selected=true,
                                        Color = Color.LightGreen
                                    },

                                 new DotNet.Highcharts.Options.Point
                                    {
                                        Name = "Orders Not Collected By Students", // set title slice
                                        Y = (Double)viewModel.OrdersNotCollected,
                                        Sliced = true,
                                        Color = Color.LightBlue,
                                        Selected = true
                                    },

                                new DotNet.Highcharts.Options.Point
                                    {
                                        Name = "Students Who Need To Be Notified", // set title of slice
                                        Y = (Double)viewModel.StudentsWhoNeedToBeNotified,
                                        Sliced = true,
                                        Color = Color.Orange,
                                        Selected = true
                                    },
                            })
                });


            viewModel.PieChart = pieChartOrders; // pass to viewmodel

            return viewModel;

        }

        /* Method to view approved orders that need to be ordered on Onecall*/
        [HttpGet]
        public ActionResult ViewApprovedOrders()
        {
            /* query to retrieve orders which are approved but not ordered on Onecall*/
            var orders = from o in db.Orders.Include("Components").Include("Student").Include("Student.Project.Supervisor")
                         where o.IsOrdered == false && o.IsApproved == true
                         select o;
            return View(orders);
        }

        /* Method to view components inside an order */
        public ActionResult ViewComponentDetails(int id)
        {
            Order order = db.Orders.Find(id);
            return View(order);
        }

        /* Method to get students which need to be notified about order collection */
        [HttpGet]
        public ActionResult ViewNotNotifiedOrderedOrders()
        {
            var orders = from o in db.Orders.Include("Components").Include("Student").Include("Student.Project.Supervisor")
                         where o.IsOrdered== true && o.StudentIsNotified == false
                         select o;

            return View(orders);

        }


        /* Method to deal with notifying students */
        [HttpPost]
        public ActionResult ViewNotNotifiedOrderedOrders(int id)
        {
            Order order = db.Orders.Find(id);

            if (order.StudentIsNotified != true) // if the student is not notified then send an email to notify
            {

                order.StudentIsNotified = true; // set status to true
                SendOrderReadyToCollectEmail(order.OrderID); // send the email
                db.Entry(order).State = EntityState.Modified; // save the order object changes
                db.SaveChanges(); 

                return RedirectToAction("ViewNotNotifiedOrderedOrders"); // refresh view
            }
            else
            {
                //send email method here
                SendOrderReadyToCollectEmail(order.OrderID); // send email
                return RedirectToAction("ViewNotCollectedOrders"); // send to orders not collected view
            }
        }

        /* method to send email to student to update on order status */
        [HttpPost]
        public ActionResult RequestOrdered(int id)
        {
            Order order = db.Orders.Find(id);
            order.OrderDate = DateTime.Now;
            order.IsOrdered = true;
            order.OrderedBy = User.Identity.Name;

            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            // send email to student once order has been ordered on Onecall
            SendOrderOrderedEmail(order.OrderID);

            return RedirectToAction("ViewApprovedOrders");
        }

        /* Method which retrieves orders which have not been collected despite being notified and sends to view */
        public ActionResult ViewNotCollectedOrders(string searchString)
        {
  
            var orders = from o in db.Orders.Include("Components").Include("Student")
                         where o.StudentIsNotified == true && o.IsCollected == false   // find the notified orders 
                         select o;

            // uses search string parameter to speed up finding the order
            if (!String.IsNullOrEmpty(searchString)) // if its not blank
            {
                try
                {
                    int vendorOrderNumber = Convert.ToInt32(searchString); // validation check to eliminate text entry
                    orders = orders.Where(o => o.VendorOrderNumber == vendorOrderNumber); // find the order with the order number

                }
                catch (Exception)
                {

                }
            }

            return View(orders);
        }

        // to be implemented but due to time constraints this cannot be done
        /*public ActionResult ViewCollectedOrders(string searchString)
        {
            var orders = from o in db.Orders.Include("Component").Include("Student").Include("Student.Project.Supervisor")
                         where o.IsCollected == false   // find the notified orders 
                         select o;

            
            orders = orders.Where(o => o.Student.FirstName.Contains(searchString) || o.Student.LastName.Contains(searchString));
            

            return View(orders);
        } */

        /* Method to get project spend for student*/ 
        public ActionResult ViewProjectSpendForStudent(string searchString)
        {
            // gets orders which had been approved
            var orders = from o in db.Orders.Include("Student").Include("Student.Userprofile")
                         where o.IsApproved==true  // find the notified orders 
                         select o;


                Order order = new Order(); // creates an order object
                orders = orders.Where(o => o.Student.UserProfile.UserName.Contains(searchString)); // searches orders related entities for the student username

                if (orders.Count() >= 1) // check if student is found
                {  
                    order = orders.FirstOrDefault();// get the order raised by student 
                    order.OrderTotal = orders.Sum(i => i.OrderTotal); // change order total value to sum of orders
                    return View(order); 
                }
                else // refresh view
                {
                    return View();
                }

            
        }
        // View all orders which had been ordered
        public ActionResult ViewOrderedOrders()
        {
            // gets orders which had been ordered
            var query = from o in db.Orders.Include("Components").Include("Student")
                        where o.IsOrdered == true
                        select o;


            OrdersDisplayView orders = new OrdersDisplayView(); // initalises viewmodel
            orders.Orders = query; // passes order object to view model object
            if (query.Count() > 0) // if more then one query exists the calculate grand total from all orders
            {
                orders.GrandTotal = query.Sum(i => i.OrderTotal);
            }
            return View(orders);
        }
        
        // method to update order status to collected
        [HttpPost]
        public ActionResult UpdateOrderAsCollected(int id)
        {
            Order order = db.Orders.Find(id);
            order.IsCollected = true;
            order.CollectionDate = DateTime.Now;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("ViewNotCollectedOrders"); 
        }

       
        
        /* clears memory allocated for db */
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
    


}
