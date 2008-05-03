//===============================================================================
// Microsoft patterns & practices
// Composite WPF (PRISM)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism;
using Prism.Interfaces;
using System.Windows.Controls;
using Prism.Regions;
using System.Windows;

namespace StockTraderRI.Modules.WatchList.Tests
{
    public class MockRegionManagerService : IRegionManagerService
    {
        public string GetRegionArgumentRegionName;
        
        public MockRegion MockRegion = new MockRegion();


        #region IRegionManagerService Members

        public void Register(string regionName, IRegion region)
        {
            throw new NotImplementedException();
        }

        public IRegion GetRegion(string regionName)
        {
            GetRegionArgumentRegionName = regionName;
            return MockRegion;
        }

        public bool HasRegion(string regionName)
        {
            throw new NotImplementedException();
        }

        public void SetRegion(System.Windows.DependencyObject containerElement, string regionName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class MockRegion : IRegion
    {
        private IList<UIElement> views = new List<UIElement>();

        #region IRegion Members

        public void Add(UIElement view)
        {
            views.Add(view);
        }

        public void Remove(UIElement view)
        {
            throw new NotImplementedException();
        }

        public IList<UIElement> Views
        {
            get { return views; }
        }

        public void Show(UIElement view)
        {
            throw new NotImplementedException();
        }

        public void Add(UIElement view, string name)
        {
            throw new NotImplementedException();
        }

        public UIElement GetView(string name)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}