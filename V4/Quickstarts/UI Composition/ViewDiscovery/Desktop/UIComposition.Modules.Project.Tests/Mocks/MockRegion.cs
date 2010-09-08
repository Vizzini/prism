//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace UIComposition.Modules.Project.Tests.Mocks
{
    public class MockRegion : IRegion
    {
        public bool ActivateCalled;
        public int ViewsCount;
        public string NamedViewAdded;

        public string Name { get; set; }

        public IRegionManager Add(object view)
        {
            ViewsCount++;
            return null;
        }

        public void Remove(object view)
        {
            ViewsCount--;
        }

        public IViewsCollection Views
        {
            get { return null; }
        }

        public IViewsCollection ActiveViews
        {
            get { throw new System.NotImplementedException(); }
        }

        public object Context
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public void Activate(object view)
        {
            ActivateCalled = true;
        }

        public void Deactivate(object view)
        {
            throw new System.NotImplementedException();
        }

        public IRegionManager Add(object view, string name)
        {
            ViewsCount++;
            NamedViewAdded = name;
            return null;
        }

        public object GetView(string name)
        {
            if (NamedViewAdded == name)
                return new UserControl();

            return null;
        }

        public IRegionManager RegionManager { get; set; }
        public IRegionBehaviorCollection Behaviors
        {
            get { throw new System.NotImplementedException(); }
        }

        public IRegionManager Add(object view, string name, bool createRegionManagerScope)
        {
            ViewsCount++;
            NamedViewAdded = name;
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;



        public void RequestNavigate(System.Uri source, System.Action<NavigationResult> navigationCallback)
        {
            throw new System.NotImplementedException();
        }
    }
}
