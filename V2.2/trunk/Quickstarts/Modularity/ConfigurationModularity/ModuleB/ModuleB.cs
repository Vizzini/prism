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
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Regions;

namespace ModuleB
{
    public class ModuleB : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly DefaultViewB _defaultViewB;

        public ModuleB(IRegionManager regionManager, DefaultViewB defaultViewB)
        {
            _defaultViewB = defaultViewB;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.Regions["MainRegion"].Add(_defaultViewB);
        }
    }
}
