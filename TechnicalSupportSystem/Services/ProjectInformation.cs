using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechnicalSupportSystem.Models;
using TechnicalSupportSystem.Repository;

namespace TechnicalSupportSystem.Services
{
    public class ProjectInformation:IProjectInformation
    {
        private IRepository _repository = null;

        public ProjectInformation(IRepository repository)
        {
            _repository = repository;

        }

        public void UpdateProjectSupervisor(Supervisor supervisor)
        {
            // to be implemented
        }
    }
}