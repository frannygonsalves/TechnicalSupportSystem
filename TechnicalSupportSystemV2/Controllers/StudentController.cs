using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechnicalSupportSystem.Models;
using TechnicalSupportSystemV2.DAL;
using TechnicalSupportSystemV2.ViewModels;
using Postal;
using System.Collections;
using TechnicalSupportSystemV2.Repository;
using TechnicalSupportSystemV2.Services;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using System.Drawing;

namespace TechnicalSupportSystemV2.Controllers
{
    public class StudentController : Controller
    {
       
        /* Private members */
        private IRepository _repository;
        private ITotalOrdersCostCalculator _calculator;
        private IOrderInformation _orderInformation;
        //
        // GET: /Student/

        /* Ninject Dependency resolver  */
        public StudentController(IRepository repository, ITotalOrdersCostCalculator calculator, IOrderInformation orderInformation)
        {
            this._repository = repository; // gets the service and passes it to object
            this._calculator = calculator;
            this._orderInformation = orderInformation;
        }

        /* Method which displays the menu to the student */
        public ActionResult Index() // Index Page
        {
            string username = User.Identity.Name; // Gets the current logged in user
            Student student = _repository.SingleOrDefault<Student>(s => s.UserProfile.UserName == username); // retrieves the user from the student repository
            return View(student); // passes the student object to the view
        }

        /* Method which retrieves the student details and displays the remaining budget*/
        [HttpGet]
        public ActionResult RemainingBudget()
        {
            string username = User.Identity.Name;

            Student student = _repository.Query<Student>().Include("Project").Include("Orders").SingleOrDefault(i => i.UserProfile.UserName == username); // retrieve user and load project and orders entities in advance
            var budget = student.Project.Budget;
            var totalOrdersCost = _calculator.TotalCost(student);
            var project = student.Project;

            if(student.Project==null) // If the studen't hasn't entered project information then redirect student to enter project details as without project details you can't view the remaining budget 
            {
                return RedirectToAction("RequestComponents");
            }

            ProjectSpendView viewModel = new ProjectSpendView(); // Initilise the viewmodel and pass relevant object information to view objects members 
            viewModel.Budget = budget;
            viewModel.TotalOrdersCost = totalOrdersCost;
            viewModel.SpendLeft = budget - totalOrdersCost; // Show budget if supervisor approved order
            viewModel.Project = project; 

            viewModel = PieChartForProjectSpendViewModel(viewModel); // create the piechart by passing current viewmodel to the piechart method

            return View(viewModel); // send the object to the view
        }


        /* Method which shows  project spend */ 
        [HttpGet]
        public ActionResult ProjectSpend()
        {
            string username = User.Identity.Name;
            Student student = _repository.Query<Student>().Include("Orders.Components").Include("Project").SingleOrDefault(i => i.UserProfile.UserName == username); // Eager loading used to retrieve student object
            var viewModel = new ProjectSpendView(); // Initilise the viewmodel and pass relevant object information to view objects members 
            
            if(student.Project==null) // collect project details if they are not saved
            {
                return RedirectToAction("RequestComponents");
            }

            viewModel.Orders = _orderInformation.GetApprovedOrders(student);
            viewModel.Budget = student.Project.Budget;

            viewModel.TotalOrdersCost = _orderInformation.GetTotalOrdersCost(student.Orders); // use sum function to calculate the total cost of all orders for student.orders object

            viewModel = BarChartForProjectSpendViewModel(viewModel); // // create the piechart by passing current viewmodel to the piechart method

           
            return View(viewModel);
        }

        
        /* Method which creates a bar chart from the provided viewmodel object information*/ 
        public ProjectSpendView BarChartForProjectSpendViewModel(ProjectSpendView viewModel)
        {
            List<Component> components = new List<Component>(); // creates list to store component objects

            /* Method which adds components within orders which are approved to the list*/
            foreach(Order o in viewModel.Orders)
            {

                if (o.IsApproved == true)
                {
                    components.AddRange(o.Components);
                }

            }

            var pricesList = new List<object>(); // a price list to store prices
            var categoryList = new List<string>(); // a category list to store categories

            /* Method to add prices of components to the pricelist and the names of components in categories */
            foreach (Component c in components)
            {
                pricesList.Add(new object[] { c.Price });
                categoryList.Add(c.Name);
            }

            object[] prices = pricesList.ToArray(); /* Creates array and passes information from prices list to array*/
            string[] categories = categoryList.ToArray(); /* Creates category array and passes categories list to array */


            Highcharts barChart = new Highcharts("barChart") //Initalises barchart
                .InitChart(new Chart { DefaultSeriesType = ChartTypes.Column, Margin = new[] { 50, 50, 100, 80 }, Width = 400, Height = 300 }) // Size parameters
                .SetTitle(new Title { Text = "Purchases" }) // sets the title of chart
                .SetXAxis(new XAxis // sets the x axis information 
                {
                    Categories = categories,
                    Labels = new XAxisLabels
                    {
                        Rotation = -45,
                        Align = HorizontalAligns.Right,
                        Style = "font: 'normal 13px Verdana, sans-serif'"
                    }
                })
                .SetYAxis(new YAxis // sets the y axis information 
                {
                    Min = 0,
                    Max = (double)viewModel.Budget + (double)viewModel.TotalOrdersCost,
                    Title = new YAxisTitle { Text = "Cost" }
                })
                .SetLegend(new Legend { Enabled = false }) 
                .SetSeries(new Series // sets series information 
                {
                    Name = "Price",
                    Data = new Data(prices),
                });

            viewModel.BarChart = barChart; // passes it to viewmodel object

            return viewModel; // returns a view model 

        }

        /* Method which creates a pie chart from the provided viewmodel object information*/ 
        public ProjectSpendView PieChartForProjectSpendViewModel(ProjectSpendView viewModel)
        {
            Highcharts pieChart = new Highcharts("pieChart")
                .InitChart(new Chart { PlotShadow = false, Width = 500, Height = 500 }) // Size parameters 
                .SetTitle(new Title { Text = "Budget" }) // sets the title of pie chart
                .SetPlotOptions(new PlotOptions // sets plot options
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
                .SetSeries(new Series // sets series information 
                {
                    Type = ChartTypes.Pie,
                    Name = "Budget",
                    Data = new Data(new object[]
                            {
                                  new DotNet.Highcharts.Options.Point
                                    {
                                        Name = "Budget",
                                        Y = (double)viewModel.Budget,
                                        Selected=true,
                                        Color = Color.LightGreen
                                    },

                                 new DotNet.Highcharts.Options.Point
                                    {
                                        Name = "Budget If Pending Orders Approved",
                                        Y = (double)viewModel.SpendLeft,
                                        Sliced = true,
                                        Color = Color.LightBlue,
                                        Selected = true
                                    },
                            })
                });


            viewModel.PieChart = pieChart; // passes it to view model object

            return viewModel; // returns viewmodel
        }

        // Method that retrieves component details from order and sends to view
        [HttpGet]
        public ActionResult ViewComponentDetails(int id)
        {
            Order order = _repository.Query<Order>().Include("Components").SingleOrDefault(i => i.OrderID == id);

            return View(order);
            
        }


        /* Place order method */
        [HttpGet]
        public ActionResult RequestComponents()
        {
            string username = User.Identity.Name; // get current logged in user

            Student student = _repository.SingleOrDefault<Student>(s => s.UserProfile.UserName == username);


            if (student.Project == null) // if project details don't exist then get the project details
            {
                return View();
            }
            else // otherwise go to save components methods to save component informaton and raise an order
            {
                return RedirectToAction("SaveComponents");
            }
        }

        /* A method to process project details form data from the request components view */ 
        [HttpPost]
        public ActionResult RequestComponents(Project projectDetails)
        {
            string username = User.Identity.Name; // get current logged in user
            Student student = _repository.SingleOrDefault<Student>(s => s.UserProfile.UserName == username);
            projectDetails.Supervisor.UserProfile.Email=projectDetails.Supervisor.UserProfile.Email.ToLower(); // function to lowercase email address and replace original

            //save the project details against the student
            if (ModelState.IsValid) // if everything on form ok
            {
                student.Project = projectDetails; // link project details to student object
                _repository.Insert(projectDetails); // save project details to db
                _repository.Update(student); // update student object to add link to project details
                _repository.Save();
            }

            SendSupervisorEmailAboutProject(student.StudentID); // send email to supervisor saying project registered method

            return RedirectToAction("SaveComponents"); // redirect to save components method
        }

        // send email to sueprvisor method
        public void SendSupervisorEmailAboutProject(int id)
        {
            Student student = _repository.Query<Student>().Include("Project").Include("Project.Supervisor").Include("Project.Supervisor.UserProfile").FirstOrDefault(i => i.StudentID == id);
            Supervisor supervisor = student.Project.Supervisor;
            Project project = student.Project;
            dynamic email = new Email("ProjectRegisteredSupervisorEmail");
            email.To = supervisor.UserProfile.Email;
            email.ProjectName = project.ProjectName;
            email.StudentFirstName = student.FirstName;
            email.StudentLastName = student.LastName;
            email.Send();
        }


        // Method to save component details
        [HttpGet]
        public ActionResult SaveComponents()
        {
            var order = new Order
            {
                Components = new[] {
            new Component { Name = "Tall Hat" , Description= "test", StockCode="ffff", Price = 10  },
            new Component {  Name = "Tall Hawt" , Description= "test2", StockCode="esgs", Price = 40}
            }
            }; // order object created with two components and sent to the view where it is then converted into json to create knockout model.



            

            return View(order);
        }


        /*Method that handles post data from the knockout form */
        [HttpPost]
        public ActionResult MakeOrder(Order order)
        {
            // Code to prevent double submission
            Order orderdb = _repository.SingleOrDefault<Order>(i => i.OrderID == order.OrderID);
            if (orderdb != null)
            {
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("OrderPlaced", new { id = order.OrderID });

                return Json(new { Url = redirectUrl });
            }

            string username = User.Identity.Name; // get current logged in user
            Student student = _repository.SingleOrDefault<Student>(s => s.UserProfile.UserName == username);


            decimal cost = order.Components.Sum(i => i.Price * i.Quantity); // sum function which gets the price and multiplies it with  quantity 
            decimal budget = student.Project.Budget;

            // fills in the other order information 
            order.Student = student;
            order.RequestDate = DateTime.Now;
            order.IsCollected = false;
            order.IsApproved = false;
            order.IsChecked = false;
            order.ComponentTotal = cost;
            order.OrderTotal = order.ComponentTotal + order.DeliveryCost; // computes the order total

            try
            {
                foreach (Component c in order.Components)  // each component in order.components
                {
                    c.Order = order; //link order to component.order
                    _repository.Insert<Component>(c); // save to repository
                }

                _repository.Insert<Order>(order); // save the order to respository
                _repository.Save();

                

                var redirectUrl = new UrlHelper(Request.RequestContext).Action("OrderPlaced", new { id = order.OrderID }); // once saved go to order placed view to see order information

                return Json(new { Url = redirectUrl });

            }
                // display view if something goes wrong
            catch (Exception)
            {
                return View("OrderNotPlaced");
            }

        }

        /* Dispays order that was just placed by retrieving order from db and passing to view */
        public ActionResult OrderPlaced(int id)
        {
            Order order = _repository.SingleOrDefault<Order>(i=>i.OrderID==id);
            return View(order);
            
        }

        /*/ Displays orders that were placed by student */
        [HttpGet]
        public ActionResult ViewOrders()
        {
            string username = User.Identity.Name; // get current logged in user
            Student student = _repository.SingleOrDefault<Student>(s => s.UserProfile.UserName == username);

            // lazy loading can't load related student, component and project entities in time therefore eager loading is used//
            var query = from order in _repository.Query<Order>()
                        where (order.Student.StudentID == student.StudentID)
                        select order;



            return View(query);
        }


        /* clears memory allocated*/
        protected override void Dispose(bool disposing)
        {
            _repository.Dispose();
            base.Dispose(disposing);
        }
    }
}