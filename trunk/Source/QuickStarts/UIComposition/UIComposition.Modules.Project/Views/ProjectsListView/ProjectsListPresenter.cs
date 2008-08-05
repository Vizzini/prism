//===============================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation
//===============================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===============================================================================

namespace UIComposition.Modules.Project
{
    using UIComposition.Infrastructure;
    using UIComposition.Modules.Project.Services;

    public class ProjectsListPresenter : IProjectsListPresenter
    {
        private readonly IProjectService projectService;

        public ProjectsListPresenter(IProjectsListView view, IProjectService projectService)
        {
            this.View = view;
            this.projectService = projectService;
        }

        public IProjectsListView View { get; set; }

        public void SetProjects(int employeeId)
        {
            this.View.Model = new ProjectsListPresentationModel
                                  {
                                      Projects = this.projectService.RetrieveProjects(employeeId)
                                  };
        }
    }
}
