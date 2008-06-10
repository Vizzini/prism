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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Interfaces;
using Prism.Regions;
using Prism.Tests.Mocks;
using System.Windows.Data;

namespace Prism.Tests.Regions
{
    [TestClass]
    public class ItemsControlRegionAdapterFixture
    {
        [TestMethod]
        public void AdapterAssociatesItemsControlWithRegion()
        {
            var control = new ItemsControl();
            IRegionAdapter adapter = new TestableItemsControlRegionAdapter();

            IRegion region = adapter.Initialize(control);
            Assert.IsNotNull(region);

            Assert.AreSame(control.ItemsSource, region.Views);
        }

        [TestMethod]
        public void ShouldMoveAlreadyExistingContentInControlToRegion()
        {
            var control = new ItemsControl();
            var view = new object();
            control.Items.Add(view);
            IRegionAdapter adapter = new TestableItemsControlRegionAdapter();

            var region = (NewMockRegion)adapter.Initialize(control);

            Assert.AreEqual(1, region.UnderlyingCollection.Count);
            Assert.AreSame(view, region.UnderlyingCollection[0]);
            Assert.AreSame(view, control.Items[0]);
        }

        [TestMethod]
        public void ControlsCurrentItemIsSynchronizedItemSource()
        {
            TabControl tabControl = new TabControl();

            IRegionAdapter adapter = new TestableItemsControlRegionAdapter();

            var region = (NewMockRegion)adapter.Initialize(tabControl);

            Assert.AreEqual(true, tabControl.IsSynchronizedWithCurrentItem);
        }

        [TestMethod]
        public void ControlWithExistingItemSourceThrows()
        {
            TabControl tabControl = new TabControl(){ItemsSource = new List<string>()};

            IRegionAdapter adapter = new TestableItemsControlRegionAdapter();

            try
            {
                var region = (NewMockRegion)adapter.Initialize(tabControl);
                Assert.Fail();   
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(InvalidOperationException));
                StringAssert.Contains(ex.Message, "ItemsControl's ItemsSource property must not be set when also used with Prism Regions.");
            }
        }

        [TestMethod]
        public void ControlWithExistingBindingOnItemsSourceWithNullValueThrows()
        {
            TabControl tabControl = new TabControl();
            Binding binding = new Binding("Enumerable");
            binding.Source = new SimpleModel() {Enumerable = null};
            BindingOperations.SetBinding(tabControl, ItemsControl.ItemsSourceProperty, binding);

            IRegionAdapter adapter = new TestableItemsControlRegionAdapter();

            try
            {
                var region = (NewMockRegion)adapter.Initialize(tabControl);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(InvalidOperationException));
                StringAssert.Contains(ex.Message, "ItemsControl's ItemsSource property must not be set when also used with Prism Regions.");
            }
        }

        class SimpleModel
        {
            public IEnumerable Enumerable { get; set; }
        }
	

    }

    internal class TestableItemsControlRegionAdapter : ItemsControlRegionAdapter
    {
        private NewMockRegion region = new NewMockRegion();

        public override IRegion CreateRegion()
        {
            return region;
        }
    }
}