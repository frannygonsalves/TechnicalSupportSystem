using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TechnicalSupportSystem.Models;
using TechnicalSupportSystem.Models;

namespace TechnicalSupportSystem.DAL
{
    public class SystemDBContext:DbContext
    {
        public DbSet<Technician> Technicians { get; set; }
        public DbSet<Student> Students {get;set;}
        public DbSet<Project> Projects { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}