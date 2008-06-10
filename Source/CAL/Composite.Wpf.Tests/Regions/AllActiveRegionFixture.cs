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
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.Composite.Wpf.Regions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Composite.Wpf.Tests.Regions
{
    [TestClass]
    public class AllActiveRegionFixture
    {
        [TestMethod]
        public void AddingViewsToRegionMarksThemAsActive()
        {
            IRegion region = new AllActiveRegion();
            var view = new object();

            region.Add(view);

            Assert.IsTrue(region.ActiveViews.Contains(view));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeactivateThrows()
        {
            IRegion region = new AllActiveRegion();
            var view = new object();
            region.Add(view);

            region.Deactivate(view);
        }


    }
}